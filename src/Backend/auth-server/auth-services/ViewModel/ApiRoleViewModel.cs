using System;
using System.Collections.Generic;
using System.Text;

namespace auth_services.ViewModel
{
    public class ApiRoleViewModel
    {
        public string Type { get; set; }
        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
