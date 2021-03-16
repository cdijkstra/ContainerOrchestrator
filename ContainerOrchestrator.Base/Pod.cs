using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerOrchestrator.Base
{
    public class Pod
    {
        public static int BasicCPURequest = 100;
        public static int BasicMemoryRequest = 100;

        public string Name { get; set; }

        public string Image { get; set; }

        public string[] Commands { get; set; }

        public Limitation Limit { get; set; }

        public Limitation Request { get; set; }

        public string NodeName { get; set; }

        public PodStatus Status { get; set; }

        public DateTime LastModified { get; set; }

        public Pod()
        {
            Limit = new Limitation();
            Request = new Limitation();
        }

        public override string ToString()
        {
            return $"{Name} with {Image} Limits: {Request.Memory}/{Limit.Memory} Memory {Request.CPU}/{Limit.CPU} CPU on '{NodeName}' status {Status.ToString()}";
        }
    }


}
