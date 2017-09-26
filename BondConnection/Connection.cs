using System;
using System.IO;
using System.Text;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

using Sockets;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;




namespace BondConnection
{
    public partial class Connection : ContentPage
    {
        static int Port = 4096;
        int i = 0;




        Editor Listener_Disp = new Editor();
        Button Listener_Rady = new Button();
        Button Listener_Send = new Button();
        Editor Client_Disp = new Editor();
        Button Client_Send = new Button();
        Button Client_Conect = new Button();
        Button plainTextButton = new Button();


        ITcpSocketClient serverSocket;

        System.IO.Stream clientSocket;

        //クライアント側のストリーム
        private StreamWriter clientWriter = null;
        private StreamReader clientReader = null;

        //サーバ側のストリーム
        //private StreamWriter serverWriter = null;
        // private StreamReader serverReader = null;

        //サーバスレッド
        // Thread threadServer = null;

        //クライアントスレッド
        // Thread threadClient = null;


        //StreamSocketListener listener;

        //HostName localHost;

        //string port = "5000";

        //StreamSocketListener listener;

        //HostName localHost;

        //string port = "5000";





        public Connection()
        {
            ConnectionDisp();
        }

        /// <summary>
        /// Connections the disp.
        /// </summary>
        public void ConnectionDisp()
        {
            double top;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    top = 20;
                    //BackgroundColor = Color.Black;
                    break;
                case Device.Android:
                case Device.WinPhone:
                // case Device.Windows:
                default:
                    top = 0;
                    break;
            }
            /*layout.Margin*/
            Padding = new Thickness(5, top, 5, 0);
            //BackgroundColor = Color.OrangeRed;


            Listener_Disp = new Editor()
            {
                Text = "ListenerText",
            };

            Listener_Rady = new Button()
            {
                Text = "待ち受け開始",
                BackgroundColor = Color.DarkGray,
                BorderColor = Color.Black

            };
            Listener_Rady.Clicked += (object sender, EventArgs e) => ListenerConnection(sender, e);

            Listener_Send = new Button()
            {
                Text = "送信",
                BackgroundColor = Color.DarkGray,
                TextColor = Color.Blue,
                BorderColor = Color.Black,
                IsEnabled = false
                    

            };
            Listener_Send.Clicked += async (object sender, EventArgs e) =>
            {
                var Result = await DependencyService.Get<IEntryAlertService>().Show("コメント", "入力後,送信を押して下さい", "送信", "Cancel", false);
                string anser = string.Format("{0}:{1}", Result.PressedButtonTitle, Result.Text);
                Listener_MessageSend(sender, e, anser);
            };

            BoxView LineBox = new BoxView()
            {
                Color = Color.OrangeRed,
                WidthRequest = 300,
                HeightRequest = 5,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand

            };



            Client_Disp = new Editor()
            {
                Text = "ConectText"
            };


            Client_Conect = new Button()
            {
                Text = "接続開始",
                TextColor = Color.Black,
            };
            Client_Conect.Clicked += (object sender, EventArgs e) =>
            {
                ClientConnection(sender, e);

            };

            ///
            Client_Send = new Button()
            {
                //IsEnabled = false,
                Text = "コメント作成／送信"

            };
            Client_Send.Clicked += async (object sender, EventArgs e) =>
            {
                var Result = await DependencyService.Get<IEntryAlertService>().Show("コメント", "入力後,送信を押して下さい", "送信", "Cancel", false);
                string anser = string.Format("{0}:{1}", Result.PressedButtonTitle, Result.Text);
                ClientConnection_MessageSend(sender, e, anser);
            };

            var buttonStartGps = new Button
            {
                Text = "Start GPS"
            };
            buttonStartGps.Clicked += (sender, e) =>
            {
                var geoLocator = DependencyService.Get<IGeolocator>();
                geoLocator.LocationReceived += (_, args) =>
                {
                    Client_Disp.Text = String.Format("{0:0.00}/{1:0.00}", args.Latitude, args.Longitude);
                };
                geoLocator.StartGps();
            };
            //消費カロリー（kcal）＝1.05×METs（メッツ）×時間×体重(kg)



            //var label = new Label();
            //var plainTextButton = new Button { Text = "Show plain text dialog" };
            //var passwordButton = new Button { Text = "Show password dialog" };

