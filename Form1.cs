using System.Runtime.InteropServices;

namespace CursorLockerTrayApp;

public partial class Form1 : Form
{
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private Thread lockThread;
    private bool isLocking = false;
    private bool stopRequested = false;

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(Keys vKey);

    public Form1()
    {
        InitializeComponent();

        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Start Lock", null, ToggleLock);
        trayMenu.Items.Add("Exit", null, ExitApp);

        trayIcon = new NotifyIcon()
        {
            Text = "Cursor Locker",
            Icon = new Icon("trayicon.ico"),
            ContextMenuStrip = trayMenu,
            Visible = true
        };

        // Hide main form
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Load += (s, e) => this.Hide();
    }

    private void ToggleLock(object? sender, EventArgs e)
    {
        if (!isLocking)
        {
            StartLocking();
            trayMenu.Items[0].Text = "Stop Lock";
        }
        else
        {
            StopLocking();
            trayMenu.Items[0].Text = "Start Lock";
        }
    }

    private void StartLocking()
    {
        stopRequested = false;
        isLocking = true;
        lockThread = new Thread(() =>
        {
           while (!stopRequested)
        {
            // Move cursor to center
            int centerX = Screen.PrimaryScreen.Bounds.Width / 2;
            int centerY = Screen.PrimaryScreen.Bounds.Height / 2;
            SetCursorPos(centerX, centerY);

            // Check if ESC is pressed
            if ((GetAsyncKeyState(Keys.Escape) & 0x8000) != 0)
            {
                Invoke(() =>
                {
                    StopLocking();
                    trayMenu.Items[0].Text = "Start Lock";
                    trayIcon.ShowBalloonTip(1000, "Cursor Locker", "Lock stopped (ESC pressed)", ToolTipIcon.Info);
                });
                break;
            }

            Thread.Sleep(10);
        }
        });
        lockThread.IsBackground = true;
        lockThread.Start();
    }

    private void StopLocking()
    {
        stopRequested = true;
        isLocking = false;
    }

    private void ExitApp(object? sender, EventArgs e)
    {
        StopLocking();
        trayIcon.Visible = false;
        Application.Exit();
    }
}
