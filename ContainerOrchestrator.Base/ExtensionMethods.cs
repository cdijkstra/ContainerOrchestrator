using Orcastrate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerOrchestrator.Base
{
    public static class ExtensionMethods
    {
        public static string PrettyPrint(this Pod pod)
        {
            return $"{pod.Name} with {pod.Image} Limits: {pod.Request.Memory}/{pod.Limit.Memory} Memory {pod.Request.CPU}/{pod.Limit.CPU} CPU on '{pod.NodeName}' status {pod.Status}";
        }
        public static string PrettyPrint(this Node node)
        {
            return $"{node.Name} ({node.IpAddress}) - {node.FreeMemory}/{node.AllMemory} Memory - {node.FreeCPU}/{node.AllCPU} CPU - {node.LastHeartbeat}";
        }
        
    }
}
