using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;

namespace InnovationMonitor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new HomePage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("ios={Your Xamarin iOS App Secret};android={Your Xamarin Android App secret}", typeof(Distribute));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
