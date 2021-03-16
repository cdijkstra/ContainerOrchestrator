using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerOrchestrator.Base
{
    public class Node
    {
        public string IpAddress { get; set; }

        public string Name { get; set; }

        public int AllMemory { get; set; }

        public int AllCPU { get; set; }

        public int FreeMemory { get; set; }

        public int FreeCPU { get; set; }

        public DateTime LastHeartbeat { get; set; }

        public override string ToString()
        {
            return $"{Name} ({IpAddress}) - {FreeMemory}/{AllMemory} Memory - {FreeCPU}/{AllCPU} CPU - {LastHeartbeat}";
        }
    }
}
