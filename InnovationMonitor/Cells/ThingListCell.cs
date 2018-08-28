using System;
using Xamarin.Forms;

namespace InnovationMonitor
{
    public class ListButton : Button { }

    public class ThingListCell : ViewCell
    {
        public ThingListCell()
        {
            var label1 = new Label
            {
                Text = "Label 1",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = MyColors.Grey,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
            };
            label1.SetBinding(Label.TextProperty, new Binding("."));

            View = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15),
                Children = { label1 },
                Spacing = 0
            };
        }
    }
}
