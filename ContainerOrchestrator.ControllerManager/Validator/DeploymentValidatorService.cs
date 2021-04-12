using System;
using System.Threading;
using System.Threading.Tasks;
using ContainerOrchestrator.Scheduler.ServiceHandlers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Validatorapi;
using AcceptableLimits = Validatorapi.AcceptableLimits;
using Deployment = Validator.Deployment;
using DeploymentResponse = Validator.DeploymentResponse;

namespace ContainerOrchestrator.ControllerManager.Validator
{
    public class DeploymentValidatorService : global::Validator.Validator.ValidatorBase
    {
        private ValidatorApi.ValidatorApiClient _client;

        public DeploymentValidatorService(string serverAddress)
        {
            _client = ConnectionHandler.GetClient(serverAddress);
        }

        public override async Task<DeploymentResponse> ValidateDeployment(Deployment request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("Validating");
                var acceptableLimits = await _client.GetAcceptableLimitsAsync(new Empty(), cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(500)).Token);
                // Min and max CPU/MEMORY 
                if (!CPULimitsAllowed(request, acceptableLimits))
                {
                    return new DeploymentResponse {Allowed = false, Deployment = request, Reason = "CPU limit"};
                }
                if (!MemoryLimitsAllowed(request, acceptableLimits))
                {
                    return new DeploymentResponse {Allowed = false, Deployment = request, Reason = "Memory limit"};
                }

                // Amount of replicas... too many?
                if (!ReplicaNumberAllowed(request, acceptableLimits.Replicas))
                {
                    return new DeploymentResponse {Allowed = false, Deployment = request, Reason = "Replica limit"};
                }
                
               // More checks..?

                return new DeploymentResponse {Allowed = true, Deployment = request};
                
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream timed out, stream cancelled.");
            }
            finally
            {
                await ConnectionHandler.CloseChannelAsync();
            }

            return null;
        }

        private bool CPULimitsAllowed(Deployment deployment, AcceptableLimits acceptableLimits)
        {
            if (deployment.Limit.CPU > acceptableLimits.Minima.CPU && deployment.Limit.CPU < acceptableLimits.Maxima.CPU)
            {
                return true;
            }
            return false;
        }

        private bool MemoryLimitsAllowed(Deployment deployment, AcceptableLimits acceptableLimits)
        {
            if (deployment.Limit.Memory > acceptableLimits.Minima.Memory && deployment.Limit.Memory < acceptableLimits.Maxima.Memory)
            {
                return true;
            }
            return false;
        }

        private bool ReplicaNumberAllowed(Deployment deployment, int allowedNumberOfReplicas)
        {
            return deployment.Replicas > allowedNumberOfReplicas;
        }
    }
}