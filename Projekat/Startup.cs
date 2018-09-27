using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.Owin;
using Owin;
using Projekat.Models;
using System.Data.Entity;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(Projekat.Startup))]
namespace Projekat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            DefaultParameters();
        }

        public void DefaultParameters()
        {
            AuctionDb db = new AuctionDb();
            AdminSettings admin = db.adminSettings.FirstOrDefault();

            admin.N = 10;

            admin.D = 18000;

            admin.S = 30;

            admin.G = 50;

            admin.P = 100;

            admin.C = "RSD";

            admin.T = 100;

            db.Entry(admin).State = EntityState.Modified;
            db.SaveChanges();
        
    }

    }
}
