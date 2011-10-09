using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Vre.Server
{
    public partial class FrmMain : Form
    {
        #region p/invokes to disable form close
        const int MF_BYPOSITION = 0x400;

        [DllImport("User32")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32")]
        private static extern int GetMenuItemCount(IntPtr hWnd);
        #endregion

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // Disable 'X' close button on window.
            //
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);

#if !DEBUG
            Visible = false;
#endif
        }

        private void tmrStartup_Tick(object sender, EventArgs e)
        {
            tmrStartup.Enabled = false;

            lblStatus.Text = "Starting...";
            pbStateTransition.Visible = true;
            Update();

            start();

            pbStateTransition.Visible = false;
            lblStatus.Text = "Running in User Mode.";

            btnStop.Enabled = true;
            updateStatus1();

            notifyIcon.Visible = true;
#if DEBUG
            tmrRefresh.Enabled = true;
#else
            Hide();
            notifyIcon.ShowBalloonTip(5000);            
#endif
        }

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            updateStatus2();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            tmrRefresh.Enabled = false;
            lblStatus.Text = "Stopping...";
            pbStateTransition.Visible = true;
            Update();

            stop();

            Application.Exit();
        }

        //private void startupEventHandler(object sender, StateTransitionEventArgs arg)
        //{
        //    if (this.InvokeRequired)
        //        this.Invoke(new updateState(updateTransitionState), arg);
        //    else
        //        updateTransitionState(arg);
        //}

        //private delegate void updateState(StateTransitionEventArgs arg);

        //private void updateTransitionState(StateTransitionEventArgs arg)
        //{
        //    pbStateTransition.Value = arg.Percent;
        //    lblStatus.Text = arg.Text;
        //    Update();
        //}

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                tmrRefresh.Enabled = false;
                Hide();
            }
            else
            {
                tmrRefresh.Enabled = true;
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left == e.Button)
            {
                notifyIcon.ShowBalloonTip(5000);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
        }

        private void start()
        {
            ServiceRunner.Start();
        }

        private void stop()
        {
            ServiceRunner.Stop();
        }

        private void updateStatus1()
        {
            Text = "Vre Server";

            //lblListeningAddress.Text = Program.Configuration.GetConfig("Communications", "ListenIpAddress", "0.0.0.0");

            lblServiceStatus.Text = ServiceRunner.Status;

            string endpoints = string.Empty;
            foreach (string ep in ServiceRunner.Listeners) endpoints += ep + "\r\n";

            tbListeners.Text = endpoints;

            notifyIcon.Text = Text;
            notifyIcon.BalloonTipIcon = ToolTipIcon.None;
            notifyIcon.BalloonTipTitle = Text;
            notifyIcon.BalloonTipText = string.Format(
                "Running in User Mode\r\n"
                + "Status: {0}\r\n"
                + "Endpoints:\r\n{1}"
                , lblServiceStatus.Text, endpoints);
            notifyIcon.Icon = Icon;
            notifyIcon.ContextMenuStrip = notificationMenu;
        }

        private void updateStatus2()
        {
            lblServiceStatus.Text = ServiceRunner.Status;

            tbExtendedStatus.Text = ServiceRunner.ExtendedStatus;
        }
    }
}
