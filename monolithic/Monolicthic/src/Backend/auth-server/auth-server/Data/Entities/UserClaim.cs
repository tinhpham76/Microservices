﻿namespace auth_server.Data.Entities
{
    public abstract class UserClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}
