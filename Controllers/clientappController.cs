using jbar.Classes;
using jbar.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace jbar.Controllers
{
   
    public class clientappController : Controller
    {
        string baseServer = "https://jbar.app/api/app";
        // GET: clientapp
        public ActionResult Index()
        {
            if (Request.Cookies["clientToken"] != null)
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
            if (Request.Cookies["clientToken"] == null)
            {
                using (WebClient client = new WebClient())
                {
                    var collection = new NameValueCollection();
                    collection.Add("phone", phone);
                    collection.Add("code", code);
                    collection.Add("userType", "0");
                    byte[] response = client.UploadValues(baseServer + "/Verify", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                }


                responseModel model = JsonConvert.DeserializeObject<responseModel>(result);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(model.message + ":" + phone);
                string tkn = System.Convert.ToBase64String(plainTextBytes);

                Response.Cookies["clientToken"].Value = tkn;
            }

            if (Request.Cookies["firebase"] == null)
            {
                //ViewBag.fireBase ="1";
            }
            ViewBag.fireBase = "1";
            return View("Main");



        }

        [HttpPost]
        public ActionResult getCode(string phone)
        {
            string result = "";

            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("phone", phone);
                collection.Add("userType", "0");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                byte[] response = client.UploadValues(baseServer + "/register", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }
            ViewBag.phone = phone;
            return View("Verify");
        }


        public ActionResult DriverPartial()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getProfile", collection);


                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendProfileVM model = JsonConvert.DeserializeObject<sendProfileVM>(result);
                    return PartialView("/Views/Shared/client/_clientProfileParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }


        }

        public ActionResult personPartial()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getProfile", collection);


                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendProfileVM model = JsonConvert.DeserializeObject<sendProfileVM>(result);
                    return PartialView("/Views/Shared/client/_clientProfilePersonParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }


        }

        public ActionResult companyPartial()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getProfile", collection);


                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendProfileVM model = JsonConvert.DeserializeObject<sendProfileVM>(result);
                    return PartialView("/Views/Shared/client/_clientProfileCompanyParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }


        }
        public ActionResult loggiginPartial()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getProfile", collection);


                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendProfileVM model = JsonConvert.DeserializeObject<sendProfileVM>(result);
                    return PartialView("/Views/Shared/client/_clientProfileBarbariParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }


        }

        public async Task<ActionResult> getHomeAsync()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {

               

                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getOrderClient", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    getOrderVM model = JsonConvert.DeserializeObject<getOrderVM>(result);
                    return PartialView("/Views/Shared/client/_clientHomePartial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }

        }
        public ActionResult getHome()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getOrderClient", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    getOrderVM model = JsonConvert.DeserializeObject<getOrderVM>(result);
                    return PartialView("/Views/Shared/client/_clientHomePartial.cshtml",model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
           
        }

        [HttpPost]
        public ActionResult setNewOrder(setOrderclientVM input)
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    collection.Add("date", input.date);
                    collection.Add("description", input.description);
                    collection.Add("destinCityID", input.destinCityID);
                    collection.Add("loadAmount", input.loadAmount.ToString());
                    collection.Add("loadingFinishHour", input.loadingFinishHour);
                    collection.Add("loadingStartHour", input.loadingStartHour);
                    collection.Add("loadTypeID", input.loadTypeID);
                    collection.Add("originCityID", input.originCityID);
                    collection.Add("pricePerTone", input.pricePerTone.ToString().Replace(",", ""));
                    collection.Add("priceTotal", input.priceTotal.ToString().Replace(",",""));
                    collection.Add("recieveSMS", input.recieveSMS);
                    collection.Add("showPhone", input.showPhone);
                    collection.Add("type", input.type);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/setOrder", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    responseModel responseModel = JsonConvert.DeserializeObject<responseModel>(result);
                    return Content(responseModel.status.ToString());
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
        }

       [HttpPost]
        public ActionResult getLoggigList()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getLoadType", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendLoadTypeVM model = JsonConvert.DeserializeObject<sendLoadTypeVM>(result);
                    return PartialView("/Views/Shared/client/_clientLoggiglistParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
        }
        [HttpPost]
        public ActionResult getOrderForm()
        {
            return PartialView("/Views/Shared/client/_clientSetOrderParial.cshtml");
        }

        [HttpPost]
        public ActionResult getOrderList(string originCityID, string destinCityID, string type)
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    collection.Add("originCityID", originCityID);
                    collection.Add("destinCityID", destinCityID);
                    collection.Add("type", type);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getOrder", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    getOrderVM model = JsonConvert.DeserializeObject<getOrderVM>(result);
                    return PartialView("/Views/Shared/client/_clientOrderlistParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
        }


        [HttpPost]
        public async Task<ActionResult> getOrderDetailAsync(string orderID)
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseServer + "/getOrderDetailClientAsync");
                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
                    orderDetailVM mdl = new orderDetailVM();
                    mdl.orderID = orderID;
                    string content = JsonConvert.SerializeObject(mdl);

                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var rsp =await client.PostAsync("", byteContent).ConfigureAwait(false);
                    sendDetailVM model = JsonConvert.DeserializeObject<sendDetailVM>(rsp.ToString());
                    model.orderID = orderID;
                    return PartialView("/Views/Shared/client/_cleintOrderDetailParial.cshtml", model);

                }
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    collection.Add("orderID", orderID);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getOrderDetailClient", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendDetailVM model = JsonConvert.DeserializeObject<sendDetailVM>(result);
                    model.orderID = orderID;
                    return PartialView("/Views/Shared/client/_cleintOrderDetailParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
        }

        [HttpPost]
        public ActionResult getOrderDetail(string orderID)
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    collection.Add("orderID", orderID);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getOrderDetailClient", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendDetailVM model = JsonConvert.DeserializeObject<sendDetailVM>(result);
                    model.orderID = orderID;
                    return PartialView("/Views/Shared/client/_cleintOrderDetailParial.cshtml", model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
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
                name = methods.RandomString() + ".png";
                postedFile.SaveAs(path + Path.GetFileName(name));
            }
            return Content(name);
        }

        [HttpPost]
        public void updateLocation(getCityNameVM model)
        {
            string result = "";
            string token = Request.Cookies["clientToken"].Value;
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
        public ActionResult setProfile(setProfileVM model)
        {
            string token = Request.Cookies["clientToken"].Value;
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
                collection.Add("cartdriver", model.cartDriver);
                collection.Add("cartNavgan", model.cartNavgan);
                collection.Add("cityID", model.cityID);
                collection.Add("typeID", model.typeID);
                collection.Add("profileImage", model.profileImage);
                collection.Add("pelakIran", model.pelakIran);
                collection.Add("pelakHarf", model.pelakHarf);
                collection.Add("pelak2", model.pelak2);
                collection.Add("pelak1", model.pelak1);
                collection.Add("name", model.name);
                collection.Add("username", model.username);
                collection.Add("emPhone", model.emPhone);


                byte[] response = client.UploadValues(baseServer + "/setProfile", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            return Content("200");
        }

        [HttpPost]
        public ActionResult setProfilePerson(setProfileVM model)
        {
            string token = Request.Cookies["clientToken"].Value;
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
                collection.Add("cartMelliModir", model.cartMelliModir);
                collection.Add("RPT", model.RPT);
                collection.Add("cityID", model.cityID);
                collection.Add("profileImage", model.profileImage);
                collection.Add("postalCode", model.postalCode);
                collection.Add("name", model.name);
                collection.Add("address", model.address);
                collection.Add("clientType", model.clientType);
                collection.Add("codeMelli", model.codeMelli);


                byte[] response = client.UploadValues(baseServer + "/setProfile", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            return Content("200");
        }

        [HttpPost]
        public ActionResult setProfileCompany(setProfileVM model)
        {
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();

                collection.Add("firebaseToken", model.firebaseToken);
                collection.Add("cartMelliModir", model.cartMelliModir);
                collection.Add("RPT", model.RPT);
                collection.Add("cityID", model.cityID);
                collection.Add("profileImage", model.profileImage);
                collection.Add("postalCode", model.postalCode);
                collection.Add("name", model.name);
                collection.Add("coName", model.coName);
                collection.Add("emPhone", model.emPhone);
                collection.Add("address", model.address);
                collection.Add("codeMelli", model.codeMelli);
                collection.Add("shenaseSherkat", model.shenaseSherkat);
                collection.Add("clientType", model.clientType);
                

                byte[] response = client.UploadValues(baseServer + "/setProfile", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            return Content("200");
        }

        [HttpPost]
        public ActionResult setProfileLoggiging(setProfileVM model)
        {
            string token = Request.Cookies["clientToken"].Value;
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
                collection.Add("cartMelliModir", model.cartMelliModir);
                collection.Add("RPT", model.RPT);
                collection.Add("coName", model.coName);
                collection.Add("emPhone", model.emPhone);
                collection.Add("cityID", model.cityID);
                collection.Add("profileImage", model.profileImage);
                collection.Add("postalCode", model.postalCode);
                collection.Add("name", model.name);
                collection.Add("address", model.address);
                collection.Add("codeMelli", model.codeMelli);
                collection.Add("shenaseSherkat", model.shenaseSherkat);
                collection.Add("clientType", model.clientType);


                byte[] response = client.UploadValues(baseServer + "/setProfile", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            return Content("200");
        }


        [HttpPost]
        public ActionResult getAuthentication()
        {
            string token = Request.Cookies["clientToken"].Value;
            string result = "";

            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();
                byte[] response = client.UploadValues(baseServer + "/getUserStatus", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            sendUserStatusVM rmodel = JsonConvert.DeserializeObject<sendUserStatusVM>(result);
            return PartialView("/Views/Shared/Client/_userStatusPartial.cshtml", rmodel);
        }

        [HttpPost]
        public ActionResult addCoDriver(string phone)
        {
            string token = Request.Cookies["clientToken"].Value;
            string result = "";

            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();
                collection.Add("phone",phone);
                byte[] response = client.UploadValues(baseServer + "/addDriver", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            sendUserStatusVM rmodel = JsonConvert.DeserializeObject<sendUserStatusVM>(result);
            return Content(rmodel.status.ToString());
        }

        [HttpPost]
        public ActionResult verifyOrderrResponse(responseOrderVM inputmodel)
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return Content("400");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    collection.Add("orderID", inputmodel.orderID);
                    collection.Add("driverID", inputmodel.driverID);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/verifyOrderrResponse", collection);

                    return Content("");
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return Content("400");
            }
        }

        public ActionResult getCoDriver()
        {
            string token = Request.Cookies["clientToken"].Value;
            string result = "";

            

            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();
                byte[] response = client.UploadValues(baseServer + "/getCoDriver", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            getCoDriverResponse rmodel = JsonConvert.DeserializeObject<getCoDriverResponse>(result);
            return PartialView("/Views/Shared/client/_coDriverListPartial.cshtml",rmodel);
        }


        public ActionResult getCity(string ID, string search)
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