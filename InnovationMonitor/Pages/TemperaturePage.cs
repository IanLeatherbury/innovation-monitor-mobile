using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InnovationMonitor.Models;
using InnovationMonitor.Services;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using InnovationMonitor.Cells;
using System.Diagnostics;

namespace InnovationMonitor.Pages
{
    public class TemperaturePage : ContentPage
    {
        //ObservableCollection<string> temps = new ObservableCollection<string>();
        ObservableCollection<TempHumidityModel> temps = new ObservableCollection<TempHumidityModel>();
        ListView listView = new ListView();

        public TemperaturePage()
        {
            listView.BackgroundColor = MyColors.Black;
            listView.ItemTemplate = new DataTemplate(typeof(TemperatureCell));
            listView.HasUnevenRows = true;
            listView.ItemsSource = temps;
            listView.IsPullToRefreshEnabled = true;
            listView.RefreshCommand = RefreshCommand;

            Task.Run(async () =>
            {
                listView.IsRefreshing = true;

                var getTemps = await GetTempHumidity();

                foreach (var t in getTemps)
                {
                    temps.Add(t);
                }

                listView.IsRefreshing = false;
            });

            Content = listView;
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    listView.IsRefreshing = true;

                    var getTemps = await GetTempHumidity();

                    Device.BeginInvokeOnMainThread(() => {
                        foreach (var t in getTemps)
                        {
                            temps.Add(t);
                        }
                    });

                    listView.IsRefreshing = false;
                });
            }
        }

        public async Task<List<TempHumidityModel>> GetTempHumidity()
        {
            // Get Temp + Humidity Values
            var tempHumidityService = new TempHumidityService();

            IsBusy = true;

            try
            {
                await tempHumidityService.TakeTempHumidity();

                var tempHumidity = await tempHumidityService.GetTempHumidity();

                var orderedByDate = tempHumidity.OrderByDescending(x => DateTime.Parse(x.TimeOfReading)).ToList();

                //var temps = tempHumidity.Select(t => t.Temperature).ToList();

                return orderedByDate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
                // TODO: Implement
                //Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                //    { "Message", ex.Message },
                //    { "StackTrace", ex.ToString() }
                //}); 

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