            //plainTextButton.Clicked += async (sender, e) => {
            //    var result = await DependencyService.Get<IEntryAlertService>().Show(
            //        "Prain text", "Please enter text.", "OK", "Cancel", false);
            //    label.Text = string.Format("{0}:{1}", result.PressedButtonTitle, result.Text);
            //};

            //passwordButton.Clicked += async (sender, e) => {
            //    var result = await DependencyService.Get<IEntryAlertService>().Show(
            //        "Password", "Please enter password.", "OK", "Cancel", true);
            //    label.Text = string.Format("{0}:{1}", result.PressedButtonTitle, result.Text);
            //};

            //Content = new StackLayout
            //{
            //    Orientation = StackOrientation.Vertical,
            //    VerticalOptions = LayoutOptions.CenterAndExpand,
            //    HorizontalOptions = LayoutOptions.CenterAndExpand,
            //    Children = { label, plainTextButton, passwordButton }
            //};






            StackLayout On = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
                           // label,
                           // plainTextButton,
                           // passwordButton,
                                Listener_Disp,
                                Listener_Rady,
                                Listener_Send,
                                LineBox,
                                Client_Disp,
                                Client_Conect,
                                Client_Send,
                                buttonStartGps,

                                new Label {
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    Text = "Welcome to Xamarin Forms!"
                                },
                    }
            };
            Content = On;
        }







        /// <summary>
        /// Listeners the connection.
        /// </summary>
        /// <param name="s">S.</param>
        /// <param name="e">E.</param>
        public void ListenerConnection(Object s, EventArgs e)
        {

            bool disconnected = false;
            var listener = new TcpSocketListener();
            Listener_Rady.Text = "接続待機中";
            Listener_Rady.TextColor = Color.Blue;



            // when we get connections, read bytes until we get -1 (eof)
            listener.ConnectionReceived += async (sender, args) =>
            {
                //サーバー用ソケットの取得
                serverSocket = /*(Sockets.Plugin.TcpSocketClient)*/args.SocketClient;//*/ GetSocketClient(args);

                string Adress = args.SocketClient.RemoteAddress;
                var port = args.SocketClient.RemotePort;
                var bytesRead = -1;
                var buf = new byte[1024];

                // read from the 'ReadStream' property of the socket client to receive data
                bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, buf.Length);

                //受信したデータを文字列に変換
                Encoding enc = System.Text.Encoding.UTF8;
                string resMsg = enc.GetString(buf, 0, bytesRead);

                //末尾の\nを削除
                resMsg = resMsg.TrimEnd('\n');

                if (resMsg == "+OK to BondotConnectio!" | disconnected == false)// ID Check
                {
                    string listenerMsg = string.Format("クライアントに接続しました IpAdress{0} Port {1}" + '\n', Adress, port);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("For listener Message ", listenerMsg, "OK");

                        Listener_Rady.Text = "接続中";
                        Listener_Rady.TextColor = Color.Red;
                        Listener_Send.IsEnabled = true;
                    });

                    //クライアントに接続応答する
                    //NetworkStreamを取得
                    System.IO.Stream writer = serverSocket.GetStream();

                    //クライアントに送信する文字列を作成
                    string sendMsg = resMsg;
                    //文字列をByte型配列に変換
                    byte[] sendBytes = enc.GetBytes(sendMsg + "listener-Anser" + '\n');
                    //データを送信する
                    writer.Write(sendBytes, 0, sendBytes.Length);
                    disconnected = true;
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Listener_Disp.Text = resMsg;
                        DisplayAlert("For Client Message ", resMsg, "OK");
                    });
                }
                //閉じる
                // ns.Dispose();
                // serverSocket.Dispose();
             };

            //IPアドレスとポートをバインドして接続待機
            /// bind to the listen port across all interfaces
            listener.StartListeningAsync(Port);

        }








        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Listener_MessageSend(object sender, EventArgs args, string msg)
        {
            //クライアントに接続応答する
            //NetworkStreamを取得
            var writer = serverSocket.GetStream();


            //送信文字列をByte型配列に変換
            Encoding enc = System.Text.Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(msg + '\n');
            //データを送信する
            writer.Write(sendBytes, 0, sendBytes.Length);
            writer.FlushAsync();
        }





