using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerOrchestrator.Base
{
    public class Pod
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string[] Commands { get; set; }

        public Limitation Limit { get; set; }

        public Limitation Request { get; set; }

        public string NodeName { get; set; }

        public PodStatus Status { get; set; }

        public DateTime LastModified { get; set; }
    }
}
