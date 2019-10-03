﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;

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
    }
}