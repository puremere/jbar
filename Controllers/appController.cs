﻿using jbar.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
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
using System.Net.Http.Headers;

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
        public JObject changeUserLocation([FromBody] getCityNameVM model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
                DbGeography point = ConvertLatLonToDbGeography(model.lon, model.lat);
                user.point = point;
                dbcontext.SaveChanges();
            }

            responseModel mymodel = new responseModel();
            mymodel.status = 200;
            mymodel.message = "ok";

            string result = JsonConvert.SerializeObject(mymodel);
            JObject jObject = JObject.Parse(result);
            return jObject;
        }

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
        public sendCityVM getCity([FromBody] getCityVM model)
        {
            sendCityVM output = new sendCityVM();

            using (Context dbcontext = new Context())
            {

                if (model == null)
                {

                    List<newcity> cities = dbcontext.cities.Where(x => x.userID == x.parentID).Select(x => new newcity { title = x.title, parentID = x.parentID, userID = x.userID }).ToList();
                    output.lst = cities;
                    output.status = 200;

                }
                else
                {

                    if (!string.IsNullOrEmpty(model.search))
                    {

                        List<city> cities = dbcontext.cities.ToList();
                        List<newcity> afterSearch = (from u in dbcontext.cities
                                                     join p in dbcontext.cities on u.parentID equals p.userID
                                                     where u.userID != u.parentID && (p.title.Contains(model.search) || u.title.Contains(model.search))
                                                     select new newcity { title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList();


                        output.lst = afterSearch;
                        output.status = 200;

                    }
                    else
                    {
                        Guid myguid = new Guid(model.ID);
                        List<newcity> afterSearch = (from u in dbcontext.cities
                                                     join p in dbcontext.cities on u.parentID equals p.userID
                                                     where u.parentID == myguid && u.userID != u.parentID
                                                     select new newcity { title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList();
                        output.lst = afterSearch;
                        output.status = 200;


                    }


                }

            }


            return output;
        }

        [System.Web.Http.HttpPost]
        public sendTypeVM getType([FromBody] getCityVM model)
        {

            sendTypeVM output = new sendTypeVM();
            string modelstring = "";
            using (Context dbcontext = new Context())
            {

                if (model == null)
                {
                    List<cartype> types = dbcontext.cartypes.Where(x => x.typeID == x.parentID).ToList();
                    output.lst = types;
                    output.status = 200;

                }
                else
                {
                    Guid myguid = new Guid(model.ID);
                    List<cartype> types = dbcontext.cartypes.Where(x => x.parentID == myguid && x.typeID != myguid).ToList();
                    output.lst = types;
                    output.status = 200;
                }


            }

            return output;
        }
        [System.Web.Http.HttpPost]
        public sendLoadTypeVM getLoadType([FromBody] getCityVM model)
        {

            sendLoadTypeVM output = new sendLoadTypeVM();
            string modelstring = "";
            using (Context dbcontext = new Context())
            {

                if (model == null)
                {
                    List<loadType> types = dbcontext.loadTypes.ToList();
                    output.lst = types;
                    output.status = 200;

                }
                else
                {

                    List<loadType> types = dbcontext.loadTypes.Where(x => x.title.Contains(model.search)).ToList();
                    output.lst = types;
                    output.status = 200;
                }


            }

            return output;
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
                user myuser = dbcontext.users.SingleOrDefault(x => x.phone == model.phone && x.userType == model.userType);
                if (myuser != null)
                {
                    myuser.code = "9999"; // num.ToString(),
                }
                else
                {
                    string status = model.userType == "1" ? "1" : "0";

                    user newuser = new user()
                    {
                        userID = Guid.NewGuid(),
                        phone = model.phone,
                        name = "",
                        code = "9999", // num.ToString(),
                        userType = model.userType,
                        status = status
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
        public JObject UploadFiles()
        {
            responseModel mymodel = new responseModel();
            //Create the Directory.
            string path = HttpContext.Current.Server.MapPath("~/Uploads/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Fetch the File.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];


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



        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setOrder([FromBody] setOrderVM model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                Guid orgincity = new Guid(model.originCityID);
                Guid desctincity = new Guid(model.destinCityID);

                city origin = dbcontext.cities.SingleOrDefault(x => x.userID == orgincity);
                city destination = dbcontext.cities.SingleOrDefault(x => x.userID == desctincity);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Add("Api-Key", "service.d09e8d248510468daf13691f4c352241");
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
                    description = model.description,
                    destinCityID = new Guid(model.destinCityID),
                    driverID = new Guid(),
                    loadAmount = model.loadAmount,
                    loadingFinishHour = model.loadingFinishHour,
                    loadingStartHour = model.loadingStartHour,
                    loadTypeID = new Guid(model.loadTypeID),
                    orderID = orderguid,
                    originCityID = new Guid(model.originCityID),
                    pricePerTone = model.pricePerTone,
                    priceTotal = model.priceTotal,
                    recieveSMS = model.recieveSMS,
                    showPhone = model.showPhone,

                };
                dbcontext.orders.Add(neworder);
                dbcontext.SaveChanges();

                List<string> typeList = model.type.Trim(',').Split(',').ToList();
                foreach (var type in typeList)
                {
                    ordertype newtype = new ordertype()
                    {
                        orderID = orderguid,
                        ordertypeID = Guid.NewGuid(),
                        typeID = new Guid(type)
                    };
                    dbcontext.ordertypes.Add(newtype);
                }



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
        public getOrderVM getOrder([FromBody] setOrderVM model)
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
                    order myorder = dbcontext.orders.SingleOrDefault(c => c.orderID == orderGuid);

                    newcity origin = (from u in dbcontext.cities
                                     join p in dbcontext.cities on u.parentID equals p.userID
                                     where u.userID != u.parentID && u.userID == myorder.originCityID
                                     select new newcity { lat = u.lat, lon=u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();
                    newcity destination = (from u in dbcontext.cities
                                        join p in dbcontext.cities on u.parentID equals p.userID
                                        where u.userID != u.parentID && u.userID == myorder.destinCityID
                                        select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();

                    result.origing = origin;
                    result.destination = destination;
                    result.description = myorder.description;
                    result.distance = myorder.distance;

                     


                }
                catch (Exception e)
                {


                }



                result.Keloometr = "0";
            }
            return result;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public getDashbaord getDashboard()
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
                    cartype cartype = dbcontext.cartypes.SingleOrDefault(x => x.typeID == carguid);
                    obj.type = cartype;
                    obj.status = 200;
                }
                List<city> citylist = dbcontext.cities.ToList();
                List<city> citySelected = (from u in dbcontext.cities
                                           where u.userID != u.parentID
                                           let distance = u.citypoint.Distance(user.point)
                                           orderby distance ascending
                                           select u).ToList();
                obj.origin = citySelected.First();
                obj.status = 200;


            }
            return obj;
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
                        user.hooshmandMashin = user.hooshmandMashin;
                    if (!string.IsNullOrEmpty(model.address))
                        user.address = model.address;
                    if (!string.IsNullOrEmpty(model.clientType))
                        user.clientType = model.clientType;
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
        public sendProfileVM getProfile()
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            sendProfileVM mymodel = new sendProfileVM();
            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);

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

                mymodel.user = user;
                mymodel.status = 200;
                mymodel.baseURL = "https://jbar.app/Uploads/";
                mymodel.city = citystring;
                mymodel.type = typestring;
            }


            return mymodel;
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
                //dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [cities]");
                //dbcontext.SaveChanges();
                List<city> lst = dbcontext.cities.ToList();

                Guid tguid = Guid.NewGuid();



                string textFile = "E:\\cities.txt";
                if (File.Exists(textFile))
                {
                    // Read a text file line by line.
                    string[] lines = File.ReadAllLines(textFile);

                    foreach (var city in lst)
                    {
                        string cityCode = city.title;
                        foreach (string line in lines)
                        {
                            List<string> line0 = line.Replace("(", "").Replace("),", "").Split('\t').ToList();
                            string code = line0[0];
                            if (code == cityCode)
                            {
                                string title = line0[1];
                                string titleEN = "";
                                string lat = line0[4];
                                string lon = line0[5];
                                string rcode = "";
                                if (lat == "")
                                {

                                }
                                Guid id = Guid.NewGuid();
                                city newcity = dbcontext.cities.SingleOrDefault(x => x.title == title && x.userID != x.parentID);
                                if (newcity == null)
                                {
                                    newcity = new city()
                                    {
                                        code = rcode,
                                        lat = lat,
                                        lon = lon,
                                        title = title,
                                        userID = id,
                                        parentID = city.userID,
                                    };
                                    dbcontext.cities.Add(newcity);
                                }
                                else
                                {
                                    newcity.lat = lat;
                                    newcity.lon = lon;
                                }




                            }





                        }
                        int index = lst.IndexOf(city);
                    }



                }
                dbcontext.SaveChanges();
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
    }
}