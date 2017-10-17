using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Android.Support.V4.App;


namespace BondConnection.Droid
{
    [Activity(Label = "BondConnection.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly int NOTIFICATION_ID = 8473; // 通知に付与するID
        readonly int REQUEST_CODE = 5364; // Intent送信リクエストのID
        App _app;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);

            Android.Util.Log.Debug("Main", $"OnCreate. Instance Handle = {this.Handle}");
            Toast.MakeText(this, "NotificationSample.Droid MainActivity", ToastLength.Long).Show();//***************************

            var startUpParam = string.Empty;

            // 画面が破棄された状態で通知をタップした場合、
            // ここで通知からのデータを取り出す。
            var intent = this.Intent;
            if (intent != null)
            {
                startUpParam = intent?.Data?.ToString() ?? string.Empty;
                Android.Util.Log.Debug("Main", $"OnCreate - {startUpParam}");
                Toast.MakeText(this, string.Format("Intent {0}", startUpParam), ToastLength.Long).Show();//**************************
            }

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // 通知から起動されたなら、 startUpParam は何か入っている
            _app = new App(startUpParam);
            LoadApplication(_app);

            // 通知を表示するメッセージを受信
            MessagingCenter.Subscribe<MainPage>(this, "NOTIFY", sender =>
            {
                var builder = new NotificationCompat.Builder(ApplicationContext);

                builder.SetSmallIcon(Resource.Drawable.icon);
                builder.SetContentTitle("通知が来たぞ！押せー！！");

                // 通知をタップした時に呼び出すIntent
                var activityIntent = new Intent(this, typeof(MainActivity)); // 自分をもう1回呼ぶ
                activityIntent.SetData(Android.Net.Uri.Parse("myapp://something/param"));

                // SingleTop が超重要で、これを付けないと、通知をタップした時
                // もうひとつ画面が起動してしまう。
                activityIntent.SetFlags(ActivityFlags.SingleTop);

                builder.SetAutoCancel(true); // タップしたら通知は消すよ

                var contentIntent = PendingIntent.GetActivity(
                    ApplicationContext, REQUEST_CODE, activityIntent, PendingIntentFlags.OneShot);
                builder.SetContentIntent(contentIntent);

                var manager = NotificationManagerCompat.From(ApplicationContext);
                manager.Notify(NOTIFICATION_ID, builder.Build());
            });
        }


        // SingleTop で且つ、画面が表示されている状態で通知をタップすると、
        // 送信されたIntentはここで受信する。
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Android.Util.Log.Debug("Main", $"OnNewIntent");
            Toast.MakeText(this, "OnNewIntent to  MainActivity", ToastLength.Short).Show();//**************************
            if (intent?.Data != null)
            {
                var uri = intent.Data;
                Android.Util.Log.Debug("Main", $"OnNewIntent - {uri}");
                Toast.MakeText(this, "OnNewIntent-uri to  MainActivity", ToastLength.Short).Show();//**************************
                // ここに来たということは、Formsの画面は表示中なはずなので、
                // 現在表示されている Page の Navigation をどうにかして得て、
                // PushAsync などができる
                _app.CurrentPage.Navigation.PushAsync(new Connection());//***実行メッソド名
            }
        }



            //AlertDialog.Builder alert = new AlertDialog.Builder(this);
            //alert.SetTitle("タイトル");
            //alert.SetMessage("アラートダイアログ");
            //alert.Show();

            //LoadApplication(new App());
   }

}
