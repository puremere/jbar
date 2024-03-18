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
        string baseServer = "https://jbar.app/api/app";// ;"https://localhost:44389/"
        // GET: panel
        public ActionResult Login()
        {
            Context dbcontext = new Context();
            newOrderStatus status1 = new newOrderStatus()
            {
                newOrderStatusID = Guid.NewGuid(),
                statusCode = "1",
                title = "در انتظار"
            };
            dbcontext.newOrderStatuses.Add(status1);
            dbcontext.SaveChanges();
            status1 = new newOrderStatus()
            {
                newOrderStatusID = Guid.NewGuid(),
                statusCode = "2",
                title = "عملیاتی"
            };
            dbcontext.newOrderStatuses.Add(status1);
            dbcontext.SaveChanges();
            status1 = new newOrderStatus()
            {
                newOrderStatusID = Guid.NewGuid(),
                statusCode = "3",
                title = "رد شده"
            };
            dbcontext.newOrderStatuses.Add(status1);
            dbcontext.SaveChanges();
            status1 = new newOrderStatus()
            {
                newOrderStatusID = Guid.NewGuid(),
                statusCode = "4",
                title = "تمام شده"
            };
            dbcontext.newOrderStatuses.Add(status1);
            dbcontext.SaveChanges();
            //List<detailCollection> listItem = new List<detailCollection>();
            //detailCollection item1 = new detailCollection()
            //{
            //    key = "sleeve",
            //    value = "1629132d-335e-4823-9d98-cdcac5e8b9d0",
            //     formID = new Guid("0687a7ca-c78a-41dc-bf5c-25927eac2f62"),
            //      formItemID = new Guid("5e094ad2-cea6-418a-8bb8-2608fcdf0c4c"),
            //       formItemTypeCode = "4"
            //};
            //listItem.Add(item1);
            //detailCollection item2 = new detailCollection()
            //{
            //    key = "fullHeight",
            //    value = "190",
            //    formID = new Guid("0687a7ca-c78a-41dc-bf5c-25927eac2f62"),
            //    formItemID = new Guid("bb1edc2c-6eff-4499-a84e-036bd071d2e2"),
            //    formItemTypeCode = "7"
            //};
            //listItem.Add(item2);
            //detailCollection item3 = new detailCollection()
            //{
            //    key = "userPhone",
            //    value = "09194594505",
            //    formID = new Guid("07e4a30c-3b02-41e2-a7bc-fa85148b66a6"),
            //    formItemID = new Guid("bae7ea90-9c56-49bb-8d48-3555dbe621d0"),
            //    formItemTypeCode = "7"
            //};
            //listItem.Add(item3);
            //detailCollection item4 = new detailCollection()
            //{
            //    key = "userTitle",
            //    value = "mere",
            //    formID = new Guid("07e4a30c-3b02-41e2-a7bc-fa85148b66a6"),
            //    formItemID = new Guid("c1167cc8-b133-4928-b171-7b91c0d63827"),
            //    formItemTypeCode = "7"
            //};
            //listItem.Add(item4);

            //string laststring = JsonConvert.SerializeObject(listItem);
            //string srt = "";
            //Guid userid = new Guid("9f82f3e1-df02-46aa-86cb-7bdbd9887147");
            //formItemDesign ftd = new formItemDesign()
            //{
            //    formItemDesignID = Guid.NewGuid(),
            //    title = "چند انتخابی - اسلایدر",
            //    number = 1

            //};
            //dbcontext.formItemDesigns.Add(ftd);
            //dbcontext.SaveChanges();
            //formItemType frmt = new formItemType()
            //{
            //    formItemTypeID = Guid.NewGuid(),
            //    title = "آیتم آپلود"

            //};
            // dbcontext.formItemTypes.Add(frmt);
            //dbcontext.SaveChanges();
            //frmt = new formItemType()
            //{
            //    formItemTypeID = Guid.NewGuid(),
            //    title = "آیتم چند گزینه ای"

            //};
            //dbcontext.formItemTypes.Add(frmt);
            //dbcontext.SaveChanges();
            //frmt = new formItemType()
            //{
            //    formItemTypeID = Guid.NewGuid(),
            //    title = "آیتم انتخابی"

            //};
            //dbcontext.formItemTypes.Add(frmt);
            //dbcontext.SaveChanges();
            //frmt = new formItemType()
            //{
            //    formItemTypeID = Guid.NewGuid(),
            //    title = "آیتم تاریخ"

            //};
            //dbcontext.formItemTypes.Add(frmt);
            //dbcontext.SaveChanges();
            //frmt = new formItemType()
            //{
            //    formItemTypeID = Guid.NewGuid(),
            //    title = "آیتم موقعیت"

            //};
            //dbcontext.formItemTypes.Add(frmt);
            //dbcontext.SaveChanges();
            // frmt = new formItemType()
            //{
            //    formItemTypeID = Guid.NewGuid(),
            //    title = "آیتم متنی"

            //};
            //dbcontext.formItemTypes.Add(frmt);
            //dbcontext.SaveChanges();
            //List<vehicleStatus> stl = dbcontext.vehicleStatuses.ToList();
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [mabnas]");
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [namads]"); 
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [codings]");
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [formulas]");
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [processes]"); 
            //dbcontext.SaveChanges();


            //userWorkingStatus yd = new userWorkingStatus()
            //{
            //    title = "در حال اجرا",
            //    workingStatusID = Guid.NewGuid(),
            //};
            //dbcontext.userWorkingStatuses.Add(yd);
            //dbcontext.SaveChanges();
            //verifyStatus vs = new verifyStatus()
            //{
            //    title = "تایید نشده",
            //    verifyStatusID = Guid.NewGuid(),
            //    message = "لازم است برای احراز قویت اقدام نمایید",
            //    statusCode = "0"

            //};
            //dbcontext.verifyStatuses.Add(vs);
            //dbcontext.SaveChanges();

            //vs = new verifyStatus()
            //{
            //    title = "دردست بررسی",
            //    verifyStatusID = Guid.NewGuid(),
            //    message = "مدارک شما در دست بررسی می باشد",
            //    statusCode = "1"

            //};
            //dbcontext.verifyStatuses.Add(vs);
            //dbcontext.SaveChanges();

            //vs = new verifyStatus()
            //{
            //    title = "رد شده",
            //    verifyStatusID = Guid.NewGuid(),
            //    message = "به دلیل عدم ارائه مدارک کافی هویت شما احراز نشدهد است",
            //    statusCode = "2"

            //};
            //dbcontext.verifyStatuses.Add(vs);
            //dbcontext.SaveChanges();


            //vs = new verifyStatus()
            //{
            //    title = "تایید شده",
            //    verifyStatusID = Guid.NewGuid(),
            //    message = " هویت شما احراز شده است",
            //    statusCode = "3"

            //};
            //dbcontext.verifyStatuses.Add(vs);
            //dbcontext.SaveChanges();

            //yd = new userWorkingStatus()
            //{
            //    title = "بدون اقدام",
            //    workingStatusID = Guid.NewGuid(),
            //};
            //dbcontext.userWorkingStatuses.Add(yd);
            //dbcontext.SaveChanges();
            //yd = new userWorkingStatus()
            //{
            //    title = "غیر عملیاتی",
            //    workingStatusID = Guid.NewGuid(),
            //};
            //dbcontext.userWorkingStatuses.Add(yd);
            //dbcontext.SaveChanges();



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


            //mabna mabna = new mabna()
            //{
            //    mabnaID = Guid.NewGuid(),
            //    title = "قیمت تمام شده سفارش",
            //    value = "1",
            //    userID = userid,

            //};
            //dbcontext.mabnas.Add(mabna);
            //dbcontext.SaveChanges();
            //mabna = new mabna()
            //{
            //    mabnaID = Guid.NewGuid(),
            //    title = "ارزش بار",
            //    value = "2",
            //    userID = userid,
            //};
            //dbcontext.mabnas.Add(mabna);
            //dbcontext.SaveChanges();
            //mabna = new mabna()
            //{
            //    mabnaID = Guid.NewGuid(),
            //    title = "مسافت کلی بار",
            //    value = "3",
            //    userID = userid,
            //};
            //dbcontext.mabnas.Add(mabna);
            //dbcontext.SaveChanges();
            //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [formulas]");
            //namad formula = new namad()
            //{

            //    namadID = Guid.NewGuid(),
            //    value = "&#43;",
            //    title = "plus",
            //    userID = userid,

            //};
            //dbcontext.namads.Add(formula);
            //dbcontext.SaveChanges();
            //formula = new namad()
            //{

            //    namadID = Guid.NewGuid(),
            //    value = "&#8722;",
            //    title = "minus",
            //    userID = userid,
            //};
            //dbcontext.namads.Add(formula);
            //dbcontext.SaveChanges();

            //formula = new namad()
            //{

            //    namadID = Guid.NewGuid(),
            //    value = "&#215;",
            //    title = "multiple",
            //    userID = userid,
            //};
            //dbcontext.namads.Add(formula);
            //dbcontext.SaveChanges();
            //formula = new namad()
            //{

            //    namadID = Guid.NewGuid(),
            //    value = "&#8260;",
            //    title = "fraction",
            //    userID = userid,
            //};
            //dbcontext.namads.Add(formula);
            //dbcontext.SaveChanges();

            //formula = new namad()
            //{

            //    namadID = Guid.NewGuid(),
            //    value = "&#61;",
            //    title = "equal",
            //    userID = userid,
            //};
            //dbcontext.namads.Add(formula);
            //dbcontext.SaveChanges();

            return View();

        }
        public ActionResult Dashboard()
        {
            return View();
        }


        public async Task<ActionResult> setOrder()
        {

            panelSetOrder fmodel = new panelSetOrder();
            fmodel = await methods.PostData(new nullclass(), fmodel, "/setOrderAction", Request.Cookies["clientToken"].Value);
            return View(fmodel);
        }

        public PartialViewResult getCityPartail(string name, string ID)
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

        // processForm
        
        
        public async Task<ActionResult> processForm(Guid processID)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            processFormActionVM responsemodel = new processFormActionVM();
            process model = new process();
            model.processID = processID;
            responsemodel = await methods.PostData(model, responsemodel, "/getProcessForm", Request.Cookies["clientToken"].Value);
            responsemodel.process = model;
            return View(responsemodel);
        }
        [HttpPost]
        public async Task<ActionResult> setNewFormProcess(setNewFormProcessVM model)
        {

            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setNewFormProcess", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            
            return RedirectToAction("processForm",new { processID = model.processID});
        }


        // formItem

        public async Task<ActionResult> formItem(Guid formID)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            formItemActionVM responsemodel = new formItemActionVM();
            form model = new form();
            model.formID = formID;
            responsemodel = await methods.PostData(model, responsemodel, "/getFormItem", Request.Cookies["clientToken"].Value);
            responsemodel.form = model;
            return View(responsemodel);
        }
        [HttpPost]
        public async Task<ActionResult> addFormItem(formItemVM model)
        {
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (file.ContentLength > 0)
                {
                    string fname = file.FileName;


                    file.SaveAs(Server.MapPath("~/Uploads/") + fname);
                    model.itemtImage = file.FileName;
                }
                
            }
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setFormItem", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            
            return RedirectToAction("formItem", new { formID = model.formID } );
        }
        [HttpPost]
        public async Task<ActionResult> removeFormItem(formItemVM model)
        {
            
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/removeFormItem", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            else
            {
                if (!string.IsNullOrEmpty(responsemodel.message ))
                {
                    string fname = Path.Combine(Server.MapPath("Uploads"), responsemodel.message);
                    bool exists = System.IO.File.Exists(fname);
                    if (exists)
                        System.IO.File.Delete(fname);

                }
            }

            return RedirectToAction("formItem", new { formID = model.formID });
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
        public async Task<ActionResult> addFormulaToProcess(processFormula model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setProcessFormula", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            process pr = new process();
            pr.processID = model.proccessID;
            return RedirectToAction("processFormula", pr);
        }



        // formula
        public async Task<ActionResult> Formula(Guid processID)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            process pr = new process();
            pr.processID = processID;
            formulaActionVM responsemodel = await methods.PostData(pr, new formulaActionVM(), "/getFormula", Request.Cookies["clientToken"].Value);
            responsemodel.process = pr;
            return View(responsemodel);
        }
        [HttpPost]
        public async Task<ActionResult> setNewFormula(formula model)
        {

            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setFormula", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;

           
            return RedirectToAction("Formula" , new { processID = model.processID});
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

        [HttpPost]
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
        [HttpPost]
        public async Task<ActionResult> getCodriverList(coDriverSearchVM model)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            getCoDriverResponse responsemodel = new getCoDriverResponse();
            responsemodel = await methods.PostData(model, responsemodel, "/getCodriverList", Request.Cookies["clientToken"].Value);
            return PartialView("/Views/Shared/panel/_codriverList.cshtml", responsemodel);

        }
        public async Task<ActionResult> addDriverAsync(addDriverVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/addDriverAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("coDriver");
        }
        [HttpPost]
        public async Task<ActionResult> setInfoForDriver(setVehicleForVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/changeUserInfoAsync", Request.Cookies["clientToken"].Value);
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
        public async Task<ActionResult> getVehicleList(vehicleSearchVM model)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            getVehicleResponce responsemodel = new getVehicleResponce();
            responsemodel = await methods.PostData(model, responsemodel, "/getVehicleList", Request.Cookies["clientToken"].Value);
            return PartialView("/Views/Shared/panel/_vehicleList.cshtml", responsemodel);

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
        public async Task<ActionResult> changeVehicleInfo(setYadakForVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/changeVehicleInfoAsync", Request.Cookies["clientToken"].Value);
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
        public async Task<ActionResult> getYadakList(vehicleSearchVM model)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            getYadakResponce responsemodel = new getYadakResponce();
            responsemodel = await methods.PostData(model, responsemodel, "/getYadakList", Request.Cookies["clientToken"].Value);
            return PartialView("/Views/Shared/panel/_yadakList.cshtml", responsemodel);

        }
        [HttpPost]
        public async Task<ActionResult> setYadakAsync(yadak model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setYadakAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("yadak");
        }

        [HttpPost]
        public async Task<ActionResult> changeYadakInfo(setYadakForVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/changeYadakInfoAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("vehicle");
        }


        // product
        public async Task<ActionResult> products()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            productActionVM responsemodel = new productActionVM();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getProductsAsync", Request.Cookies["clientToken"].Value);
            return View(responsemodel);
        }

        public async Task<ActionResult> setProductAsync(addProductVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setProductAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("products");
        }

        public async Task<ActionResult> removeProductAsync(addProductVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/removeProductAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("products");
        }
        //productType
        public async Task<ActionResult> productType()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            productTypeActionVM responsemodel = new productTypeActionVM();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getProductTypeAsync", Request.Cookies["clientToken"].Value);
            return View(responsemodel);
        }
        [HttpPost]
        public async Task<ActionResult> addProductTypeAsync(addProductTypeVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/addProductTypeAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction(model.from);
        }

        // tag 
        public async Task<ActionResult> setTag(tagVM model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setTag", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction(model.from);
        }


        //orderOptions
        public async Task<ActionResult> orderOptions()
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            orderOptionActionVM responsemodel = new orderOptionActionVM();
            responsemodel = await methods.PostData(new nullclass(), responsemodel, "/getOrderOptionAsync", Request.Cookies["clientToken"].Value);
            return View(responsemodel);
        }
        [HttpPost]
        public async Task<ActionResult> addOrderOptionsAsync(addOrderOptionVM model)
        {
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string fname = file.FileName;
              
                
                file.SaveAs(Server.MapPath("~/Uploads/") + fname);
                model.image = file.FileName;
            }
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/addOrderOptionAsync", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            else
            {
                if (responsemodel.message != "")
                {
                    string fname = Path.Combine(Server.MapPath("Uploads"), responsemodel.message);
                    bool exists = System.IO.File.Exists(fname);
                    if (exists)
                        System.IO.File.Delete(fname);
                       
                }
            }
            return RedirectToAction("orderOptions");
        }

        //form

      
        public async Task<ActionResult> Form(process model)
        {
            if (TempData["er"] != null)
                ViewBag.error = TempData["er"].ToString();
            processFormActionVM responsemodel = new processFormActionVM();
            responsemodel = await methods.PostData(model, responsemodel, "/getForm", Request.Cookies["clientToken"].Value);

            return View(responsemodel);
        }
        public async Task<ActionResult> setNewForm(form model)
        {
            responseModel responsemodel = await methods.PostData(model, new responseModel(), "/setForm", Request.Cookies["clientToken"].Value);
            if (responsemodel.status != 200)
                TempData["er"] = responsemodel.message;
            return RedirectToAction("form");
        }
        
    }
}