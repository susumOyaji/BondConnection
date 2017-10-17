using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BondConnection
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Title = "MainPage";
            var button = new Button
            {
                Text = "Notify!",
                //FontSize = 30,
                HorizontalOptions = LayoutOptions.Fill,//中央に配置する（横方向）
                VerticalOptions = LayoutOptions.Start // 中央に配置する（縦方向）    
            };

            Content = button;



            button.Clicked += (sender, e) =>
            {
                // ネイティブ側から通知を出させるためのメッセージを送信
                // Android の場合、MainActivity で受信する
                MessagingCenter.Send(this, "NOTIFY");
            };



        }
    }
}
