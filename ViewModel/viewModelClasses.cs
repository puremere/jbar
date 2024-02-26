using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jbar.Model;

namespace jbar.ViewModel
{

    public class distanceVM
    {
        public double? distance { get; set; }
    }
    public class viewModelClasses
    {
    }
    public class responseModel
    {
        public int status { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }
    public class doSignIn
    {
        public string code { get; set; }
        public string phone { get; set; }
        public string userType { get; set; }
    }
    public class sendOrderNotif
    {
        public string orderID { get; set; }
        public double lat { get; set; }

        public double lon { get; set; }
    }
    public class getCityVM
    {
        public string ID { get; set; }
        public string search { get; set; }

    }


    public class getCityNameVM
    {
        public double lat { get; set; }

        public double lon { get; set; }
    }
    public class setProfileVM
    {

        public string username { get; set; }
        public string name { get; set; }
        public string coName { get; set; }
        public string shenaseSherkat { get; set; }

        public string firebaseToken { get; set; }
        public string hooshmandMashin { get; set; }
        public string postalCode { get; set; }
        public string cartMelliModir { get; set; }
        public string RPT { get; set; }
        public string clientType { get; set; }
        public string profileImage { get; set; }
        public string codeMelli { get; set; }
        public string cityID { get; set; }
        public string address { get; set; }
        public string emPhone { get; set; }
        public string typeID { get; set; }
        public string pelakIran { get; set; }
        public string pelak1 { get; set; }
        public string pelakHarf { get; set; }
        public string pelak2 { get; set; }
        public string cartNavgan { get; set; }
        public string cartDriver { get; set; }
    }

    public class sendTypeVMF
    {
        public string json { get; set; }

    }
    public class sendProfileVM
    {
        public Model.user user { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public string baseURL { get; set; }
        public string city { get; set; }
        public string type { get; set; }

    }

    public class sndUser
    {
        public string userType { get; set; }
        public string name { get; set; }
        public string profileImage { get; set; }
        public string coName { get; set; }
        public string phone { get; set; }
        public string code { get; set; }
        public string codeMelli { get; set; }
        public string shenaseSherkat { get; set; }
        public string cityID { get; set; }
        public string address { get; set; }
        public string emPhone { get; set; }
        public string typeID { get; set; }
        public string hooshmandMashin { get; set; }
        public string pelakIran { get; set; }
        public string pelak1 { get; set; }
        public string pelakHarf { get; set; }
        public string pelak2 { get; set; }
        public string cartNavgan { get; set; }
        public string cartDriver { get; set; }
        public string cartMelliModir { get; set; }
        public string rooznameOrParvane { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public string baseURL { get; set; }
        public string cityName { get; set; }
        public string typeName { get; set; }
    }
    public class sendLoadTypeVM
    {
        public List<Model.loadType> lst { get; set; }
        public int status { get; set; }
        public string message { get; set; }
    }


    public class driverList
    {
      public   List<user> dlist { get; set; }
    }
    public class codrivervm
    {
        public Guid did { get; set; }
        public string dname { get; set; }
        public string phone { get; set; }
        public string plateNumber { get; set; }
    }

    public class getVehicleAsyncVM
    {
       public  IEnumerable<vehicle> vlist { get; set; }
        public IEnumerable<yadak> ylist { get; set; }
    }
    public class vehicleVM 
    {
        public Guid vehicleID { get; set; }
        public string pelak1 { get; set; }
        public string pelakHarf { get; set; }
        public string pelak2 { get; set; }
        public string iran { get; set; }
        public string yadakNumber { get; set; }
        public string status { get; set; }
    }
    public class getVehicleResponce
    {
        public List<vehicleVM> vehicleList { get; set; }
        public List<yadakVM> yadakList { get; set; }
    }
    public class setYadakForVM
    {
        public Guid vehicleID { get; set; }
        public Guid yadakID { get; set; }
    }
    public class setVehicleForVM
    {
        public Guid driverID { get; set; }
        public Guid vehicleID { get; set; }
    }

    public class codingVM
    {
        public Guid codingID { get; set; }
        public Guid parentID { get; set; }

        public int codingType { get; set; }
        public int codeHesab { get; set; }
        public string title { get; set; }
    }

    public class yadakVM
    {

