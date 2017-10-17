using System;

using Xamarin.Forms;

namespace BondConnection
{
    public class App : Application
    {
        readonly NavigationPage _navigationPage;

        public App(string startUpParam)
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


            // NavigationPage を保持しておいて CurrentPage を外部に公開する
            _navigationPage = new NavigationPage(new MainPage());
            MainPage = _navigationPage;

            //MainPage = new NavigationPage(content);
            //MainPage = new Starting();

            //MainPage = new Connection();
            //Connection Main = new Connection();
            //Main.ListenerConnection();
            //Main.ClientConnection();
        }

        // 出、出〜！カッコつけてラムダで getter 書奴〜ｗｗｗｗ
        public Page CurrentPage => _navigationPage.CurrentPage;


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
