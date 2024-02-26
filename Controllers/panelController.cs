using jbar.Classes;
using jbar.Model;
using jbar.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using jbar.Classes;



namespace jbar.Controllers
{
    [panelCheck]
    [doForAll]
    public class panelController : Controller
    {
        string baseServer = "https://localhost:44389/api/app";
        // GET: panel
        public ActionResult Login()
        {
            Context dbcontext = new Context();
            //List<vehicleStatus> stl = dbcontext.vehicleStatuses.ToList();
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [mabnas]");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [namads]"); 
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [codings]");
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [formulas]");
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [processes]"); 
             dbcontext.SaveChanges();






            //dbcontext.SaveChanges();
            //yadakStatus yd = new yadakStatus()
            //{
            //    title = "بدون کشنده",
            //    yadakStatusID = Guid.NewGuid(),
            //};
            //dbcontext.yadakStatuses.Add(yd);
            //dbcontext.SaveChanges();
            //yd = new yadakStatus()
            //{
            //    title = "عملیاتی",
            //    yadakStatusID = Guid.NewGuid(),
            //};
            //dbcontext.yadakStatuses.Add(yd);
            //dbcontext.SaveChanges();
            //yd = new yadakStatus()
            //{
            //    title = "در دست تعمیرات",
            //    yadakStatusID = Guid.NewGuid(),
            //};
            //dbcontext.yadakStatuses.Add(yd);
            //dbcontext.SaveChanges();
            Guid userid = new Guid("d7ddbb1d-5bac-40e0-a3f2-64d06712fba6");

            mabna mabna = new mabna()
            {
                mabnaID = Guid.NewGuid(),
                title = "قیمت تمام شده سفارش",
                value = "1",
                 userID = userid,
                 
            };
            dbcontext.mabnas.Add(mabna);
            dbcontext.SaveChanges();
            mabna = new mabna()
            {
                mabnaID = Guid.NewGuid(),
                title = "ارزش بار",
                value = "2",
                userID = userid,
            };
            dbcontext.mabnas.Add(mabna);
            dbcontext.SaveChanges();
            mabna = new mabna()
            {
                mabnaID = Guid.NewGuid(),
                title = "مسافت کلی بار",
                value = "3",
                userID = userid,
            };
            dbcontext.mabnas.Add(mabna);
            dbcontext.SaveChanges();
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [formulas]");
            namad formula = new namad()
            {

                namadID = Guid.NewGuid(),
                value = "&#43;",
                title = "plus",
                userID = userid,
               
            };
            dbcontext.namads.Add(formula);
            dbcontext.SaveChanges();
            formula = new namad()
            {

                namadID = Guid.NewGuid(),
                value = "&#8722;",
                title = "minus",
                userID = userid,
            };
            dbcontext.namads.Add(formula);
            dbcontext.SaveChanges();

            formula = new namad()
            {

                namadID = Guid.NewGuid(),
                value = "&#215;",
                title ="multiple",
                userID = userid,
            };
            dbcontext.namads.Add(formula);
            dbcontext.SaveChanges();
            formula = new namad()
            {

                namadID = Guid.NewGuid(),
                value = "&#8260;",
                title = "fraction",
                userID = userid,
            };
            dbcontext.namads.Add(formula);
            dbcontext.SaveChanges();

            formula = new namad()
            {

                namadID = Guid.NewGuid(),
                value = "&#61;",
                title = "equal",
                userID = userid,
            };
            dbcontext.namads.Add(formula);
            dbcontext.SaveChanges();

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






        
       
        // process
        public async Task<ActionResult> Process()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            processActionVM responsemodel = new processActionVM();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getProcess", Request.Cookies["clientToken"].Value);
            return View(responsemodel);
        }

        public async Task<ActionResult> setNewProcess(process model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setProcess", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("process");
        }

        // processFormula
        [HttpPost]
        public async Task<ActionResult> processFormula(process model)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            processFormulaActionVM responsemodel = new processFormulaActionVM();
            responsemodel = await methods.PostData(model, responsemodel, "/getProcessFormula", Request.Cookies["clientToken"].Value);
            responsemodel.process = model;
            return View(responsemodel);
        }
        [HttpPost]
        public async Task<ActionResult> addFormulaToProcess (processFormula model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setProcessFormula", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("process");
        }

        // formula
        public async Task<ActionResult> Formula()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            formulaActionVM responsemodel = await methods.PostData(new nullclass(), new formulaActionVM(), "/getFormula", Request.Cookies["clientToken"].Value);
            return View(responsemodel);
        }

        public async Task<ActionResult> setNewFormula(formula model)
        {
             
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setFormula", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("Formula");
        }


        //coding
        public async Task<ActionResult> Coding()
        {

            //Context dbcontext = new Context();
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [codings]");
            //dbcontext.SaveChanges();
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            List<coding> responsemodel = new List<coding>();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getCoding", Request.Cookies["clientToken"].Value);
            return View(responsemodel);
            
        }

        public async Task<ActionResult> setNewCoding(coding model)
        {
            
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setCoding", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)                
                TempData["er"] = responsemodel.message;
            return RedirectToAction("Coding");
        }


        //coDriver
        public async Task<ActionResult> coDriver()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            getCoDriverResponse responsemodel = new getCoDriverResponse();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getCoDriverAsync", Request.Cookies["clientToken"].Value);
            return View(responsemodel);

        }
        public async Task<ActionResult> addDriverAsync(addDriverVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/addDriverAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("coDriver");
        }
        [HttpPost]
        public async Task<ActionResult> setVehicleForDriver(setVehicleForVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setVehicleForDriverAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("coDriver");
        }

        //vehicle

        public async Task<ActionResult> vehicle()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            getVehicleResponce responsemodel = new getVehicleResponce();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getVehicleAsync", Request.Cookies["clientToken"].Value);
            return View(responsemodel);

        }
        [HttpPost]
        public async Task<ActionResult> setVehicleAsync(vehicle model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setVehicleAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("vehicle");
        }

        [HttpPost]
        public async Task<ActionResult> setYdakForVehicle(setYadakForVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setYdakForVehicleAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("vehicle");
        }

        //yadak

        public async Task<ActionResult> yadak()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            getYadakResponce responsemodel = new getYadakResponce();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getYadakAsync", Request.Cookies["clientToken"].Value);
            return View(responsemodel);

        }
        [HttpPost]
        public async Task<ActionResult> setYadakAsync(yadak model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setYadakAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("yadak");
        }
    }
}