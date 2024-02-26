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
        public DbSet<orderResponse> orderResponses { get; set; }
        public DbSet<Comment> comments { get; set; }

        public DbSet<processOrder> processOrders { get; set; }
        public DbSet<process> processes { get; set; }
        public DbSet<vehicle> vehicles { get; set; }
        public DbSet<yadak> yadaks { get; set; }
        public DbSet<yadakVehicle> yadakVehicles { get; set; }
        public DbSet<coding> codings { get; set; }
        public DbSet<sanad> sanads { get; set; }
        public DbSet<article> articles { get; set; }
        public DbSet<sanadSource> sanadSources { get; set; }
        public DbSet<formula> formulas { get; set; }
        public DbSet<namad> namads { get; set; }
        public DbSet<mabna> mabnas { get; set; }
        public DbSet<processFormula> processFormulas { get; set; }
        public DbSet<vehicleStatus> vehicleStatuses { get; set; }
        public DbSet<yadakStatus> yadakStatuses { get; set; }
        public DbSet<userVehicle> userVehicles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<user>()
                        .HasIndex(p => new { p.phone, p.userType })
                        .IsUnique();

            modelBuilder.Entity<vehicle>().HasOptional(s => s.yadak).WithMany().HasForeignKey(x => x.yadakID);
            modelBuilder.Entity<user>().HasOptional(s => s.vehicle).WithMany().HasForeignKey(x => x.vehicleID);
            modelBuilder.Entity<namad>().HasRequired(s => s.user).WithMany().HasForeignKey(x => x.userID).WillCascadeOnDelete(false);
            modelBuilder.Entity<mabna>().HasRequired(s => s.user).WithMany().HasForeignKey(x => x.userID).WillCascadeOnDelete(false);

            modelBuilder.Entity<order>()
                         .HasOptional(m => m.driveruser)
                         .WithMany(t => t.driverOrders)
                         .HasForeignKey(m => m.driverID)
                         .WillCascadeOnDelete(false);
            modelBuilder.Entity<order>()
                        .HasRequired(m => m.clientuser)
                        .WithMany(t => t.clientOrders)
                        .HasForeignKey(m => m.clientID)
                        .WillCascadeOnDelete(false);
            modelBuilder.Entity<order>()
                        .HasRequired(m => m.origincity)
                        .WithMany(t => t.originOrders)
                        .HasForeignKey(m => m.originCityID)
                        .WillCascadeOnDelete(false);
            modelBuilder.Entity<order>()
                        .HasRequired(m => m.destincity)
                        .WithMany(t => t.destinOrders)
                        .HasForeignKey(m => m.destinCityID)
                        .WillCascadeOnDelete(false);

        }

    }










    public class coding
    {
        [Key]
        public Guid codingID { get; set; }
        public Guid parentID { get; set; }

        public int codingType { get; set; }
        public int codeHesab { get; set; }
        public string title { get; set; }

        public Guid? userID { get; set; }
        [ForeignKey("userID")]
        public virtual user user { get; set; }
        public virtual ICollection<processFormula> ProcessFormulas { get; set; }


    }
    public class sanad
    {
        [Key]
        public Guid sanadID { get; set; }
        public int number { get; set; }
        public int atf { get; set; }
        public int status { get; set; }
        public DateTime date { get; set; }
        public string description { get; set; }
        public Guid userID { get; set; }
        [ForeignKey("userID")]
        public virtual user user { get; set; }

    }
    public class article
    {
        [Key]
        public Guid articleID { get; set; }

        public Guid sanadID { get; set; }
        public int tarafHesab { get; set; }
        public int status { get; set; }
        public DateTime date { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public int koll { get; set; }
        public int grooh { get; set; }
        public int moin { get; set; }
        public int tafsil1 { get; set; }
        public int tafsil2 { get; set; }
        public int tafsil3 { get; set; }
        public int tafsil4 { get; set; }
        public int tafsil5 { get; set; }
        public int type { get; set; }
        public Guid userID { get; set; }
        [ForeignKey("userID")]
        public virtual user user { get; set; }
    }
    public class sanadSource
    {
        [Key]
        public Guid sanadSourceID { get; set; }
        public string title { get; set; }
        public Guid codingID { get; set; }
        public Guid userID { get; set; }
        [ForeignKey("userID")]
        public virtual user user { get; set; }
    }

    public class yadakStatus
    {
        [Key]
        public Guid yadakStatusID { get; set; }
        public string title { get; set; }

        public virtual ICollection<yadak> Yadaks { get; set; }
        public Guid? userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }
    }
    public class vehicleStatus
    {
        [Key]
        public Guid vehicleStatusID { get; set; }
        public string title { get; set; }
        public Guid? userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }
    }
    public class vehicle
    {
        [Key]
        public Guid vehicleID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int col { get; set; }
        public string pelak1 { get; set; }
        public string pelakHarf { get; set; }
        public string pelak2 { get; set; }
        public string iran { get; set; }
        
        
        public Guid userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }


        public Guid StatusID { get; set; }
        [ForeignKey("StatusID")]
        public virtual vehicleStatus vehicleStatus { get; set; }

        public Guid? yadakID { get; set; }
        [ForeignKey("yadakID")]
        public virtual yadak yadak { get; set; }
    }
    public class yadak
    {
        [Key]
        public Guid yadakID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int col { get; set; }
        public string yadakNumber { get; set; }
        public Guid userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }

        [ForeignKey("yadakStatus")]
        public Guid StatusID { get; set; }
        public virtual yadakStatus yadakStatus { get; set; }


        public Guid typeID { get; set; }
        [ForeignKey("typeID")]
        public virtual cartype Cartype { get; set; }



    }

    public class yadakVehicle
    {
        [Key]
        public Guid yadakVehicleID { get; set; }
        public Guid yadakID { get; set; }
        public Guid vehicleID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isFinished { get; set; }
    } // ???
    public class userVehicle
    {
        [Key]
        public Guid userVehicleID { get; set; }
        public Guid vehicleID { get; set; }
        public Guid userID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isFinished { get; set; }
    } // ????

    public class namad   // این آیتم مقداریه که به عنوان بیس در نظر گرفته میشه مثل ارزش کل بار 
    {
        [Key]
        public Guid namadID { get; set; }
        public string title { get; set; }
        public string value { get; set; }
        public Guid userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }
        public virtual ICollection<formula> Formulas { get; set; }

    }
    public class mabna   // این آیتم مقداریه که به عنوان بیس در نظر گرفته میشه مثل ارزش کل بار 
    {
        [Key]
        public Guid mabnaID { get; set; }
        public string title { get; set; }
        public string value { get; set; }
        public Guid? userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }

        public virtual ICollection<formula> Formulas { get; set; }
    }

    public class formula
    {
        [Key]
        public Guid formulaID { get; set; }
        public int col { get; set; }
        public int leftID { get; set; }
        public int rightID { get; set; }


        public Guid? mabnaID { get; set; }
        [ForeignKey("mabnaID")]
        public virtual mabna mabna { get; set; }


        public Guid namadID { get; set; }
        [ForeignKey("namadID")]
        public virtual namad namad { get; set; }

        public Guid userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }

        public decimal number { get; set; }

        public string name { get; set; }
        public string result { get; set; }

        public virtual ICollection<processFormula> ProcessFormulas { get; set; }
    }
    public class process// پروسه ای که بابری برای خدش توولید میکنه و اردرهاشو تطبیق میده
    {
        [Key]
        public Guid processID { get; set; }
        public string title { get; set; }
        public Guid? userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }

        public virtual ICollection<processFormula> ProcessFormulas { get; set; }
    }

    public class processFormula// میگه چه فرمولهایی برای چه پروسه هایی هست که روی یک کدینگ اثر میزاره یا بدهکار یا بستانکار
    {
        [Key]
        public Guid processFormulaID { get; set; }
        public Guid proccessID { get; set; }
        [ForeignKey("proccessID")]
        public virtual process Process { get; set; }

        public Guid FormulaID { get; set; }
        [ForeignKey("FormulaID")]
        public virtual formula Formula { get; set; }


        public Guid codingID { get; set; }
        [ForeignKey("codingID")]
        public virtual coding Coding { get; set; }

        public string transactionType { get; set; } // بدهکاری یا بستانکاری عدد حاصل از فرمول را رووی کدینگ اعمال میکند
    }
    public class processOrder
    {
        [Key]
        public Guid processOrderID { get; set; }
        public Guid ordrID { get; set; }
        public Guid processID { get; set; }
    }   // ???


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

        public order()
        {
            this.Cartypes = new HashSet<cartype>();
        }


        [Key]
        public Guid orderID { get; set; }
        

      
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
      
        public string orderStatus { get; set; }


        public virtual ICollection<cartype> Cartypes { get; set; }

        public Guid clientID { get; set; }
        [ForeignKey("clientID")]
        public virtual user clientuser { get; set; }

        public Guid? driverID { get; set; }
        [ForeignKey("driverID")]
        public virtual user driveruser { get; set; }


        public Guid originCityID { get; set; }
        [ForeignKey("originCityID")]
        public virtual city origincity { get; set; }

        public Guid destinCityID { get; set; }
        [ForeignKey("destinCityID")]
        public virtual city destincity { get; set; }


        public Guid loadTypeID { get; set; }
        [ForeignKey("loadTypeID")]
        public virtual loadType loadtype { get; set; }

    }

    // باربری خودش روش های مختلف باربری رو مشخص میکنه  الصاق میکنه به یک سفارش که سند های ثابت با فرمول بخوره

    public class loadType
    {
        public Guid loadTypeID { get; set; }
        public string title { get; set; }
        public Guid? userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }

        public virtual ICollection<order> Orders { get; set; }
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
        public user()
        {
            Mabnas = new List<mabna>();
            Namads = new List<namad>();
        }
        [Key]
       

        public Guid userID { get; set; }
        public string firebaseToken { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string userType { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string profileImage { get; set; }
        public string coName { get; set; }
        [Column(TypeName = "VARCHAR")]
        
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


        public Guid? vehicleID { get; set; }
        public virtual vehicle vehicle { get; set; }

        public Guid? barbariID { get; set; }
        [ForeignKey("barbariID")]
        public virtual user barbari { get; set; }



        public virtual ICollection<user> CoDrivers { get; set; }
        public virtual ICollection<coding> Codings { get; set; }
        public virtual ICollection<sanad> Sanads { get; set; }
        public virtual ICollection<article> Articles { get; set; }
        public virtual ICollection<sanadSource> SanadSources { get; set; }
        public virtual ICollection<yadakStatus> YadakStatuses { get; set; }
        public virtual ICollection<vehicleStatus> vehicleStatuses { get; set; }
        public virtual ICollection<vehicle> Vehicles { get; set; }
        public virtual ICollection<yadak> yadaks { get; set; }
        public virtual ICollection<formula> Formulas { get; set; }
        public virtual ICollection<process> processes { get; set; }
        public virtual ICollection<order> driverOrders { get; set; }
        public virtual ICollection<order> clientOrders { get; set; }
        public virtual ICollection<loadType> loadTypes { get; set; }
        public virtual ICollection<cartype> cartype { get; set; }
        public virtual ICollection<namad> Namads { get; set; }
        public virtual ICollection<mabna> Mabnas { get; set; }



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


        public virtual ICollection<order> originOrders { get; set; }
        public virtual ICollection<order> destinOrders { get; set; }
    }

    public class cartype
    {
        public cartype()
        {
            this.Orders = new HashSet<order>();
        }

        [Key]
        public Guid typeID { get; set; }
        public Guid parentID { get; set; }
        public string title { get; set; }
        public virtual ICollection<yadak> Yadaks { get; set; }

        public Guid? userID { get; set; } // مرتبط با کدوم باربریه
        [ForeignKey("userID")]
        public virtual user user { get; set; }

        public virtual ICollection<order> Orders { get; set; }
    }

}
