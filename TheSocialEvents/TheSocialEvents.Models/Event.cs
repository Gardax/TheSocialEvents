using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSocialEvents.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Country { get; set; }

        public string Town { get; set; }

        public string Address { get; set; }

        public User Creator { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }

        public virtual ICollection<UsersEvents> UsersEvents { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } 

        public Event()
        {
            this.UsersEvents=new HashSet<UsersEvents>();
            this.Comments=new HashSet<Comment>();
        }
    }
}
