using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Snarl;

namespace NativeWindowApplication
{

    // Summary description for WittySnarlMsgWnd.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]

    public class snarlMsgWnd : NativeWindow
    {
        CreateParams cp = new CreateParams();

        public int SNARL_GLOBAL_MESSAGE;
        
        private string iconPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\LastFmSnarls.ico";
        

        public snarlMsgWnd()
        {
            // Create the actual window
            this.CreateHandle(cp);
            this.SNARL_GLOBAL_MESSAGE = Snarl.SnarlConnector.GetGlobalMsg();
            
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == this.SNARL_GLOBAL_MESSAGE)
            {
                if ((int)m.WParam == Snarl.SnarlConnector.SNARL_LAUNCHED)
                {
                    Snarl.SnarlConnector.GetSnarlWindow(true);

                    SnarlConnector.RegisterConfig(this.Handle, "last.fm snarls", Snarl.WindowsMessage.WM_USER + 58, iconPath);

                    SnarlConnector.RegisterAlert("last.fm snarls", "Greeting");
                    SnarlConnector.RegisterAlert("last.fm snarls", "Now being played track");
                    SnarlConnector.RegisterAlert("last.fm snarls", "Recently played track");
                    SnarlConnector.RegisterAlert("last.fm snarls", "Connection error");
                    SnarlConnector.RegisterAlert("last.fm snarls", "Debug messages");
                }
            }
            base.WndProc(ref m);

        }

    }

}
