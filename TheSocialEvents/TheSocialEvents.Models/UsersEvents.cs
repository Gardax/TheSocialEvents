using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSocialEvents.Models.Enumerations;

namespace TheSocialEvents.Models
{
    public class UsersEvents
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public int EventId { get; set; }

        public virtual Event Event { get; set; }

        public virtual User User { get; set; }

        public State State { get; set; }
    }
}
