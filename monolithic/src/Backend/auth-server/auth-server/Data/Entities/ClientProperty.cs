﻿namespace auth_server.Data.Entities
{
    public class ClientProperty : Property
    {
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
