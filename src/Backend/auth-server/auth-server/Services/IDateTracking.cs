using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auth_server.Services
{
    public interface IDateTracking
    {
        DateTime CreateDate { get; set; }
        DateTime? LastModifiedDate { get; set; }
    }
}
