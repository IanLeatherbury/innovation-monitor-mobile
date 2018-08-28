using System;
using System.Collections.Generic;

namespace InnovationMonitor.Constants
{
    public static class TabbedPageIcons
    {
        /// <summary>
        /// Dictionary of SVG images to use for the TabbarPage.
        /// Used in each platform <see cref="TabbedPageRenderer"/>
        /// </summary>
        public static Dictionary<string, string> TabbedPageIconsDictionary = new Dictionary<string, string>
        {
            { "thingListPage", "InnovationMonitor.Resources.Images.beer.svg?assembly=InnovationMonitor"},
            { "analyzePage" ,"InnovationMonitor.Resources.Images.analyze.svg?assembly=InnovationMonitor"},
        };
    }
}
