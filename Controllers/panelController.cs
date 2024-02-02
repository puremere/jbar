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
    public class panelController : Controller
    {
        string baseServer = "https://jbar.app/api/app";
        // GET: panel
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult setOrder()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return RedirectToAction("Login");
            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";

            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                byte[] response = client.UploadValues(baseServer + "/getCity", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            sendCityVM res = JsonConvert.DeserializeObject<sendCityVM>(result);


            using (WebClient client = new WebClient())
            {
                client.Headers.Set("Authorization", "Basic " + token);
                var collection = new NameValueCollection();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                byte[] response = client.UploadValues(baseServer + "/getLoadType", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
                
            }
            sendLoadTypeVM model = JsonConvert.DeserializeObject<sendLoadTypeVM>(result);
            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
               
                byte[] response = client.UploadValues(baseServer + "/getTypeFull", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            sendTypeVM typelist = JsonConvert.DeserializeObject<sendTypeVM>(result);
            panelSetOrder fmodel = new panelSetOrder()
            {
                cityList = res,
                loadList = model,
                typeList = typelist.lst.Where(x => x.parentID != x.typeID).ToList()

            };
            return View(fmodel);
        }

        public PartialViewResult getCityPartail(string name,string ID)
        {

            string result = "";

            using (WebClient client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("ID", ID);
               
                byte[] response = client.UploadValues(baseServer + "/getCity", collection);

                result = System.Text.Encoding.UTF8.GetString(response);

            }
            sendCityVM res = JsonConvert.DeserializeObject<sendCityVM>(result);
            panelCityVM model = new panelCityVM()
            {
                listname = name,
                cityList = res
            };

            return PartialView("/Views/Shared/panel/_cityPartial.cshtml", model);
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
        public ActionResult Orders()
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return RedirectToAction("Login");
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
                    return View(model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult OrderDetail(string ID)
        {
            if (Request.Cookies["clientToken"] == null)
            {
                return RedirectToAction("Login");

            }
            string token = Request.Cookies["clientToken"].Value;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Set("Authorization", "Basic " + token);
                    var collection = new NameValueCollection();
                    collection.Add("orderID", ID);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
                    byte[] response = client.UploadValues(baseServer + "/getOrderDetailClient", collection);

                    result = System.Text.Encoding.UTF8.GetString(response);
                    sendDetailVM model = JsonConvert.DeserializeObject<sendDetailVM>(result);
                    model.orderID = ID;
                    return View( model);
                }
            }
            catch (Exception e)
            {


                HttpCookie nameCookie = Request.Cookies["clientToken"];
                nameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(nameCookie);
                return RedirectToAction("Login");

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
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // .NET 4.0
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
            return RedirectToAction("orders");



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
    }
}