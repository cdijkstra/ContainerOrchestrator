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

        public DateTime LastHeartbeat { get; set; }

    }
}
