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
                user.lat = model.lat.ToString();
                user.lon = model.lon.ToString();
                //user.userType = "1";
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
                    if (model.search == null && model.ID == null)
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
                    if (model.ID == null)
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


            }

            return output;
        }
        [System.Web.Http.HttpPost]
        public sendTypeVM getTypeFull([FromBody] getCityVM model)
        {

            sendTypeVM output = new sendTypeVM();
            string modelstring = "";
            using (Context dbcontext = new Context())
            {

                List<cartype> types = dbcontext.cartypes.ToList();
                output.lst = types;
                output.status = 200;
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
                    viewCount = 0,
                    publishDate = orderdete,
                    orderStatus = "0"



                };
                dbcontext.orders.Add(neworder);
                List<string> typeList = model.type.Trim(',').Split(',').ToList();
                foreach (var type in typeList)
                {
                    ordertype newtype = new ordertype()
                    {
                        orderID = orderguid,
                        ordertypeID = Guid.NewGuid(),
                        typeID = new Guid(type),

                    };
                    dbcontext.ordertypes.Add(newtype);
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
        public getOrderVM getUserOrder([FromBody] setOrderVM model)
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
                                         where p.orderStatus == "2" && p.driverID == userID
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
                                         where p.orderStatus == "1"
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
        public sendDetailVM getOrderDetail([FromBody] orderDetailVM model)
        {
            sendDetailVM result = new sendDetailVM();
            using (Context dbcontext = new Context())
            {
                try
                {
                    Guid orderGuid = new Guid(model.orderID);
                    order myorder = dbcontext.orders.SingleOrDefault(c => c.orderID == orderGuid);
                    user client = dbcontext.users.SingleOrDefault(x => x.userID == myorder.clientID);
                    newcity origin = (from u in dbcontext.cities
                                      join p in dbcontext.cities on u.parentID equals p.userID
                                      where u.userID != u.parentID && u.userID == myorder.originCityID
                                      select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();
                    newcity destination = (from u in dbcontext.cities
                                           join p in dbcontext.cities on u.parentID equals p.userID
                                           where u.userID != u.parentID && u.userID == myorder.destinCityID
                                           select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();


                    List<newtype> types = (from o in dbcontext.cartypes
                                           join ot in dbcontext.ordertypes on o.typeID equals ot.typeID
                                           join otp in dbcontext.cartypes on o.parentID equals otp.typeID
                                           where ot.orderID == orderGuid
                                           select new newtype { title = otp.title + " - " + o.title }).ToList();

                    List<orderCommentVM> comments = (from c in dbcontext.comments
                                                     join u in dbcontext.users on c.userID equals u.userID
                                                     where c.orderID == orderGuid
                                                     select new orderCommentVM { clientImage = "https://www.jbar.app/Uploads/" + u.profileImage, clientMark = c.clientMark, clientTitle = u.name, content = c.content, date = c.date }).ToList();


                    
                    foreach (var item in comments)
                    {
                       
                        item.srtdate = dateTimeConvert.ToPersianDateString(item.date);
                    }
                    List<Comment> lst = dbcontext.comments.ToList();
                    //foreach (var item in lst)
                    //{
                    //    dbcontext.comments.Remove(item);
                    //}
                    //dbcontext.SaveChanges();

                    var sb = new StringBuilder();
                    sb.Append(Math.Ceiling(myorder.distance / 1000).ToString());
                    sb.Append("کیلومتر ");

                    result.origing = origin;
                    result.clientPhone = client.phone;
                    result.orderStatus = myorder.orderStatus;
                    result.destination = destination;
                    result.description = myorder.description;
                    result.distance = sb.ToString();
                    result.typeOrderList = types;
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
                    sb2.Append((int)(DateTime.Now - myorder.date).TotalHours);
                    sb2.Append(" ساعت پیش ");

                    var sb3 = new StringBuilder();
                    sb3.Append((int)(DateTime.Now - myorder.date).Minutes);
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
                    newcity origin = (from u in dbcontext.cities
                                      join p in dbcontext.cities on u.parentID equals p.userID
                                      where u.userID != u.parentID && u.userID == myorder.originCityID
                                      select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();
                    newcity destination = (from u in dbcontext.cities
                                           join p in dbcontext.cities on u.parentID equals p.userID
                                           where u.userID != u.parentID && u.userID == myorder.destinCityID
                                           select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();


                    List<newtype> types = (from o in dbcontext.cartypes
                                           join ot in dbcontext.ordertypes on o.typeID equals ot.typeID
                                           join otp in dbcontext.cartypes on o.parentID equals otp.typeID
                                           where ot.orderID == orderGuid
                                           select new newtype { title = otp.title + " - " + o.title }).ToList();

                    List<orderCommentVM> comments = (from c in dbcontext.comments
                                                     join u in dbcontext.users on c.userID equals u.userID
                                                     where c.orderID == orderGuid
                                                     select new orderCommentVM { clientImage = "https://www.jbar.app/Uploads/" + u.profileImage, clientMark = c.clientMark, clientTitle = u.name, content = c.content, date = c.date }).ToList();


                    var qResponse = (from c in dbcontext.orderResponses
                                     join u in dbcontext.users on c.driverID equals u.userID
                                     where c.orderID == orderGuid
                                     select new responsToOrder { driverID = c.driverID.ToString(), phone = u.phone, price = c.price, title = u.name });

                    if (myorder.orderStatus == "2")
                    {
                        qResponse = qResponse.Where(x => x.driverID == myorder.driverID.ToString());
                    }

                    List<responsToOrder> orderResponse = qResponse.ToList();
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
                    result.typeOrderList = types;
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
        public async Task<sendDetailVM> getOrderDetailClientAsync([FromBody] orderDetailVM model)
        {
            sendDetailVM result = new sendDetailVM();
            using (Context dbcontext = new Context())
            {
                try
                {
                    Guid orderGuid = new Guid(model.orderID);
                    order myorder = dbcontext.orders.SingleOrDefault(c => c.orderID == orderGuid);
                    user client = dbcontext.users.SingleOrDefault(x => x.userID == myorder.clientID);
                    newcity origin = (from u in dbcontext.cities
                                      join p in dbcontext.cities on u.parentID equals p.userID
                                      where u.userID != u.parentID && u.userID == myorder.originCityID
                                      select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToList().First();
                    List<newcity> destination = await (from u in dbcontext.cities
                                           join p in dbcontext.cities on u.parentID equals p.userID
                                           where u.userID != u.parentID && u.userID == myorder.destinCityID
                                           select new newcity { lat = u.lat, lon = u.lon, title = u.title + " ( " + p.title + " ) ", userID = u.userID, parentID = u.parentID }).ToListAsync();


                    List<newtype> types = await (from o in dbcontext.cartypes
                                           join ot in dbcontext.ordertypes on o.typeID equals ot.typeID
                                           join otp in dbcontext.cartypes on o.parentID equals otp.typeID
                                           where ot.orderID == orderGuid
                                           select new newtype { title = otp.title + " - " + o.title }).ToListAsync();

                    List<orderCommentVM> comments = await (from c in dbcontext.comments
                                                     join u in dbcontext.users on c.userID equals u.userID
                                                     where c.orderID == orderGuid
                                                     select new orderCommentVM { clientImage = "https://www.jbar.app/Uploads/" + u.profileImage, clientMark = c.clientMark, clientTitle = u.name, content = c.content, date = c.date }).ToListAsync();


                    var qResponse =  (from c in dbcontext.orderResponses
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
                    result.destination = destination.First();
                    result.description = myorder.description;
                    result.distance = sb.ToString();
                    result.typeOrderList = types;
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
        public getCoDriverResponse getCoDriver()
        {

            getCoDriverResponse obj = new getCoDriverResponse();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);
                List<codrivervm> lst = (from cd in dbcontext.coDrivers
                                        join u in dbcontext.users on cd.coDriverID equals u.userID
                                        where cd.DriverID == user.userID
                                        select new codrivervm { did = u.userID, dname = u.name,dusername = u.username }).ToList();

                obj.codrivers = lst;
                obj.status = 200;


            }
            return obj;
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public JObject addDriver([FromBody] addDriverVM model)
        {
            getDashbaord obj = new getDashbaord();
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            responseModel mymodel = new responseModel();
            Guid userID = new Guid(someObject.ToString());
            using (Context dbcontext = new Context())
            {
                user user = dbcontext.users.SingleOrDefault(x => x.userID == userID);

                user codriver = dbcontext.users.SingleOrDefault(x => x.phone == model.phone && x.userType == "1");

                if (codriver != null)
                {
                    coDriver coobj = new coDriver()
                    {
                        coDriverID = codriver.userID,
                        DriverID = user.userID
                    };
                    dbcontext.coDrivers.Add(coobj);
                    dbcontext.SaveChanges();
                    mymodel.status = 200;

                }
                else
                {
                    mymodel.status = 400;
                    mymodel.message = "کاربر مورد نظر عضو سامانه نمی باشد";
                }
            }


            string result = JsonConvert.SerializeObject(mymodel);

            JObject jObject = JObject.Parse(result);
            return jObject;
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

            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);

            Guid userID = new Guid(someObject.ToString());
            Guid orderID = new Guid(model.orderID);
            using (Context dbcontext = new Context())
            {
                orderResponse lastorder = dbcontext.orderResponses.SingleOrDefault(x => x.driverID == userID && x.orderID == orderID);
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

                dbcontext.SaveChanges();
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
                user.point = null;
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
                dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [orders]");
                dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [ordertypes]");
                dbcontext.SaveChanges();
                List<ordertype> lst0 = dbcontext.ordertypes.ToList();
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
        public async Task<List<process>> getProcess()
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());

            using (Context dbcontext = new   Context())
            {
                var query = from process in dbcontext.processes select process;
                return await query.ToListAsync();
            }
        }
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<JObject> setProcess([FromBody] process model)
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());

            using (Context dbcontext = new Context())
            {
                responseModel mymodel = new responseModel();
                var process = await dbcontext.processes.FirstOrDefaultAsync(i => i.title == model.title);
                if (process == null)
                {
                    dbcontext.processes.Add(new process() { nodeID = userID, processID = Guid.NewGuid(), title = model.title });
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


        // formula
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public async Task<formulaActionVM> getFormula()
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            formulaActionVM model = new formulaActionVM();
            using (Context dbcontext = new Context())
            {
                var query = from formula in dbcontext.formulas orderby formula.col select  formula;
                var query1 = from mabna in dbcontext.mabnas select mabna;
                var query2 = from namad in dbcontext.namads select namad;
                model.formulaList = await query.ToListAsync();
                model.mabna = await query1.ToListAsync();
                model.namadList = await query2.ToListAsync();
                return model;
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
                model.nodeID = userID;
                using (Context dbcontext = new Context())
                {
                    
                    var process = await dbcontext.formulas.FirstOrDefaultAsync(i => i.leftID == model.leftID && i.rightID == model.rightID && i.number == model.number && i.namadID == model.namadID && i.mabnaID == model.mabnaID && i.mabnaName == model.mabnaName && i.col == model.col);
                    if (process == null)
                    {

                        namad  namadFun = await  dbcontext.namads.FirstOrDefaultAsync(x => x.namadID == model.namadID);
                        mabna mabnaFun =  await dbcontext.mabnas.FirstOrDefaultAsync(x => x.mabnaID == model.mabnaID);
                        var finalLeftFun =await dbcontext.formulas.FirstOrDefaultAsync(x => x.col == model.leftID);
                        var finalRightFun = await dbcontext.formulas.FirstOrDefaultAsync(x => x.col == model.rightID);


                        model.mabnaName = mabnaFun != null ? mabnaFun.title : "";
                        model.namadName = namadFun != null ? namadFun.title : "";
                        
                        string fuNamad = HttpUtility.HtmlDecode(model.namadName);
                        string numberormabna = model.number == 0 ? model.mabnaName : model.number.ToString();
                        model.result = (model.leftID == 0 && model.rightID == 0) ? "( " + numberormabna + " )" : "( " + finalLeftFun.result + " " + fuNamad + " " + finalRightFun.result + " )";

                        dbcontext.formulas.Add(new formula() { namadName = model.namadName, name = model.name, nodeID = userID, mabnaID = model.mabnaID, mabnaName = model.mabnaName, number = model.number, formulaID = Guid.NewGuid(), namadID = model.namadID, leftID = model.leftID, rightID = model.rightID, col = model.col, result = model.result });
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
        public async Task<List<coding>> getCoding()
        {
            object someObject;
            Request.Properties.TryGetValue("UserToken", out someObject);
            Guid userID = new Guid(someObject.ToString());
            formulaActionVM model = new formulaActionVM();
            using (Context dbcontext = new Context())
            {
                var query = from coding in dbcontext.codings   where coding.nodeID == userID select coding;
                return await query.ToListAsync();
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
                model.nodeID = userID;
                using (Context dbcontext = new Context())
                {

                    var coding = await dbcontext.codings.FirstOrDefaultAsync(i => i.parentID == model.parentID && i.title == model.title );
                    if (coding == null)
                    {

                        coding parentcoding = await dbcontext.codings.FirstOrDefaultAsync(x => x.codingID == model.parentID);

                        int itemCodingType = 1;
                        int parentCodingType = parentcoding.codingType;
                        
                        if (parentcoding != null)
                        {
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

                        dbcontext.codings.Add(new coding() {  codingID = Guid.NewGuid(), codingType = itemCodingType , parentID = model.parentID , codeHesab = model.codeHesab, title = model.title});
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


    }
}