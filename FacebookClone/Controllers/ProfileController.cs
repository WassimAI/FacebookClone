using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacebookClone.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }

        //POST: Profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {
            //init Db
            Db db = new Db();

            //Create List
            List<LiveSeachUserVm> usernames = db.Users.Where(x => x.Username.Contains(searchVal) && x.Username != User.Identity.Name).ToArray().Select(x => new LiveSeachUserVm(x)).ToList();

            //Return Json, the function below will actually convert the object to json!
            return Json(usernames);
        }

        //POST: Profile/AddFriend
        [HttpPost]
        public void AddFriend(string friend)
        {
            //Init Db
            Db db = new Db();

            //Get user (logged in) ID
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            //Get friend ID
            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO2.Id;

            //Add Dto
            FriendsDTO friendDTO = new FriendsDTO()
            {
                User1 = userId,
                User2 = friendId,
                IsActive = false
            };

            db.Friends.Add(friendDTO);
            db.SaveChanges();

        }

        //POST: Profile/DisplayFriendRequest
        [HttpPost]
        public JsonResult DisplayFriendRequest()
        {
            //inti db
            Db db = new Db();

            //Get user Id
            int UserId = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault().Id;

            //Get friends list
            List<FriendRequestVM> list = new List<FriendRequestVM>();
            list = db.Friends.Where(x => x.User2.Equals(UserId) && x.IsActive==false).ToArray().Select(x => new FriendRequestVM(x)).ToList();

            //Init list of users
            List<UserDTO> usersList = new List<UserDTO>();
            foreach(var friend in list)
            {
                usersList.Add(db.Users.Where(x => x.Id == friend.User1).FirstOrDefault());
            }

            //Return Json
            return Json(usersList);
        }

        //POST: Profile/AcceptFriendRequest
        [HttpPost]
        public void AcceptFriendRequest(int friendId)
        {
            //Init Db
            Db db = new Db();

            //Get UserId
            int userId = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault().Id;

            //Make Friends
            FriendsDTO friendDTO = db.Friends.Where(x => x.User1.Equals(friendId) && x.User2.Equals(userId) && x.IsActive == false).FirstOrDefault();
            friendDTO.IsActive = true;
            db.SaveChanges();
        }

        //POST: Profile/DeclineFriendRequest
        [HttpPost]
        public void DeclineFriendRequest(int friendId)
        {
            //Init Db
            Db db = new Db();

            //Get UserId
            int userId = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault().Id;

            //Remove Friends
            FriendsDTO friendDTO = db.Friends.Where(x => x.User1.Equals(friendId) && x.User2.Equals(userId)).FirstOrDefault();
            db.Friends.Remove(friendDTO);
            db.SaveChanges();
        }

        //POST: Profile/SendMessage
        [HttpPost]
        public void SendMessage(string friend, string message)
        {
            //init db
            Db db = new Db();

            //Get userId
            int userId = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault().Id;

            //Get friendId
            int friendId = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault().Id;

            //Save the message
            MessageDTO messageDTO = new MessageDTO()
            {
                From = userId,
                To = friendId,
                DateSent = DateTime.Now,
                Read = false,
                Message = message
            };

            db.Messages.Add(messageDTO);
            db.SaveChanges();
        }

        //POST: Profile/DisplayUnreadMessages
        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {
            //init db
            Db db = new Db();

            //Get userId
            int userId = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault().Id;

            //Create list of unread messages
            List<MessageVM> messages = new List<MessageVM>();

            messages = db.Messages.Where(x=>x.To==userId && x.Read==false).ToArray().Select(x => new MessageVM(x)).ToList();

            //Make message read
            db.Messages.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);
            db.SaveChanges();

            //Return JSON
            return Json(messages);
        }
    }
}