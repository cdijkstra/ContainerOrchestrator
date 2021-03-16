using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerOrchestrator.Base
{
    public class GenericResponse
    {
        public bool Successful { get; set; }

        public string ErrorMessage { get; set; }
    }
}
