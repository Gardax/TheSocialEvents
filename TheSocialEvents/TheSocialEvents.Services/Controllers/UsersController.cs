using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TheSocialEvents.Data;
using TheSocialEvents.Models;
using TheSocialEvents.Services.Models;

namespace TheSocialEvents.Services.Controllers
{
    public class UsersController : ApiController
    {
        public const int MinNameLength = 6;
        public const int MaxNameLength = 30;

        private const string ValidNameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_. -";

        private const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";

        private static readonly Random rand = new Random();

        //private const int SessionKeyLength = 50;

        private const int Sha1Length = 40;

        [HttpPost]
        [ActionName("register")] //api/users/register
        public HttpResponseMessage PostRegisterUser(UserModel model)
        {
            try
            {
                var dbContext = new TheSocialEventsContext();
                using (dbContext)
                {
                    this.ValidateEmail(model.Email);
                    this.ValidateName(model.FullName);
                   

                    var user = dbContext.Users.FirstOrDefault(u => u.Email == model.Email);

                    if (user != null)
                    {
                        throw new InvalidOperationException("Users exists");
                    }

                    user = new User()
                    {
                        Email = model.Email,
                        FullName = model.FullName,
                        AuthCode = model.AuthCode,
                        PictureUrl = model.PictureUrl
                    };

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();

                    user.SessionKey = this.GenerateSessionKey(user.Id);
                    dbContext.SaveChanges();

                    var loggedModel = new LoggedUserModel()
                    {
                        FullName = user.FullName,
                        SessionKey = user.SessionKey
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.Created,
                                              loggedModel);
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest,
                                             ex.Message);
                return response;
            }
        }

        [HttpPost]
        [ActionName("login")]  //api/users/login
        public HttpResponseMessage PostLoginUser(UserModel model)
        {
            try
            {
                ValidateEmail(model.Email);
            

                var context = new TheSocialEventsContext();
                using (context)
                {
                    var user = context.Users.FirstOrDefault(u => u.Email == model.Email
                        && u.AuthCode == model.AuthCode);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid username or password");
                    }
                    if (user.SessionKey == null)
                    {
                        user.SessionKey = this.GenerateSessionKey(user.Id);
                        context.SaveChanges();
                    }

                    var loggedModel = new LoggedUserModel()
                    {
                        FullName = user.FullName,
                        SessionKey = user.SessionKey
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.Created,
                                        loggedModel);
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest,
                                         ex.Message);
                return response;
            }
        }

        [HttpPut]
        [ActionName("logout")]  //api/users/logout/{sessionKey}
        public HttpResponseMessage PutLogoutUser(string sessionKey)
        {
            try
            {
                var context = new TheSocialEventsContext();
                using (context)
                {
                    var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                    if (user == null)
                    {
                        throw new ArgumentException("Invalid user authentication.");
                    }

                    user.SessionKey = null;
                    context.SaveChanges();

                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        private void ValidateName(string nickname)
        {
            if (nickname == null)
            {
                throw new ArgumentNullException("Name cannot be null");
            }
            else if (nickname.Length < MinNameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Name must be at least {0} characters long",
                    MinNameLength));
            }
            else if (nickname.Length > MaxNameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Name must be less than {0} characters long",
                    MaxNameLength));
            }
            else if (nickname.Any(ch => !ValidNameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "Name must contain only Latin letters, digits .,_");
            }
        }

        private void ValidateEmail(string email)
        {
            new System.Net.Mail.MailAddress(email);
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }
            return skeyBuilder.ToString();
        }
    }
}