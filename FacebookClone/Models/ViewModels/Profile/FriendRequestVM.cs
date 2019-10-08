using FacebookClone.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class FriendRequestVM
    {
        public FriendRequestVM()
        {

        }

        public FriendRequestVM(FriendsDTO row)
        {
            User1 = row.User1;
            User2 = row.User2;
            IsActive = row.IsActive;
        }
        public int User1 { get; set; }
        public int User2 { get; set; }
        public bool IsActive { get; set; }
    }
}