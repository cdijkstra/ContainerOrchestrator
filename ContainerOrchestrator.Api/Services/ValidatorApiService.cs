using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Validatorapi;
using AcceptableLimits = Validatorapi.AcceptableLimits;
using Limitation = Validatorapi.Limitation;
using ValidatorClient = Validator.Validator.ValidatorClient;

// using ValidatorClient = Validator.Validator.ValidatorClient;

namespace ContainerOrchestrator.Api
{
    public class ValidatorApiService : ValidatorApi.ValidatorApiBase
    {
        private readonly ILogger<ValidatorApiService> _logger;
        private GrpcChannel _channel;
        private ValidatorClient _validatorClient;

        public ValidatorApiService(ILogger<ValidatorApiService> logger)
        {
            _logger = logger;
            _channel = GrpcChannel.ForAddress("https://localhost:5003");
            _validatorClient = new ValidatorClient(_channel);
        }

        // public override Task<DeploymentResponse> ValidateDeployment(Deployment request, ServerCallContext context)
        // {
        //     ToDo: Use base proto file for messages like Deployment/Pod et cetera
        //     Validator.Deployment deployment = new Validator.Deployment
        //     {
        //         Image = request.Image,
        //         LastModified = request.LastModified,
        //         Limit = request.Limit,
        //         Name = request.Name,
        //         Replicas = request.Replicas,
        //         Request = request.Request
        //     };
        //     _validatorClient.ValidateDeployment();
        // }

        public override Task<AcceptableLimits> GetAcceptableLimits(Empty request, ServerCallContext context)
        {
            // ToDo: query DB for acceptable limits
            AcceptableLimits acceptableLimits = new AcceptableLimits
            {
                Minima = new Limitation
                {
                    CPU = 2000,
                    Memory = 200
                },
                Maxima = new Limitation
                {
                    CPU = 50000,
                    Memory = 2000
                },
                Replicas = 100000
            };
        
            return Task.FromResult(acceptableLimits);
        }

        private async Task SendInvalidDummyDeploymentTooHighRequests()
        {
            var deployment = new Deployment
            {
                Name = "deployment" + Guid.NewGuid(),
                Replicas = 2,
                Image = "nginx",
                Commands = {""},
                Limit = new Limitation {CPU = 5000000, Memory = 20000},
                Request = new Limitation {CPU = 1000000, Memory = 10000},
                LastModified = Timestamp.FromDateTime(DateTime.Now)
            };
        }
        
        private async Task SendInvalidDummyDeploymentRequestHigherThanLimit()
        {
            var deployment = new Deployment
            {
                Name = "deployment" + Guid.NewGuid(),
                Replicas = 2,
                Image = "nginx",
                Commands = {""},
                Limit = new Limitation {CPU = 100, Memory = 100},
                Request = new Limitation {CPU = 5000, Memory = 200},
                LastModified = Timestamp.FromDateTime(DateTime.Now)
            };
        }
    }
}

