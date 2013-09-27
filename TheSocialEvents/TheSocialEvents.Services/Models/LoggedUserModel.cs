using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheSocialEvents.Services.Models
{
    public class LoggedUserModel
    {
        public string FullName { get; set; }

        public string SessionKey { get; set; }
    }
}