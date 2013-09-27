using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSocialEvents.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public Event Event { get; set; }

        public User User { get; set; }
    }
}
