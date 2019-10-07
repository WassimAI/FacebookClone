using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using FacebookClone.Models.Data;

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
            var friendCount = db.Friends.Count(x => x.User2.Equals(friendId) && x.IsActive == false);

            //Set clients
            var clients = Clients.Others;

            //Call Js function
            clients.frnotify(friend, friendCount);
        }
    }
}