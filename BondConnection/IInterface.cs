using System;
using System.Threading.Tasks;


namespace BondConnection
{
   public interface TcpIpSocket1
    {
        //TcpServer TS = new TcpServer();

        Task<string> ClientConnect();
        string getstring();

        void ServerConnect();
    }

    public interface ITcpSocket1
    {
        //Task ServerConnect();
        void ServerConnect();
        string SeverToConnect();
        Task ClientConnect();
        //Task<string> ClientConnect1();
        void SeverIpadressSet(string arg,int port);
        string SeverToReceive();
        string[] GetConectIp();
       

        bool IsConnected { get; }
        string getIPAddress();
        string OsVersion { get; }

       
        //event LocationEventHandler LocationReceived;


    }


    // イベントのパラメーター
    public class LocationEventArgs : EventArgs
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    // 位置を受信した際のイベントハンドラー
    public delegate void LocationEventHandler(object sender, LocationEventArgs args);

    // GPS を利用するための、共通なインターフェース
    public interface IGeolocator
    {
        void StartGps();
        event LocationEventHandler LocationReceived;
        string getIPAddress();
    }


    public interface IEntryAlertService
    {
        Task<EntryAlertResult> Show(string title, string message,
          string accepte, string cancel, bool isPassword = false);
    }

    public class EntryAlertResult
    {
        public string PressedButtonTitle { get; set; }
        public string Text { get; set; }
    }

}