        public Guid yadakID { get; set; }
        public string yadakNumber { get; set; }
        public string status { get; set; }
        public string loadtype { get; set; }
    }

    public class cartypeVM
    {
        public Guid typeID { get; set; }
        public string title { get; set; }
    }
    public class getYadakResponce
    {
        public List<yadakVM> yadakList { get; set; }
        public List<cartypeVM> typeList { get; set; }
    }

    public class getCoDriverResponse
    {
        public List<codrivervm> codrivers { get; set; }
        public List<vehicleVM> vehicleList { get; set; }
        public int status { get; set; }
        public int message { get; set; }
    }
    public class getDashbaord
    {
        public Model.cartype type { get; set; }
        public Model.city origin { get; set; }
        public int status { get; set; }
    }
    public class sendTypeVM
    {
        public List<Model.cartype> lst { get; set; }
        public int status { get; set; }
        public string message { get; set; }

    }

    public class newtype
    {
        public string title { get; set; }
    }
    public class orderListVM
    {
        public string orderID { get; set; }

        public string status { get; set; }
        public string origin { get; set; }
        public string destin { get; set; }
        public int priceTotal { get; set; }
        public int pricePerTon { get; set; }
        public string viewNumber { get; set; }

    }
    public class getOrderVM
    {
        public List<orderListVM> orderList { get; set; }
        public int status { get; set; }
        public string message { get; set; }

    }
    public class sendCityVM
    {
        public List<newcity> lst { get; set; }
        public int status { get; set; }
        public string message { get; set; }

    }

    public class panelCityVM
    {
        public sendCityVM cityList { get; set; }
        public string listname { get; set; }
    }



    public class newcity
    {
        public Guid userID { get; set; }
        public Guid parentID { get; set; }
        public string title { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
    }



    public class Datum
    {
        public int place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public object osm_id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public int place_rank { get; set; }
        public double importance { get; set; }
        public string addresstype { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public List<string> boundingbox { get; set; }
        public Geojson geojson { get; set; }
    }

    public class Geojson
    {
        public string type { get; set; }
        public List<object> coordinates { get; set; }
    }

    public class GeoVM
    {
        public List<Datum> data { get; set; }
    }


    public class orrdinates
    {
        public List<List<double>> obb { get; set; }
    }

    public class notifResultVM
    {
        public newcity origin { get; set; }
        public newcity destination { get; set; }
        public int netTotal { get; set; }
        public string distance { get; set; }
    }
    public class notifVM
    {
        public bigStyle big_style { get; set; }
        public titleModel title { get; set; }
        public string data { get; set; }
        public int id { get; set; }
        public string direction { get; set; }
        public string sound { get; set; }
        public string icon { get; set; }
        public string vibration { get; set; }
        public string color { get; set; }
        public clickAction click_action { get; set; }
        public clickAction delete_action { get; set; }
        public List<clickAction> actions { get; set; }


    }
    public class clickAction
    {
        public string title { get; set; }
        public string type { get; set; }
        public string data { get; set; }
    }
    public class titleModel
    {
        public string text { get; set; }
        public string color { get; set; }
    }

    public class bigStyle
    {
        public string type { get; set; }
        public string picture { get; set; }
    }

    public class orderDetailVM
    {
        public string orderID { get; set; }
    }

    public class requestOrderVM
    {
        public string orderID { get; set; }
        public double price { get; set; }
    }
    public class responseOrderVM
    {
        public string orderID { get; set; }
        public string driverID { get; set; }
    }

    public class addDriverVM
    {
        public string phone { get; set; }
    }
    public class setCommentVM
    {
        public string content { get; set; }
        public string mark { get; set; }
        public string orderID { get; set; }
    }

