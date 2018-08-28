using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;

using Xamarin.Forms;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using System.IO;

namespace InnovationMonitor
{
    public class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            BackgroundColor = MyColors.Black;

            var title = new Label();
            title.Text = "Settings Page";

            var sendButton = new Button { 
                Text = "Send Picture", 
                TextColor = MyColors.White, 
                FontAttributes = FontAttributes.Bold,
            };
            sendButton.Margin = new Thickness(20);

            var image = new Image
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 400,
                WidthRequest = 400,
            };

            var label = new Label { TextColor = MyColors.White };
            label.Margin = new Thickness(0,0,0,20);

            var scrollView = new ScrollView();

            var stackLayout = new StackLayout();
            stackLayout.Children.Add(image);
            stackLayout.Children.Add(sendButton);
            stackLayout.Children.Add(label);

            scrollView.Content = stackLayout;

            Device.BeginInvokeOnMainThread(async () =>
            {
                // Show latest picture when page loads
                var listOfFiles = await GetFilesListAsync(ContainerType.mycontainer);

                var max = listOfFiles.Max(t => t.Item1);
                int index = listOfFiles.FindIndex(t => t.Item1 == max);

                image.Source = await GetImageAsync(ContainerType.mycontainer, listOfFiles[index].Item2);
            });

            Content = scrollView;

            sendButton.Clicked += async (sender, e) =>
            {
                var client = new HttpClient();

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2fc81d194b10448fbe796c21e2256ce4");

                var uri = "https://westus2.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Categories&language=en";

                HttpResponseMessage response;

                // Request body
                byte[] byteData = Encoding.UTF8.GetBytes(@"{""url"":""https://iansraspiblob.blob.core.windows.net/mycontainer//home/pi/iot-hub-python-raspberrypi-client-app/imgs/c06e10f0-5cf7-42d9-b819-f46568303a42.jpg""}");

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(uri, content);
                    string responseContent = await response.Content.ReadAsStringAsync();   
                    Device.BeginInvokeOnMainThread(() => {
                        label.Text = responseContent;
                    });
                }
            };
        }

        public static async Task<ImageSource> GetImageAsync(ContainerType containerType, string name)
        {
            var container = GetContainer(containerType);

            var blob = container.GetBlobReference(name);
            if (await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                byte[] blobBytes = new byte[blob.Properties.Length];

                await blob.DownloadToByteArrayAsync(blobBytes, 0);

                Stream stream = new MemoryStream(blobBytes);
                return (ImageSource.FromStream(() => stream));
            }
            return null;
        }


        public static async Task<List<Tuple<DateTimeOffset?, string>>> GetFilesListAsync(ContainerType containerType)
        {
            var container = GetContainer(containerType);

            var allBlobsList = new List<Tuple<DateTimeOffset?, string>>();
            BlobContinuationToken token = null;

            CloudBlobDirectory directory = container.GetDirectoryReference("/home/pi/iot-hub-python-raspberrypi-client-app/imgs");

            do
            {
                var result = await directory.ListBlobsSegmentedAsync(token);
                if (result.Results.Count() > 0)
                {
                    foreach (IListBlobItem item in result.Results)
                    {
                        if (item.GetType() == typeof(CloudBlockBlob))
                        {
                            var blobs = result.Results.OfType<CloudBlockBlob>().
                                              Select(b => new { b.Properties.LastModified, b.Name }).
                                              AsEnumerable().
                                              Select(b => new Tuple<DateTimeOffset?, string>(b.LastModified, b.Name)).ToList();

                            allBlobsList.AddRange(blobs);

                            CloudBlockBlob blockBlob = (CloudBlockBlob)item;

                            Debug.WriteLine("Block blob of length {0}: {1}", blockBlob.Properties.Length, blockBlob.Uri);
                        }
                        else if (item.GetType() == typeof(CloudPageBlob))
                        {
                            CloudPageBlob pageBlob = (CloudPageBlob)item;

                            Debug.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                        }
                        else if (item.GetType() == typeof(CloudBlobDirectory))
                        {
                            CloudBlobDirectory directory_original = (CloudBlobDirectory)item;

                            Debug.WriteLine("Directory: {0}", directory_original.Uri);
                        }
                    }

                }
                token = result.ContinuationToken;
            } while (token != null);

            return allBlobsList;
        }

        static CloudBlobContainer GetContainer(ContainerType containerType)
        {
            var account = CloudStorageAccount.Parse(Constants.Constants.StorageConnection);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerType.ToString().ToLower());
        }

        public enum ContainerType
        {
            mycontainer,
        }
    }
}

