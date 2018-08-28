using System;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using FFImageLoading;
using FFImageLoading.Svg.Platform;

using InnovationMonitor;
using InnovationMonitor.iOS;
using InnovationMonitor.Constants;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]
namespace InnovationMonitor.iOS
{
    public class TabbedPageRenderer : TabbedRenderer
    {
        public TabbedPageRenderer()
        {
        }

        protected async override System.Threading.Tasks.Task<Tuple<UIKit.UIImage, UIKit.UIImage>> GetIcon(Page page)
        {

            string resourcePath = string.Empty;

            // Load the icon as a SVG vector
            if (TabbedPageIcons.TabbedPageIconsDictionary.TryGetValue(page.AutomationId, out resourcePath))
            {
                UIKit.UIImage img = await ImageService.Instance
                                            .LoadEmbeddedResource(resourcePath)
                                            .WithCustomDataResolver(new SvgDataResolver(26, 0, true))
                                            .AsUIImageAsync();


                return new Tuple<UIKit.UIImage, UIKit.UIImage>(img, img);
            }


            return await base.GetIcon(page);
        }
    }
}
