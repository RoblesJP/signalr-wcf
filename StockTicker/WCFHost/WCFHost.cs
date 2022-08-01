using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using System.Configuration;
using System.ServiceProcess;
using StockTicker;
using System.ServiceModel;
using System;

[assembly: OwinStartup(typeof(WCFHost.Startup))]
namespace WCFHost
{
    public partial class WCFHost : ServiceBase
    {
        private IDisposable _webHost;
        private static WCFHost _servicesHost;
        private ServiceHost _stockTickerService;

        public WCFHost()
        {
            ServiceName = "MyWCFServiceHost";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string url = ConfigurationManager.AppSettings["WebHost"];
            _webHost = WebApp.Start(url);
            _stockTickerService = new ServiceHost(typeof(StockTickerService));
            _stockTickerService.Open();
        }

        protected override void OnStop()
        {
        }

        public static void Start()
        {
            _servicesHost = new WCFHost();
            _servicesHost.OnStart(null);
        }
    }
}

