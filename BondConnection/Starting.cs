using System;

using Xamarin.Forms;

namespace BondConnection
{
    public class Starting : ContentPage
    {
        public Starting()
        {
            //NavigationPage.SetHasNavigationBar(this, true);

            var content = new ContentPage
            {
                Title = "BondConnection",
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!",
                                
                        }

                    }
                }
            
            };
            //Content = content;
       }
    }
}