//********************************************************************************************
//********************************************** Client **************************************





        /// <summary>
        /// Clients the connection.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        public async void ClientConnection(Object sender, EventArgs args)
        {
            var address = "127.0.0.1";


            TcpSocketClient client = new TcpSocketClient();

            await client.ConnectAsync(address, Port);


            //NetworkStreamを取得する
            // Uses the GetStream public method to return the NetworkStream.
            clientSocket = client.GetStream();
            //クライアントがサーバとのデータのやり取りに使うストリームを取得
            System.IO.Stream ns = client.GetStream();

            clientReader = new StreamReader(ns, Encoding.UTF8);
            clientWriter = new StreamWriter(ns, Encoding.UTF8);



            var sendMsg = "+OK to BondotConnectio!";

            //サーバーにデータを送信する
            //文字列をByte型配列に変換
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(sendMsg + '\n');


            //データを送信する************************************
            clientSocket.Write(sendBytes, 0, sendBytes.Length);
            clientSocket.Flush();

            Client_Disp.Text = sendMsg;
            Client_Send.IsEnabled = true;



            //リステナーからのデータを受信する
            var buf = new byte[1024];
            var r = clientSocket.Read(buf, 0, buf.Length);

            ////受信したデータを文字列に変換
            Encoding enr = System.Text.Encoding.UTF8;
            string resMsg = enr.GetString(buf, 0, r);

            ////末尾の\nを削除
            resMsg = resMsg.TrimEnd('\n');
            Client_Disp.Text = resMsg;


            if (resMsg == "+OK to BondotConnectio!listener-Anser")//Client ID Check
            {
                await DisplayAlert("For Client Message ", "Listenerと接続しました", "OK");
                Client_Conect.Text = "接続中";
                Client_Conect.IsEnabled = false;
                Client_Conect.TextColor = Color.Red;

                Device.BeginInvokeOnMainThread(() =>
               {
                   Task.Run(() =>
                   {
                       ClientListen(clientReader);//  重たい処理のつもり*/
                   });
               });

            }







            //クライアントに接続応答する***********************
            // System.IO.Stream ns = serverSocket.GetStream();
            //serverWriter = new StreamWriter(ns, Encoding.UTF8);

            // sendMsg = resMsg + "listener-Anser" + '\n';


            //文字列をByte型配列に変換
            // byte[] sendBytes = enc.GetBytes(sendMsg + "listener-Anser" + '\n');
            // SendMessage(serverWriter, sendMsg);

            // wait a little before sending the next bit of data
            //await Task.Delay(TimeSpan.FromMilliseconds(500));
            //await client.DisconnectAsync();

        }





        //クライアント側で、サーバからのデータ送信を待機する
        private void ClientListen(StreamReader reader)
        {
            try
            {
                while (true)
                {
                    //受け取ったメッセージを処理する
                    lock (this)
                    {
                        //受信するデータがない場合は、データが読み込み可能になるまでここで待機することになる。
                        //データが受信可能な状態になると、受信したデータは文字列messageに格納される。
                        string recevemessage = reader.ReadLine();
                        if (recevemessage != null)
                        {
                            Debug.WriteLine(i.ToString() + recevemessage);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Client_Disp.Text = recevemessage;
                                DisplayAlert("For Sever Message ", recevemessage, "OK");
                            });
                            i = ++i;

                        }
                        //return message;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message + "\r\n" + exception.StackTrace);
            }

        }



        /// <summary>
        /// Clients the message send.
        /// </summary>
        /// <param name="s">S.</param>
        /// <param name="e">E.</param>
        public async void ClientConnection_MessageSend(Object s, EventArgs e, string anser)
        {
            var address = "127.0.0.1";
            TcpSocketClient client = new TcpSocketClient();

            await client.ConnectAsync(address, Port);

            //NetworkStreamを取得する
            // Uses the GetStream public method to return the NetworkStream.
            System.IO.Stream gs = client.GetStream();

            //文字列をByte型配列に変換
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(anser + '\n');


            //データを送信する
            gs.Write(sendBytes, 0, sendBytes.Length);

            await client.WriteStream.FlushAsync();
            //await client.DisconnectAsync();

        }


    }

}



