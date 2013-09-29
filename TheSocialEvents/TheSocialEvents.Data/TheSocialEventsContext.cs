using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSocialEvents.Models;

namespace TheSocialEvents.Data
{
    public class TheSocialEventsContext:DbContext
    {
        public TheSocialEventsContext()
            : base("TheSocialEventsDb")
        {
 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UsersEvents> UsersEvents { get; set; }
        public DbSet<MasterEvent> MasterEvents { get; set; }
    }
}
