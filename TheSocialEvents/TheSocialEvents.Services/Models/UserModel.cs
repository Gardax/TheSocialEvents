using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheSocialEvents.Services.Models
{
    public class UserModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string PictureUrl { get; set; }

        public string AuthCode { get; set; }

        public string SessionKey { get; set; }
    }
}