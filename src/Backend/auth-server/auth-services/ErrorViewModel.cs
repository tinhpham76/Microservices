using System;
using System.Collections.Generic;
using System.Text;

namespace auth_services
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
