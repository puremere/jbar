using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public List<user> dlist { get; set; }
    }
    public class codrivervm
    {
        public Guid did { get; set; }
        public string dname { get; set; }
        public string phone { get; set; }
        public string plateNumber { get; set; }
        public string verifyStatus { get; set; }
        public string workingStatus { get; set; }
    }

    public class getVehicleAsyncVM
    {
        public IEnumerable<vehicle> vlist { get; set; }
        public IEnumerable<yadak> ylist { get; set; }
    }
    public class vehicleSearchVM
    {
        public Guid statusID { get; set; }
        public string query { get; set; }
    }
    public class coDriverSearchVM
    {
        public Guid verifyStatusID { get; set; }
        public Guid workingStatusID { get; set; }
        public string query { get; set; }
    }
    public class yadakSearchVM
    {
        public Guid statusID { get; set; }
        public string query { get; set; }
    }

    public class verifyStatusVM
    {
        public Guid verifyStatusID { get; set; }
        public string title { get; set; }
    }


    public class userWorkingStatusVM
    {
        public Guid workingStatusID { get; set; }
        public string title { get; set; }
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
    public class vehicleSatatusVM
    {
        public Guid statid { get; set; }
        public string title { get; set; }
    }

    public class getVehicleResponce
    {
        public List<vehicleVM> vehicleList { get; set; }
        public List<yadakVM> yadakList { get; set; }
        public List<vehicleSatatusVM> statList { get; set; }
        public List<vehicleSatatusVM> yadakstatList { get; set; }
    }
    public class setYadakForVM
    {
        public Guid vehicleID { get; set; }
        public Guid yadakID { get; set; }
        public Guid statusID { get; set; }
        public Guid yadakStatusID { get; set; }


    }
    public class addOrderOptionVM
    {
        public Guid orderOptionID { get; set; }
        public Guid parentID { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string from { get; set; }

    }
    public class addProductTypeVM
    {
        public Guid parentID { get; set; }
        public string title { get; set; }
        public string from { get; set; }

    }
    public class addProductVM
    {
        public Guid productID { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string barcode { get; set; }
        public string address { get; set; }
        public List<Guid> productTypeID { get; set; }
        public List<Guid> produtTagID { get; set; }


    }

    public class orderOptionActionVM
    {
        public List<orderOptionVM> prtlist { get; set; }
    }
    public class productTypeActionVM
    {
        public List<productTypeVM> prtlist { get; set; }
    }

    public class tagVM
    {
        public Guid tagID { get; set; }
        public string title { get; set; }
        public string from { get; set; }
    }
    public class orderOptionVM
    {
        public string title { get; set; }
        public string image { get; set; }
        public Guid orderOptionID { get; set; }
        public Guid? parentID { get; set; }
    }
    public class productTypeVM
    {
        public string title { get; set; }
        public Guid productTypeID { get; set; }
        public Guid parentID { get; set; }
    }
    public class productVM
    {
        public Guid productID { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string tagsrt { get; set; }
        public string productTypesrt { get; set; }
    }
    public class setVehicleForVM
    {
        public Guid driverID { get; set; }
        public Guid workingStatID { get; set; }
        public Guid verifStatID { get; set; }
        public Guid vehicleID { get; set; }
        public Guid vehicleStatusID { get; set; }
    }

    public class codingVM
    {
        public Guid codingID { get; set; }
        public Guid parentID { get; set; }

        public int codingType { get; set; }
        public int codeHesab { get; set; }
        public string title { get; set; }
        public string inList { get; set; }

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
        public List<vehicleSatatusVM> slist { get; set; }

    }

    public class getCoDriverResponse
    {
        public List<codrivervm> codrivers { get; set; }
        public List<vehicleVM> vehicleList { get; set; }
        public List<verifyStatusVM> verifyList { get; set; }
        public List<userWorkingStatusVM> worikingstatusList { get; set; }
        public List<vehicleSatatusVM> vehicleStatList { get; set; }

        public int status { get; set; }
        public int message { get; set; }
    }

    public class driverStatusVM
    {
        public Guid verifStatID { get; set; }
        public Guid workingStatID { get; set; }
        public Guid driverID { get; set; }


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


        public string fullname { get; set; }
        public DateTime date { get; set; }
        public string address { get; set; }

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


        // tailor



        public decimal fullHeight { get; set; }
        public decimal headCirc { get; set; }
        public decimal neckCirc { get; set; }
        public decimal frontLength { get; set; }
        public decimal chestCirc { get; set; }
        public decimal jacketWaist { get; set; }
        public decimal jktHipCirc { get; set; }
        public decimal jktBLength { get; set; }
        public decimal sleeveLength { get; set; }
        public decimal bieop { get; set; }
        public decimal troWaist { get; set; }
        public decimal troHip { get; set; }
        public decimal troLength { get; set; }
        public decimal troInseam { get; set; }
        public decimal troHem { get; set; }
        public decimal beretSize { get; set; }
        public decimal jacketSize { get; set; }
        public decimal shirtSize { get; set; }
        public decimal trouserSize { get; set; }
        public decimal shoeSize { get; set; }
        public string comment { get; set; }


        public string phone { get; set; }
        public string fullname { get; set; }
        public string IDNumber { get; set; }
        public string Rank { get; set; }
        public string unitName { get; set; }
        public string address { get; set; }


















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
    public class UploadModel
    {
        //Other inputs

        [Required]
        [DataType(DataType.Upload)]

        public List<Microsoft.AspNetCore.Http.IFormFile> Files { get; set; }
    }
    public class setOrderVM
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public string query { get; set; }
        public string video { get; set; }
        public string orderOptions { get; set; }

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



        public decimal fullHeight { get; set; }
        public decimal headCirc { get; set; }
        public decimal neckCirc { get; set; }
        public decimal frontLength { get; set; }
        public decimal chestCirc { get; set; }
        public decimal jacketWaist { get; set; }
        public decimal jktHipCirc { get; set; }
        public decimal jktBLength { get; set; }
        public decimal sleeveLength { get; set; }
        public decimal bieop { get; set; }
        public decimal troWaist { get; set; }
        public decimal troHip { get; set; }
        public decimal troLength { get; set; }
        public decimal troInseam { get; set; }
        public decimal troHem { get; set; }
        public decimal beretSize { get; set; }
        public decimal jacketSize { get; set; }
        public decimal shirtSize { get; set; }
        public decimal trouserSize { get; set; }
        public decimal shoeSize { get; set; }
        public string comment { get; set; }


        public string phone { get; set; }
        public string fullname { get; set; }
        public string IDNumber { get; set; }
        public string Rank { get; set; }
        public string unitName { get; set; }
        public string address { get; set; }

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
    public class orderHistoryDetailVM
    {
        public string formTitle { get; set; }
        public Guid flowID { get; set; }
        public DateTime creationDate { get; set; }
        public string serventname { get; set; }
    }
    public class orderHistoryVM
    {
        public List<orderHistoryDetailVM> history { get; set; }
    }
    public class processFormVM
    {
        public string title { get; set; }
        public string formType { get; set; }
        public Guid processFormID { get; set; }
    }
    public class processFormulaVM
    {
        public string formulaName { get; set; }
        public string codingType { get; set; }
        public string codingName { get; set; }
        public Guid processFormulaID { get; set; }
    }

    public class processFormActionVM
    {
        public List<processFormVM> processFormList { get; set; }
        public List<processFormVM> allForm { get; set; }
        public process process { get; set; }

    }
    public class processFormulaActionVM
    {
        public List<processFormulaVM> processFormulaList { get; set; }
        public List<formulaVM> FormulaList { get; set; }
        public List<codingVM> codingList { get; set; }
        public process process { get; set; }
    }

    public class processVM
    {
        public Guid processID { get; set; }
        public string title { get; set; }

    }

    public class detailCollection
    {
        public string key { get; set; }
        public string value { get; set; }
        public string formItemTypeCode { get; set; }
        public Guid formItemID { get; set; }
        public Guid formID { get; set; }
       
    }
    public class setFormDetailVM
    {
        public string formDetailList { get; set; }
        public Guid processID { get; set; }
        public Guid orderID { get; set; }
        public string name { get; set; }

    }

    public class formFullDetailRequest
    {
        public Guid processID { get; set; }
        public Guid orderID { get; set; }
    }

    public class formFullDetailItemVM : formItemVM
    {
        public List<orderOptionVM> orderOptions { get; set; }
       
    }
   
    public class formItemList
    {
        public List<formFullDetailItemVM> formItemDetailList { get; set; }
        public Guid formID { get; set; }
        public string formTitle { get; set; }
    }

    public class listOfFormVM
    {
        public List<formItemList> allForm { get; set; }
        public Guid orderID { get; set; }
    }
    public class setNewFormProcessVM
    {
        public Guid processID { get; set; }
        public Guid formID { get; set; }
    }

    public class formItemVM
    {
        public Guid formItemID { get; set; }

        public string itemName { get; set; }
        public string itemDesc { get; set; }
        public string itemPlaceholder { get; set; }
        public string itemtImage { get; set; }
        public string catchUrl { get; set; }
        public string isMultiple { get; set; }
        public string mediaType { get; set; }
        public string collectionName { get; set; }
        public string UIName { get; set; }
        public string formItemTypeTitle { get; set; }
        public string formItemTypeCode { get; set; }

        public Guid? optionSelected { get; set; }
        public Guid? formItemDesingID { get; set; }

        
        public Guid formItemTypeID { get; set; }

       public Guid formID { get; set; }




    }

    public class formItemDesingVM
    {

        public Guid formItemDesingID { get; set; }
        public string title { get; set; }
    }
    public class formItemTypeVM
    {

        public Guid formItemTypeID { get; set; }
        public string title { get; set; }
    }
    public class formFullDetailVM
    {
        public List<formFullDetailItemVM> formItemList { get; set; }
    }
    public class formItemActionVM
    {
        public List<formItemVM> formItemList { get; set; }
        public List<orderOptionVM> orderOptionList { get; set; }
        public List<formItemTypeVM> formItemTypeList { get; set; }
        public List<formItemDesingVM> formItemDesingList { get; set; }

        public form form { get; set; }
    }

    public class userFlowVM
    {
        public List<orderFlowVM> flowList { get; set; }
    }
    public class orderFlowVM
    {
        public Guid  flowID { get; set; }
        public string  processName { get; set; }
        public Guid processID { get; set; }
        public string  orderName { get; set; }
        public Guid orderID { get; set; }
        public string isFinished { get; set; }
    }
    public class processActionVM
    {
        public List<processVM> processList { get; set; }
        public List<formulaVM> formulaList { get; set; }
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
        public string formItemName { get; set; }
        public string mabnaName { get; set; }
    }
    public class formulaActionVM
    {
        public List<formulaVM> formulaList { get; set; }
        public List<namadVM> namadList { get; set; }
        public List<mabnaVM> mabna { get; set; }
        public List<formItemVM> formItemList { get; set; }
        public process process { get; set; }
    }
    public class productActionVM
    {
        public List<productVM> productList { get; set; }
        public List<productTypeVM> productTypeList { get; set; }
        public List<tagVM> taglist { get; set; }
    }


    public class nullclass
    {

    }
}

