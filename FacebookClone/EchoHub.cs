using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using FacebookClone.Models.Data;
using System.Web.Mvc;

namespace FacebookClone
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello(string message="")
        {
            //Clients.All.hello();
            Trace.WriteLine(message);

            //set client
            var clients = Clients.All;

            //call js function
            clients.test("this is a test");

        }

        public void Notify(string friend)
        {
            //Init Db
            Db db = new Db();

            //Get friend Id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO.Id;

            //Get fr count
            var friendReqCount = db.Friends.Count(x => x.User2.Equals(friendId) && x.IsActive == false);

            //Set clients
            var clients = Clients.Others;

            //Call Js function
            clients.frnotify(friend, friendReqCount);
        }

        public void GetFrCount()
        {
            //Init Db
            Db db = new Db();

            //Get friend Id
            int userId = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault().Id;

            //Get Friend Count
            var friendReqCount = db.Friends.Count(x => x.User2.Equals(userId) && x.IsActive == false);

            //Set Clients
            var clients = Clients.Caller;

            //Call Js Function
            clients.frcount(Context.User.Identity.Name, friendReqCount);

        }

        public void GetFCount(int friendId)
        {
            //Init Db
            Db db = new Db();

            //Get friend Id
            int userId = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault().Id;

            //Friend Count for user (logged in)
            var friendCount1 = db.Friends.Count(x => x.User1 == userId && x.IsActive == true || x.User2 == userId && x.IsActive == true);

            //Get friend username
            string friendUsername = db.Users.Where(x => x.Id == friendId).FirstOrDefault().Username;

            //Friend Count for friend
            var friendCount2 = db.Friends.Count(x => x.User1 == friendId && x.IsActive == true || x.User2 == friendId && x.IsActive == true);

            //Set clients
            var clients = Clients.All;

            //Call Js Function
            clients.fCount(Context.User.Identity.Name, friendUsername, friendCount1, friendCount2);
        }

        public void NotifyOfMessage(string friend)
        {
            //Init Db
            Db db = new Db();

            //Get friend Id
            int friendId = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault().Id;

            //Get msg count
            var messageCount = db.Messages.Count(x => x.To == friendId && x.Read == false);

            //Set clients
            var clients = Clients.Others;//It means other clients than the guy who is using the web and made the call

            //Call Js Function
            clients.msgCount(friend, messageCount);
        }

        public void NotifyOfMessageOwner()
        {
            //Init Db
            Db db = new Db();

            //Get friend Id
            int userId = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault().Id;

            //Get msg count
            var messageCount = db.Messages.Count(x => x.To == userId && x.Read == false);

            //Set clients
            var clients = Clients.Caller;//Caller means the same one using the web and made the call

            //Call Js function
            clients.msgCount(Context.User.Identity.Name, messageCount);
        }
    }
}