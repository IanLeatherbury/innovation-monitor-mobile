using System;
using InnovationMonitor.Pages;
using Xamarin.Forms;

namespace InnovationMonitor
{
    public class ThingListPage : ContentPage
    {
        public ThingListPage()
        {
            this.Title = "SS Brewtech";

            var stackLayout = new StackLayout();
            stackLayout.BackgroundColor = MyColors.Black;

            var image = new Image();
            image.Source = "bme_chronical_large.jpg";
            image.HeightRequest = 300;

            this.BackgroundColor = MyColors.Black;

            var listView = new ListView();
            listView.BackgroundColor = MyColors.Black;
            listView.ItemTemplate = new DataTemplate(typeof(ThingListCell));
            listView.HasUnevenRows = true;
            listView.Header = image;

            listView.ItemsSource = new[] { "Take a picture", "Monitor the temperature", "Take a video" };

            listView.ItemTapped += async (sender, e) =>
            {
                if ((string)e.Item == "Take a picture")
                {
                    await Navigation.PushAsync(new PicturePage());
                }
                else if ((string)e.Item == "Take a video")
                {
                    await Navigation.PushAsync(new VideoPage());
                }
                else if ((string)e.Item == "Monitor the temperature")
                {
                    await Navigation.PushAsync(new TemperaturePage());
                }
                ((ListView)sender).SelectedItem = null; // de-select the row
            };

            Padding = new Thickness(0, 20, 0, 0);

            stackLayout.Children.Add(listView);

            Content = stackLayout;
        }
    }
}

