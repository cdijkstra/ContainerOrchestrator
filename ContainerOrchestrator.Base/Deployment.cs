using System;

namespace ContainerOrchestrator.Base
{
    public class Deployment
    {
        public string Name { get; set; }

        public int Replicas { get; set; }

        public string Image { get; set; }

        public string[] Commands { get; set; }

        public Limitation Limit { get; set; }

        public Limitation Request { get; set; }

        public DateTime LastModified { get; set; }

    }
}
