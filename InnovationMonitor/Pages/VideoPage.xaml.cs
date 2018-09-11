using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;

using Xamarin.Forms;

namespace InnovationMonitor.Pages
{
    public partial class VideoPage : ContentPage
    {
        public VideoPage()
        {
            InitializeComponent();

            BackgroundColor = MyColors.Black;
        }

        void OnGetVideoButtonClicked(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var listOfFiles = await GetFilesListAsync(ContainerType.myvideocontainer);

                var max = listOfFiles.Max(t => t.Item1);
                int index = listOfFiles.FindIndex(t => t.Item1 == max);

                await CrossMediaManager.Current.Play(listOfFiles[index].Item2, MediaFileType.Video);

            });
        }

        async void OnTakeVideoButtonClicked(object sender, EventArgs args)
        {
            var request = WebRequest.Create(Constants.Constants.TakeVideoURL);
            request.ContentType = "application/xml";
            request.Method = "GET";

            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Debug.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
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

            CloudBlobDirectory directory = container.GetDirectoryReference("/home/pi/iot-hub-python-raspberrypi-client-app/vids/");

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
                                              Select(b => new { b.Properties.LastModified, b.Uri.AbsoluteUri }).
                                              AsEnumerable().
                                              Select(b => new Tuple<DateTimeOffset?, string>(b.LastModified, b.AbsoluteUri)).ToList();

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
            myvideocontainer,
        }
    }
}
