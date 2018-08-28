using System;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace InnovationMonitor
{
    public class HomePage : TabbedPage
    {
        public HomePage()
        {                        
            var thingListPage = new NavigationPage(new ThingListPage());
            thingListPage.AutomationId = "thingListPage";

            //Nav bar color
            thingListPage.BarBackgroundColor = MyColors.Black;
            thingListPage.BarTextColor = Color.White;
            thingListPage.Title = "Monitor";

            //Tab bar background color
            this.BarBackgroundColor = MyColors.LightBlack;
            this.BarTextColor = Color.White;

            var settingsPage = new NavigationPage(new SettingsPage());
            settingsPage.Title = "Analyze";
            settingsPage.AutomationId = "analyzePage";
            settingsPage.BarBackgroundColor = MyColors.Black;
            settingsPage.BarTextColor = MyColors.White;

            Children.Add(thingListPage);
            Children.Add(settingsPage);
        }
    }
}

