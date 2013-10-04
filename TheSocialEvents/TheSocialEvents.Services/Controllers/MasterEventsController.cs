using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TheSocialEvents.Data;
using TheSocialEvents.Models;
using TheSocialEvents.Models.Enumerations;
using TheSocialEvents.Services.Models;

namespace TheSocialEvents.Services.Controllers
{
    public class MasterEventsController : ApiController
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

                var eventToAdd = new MasterEvent()
                {
                    Name = newEvent.Name,
                    Description = newEvent.Description,
                    Address = newEvent.Address,
                    Creator = user
                };
                context.MasterEvents.Add(eventToAdd);

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

                var eventEntities = context.MasterEvents;

                var events = from singleEvent in eventEntities
                             select new MasterEventModel()
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
                                 Users = from confirmedUsers in singleEvent.Users
                                             select new ProfileUserModel()
                                             {
                                                 FullName = confirmedUsers.FullName,
                                                 PictureUrl = confirmedUsers.PictureUrl
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

                var eventEntities = context.MasterEvents.OrderByDescending(e=>e.Id);

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

                var eventEntity = context.MasterEvents.FirstOrDefault(e => e.Id == eventId);

                if (eventEntity == null)
                {
                    throw new ArgumentException("There is no such event!");
                }

                var singleEvent = new MasterEventModel()
                {
                    Id = eventEntity.Id,
                    Name = eventEntity.Name,
                    Description = eventEntity.Description,
                    Country = eventEntity.Country,
                    Town = eventEntity.Town,
                    Address = eventEntity.Address,
                    Creator = eventEntity.Creator.FullName,
                    Comments = (eventEntity.Comments!=null)?from comment in eventEntity.Comments
                               select new CommentModel()
                               {
                                   Text = comment.Text,
                                   UserName = comment.User.FullName
                               } : new List<CommentModel>(),
                    Users = (eventEntity.Users!=null)?from confirmedUsers in eventEntity.Users
                                select new ProfileUserModel()
                                {
                                    FullName = confirmedUsers.FullName,
                                    PictureUrl = confirmedUsers.PictureUrl
                                }:new LinkedList<ProfileUserModel>()
                    //Comments = from comment in eventEntity.Comments
                    //           select new CommentModel()
                    //              {
                    //                 Text = comment.Text,
                    //                 UserName = comment.User.FullName
                    //              },
                    //Users = from confirmedUser in eventEntity.Users
                    //        select new ProfileUserModel()
                    //                   {
                    //                       FullName = confirmedUser.FullName,
                    //                       PictureUrl = confirmedUser.PictureUrl
                    //                   }

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

                var theEvent = context.MasterEvents.FirstOrDefault(e => e.Id == eventId);
                if(theEvent==null)
                {
                    throw new ArgumentException("There is no such event!");
                }
                theEvent.Users.Add(user);
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

                var theEvent = context.MasterEvents.FirstOrDefault(e => e.Id == eventId);
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
