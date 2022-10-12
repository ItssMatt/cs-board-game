using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace HIT
{
    public partial class Form1 : Form
    {
        public int checkServer = 0;
        public static Form1 instance;
        public SimpleTcpClient client;
        string publicIP;
        public string player_name;

        public List<Game.Player> players = new List<Game.Player>();
        public Game.Player local_player;
        public Game.Player last_chancellor;
        public Game.Player last_president;
        public Game.Player vote_chancellor;
        public Game.Actions current_action;
        public long game_started = 0;

        public Form1()
        {
            InitializeComponent();

            instance = this;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            publicIP = new WebClient().DownloadString("https://api.ipify.org"); // getting the client's public ip

            client = new SimpleTcpClient();
            client.DelimiterDataReceived += Client_DelimiterDataReceived; // this method is used by the client to receive data from server

            Thread main_thrd = new Thread(new ThreadStart(MainThread)); // main background thread
            main_thrd.IsBackground = true;
            main_thrd.Start();
        }

        private void MainThread()
        {
            while(true)
            {
                Thread.Sleep(2000);
                if (!connected) continue;
                checkServer += 2000;
                if(checkServer == 10000)
                {
                    MessageBox.Show("You have been disconnected for the following reason:\n(!) The server closed the connection.", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    disconnect();
                }
            }
        }

        public bool connected = false;

        public void disconnect()
        {
            client.Disconnect(); 
            connected = false;
            connButton.Enabled = true;
            readyButton.Enabled = true;
            logBox.Text = String.Empty;
        }

        private void Client_DelimiterDataReceived(object sender, SimpleTCP.Message e)
        {
            checkServer = 0;
            if (e.MessageString.StartsWith("[!]")) // message from server to print lobby message
            {
                string[] split = e.MessageString.Split(new[] { "[!]" }, StringSplitOptions.None);

                logBox.Invoke((MethodInvoker)delegate
                {
                    logBox.AppendText(split[1] + "\n");
                });
                if (Form2.instance != null)
                {
                    Form2.instance.Invoke((MethodInvoker)delegate {
                        Form2.instance.appendfn(split[1] + "\n");
                    });
                }
            }
            else if (e.MessageString.StartsWith("[!!]")) // message from server that the game is no longer starting, canceled
            {
                string[] split = e.MessageString.Split(new[] { "[!!]" }, StringSplitOptions.None);
                readyButton.Invoke((MethodInvoker)delegate
                {
                    readyButton.Enabled = true;
                });
                logBox.Invoke((MethodInvoker)delegate
                {
                    logBox.AppendText(split[1] + "\n");
                });
            }
            else if(e.MessageString.StartsWith("[!!!]")) // message from server with the player list
            {
                string[] split = e.MessageString.Split(new[] { "[!!!]" }, StringSplitOptions.None);
                ushort total_players = ushort.Parse(split[1]);
                game_started = long.Parse(split[2]);
                current_action = (Game.Actions)ushort.Parse(split[5]);
                string[] splitplayers = split[7].Split(new[] { "[&]" }, StringSplitOptions.None);
                players.Clear();
                for (ushort i = 0; i < total_players; i++)
                {
                    Game.Player p = new Game.Player
                    {
                        pName = splitplayers[i * 9 + 0],
                        pScore = 0,
                        isLiberal = bool.Parse(splitplayers[i * 9 + 1]),
                        isFascist = bool.Parse(splitplayers[i * 9 + 2]),
                        isHitler = bool.Parse(splitplayers[i * 9 + 3]),
                        isChancellor = bool.Parse(splitplayers[i * 9 + 4]),
                        isPresident = bool.Parse(splitplayers[i * 9 + 5]),
                        isDead = bool.Parse(splitplayers[i * 9 + 6]),
                        votedYes = bool.Parse(splitplayers[i * 9 + 7]),
                        votedNo = bool.Parse(splitplayers[i * 9 + 8])
                    };
                    if (p.pName.Equals(player_name))
                        local_player = p;
                    if (p.pName.Equals(split[3]))
                        last_chancellor = p;
                    if (p.pName.Equals(split[4]))
                        last_president = p;
                    switch (current_action)
                    {
                        case Game.Actions.CHOOSING_CHANCELLOR:
                            {
                                break;
                            }
                        case Game.Actions.VOTE_CHANCELLOR:
                            {
                                if (p.pName.Equals(split[6]))
                                    vote_chancellor = p;
                                break;
                            }
                    }

                    players.Add(p);
                }
                foreach (Form frmm in Application.OpenForms)
                {
                    if (frmm.Name.Equals("Form3"))
                    {
                        frmm.Dispose();
                        break;
                    }
                }
                switch (current_action)
                {
                    case Game.Actions.CHOOSING_CHANCELLOR:
                        {
                            if (local_player.pName.Equals(split[6]))
                            {
                                foreach (Form frmm in Application.OpenForms)
                                {
                                    if (frmm.Name.Equals("Form3"))
                                    {
                                        frmm.Dispose();
                                        break;
                                    }
                                }
                                Thread tt = new Thread(threadedForm2);
                                tt.SetApartmentState(ApartmentState.STA);
                                tt.Start();
                            }
                            break;
                        }
                }
                foreach (Form frmm in Application.OpenForms)
                {
                    if (frmm.Name.Equals("Form2"))
                    {
                        Form2.instance.Invoke((MethodInvoker)delegate {
                            Form2.instance.updatePlayerBoard();
                        });
                        return;
                    }
                }
                Thread t = new Thread(threadedForm);
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            else if(e.MessageString.StartsWith("[!!!!]")) // message from server with the player's score
            {
                string[] split = e.MessageString.Split(new[] { "[!!!!]" }, StringSplitOptions.None);
                ushort total_players = ushort.Parse(split[1]);
                string[] splitplayers = split[2].Split(new[] { "[&]" }, StringSplitOptions.None);
                for (ushort i = 0; i < total_players; i++)
                {
                    if (!Form1.instance.local_player.pName.Equals(splitplayers[i * 2 + 0]))
                        continue;
                    Form1.instance.local_player.pScore = int.Parse(splitplayers[i * 2 + 1]);
                }
            }
            else if (e.MessageString.StartsWith("[X]")) // message from server that an error was thrown
            {
                string[] split = e.MessageString.Split(new[] { "[X]" }, StringSplitOptions.None);
                string ipport = publicIP + ":" + client.TcpClient.Client.LocalEndPoint.ToString().Split(':')[1];
                if (e.MessageString.Length == 3 || ipport.Equals(split[1]))
                {
                    MessageBox.Show("You have been disconnected for the following reason:\n(!) " + (e.MessageString.Length == 3 ? "The server closed the connection." : split[2]), "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    disconnect();
                }
            }
        }
        private void threadedForm(object arg)
        {
            Application.Run(new Form2());
        }

        private void threadedForm2(object arg)
        {
            Application.Run(new Form3());
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            disconnect();
        }

        private void connButton_Click(object sender, EventArgs e)
        {
            if(!IPAddress.TryParse(ipBox.Text, out _))
            {
                MessageBox.Show("The IP you entered is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int port;
            if (!int.TryParse(portBox.Text, out port) || port > 65535 || port <= 0)
            {
                MessageBox.Show("The port you entered is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(nameBox.Text.Length < 3 || nameBox.Text.Length > 8)
            {
                MessageBox.Show("Your name must be at least 3 characters and no more than 8 characters!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                client.Connect(ipBox.Text, port);
                client.WriteLine("[*]" + nameBox.Text + "[*]" + passBox.Text + "[*]");
                connButton.Enabled = false;
                connected = true;
                player_name = nameBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find the server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void readyButton_Click(object sender, EventArgs e)
        {
            if(!connected)
            {
                MessageBox.Show("You are not connected to a server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            client.WriteLine("[**]");
            readyButton.Enabled = false;
        }

        private void sendbtn_Click(object sender, EventArgs e)
        {
            if (!connected || chatBox.Text.Length == 0 || string.IsNullOrWhiteSpace(chatBox.Text))
            {
                return;
            }
            client.WriteLine("[***]" + chatBox.Text);
            chatBox.Text = String.Empty;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
