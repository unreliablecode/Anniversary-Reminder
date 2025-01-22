using System;  
using System.Windows.Forms;  
using System.Runtime.InteropServices;  
  
namespace AnniversaryReminder  
{  
    public partial class MainForm : Form  
    {  
        private NotifyIcon trayIcon;  
        private ContextMenu trayMenu;  
        private bool isClosedByUser = false;  
        private Timer reminderTimer;  
  
        public MainForm()  
        {  
            InitializeComponent();  
            this.WindowState = FormWindowState.Minimized;  
            this.ShowInTaskbar = false;  
  
            // Initialize the NotifyIcon  
            trayIcon = new NotifyIcon()  
            {  
                Icon = new System.Drawing.Icon("icon.ico"), // Make sure you have an icon file named icon.ico  
                Visible = true,  
                Text = "Anniversary Reminder"  
            };  
  
            // Create a context menu for the tray icon  
            trayMenu = new ContextMenu();  
            trayMenu.MenuItems.Add("Exit", OnExit);  
  
            trayIcon.ContextMenu = trayMenu;  
  
            // Subscribe to the MouseClick event to show the form when the tray icon is clicked  
            trayIcon.MouseClick += TrayIcon_MouseClick;  
  
            // Set up the reminder timer  
            SetupReminderTimer();  
        }  
  
        private void SetupReminderTimer()  
        {  
            reminderTimer = new Timer();  
            reminderTimer.Interval = 60 * 1000; // Check every minute  
            reminderTimer.Tick += ReminderTimer_Tick;  
            reminderTimer.Start();  
        }  
  
        private void ReminderTimer_Tick(object sender, EventArgs e)  
        {  
            CheckForAnniversary();  
        }  
  
        private void CheckForAnniversary()  
        {  
            DateTime now = DateTime.Now;  
            DateTime anniversaryDate = new DateTime(now.Year, 1, 1);  //change to your own date
  
            // Check if today is May 19th  
            if (now.Date == anniversaryDate.Date)  
            {  
                // Define the reminder times  
                TimeSpan[] reminderTimes = new TimeSpan[]  
                {  
                    new TimeSpan(0, 0, 0),  // 00:00  
                    new TimeSpan(7, 0, 0),  // 07:00  
                    new TimeSpan(10, 0, 0), // 10:00  
                    new TimeSpan(14, 0, 0)  // 14:00  
                };  
  
                // Check if the current time matches any of the reminder times  
                foreach (TimeSpan reminderTime in reminderTimes)  
                {  
                    if (now.TimeOfDay == reminderTime)  
                    {  
                        ShowNotification("Happy Anniversary!");  
                    }  
                }  
            }  
        }  
  
        private void ShowNotification(string message)  
        {  
            trayIcon.ShowBalloonTip(5000, "Anniversary Reminder", message, ToolTipIcon.Info);  
        }  
  
        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)  
        {  
            if (e.Button == MouseButtons.Left)  
            {  
                this.WindowState = FormWindowState.Normal;  
                this.ShowInTaskbar = true;  
            }  
        }  
  
        private void OnExit(object sender, EventArgs e)  
        {  
            isClosedByUser = true;  
            Application.Exit();  
        }  
  
        protected override void OnFormClosing(FormClosingEventArgs e)  
        {  
            if (!isClosedByUser)  
            {  
                e.Cancel = true;  
                this.WindowState = FormWindowState.Minimized;  
                this.ShowInTaskbar = false;  
            }  
            else  
            {  
                trayIcon.Visible = false;  
                trayIcon.Dispose();  
            }  
        }  
  
        [DllImport("user32.dll", SetLastError = true)]  
        private static extern IntPtr GetForegroundWindow();  
  
        [DllImport("user32.dll", SetLastError = true)]  
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);  
  
        [DllImport("user32.dll", SetLastError = true)]  
        private static extern uint GetKeyboardLayout(uint idThread);  
  
        private bool IsAltF4Pressed()  
        {  
            IntPtr hWnd = GetForegroundWindow();  
            uint processId;  
            GetWindowThreadProcessId(hWnd, out processId);  
            uint keyboardLayout = GetKeyboardLayout(processId);  
  
            short keyState = (short)GetKeyState(0x12); // VK_MENU (Alt key)  
            short keyStateF4 = (short)GetKeyState(0x73); // VK_F4  
  
            return (keyState & 0x8000) != 0 && (keyStateF4 & 0x8000) != 0;  
        }  
  
        [DllImport("user32.dll", SetLastError = true)]  
        private static extern short GetKeyState(int nVirtKey);  
  
        protected override void WndProc(ref Message m)  
        {  
            const int WM_SYSCOMMAND = 0x0112;  
            const int SC_CLOSE = 0xF060;  
  
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)  
            {  
                if (IsAltF4Pressed())  
                {  
                    isClosedByUser = true;  
                }  
            }  
  
            base.WndProc(ref m);  
        }  
  
        [STAThread]  
        static void Main()  
        {  
            Application.EnableVisualStyles();  
            Application.SetCompatibleTextRenderingDefault(false);  
            Application.Run(new MainForm());  
        }  
    }  
}  
