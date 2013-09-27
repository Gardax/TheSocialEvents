using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TheSocialEvents.Models;

namespace TheSocialEvents.Services.Models
{
    [DataContract]
    public class CreateEventModel
    {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Town { get; set; }

        [DataMember]
        public string Address { get;set; }

        [DataMember]
        public User Creator { get; set; }

        [DataMember]
        public DateTime TimeFrom { get; set; }

        [DataMember]
        public DateTime TimeTo { get; set; }

        [DataMember]
        public IEnumerable<string> InvitedUsersEmails { get; set; } 
    }
}