using System;
using System.Threading.Tasks;
using ContainerOrchestrator.ControllerManager.Validator;

namespace ContainerOrchestrator.ControllerManager
{
    class Program
    {
        public const string SERVER_ADDRESS = "https://localhost:5002";

        static async Task Main(string[] args)
        {
            var deploymentValidator = new DeploymentValidatorService(SERVER_ADDRESS);
        }
    }
}