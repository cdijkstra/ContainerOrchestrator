using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerOrchestrator.Base
{
    public enum PodStatus
    {
        Pending,
        Creating,
        Running,
        Failed,
        Terminating
    }
}
