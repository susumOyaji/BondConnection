using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text;

using BondConnection.Droid;
using Java.Lang;
using Xamarin.Forms;
//using EntryAlertSample.Android;

using Android.Provider;



[assembly: Dependency(typeof(EntryAlertService))]
namespace BondConnection.Droid
{
    public class EntryAlertService : IEntryAlertService
    {
        public Task<EntryAlertResult> Show(string title, string message,
                                            string accepte, string cancel, bool isPassword = false)
        {
            var tcs = new TaskCompletionSource<EntryAlertResult>();

            var editText = new EditText(Forms.Context);
            if (isPassword)
            {
                editText.InputType = global::Android.Text.InputTypes.TextVariationPassword
                | global::Android.Text.InputTypes.ClassText;
            }

            new AlertDialog.Builder(Forms.Context)
                .SetTitle(title)
                .SetMessage(message)
                .SetView(editText)
                .SetNegativeButton(cancel, (o, e) => tcs.SetResult(new EntryAlertResult
                {
                    PressedButtonTitle = cancel,
                    Text = editText.Text
                }))
                .SetPositiveButton(accepte, (o, e) => tcs.SetResult(new EntryAlertResult
                {
                    PressedButtonTitle = accepte,
                    Text = editText.Text
                }))
                .Show();

            return tcs.Task;
        }
    }

}










