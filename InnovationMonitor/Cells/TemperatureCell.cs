using System;
using Xamarin.Forms;

namespace InnovationMonitor.Cells
{
    public class TemperatureCell : ViewCell
    {
        public TemperatureCell()
        {
            // Text labels
            var internalTempStringLabel = new Label()
            {
                Text = "Beer Temp: ",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = MyColors.Grey,
                Margin = new Thickness(10)
            };

            var externalTempStringLabel = new Label()
            {
                Text = "External Temp: ",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = MyColors.Grey,
                Margin = new Thickness(10)
            };

            var timeOfReadingLabel = new Label()
            {
                Text = "Time Of Reading: ",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = MyColors.Grey,
                Margin = new Thickness(10)
            };

            // Actual values
            var externalTempValueLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Italic,
                TextColor = MyColors.Grey,
                Margin = new Thickness(10)
            };
            var internalTempValueLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Italic,
                TextColor = MyColors.Grey,
                Margin = new Thickness(10)

            };
            var timeOfReadingValueLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Italic,
                TextColor = MyColors.Grey,
                Margin = new Thickness(10)

            };

            // Layouts
            var verticaLayout = new StackLayout();
            var horizontalLayoutTop = new StackLayout();
            var horizontalLayoutBottom = new StackLayout();
            var horizontalLayoutTemp = new StackLayout();

            //set bindings
            internalTempValueLabel.SetBinding(Label.TextProperty, new Binding("InternalTemp"));
            externalTempValueLabel.SetBinding(Label.TextProperty, new Binding("ExternalTemp"));
            timeOfReadingValueLabel.SetBinding(Label.TextProperty, new Binding("TimeOfReading"));

            //Set properties for desired design
            horizontalLayoutTop.Orientation = StackOrientation.Horizontal;
            horizontalLayoutTop.HorizontalOptions = LayoutOptions.Fill;

            horizontalLayoutBottom.Orientation = StackOrientation.Horizontal;
            horizontalLayoutBottom.HorizontalOptions = LayoutOptions.Fill;

            horizontalLayoutTemp.Orientation = StackOrientation.Horizontal;
            horizontalLayoutTemp.HorizontalOptions = LayoutOptions.Fill;

            //add views to the view hierarchy
            verticaLayout.Children.Add(horizontalLayoutTop);
            verticaLayout.Children.Add(horizontalLayoutBottom);

            horizontalLayoutTop.Children.Add(internalTempStringLabel);
            horizontalLayoutTop.Children.Add(internalTempValueLabel);

            horizontalLayoutBottom.Children.Add(externalTempStringLabel);
            horizontalLayoutBottom.Children.Add(externalTempValueLabel);

            horizontalLayoutTemp.Children.Add(timeOfReadingLabel);
            horizontalLayoutTemp.Children.Add(timeOfReadingValueLabel);

            verticaLayout.Children.Add(horizontalLayoutTop);
            verticaLayout.Children.Add(horizontalLayoutBottom);
            verticaLayout.Children.Add(horizontalLayoutTemp);

            // add to parent view
            View = verticaLayout;
        }
    }
}
