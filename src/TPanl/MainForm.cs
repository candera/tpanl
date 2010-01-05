using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Reflection;
using TPanl.Net;

namespace TPanl
{
    public partial class MainForm : Form
    {
        private bool _allowClose;
        private readonly OpenFileDialog _loadProfileDialog = new OpenFileDialog();
        private readonly List<TimerAction> _pendingActions = new List<TimerAction>();
        private bool _profileLoaded;
        private bool _socketClientConnected = false; 

        public MainForm() : this(null)
        {
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            if (args != null && args.Length > 0)
            {
                LoadProfile(args[0]); 
            }
        }

        public void SendKeys(KeyEventSequence keys)
        {
            Log(string.Format("Sending {0}", keys.ToString()));
            Win32.INPUT[] inputs = keys.ToInputArray();
            uint result = Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Win32.INPUT)));

            Log("SendInput reports it sent this many events: " + result.ToString());
            ShowKeysSending(); 
            //SetIcon("green-yellow-yellow", TimeSpan.FromMilliseconds(125), "green-yellow-black"); 
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_allowClose)
            {
                PutToTray();
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void DrawFilledRegion(Graphics g, Pen pen, Brush brush, int x, int y, int width, int height)
        {
            g.FillRectangle(brush, x, y, width, height);
            g.DrawRectangle(pen, x, y, width, height); 
        }
        private byte GetByte(int data, int i)
        {
            return (byte)((data >> (i * 8)) & 0xff);
        }
        private Icon GetIcon()
        {
            Bitmap bitmap = new Bitmap(32, 32);
            Graphics g = Graphics.FromImage(bitmap);

            g.Clear(Color.Transparent);
            g.DrawRectangle(Pens.Black, new Rectangle(3, 3, 28, 28));
            Brush brush;
            if (_profileLoaded)
            {
                brush = Brushes.Green;
            }
            else
            {
                brush = Brushes.Red;
            }
            g.FillRectangle(brush, new Rectangle(4, 4, 26, 26));

            DrawFilledRegion(g, Pens.Black, _socketClientConnected ? Brushes.Yellow : Brushes.Black, 13, 13, 8, 8);

            return Icon.FromHandle(bitmap.GetHicon());
        }
        private void LoadProfile(string path)
        {
            path = Path.GetFullPath(path); 
            using (StreamReader reader = File.OpenText(path))
            {
                string line = null;
                int lineNumber = 0;
                ClickCommandMap commandMap = new ClickCommandMap();
                while ((line = reader.ReadLine()) != null)
                {
                    ++lineNumber;

                    // Ignore empty lines
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    // Ignore comment lines
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] parts = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 2)
                    {
                        Log("Ignoring line " + lineNumber.ToString() + ". Syntax error: " + line);
                        continue;
                    }

                    string button = parts[0].Trim();
                    string specification = parts[1].Trim();

                    if (!button.Contains("/"))
                    {
                        button = "socket/" + button; 
                    }

                    button = button.ToLowerInvariant(); 

                    KeyEventSequence eventSequence = KeySequenceParser.Parse(specification);

                    if (eventSequence == null)
                    {
                        Log("Ignoring line " + lineNumber.ToString() + ". Syntax error: " + line);
                    }

                    commandMap[button] = eventSequence;

                }
                Command.UnregisterAll(); 
                Command.Register("CLICK", new ClickCommandFactory(commandMap));
                Log("Loaded profile " + path);

                _profileLoaded = true;
                SetIcon(); 
            }
        }
        private void Log(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(Log), message);
            }
            else
            {
                textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss:") + message);
                textBox1.AppendText("\r\n");
            }
        }
        private void PutToTray()
        {
            Hide();
            ShowInTaskbar = false;
        }
        private void RestoreFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Activate(); 
        }

        private void WriteResponse(StreamWriter writer, string status, IDictionary<string, object> headers, string body)
        {
            writer.WriteLine("HTTP/1.1 " + status);
            if (body != null)
            {
                writer.WriteLine("Content-Length: {0}", body.Length);
            }
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    writer.WriteLine("{0}: {1}", header.Key, header.Value); 
                }
            }

            writer.WriteLine();

            if (body != null)
            {
                writer.WriteLine(body); 
            }

            Log("Wrote response");

            writer.Flush(); 
        }
        private void SetIcon()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(SetIcon));
            }
            else
            {
                notifyIcon1.Icon = GetIcon(); 
            }
        }
        private void ShowKeysSending()
        {
            throw new NotImplementedException();
        }
     
        private void SetStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatus), message);
            }
            else
            {
                toolStripStatusLabel1.Text = message;
                notifyIcon1.Text = "Keylay - " + message;
                Log(message);
            }

        }
        private void UnloadProfile()
        {
            Command.UnregisterAll();
            _profileLoaded = false;
            SetIcon(); 
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var listener = SimpleHttpListener.Start(0xFAC0, HandleHttpRequest, Log);
            Log("HTTP Listener started");
        }

        private void HandleHttpRequest(SimpleHttpContext context)
        {
            if (context.Request.Method.Equals("GET") && context.Request.RawUrl.Equals("/"))
            {
                string content = "<html><body><h1>Test!</h1></body></html>";
                context.Write(content);
            }
            else
            {
                context.Response.StatusCode = HttpStatusCode.NotFound;
                context.Response.StatusDescription = "Not Found";
            }

        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                PutToTray();
            }
        }
        private void miDisplayDiagnostics_Click(object sender, EventArgs e)
        {
            RestoreFromTray(); 
        }
        private void miExit_Click(object sender, EventArgs e)
        {
            _allowClose = true; 
            Close(); 
        }
        private void miLoadProfile_Click(object sender, EventArgs e)
        {
            _loadProfileDialog.Multiselect = false;

            if (_loadProfileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadProfile(_loadProfileDialog.FileName);
            }
        }
        private void miUnloadProfile_Click(object sender, EventArgs e)
        {
            UnloadProfile(); 
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_pendingActions.Count == 0)
            {
                return; 
            }

            List<TimerAction> actionsDue = new List<TimerAction>();

            foreach (TimerAction action in _pendingActions)
            {
                if (action.When <= DateTime.Now)
                {
                    actionsDue.Add(action); 
                }
            }

            foreach (TimerAction actionDue in actionsDue)
            {
                actionDue.Action();
                _pendingActions.Remove(actionDue); 
            }

        }

    }
}
