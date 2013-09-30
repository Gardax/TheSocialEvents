using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Providers.Entities;
using TheSocialEvents.Data;
using TheSocialEvents.Models;
using TheSocialEvents.Models.Enumerations;
using TheSocialEvents.Services.Models;

namespace TheSocialEvents.Services.Controllers
{
    public class EventsController : ApiController
    {
        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage CreateEvent([FromBody]CreateEventModel newEvent, string sessionKey)
        {
            try
            {
                var context = new TheSocialEventsContext();


                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ArgumentException("Invalid authentication!");
                }

                var eventToAdd = new Event()
                                     {
                                         Name = newEvent.Name,
                                         Description = newEvent.Description,
                                         Address = newEvent.Address,
                                         Creator = user
                                     };
                context.Events.Add(eventToAdd);

                if (newEvent.InvitedUsersEmails != null)
                {
                    var invitedUsers = context.Users.Where(u => newEvent.InvitedUsersEmails.Contains(u.Email)).ToList();

                    if (invitedUsers != null)
                    {
                        foreach (var invitedUser in invitedUsers)
                        {
                            context.UsersEvents.Add(new UsersEvents()
                                                        {
                                                            User = invitedUser,
                                                            Event = eventToAdd,
                                                            State = State.NotAnswered
                                                        });
                        }
                    }
                }
                context.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.Created);
                return response;

            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest,
                                             ex.Message);
                return response;
            }
        }

        [HttpGet]
        [ActionName("GetAllEventsWithDetails")]
        public HttpResponseMessage GetAllEventsWithDetails(string sessionKey)
        {
            try
            {
                var context = new TheSocialEventsContext();

                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ArgumentException("Invalid authentication!");
                }

                var eventEntities = context.Events;

                var events = from singleEvent in eventEntities
                             select new EventModel()
                                        {
                                            Name = singleEvent.Name,
                                            Description = singleEvent.Description,
                                            Country = singleEvent.Country,
                                            Town = singleEvent.Town,
                                            Address = singleEvent.Address,
                                            Creator = singleEvent.Creator.FullName,
                                           
                                            Comments = from comment in singleEvent.Comments
                                                       select new CommentModel()
                                                                  {
                                                                      Text = comment.Text,
                                                                      UserName = comment.User.FullName
                                                                  },
                                            InvitedUsers = from invitesUsers in singleEvent.UsersEvents.Where(u=>u.State==State.NotAnswered)
                                                           select new ProfileUserModel()
                                                                      {
                                                                          FullName = invitesUsers.User.FullName,
                                                                          PictureUrl = invitesUsers.User.PictureUrl
                                                                      },
                                            Confirmed = from confirmedUsers in singleEvent.UsersEvents.Where(u => u.State == State.Confirmed)
                                                        select new ProfileUserModel()
                                                        {
                                                            FullName = confirmedUsers.User.FullName,
                                                            PictureUrl = confirmedUsers.User.PictureUrl
                                                        },
                                            Declined = from declinedUsers in singleEvent.UsersEvents.Where(u => u.State == State.Declined)
                                                        select new ProfileUserModel()
                                                        {
                                                            FullName = declinedUsers.User.FullName,
                                                            PictureUrl = declinedUsers.User.PictureUrl
                                                        }

                                        };
                var response = this.Request.CreateResponse(HttpStatusCode.OK, events);
                return response;
            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpGet]
        [ActionName("GetAllEvents")]
        public HttpResponseMessage GetAllEvents(string sessionKey)
        {
            try
            {
                var context = new TheSocialEventsContext();
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ArgumentException("Invalid authentication!");
                }

                var eventEntities = context.Events.Where(e=>e.UsersEvents
                    .Any(ue=>ue.UserId==user.Id)).OrderByDescending(e=>e.Id);

                var events = from singleEvent in eventEntities
                             select new EventModel()
                             {
                                 Id = singleEvent.Id,
                                 Name = singleEvent.Name,
                                 Creator = singleEvent.Creator.FullName,
                             };
                var response = this.Request.CreateResponse(HttpStatusCode.OK, events);
                return response;
            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpGet]
        [ActionName("GetSingleEvent")]
        public HttpResponseMessage GetSingleEvent(string sessionKey, int eventId)
        {
            try
            {
                var context = new TheSocialEventsContext();
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ArgumentException("Invalid authentication!");
                }

                var eventEntity= context.Events.FirstOrDefault(e=>e.Id==eventId);

                if(eventEntity==null)
                {
                    throw new ArgumentException("There is no such event!");
                }
                var singleEvent = new EventModel()
                                {
                                    Id = eventEntity.Id,
                                    Name = eventEntity.Name,
                                    Description = eventEntity.Description,
                                    Country = eventEntity.Country,
                                    Town = eventEntity.Town,
                                    Address = eventEntity.Address,
                                    Creator = eventEntity.Creator.FullName,
                                  
                                    Comments = from comment in eventEntity.Comments
                                               select new CommentModel()
                                               {
                                                   Text = comment.Text,
                                                   UserName = comment.User.FullName
                                               },
                                    InvitedUsers = from invitesUsers in eventEntity.UsersEvents.Where(u => u.State == State.NotAnswered)
                                                   select new ProfileUserModel()
                                                   {
                                                       FullName = invitesUsers.User.FullName,
                                                       PictureUrl = invitesUsers.User.PictureUrl
                                                   },
                                    Confirmed = from confirmedUsers in eventEntity.UsersEvents.Where(u => u.State == State.Confirmed)
                                                select new ProfileUserModel()
                                                {
                                                    FullName = confirmedUsers.User.FullName,
                                                    PictureUrl = confirmedUsers.User.PictureUrl
                                                },
                                    Declined = from declinedUsers in eventEntity.UsersEvents.Where(u => u.State == State.Declined)
                                               select new ProfileUserModel()
                                               {
                                                   FullName = declinedUsers.User.FullName,
                                                   PictureUrl = declinedUsers.User.PictureUrl
                                               }
                                };
                var response = this.Request.CreateResponse(HttpStatusCode.OK, singleEvent);
                return response;
            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [ActionName("ConfirmInvitationToEvent")]
        public HttpResponseMessage ConfirmInvitationToEvent(string sessionKey, int eventId)
        {
            try
            {
                var context = new TheSocialEventsContext();
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ArgumentException("Invalid authentication!");
                }

                var theEvent = context.UsersEvents.FirstOrDefault(e => e.EventId == eventId && e.UserId == user.Id);
                if(theEvent==null)
                {
                    throw new ArgumentException("You are not invited to this event!");
                }
                theEvent.State=State.Confirmed;
                context.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;

            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [ActionName("DeclineInvitationToEvent")]
        public HttpResponseMessage DeclineInvitationToEvent(string sessionKey, int eventId)
        {
            try
            {
                var context = new TheSocialEventsContext();
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ArgumentException("Invalid authentication!");
                }

                var theEvent = context.UsersEvents.FirstOrDefault(e => e.EventId == eventId && e.UserId == user.Id);
                if (theEvent == null)
                {
                    throw new ArgumentException("You are not invited to this event!");
                }
                theEvent.State = State.Declined;
                context.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;

            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        [ActionName("AddComment")]
        public HttpResponseMessage DeclineInvitationToEvent(string sessionKey, int eventId, CommentModel comment)
        {
            try
            {
                var context = new TheSocialEventsContext();
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ArgumentException("Invalid authentication!");
                }

                var theEvent = context.Events.FirstOrDefault(e => e.Id == eventId);
                if (theEvent == null)
                {
                    throw new ArgumentException("There is no such event!");
                }
                theEvent.Comments.Add(new Comment()
                                          {
                                              Text = comment.Text,
                                              User = user
                                          });
                context.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;

            }
            catch (Exception ex)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

    }
}
