using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Xamarin.Forms;

namespace InnovationMonitor
{
    public class PicturePage : ContentPage
    {
        public PicturePage()
        {
            BackgroundColor = MyColors.Black;

            var takePictureButton = new Button
            {
                Text = "Take a picture",
                TextColor = MyColors.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            var getPictureButton = new Button
            {
                Text = "Get the picture",
                TextColor = MyColors.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            var image = new Image
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 400,
                WidthRequest = 400,
            };

            Device.BeginInvokeOnMainThread(async () =>
            {
                // Show latest picture when page loads
                var listOfFiles = await GetFilesListAsync(ContainerType.mycontainer);

                var max = listOfFiles.Max(t => t.Item1);
                int index = listOfFiles.FindIndex(t => t.Item1 == max);

                image.Source = await GetImageAsync(ContainerType.mycontainer, listOfFiles[index].Item2);
            });

            getPictureButton.Clicked += async (sender, e) =>
            {
                // Get latest picture
                var listOfFiles = await GetFilesListAsync(ContainerType.mycontainer);

                var max = listOfFiles.Max(t => t.Item1);
                int index = listOfFiles.FindIndex(t => t.Item1 == max);

                image.Source = await GetImageAsync(ContainerType.mycontainer, listOfFiles[index].Item2);
            };


            takePictureButton.Clicked += async (sender, e) =>
            {
                var request = WebRequest.Create(Constants.Constants.TakePictureURL);
                request.ContentType = "application/xml";
                request.Method = "GET";

                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Debug.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                        return;
                    }
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            var content = reader.ReadToEnd();
                            if (string.IsNullOrWhiteSpace(content))
                            {
                                Debug.WriteLine("Response contained empty body...");
                            }
                            else
                            {
                                Debug.WriteLine("Response Body: \r\n {0}", content);
                            }
                        }
                    }
                }
            };

            Content = new StackLayout
            {
                Children = {
                    image,
                    takePictureButton,
                    getPictureButton
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

