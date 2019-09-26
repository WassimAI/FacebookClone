using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Account;
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

        public ActionResult CreateAccount(UserVM model, HttpPostedFileBase file)
        {
            //Init Db
            Db db = new Db();

            //Check Model state
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please make sure all the fields are entered and correct");
                return View(model);
            }

            //Make sure username is unique
            if(db.Users.Any(x=>x.Username == model.Username))
            {
                ModelState.AddModelError("", "Sorry, the username already exists");
                return View(model);
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
            var uploadDir = new DirectoryInfo(string.Format("{0}Uploads", Server.MapPath(@"\")));

            //Check if file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                //Get and verify extension
                string ext = file.ContentType.ToLower();

                if (ext != "imgage/jpg" &&
                    ext != "imgage/jpeg" &&
                    ext != "imgage/png" &&
                    ext != "imgage/gof" &&
                    ext != "imgage/x-png" &&
                    ext != "imgage/pjpeg")
                {
                    ModelState.AddModelError("", "The image was not uploaded, wrong image extension");
                    return View(model);
                }

                //Set image name
                string imgName = user.Id + ".jpg";

                //Set Image path
                var path = string.Format("{0}\\{1}", uploadDir, imgName);

                //Save the image
                file.SaveAs(path);
            }

            //Redirect
            return Redirect("~/" + model.Username);
        }

        

    }
}