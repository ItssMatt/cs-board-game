using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIT
{
    public partial class Form3 : Form
    {
        public static Form3 instance;

        public Form3()
        {
            instance = this;
            InitializeComponent();
            this.Text = new Game().getActionName(Form1.instance.current_action);
            switch (Form1.instance.current_action)
            {
                case Common.Game.Actions.CHOOSING_CHANCELLOR:
                    {
                        foreach (Game.Player p in Form1.instance.players)
                        {
                            if (Form1.instance.local_player == p || Form1.instance.last_chancellor == p || Form1.instance.last_president == p || p.isDead) continue;
                            Button b = new Button();
                            b.Click += new EventHandler(dynamicbutton_Click);
                            b.Text = p.pName;
                            b.Dock = DockStyle.Top;
                            this.Controls.Add(b);
                        }
                        break;
                    }
            }
        }

        private void dynamicbutton_Click(object sender, EventArgs e)
        {
            if (Form2.instance.getActionPlayerName(Form1.instance.current_action).Equals(Form1.instance.player_name))
            {
                Button btn = (Button)sender;
                Form1.instance.client.WriteLine("[****]" + btn.Text);
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Form1.instance.connected) e.Cancel = true;
        }
    }
}
