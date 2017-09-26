using BondConnection;
using BondConnection.iOS;
using Xamarin.Forms;
using UIKit;
using System.Threading.Tasks;

[assembly: Dependency(typeof(EntryAlertService))]

namespace BondConnection.iOS
{
    public class EntryAlertService : IEntryAlertService
    {
        public Task<EntryAlertResult> Show(string title, string message,
                                            string accepte, string cancel, bool isTextwindow = false)
        {
            var tcs = new TaskCompletionSource<EntryAlertResult>();

            UIApplication.SharedApplication.InvokeOnMainThread(() => {
                var alert = new UIAlertView(title, message, null, cancel, new[] { accepte });

                if (isTextwindow)
                {
                    alert.AlertViewStyle = UIAlertViewStyle.SecureTextInput;
                }
                else
                {
                    alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
                }

                alert.Clicked += (sender, e) => tcs.SetResult(new EntryAlertResult
                {
                    PressedButtonTitle = alert.ButtonTitle(e.ButtonIndex),
                    Text = alert.GetTextField(0).Text
                });
                alert.Show();
            });

            return tcs.Task;
        }
    }
}