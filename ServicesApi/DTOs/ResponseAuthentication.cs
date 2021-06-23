namespace ServicesApi.DTOs
{
    using System;

    public class ResponseAuthentication
    {
        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}