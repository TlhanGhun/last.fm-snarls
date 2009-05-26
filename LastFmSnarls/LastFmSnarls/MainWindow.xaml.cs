using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using Snarl;
using LastFmLib;
using LastFmLib.General;
using LastFmLib.API20.Types;
using LastFmLib.API20;

namespace LastFmSnarls
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IntPtr hwnd = IntPtr.Zero;
        private string versionString = "1.0 Beta 2";
        private static string iconPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\LastFmSnarls.ico";
        private NativeWindowApplication.snarlMsgWnd snarlComWindow;
        private static string userNameString = "";
        private static bool DEBUG = true;

        private Thread backgroundWorker = new Thread(monitorUser);

        public MainWindow()
        {
            InitializeComponent();
            
            this.Title = "last.fm snarls " + versionString;
            if (hwnd == IntPtr.Zero)
            {
                snarlComWindow = new NativeWindowApplication.snarlMsgWnd();
                hwnd = snarlComWindow.Handle;
            }
                    
            SnarlConnector.RegisterConfig(hwnd, "last.fm snarls", Snarl.WindowsMessage.WM_USER + 58, iconPath);

            SnarlConnector.RegisterAlert("last.fm snarls", "Greeting");
            SnarlConnector.RegisterAlert("last.fm snarls", "Now being played track");
            SnarlConnector.RegisterAlert("last.fm snarls", "Recently played track");
            SnarlConnector.RegisterAlert("last.fm snarls", "Connection error");
            if (DEBUG)
            {
                SnarlConnector.RegisterAlert("last.fm snarls", "Debug messages");
            }
            
        }

        ~MainWindow()
        {
            SnarlConnector.RevokeConfig(hwnd);
            if(hwnd != IntPtr.Zero) {
                snarlComWindow.DestroyHandle();
            }
            
                backgroundWorker.Abort();
                
                
            
        }

        static void monitorUser()
        {
            int lastConnectionErrorId = 0;

            MD5Hash key = new MD5Hash("1ca43b465fcb2306f51f4df7c010067c", true, Encoding.ASCII);
            MD5Hash secret = new MD5Hash("8c225ea33ff798e680368c2cd45a5f1e", true, Encoding.ASCII);
            AuthData myAuth = new AuthData(key, secret);
            Settings20.AuthData = myAuth;

            LastFmClient client = LastFmClient.Create(myAuth);
          //  client.LastFmUser.Username = "xxxx";
          //  client.LastFmUser.EncryptAndSetPassword("xxxx");

            RecentTrack lastPlaying = null;
            RecentTrack lastRecent = null;


            while (true)
            {
                try
                {
                    List<RecentTrack> recentTracks = client.User.GetRecentTracks(userNameString, 2);

                    RecentTrack nowPlaying = null;
                    RecentTrack lastTrack = null;

                    recentTracks.Reverse();

                    foreach (RecentTrack currentTrack in recentTracks)
                    {

                        if (currentTrack.NowPlaying)
                        {
                            nowPlaying = currentTrack;
                        }
                        else
                        {
                            lastTrack = currentTrack;
                        }

                    }

                    lastConnectionErrorId = 0;

                    if (nowPlaying != null && lastPlaying != nowPlaying)
                    {

                        string artworkPath = getArtworkPath(nowPlaying);
                        SnarlConnector.ShowMessageEx("Now being played track", nowPlaying.Artist.Name.ToString(), nowPlaying.Title.ToString() + "\n\n" + nowPlaying.Album.ToString(), 10, artworkPath, hwnd, Snarl.WindowsMessage.WM_USER + 11, "");
                        lastPlaying = nowPlaying;
                        if (artworkPath != iconPath)
                        {
                            System.IO.File.Delete(artworkPath);
                        }
                    }
                    if (lastTrack != null && lastRecent != lastTrack)
                    {
                        string artworkPath = getArtworkPath(lastTrack);
                        SnarlConnector.ShowMessageEx("Recently played track", lastTrack.Artist.Name.ToString(), lastTrack.Title.ToString() + "\n\n" + lastTrack.Album.ToString(), 10, artworkPath, hwnd, Snarl.WindowsMessage.WM_USER + 12, "");
                        lastRecent = lastTrack;
                        if (artworkPath != iconPath)
                        {
                            System.IO.File.Delete(artworkPath);
                        }
                    }
                    Thread.Sleep(2000);
                }


                catch (Exception exp)
                {
                    if (lastConnectionErrorId == 0)
                    {
                        lastConnectionErrorId = SnarlConnector.ShowMessageEx("Connection error", "Connection to last.fm failed", "Connection to the last.fm can't be established. Maybe they are down or your internet connection is not available", 20, iconPath, hwnd, Snarl.WindowsMessage.WM_USER + 13, "");
                        if (DEBUG)
                        {
                            SnarlConnector.ShowMessageEx("Debug message", "Error message", exp.Message, 0, iconPath, hwnd, Snarl.WindowsMessage.WM_USER + 77, "");
                            SnarlConnector.ShowMessageEx("Debug message", "Error source", exp.Source, 0, iconPath, hwnd, Snarl.WindowsMessage.WM_USER + 77, "");
                            SnarlConnector.ShowMessageEx("Debug message", "Stack trace", exp.StackTrace.Substring(0, 500), 0, iconPath, hwnd, Snarl.WindowsMessage.WM_USER + 77, "");
                        }
                    }
                }
            }
        }

            private static string getArtworkPath(RecentTrack thisTrack) {
                string filenameArtwork = "";
                System.Drawing.Bitmap artwork = thisTrack.DownloadImage(modEnums.ImageSize.MediumOrSmallest);
                if (artwork != null)
                {
                    filenameArtwork = System.IO.Path.GetTempFileName();
                    artwork.Save(filenameArtwork);
                    return filenameArtwork;
                }
                else
                {
                    return iconPath;
                }
            }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.Start();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            userName.IsEnabled = false;
        }



        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.Abort();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            userName.IsEnabled = true;
            backgroundWorker = new Thread(monitorUser);
        }

        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox temp = (System.Windows.Controls.TextBox)sender;
            if(temp.Text != "") {
                userNameString = temp.Text;
                startButton.IsEnabled = true;
            }
            else
            {
                startButton.IsEnabled = false;
            }

        }

    }
}
