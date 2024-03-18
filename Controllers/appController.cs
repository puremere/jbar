using jbar.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using jbar.Classes;
using jbar.Model;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Data.Entity.Spatial;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using System.Web.Hosting;
using System.Text;
using System.Net.Http;

namespace jbar.Controllers
{
    public class appController : System.Web.Http.ApiController
    {


        public static DbGeography ConvertLatLonToDbGeography(double longitude, double latitude)
        {
            var point = string.Format("POINT({1} {0})", latitude, longitude);
            return DbGeography.FromText(point);
        }


        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> changeUserLocation([FromBody] getCityNameVM model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                user user = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == userID);
                user.lat = model.lat.ToString();
                user.lon = model.lon.ToString();
                //user.userType = "1";
                DbGeography point = ConvertLatLonToDbGeography(model.lon, model.lat);
                user.point = point;
                await dbcontext.SaveChangesAsync();
            }

            responseModel mymodel = new responseModel();
            mymodel.status = 200;
            mymodel.message = "ok";

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        } // در داخل راننده من استفاده نشده

        [System.Web.Http.HttpPost]
        public async Task<JObject> sendNotif([FromBody] sendOrderNotif model)
        {
            using (Context dbcontext = new Context())
            {
                order order = dbcontext.orders.ToList().Last();
                string orderID = order.orderID.ToString();
                DbGeography point = ConvertLatLonToDbGeography(model.lat, model.lon);


                List<user> useddrs = (from u in dbcontext.users
                                      where u.userType == "1" && u.point.Distance(point) < 2000000
                                      select u).ToList();
                string src = HostingEnvironment.ApplicationPhysicalPath + "\\File\\key.json";
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(src),
                });

                foreach (var item in useddrs)
                {


                    notifVM fm = new notifVM();
                    bigStyle big_style = new bigStyle();
                    big_style.type = "";
                    titleModel title = new titleModel();
                    title.text = "سفارش جدید در اطراف شما ثبت شد";
                    clickAction click_action = new clickAction();
                    click_action.title = "";
                    click_action.type = "openapp";
                    click_action.data = "/home/viewDetail?q=" + orderID;
                    fm.big_style = big_style;
                    fm.title = title;
                    fm.click_action = click_action;

                    string mdljson = JsonConvert.SerializeObject(fm);
                    Dictionary<string, string> dat = new Dictionary<string, string>();
                    dat.Add("mydata", mdljson);
                    var message = new Message()
                    {

                        Data = dat,
                        Notification = new Notification
                        {
                            Title = "بار جدید اومد مردک",
                            Body = "",


                        },
                        Token = item.firebaseToken
                    };
                    var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                }


            }
            responseModel mymodel = new responseModel();
            mymodel.status = 200;
            mymodel.message = "ok";

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;

        }

        [System.Web.Http.HttpPost]
        public async Task<sendCityVM> getCity([FromBody] getCityVM model)
        {
            sendCityVM output = new sendCityVM();

            using (Context dbcontext = new Context())
            {

                if (model == null)
                {

                    List<newcity> cities = await dbcontext.cities.Where(x => x.userID == x.parentID).Select(x => new newcity { title = x.title, parentID = x.parentID, userID = x.userID }).ToListAsync();
                    output.lst = cities;
                    output.status = 200;

                }
                else
                {
                    if (model.search == null && model.ID == null)
                    {
                        List<newcity> cities = await dbcontext.cities.Where(x => x.userID == x.parentID).Select(x => new newcity { title = x.title, parentID = x.parentID, userID = x.userID }).ToListAsync();
                        output.lst = cities;
                        output.status = 200;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.search))
                        {

                            List<city> cities = dbcontext.cities.ToList();
                            List<newcity> afterSearch = await (from u in dbcontext.cities
                                                               join p in dbcontext.cities on u.parentID equals p.userID
                                                               where u.userID != u.parentID && (p.title.Contains(model.search) || u.title.Contains(model.search))
                                                               select new newcity { title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToListAsync();


                            output.lst = afterSearch;
                            output.status = 200;

                        }
                        else
                        {
                            Guid myguid = new Guid(model.ID);
                            List<newcity> afterSearch = await (from u in dbcontext.cities
                                                               join p in dbcontext.cities on u.parentID equals p.userID
                                                               where u.parentID == myguid && u.userID != u.parentID
                                                               select new newcity { title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToListAsync();
                            output.lst = afterSearch;
                            output.status = 200;


                        }
                    }




                }

            }


            return output;
        }

        [System.Web.Http.HttpPost]
        public async Task<sendTypeVM> getType([FromBody] getCityVM model)
        {

            sendTypeVM output = new sendTypeVM();
            string modelstring = "";
            using (Context dbcontext = new Context())
            {

                if (model == null)
                {
                    List<cartypeVM> types = await dbcontext.cartypes.Where(x => x.typeID == x.parentID).Select(x => new cartypeVM { parentID = x.parentID, typeID = x.typeID, title = x.title }).ToListAsync();
                    output.lst = types;
                    output.status = 200;

                }
                else
                {
                    if (model.ID == null)
                    {
                        List<cartypeVM> types = await dbcontext.cartypes.Where(x => x.typeID == x.parentID).Select(x => new cartypeVM { parentID = x.parentID, typeID = x.typeID, title = x.title }).ToListAsync();
                        output.lst = types;
                        output.status = 200;

                    }
                    else
                    {
                        Guid myguid = new Guid(model.ID);
                        List<cartypeVM> types = await dbcontext.cartypes.Where(x => x.parentID == myguid && x.typeID != myguid).Select(x => new cartypeVM { parentID = x.parentID, typeID = x.typeID, title = x.title }).ToListAsync();
                        output.lst = types;
                        output.status = 200;

                    }

                }


            }

            return output;
        }
        [System.Web.Http.HttpPost]
        public async Task<sendTypeVM> getTypeFull([FromBody] getCityVM model)
        {
            try
            {
                sendTypeVM output = new sendTypeVM();
                string modelstring = "";
                using (Context dbcontext = new Context())
                {

                    List<cartypeVM> types = await dbcontext.cartypes.Select(x => new cartypeVM { parentID = x.parentID, title = x.title, typeID = x.typeID }).ToListAsync();
                    output.lst = types;
                    output.status = 200;
                }
                return output;
            }
            catch (Exception e)
            {

                throw;
            }


        }
        [System.Web.Http.HttpPost]
        public async Task<sendLoadTypeVM> getLoadType([FromBody] getCityVM model)
        {

            sendLoadTypeVM output = new sendLoadTypeVM();
            string modelstring = "";
            try
            {
                using (Context dbcontext = new Context())
                {

                    if (model == null)
                    {
                        List<loadTypeVM> types = await dbcontext.loadTypes.Select(x => new loadTypeVM { loadTypeID = x.loadTypeID, title = x.title }).ToListAsync();
                        output.lst = types;
                        output.status = 200;

                    }
                    else
                    {

                        List<loadTypeVM> types = await dbcontext.loadTypes.Where(x => x.title.Contains(model.search)).Select(x => new loadTypeVM { loadTypeID = x.loadTypeID, title = x.title }).ToListAsync();
                        output.lst = types;
                        output.status = 200;
                    }

                    return output;
                }
            }
            catch (Exception e)
            {

                throw;
            }



        }

        [System.Web.Http.HttpPost]
        public JObject register([FromBody] doSignIn model)
        {

            responseModel output = new responseModel();
            string outputstring = "";
            Random rnd = new Random();
            int num = rnd.Next(1111, 9999);


            using (Context dbcontext = new Context())
            {


                //Guid Userguid = new Guid("5417296b-b07e-404a-bc71-f04dc8baac2f");

                //user user = dbcontext.users.SingleOrDefault(x => x.userID == Userguid);
                //dbcontext.users.Remove(user);
                //dbcontext.SaveChanges();
                user myuser = dbcontext.users.SingleOrDefault(x => x.phone == model.phone && x.userType == model.userType);
                if (myuser != null)
                {
                    myuser.code = "9999"; // num.ToString(),
                }
                else
                {
                    string status = model.userType == "1" ? "1" : "0";
                    Guid statusID = dbcontext.verifyStatuses.FirstOrDefault().verifyStatusID;
                    Guid workingID = dbcontext.userWorkingStatuses.FirstOrDefault().workingStatusID;

                    user newuser = new user()
                    {
                        userID = Guid.NewGuid(),
                        phone = model.phone,
                        name = "",
                        code = "9999", // num.ToString(),
                        userType = model.userType,
                        status = status,
                        verifyStatusID = statusID,
                        workingStatusID = workingID
                    };
                    dbcontext.users.Add(newuser);
                }

                dbcontext.SaveChanges();
            }

            output.status = 200;
            outputstring = JsonConvert.SerializeObject(output);
            JObject jObject = JObject.Parse(outputstring); return jObject;


        }

        [System.Web.Http.HttpPost]
        public JObject Verify([FromBody] doSignIn model)
        {
            responseModel output = new responseModel();
            using (Context dbcontext = new Context())
            {
                user myuser = dbcontext.users.SingleOrDefault(x => x.phone == model.phone && x.code == model.code && x.userType == model.userType);
                if (myuser != null)
                {
                    output.status = 200;
                    output.message = TokenManager.GenerateToken(model.phone, myuser.userID.ToString());

                }
                else
                {
                    output.message = "Invalid User.";
                    output.status = 400;
                }


            }
            string outputstring = JsonConvert.SerializeObject(output);
            JObject jObject = JObject.Parse(outputstring); return jObject;

        }

        [System.Web.Http.HttpPost]
        public async Task<JObject> UploadFiles()
        {
            try
            {
                responseModel mymodel = new responseModel();
                //Create the Directory.
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Fetch the File.
                HttpPostedFile postedFile = System.Web.HttpContext.Current.Request.Files[0];


                string fileName = "";
                string name = methods.RandomString();
                //Fetch the File Name.
                fileName = name + Path.GetExtension(postedFile.FileName);
                //Save the File.
                postedFile.SaveAs(path + fileName);
                mymodel.status = 200;
                mymodel.message = fileName;
                //Send OK Response to Client.
                string result = JsonConvert.SerializeObject(mymodel);
                JObject jObject = JObject.Parse(result);
                return jObject;
            }



            catch (Exception baseException)
            {

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(baseException.Message),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        //[HttpPost]
        //public async Task<JObject> UploadFilesAsync([Microsoft.AspNetCore.Mvc.FromForm] UploadModel model)

        //{
        //    long size = model.Files.Sum(f => f.Length);
        //    string fileName = "";
        //    foreach (var formFile in model.Files)
        //    {
        //        if (formFile.Length > 0)
        //        {
        //            var uploads = System.Web.HttpContext.Current.Server.MapPath("/uploads") ; 
        //            var filePath = Path.GetTempFileName();
        //            fileName = Path.GetFileName(formFile.FileName);
        //            using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }
        //        }

        //    }
        //    responseModel mymodel = new responseModel();
        //    mymodel.status = 200;
        //    mymodel.message = fileName;
        //    string result = JsonConvert.SerializeObject(mymodel);
        //    JObject jObject = JObject.Parse(result);
        //    return jObject;

        //}

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<panelSetOrder> setOrderAction()
        {
            sendCityVM citylist = new sendCityVM();
            sendLoadTypeVM loadList = new sendLoadTypeVM();
            sendTypeVM typeList = new sendTypeVM();
            using (Context dbcontext = new Context())
            {
                citylist.lst = await dbcontext.cities.Where(x => x.userID == x.parentID).Select(x => new newcity { title = x.title, parentID = x.parentID, userID = x.userID }).ToListAsync();
                loadList.lst = await dbcontext.loadTypes.Select(x => new loadTypeVM { loadTypeID = x.loadTypeID, title = x.title }).ToListAsync();
                typeList.lst = await dbcontext.cartypes.Where(x => x.parentID != x.typeID).Select(x => new cartypeVM { parentID = x.parentID, typeID = x.typeID, title = x.title }).ToListAsync();
                panelSetOrder fmodel = new panelSetOrder()
                {
                    cityList = citylist,
                    loadList = loadList,
                    typeList = typeList.lst

                };

                return fmodel;
            }

        }

        //jbar
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setOrder([FromBody] setOrderVM model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            string result = "";
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {


                    city origin = dbcontext.cities.SingleOrDefault(x => x.userID == model.originCityID);
                    city destination = dbcontext.cities.SingleOrDefault(x => x.userID == model.destinCityID);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpClient HttpClient = new HttpClient();
                    HttpClient.DefaultRequestHeaders.Add("Api-Key", "service.88c9de7768a84c0393b15c115871da47");
                    string url = "https://api.neshan.org/v1/distance-matrix/no-traffic?type=car&origins=" + origin.lat + "," + origin.lon + "&destinations=" + destination.lat + "," + destination.lon + "";
                    string page = await HttpClient.GetStringAsync(url);
                    distancVM distancemodel = JsonConvert.DeserializeObject<distancVM>(page);
                    int distance = distancemodel.rows.First().elements.First().distance.value;


                    user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
                    List<string> dates = model.date.Trim(',').Split(',').ToList();
                    DateTime orderdete = dateTimeConvert.UnixTimeStampToDateTime(double.Parse(dates.First()));
                    Guid orderguid = Guid.NewGuid();



                    order neworder = new order()
                    {
                        clientID = user.userID,
                        distance = distance,
                        date = orderdete,
                        orderdate = DateTime.Now,
                        description = model.description,
                        destinCityID = model.destinCityID,
                        loadAmount = model.loadAmount,
                        loadingFinishHour = model.loadingFinishHour,
                        loadingStartHour = model.loadingStartHour,
                        loadTypeID = new Guid(model.loadTypeID),
                        orderID = orderguid,
                        originCityID = model.originCityID,
                        pricePerTone = model.pricePerTone,
                        priceTotal = model.priceTotal,
                        recieveSMS = model.recieveSMS,
                        showPhone = model.showPhone,
                        viewCount = 0,
                        publishDate = orderdete,
                        orderStatus = "0",




                    };
                    dbcontext.orders.Add(neworder);
                    List<string> typeList = model.type.Trim(',').Split(',').ToList();
                    foreach (var type in typeList)
                    {
                        //dbcontext.order.Add(newtype);
                        //ordertype newtype = new ordertype()
                        //{
                        //    orderID = orderguid,
                        //    ordertypeID = Guid.NewGuid(),
                        //    typeID = new Guid(type),

                        //};

                    }

                    dbcontext.SaveChanges();

                }

                responseModel mymodel = new responseModel();
                mymodel.status = 200;
                mymodel.message = "ok";
                result = JsonConvert.SerializeObject(mymodel);


            }
            catch (Exception e)
            {

                responseModel mymodel = new responseModel();
                mymodel.status = 400;
                mymodel.message = "Notok";
                result = JsonConvert.SerializeObject(mymodel);
            }


            JObject jObject = JObject.Parse(result);
            return jObject;
        }


        //tailor
        //[BasicAuthentication]
        //[System.Web.Http.HttpPost]
        //public async Task<JObject> setOrder([FromBody] setOrderVM model)
        //{
        //    object someObject;
        //    Request.Properties.TryGetValue("UserToken", out someObject);

        //    Guid userID = new Guid(someObject.ToString());
        //    try
        //    {
        //        using (Context dbcontext = new Context())
        //        {


        //            Guid orgincity = new Guid(model.originCityID);
        //            Guid desctincity = new Guid(model.destinCityID);

        //            city origin = dbcontext.cities.SingleOrDefault(x => x.userID == orgincity);
        //            city destination = dbcontext.cities.SingleOrDefault(x => x.userID == desctincity);
        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
        //            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        //            HttpClient HttpClient = new HttpClient();
        //            HttpClient.DefaultRequestHeaders.Add("Api-Key", "service.88c9de7768a84c0393b15c115871da47");
        //            string url = "https://api.neshan.org/v1/distance-matrix/no-traffic?type=car&origins=" + origin.lat + "," + origin.lon + "&destinations=" + destination.lat + "," + destination.lon + "";
        //            string page = await HttpClient.GetStringAsync(url);
        //            distancVM distancemodel = JsonConvert.DeserializeObject<distancVM>(page);
        //            int distance = distancemodel.rows.First().elements.First().distance.value;


        //            user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
        //            List<string> dates = model.date.Trim(',').Split(',').ToList();
        //            DateTime orderdete = dateTimeConvert.UnixTimeStampToDateTime(double.Parse(dates.First()));
        //            Guid orderguid = Guid.NewGuid();


        //            Guid thirdPartyID = Guid.NewGuid();
        //            thirdParty th = new thirdParty()
        //            {
        //                address = model.address,
        //                fullname = model.fullname,
        //                IDNumber = model.IDNumber,
        //                phone = model.phone,
        //                Rank = model.Rank,
        //                unitName = model.unitName,
        //                ThirdPartyID = thirdPartyID
        //            };
        //            dbcontext.thirdParties.Add(th);

        //            order neworder = new order()
        //            {
        //                thirdPartyID = thirdPartyID,
        //                clientID = user.userID,
        //                distance = 0,
        //                date = DateTime.Now,
        //                orderdate = DateTime.Now,
        //                description = model.comment,
        //                destinCityID = null,
        //                driverID = null,
        //                loadAmount = model.loadAmount,
        //                loadingFinishHour =model.loadingFinishHour,
        //                loadingStartHour =   model.loadingStartHour,
        //                loadTypeID = new Guid(),
        //                orderID = orderguid,
        //                originCityID =  orgincity,
        //                pricePerTone =  model.pricePerTone,
        //                priceTotal = model.priceTotal,
        //                recieveSMS = model.recieveSMS,
        //                showPhone = model.showPhone,
        //                viewCount = 0,
        //                publishDate =  orderdete,
        //                orderStatus = "1",
        //                video = model.video,


        //                beretSize = model.beretSize,
        //                bieop = model.bieop,
        //                comment = model.comment,
        //                chestCirc = model.chestCirc,
        //                frontLength = model.frontLength,
        //                fullHeight = model.headCirc,
        //                headCirc = model.headCirc,
        //                jacketSize = model.jacketSize,
        //                jacketWaist = model.jacketWaist,
        //                jktBLength = model.jktBLength,
        //                jktHipCirc = model.jktHipCirc,
        //                neckCirc = model.neckCirc,
        //                shirtSize = model.shirtSize,
        //                shoeSize = model.shoeSize,
        //                sleeveLength = model.sleeveLength,
        //                troHem = model.troHem,
        //                troHip = model.troHip,
        //                troInseam = model.troInseam,
        //                troLength = model.troLength,
        //                trouserSize = model.trouserSize,
        //                troWaist = model.troWaist,


        //            };
        //            dbcontext.orders.Add(neworder);
        //            //List<string> typeList = model.type.Trim(',').Split(',').ToList();
        //            //foreach (var type in typeList)
        //            //{
        //            //    //dbcontext.order.Add(newtype);
        //            //    //ordertype newtype = new ordertype()
        //            //    //{
        //            //    //    orderID = orderguid,
        //            //    //    ordertypeID = Guid.NewGuid(),
        //            //    //    typeID = new Guid(type),

        //            //    //};

        //            //}



        //            await dbcontext.SaveChangesAsync();

        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }


        //    responseModel mymodel = new responseModel();
        //    mymodel.status = 200;
        //    mymodel.message = "ok";

        //    string result = JsonConvert.SerializeObject(mymodel);
        //    JObject jObject = JObject.Parse(result);
        //    return jObject;
        //}


        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public getOrderVM getOrderClient()
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            getOrderVM mymodel = new getOrderVM();



            using (Context dbcontext = new Context())
            {

                List<order> lstorder = dbcontext.orders.ToList();
                List<orderListVM> lst = (from p in dbcontext.orders
                                         join c in dbcontext.cities on
                                         p.originCityID equals c.userID
                                         join cc in dbcontext.cities on
                                         p.destinCityID equals cc.userID
                                         where p.clientID == userID
                                         orderby p.orderdate descending
                                         select new orderListVM
                                         {
                                             orderID = p.orderID.ToString(),
                                             status = p.orderStatus,
                                             origin = c.title,
                                             destin = cc.title,
                                             pricePerTon = (int)p.pricePerTone,
                                             priceTotal = (int)p.priceTotal,
                                             viewNumber = "0"

                                         }).ToList();



                foreach (var item in lst)
                {
                    if (item.status == "1")
                    {
                        Guid orid = new Guid(item.orderID);
                        int count = dbcontext.orderResponses.Count(x => x.orderID == orid);
                        item.viewNumber = count.ToString();
                    }
                }
                mymodel.orderList = lst;
            }



            mymodel.status = 200;
            mymodel.message = "ok";


            return mymodel;
        }




        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task< getOrderVM> getUserOrder([FromBody] setOrderVM model)
        {

            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());

            getOrderVM mymodel = new getOrderVM();

            using (Context dbcontext = new Context())
            {
                List<order> lstorder = dbcontext.orders.ToList();
                List<orderListVM> lst = await dbcontext.orders.Include(x => x.origincity).Include(x => x.destincity).Where(x => x.driverID == userID).Select(x => new orderListVM
                {
                    orderID = x.orderID.ToString(),
                    origin = x.origincity.title,
                    destin = x.destincity.title,
                    pricePerTon = (int)x.pricePerTone,
                    priceTotal = (int)x.priceTotal,
                    viewNumber = "10"

                }).ToListAsync();
                //(from p in dbcontext.orders
                //                         join c in dbcontext.cities on
                //                         p.originCityID equals c.userID
                //                         join cc in dbcontext.cities on
                //                         p.destinCityID equals cc.userID
                //                         where p.orderStatus == "2" && p.driverID == userID
                //                         select new orderListVM
                //                         {
                //                             orderID = p.orderID.ToString(),
                //                             origin = c.title,
                //                             destin = cc.title,
                //                             pricePerTon = (int)p.pricePerTone,
                //                             priceTotal = (int)p.priceTotal,
                //                             viewNumber = "10"

                //                         }).ToList();




                mymodel.orderList = lst;
            }


            mymodel.status = 200;
            mymodel.message = "ok";


            return mymodel;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public getOrderVM getUnpublishOrder([FromBody] setOrderVM model)
        {

            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());

            getOrderVM mymodel = new getOrderVM();

            using (Context dbcontext = new Context())
            {
                List<order> lstorder = dbcontext.orders.ToList();
                List<orderListVM> lst = (from p in dbcontext.orders
                                         join c in dbcontext.cities on
                                         p.originCityID equals c.userID
                                         join cc in dbcontext.cities on
                                         p.destinCityID equals cc.userID
                                         where p.orderStatus == "0"
                                         select new orderListVM
                                         {
                                             orderID = p.orderID.ToString(),
                                             origin = c.title,
                                             destin = cc.title,
                                             pricePerTon = (int)p.pricePerTone,
                                             priceTotal = (int)p.priceTotal,
                                             viewNumber = "10"

                                         }).ToList();




                mymodel.orderList = lst;
            }


            mymodel.status = 200;
            mymodel.message = "ok";


            return mymodel;
        }


        //jbar
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getOrderVM> getOrder([FromBody] setOrderVM model)
        {

            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());

            getOrderVM mymodel = new getOrderVM();

            using (Context dbcontext = new Context())
            {
                List<order> lstorder = dbcontext.orders.ToList();
                var qlist = dbcontext.orders.Include(x => x.origincity).Include(x => x.destincity).Where(x=>x.orderStatus == "1").AsQueryable();

                if (model != null)
                {
                    if (model.originCityID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {

                        qlist = qlist.Where(x => x.originCityID == model.originCityID);
                    }
                    if (model.destinCityID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {

                        qlist = qlist.Where(x => x.destinCityID == model.destinCityID);
                    }

                }
                

                List<orderListVM> lst = await (from p in qlist
                                               select new orderListVM
                                               {
                                                   orderID = p.orderID.ToString(),
                                                   origin = p.origincity.title,
                                                   destin = p.destincity.title,
                                                   pricePerTon = (int)p.pricePerTone,
                                                   priceTotal = (int)p.priceTotal,
                                                   viewNumber = "10"

                                               }).ToListAsync();




                mymodel.orderList = lst;
            }


            mymodel.status = 200;
            mymodel.message = "ok";


            return mymodel;
        }

        //tailor
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getOrderVM> getOrderTailor([FromBody] setOrderVM model)
        {

            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());

            getOrderVM mymodel = new getOrderVM();

            using (Context dbcontext = new Context())
            {
                List<order> lstorder = dbcontext.orders.ToList();
                var qlist = dbcontext.orders.Include(x => x.thirdPartyID).AsQueryable();

                if (!string.IsNullOrEmpty(model.query))
                {
                    qlist = qlist.Where(x => x.ThirdParty.address.Contains(model.query) || x.ThirdParty.fullname.Contains(model.query) || x.ThirdParty.Rank.Contains(model.Rank) || x.ThirdParty.IDNumber.Contains(model.IDNumber));

                }
                if (!string.IsNullOrEmpty(model.dateFrom))
                {
                    DateTime from = DateTime.Parse(model.dateFrom);
                    qlist = qlist.Where(x => x.date >= from);
                }
                if (!string.IsNullOrEmpty(model.dateTo))
                {
                    DateTime to = DateTime.Parse(model.dateTo);
                    qlist = qlist.Where(x => x.date <= to);
                }

                List<orderListVM> lst = await (from p in qlist
                                               select new orderListVM
                                               {
                                                   orderID = p.orderID.ToString(),
                                                   address = p.ThirdParty.address,
                                                   fullname = p.ThirdParty.fullname,
                                                   date = p.date,
                                                   status = "",

                                               }).ToListAsync();




                mymodel.orderList = lst;
            }


            mymodel.status = 200;
            mymodel.message = "ok";


            return mymodel;
        }


        //tailor

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public sendDetailVM getOrderDetailTailor([FromBody] orderDetailVM model)
        {
            sendDetailVM result = new sendDetailVM();
            using (Context dbcontext = new Context())
            {
                try
                {
                    Guid orderGuid = new Guid(model.orderID);
                    order myorder = dbcontext.orders.SingleOrDefault(c => c.orderID == orderGuid);
                    user client = dbcontext.users.SingleOrDefault(x => x.userID == myorder.clientID);
                    thirdParty th = dbcontext.thirdParties.SingleOrDefault(x => x.ThirdPartyID == myorder.thirdPartyID);









                    result.origing = new newcity();
                    result.orderStatus = "";
                    result.destination = new newcity();
                    result.description = "";
                    result.distance = "0";
                    result.typeOrderList = new List<newtype>();
                    result.netTotal = 0;
                    result.netPerTon = 0;
                    result.pricePerKiloometre = 0;
                    result.comments = new List<orderCommentVM>();
                    result.returnOrderCount = 5;
                    result.clientTotalComment = 28;
                    result.clientMark = 4.2;
                    result.clientName = "";
                    result.clientStatus = "";
                    result.totalView = 0; // myorder.viewCount;


                    result.address = th.address;
                    result.beretSize = myorder.beretSize;
                    result.bieop = myorder.bieop;
                    result.chestCirc = myorder.chestCirc;
                    result.comment = myorder.comment;
                    result.frontLength = myorder.frontLength;
                    result.fullHeight = myorder.fullHeight;
                    result.fullname = th.fullname;
                    result.headCirc = myorder.headCirc;
                    result.IDNumber = th.IDNumber;
                    result.jacketSize = myorder.jacketSize;
                    result.jacketWaist = myorder.jacketWaist;
                    result.jktBLength = myorder.jktBLength;
                    result.jktHipCirc = myorder.jktHipCirc;
                    result.neckCirc = myorder.neckCirc;
                    result.phone = th.phone;
                    result.Rank = th.Rank;
                    result.shirtSize = myorder.shirtSize;
                    result.shoeSize = myorder.shoeSize;
                    result.sleeveLength = myorder.sleeveLength;
                    result.troHem = myorder.troHem;
                    result.troHip = myorder.troInseam;
                    result.troLength = myorder.troLength;
                    result.trouserSize = myorder.trouserSize;
                    result.troWaist = myorder.troWaist;
                    result.unitName = th.unitName;
                    result.status = 200;



                }
                catch (Exception e)
                {
                }


            }
            return result;
        }


        // jbar
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<sendDetailVM> getOrderDetail([FromBody] orderDetailVM model)
        {
            sendDetailVM result = new sendDetailVM();
            using (Context dbcontext = new Context())
            {
                try
                {
                    Guid orderGuid = new Guid(model.orderID);
                    order myorder = await dbcontext.orders.SingleOrDefaultAsync(c => c.orderID == orderGuid);
                    user client = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == myorder.clientID);
                    city orginc = await dbcontext.cities.Include(x => x.parentcity).SingleOrDefaultAsync(x => x.userID == myorder.originCityID);
                    newcity origin = new newcity { lat = orginc.lat, lon = orginc.lon, title = orginc.title + " ( " + orginc.parentcity.title + " ) ", userID = orginc.userID, parentID = orginc.parentID };


                    newcity destination = await dbcontext.cities.Include(x => x.parentcity).Select(u => new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + u.parentcity.title + " ) ", userID = u.userID, parentID = u.parentID }).SingleOrDefaultAsync(x => x.userID == myorder.destinCityID);

                    //(from u in dbcontext.cities
                    //                  join p in dbcontext.cities on u.parentID equals p.userID
                    //                  where u.userID != u.parentID && u.userID == myorder.originCityID
                    //                  select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();

                    //List<newcity> destination = await (from u in dbcontext.cities
                    //                                   join p in dbcontext.cities on u.parentID equals p.userID
                    //                                   where u.userID != u.parentID && u.userID == myorder.destinCityID
                    //                                   select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToListAsync();


                    //List<newtype> types = await (from o in dbcontext.cartypes
                    //                             join ot in dbcontext.ordertypes on o.typeID equals ot.typeID
                    //                             join otp in dbcontext.cartypes on o.parentID equals otp.typeID
                    //                             where ot.orderID == orderGuid
                    //                             select new newtype { title = otp.title + " - " + o.title }).ToListAsync();

                    List<orderCommentVM> comments = await (from c in dbcontext.comments
                                                           join u in dbcontext.users on c.userID equals u.userID
                                                           where c.orderID == orderGuid
                                                           select new orderCommentVM { clientImage = "https://www.jbar.app/Uploads/" + u.profileImage, clientMark = c.clientMark, clientTitle = u.name, content = c.content, date = c.date }).ToListAsync();


                    var qResponse = (from c in dbcontext.orderResponses
                                     join u in dbcontext.users on c.driverID equals u.userID
                                     where c.orderID == orderGuid
                                     select new responsToOrder { driverID = c.driverID.ToString(), phone = u.phone, price = c.price, title = u.name }).AsQueryable();

                    if (myorder.orderStatus == "2")
                    {
                        qResponse = qResponse.Where(x => x.driverID == myorder.driverID.ToString());
                    }

                    List<responsToOrder> orderResponse = await qResponse.ToListAsync();
                    result.orderRespons = orderResponse;
                    foreach (var item in comments)
                    {
                        item.srtdate = dateTimeConvert.ToPersianDateString(item.date);
                    }
                    var sb = new StringBuilder();
                    sb.Append(Math.Ceiling(myorder.distance / 1000).ToString());
                    sb.Append("کیلومتر ");


                    result.origing = origin;
                    result.orderStatus = myorder.orderStatus;
                    result.destination = destination;
                    result.description = myorder.description;
                    result.distance = sb.ToString();
                    result.typeOrderList = new List<newtype>();
                    result.netTotal = (int)myorder.priceTotal == 0 ? (int)myorder.pricePerTone * (int)myorder.loadAmount : (int)myorder.priceTotal;
                    result.netPerTon = (int)myorder.pricePerTone;
                    result.pricePerKiloometre = (int)((int)myorder.priceTotal / (myorder.distance / 1000));
                    result.comments = comments;
                    result.returnOrderCount = 5;
                    result.clientTotalComment = 28;
                    result.clientMark = 4.2;
                    result.clientName = client.name;
                    result.clientStatus = client.status;
                    result.totalView = 0; // myorder.viewCount;
                    result.status = 200;
                    var sb2 = new StringBuilder();
                    sb2.Append((DateTime.Now - myorder.date).TotalHours);
                    sb2.Append(" ساعت پیش ");

                    var sb3 = new StringBuilder();
                    sb3.Append((DateTime.Now - myorder.date).Minutes);
                    sb3.Append("  دقیقه پیش ");
                    result.passedTime = (int)(DateTime.Now - myorder.date).TotalHours > 1 ? sb2.ToString() : sb3.ToString();



                }
                catch (Exception e)
                {


                }
                return result;
            }
           
        }



        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public sendDetailVM getOrderDetailClient([FromBody] orderDetailVM model)
        {
            sendDetailVM result = new sendDetailVM();
            using (Context dbcontext = new Context())
            {
                try
                {
                    Guid orderGuid = new Guid(model.orderID);
                    order myorder = dbcontext.orders.SingleOrDefault(c => c.orderID == orderGuid);
                    user client = dbcontext.users.SingleOrDefault(x => x.userID == myorder.clientID);
                    thirdParty th = dbcontext.thirdParties.SingleOrDefault(x => x.ThirdPartyID == myorder.thirdPartyID);









                    result.origing = new newcity();
                    result.orderStatus = "";
                    result.destination = new newcity();
                    result.description = "";
                    result.distance = "0";
                    result.typeOrderList = new List<newtype>();
                    result.netTotal = 0;
                    result.netPerTon = 0;
                    result.pricePerKiloometre = 0;
                    result.comments = new List<orderCommentVM>();
                    result.returnOrderCount = 5;
                    result.clientTotalComment = 28;
                    result.clientMark = 4.2;
                    result.clientName = "";
                    result.clientStatus = "";
                    result.totalView = 0; // myorder.viewCount;


                    result.address = th.address;
                    result.beretSize = myorder.beretSize;
                    result.bieop = myorder.bieop;
                    result.chestCirc = myorder.chestCirc;
                    result.comment = myorder.comment;
                    result.frontLength = myorder.frontLength;
                    result.fullHeight = myorder.fullHeight;
                    result.fullname = th.fullname;
                    result.headCirc = myorder.headCirc;
                    result.IDNumber = th.IDNumber;
                    result.jacketSize = myorder.jacketSize;
                    result.jacketWaist = myorder.jacketWaist;
                    result.jktBLength = myorder.jktBLength;
                    result.jktHipCirc = myorder.jktHipCirc;
                    result.neckCirc = myorder.neckCirc;
                    result.phone = th.phone;
                    result.Rank = th.Rank;
                    result.shirtSize = myorder.shirtSize;
                    result.shoeSize = myorder.shoeSize;
                    result.sleeveLength = myorder.sleeveLength;
                    result.troHem = myorder.troHem;
                    result.troHip = myorder.troInseam;
                    result.troLength = myorder.troLength;
                    result.trouserSize = myorder.trouserSize;
                    result.troWaist = myorder.troWaist;
                    result.unitName = th.unitName;
                    result.status = 200;



                }
                catch (Exception e)
                {


                }


            }
            return result;
        }


        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<sendDetailVM> getOrderDetailClientAsync([FromBody] orderDetailVM model)
        {
            sendDetailVM result = new sendDetailVM();
            using (Context dbcontext = new Context())
            {
                try
                {
                    Guid orderGuid = new Guid(model.orderID);
                    order myorder = await dbcontext.orders.SingleOrDefaultAsync(c => c.orderID == orderGuid);
                    user client = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == myorder.clientID);
                    city orginc = await dbcontext.cities.Include(x => x.parentcity).SingleOrDefaultAsync(x => x.userID == myorder.originCityID);
                    newcity origin = new newcity { lat = orginc.lat, lon = orginc.lon, title = orginc.title + " ( " + orginc.parentcity.title + " ) ", userID = orginc.userID, parentID = orginc.parentID };


                    newcity destination = await dbcontext.cities.Include(x => x.parentcity).Select(u => new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + u.parentcity.title + " ) ", userID = u.userID, parentID = u.parentID }).SingleOrDefaultAsync(x => x.userID == myorder.destinCityID);

                    //(from u in dbcontext.cities
                    //                  join p in dbcontext.cities on u.parentID equals p.userID
                    //                  where u.userID != u.parentID && u.userID == myorder.originCityID
                    //                  select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();

                    //List<newcity> destination = await (from u in dbcontext.cities
                    //                                   join p in dbcontext.cities on u.parentID equals p.userID
                    //                                   where u.userID != u.parentID && u.userID == myorder.destinCityID
                    //                                   select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToListAsync();


                    //List<newtype> types = await (from o in dbcontext.cartypes
                    //                             join ot in dbcontext.ordertypes on o.typeID equals ot.typeID
                    //                             join otp in dbcontext.cartypes on o.parentID equals otp.typeID
                    //                             where ot.orderID == orderGuid
                    //                             select new newtype { title = otp.title + " - " + o.title }).ToListAsync();

                    List<orderCommentVM> comments = await (from c in dbcontext.comments
                                                           join u in dbcontext.users on c.userID equals u.userID
                                                           where c.orderID == orderGuid
                                                           select new orderCommentVM { clientImage = "https://www.jbar.app/Uploads/" + u.profileImage, clientMark = c.clientMark, clientTitle = u.name, content = c.content, date = c.date }).ToListAsync();


                    var qResponse = (from c in dbcontext.orderResponses
                                     join u in dbcontext.users on c.driverID equals u.userID
                                     where c.orderID == orderGuid
                                     select new responsToOrder { driverID = c.driverID.ToString(), phone = u.phone, price = c.price, title = u.name }).AsQueryable();

                    if (myorder.orderStatus == "2")
                    {
                        qResponse = qResponse.Where(x => x.driverID == myorder.driverID.ToString());
                    }

                    List<responsToOrder> orderResponse = await qResponse.ToListAsync();
                    result.orderRespons = orderResponse;
                    foreach (var item in comments)
                    {
                        item.srtdate = dateTimeConvert.ToPersianDateString(item.date);
                    }
                    var sb = new StringBuilder();
                    sb.Append(Math.Ceiling(myorder.distance / 1000).ToString());
                    sb.Append("کیلومتر ");


                    result.origing = origin;
                    result.orderStatus = myorder.orderStatus;
                    result.destination = destination;
                    result.description = myorder.description;
                    result.distance = sb.ToString();
                    result.typeOrderList = new List<newtype>();
                    result.netTotal = (int)myorder.priceTotal == 0 ? (int)myorder.pricePerTone * (int)myorder.loadAmount : (int)myorder.priceTotal;
                    result.netPerTon = (int)myorder.pricePerTone;
                    result.pricePerKiloometre = (int)((int)myorder.priceTotal / (myorder.distance / 1000));
                    result.comments = comments;
                    result.returnOrderCount = 5;
                    result.clientTotalComment = 28;
                    result.clientMark = 4.2;
                    result.clientName = client.name;
                    result.clientStatus = client.status;
                    result.totalView = 0; // myorder.viewCount;
                    result.status = 200;
                    var sb2 = new StringBuilder();
                    sb2.Append((DateTime.Now - myorder.date).TotalHours);
                    sb2.Append(" ساعت پیش ");

                    var sb3 = new StringBuilder();
                    sb3.Append((DateTime.Now - myorder.date).Minutes);
                    sb3.Append("  دقیقه پیش ");
                    result.passedTime = (int)(DateTime.Now - myorder.date).TotalHours > 1 ? sb2.ToString() : sb3.ToString();



                }
                catch (Exception e)
                {


                }


            }
            return result;
        }






        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getDashbaord> getDashboard()
        {

            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
                if (!string.IsNullOrEmpty(user.typeID))
                {
                    Guid carguid = new Guid(user.typeID);
                    cartypeVM cartype =await dbcontext.cartypes.Select(x=> new cartypeVM {  parentID = x.parentID, title = x.title, typeID = x.typeID}).SingleOrDefaultAsync(x => x.typeID == carguid);
                    obj.type = cartype;
                    obj.status = 200;
                }
                List<city> citylist = dbcontext.cities.ToList();
                List<newcity> citySelected = (from u in dbcontext.cities
                                           where u.userID != u.parentID
                                           let distance = u.citypoint.Distance(user.point)
                                           orderby distance ascending
                                           select new newcity {  title =u.title, parentID = u.parentID, userID = u.userID, lat = u.lat, lon = u.lon}).ToList();
                obj.origin = citySelected.First();
                obj.status = 200;


            }
            return obj;
        }

        public async Task<JObject> verifyOrder([FromBody] requestOrderVM model)
        {
            responseModel mymodel = new responseModel();
            string result = "";
            using (Context dbcontext = new Context())
            {
                Guid orderID = new Guid(model.orderID);
                order selectedOrder = dbcontext.orders.SingleOrDefault(x => x.orderID == orderID);
                selectedOrder.orderStatus = "1";
                dbcontext.SaveChanges();


                sendDetailVM modeltosend = new sendDetailVM();
                newcity origin = (from u in dbcontext.cities
                                  join p in dbcontext.cities on u.parentID equals p.userID
                                  where u.userID != u.parentID && u.userID == selectedOrder.originCityID
                                  select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();
                newcity destination = (from u in dbcontext.cities
                                       join p in dbcontext.cities on u.parentID equals p.userID
                                       where u.userID != u.parentID && u.userID == selectedOrder.destinCityID
                                       select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();


                var sb = new StringBuilder();
                sb.Append(Math.Ceiling(selectedOrder.distance / 1000).ToString());
                sb.Append("کیلومتر ");
                modeltosend.netTotal = (int)selectedOrder.priceTotal == 0 ? (int)selectedOrder.pricePerTone * (int)selectedOrder.loadAmount : (int)selectedOrder.priceTotal;
                modeltosend.distance = sb.ToString();
                modeltosend.origing = origin;
                modeltosend.destination = destination;
                modeltosend.orderID = model.orderID;
                string notifString = JsonConvert.SerializeObject(modeltosend);




                city ordercity = dbcontext.cities.SingleOrDefault(x => x.userID == selectedOrder.originCityID);
                DbGeography point = ordercity.citypoint;

                //List<user> useddrs0 = dbcontext.users.Where(x=>x.phone == "09194594505").ToList();
                List<user> useddrs = (from u in dbcontext.users
                                      where u.userType == "1" && u.point.Distance(point) < 2000000
                                      select u).ToList();
                string src = HostingEnvironment.ApplicationPhysicalPath + "\\File\\key.json";
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(src),
                    });
                }


                foreach (var item in useddrs)
                {

                    if (true) //item.phone == "09194594505"
                    {
                        notifVM fm = new notifVM();
                        fm.data = notifString;
                        bigStyle big_style = new bigStyle();
                        big_style.type = "";
                        titleModel title = new titleModel();
                        title.text = "productDetail";
                        clickAction click_action = new clickAction();
                        click_action.title = "";
                        click_action.type = "openapp";
                        //click_action.data = "/home/viewDetail?q=" + orderID;
                        fm.big_style = big_style;
                        fm.title = title;
                        fm.click_action = click_action;

                        string mdljson = JsonConvert.SerializeObject(fm);
                        Dictionary<string, string> dat = new Dictionary<string, string>();
                        dat.Add("mydata", mdljson);
                        var message = new Message()
                        {

                            Data = dat,
                            Notification = new Notification
                            {
                                Title = "درخواست جدید انتقال بار",
                                Body = "",


                            },
                            Token = item.firebaseToken
                        };
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                        mymodel.status = 200;
                    }

                }






            }
            result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;

        }
        public async Task<JObject> verifyOrderrResponse([FromBody] responseOrderVM model)
        {
            responseModel mymodel = new responseModel();
            string result = "";
            using (Context dbcontext = new Context())
            {
                Guid orderID = new Guid(model.orderID);
                Guid driverID = new Guid(model.driverID);
                order selectedOrder = dbcontext.orders.SingleOrDefault(x => x.orderID == orderID);
                selectedOrder.orderStatus = "2";
                selectedOrder.driverID = driverID;
                dbcontext.SaveChanges();


                sendDetailVM modeltosend = new sendDetailVM();


                modeltosend.orderID = model.orderID;
                string notifString = JsonConvert.SerializeObject(modeltosend);




                city ordercity = dbcontext.cities.SingleOrDefault(x => x.userID == selectedOrder.originCityID);
                DbGeography point = ordercity.citypoint;


                user driver = dbcontext.users.SingleOrDefault(x => x.userID == driverID);
                string src = HostingEnvironment.ApplicationPhysicalPath + "\\File\\key.json";
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(src),
                    });
                }


                notifVM fm = new notifVM();
                fm.data = notifString;
                bigStyle big_style = new bigStyle();
                big_style.type = "";
                titleModel title = new titleModel();
                title.text = "orderVerify";
                clickAction click_action = new clickAction();
                click_action.title = "";
                click_action.type = "openapp";
                click_action.data = "/home/viewDetail?q=" + orderID;
                fm.big_style = big_style;
                fm.title = title;
                fm.click_action = click_action;

                string mdljson = JsonConvert.SerializeObject(fm);
                Dictionary<string, string> dat = new Dictionary<string, string>();
                dat.Add("mydata", mdljson);
                var message = new Message()
                {

                    Data = dat,
                    Notification = new Notification
                    {
                        Title = "بار شما جدید شما تایید شد",
                        Body = "",
                    },
                    Token = driver.firebaseToken
                };
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                mymodel.status = 200;






            }
            result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> requestOrder([FromBody] requestOrderVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);

                Guid userID = new Guid(someObject.ToString());
                Guid orderID = new Guid(model.orderID);
                using (Context dbcontext = new Context())
                {
                    orderResponse lastorder = await dbcontext.orderResponses.SingleOrDefaultAsync(x => x.driverID == userID && x.orderID == orderID);
                    if (lastorder == null)
                    {
                        orderResponse rsp = new orderResponse()
                        {
                            orderresponseID = Guid.NewGuid(),
                            driverID = userID,
                            orderID = orderID,
                            price = model.price,
                        };
                        dbcontext.orderResponses.Add(rsp);
                    }
                    else
                    {
                        lastorder.price = model.price;
                    }

                    await dbcontext.SaveChangesAsync();
                    sendDetailVM modeltosend = new sendDetailVM();
                    modeltosend.orderID = model.orderID;
                    string notifString = JsonConvert.SerializeObject(modeltosend);
                    order order = dbcontext.orders.SingleOrDefault(x => x.orderID == orderID);
                    user orderclient = dbcontext.users.SingleOrDefault(x => x.userID == order.clientID);
                    string src = HostingEnvironment.ApplicationPhysicalPath + "\\File\\key.json";
                    if (FirebaseApp.DefaultInstance == null)
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile(src),
                        });
                    }


                    notifVM fm = new notifVM();
                    fm.data = notifString;
                    bigStyle big_style = new bigStyle();
                    big_style.type = "";
                    titleModel title = new titleModel();
                    title.text = "orderVerify";
                    clickAction click_action = new clickAction();
                    click_action.title = "";
                    click_action.type = "openapp";
                    click_action.data = "/home/viewDetail?q=" + orderID;
                    fm.big_style = big_style;
                    fm.title = title;
                    fm.click_action = click_action;

                    string mdljson = JsonConvert.SerializeObject(fm);
                    Dictionary<string, string> dat = new Dictionary<string, string>();
                    dat.Add("mydata", mdljson);
                    var message = new Message()
                    {

                        Data = dat,
                        Notification = new Notification
                        {
                            Title = "درخواست جدید برای بار شما",
                            Body = "",
                        },
                        Token = orderclient.firebaseToken
                    };
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                    mymodel.status = 200;
                    mymodel.message = "ok";
                }


                string result = JsonConvert.SerializeObject(mymodel);

                JObject jObject = JObject.Parse(result);
                return jObject;
            }
            catch (Exception e)
            {

                throw;
            }
            
        }
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public JObject addComment([FromBody] setCommentVM model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            Guid orderID = new Guid(model.orderID);
            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
                order order = dbcontext.orders.SingleOrDefault(x => x.orderID == orderID);
                Comment comment = new Comment()
                {
                    CommentID = Guid.NewGuid(),
                    clientID = order.clientID,
                    orderID = order.orderID,
                    clientMark = model.mark,
                    content = model.content,
                    date = DateTime.Now,
                    userID = userID
                };
                dbcontext.comments.Add(comment);
                dbcontext.SaveChanges();
            }
            responseModel mymodel = new responseModel();
            mymodel.status = 200;
            mymodel.message = "ok";
            string result = JsonConvert.SerializeObject(mymodel);

            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public getCommentVM getAllComment([FromBody] setCommentVM model)
        {
            getCommentVM result = new getCommentVM();

            using (Context dbcontext = new Context())
            {

                Guid orderID = new Guid(model.orderID);

                List<orderCommentVM> comments = (from c in dbcontext.comments
                                                 join u in dbcontext.users on c.userID equals u.userID
                                                 where c.orderID == orderID
                                                 select new orderCommentVM { clientImage = "https://www.jbar.app/Uploads/" + u.profileImage, clientMark = c.clientMark, clientTitle = u.name, content = c.content, date = c.date }).ToList();


                foreach (var item in comments)
                {
                    item.srtdate = dateTimeConvert.ToPersianDateString(item.date);
                }
                result.lst = comments;
                result.status = 200;
            }
            return result;
        }



        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public JObject setProfile([FromBody] setProfileVM model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());

            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
                if (user != null)
                {

                    if (!string.IsNullOrEmpty(model.hooshmandMashin))
                        user.hooshmandMashin = model.hooshmandMashin;
                    if (!string.IsNullOrEmpty(model.address))
                        user.address = model.address;
                    if (!string.IsNullOrEmpty(model.clientType)) { user.clientType = model.clientType; user.status = "1"; }

                    if (!string.IsNullOrEmpty(model.postalCode))
                        user.postalCode = model.postalCode;
                    if (!string.IsNullOrEmpty(model.cartDriver))
                        user.cartDriver = model.cartDriver;
                    if (!string.IsNullOrEmpty(model.profileImage))
                        user.profileImage = model.profileImage;
                    if (!string.IsNullOrEmpty(model.cartMelliModir))
                        user.cartMelliModir = model.cartMelliModir;
                    if (!string.IsNullOrEmpty(model.RPT))
                        user.rooznameOrParvaneOrTasvir = model.RPT;
                    if (!string.IsNullOrEmpty(model.cartNavgan))
                        user.cartNavgan = model.cartNavgan;
                    if (!string.IsNullOrEmpty(model.cityID))
                        user.cityID = model.cityID;
                    if (!string.IsNullOrEmpty(model.codeMelli))
                        user.codeMelli = model.codeMelli;
                    if (!string.IsNullOrEmpty(model.emPhone))
                        user.emPhone = model.emPhone;
                    if (!string.IsNullOrEmpty(model.name))
                        user.name = model.name;
                    if (!string.IsNullOrEmpty(model.username))
                        user.username = model.username;
                    if (!string.IsNullOrEmpty(model.pelak1))
                        user.pelak1 = model.pelak1;
                    if (!string.IsNullOrEmpty(model.pelak2))
                        user.pelak2 = model.pelak2;
                    if (!string.IsNullOrEmpty(model.pelakHarf))
                        user.pelakHarf = model.pelakHarf;
                    if (!string.IsNullOrEmpty(model.pelakIran))
                        user.pelakIran = model.pelakIran;
                    if (!string.IsNullOrEmpty(model.typeID))
                        user.typeID = model.typeID;
                    if (!string.IsNullOrEmpty(model.coName))
                        user.coName = model.coName;
                    if (!string.IsNullOrEmpty(model.shenaseSherkat))
                        user.shenaseSherkat = model.shenaseSherkat;
                    if (!string.IsNullOrEmpty(model.firebaseToken))
                        user.firebaseToken = model.firebaseToken;



                }
                dbcontext.SaveChanges();
            }

            responseModel mymodel = new responseModel();
            mymodel.status = 200;
            mymodel.message = "ok";
            string result = JsonConvert.SerializeObject(mymodel);

            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public sendUserStatusVM getUserStatus()
        {
            sendUserStatusVM result = new sendUserStatusVM();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());

            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
                if (user != null)
                {
                    if (user.status == "0" || user.status == null)
                    {
                        result.statusCode = "0";
                        result.statusTitle = "تایید نشده";
                        result.message = "لازم است برای احراز قویت اقدام نمایید";
                    }
                    else if (user.status == "1")
                    {
                        result.statusCode = "1";
                        result.statusTitle = "دردست بررسی";
                        result.statusTitle = "مدارک شما در دست بررسی می باشد";
                    }
                    else if (user.status == "2")
                    {
                        result.statusCode = "2";
                        result.statusTitle = "رد شده";
                        result.statusTitle = "به دلیل عدم ارائه مدارک کافی هویت شما احراز نشدهد است";
                    }
                    else if (user.status == "3")
                    {
                        result.statusCode = "3";
                        result.statusTitle = "تایید شده";
                        result.statusTitle = " هویت شما احراز شده است";
                    }
                    result.status = 200;
                }
                else
                {
                    result.status = 400;
                }
            }
            return result;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<sendProfileVM> getProfile()
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            sendProfileVM mymodel = new sendProfileVM();
            try
            {
                using (Context dbcontext = new Context())
                {
                    user user = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == userID);
                    userVM modeluser = new userVM { phone = user.phone, username = user.username };
                    string typestring = "";
                    if (!string.IsNullOrEmpty(user.typeID))
                    {
                        Guid typeguid = new Guid(user.typeID);
                        typestring = dbcontext.cartypes.SingleOrDefault(x => x.typeID == typeguid).title;

                    }

                    string citystring = "";
                    if (!string.IsNullOrEmpty(user.cityID))
                    {
                        Guid cityguid = new Guid(user.cityID);
                        citystring = dbcontext.cities.SingleOrDefault(x => x.userID == cityguid).title;

                    }

                    mymodel.user = modeluser;

                    mymodel.status = 200;
                    mymodel.baseURL = "https://jbar.app/Uploads/";
                    mymodel.city = citystring;
                    mymodel.type = typestring;
                }


                return mymodel;

            }
            catch (Exception e)
            {

                throw;
            }

        }
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public sendCityVM getLocation([FromBody] getCityNameVM model)
        {
            sendCityVM output = new sendCityVM();

            using (Context dbcotext = new Context())
            {
                string url = "https://api.bigdatacloud.net/data/reverse-geocode-client?latitude=" + model.lat + "&longitude=" + model.lon + "&localityLanguage=fa";
                string result = Get(url);
                if (result != "")
                {
                    GeoVM geomodel = JsonConvert.DeserializeObject<GeoVM>(result);
                    List<newcity> afterSearch = (from u in dbcotext.cities
                                                 join p in dbcotext.cities on u.parentID equals p.userID
                                                 where u.userID != u.parentID && (p.title == geomodel.data.First().lat)
                                                 select new newcity { title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList();


                    output.lst = afterSearch;
                    output.status = 200;
                }
                else
                {
                    output.lst = new List<newcity>();
                    output.status = 400;
                }
            }



            return output;
        }


        [System.Web.Http.HttpPost]
        public void setstates()
        {
            using (Context dbcontext = new Context())
            {
                //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [cities]");
                //dbcontext.SaveChanges();
                //List<city> lst = dbcontext.cities.ToList();

                string textFile = "E:\\states.txt";
                if (File.Exists(textFile))
                {
                    // Read a text file line by line.
                    string[] lines = File.ReadAllLines(textFile);
                    foreach (string line in lines)
                    {
                        List<string> line0 = line.Replace("(", "").Replace("),", "").Split('\t').ToList();
                        string code = line0[3].ToString();
                        string title = line0[0];
                        string titleEN = "";
                        string lat = line0[3];
                        string lon = line0[2];
                        string rcode = "";
                        Guid id = Guid.NewGuid();
                        city newcity = new city()
                        {
                            code = rcode,
                            lat = lat,
                            lon = lon,
                            title = title,
                            userID = id,
                            parentID = id,
                        };
                        dbcontext.cities.Add(newcity);
                        dbcontext.SaveChanges();



                    }


                }
            }

        }

        [System.Web.Http.HttpPost]
        public void setcity()
        {
            using (Context dbcontext = new Context())
            {
                //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [orders]");
                //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [ordertypes]");
                //dbcontext.SaveChanges();
                //List<ordertype> lst0 = dbcontext.ordertypes.ToList();
                List<order> lst = dbcontext.orders.ToList();

                Guid tguid = Guid.NewGuid();



                //string textFile = "E:\\cities.txt";
                //if (File.Exists(textFile))
                //{
                //    // Read a text file line by line.
                //    string[] lines = File.ReadAllLines(textFile);

                //    foreach (var city in lst)
                //    {
                //        string cityCode = city.title;
                //        foreach (string line in lines)
                //        {
                //            List<string> line0 = line.Replace("(", "").Replace("),", "").Split('\t').ToList();
                //            string code = line0[0];
                //            if (code == cityCode)
                //            {
                //                string title = line0[1];
                //                string titleEN = "";
                //                string lat = line0[4];
                //                string lon = line0[5];
                //                string rcode = "";
                //                if (lat == "")
                //                {

                //                }
                //                Guid id = Guid.NewGuid();
                //                city newcity = dbcontext.cities.SingleOrDefault(x => x.title == title && x.userID != x.parentID);
                //                if (newcity == null)
                //                {
                //                    newcity = new city()
                //                    {
                //                        code = rcode,
                //                        lat = lat,
                //                        lon = lon,
                //                        title = title,
                //                        userID = id,
                //                        parentID = city.userID,
                //                    };
                //                    dbcontext.cities.Add(newcity);
                //                }
                //                else
                //                {
                //                    newcity.lat = lat;
                //                    newcity.lon = lon;
                //                }




                //            }





                //        }
                //        int index = lst.IndexOf(city);
                //    }



                //}
                //dbcontext.SaveChanges();
            }

        }

        public void setENtitle()
        {
            using (Context dbcontext = new Context())
            {

                List<city> cities = dbcontext.cities.Where(x => x.userID != x.parentID).ToList();
                foreach (var cit in cities)
                {
                    //string url = "https://nominatim.openstreetmap.org/reverse?format=json&lat=" + cit.lat + "&lon=" + cit.lon + "&zoom=18&addressdetails=1";
                    string url = "https://api.bigdatacloud.net/data/reverse-geocode-client?latitude=" + cit.lat + "&longitude=" + cit.lon + "&localityLanguage=fa";
                    string result = Get(url);
                    if (result != "")
                    {
                        //GEoVM2 model = JsonConvert.DeserializeObject<GEoVM2>(result);
                        ////cit.District = model.Address.District != null ? model.Address.District : "";
                        ////cit.country = model.Address.Country != null ? model.Address.Country : "";
                        //cit.cty = model.city != null ? model.city : "";
                        ////cit.town = model.Address.Town != null ? model.Address.Town : "";




                    }
                    int index = cities.IndexOf(cit);

                    //if (cit.title == "کلاله")
                    //{


                    //}


                }


                dbcontext.SaveChanges();

            }
        }

        public void setLoadType()
        {
            using (Context dbcontext = new Context())
            {
                loadType load = new loadType()
                {
                    loadTypeID = Guid.NewGuid(),
                    title = "CSO",
                };
                dbcontext.loadTypes.Add(load);
                load = new loadType()
                {
                    loadTypeID = Guid.NewGuid(),
                    title = "S500",
                };
                dbcontext.loadTypes.Add(load);

                load = new loadType()
                {
                    loadTypeID = Guid.NewGuid(),
                    title = "TP200",
                };
                dbcontext.loadTypes.Add(load);
                load = new loadType()
                {
                    loadTypeID = Guid.NewGuid(),
                    title = "TP400",
                };
                dbcontext.loadTypes.Add(load);

                load = new loadType()
                {
                    loadTypeID = Guid.NewGuid(),
                    title = "آب سردکن انواع",
                };
                dbcontext.loadTypes.Add(load);
                dbcontext.SaveChanges();
            }

        }

        public string Get(string uri)
        {
            String request = "";
            try
            {

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string content = null;

                var wc = new WebClient();
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                  "Windows NT 5.2; .NET CLR 1.0.3705;)");

                Stream stream = wc.OpenRead(uri);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                request = reader.ReadToEnd();

            }
            catch (Exception e)
            {


            }


            return request;
        }

        [System.Web.Http.HttpPost]
        public void getPoly()
        {
            using (Context dbcontext = new Context())
            {
                List<city> lst = dbcontext.cities.Where(x => x.userID != x.parentID && x.citypoint == null).ToList();
                //foreach(var item in lst)
                //{
                //    //item.title = "رستمکلا";
                //    item.title = "چابهار";
                //    item.lat = "25.2935488";
                //    item.lon = "60.6469115";
                //    dbcontext.SaveChanges();
                //}

                foreach (var city in lst)
                {
                    try
                    {
                        DbGeography point = ConvertLatLonToDbGeography(double.Parse(city.lon), double.Parse(city.lat));
                        city.citypoint = point;

                    }
                    catch (Exception)
                    {


                    }


                    //try
                    //{
                    //    string url = "https://nominatim.openstreetmap.org/search.php?city=" + city.title + "&polygon_geojson=1&format=jsonv2";
                    //    string result = Get(url);
                    //    result = "{\'data\': " + result + "}";
                    //    GeoVM model = JsonConvert.DeserializeObject<GeoVM>(result);
                    //    string lat = model.data.First().lat;
                    //    string lon = model.data.First().lon;
                    //    city.lat = lat;
                    //    city.lon = lon;



                    //    string polusrt = "";
                    //    var sb = new StringBuilder();
                    //    foreach (var item in model.data.First().geojson.coordinates)
                    //    {
                    //        string srt = "{\'obb\': " + item.ToString() + "}";
                    //        orrdinates ordiobj = JsonConvert.DeserializeObject<orrdinates>(srt);

                    //        var count = 0;

                    //        sb.Append(@"POLYGON((");
                    //        foreach (var coordinate in ordiobj.obb)
                    //        {
                    //            if (count == 0)
                    //            {
                    //                sb.Append(coordinate[0] + " " + coordinate[1]);
                    //            }
                    //            else
                    //            {
                    //                sb.Append("," + coordinate[0] + " " + coordinate[1]);
                    //            }

                    //            count++;
                    //        }
                    //        sb.Append("," + ordiobj.obb[0][0] + " " + ordiobj.obb[0][1]);
                    //        sb.Append(@"))");

                    //    }

                    //    DbGeography objgeo = DbGeography.PolygonFromText(sb.ToString(), 4326);
                    //    city.poly = objgeo;
                    //    city.lat = lat;
                    //    city.lon = lon;
                    //    dbcontext.SaveChanges();


                    //}
                    //catch (Exception e )
                    //{

                    //    int index = lst.IndexOf(city);
                    //}





                }
                dbcontext.SaveChanges();
            }
        }




        // بخش دوم


        // process
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<processActionVM> getProcess()
        {
            object someObject;
            processActionVM responseModel = new processActionVM();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    var query = from process in dbcontext.processes
                                where process.userID == userID
                                select new processVM
                                {
                                    processID = process.processID,
                                    title = process.title
                                };
                    responseModel.processList = await query.ToListAsync();
                    //responseModel.formulaList = await dbcontext.formulas.Where(x => x.name != "").ToListAsync();
                }
            }
            catch (Exception e)
            {

                throw;
            }


            return responseModel;
        }

        //processFormula
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<processFormulaActionVM> getProcessFormula([FromBody] process model)
        {
            object someObject;
            processFormulaActionVM responseModel = new processFormulaActionVM();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());

            using (Context dbcontext = new Context())
            {

                var query = from process in dbcontext.processes select process;
                responseModel.processFormulaList = await dbcontext.processFormulas.Include(x => x.Process).Include(x => x.Coding).Include(x => x.Formula).Where(x => x.proccessID == model.processID).Select(x => new processFormulaVM { codingType = x.transactionType, codingName = x.Coding.title, formulaName = x.Formula.name, processFormulaID = x.processFormulaID, }).ToListAsync();

                responseModel.FormulaList = await dbcontext.formulas.Where(x => x.name != "" && x.name != null).Select(x => new formulaVM { formulaID = x.formulaID, name = x.name }).ToListAsync();
                responseModel.codingList = await dbcontext.codings.Where(x => x.userID == userID).OrderBy(x => x.codeHesab).Select(x => new codingVM { codingID = x.codingID, title = x.title + "(" + x.codeHesab + ")" }).ToListAsync();

            }

            return responseModel;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setProcessFormula([FromBody] processFormula model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());

            using (Context dbcontext = new Context())
            {
                responseModel mymodel = new responseModel();
                processFormula pf = await dbcontext.processFormulas.SingleOrDefaultAsync(x => x.FormulaID == model.FormulaID && x.proccessID == model.proccessID);

                if (pf == null)
                {
                    dbcontext.processFormulas.Add(new processFormula { codingID = model.codingID, FormulaID = model.FormulaID, proccessID = model.proccessID, transactionType = model.transactionType, processFormulaID = Guid.NewGuid() });
                    await dbcontext.SaveChangesAsync();
                    mymodel.status = 200;
                    mymodel.message = "ok";
                }
                else
                {
                    mymodel.status = 400;
                    mymodel.message = "آیتم مورد نظر وجود دارد";
                }

                string result = JsonConvert.SerializeObject(mymodel);
                JObject jObject = JObject.Parse(result);
                return jObject;
            }
        }



        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setProcess([FromBody] process model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    responseModel mymodel = new responseModel();
                    var process = await dbcontext.processes.FirstOrDefaultAsync(i => i.title == model.title);
                    if (process == null)
                    {
                        dbcontext.processes.Add(new process() { userID = userID, processID = Guid.NewGuid(), title = model.title });
                        await dbcontext.SaveChangesAsync();

                        mymodel.status = 200;
                        mymodel.message = "ok";

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "پروسه هم نام وجود دارد";
                    }

                    string result = JsonConvert.SerializeObject(mymodel);
                    JObject jObject = JObject.Parse(result);
                    return jObject;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        //processForm
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<processFormActionVM> getProcessForm([FromBody] process model)
        {
            object someObject;
            processFormActionVM responseModel = new processFormActionVM();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    var query = from process in dbcontext.processes select process;
                    process q = await dbcontext.processes.Include(x => x.formList).SingleOrDefaultAsync(x => x.processID == model.processID);
                    responseModel.processFormList = q.formList.Select(x => new processFormVM { processFormID = x.formID, title = x.title }).ToList();
                    responseModel.allForm = await dbcontext.forms.Select(x => new processFormVM { processFormID = x.formID, title = x.title }).ToListAsync();

                }
            }
            catch (Exception e)
            {

                throw;
            }


            return responseModel;
        }


        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setNewFormProcess([FromBody] setNewFormProcessVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());

                using (Context dbcontext = new Context())
                {
                    process pr = await dbcontext.processes.Include(x => x.formList).SingleOrDefaultAsync(x => x.processID == model.processID);
                    form frm = await dbcontext.forms.SingleOrDefaultAsync(x => x.formID == model.formID);
                    if (!pr.formList.Contains(frm))
                    {
                        pr.formList.Add(frm);
                        await dbcontext.SaveChangesAsync();
                        mymodel.status = 200;
                        mymodel.message = "";
                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "آیتم تکراری";
                    }



                }


            }
            catch (Exception)
            {
                mymodel.status = 400;
                mymodel.message = "خطا";

            }
            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }
        //formItem



        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<formItemActionVM> getFormItem(form model)
        {
            object someObject;
            formItemActionVM responseModel = new formItemActionVM();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());

            try
            {
                using (Context dbcontext = new Context())
                {

                    var query = dbcontext.formItems.Where(x => x.formID == model.formID).Include(x => x.FormItemDesign).Include(x => x.op).Include(x => x.FormItemType).Select(x => new formItemVM { formID = x.formID, UIName = x.FormItemDesign.title, formItemDesingID = x.FormItemDesign.formItemDesignID, formItemTypeID = x.formItemTypeID, optionSelected = x.OptionID, collectionName = x.op.title, formItemTypeTitle = x.FormItemType.title, itemDesc = x.itemDesc, catchUrl = x.catchUrl, formItemID = x.formItemID, isMultiple = x.isMultiple, itemName = x.itemName, itemPlaceholder = x.itemPlaceholder, itemtImage = "Uploads/" + x.itemtImage, mediaType = x.mediaType });
                    responseModel.formItemList = await query.ToListAsync();

                    var query2 = dbcontext.orderOptions.Where(x => x.userID == userID && x.orderOptionID == x.parentID).Select(x => new orderOptionVM { image = x.image, orderOptionID = x.orderOptionID, title = x.title });
                    responseModel.orderOptionList = await query2.ToListAsync();

                    var query3 = dbcontext.formItemTypes.Select(x => new formItemTypeVM { title = x.title, formItemTypeID = x.formItemTypeID });
                    responseModel.formItemTypeList = await query3.ToListAsync();

                    var query4 = dbcontext.formItemDesigns.Select(x => new formItemDesingVM { title = x.title, formItemDesingID = x.formItemDesignID });
                    responseModel.formItemDesingList = await query4.ToListAsync();
                }

            }
            catch (Exception e)
            {

                throw;
            }

            return responseModel;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setFormItem([FromBody] formItemVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());

                using (Context dbcontext = new Context())
                {

                    if (model.formItemID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {

                        var formItem = await dbcontext.formItems.FirstOrDefaultAsync(i => i.formItemID == model.formItemID);
                        if (string.IsNullOrEmpty(model.itemtImage))
                        {
                            mymodel.message = model.itemtImage;
                        }

                        formItem.itemtImage = model.itemtImage;
                        formItem.itemName = model.itemName;
                        formItem.itemPlaceholder = model.itemPlaceholder;
                        formItem.itemDesc = model.itemDesc;
                        formItem.formItemTypeID = model.formItemTypeID;
                        formItem.OptionID = model.optionSelected;
                        formItem.isMultiple = model.isMultiple;
                        mymodel.status = 200;
                        await dbcontext.SaveChangesAsync();


                    }
                    else
                    {
                        var formItem = await dbcontext.formItems.FirstOrDefaultAsync(i => i.itemName == model.itemName && i.formID == model.formID);
                        if (formItem == null)
                        {

                            formItem fri = new formItem()
                            {
                                catchUrl = model.catchUrl,
                                itemDesc = model.itemDesc,
                                isMultiple = model.isMultiple,
                                formID = model.formID,
                                formItemID = Guid.NewGuid(),
                                formItemTypeID = model.formItemTypeID,
                                itemtImage = model.itemtImage,
                                itemName = model.itemName,
                                mediaType = model.mediaType,
                                itemPlaceholder = model.itemPlaceholder,
                                OptionID = model.optionSelected,
                                formItemDesingID = model.formItemDesingID

                            };


                            dbcontext.formItems.Add(fri);
                            await dbcontext.SaveChangesAsync();

                            mymodel.status = 200;
                            mymodel.message = "ok";

                        }
                        else
                        {
                            mymodel.status = 400;
                            mymodel.message = "پروسه هم نام وجود دارد";
                        }
                    }



                }
            }
            catch (Exception eror)
            {

                throw;
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> removeFormItem([FromBody] formItemVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());
                using (Context dbcontext = new Context())
                {

                    var formItem = await dbcontext.formItems.FirstOrDefaultAsync(i => i.formItemID == model.formItemID);
                    if (formItem != null)
                    {
                        mymodel.message = formItem.itemtImage;
                        dbcontext.formItems.Remove(formItem);
                        await dbcontext.SaveChangesAsync();

                        mymodel.status = 200;


                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "!خطا";
                    }


                }
            }
            catch (Exception eror)
            {

                mymodel.status = 400;
                mymodel.message = "!خطا";
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        // formula
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<formulaActionVM> getFormula(process process)
        {
            object someObject;
            object phone;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Request.Properties.TryGetValue("Userp", out phone);
            Guid userID = new Guid(someObject.ToString());


            formulaActionVM model = new formulaActionVM();
            try
            {
                using (Context dbcontext = new Context())
                {
                    process prc = await dbcontext.processes.Include(x => x.formList).SingleOrDefaultAsync(x => x.processID == process.processID);
                    List<formulaVM> formulalst = new List<formulaVM>();
                    var query = dbcontext.formulas.Where(x => x.userID == userID).Include(x => x.namad).Include(x => x.FormItem).AsQueryable();
                    //List<formula> lst = query.ToList();
                    List<formulaVM> formulas = await query.Select(item => new formulaVM
                    {
                        col = item.col,
                        formulaID = item.formulaID,
                        leftID = item.leftID,
                        name = item.name,
                        number = item.number,
                        result = item.result,
                        rightID = item.rightID,
                        namadName = item.namad == null ? "" : item.namad.value,
                        formItemName = item.FormItem == null ? "" : item.FormItem.itemDesc

                    }).ToListAsync();

                    Guid id1 = prc.formList.FirstOrDefault().formID;
                    Guid id2 = prc.formList.Skip(1).FirstOrDefault().formID;
                    formItemType frt = await dbcontext.formItemTypes.SingleOrDefaultAsync(x => x.title == "آیتم عددی");
                    var query1 = from formItem in dbcontext.formItems where formItem.formItemTypeID == frt.formItemTypeID select new formItemVM { itemDesc = formItem.itemDesc, formItemID = formItem.formItemID };
                    var query2 = from namad in dbcontext.namads select new namadVM { namadID = namad.namadID, title = namad.value };
                    model.formulaList = formulas;
                    model.formItemList = await query1.ToListAsync();
                    model.namadList = await query2.ToListAsync();
                    return model;
                }

            }
            catch (Exception e)
            {

                throw;
            }

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setFormula([FromBody] formula model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());
                model.userID = userID;
                using (Context dbcontext = new Context())
                {

                    var process = await dbcontext.formulas.FirstOrDefaultAsync(i => i.leftID == model.leftID && i.rightID == model.rightID && i.number == model.number && i.namadID == model.namadID && i.formItemID == model.formItemID && i.col == model.col);
                    if (process == null)
                    {
                        var lastprocess = await dbcontext.formulas.OrderByDescending(x => x.col).FirstOrDefaultAsync();
                        int col = lastprocess != null ? lastprocess.col + 1 : 1;

                        var finalLeftFun = await dbcontext.formulas.FirstOrDefaultAsync(x => x.col == model.leftID);
                        var finalRightFun = await dbcontext.formulas.FirstOrDefaultAsync(x => x.col == model.rightID);



                        formItem fi = await dbcontext.formItems.SingleOrDefaultAsync(x => x.formItemID == model.formItemID);
                        namad na = await dbcontext.namads.SingleOrDefaultAsync(x => x.namadID == model.namadID);

                        string fuNamad = HttpUtility.HtmlDecode(na.value);
                        string numberormabna = "";
                        if (fi != null || model.number != 0)
                        {
                            numberormabna = model.number == 0 ? fi.itemDesc : model.number.ToString();

                        }

                        model.result = (model.leftID == 0 && model.rightID == 0) ? "( " + numberormabna + " )" : "( " + finalLeftFun.result + " " + fuNamad + " " + finalRightFun.result + " )";


                        dbcontext.formulas.Add(new formula() { processID = model.processID, name = model.name, userID = userID, formItemID = model.formItemID, number = model.number, formulaID = Guid.NewGuid(), namadID = model.namadID, leftID = model.leftID, rightID = model.rightID, col = col, result = model.result });
                        await dbcontext.SaveChangesAsync();

                        mymodel.status = 200;
                        mymodel.message = "ok";

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "پروسه هم نام وجود دارد";
                    }


                }
            }
            catch (Exception eror)
            {

                throw;
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        // coding
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<List<codingVM>> getCoding()
        {
            object someObject;
            try
            {

                object phone;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Request.Properties.TryGetValue("Userp", out phone);
                Guid userID = new Guid(someObject.ToString());
                formulaActionVM model = new formulaActionVM();
                using (Context dbcontext = new Context())
                {
                    user userItem = await dbcontext.users.Where(x => x.phone == phone.ToString() && x.userType == "0").Include(x => x.Codings).SingleOrDefaultAsync();
                    List<codingVM> lst = new List<codingVM>();
                    foreach (var coding in userItem.Codings)
                    {
                        lst.Add(new codingVM()
                        {
                            codeHesab = coding.codeHesab,
                            codingID = coding.codingID,
                            codingType = coding.codingType,
                            parentID = coding.parentID,
                            title = coding.title,
                            inList = coding.inList

                        });

                    }
                    return lst;

                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setCoding([FromBody] coding model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());
                model.userID = userID;

                if (model.codingID != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    using (Context dbcontext = new Context())
                    {
                        var coding = await dbcontext.codings.FirstOrDefaultAsync(i => i.codingID == model.codingID);
                        coding.inList = model.inList == "on" ? "1" : "0";
                        coding.title = model.title;
                        coding.codeHesab = model.codeHesab;
                        await dbcontext.SaveChangesAsync();
                        mymodel.status = 200;
                        mymodel.message = "ok";


                    }
                }
                else
                {
                    using (Context dbcontext = new Context())
                    {

                        var coding = await dbcontext.codings.FirstOrDefaultAsync(i => i.codeHesab == model.codeHesab);
                        if (coding == null)
                        {

                            coding parentcoding = await dbcontext.codings.FirstOrDefaultAsync(x => x.codingID == model.parentID);

                            int itemCodingType = 1;


                            if (parentcoding != null)
                            {
                                int parentCodingType = parentcoding.codingType;
                                switch (parentCodingType)
                                {

                                    case 1:
                                        itemCodingType = 2;
                                        break;
                                    case 2:
                                        itemCodingType = 3;
                                        break;
                                    case 3:
                                        itemCodingType = 4;
                                        break;
                                    case 4:
                                        itemCodingType = 5;
                                        break;
                                    case 5:
                                        itemCodingType = 6;
                                        break;
                                    case 6:
                                        itemCodingType = 7;
                                        break;
                                    case 7:
                                        itemCodingType = 8;
                                        break;
                                }
                            }

                            dbcontext.codings.Add(new coding() { userID = userID, codingID = Guid.NewGuid(), codingType = itemCodingType, parentID = model.parentID, codeHesab = model.codeHesab, title = model.title });
                            await dbcontext.SaveChangesAsync();

                            mymodel.status = 200;
                            mymodel.message = "ok";

                        }
                        else
                        {
                            mymodel.status = 400;
                            mymodel.message = "پروسه هم نام وجود دارد";
                        }


                    }
                }


            }
            catch (Exception eror)
            {

                throw;
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        //codriver
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getCoDriverResponse> getCoDriverAsync()
        {

            getCoDriverResponse obj = new getCoDriverResponse();
            object someObject;
            object phone;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Request.Properties.TryGetValue("Userp", out phone);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {


                    obj.codrivers = await dbcontext.users.Where(x => x.barbariID == userID).Include(x => x.vehicle).Include(x => x.workingStatus).Include(x => x.verifyStatus).Select(item => new codrivervm
                    {
                        did = item.userID,
                        dname = item.name,
                        plateNumber = item.vehicle != null ? "" : item.vehicle.iran + " ایران " + " " + item.vehicle.pelak2 + " " + item.vehicle.pelakHarf + " " + item.vehicle.pelak1,
                        phone = item.phone,
                        verifyStatus = item.verifyStatus != null ? item.verifyStatus.title : "",
                        workingStatus = item.workingStatus != null ? item.workingStatus.title : ""

                    }).ToListAsync();



                    obj.vehicleList = await dbcontext.vehicles.Include(x => x.vehicleStatus).Where(x => x.vehicleStatus.title == "بدون راننده" && x.userID == userID).Select(p => new vehicleVM { iran = p.iran, pelak1 = p.pelak1, pelak2 = p.pelak2, pelakHarf = p.pelakHarf, vehicleID = p.vehicleID }).ToListAsync();
                    obj.verifyList = await dbcontext.verifyStatuses.Where(x => x.userID == userID).Select(x => new verifyStatusVM { verifyStatusID = x.verifyStatusID, title = x.title }).ToListAsync();
                    obj.worikingstatusList = await dbcontext.userWorkingStatuses.Where(x => x.userID == userID).Select(x => new userWorkingStatusVM { workingStatusID = x.workingStatusID, title = x.title }).ToListAsync();
                    obj.vehicleStatList = await dbcontext.vehicleStatuses.Where(x => x.userID == userID).Select(x => new vehicleSatatusVM { statid = x.vehicleStatusID, title = x.title }).ToListAsync();

                    obj.status = 200;


                }
            }
            catch (Exception e)
            {

                throw;
            }

            return obj;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getCoDriverResponse> getCodriverList([FromBody] coDriverSearchVM model)
        {

            getCoDriverResponse obj = new getCoDriverResponse();
            object someObject;
            object phone;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Request.Properties.TryGetValue("Userp", out phone);
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {


                var coderiverq = dbcontext.users.Where(x => x.barbariID == userID).Include(x => x.vehicle).Include(x => x.workingStatus).Include(x => x.verifyStatus).AsQueryable();

                if (model.verifyStatusID != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    coderiverq = coderiverq.Where(x => x.verifyStatusID == model.verifyStatusID);
                }

                if (model.workingStatusID != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    coderiverq = coderiverq.Where(x => x.workingStatusID == model.workingStatusID);
                }
                if (!string.IsNullOrEmpty(model.query))
                {
                    coderiverq = coderiverq.Where(x => x.name.Contains(model.query) || x.phone.Contains(model.query) || x.username.Contains(model.query));
                }

                obj.codrivers = await coderiverq.Select(item => new codrivervm
                {
                    did = item.userID,
                    dname = item.name,
                    plateNumber = item.vehicle == null ? "" : item.vehicle.iran + " ایران " + " " + item.vehicle.pelak2 + " " + item.vehicle.pelakHarf + " " + item.vehicle.pelak1,
                    phone = item.phone,
                    verifyStatus = item.verifyStatus != null ? item.verifyStatus.title : "",
                    workingStatus = item.workingStatus != null ? item.workingStatus.title : ""

                }).ToListAsync();



                obj.status = 200;


            }
            return obj;
        }


        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> addDriverAsync([FromBody] addDriverVM model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            responseModel mymodel = new responseModel();
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                try
                {
                    user user = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == userID);
                    user codriver = await dbcontext.users.SingleOrDefaultAsync(x => x.phone == model.phone && x.userType == "1");

                    if (codriver != null)
                    {
                        codriver.barbariID = userID;
                        await dbcontext.SaveChangesAsync();
                        mymodel.status = 200;
                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "کاربر مورد نظر عضو سامانه نمی باشد";
                    }
                }
                catch (Exception e)
                {

                    mymodel.status = 400;
                    mymodel.message = "خطا در برقراری ارتباط با سرور";
                }

            }


            string result = JsonConvert.SerializeObject(mymodel);

            JObject jObject = JObject.Parse(result);
            return jObject;
        }


        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> changeUserInfoAsync([FromBody] setVehicleForVM model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            responseModel mymodel = new responseModel();
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                try
                {
                    //
                    user driver = await dbcontext.users.Include(x => x.vehicle).Where(x => x.userID == model.driverID).SingleOrDefaultAsync();

                    if (model.vehicleStatusID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        driver.vehicle.StatusID = model.vehicleStatusID;
                    }
                    if (model.verifStatID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        driver.verifyStatusID = model.verifStatID;
                    }

                    if (model.vehicleID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        // خودرو برای راننده ست شد
                        vehicle selectedVehicle = await dbcontext.vehicles.Where(x => x.vehicleID == model.vehicleID).SingleOrDefaultAsync();
                        selectedVehicle.driverID = driver.userID;


                        // تغییر استتوس خودر
                        List<vehicleStatus> dddys = dbcontext.vehicleStatuses.ToList();
                        vehicleStatus yds = await dbcontext.vehicleStatuses.Where(x => x.title == "در حال استفاده").FirstAsync();
                        vehicle v = await dbcontext.vehicles.Where(x => x.vehicleID == model.vehicleID).SingleOrDefaultAsync();
                        v.StatusID = yds.vehicleStatusID;


                        //ثبت هیستوری تغییرات
                        userVehicle lastvh = await dbcontext.userVehicles.Where(x => x.userID == model.driverID && x.vehicleID == model.vehicleID && x.isFinished == false).SingleOrDefaultAsync();
                        if (lastvh != null)
                        {
                            lastvh.endDate = DateTime.Now;
                            lastvh.isFinished = true;


                        }
                        userVehicle veya = new userVehicle()
                        {
                            isFinished = false,
                            endDate = DateTime.Now,
                            startDate = DateTime.Now,
                            vehicleID = model.vehicleID,
                            userID = model.driverID,
                            userVehicleID = Guid.NewGuid(),

                        };
                        dbcontext.userVehicles.Add(veya);

                    }

                    await dbcontext.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw;
                }



            }
            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        //vehicle
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getVehicleResponce> getVehicleAsync()
        {
            getVehicleResponce obj = new getVehicleResponce();
            try
            {

                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);

                Guid userID = new Guid(someObject.ToString());
                using (Context dbcontext = new Context())
                {
                    var vlist = await dbcontext.vehicles.Include(x => x.yadak).Include(x => x.vehicleStatus).Where(x => x.userID == userID).Select(v => new vehicleVM()
                    {
                        iran = v.iran,
                        pelak1 = v.pelak1,
                        pelak2 = v.pelak2,
                        pelakHarf = v.pelakHarf,
                        vehicleID = v.vehicleID,
                        status = v.vehicleStatus.title,
                        yadakNumber = v.yadak != null ? v.yadak.yadakNumber : "",

                    }).Take(12).ToListAsync();

                    var ylist = await dbcontext.yadaks.Include(x => x.yadakStatus).Where(x => x.userID == userID && x.yadakStatus.title == "بدون کشنده").Select(y => new yadakVM { yadakID = y.yadakID, yadakNumber = y.yadakNumber }).ToListAsync();
                    var slist = await dbcontext.vehicleStatuses.Where(x => x.userID == userID).Select(m => new vehicleSatatusVM { statid = m.vehicleStatusID, title = m.title }).ToListAsync();
                    var yslist = await dbcontext.yadakStatuses.Where(x => x.userID == userID).Select(m => new vehicleSatatusVM { statid = m.yadakStatusID, title = m.title }).ToListAsync();
                    obj.vehicleList = vlist;
                    obj.yadakList = ylist;
                    obj.statList = slist;
                    obj.yadakstatList = yslist;
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return obj;

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getVehicleResponce> getVehicleList([FromBody] vehicleSearchVM model)
        {
            getVehicleResponce obj = new getVehicleResponce();
            try
            {

                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);

                Guid userID = new Guid(someObject.ToString());
                using (Context dbcontext = new Context())
                {
                    var vlistq = dbcontext.vehicles.Include(x => x.yadak).Include(x => x.vehicleStatus).Where(x => x.userID == userID).AsQueryable();

                    if (model.query != null)
                    {
                        vlistq = vlistq.Where(x => x.iran.Contains(model.query) || x.pelak1.Contains(model.query) || x.pelak2.Contains(model.query) || x.pelakHarf.Contains(model.query));
                    }

                    if (model.statusID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        vlistq = vlistq.Where(x => x.StatusID == model.statusID);
                        var ll = vlistq.ToList();
                    }

                    var vlist = await vlistq.Select(v => new vehicleVM()
                    {
                        iran = v.iran,
                        pelak1 = v.pelak1,
                        pelak2 = v.pelak2,
                        pelakHarf = v.pelakHarf,
                        vehicleID = v.vehicleID,
                        status = v.vehicleStatus.title,
                        yadakNumber = v.yadak != null ? v.yadak.yadakNumber : "",

                    }).ToListAsync();
                    obj.vehicleList = vlist;

                }
            }
            catch (Exception e)
            {

                throw;
            }
            return obj;

        }



        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setVehicleAsync([FromBody] vehicle model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            responseModel mymodel = new responseModel();
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                try
                {
                    user user = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == userID);

                    vehicle vehicle = await dbcontext.vehicles.SingleOrDefaultAsync(x => x.userID == model.userID && x.pelak1 == model.pelak1 && x.pelak2 == model.pelak2 && x.pelakHarf == model.pelakHarf && x.iran == model.iran);
                    vehicleStatus st = await dbcontext.vehicleStatuses.FirstAsync();
                    if (vehicle == null)
                    {
                        vehicle vh = new vehicle()
                        {
                            vehicleID = Guid.NewGuid(),
                            iran = model.iran,
                            pelakHarf = model.pelakHarf,
                            pelak2 = model.pelak2,
                            pelak1 = model.pelak1,
                            userID = user.userID,
                            StatusID = st.vehicleStatusID

                        };
                        dbcontext.vehicles.Add(vh);
                        await dbcontext.SaveChangesAsync();
                        mymodel.status = 200;

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "آیتم مورد نظر قبلا ثبت شده است";
                    }
                }
                catch (Exception e)
                {

                    mymodel.status = 400;
                    mymodel.message = "خطا در برقراری ارتباط با سرور";
                }

            }
            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> changeVehicleInfoAsync([FromBody] setYadakForVM model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            responseModel mymodel = new responseModel();
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                try
                {
                    vehicle vh = await dbcontext.vehicles.SingleOrDefaultAsync(x => x.vehicleID == model.vehicleID);
                    if (model.yadakStatusID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {

                        vh.yadak.StatusID = model.yadakStatusID;
                        vh.yadakID = null;

                    }

                    if (model.statusID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {

                        vh.StatusID = model.statusID;

                    }
                    if (model.yadakID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        // تغییر یدک برای خودرو و ثبت تغییرات قبلی 
                        //vh.yadakID = model.yadakID;
                        yadak yd = await dbcontext.yadaks.SingleOrDefaultAsync(x => x.yadakID == model.yadakID);
                        if (yd != null)
                        {
                            yd.vehicleID = vh.vehicleID;
                        }
                        yadakStatus yds = await dbcontext.yadakStatuses.Where(x => x.title == "عملیاتی").FirstAsync();
                        yd.StatusID = yds.yadakStatusID;

                        yadakVehicle lastvh = await dbcontext.yadakVehicles.Where(x => x.yadakID == model.yadakID && x.vehicleID == model.vehicleID && x.isFinished == false).SingleOrDefaultAsync();
                        if (lastvh != null)
                        {
                            lastvh.endDate = DateTime.Now;
                            lastvh.isFinished = true;
                        }
                        yadakVehicle veya = new yadakVehicle()
                        {
                            isFinished = false,
                            endDate = DateTime.Now,
                            startDate = DateTime.Now,
                            vehicleID = model.vehicleID,
                            yadakID = model.yadakID,
                            yadakVehicleID = Guid.NewGuid(),

                        };
                        dbcontext.yadakVehicles.Add(veya);

                    }



                    await dbcontext.SaveChangesAsync();
                }
                catch (Exception e)
                {

                    throw;
                }



            }
            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        //yadak
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getYadakResponce> getYadakAsync()
        {
            getYadakResponce obj = new getYadakResponce();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            using (Context dbcontext = new Context())
            {

                Guid userID = new Guid(someObject.ToString());
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);

                obj.yadakList = await dbcontext.yadaks.Include(x => x.Cartype).Include(x => x.yadakStatus).Select(v => new yadakVM { loadtype = v.Cartype.title, status = v.yadakStatus.title, yadakID = v.yadakID, yadakNumber = v.yadakNumber }).ToListAsync();
                obj.typeList = await (from l in dbcontext.cartypes select new cartypeVM { parentID = l.parentID, title = l.title, typeID = l.typeID }).ToListAsync();
                obj.slist = await dbcontext.yadakStatuses.Where(x => x.userID == userID).Select(m => new vehicleSatatusVM { statid = m.yadakStatusID, title = m.title }).ToListAsync();

            }
            return obj;

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<getYadakResponce> getYadakList(yadakSearchVM model)
        {
            getYadakResponce obj = new getYadakResponce();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            using (Context dbcontext = new Context())
            {

                Guid userID = new Guid(someObject.ToString());

                var ylistq = dbcontext.yadaks.Include(x => x.Cartype).Include(x => x.yadakStatus).AsQueryable();

                if (model.query != null)
                {
                    ylistq = ylistq.Where(x => x.yadakNumber.Contains(model.query));
                }

                if (model.statusID != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    ylistq = ylistq.Where(x => x.StatusID == model.statusID);
                }
                var vlist = await ylistq.Select(v => new yadakVM { loadtype = v.Cartype.title, status = v.yadakStatus.title, yadakID = v.yadakID, yadakNumber = v.yadakNumber }).ToListAsync();
                obj.yadakList = vlist;
            }


            return obj;

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setYadakAsync([FromBody] yadak model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            responseModel mymodel = new responseModel();
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                try
                {
                    user user = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == userID);

                    yadak yadak = await dbcontext.yadaks.SingleOrDefaultAsync(x => x.userID == model.userID && x.yadakNumber == model.yadakNumber && x.typeID == model.typeID);
                    yadakStatus st = await dbcontext.yadakStatuses.FirstAsync();
                    if (yadak == null)
                    {
                        yadak vh = new yadak()
                        {
                            yadakID = Guid.NewGuid(),
                            typeID = model.typeID,
                            yadakNumber = model.yadakNumber,
                            userID = userID,
                            StatusID = st.yadakStatusID



                        };
                        dbcontext.yadaks.Add(vh);
                        await dbcontext.SaveChangesAsync();
                        mymodel.status = 200;

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "آیتم مورد نظر قبلا ثبت شده است";
                    }
                }
                catch (Exception e)
                {

                    mymodel.status = 400;
                    mymodel.message = "خطا در برقراری ارتباط با سرور";
                }

            }
            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> changeYadakInfoAsync([FromBody] setYadakForVM model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            responseModel mymodel = new responseModel();
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                try
                {

                    if (model.statusID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        yadak vh = await dbcontext.yadaks.Where(x => x.yadakID == model.yadakID).SingleOrDefaultAsync();

                        vh.StatusID = model.statusID;
                    }
                    await dbcontext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw;
                }



            }
            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }


        // products


        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<productActionVM> getProductsAsync()
        {
            object someObject;
            try
            {

                object phone;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Request.Properties.TryGetValue("Userp", out phone);
                Guid userID = new Guid(someObject.ToString());
                productActionVM model = new productActionVM();
                using (Context dbcontext = new Context())
                {
                    var products = await dbcontext.products.Where(x => x.userID == userID).Include(x=>x.productTypes).Include(x => x.Tags).ToListAsync();//.Include(x => x.productType)
                    List<productVM> finallst = new List<productVM>();
                    foreach (var x in products)
                    {
                        productVM vmproducts = new productVM()
                        {
                            address = x.address,
                            code = x.code,
                            productID = x.productID,
                            title = x.title,
                            //productTypesrt = x.productType.title,
                            tagsrt = String.Join(", ", x.Tags.Select(t => t.title)),
                             productTypesrt = String.Join(", ", x.productTypes.Select(t => t.title)),

                        };
                        finallst.Add(vmproducts);

                    };
                    model.productList = finallst;
                    model.productTypeList = await dbcontext.productTypes.Select(x => new productTypeVM { productTypeID = x.productTypeID, title = x.title, parentID = x.parentID }).ToListAsync();
                    model.taglist = await dbcontext.tags.Where(x => x.userID == userID).Select(x => new tagVM { tagID = x.tagID, title = x.title }).ToListAsync();
                    return model;

                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setProductAsync([FromBody] addProductVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());
                using (Context dbcontext = new Context())
                {

                    var product = await dbcontext.products.FirstOrDefaultAsync(x => x.title == model.title && x.code == model.code && x.barcode == model.barcode);
                    if (product == null)
                    {
                        Guid productID = Guid.NewGuid();
                        product myproduct = new product() { userID = userID, productID = productID, address = model.address, barcode = model.barcode, code = model.code, title = model.title };
                        dbcontext.products.Add(myproduct);
                        await dbcontext.SaveChangesAsync();
                        product selectedproduct = await dbcontext.products.Include(x=>x.productTypes).Include(x=>x.Tags).SingleOrDefaultAsync(x => x.productID == productID);
                        foreach (var item in model.productTypeID)
                        {
                            productType prt = await dbcontext.productTypes.SingleOrDefaultAsync(x => x.productTypeID == item);
                            selectedproduct.productTypes.Add(prt);


                        }
                        foreach (var item in model.produtTagID)
                        {
                            tag prt = await dbcontext.tags.SingleOrDefaultAsync(x => x.tagID == item);
                            selectedproduct.Tags.Add(prt);

                        }
                        
                        await dbcontext.SaveChangesAsync();

                        mymodel.status = 200;
                        mymodel.message = "ok";

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "پروسه هم نام وجود دارد";
                    }


                }
            }
            catch (Exception eror)
            {

                throw;
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> removeProductAsync([FromBody] addProductVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());
                using (Context dbcontext = new Context())
                {

                    var product = await dbcontext.products.FirstOrDefaultAsync(x => x.productID == model.productID);
                    if (product != null)
                    {
                        dbcontext.products.Remove(product);

                        await dbcontext.SaveChangesAsync();

                        mymodel.status = 200;
                        mymodel.message = "ok";

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "پروسه هم نام وجود دارد";
                    }


                }
            }
            catch (Exception eror)
            {

                mymodel.status = 400;
                mymodel.message = "امکان حذف محصول وجود ندارد";
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }


        //productType
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<productTypeActionVM> getProductTypeAsync()
        {
            object someObject;
            try
            {

                object phone;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Request.Properties.TryGetValue("Userp", out phone);
                Guid userID = new Guid(someObject.ToString());
                productTypeActionVM model = new productTypeActionVM();
                using (Context dbcontext = new Context())
                {
                    var products = await dbcontext.productTypes.Where(x => x.userID == userID).Where(x => x.productTypeID == x.parentID).Select(x => new productTypeVM
                    {
                        parentID = x.parentID,
                        productTypeID = x.productTypeID,
                        title = x.title

                    }).ToListAsync();
                    model.prtlist = products;
                    return model;

                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> addProductTypeAsync([FromBody] addProductTypeVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());

                using (Context dbcontext = new Context())
                {

                    var coding = await dbcontext.productTypes.FirstOrDefaultAsync(i => i.title == model.title);
                    if (coding == null)
                    {
                        Guid ptid = Guid.NewGuid();
                        Guid parenttid = ptid;


                        productType pr = new productType()
                        {
                            productTypeID = ptid,
                            parentID = parenttid,
                            title = model.title,
                            userID = userID,
                        };
                        dbcontext.productTypes.Add(pr);
                        await dbcontext.SaveChangesAsync();

                        if (model.parentID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            productType pp = await dbcontext.productTypes.SingleOrDefaultAsync(x => x.productTypeID == ptid);
                            pp.parentID = model.parentID;
                            await dbcontext.SaveChangesAsync();
                        }

                        mymodel.status = 200;
                        mymodel.message = "ok";

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "پروسه هم نام وجود دارد";
                    }


                }
            }
            catch (Exception eror)
            {

                throw;
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }


        //tag
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<productActionVM> getTagAsync()
        {
            object someObject;
            try
            {

                object phone;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Request.Properties.TryGetValue("Userp", out phone);
                Guid userID = new Guid(someObject.ToString());
                productActionVM model = new productActionVM();
                using (Context dbcontext = new Context())
                {
                    var products = await dbcontext.products.Where(x => x.userID == userID).Select(x => new productVM
                    {
                        address = x.address,
                        code = x.code,
                        productID = x.productID,
                        title = x.title
                    }).ToListAsync();
                    model.productList = products;
                    return model;

                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setTag([FromBody] tagVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());

                using (Context dbcontext = new Context())
                {

                    var coding = await dbcontext.tags.FirstOrDefaultAsync(i => i.title == model.title);
                    if (coding == null)
                    {
                        dbcontext.tags.Add(new tag() { userID = userID, tagID = Guid.NewGuid(), title = model.title });
                        await dbcontext.SaveChangesAsync();

                        mymodel.status = 200;
                        mymodel.message = "ok";

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "پروسه هم نام وجود دارد";
                    }


                }
            }
            catch (Exception eror)
            {

                throw;
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

        // orderOption
        //productType
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<orderOptionActionVM> getOrderOptionAsync()
        {
            object someObject;
            try
            {

                object phone;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Request.Properties.TryGetValue("Userp", out phone);
                Guid userID = new Guid(someObject.ToString());
                orderOptionActionVM model = new orderOptionActionVM();
                using (Context dbcontext = new Context())
                {
                    var orderOption = await dbcontext.orderOptions.Where(x => x.userID == userID).Select(x => new orderOptionVM
                    {
                        parentID = x.parentID,
                        orderOptionID = x.orderOptionID,
                        title = x.title,
                        image = "Uploads/" + x.image

                    }).ToListAsync();
                    model.prtlist = orderOption;
                    return model;

                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> addOrderOptionAsync([FromBody] addOrderOptionVM model)
        {
            responseModel mymodel = new responseModel();
            try
            {
                object someObject;
                Request.Properties.TryGetValue("UserToken", out someObject);
                Guid userID = new Guid(someObject.ToString());


                using (Context dbcontext = new Context())
                {

                    if (model.orderOptionID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {

                        orderOption selecteOption = await dbcontext.orderOptions.FirstOrDefaultAsync(x => x.orderOptionID == model.orderOptionID);
                        mymodel.message = selecteOption.image;
                        selecteOption.image = model.image;
                        mymodel.status = 200;
                        await dbcontext.SaveChangesAsync();


                    }
                    else
                    {
                        var orop = await dbcontext.orderOptions.FirstOrDefaultAsync(i => i.title == model.title);
                        if (orop == null)
                        {
                            Guid ptid = Guid.NewGuid();
                            Guid parenttid = ptid;



                            orderOption pr = new orderOption()
                            {
                                orderOptionID = ptid,
                                parentID = parenttid,
                                title = model.title,
                                userID = userID,
                                image = model.image
                            };
                            dbcontext.orderOptions.Add(pr);
                            await dbcontext.SaveChangesAsync();

                            if (model.parentID != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                orderOption pp = await dbcontext.orderOptions.SingleOrDefaultAsync(x => x.orderOptionID == ptid);
                                pp.parentID = model.parentID;
                                await dbcontext.SaveChangesAsync();
                            }

                            mymodel.status = 200;
                            mymodel.message = "ok";

                        }
                        else
                        {
                            mymodel.status = 400;
                            mymodel.message = "پروسه هم نام وجود دارد";
                        }
                    }



                }
            }
            catch (Exception eror)
            {

                throw;
            }

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }
        // form
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<processFormActionVM> getForm()
        {
            object someObject;
            processFormActionVM responseModel = new processFormActionVM();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    var query = from process in dbcontext.processes select process;
                    List<form> forlist = dbcontext.forms.ToList();
                    responseModel.processFormList = await dbcontext.forms.Select(x => new processFormVM { title = x.title, processFormID = x.formID }).ToListAsync();
                }
            }
            catch (Exception e)
            {
                string srt = e.InnerException.Message;
                throw e;
            }


            return responseModel;
        }
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setForm([FromBody] form model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    responseModel mymodel = new responseModel();
                    var form = await dbcontext.forms.FirstOrDefaultAsync(i => i.title == model.title);
                    if (form == null)
                    {
                        dbcontext.forms.Add(new form() { userID = userID, formID = Guid.NewGuid(), title = model.title });
                        await dbcontext.SaveChangesAsync();

                        mymodel.status = 200;
                        mymodel.message = "ok";

                    }
                    else
                    {
                        mymodel.status = 400;
                        mymodel.message = "پروسه هم نام وجود دارد";
                    }

                    string result = JsonConvert.SerializeObject(mymodel);
                    JObject jObject = JObject.Parse(result);
                    return jObject;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }


        // formDetailAPI
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<responseModel> setFormDetail([FromBody] setFormDetailVM model)
        {
            object someObject;
            responseModel responseModel = new responseModel();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());

            try
            {
                using (Context dbcontext = new Context())
                {


                    //form formModel = await dbcontext.forms.SingleOrDefaultAsync(x => x.formID == model.formID);

                    if (model.orderID != null)
                    {
                        if (model.orderID == new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            // یک نیو اردر ایجاد میکنیم

                            newOrderStatus neworderstatus = await dbcontext.newOrderStatuses.SingleOrDefaultAsync(x => x.statusCode == "1");
                            //Guid thirdPartyGUID = Guid.NewGuid();
                            //thirdParty thirdParty = new thirdParty()
                            //{
                            //    fullname = model.formDetailList.SingleOrDefault(x => x.key == "thirdPartyName").value,
                            //    address = model.formDetailList.SingleOrDefault(x => x.key == "thirdPartyAddress").value,
                            //    phone = model.formDetailList.SingleOrDefault(x => x.key == "thirdPartyPhone").value,
                            //    ThirdPartyID = thirdPartyGUID,
                            //};
                            //dbcontext.thirdParties.Add(thirdParty);


                            model.orderID = Guid.NewGuid();
                            newOrder neworder = new newOrder()
                            {
                                creationDate = DateTime.Now,
                                terminationDate = DateTime.Now,
                                newOrderID = model.orderID,
                                newOrderStatusID = neworderstatus.newOrderStatusID,
                                orderName = model.name
                                //thirdPartyID = thirdPartyGUID,
                            };
                            newOrder checkorder = await dbcontext.NewOrders.SingleOrDefaultAsync(x => x.orderName == model.name);
                            if (checkorder == null)
                            {
                                dbcontext.NewOrders.Add(neworder);
                            }
                            else
                            {
                                responseModel.status = 400;
                                responseModel.message = "سفارش تکراری است";
                                return responseModel;
                            }


                            await dbcontext.SaveChangesAsync();
                        }
                    }// 



                    newOrder orderSelected = await dbcontext.NewOrders.SingleOrDefaultAsync(x => x.newOrderID == model.orderID);
                    newOrderStatus stat = await dbcontext.newOrderStatuses.SingleOrDefaultAsync(x => x.statusCode == "1");
                    orderSelected.newOrderStatusID = stat.newOrderStatusID;

                    if ( model.processID != null ) //
                    {
                        if (model.processID == new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            process pr = await dbcontext.processes.Include(x => x.formList).Include(x => x.formList.Select(t => t.FormItems)).SingleOrDefaultAsync(x => x.isDefault == "1");
                            model.processID = pr.processID;
                        }

                    }
                    Guid newFlowID = Guid.NewGuid();
                    if (model.flowID != null) //
                    {
                        if (model.flowID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            newOrderFlow newOrderFlow = new newOrderFlow()
                            {
                                creationDate = DateTime.Now,

                                processID = model.processID,
                                isFinished = "1",
                                newOrderID = model.orderID,
                                newOrderFlowID = newFlowID,
                                ServentID = userID,
                                terminationDate = DateTime.Now,
                            };
                            dbcontext.newOrderFlows.Add(newOrderFlow);
                        }
                       
                    }
                    else
                    {
                        newOrderFlow selectedflow = await dbcontext.newOrderFlows.FirstOrDefaultAsync(x => x.newOrderFlowID == model.flowID);
                        selectedflow.isFinished = "1";
                        newFlowID = model.flowID;
                    }

                    await dbcontext.SaveChangesAsync();
                    List<detailCollection> lstform = JsonConvert.DeserializeObject<List<detailCollection>>(model.formDetailList);
                    foreach (var item in lstform)
                    {
                        string fieldToGo = "";
                        switch (item.formItemTypeCode)
                        {
                            case ("1"): // انتخابی
                                fieldToGo = "valueBool";

                                break;
                            case ("2"): // موقعیت 
                                fieldToGo = "valueString";
                                break;
                            case ("3"):// آپلود
                                fieldToGo = "valueString";
                                break;
                            case ("4"):// چند گزینه ای
                                fieldToGo = "valueGuid";
                                break;
                            case ("5"): // تاریخ
                                fieldToGo = "valueDateTime";
                                break;
                            case ("6"): // عددی
                                fieldToGo = "valueDuoble";
                                break;
                            case ("7"): // متنی
                                fieldToGo = "valueString";
                                break;
                        }
                        newOrderFields fieldItem = new newOrderFields();
                        fieldItem.formItemID = item.formItemID;
                        fieldItem.name = item.key;
                        fieldItem.newOrderFieldsID = Guid.NewGuid();
                        fieldItem.newOrderFlowID = newFlowID;
                        fieldItem.usedFeild = fieldToGo;
                        fieldItem.valueInt = 0;
                        fieldItem.valueDuoble = 0;
                        fieldItem.valueDateTime = DateTime.Now;
                        fieldItem.valueBool = false;
                        fieldItem.valueGuid = new Guid();

                        if (fieldToGo == "valueBool")
                            fieldItem.valueBool = Boolean.Parse(item.value);
                        if (fieldToGo == "valueString")
                            fieldItem.valueString = item.value;
                        if (fieldToGo == "valueDateTime")
                            fieldItem.valueDateTime = DateTime.Parse(item.value);
                        if (fieldToGo == "valueGuid")
                            fieldItem.valueGuid = new Guid(item.value);
                        if (fieldToGo == "valueDuoble")
                            fieldItem.valueDuoble = double.Parse(item.value);


                        dbcontext.newOrderFields.Add(fieldItem);

                    }

                    await dbcontext.SaveChangesAsync();

                    responseModel.status = 200;
                    responseModel.message = "";



                }
            }
            catch (Exception e)
            {

                responseModel.status = 400;
                responseModel.message = e.InnerException.Message;
                return responseModel;
            }

            return responseModel;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<listOfFormVM> formFullDetailInsert([FromBody] formFullDetailRequest model)
        {
            object someObject;
            formFullDetailVM responseModel = new formFullDetailVM();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            listOfFormVM response = new listOfFormVM();
            List<formItemList> formList = new List<formItemList>();
            try
            {
                using (Context dbcontext = new Context())
                {

                    process process = new process();

                    if (model != null)
                    {
                        if (model.processID == new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            process = await dbcontext.processes.Include(x => x.formList).Include(x => x.formList.Select(t => t.FormItems)).SingleOrDefaultAsync(x => x.isDefault == "1");
                        }

                        else
                        {
                            process = await dbcontext.processes.Include(x => x.formList).Include(x => x.formList.Select(t => t.FormItems)).SingleOrDefaultAsync(x => x.processID == model.processID);
                        }
                    }
                    else
                    {
                        process = await dbcontext.processes.Include(x => x.formList).Include(x => x.formList.Select(t => t.FormItems)).SingleOrDefaultAsync(x => x.isDefault == "1");


                    }





                    foreach (var form in process.formList)
                    {
                        formItemList fil = new formItemList();
                        fil.formID = form.formID;
                        fil.formTitle = form.title;
                        fil.formItemDetailList = await dbcontext.formItems.Where(x => x.formID == form.formID).Include(x => x.FormItemDesign).Include(x => x.op).Include(x => x.op.childList).Include(x => x.FormItemType).Select(x => new formFullDetailItemVM { formItemTypeCode = x.FormItemType.formItemTypeCode, orderOptions = x.op.childList.Where(x => x.parentID != x.orderOptionID).Select(t => new orderOptionVM { parentID = t.parentID, image = t.image, orderOptionID = t.orderOptionID, title = t.title }).ToList(), UIName = x.FormItemDesign.title, formItemDesingID = x.FormItemDesign.formItemDesignID, formItemTypeID = x.formItemTypeID, optionSelected = x.OptionID, collectionName = x.op.title, formItemTypeTitle = x.FormItemType.title, itemDesc = x.itemDesc, catchUrl = x.catchUrl, formItemID = x.formItemID, isMultiple = x.isMultiple, itemName = x.itemName, itemPlaceholder = x.itemPlaceholder, itemtImage = "Uploads/" + x.itemtImage, mediaType = x.mediaType }).ToListAsync();
                        formList.Add(fil);
                    }

                    response.allForm = formList;

                }

            }
            catch (Exception e)
            {

                throw;
            }

            return response;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<listOfFormVM> formFullDetailView([FromBody] formFullDetailRequest model)
        {
            object someObject;
            formFullDetailVM responseModel = new formFullDetailVM();

            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            listOfFormVM response = new listOfFormVM();
            List<formItemList> formList = new List<formItemList>();
            try
            {
                using (Context dbcontext = new Context())
                {
                    List<newOrderFields> insertedFields = await dbcontext.newOrderFields.Where(x => x.newOrderFlowID == model.flowID).ToListAsync();
                    process process = new process();

                    if (model == null)
                    {
                        process = await dbcontext.processes.Include(x => x.formList).Include(x => x.formList.Select(t => t.FormItems)).SingleOrDefaultAsync(x => x.isDefault == "1");
                    }
                    else
                    {
                        process = await dbcontext.processes.Include(x => x.formList).Include(x => x.formList.Select(t => t.FormItems)).SingleOrDefaultAsync(x => x.processID == model.processID);
                    }



                    foreach (var form in process.formList)
                    {



                        formItemList fil = new formItemList();
                        fil.formID = form.formID;
                        fil.formTitle = form.title;
                        fil.formItemDetailList = await dbcontext.formItems.Where(x => x.formID == form.formID).Include(x => x.FormItemDesign).Include(x => x.op).Include(x => x.op.childList).Include(x => x.FormItemType).Select(x => new formFullDetailItemVM { orderOptions = x.op.childList.Where(x => x.parentID != x.orderOptionID).Select(t => new orderOptionVM { parentID = t.parentID, image = t.image, orderOptionID = t.orderOptionID, title = t.title }).ToList(), UIName = x.FormItemDesign.title, formItemDesingID = x.FormItemDesign.formItemDesignID, formItemTypeID = x.formItemTypeID, optionSelected = x.OptionID, collectionName = x.op.title, formItemTypeCode = x.FormItemType.formItemTypeCode, formItemTypeTitle = x.FormItemType.title, itemDesc = x.itemDesc, catchUrl = x.catchUrl, formItemID = x.formItemID, isMultiple = x.isMultiple, itemName = x.itemName, itemPlaceholder = x.itemPlaceholder, itemtImage = "Uploads/" + x.itemtImage, mediaType = x.mediaType }).ToListAsync();
                        foreach (var item in fil.formItemDetailList)
                        {
                            newOrderFields filed = insertedFields.SingleOrDefault(x => x.formItemID == item.formItemID);
                            var nameOfProperty = filed.usedFeild;
                            var propertyInfo = filed.GetType().GetProperty(nameOfProperty);
                            var value = propertyInfo.GetValue(filed, null);
                            item.itemValue = value.ToString();
                        }
                        formList.Add(fil);
                    }

                    response.allForm = formList;

                }

            }
            catch (Exception e)
            {

                throw;
            }

            return response;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<userFlowVM> getUserFlow()
        {
            object someObject;
            formFullDetailVM responseModel = new formFullDetailVM();
            userFlowVM response = new userFlowVM();
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    response.flowList = await dbcontext.newOrderFlows.Where(x => x.ServentID == userID).Include(x => x.NewOrder).Include(x => x.newOrderProcess).Select(x => new orderFlowVM { processID = x.newOrderProcess.processID, orderID = x.NewOrder.newOrderID, isFinished = x.isFinished, flowID = x.newOrderFlowID, processName = x.newOrderProcess.title, orderName = x.NewOrder.orderName }).ToListAsync();
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return response;
        }
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<userFlowVM> getOrderFlow([FromBody] newOrder model)
        {
            object someObject;
            formFullDetailVM responseModel = new formFullDetailVM();
            userFlowVM response = new userFlowVM();
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    response.flowList = await dbcontext.newOrderFlows.Where(x => x.newOrderID == model.newOrderID).Include(x => x.NewOrder).Include(x => x.newOrderProcess).Select(x => new orderFlowVM { processID = x.newOrderProcess.processID, orderID = x.NewOrder.newOrderID, isFinished = x.isFinished, flowID = x.newOrderFlowID, processName = x.newOrderProcess.title, orderName = x.NewOrder.orderName }).ToListAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return response;
        }
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<flowCostVM> getFlowCost([FromBody] getFlowCost model)
        {
            object someObject;
            formFullDetailVM responseModel = new formFullDetailVM();
            flowCostVM response = new flowCostVM();
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
               
                using (Context dbcontext = new Context())
                {
                    response.codingList = await dbcontext.codings.Where(x => x.inList == "1").Select(x => new codingVM { title = x.title, codingID = x.codingID }).ToListAsync();
                    response.productList = await dbcontext.products.Select(x => new productVM { title = x.title, productID = x.productID }).ToListAsync();
                    response.flowCodingList = await dbcontext.flowCodings.Where(x=>x.flowID == model.flowID).Include(x=>x.coding).Select(x => new flowCodingVM {  amount = x.amount,  codingTitle = x.coding.title }).ToListAsync();
                    response.flowProductList = await dbcontext.flowProducts.Where(x=>x.flowID == model.flowID).Include(x=>x.product).Select(x => new flowProductVM {  amount = x.amount,  productTitle = x.product.title }).ToListAsync();

                    return response;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<responseModel> setFlowCoding([FromBody] setFlowCodingVM model)
        {
            object someObject;
            responseModel responseModel = new responseModel();
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    flowCoding flc = new flowCoding()
                    {
                        amount = model.amount,
                        CodingID = model.codingID,
                        flowID = model.flowID,
                        flowCodingID = Guid.NewGuid(),
                        date = DateTime.Now,
                    };
                    dbcontext.flowCodings.Add(flc);
                    await dbcontext.SaveChangesAsync();
                    responseModel.status = 200;
                    responseModel.message = "";
                }
            }
            catch (Exception)
            {
                responseModel.status = 400;
                responseModel.message = "";
                throw;
            }
            return responseModel;
        }
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<responseModel> setFlowProduct([FromBody] setFlowProductVM model)
        {
            object someObject;
            responseModel responseModel = new responseModel();
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            try
            {
                using (Context dbcontext = new Context())
                {
                    flowProduct flc = new flowProduct()
                    {
                        amount = model.amount,
                        productID = model.productID,
                        flowID = model.flowID,
                        flowCodingID = Guid.NewGuid(),
                        date = DateTime.Now,
                    };
                    dbcontext.flowProducts.Add(flc);
                    await dbcontext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return responseModel;
        }
        // manger 
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<newOrderSearchVM> getNewOrderSearch()
        {
            object someObject;

            userFlowVM response = new userFlowVM();
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            newOrderSearchVM model = new newOrderSearchVM();
            try
            {
                using (Context dbcontext = new Context())
                {
                    model.processList = await dbcontext.processes.Select(x => new processVM { processID = x.processID, title = x.title }).ToListAsync();
                    model.statusList = await dbcontext.newOrderStatuses.Select(x => new newOrderStatusVM { orderStatusID = x.newOrderStatusID, orderStatusTitle = x.title }).ToListAsync();

                    model.codrivers = await dbcontext.users.Where(x => x.barbariID == userID).Include(x => x.vehicle).Include(x => x.workingStatus).Include(x => x.verifyStatus).Select(item => new codrivervm
                    {
                        did = item.userID,
                        dname = item.name,
                        //plateNumber = item.vehicle != null ? "" : item.vehicle.iran + " ایران " + " " + item.vehicle.pelak2 + " " + item.vehicle.pelakHarf + " " + item.vehicle.pelak1,
                        phone = item.phone,
                        verifyStatus = item.verifyStatus != null ? item.verifyStatus.title : "",
                        workingStatus = item.workingStatus != null ? item.workingStatus.title : ""

                    }).ToListAsync();
                    return model;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<newOrderList> getManagerOrder([FromBody] manageOrderVM model)
        {

            try
            {
                newOrderList responseModel = new newOrderList();
                using (Context dbcontext = new Context())
                {

                    var q = dbcontext.NewOrders.Include(x => x.newOrderStatus).Include(x => x.ThirdParty).AsQueryable();
                    if (model != null)
                    {
                        if (model.processID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {

                            List<Guid> orderlist = await dbcontext.newOrderFlows.Where(x => x.isFinished != "1" && x.processID == model.processID).Select(x => x.newOrderID).ToListAsync();
                            q = q.Where(x => orderlist.Contains(x.newOrderID));
                        }
                        if (model.orderStatusID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            q = q.Where(x => x.newOrderStatusID == model.orderStatusID);
                        }
                    }

                    responseModel.list = await q.Select(x => new newOrderVM { newOrderStatusCode = x.newOrderStatus.statusCode, creationDate = x.creationDate, newOrderID = x.newOrderID, orderName = x.orderName, terminationDate = x.terminationDate, thirdPartyTitle = x.ThirdParty == null ? "" : x.ThirdParty.fullname, newOrderStatusTitle = x.newOrderStatus == null ? "" : x.newOrderStatus.title }).ToListAsync();
                    return responseModel;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<responseModel> setNewFlow([FromBody] newFlowVM model)
        {
            responseModel response = new responseModel();
            try
            {
                using (Context dbcontext = new Context())
                {
                    Guid newFlowID = Guid.NewGuid();
                    newOrderFlow newOrderFlow = new newOrderFlow()
                    {
                        creationDate = DateTime.Now,
                        processID = model.processID,
                        isFinished = "0",
                        newOrderID = model.orderID,
                        newOrderFlowID = newFlowID,
                        ServentID = model.serventID,
                        terminationDate = DateTime.Now,
                    };
                    dbcontext.newOrderFlows.Add(newOrderFlow);
                    await dbcontext.SaveChangesAsync();

                    newOrder order = await dbcontext.NewOrders.SingleOrDefaultAsync(x => x.newOrderID == model.orderID);
                    newOrderStatus stat = await dbcontext.newOrderStatuses.SingleOrDefaultAsync(x => x.statusCode == "2");

                    order.newOrderStatusID = stat.newOrderStatusID;

                    user servent = await dbcontext.users.SingleOrDefaultAsync(x => x.userID == model.serventID && x.userType == "1");
                    Guid ustatuid = new Guid("569bb370-b49d-4713-883f-1a3750f92978");// عملیاتی
                    servent.workingStatusID = ustatuid;

                    await dbcontext.SaveChangesAsync();

                    response.status = 200;
                    response.message = "";

                }

            }
            catch (Exception)
            {
                response.status = 400;
                response.message = "خطا";


            }
            return response;
        }

    }
}