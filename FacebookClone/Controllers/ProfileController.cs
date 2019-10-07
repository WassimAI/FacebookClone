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
    }
}