using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSocialEvents.Models
{
    public class MasterEvent
    {
         public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Country { get; set; }

        public string Town { get; set; }

        public string Address { get; set; }

        public User Creator { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } 

        public MasterEvent()
        {
            this.Users=new HashSet<User>();
            this.Comments=new HashSet<Comment>();
        }
    }
}
