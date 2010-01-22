using System;
using System.Linq; 
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
using System.Media;

namespace TPanl
{
    public partial class MainForm : Form
    {
        private Dictionary<string, string> _mimeTypes = new Dictionary<string, string>
            {
                { ".html", "text/html" },
                { ".jpg", "image/jpeg" },
                { ".js", "text/javascript" },
            };

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

            uint result = Win32.SendInput(keys); 

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

                    string button = parts[0].Trim().ToLowerInvariant();
                    string specification = parts[1].Trim();

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
                textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff:") + message);
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
            //throw new NotImplementedException();
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
            //Beep();

            if (context.Request.Method == "POST")
            {
                if (context.Request.Url.Path.Equals("/notify"))
                {
                    var region = context.Request.Url.QueryParameters["region"].First();

                    if (_profileLoaded)
                    {
                        var command = Command.Parse("CLICK " + region);

                        if (command == null)
                        {
                            Log("No key sequence found for " + region);
                            context.Response.StatusCode = HttpStatusCode.NotFound;
                            context.Response.StatusDescription = "Not Found";
                        }
                        else
                        {
                            Log("Sending keys for region " + region);
                            command.Execute(this); 
                        }
                    }
                    else
                    {
                        Log("No profile loaded!");
                        Beep(); 
                    }
                }
            }
            else if (context.Request.Method == "GET")
            {
                var resource = GetResourceForUrl(context.Request.Url.Path);

                if (resource == null)
                {
                    Log("Could not map resource for URL" + context.Request.Url);
                    context.Response.StatusCode = HttpStatusCode.NotFound;
                    context.Response.StatusDescription = "Not Found";
                }
                else
                {
                    using (var stream = GetResourceStream(resource))
                    {
                        if (stream == null)
                        {
                            Log("Could not load resource for " + resource);
                            context.Response.StatusCode = HttpStatusCode.NotFound;
                            context.Response.StatusDescription = "Not Found";
                            context.Write("Not found");
                        }
                        else
                        {
                            Log("Found resource " + resource);
                            context.Response.ContentType = GetContentTypeForResource(resource);
                            context.Write(stream);
                        }
                    }
                }
            }
            else
            {
                Log("No support for method " + context.Request.Method);
                context.Response.StatusCode = HttpStatusCode.MethodNotAllowed;
                context.Response.StatusDescription = "Method Not Allowed"; 
            }

        }

        private void Beep()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(Beep));
            }
            else
            {
                SystemSounds.Beep.Play(); 
            }
        }

        private string GetContentTypeForResource(string resource)
        {
            foreach (var mimeType in _mimeTypes)
            {
                if (resource.EndsWith(mimeType.Key))
                {
                    return mimeType.Value; 
                }
            }

            return null; 
        }

        private string GetResourceForUrl(Url url)
        {
            if (url.Equals("/"))
            {
                return "index.html";
            }
            else
            {
                // Remove leading slash
                return url.ToString().Substring(1);
            }
        }

        private Stream GetResourceStream(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType().Namespace + ".Content." + name);
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
