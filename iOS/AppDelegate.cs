using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Forms.Touch;
using FFImageLoading.Svg.Forms;
using Foundation;
using Plugin.MediaManager.Forms.iOS;
using UIKit;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

namespace InnovationMonitor.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            AppCenter.Start("da01e281-505b-43af-b1fb-65e729345872", typeof(Analytics), typeof(Crashes));

            Distribute.DontCheckForUpdatesInDebug();
            AppCenter.Start("{Your Xamarin iOS App Secret}", typeof(Distribute));

            // Code for starting up the Xamarin Test Cloud Agent
#if DEBUG
			//Xamarin.Calabash.Start();
#endif
            CachedImageRenderer.Init();
            VideoViewRenderer.Init();

            var ignore = typeof(SvgCachedImage);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
