using System;

using Xamarin.Forms;

namespace BondConnection
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            var content = new ContentPage
            {
                Title = "BondConnection",
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Button{
                            Text= "Redy"
                        },
                        new Button{
                            Text= "Connect"
                        },
                         new Button{
                            Text= "Send"
                        },


                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        }
                    }
                }
            };

            //MainPage = new NavigationPage(content);
            //MainPage = new Starting();

            MainPage = new Connection();
            //Connection Main = new Connection();
            //Main.ListenerConnection();
            //Main.ClientConnection();
        }

        protected override void OnStart()
        {
            // Handle when your app starts

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
