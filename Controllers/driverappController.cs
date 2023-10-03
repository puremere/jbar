using jbar.Classes;
using jbar.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace jbar.Controllers
{
   
    public class driverappController : Controller
    {
        string  baseServer = "https://jbar.app/api/app";
        // GET: driverapp
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult setCode(string phone, string code)
        {
            string result = "";

            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("phone", phone);
                collection.Add("code", code);
                byte[] response = client.UploadValues(baseServer + "/Verify", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }
            responseModel model = JsonConvert.DeserializeObject<responseModel>(result);
            Response.Cookies["token"].Value = model.message;
            return View("Home");
        }
       
        public ActionResult getCode(string phone)
        {
            string result = "";

            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("phone", phone);
                collection.Add("userType", "1");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                byte[] response = client.UploadValues(baseServer + "/register", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }
            ViewBag.phone = phone;
            return View("Verify");
        }

        [HttpPost]
        public ActionResult setImage()
        {
            string name = "";
            for (int i = 0; i < this.Request.Files.Count; i++)
            {

                HttpPostedFileBase postedFile = Request.Files[0];
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                 name = methods.RandomString() +".png";
                postedFile.SaveAs(path + Path.GetFileName(name));
            }
            return Content(name);
        }
        [HttpPost]
        public ActionResult setProfile(setProfileVM model)
        {
            string result = "";

            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("hooshmandMashin", model.hooshmandMashin);
                collection.Add("cartNavgan", model.cartNavgan);
                collection.Add("profileImage", model.profileImage);


                byte[] response = client.UploadValues(baseServer + "/getCity", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
        }
        
        public ActionResult getCity(string ID , string search )
        {
            string result = "";
            
            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("ID", ID);
                collection.Add("search", search);

                byte[] response = client.UploadValues(baseServer + "/getCity", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
               
            }
            sendCityVM res = JsonConvert.DeserializeObject<sendCityVM>(result);
            if (ID == null && search == null)
            {
                return PartialView("/Views/Shared/_statePartial.cshtml", res);

            }
            else
            {
                return PartialView("/Views/Shared/_cityPartial.cshtml", res);

            }
        }

        public ActionResult getType(string ID)
        {
            string result = "";

            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("ID", ID);

                byte[] response = client.UploadValues(baseServer + "/getTypeFull", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            sendTypeVM res = JsonConvert.DeserializeObject<sendTypeVM>(result);
            return PartialView("/Views/Shared/_carTypePatial.cshtml", res);
        }
        public ActionResult getType2(string ID)
        {
            string result = "";
            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("ID", ID);

                byte[] response = client.UploadValues(baseServer + "/getTypeFull", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            sendTypeVM res = JsonConvert.DeserializeObject<sendTypeVM>(result);
            return PartialView("/Views/Shared/_carTypePatial2.cshtml", res);
        }

    }
}