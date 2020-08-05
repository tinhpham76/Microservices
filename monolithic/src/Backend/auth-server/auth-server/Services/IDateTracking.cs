using System;

namespace auth_server.Services
{
    public interface IDateTracking
    {
        DateTime CreateDate { get; set; }
        DateTime? LastModifiedDate { get; set; }
    }
}
