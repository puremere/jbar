using jbar.Classes;
using jbar.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace jbar.Controllers
{
   
    public class driverappController : Controller
    {
        string baseServer = "https://localhost:44389/api/app"; //"https://jbar.app/api/app";
        // GET: driverapp
        public ActionResult Index()
        {
            if (Request.Cookies["token"] != null)
            {
                return View("base");
            }
            else
            {
                return View();
            }
          
        }

        [HttpPost]
        public ActionResult setCode(string phone, string code)
        {
            string result = "";
            if (Request.Cookies["token"] == null)
            {
                using (WebClient client = new WebClient())
                {
                    var collection = new NameValueCollection();
                    collection.Add("phone", phone);
                    collection.Add("code", code);
                    collection.Add("userType", "1");
                    byte[] response = client.UploadValues(baseServer + "/Verify", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                }


                responseModel model = JsonConvert.DeserializeObject<responseModel>(result);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(model.message + ":" + phone);
                string tkn = System.Convert.ToBase64String(plainTextBytes);

                Response.Cookies["token"].Value = tkn;
            }
            
            if (Request.Cookies["firebase"] == null)
            {
                //ViewBag.fireBase ="1";
            }
            ViewBag.fireBase = "1";
            return View("Home");



        }

        [HttpPost]
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


        public async Task<ActionResult> DriverPartial()
        {
            if (Request.Cookies["token"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["token"].Value;
            string result = "";
            try
            {
                orderDetailVM mdl = new orderDetailVM();
                
                sendProfileVM responsemodel = new sendProfileVM();
                responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getProfile", Request.Cookies["clientToken"].Value);
                return PartialView("/Views/Shared/driver/_driverProfileParial.cshtml", responsemodel);
                //using (WebClient client = new WebClient())
                //{
                //    client.Headers.Set("Authorization", "Basic " + token );
                //    var collection = new NameValueCollection();
                //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                //    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                //    byte[] response = client.UploadValues(baseServer + "/getProfile", collection);


                //    result = System.Text.Encoding.UTF8.GetString(response);
                //    sendProfileVM model = JsonConvert.DeserializeObject<sendProfileVM>(result);
                   
                //}
            }
            catch (Exception e)
            {

               
                HttpCookie nameCookie = Request.Cookies["token"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
            
            
        }

        public ActionResult getHome()
        {
            return PartialView("/Views/Shared/driver/_driverHomePartial.cshtml");
        }

       [HttpPost]
       public async Task< ActionResult> requestOrder(requestOrderVM inputmodel)
        {
            if (Request.Cookies["token"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["token"].Value;
            string result = "";
            try
            {
                requestOrderVM mdl = new requestOrderVM();
                mdl.orderID = inputmodel.orderID;
                mdl.price = inputmodel.price;
               
                responseModel responsemodel = new responseModel();
                responsemodel = await methods.PostData(mdl, responsemodel, "/requestOrder", Request.Cookies["clientToken"].Value);
                return Content(responsemodel.status.ToString());


                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    collection.Add("orderID", inputmodel.orderID);
                    collection.Add("price", inputmodel.price.ToString());
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/requestOrder", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    responseModel model = JsonConvert.DeserializeObject<responseModel>(result);
                    return Content (model.status.ToString());
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["token"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
        }


        [HttpPost]
        public async Task<ActionResult> getOrderList(getOrderList model)
        {
            if (Request.Cookies["token"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["token"].Value;
            string result = "";
            try
            {
                setOrderVM mdl = new setOrderVM();
                mdl.originCityID = model.originCityID;
                mdl.destinCityID = model.destinCityID;
                mdl.type = model.type;
                getOrderVM responsemodel = new getOrderVM();
                responsemodel = await methods.PostData(mdl, responsemodel, "/getOrder", Request.Cookies["clientToken"].Value);
                return PartialView("/Views/Shared/driver/_driverOrderlistParial.cshtml", responsemodel);
                //using (WebClient client = new WebClient())
                //{
                //    client.Headers.Set("Authorization", "Basic " + token);
                //    var collection = new NameValueCollection();
                //    collection.Add("originCityID", originCityID);
                //    collection.Add("destinCityID", destinCityID);
                //    collection.Add("type", type);
                //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                //    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                //    byte[] response = client.UploadValues(baseServer + "/getOrder", collection);

                //    result = System.Text.Encoding.UTF8.GetString(response);
                //    getOrderVM model = JsonConvert.DeserializeObject<getOrderVM>(result);
                   
                //}
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["token"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
        }


        [HttpPost]
        public async Task<ActionResult> getOrderDetail(string orderID)
        {
            if (Request.Cookies["token"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["token"].Value;
            string result = "";
            try
            {
                orderDetailVM mdl = new orderDetailVM();
                mdl.orderID = orderID;
               
                sendDetailVM responsemodel = new sendDetailVM();
                responsemodel = await methods.PostData(mdl, responsemodel, "/getOrderDetail", Request.Cookies["clientToken"].Value);
                responsemodel.orderID = orderID;
                return PartialView("/Views/Shared/driver/_driverOrderDetailParial.cshtml", responsemodel);
                //using (WebClient client = new WebClient())
                //{
                //    client.Headers.Set("Authorization", "Basic " + token);
                //    var collection = new NameValueCollection();
                //    collection.Add("orderID", orderID);
                //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                //    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                //    byte[] response = client.UploadValues(baseServer + "/getOrderDetail", collection);

                //    result = System.Text.Encoding.UTF8.GetString(response);
                //    sendDetailVM model = JsonConvert.DeserializeObject<sendDetailVM>(result);
                //    model.orderID = orderID;
                //    return PartialView("/Views/Shared/driver/_driverOrderDetailParial.cshtml", model);
                //}
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["token"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
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

        
        public async  Task<ActionResult> getUserOrder()
        {
            if (Request.Cookies["token"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["token"].Value;
            string result = "";
            try
            {
                

                getOrderVM responsemodel = new getOrderVM();
                responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getUserOrder", Request.Cookies["clientToken"].Value);

                return PartialView("/Views/Shared/driver/_driverOrderlistParial.cshtml", responsemodel);
               
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["token"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
            
        }

        [HttpPost]
        public void updateLocation(getCityNameVM model)
        {
            string result = "";
            string token = Request.Cookies["token"].Value;
            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();

                collection.Add("lat", model.lat.ToString());
                collection.Add("lon", model.lon.ToString());
                byte[] response = client.UploadValues(baseServer + "/changeUserLocation", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
        }

        [HttpPost]
        public void setNewComment(setCommentVM model)
        {
            string result = "";
            string token = Request.Cookies["token"].Value;
            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();

                collection.Add("orderID", model.orderID);
                collection.Add("mark", model.mark);
                collection.Add("content", model.content);
                byte[] response = client.UploadValues(baseServer + "/addComment", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }
        }

        [HttpPost]
        public ActionResult setProfile(setProfileVM model)
        {
            string token = Request.Cookies["token"].Value;
            string result = "";

            if (!string.IsNullOrEmpty(model.firebaseToken))
            {
                Response.Cookies["firebase"].Value = model.firebaseToken;
            }

            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();
                
                collection.Add("firebaseToken", model.firebaseToken);
                collection.Add("hooshmandMashin", model.hooshmandMashin);
                collection.Add("cartDriver", model.cartDriver);
                collection.Add("cartNavgan", model.cartNavgan);
                collection.Add("profileImage", model.profileImage);
                collection.Add("cityID", model.cityID);
                collection.Add("typeID", model.typeID);
                collection.Add("pelakIran", model.pelakIran);
                collection.Add("pelakHarf", model.pelakHarf);
                collection.Add("pelak2", model.pelak2);
                collection.Add("pelak1", model.pelak1);
                collection.Add("name", model.name);
                collection.Add("emPhone", model.emPhone);
                

                byte[] response = client.UploadValues(baseServer + "/setProfile", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            return Content("200");
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