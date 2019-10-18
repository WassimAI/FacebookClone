using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Account;
using FacebookClone.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FacebookClone.Controllers
{
    public class AccountController : Controller
    {
        // GET: /
        public ActionResult Index()
        {
            //Confirm user is not logged in
            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
            {
                return Redirect("~/" + username);
            }

            //return view
            return View();
        }

        // POST: /Account/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount(UserVM model, HttpPostedFileBase file)
        {
            //Init Db
            Db db = new Db();

            //Check Model state
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please make sure all the fields are entered and correct");
                return View("Index", model);
            }

            //Make sure username is unique
            if (db.Users.Any(x => x.Username == model.Username))
            {
                ModelState.AddModelError("", "Sorry, the username already exists");
                return View("Index", model);
            }

            //Create user Dto
            UserDTO user = new UserDTO();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Username = model.Username;
            user.EmailAddress = model.EmailAddress;
            user.Password = model.Password;

            //Add to DTO
            db.Users.Add(user);

            //Save DB
            db.SaveChanges();

            //Get inserted ID
            int id = user.Id;

            //Login user
            FormsAuthentication.SetAuthCookie(model.Username, false);

            //Set Upload folder
            var uploadDir = new DirectoryInfo(string.Format("{0}{1}", Server.MapPath(@"\"), "Uploads"));

            //Check if file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                //Get and verify extension
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/png" &&
                    ext != "image/gof" &&
                    ext != "image/x-png" &&
                    ext != "image/pjpeg")
                {
                    ModelState.AddModelError("", "The image was not uploaded, wrong image extension");
                    return View("Index", model);
                }

                //Set image name
                string imgName = user.Id + ".jpg";

                //Set Image path
                var path = string.Format("{0}\\{1}", uploadDir, imgName);

                //Save the image
                file.SaveAs(path);
            }

            //Add to wall
            WallDTO wall = new WallDTO()
            {
                Id= id,
                Message = "",
                DateEdited = DateTime.Now
            };

            db.Walls.Add(wall);
            db.SaveChanges();


            //Redirect
            return Redirect("~/" + model.Username);
        }

        // GET: /{Username}
        [Authorize]
        public ActionResult Username(string username = "")
        {
            Db db = new Db();

            //Check if user exists
            if (!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("~/");
            }

            ViewBag.username = username;

            //Get logged in user's username
            string user = User.Identity.Name;

            //Viewbag's user full name
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
            ViewBag.fullname = userDTO.FirstName + " " + userDTO.LastName;

            //Get viewing full name, the one signed in!
            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            ViewBag.viewingFullName = userDTO2.FirstName + " " + userDTO2.LastName;

            //Get username's image
            ViewBag.usernameimg = userDTO2.Id + ".jpg";

            //Check if user viewing is same as user logged in
            string userType = "guest";

            if (userDTO == userDTO2)
            {
                userType = "owner";
            }

            //Check if both users have pending friendship or not friends
            if (userType == "guest")
            {
                FriendsDTO f1 = db.Friends.Where(x => x.User1.Equals(userDTO.Id) && x.User2.Equals(userDTO2.Id)).FirstOrDefault();
                FriendsDTO f2 = db.Friends.Where(x => x.User2.Equals(userDTO.Id) && x.User1.Equals(userDTO2.Id)).FirstOrDefault();

                if(f1==null && f2 == null)
                {
                    ViewBag.notfriends = "true";
                }

                if (f1 != null)
                {
                    if (!f1.IsActive)
                    {
                        ViewBag.notfriends = "pending";
                    }
                }

                if (f2 != null)
                {
                    if (!f2.IsActive)
                    {
                        ViewBag.notfriends = "pending";
                    }
                }
            }

            //Get friend requests count - Viewbag
            var friendCount = db.Friends.Count(x => x.User2 == userDTO.Id && x.IsActive == false);

            if (friendCount > 0)
            {
                ViewBag.friendcount = friendCount;
            }

            //Get Friend Count - ViewBag
            int userId = userDTO.Id;

            var friendCount2 = db.Friends.Count(x => x.User1 == userId && x.IsActive == true || x.User2 == userId && x.IsActive == true);
            ViewBag.fCount = friendCount2;

            //View bag user ID
            ViewBag.userId = userId;

            ViewBag.usertype = userType;

            //Get Message Count
            var messageCount = db.Messages.Count(x => x.To == userId && x.Read == false);

            //Viewbag message count
            ViewBag.msgCount = messageCount;

            //Viewbag user wall
            WallDTO wall = new WallDTO();
            ViewBag.wallMessage = db.Walls.Where(x => x.Id == userId).Select(x => x.Message).FirstOrDefault();

            //View bag friend walls
            List<int> friendIds1 = db.Friends.Where(x => x.User1 == userId && x.IsActive == true)
                .ToArray().Select(x=>x.User2).ToList();

            List<int> friendIds2 = db.Friends.Where(x => x.User2 == userId && x.IsActive == true)
                .ToArray().Select(x => x.User1).ToList();

            List<int> allFriendIds = friendIds1.Concat(friendIds2).ToList();

            List<WallVM> allWalls = db.Walls.Where(x => allFriendIds.Contains(x.Id))
                .ToArray().OrderByDescending(x => x.DateEdited)
                .Select(x => new WallVM(x)).ToList();

            ViewBag.walls = allWalls;

            return View();
        }

        // GET: /Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("~/");
        }

        public ActionResult LoginPartial()
        {
            return PartialView();
        }

        //POST: Account/Login
        [HttpPost]
        public string Login(string username, string password)
        {
            //Init Db
            Db db = new Db();

            //Check if user exists
            if (db.Users.Any(x => x.Username.Equals(username) && x.Password.Equals(password)))
            {
                //Login
                FormsAuthentication.SetAuthCookie(username, false);
                return "ok";
            }

            return "problem";
        }

    }
}