/*
//Functions called by 2 buttons "Connect" and "Send"

//This works. I connect.
public async void SocketConnect(object sender, EventArgs args)
{
    try
    {
        client = new TcpSocketClient();
        await client.ConnectAsync(ipEntry.Text, int.Parse(portEntry.Text));
        // we're connected!
        connectBtn.Text = "Connected";
    }
    catch (Exception e)
    {
        var notificator = DependencyService.Get<IToastNotificator>();
        bool tapped = await notificator.Notify(ToastNotificationType.Error,
            "Error", e.Message, TimeSpan.FromSeconds(10));
    }

}

public async void SocketSend(object sender, EventArgs args)
{
    try
    {
        if (client == null)
        {
            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(ToastNotificationType.Error,
                "Error", "Connect First Please", TimeSpan.FromSeconds(10));
        }
        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(toSendEntry.Text);
        using (Stream s = client.WriteStream)
        {
            s.Write(toSend, 0, toSend.Length);
            await s.FlushAsync();
        } //Fails Here with Operation is not supported.
        await Task.Delay(70);
        using (Stream s = client.ReadStream)
        {
            if (s.Length > 0)
            {
                byte[] response = new byte[s.Length];
                s.Read(response, 0, (int)s.Length);
                responseFromServer.Text = response.ToString();
            }
        }
    }
    catch (Exception e)
    {
        var notificator = DependencyService.Get<IToastNotificator>();
        bool tapped = await notificator.Notify(ToastNotificationType.Error,
            "Error", e.Message, TimeSpan.FromSeconds(10));
    }

}
*/


/*
public sealed partial class MainPage : Page
{
    StreamSocket serverSocket;

    StreamSocket clientSocket;

    StreamSocketListener listener;

    HostName localHost;

    string port = "5000";

    public MainPage()
    {
        this.InitializeComponent();
        localHost = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();
    }

    private async void btnListen_Click(object sender, RoutedEventArgs e)
    {

        listener = new StreamSocketListener();

        listener.ConnectionReceived += (ss, ee) =>
        {
            serverSocket = ee.Socket;
            Debug.WriteLine("connected {0}", serverSocket.Information.RemoteAddress);
        };

        await listener.BindEndpointAsync(localHost, port);

        Debug.WriteLine("listen...");

    }

    private async void btnClientConnection_Click(object sender, RoutedEventArgs e)
    {
        clientSocket = new StreamSocket();
        await clientSocket.ConnectAsync(localHost, port);
        Debug.WriteLine("connected!");


    }

    private async void btnServerRecv_Click(object sender, RoutedEventArgs e)
    {

        var reader = new DataReader(serverSocket.InputStream);

        uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));

        uint size = reader.ReadUInt32();

        uint sizeFieldCount2 = await reader.LoadAsync(size);

        var str = reader.ReadString(sizeFieldCount2);

        Debug.WriteLine("server receive {0}", str);
    }

    private async void btnClientRecv_Click(object sender, RoutedEventArgs e)
    {

        var reader = new DataReader(clientSocket.InputStream);
        uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));

        uint size = reader.ReadUInt32();

        uint sizeFieldCount2 = await reader.LoadAsync(size);

        var str = reader.ReadString(sizeFieldCount2);

        Debug.WriteLine("client receive {0}", str);
    }

    private async void btnServerSend_Click(object sender, RoutedEventArgs e)
    {
        var writer = new DataWriter(serverSocket.OutputStream);
        string str = "ほげほげ";
        writer.WriteUInt32(writer.MeasureString(str));
        writer.WriteString(str);
        await writer.StoreAsync();

        Debug.WriteLine("server send");
    }

    private async void btnClientSend_Click(object sender, RoutedEventArgs e)
    {
        var writer = new DataWriter(clientSocket.OutputStream);
        string str = "ほげほげ";
        writer.WriteUInt32(writer.MeasureString(str));
        writer.WriteString(str);
        await writer.StoreAsync();

        Debug.WriteLine("client send");
    }
}


## Socketを閉じる

Socketを閉じるときはDisposeメソッドを呼びます。


serverSocket.Dispose();
clientSocket.Dispose();
listener.Dispose();
1
2
3
serverSocket.Dispose();
clientSocket.Dispose();
listener.Dispose();
*/
