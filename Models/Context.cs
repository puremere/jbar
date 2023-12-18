using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Data.Entity.Spatial;

namespace jbar.Model
{

    class Context : DbContext
    {

        public Context() : base("jbar")
        {
            Database.SetInitializer<Context>(new MigrateDatabaseToLatestVersion<Context, jbar.Migrations.Configuration>());
            //Database.SetInitializer<Context>(new DropCreateDatabaseIfModelChanges<Context>());
        }


       
        public DbSet<user> users { get; set; }
        public DbSet<cartype> cartypes { get; set; }
        public DbSet<city> cities { get; set; }
        public DbSet<order> orders { get; set; }
        public DbSet<loadType> loadTypes { get; set; }
        public DbSet<ordertype> ordertypes { get; set; }
        public DbSet<orderResponse> orderResponses { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<coDriver> coDrivers { get; set; }

        


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }


    public class coDriver
    {
        [Key]
        public Guid DriverID { get; set; }
        public Guid coDriverID { get; set; }
       
    }
    public class Comment
    {
        [Key]
        public Guid CommentID { get; set; }
        public Guid orderID { get; set; }
        public Guid userID { get; set; }
        public Guid clientID { get; set; }
        public string clientMark { get; set; }
        public DateTime date { get; set; }
        public string content { get; set; }
    }
    public class order
    {
        [Key]

        public Guid orderID { get; set; }
        public Guid driverID { get; set; }
        public Guid clientID { get; set; }
        public Guid originCityID { get; set; }
        public Guid destinCityID { get; set; }
        public DateTime orderdate { get; set; }
        public DateTime date { get; set; }
        public DateTime publishDate { get; set; }

        public double distance { get; set; }
        public string loadingStartHour { get; set; }
        public string loadingFinishHour { get; set; }
        public double loadAmount { get; set; }
        public double pricePerTone { get; set; }
        public double priceTotal { get; set; }
        public string description { get; set; }
        public string showPhone { get; set; }
        public string recieveSMS { get; set; }
        public int viewCount { get; set; }
        public Guid loadTypeID { get; set; }
        public string orderStatus { get; set; }

    }
    public class loadType
    {
        public Guid loadTypeID { get; set; }
        public string title { get; set; }
    }
    public class ordertype
    {
        [Key]

        public Guid ordertypeID { get; set; }
        public Guid orderID { get; set; }
        public Guid typeID { get; set; }
    }
    public class orderResponse
    {
        [Key]
        public Guid orderresponseID { get; set; }
        public Guid orderID { get; set; }
        public Guid driverID { get; set; }
        public double price { get; set; }
    }

    public class user
    {
        [Key]

        public Guid userID { get; set; }
        public string firebaseToken { get; set; }
        public string userType { get; set; }
        public string username { get; set; }
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
        public string postalCode { get; set; }
        public string hooshmandMashin { get; set; }
        public string pelakIran { get; set; }
        public string pelak1 { get; set; }
        public string pelakHarf { get; set; }
        public string pelak2 { get; set; }
        public string cartNavgan { get; set; }
        public string cartDriver { get; set; }
        public string cartMelliModir { get; set; }
        public string rooznameOrParvaneOrTasvir { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public string clientType { get; set; }

        public string lat { get; set; }
        public string lon { get; set; }
        public DbGeography point { get; set; }


    }

    public class city
    {
        [Key]

        public Guid userID { get; set; }
        public Guid parentID { get; set; }
        public string title { get; set; }
        
        public string lat { get; set; }
        public string lon { get; set; }
        public string code { get; set; }

        public string District { get; set; }
        public string country { get; set; }
        public string cty { get; set; }
        public string town { get; set; }
        public DbGeography citypoint { get; set; }
        //public DbGeography poly { get; set; }
    }

    public class cartype
    {
        [Key]

        public Guid typeID { get; set; }
        public Guid parentID { get; set; }
        public string title { get; set; }
    }

}
