using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheSocialEvents.Models;

namespace TheSocialEvents.Services.Models
{
    public class EventModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Description { get; set; }

        public string Town { get; set; }

        public string Address { get; set; }

        public string Creator { get; set; }

        public IEnumerable<ProfileUserModel> InvitedUsers { get; set; }

        public IEnumerable<ProfileUserModel> Confirmed { get; set; }

        public IEnumerable<ProfileUserModel> Declined { get; set; }

        public IEnumerable<UsersEvents> UsersEvents { get; set; }

        public IEnumerable<CommentModel> Comments { get; set; } 
    }
}