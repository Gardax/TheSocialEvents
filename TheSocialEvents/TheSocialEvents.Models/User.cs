using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSocialEvents.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        public string PictureUrl { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<User> Friends { get; set; }

        public virtual ICollection<UsersEvents> UsersEvents { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } 

        public User()
        {
            this.Events = new HashSet<Event>();
            this.Friends=new HashSet<User>();
            this.UsersEvents=new HashSet<UsersEvents>();
            this.Comments=new HashSet<Comment>();
        }
    }
}
