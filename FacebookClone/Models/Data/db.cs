using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.Data
{
    public class Db : DbContext
    {
        public Db():base("Db")
        {

        }
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<FriendsDTO> Friends { get; set; }
        public DbSet<MessageDTO> Messages { get; set; }
        public DbSet<WallDTO> Walls { get; set; }
        public DbSet<OnlineDTO> Onlines { get; set; }

    }
}