    public class sendUserStatusVM
    {
        public int status { get; set; }
        public string statusTitle { get; set; }
        public string statusCode { get; set; }
        public string message { get; set; }
    }
    public class getCommentVM
    {
        public int status { get; set; }
        public List<orderCommentVM> lst { get; set; }
    }
    public class responsToOrder
    {
        public string title { get; set; }
        public string phone { get; set; }
        public double price { get; set; }
        public string driverID { get; set; }
    }
    public class sendDetailVM
    {
        public int status { get; set; }
        public string orderStatus { get; set; }
        public string clientPhone { get; set; }
        public int netTotal { get; set; }
        public int netPerTon { get; set; }
        public int pricePerKiloometre { get; set; }
        public List<newtype> typeOrderList { get; set; }
        public List<responsToOrder> orderRespons { get; set; }
        public newcity origing { get; set; }
        public newcity destination { get; set; }
        public string distance { get; set; }
        public string description { get; set; }
        public int totalView { get; set; }
        public string passedTime { get; set; }
        public string clientStatus { get; set; }
        public string clientName { get; set; }
        public double clientMark { get; set; }
        public int clientTotalComment { get; set; }
        public int returnOrderCount { get; set; }
        public List<orderCommentVM> comments { get; set; }
        public string orderID { get; set; }
    }
    public class orderCommentVM
    {
        public string clientTitle { get; set; }
        public string clientImage { get; set; }
        public string clientMark { get; set; }
        public DateTime date { get; set; }
        public string srtdate { get; set; }
        public string content { get; set; }
    }

    public class setOrderclientVM
    {
        public string status { get; set; }
        public string originCityID { get; set; }
        public string destinCityID { get; set; }
        public string date { get; set; }
        public string type { get; set; }
        public string loadingStartHour { get; set; }
        public string loadingFinishHour { get; set; }
        public string loadAmount { get; set; }
        public string pricePerTone { get; set; }
        public string priceTotal { get; set; }
        public string description { get; set; }
        public string showPhone { get; set; }
        public string recieveSMS { get; set; }
        public string loadTypeID { get; set; }
    }
    public class setOrderVM
    {
        public string status { get; set; }
        public string originCityID { get; set; }
        public string destinCityID { get; set; }
        public string date { get; set; }
        public string type { get; set; }
        public string loadingStartHour { get; set; }
        public string loadingFinishHour { get; set; }
        public double loadAmount { get; set; }
        public double pricePerTone { get; set; }
        public double priceTotal { get; set; }
        public string description { get; set; }
        public string showPhone { get; set; }
        public string recieveSMS { get; set; }
        public string loadTypeID { get; set; }
    }



    // {"status":"Ok","rows":[{"elements":[{"status":"Ok","duration":{"value":38724,"text":"۱۰ ساعت ۴۵ دقیقه"},"distance":{"value":864431,"text":"۸۷۵ کیلومتر"}}]}],"origin_addresses":["35.723454,50.367155"],"destination_addresses":["37.476104,57.332076"]}
    public class distancVM
    {
        public string status { get; set; }
        public List<rowOBJ> rows { get; set; }

    }
    public class rowOBJ
    {
        public List<elementOBJ> elements { get; set; }
        public object origin_addresses { get; set; }
    }
    public class elementOBJ
    {
        public string status { get; set; }
        public durationOBJ duration { get; set; }
        public durationOBJ distance { get; set; }

    }

    public class durationOBJ
    {
        public int value { get; set; }
        public string text { get; set; }
    }

    public class panelSetOrder
    {
        public sendLoadTypeVM loadList { get; set; }
        public sendCityVM cityList { get; set; }
        public List<jbar.Model.cartype> typeList { get; set; }
    }

    public class processFormulaVM
    {
        public string formulaName { get; set; }
        public Guid processFormulaID { get; set; }
    }
    public class processFormulaActionVM
    {
        public List<processFormulaVM> processFormulaList { get; set; }
        public List<formula> FormulaList { get; set; }
        public List<coding> codingList { get; set; }
        public process process { get; set; }
    }
    public class processActionVM
    {
        public List<process> processList { get; set; }
        public List<formula> formulaList { get; set; }
    }
    public class namadVM
    {
        public Guid namadID { get; set; }
        public string title { get; set; }
    }
    public class mabnaVM
    {
        public Guid mabnaID { get; set; }
        public string title { get; set; }
    }
    
    public class formulaVM
    {
        public Guid formulaID { get; set; }
        public int col { get; set; }
        public int leftID { get; set; }
        public int rightID { get; set; }
        public decimal number { get; set; }
        public string name { get; set; }
        public string result { get; set; }
        public string namadName { get; set; }
        public string mabnaName { get; set; }
    }
    public class formulaActionVM
    {
        public List<formulaVM> formulaList { get; set; }
        public List<namadVM> namadList { get; set; }
        public List<mabnaVM> mabna { get; set; }
    }

   

    public class nullclass
    {

    }
}

