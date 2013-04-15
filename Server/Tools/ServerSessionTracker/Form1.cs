using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace ServerSessionTracker
{
    public partial class Form1 : Form
    {
        Mutex _mutex;

        public Form1()
        {
            InitializeComponent();

            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var mutexsecurity = new MutexSecurity();
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.ChangePermissions, AccessControlType.Deny));
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.Delete, AccessControlType.Deny));

            bool created;
            _mutex = new Mutex(true, "Global\\788c23ce-8a6d-4024-b498-2f1bace634f2", out created, mutexsecurity);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mutex.ReleaseMutex();
        }
    }
}
