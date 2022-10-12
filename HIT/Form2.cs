using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIT
{
    public partial class Form2 : Form
    {
        public static Form2 instance = null;

        public void appendfn(string text)
        {
            chatBox.AppendText(text);
        }

        public string getActionPlayerName(Game.Actions action)
        {
            switch (action)
            {
                case Game.Actions.NONE: return "NONE";
                case Game.Actions.CHOOSING_CHANCELLOR:
                    {
                        foreach(Game.Player p in Form1.instance.players)
                        {
                            if (p.isPresident)
                                return p.pName;
                        }
                        break;
                    }
                case Game.Actions.VOTE_CHANCELLOR: return "Everyone";
                case Game.Actions.PRESIDENT_DISCARDS_POLICY:
                    {
                        foreach (Game.Player p in Form1.instance.players)
                        {
                            if (p.isPresident)
                                return p.pName;
                        }
                        break;
                    }
                case Game.Actions.CHANCELLOR_PICKS_POLICY:
                    {
                        foreach (Game.Player p in Form1.instance.players)
                        {
                            if (p.isChancellor)
                                return p.pName;
                        }
                        break;
                    }
                case Game.Actions.PRESIDENT_INVESTIGATES_A_PLAYER:
                    {
                        foreach (Game.Player p in Form1.instance.players)
                        {
                            if (p.isPresident)
                                return p.pName;
                        }
                        break;
                    }
                case Game.Actions.PRESIDENT_PICKS_NEXT_PRESIDENT:
                    {
                        foreach (Game.Player p in Form1.instance.players)
                        {
                            if (p.isPresident)
                                return p.pName;
                        }
                        break;
                    }
                case Game.Actions.PRESIDENT_SHOOTS_PLAYER:
                    {
                        foreach (Game.Player p in Form1.instance.players)
                        {
                            if (p.isPresident)
                                return p.pName;
                        }
                        break;
                    }
            }
            return String.Empty;
        }

        public void updatePlayerBoard()
        {
            playerBox.Text = String.Empty;
            ushort fascists_counter = 0;
            foreach (Game.Player p in Form1.instance.players)
            {
                if (p.pName.Equals(Form1.instance.player_name))
                {
                    Form1.instance.local_player = p;
                }
                if (p.isFascist) fascists_counter++;
            }
            foreach (Game.Player p in Form1.instance.players)
            {
                playerBox.SelectionColor = (p.isDead ? Color.Red : Color.Black);
                if (Form1.instance.local_player == p)
                {
                    playerBox.SelectionFont = new Font(playerBox.SelectionFont, playerBox.SelectionFont.Style | FontStyle.Bold);
                }
                else
                {
                    playerBox.SelectionFont = new Font(playerBox.SelectionFont, playerBox.SelectionFont.Style & ~FontStyle.Bold);
                }
                playerBox.AppendText(p.pName);
                if (p.isFascist)
                {
                    if (Form1.instance.local_player.isFascist)
                    {
                        if (p.isHitler)
                        {
                            playerBox.SelectionColor = Color.DarkRed;
                            playerBox.AppendText(" (Hitler)");
                        }
                        else
                        {
                            if (!Form1.instance.local_player.isHitler || (Form1.instance.local_player.isHitler && fascists_counter == 2))
                            {
                                playerBox.SelectionColor = Color.Red;
                                playerBox.AppendText(" (Fascist)");
                            }
                        }
                    }
                }
                if (p.isPresident)
                {
                    playerBox.SelectionColor = Color.DarkBlue;
                    playerBox.AppendText(" (President)");
                }
                if (p.isChancellor)
                {
                    playerBox.SelectionColor = Color.DarkKhaki;
                    playerBox.AppendText(" (Chancellor)");
                }
                if(Form1.instance.current_action == Game.Actions.VOTE_CHANCELLOR)
                {
                    if(p.votedNo)
                    {
                        playerBox.SelectionColor = Color.DarkSlateBlue;
                        playerBox.AppendText(" (NEIN)");
                    }
                    else if(p.votedYes)
                    {
                        playerBox.SelectionColor = Color.DarkSlateBlue;
                        playerBox.AppendText(" (JA)");
                    }
                }
                playerBox.AppendText("\n");
            }
            switch (Form1.instance.current_action)
            {
                case Game.Actions.CHOOSING_CHANCELLOR:
                    {
                        announcelabel.Text = getActionPlayerName(Game.Actions.CHOOSING_CHANCELLOR) + " is the president and is choosing a chancellor...";
                        break;
                    }
                case Game.Actions.VOTE_CHANCELLOR:
                    {
                        announcelabel.Text = "Everyone is now voting for " + Form1.instance.vote_chancellor.pName + " as the new chancellor...";
                        break;
                    }
            }
            scorelabel.Text = "Your score: " + Form1.instance.local_player.pScore;
            actionlabel.Text = "Player's action: " + new Game().getActionName(Form1.instance.current_action);
            turnlabel.Text = "Player's turn: " + getActionPlayerName(Form1.instance.current_action);
        }

        public Form2()
        {
            InitializeComponent();
            instance = this;

            Thread main_thrd = new Thread(new ThreadStart(MainThread)); // main background thread
            main_thrd.IsBackground = true;
            main_thrd.Start();

            if (Form1.instance.local_player.isHitler)
            {
                rolepicturebox.Image = HIT.Properties.Resources.role_hitler;
                roletip.SetToolTip(rolepicturebox,
                    "Hitler: You have to be elected as chancellor after 3 fascist policies or you must achieve 6 fascist policies in order to win.\n" +
                    "Also you have to find and work with the other fascists, as you don't know who they are. They know who you are though!");
            }
            else if (Form1.instance.local_player.isFascist)
            {
                int rand = new Random().Next(0, 2);
                if (rand == 0) rolepicturebox.Image = HIT.Properties.Resources.role_fascist_1;
                else if (rand == 1) rolepicturebox.Image = HIT.Properties.Resources.role_fascist_2;
                else if (rand == 2) rolepicturebox.Image = HIT.Properties.Resources.role_fascist_3;
                roletip.SetToolTip(rolepicturebox,
                    "Fascist: You have to elect Hitler as chancellor after 3 fascist policies or you must achieve 6 fascist policies in order to win.\n" +
                    "Also you know who Hitler is, but he doesn't know you are on his side. You have to protect him at all costs!");
            }
            else if (Form1.instance.local_player.isLiberal)
            {
                int rand = new Random().Next(0, 5);
                if (rand == 0) rolepicturebox.Image = HIT.Properties.Resources.role_liberal_1;
                else if (rand == 1) rolepicturebox.Image = HIT.Properties.Resources.role_liberal_2;
                else if (rand == 2) rolepicturebox.Image = HIT.Properties.Resources.role_liberal_3;
                else if (rand == 3) rolepicturebox.Image = HIT.Properties.Resources.role_liberal_4;
                else if (rand == 4) rolepicturebox.Image = HIT.Properties.Resources.role_liberal_5;
                else if (rand == 5) rolepicturebox.Image = HIT.Properties.Resources.role_liberal_6;
                roletip.SetToolTip(rolepicturebox,
                    "Liberal: You have to work with the other liberals to find Hitler and assassinate him, or you must elect 5 liberal policies in order to win.\n" +
                    "Also you have to be careful to not elect Hitler after 3 elected fascist policies because you will lose the game!");
            }

            rolelabel.ForeColor = (Form1.instance.local_player.isLiberal ? Color.DarkTurquoise : (Form1.instance.local_player.isHitler ? Color.DarkRed : Color.Red));
            rolelabel.Text = (Form1.instance.local_player.isLiberal ? "Liberal" : (Form1.instance.local_player.isHitler ? "Hitler" : "Fascist"));
            partylabel.ForeColor = (Form1.instance.local_player.isLiberal ? Color.DarkTurquoise : Color.Red);
            partylabel.Text = (Form1.instance.local_player.isLiberal ? "Liberal" : "Fascist");

            updatePlayerBoard();
        }

        private void MainThread()
        {
            while (true)
            {
                if (!Form1.instance.connected) break;
                Thread.Sleep(1000);

                long seconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - Form1.instance.game_started;
                TimeSpan time = TimeSpan.FromSeconds(seconds);
                string str = time.ToString(@"mm\:ss");
                this.Invoke((MethodInvoker)delegate
                {
                    timelabel.Text = str;
                    scorelabel.Text = "Your score: " + Form1.instance.local_player.pScore;
                });

                bool found = false;
                foreach (Form frmm in Application.OpenForms)
                {
                    if (frmm.Name.Equals("Form1"))
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    foreach(Form f in Application.OpenForms)
                    {
                        if(f.Name.Equals("Form3"))
                        {
                            Form3.instance.Invoke((MethodInvoker)delegate
                            {
                                Form3.instance.Dispose();
                            });
                            break;
                        }
                    }
                    this.Invoke((MethodInvoker)delegate {
                        this.Dispose();
                    });
                }
            }
        }

        private void chatBtn_Click(object sender, EventArgs e)
        {
            if (!Form1.instance.connected || sendchatbox.Text.Length == 0 || string.IsNullOrWhiteSpace(sendchatbox.Text))
            {
                return;
            }
            Form1.instance.client.WriteLine("[***]" + sendchatbox.Text);
            sendchatbox.Text = String.Empty;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Form1.instance.connected) e.Cancel = true;
        }
        private void japicturebox_MouseEnter(object sender, EventArgs e)
        {
            japicturebox.Location = new Point(610, 497);
        }

        private void japicturebox_MouseLeave(object sender, EventArgs e)
        {
            japicturebox.Location = new Point(610, 547);
        }

        private void neinpicturebox_MouseEnter(object sender, EventArgs e)
        {
            neinpicturebox.Location = new Point(824, 497);
        }

        private void neinpicturebox_MouseLeave(object sender, EventArgs e)
        {
            neinpicturebox.Location = new Point(824, 547);
        }
    }
}
