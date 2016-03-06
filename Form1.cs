using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Beehive_Simulator
{
    public partial class Form1 : Form
    {
        private World world;
        private Random random = new Random();
        private DateTime start = DateTime.Now;
        private DateTime end;
        private int framesRun;
        public Form1()
        {
            InitializeComponent();
            world = new World(new BeeMessage(SendMessage));

            timer1.Interval = 50;
            timer1.Tick += new EventHandler(RunFrame);
            timer1.Enabled = false;
            UpdateStats(new TimeSpan());
            
        }

        private void UpdateStats(TimeSpan frameDuration)
        {
            Bees.Text = world.Bees.Count.ToString();
            Flowers.Text = world.Flowers.Count.ToString();
            HoneyInHive.Text = string.Format("{0:f3}", world.Hive.Honey);
            double nectar = 0;
            foreach (Flower flower in world.Flowers)
                nectar += flower.Nectar;
            NectarInFlowers.Text = string.Format("{0:f3}", nectar);
            FramesRun.Text = framesRun.ToString();
            double milliSeconds = frameDuration.TotalMilliseconds;
            if (milliSeconds != 0.0)
                FrameRate.Text = string.Format("{0:f0} ({1:f1}ms)", 1000 / milliSeconds, milliSeconds);
            else
                FrameRate.Text = "N/A";
        }

        public void RunFrame(object sender, EventArgs e)
        {
            framesRun++;
            world.Go(random);
            end = DateTime.Now;
            TimeSpan frameDuration = end - start;
            start = end;
            UpdateStats(frameDuration);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                toolStrip1.Items[0].Text = "Resume simulation";
                timer1.Stop();
            }
            else
            {
                toolStrip1.Items[0].Text = "Pause simulation";
                timer1.Start();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            framesRun = 0;
            world = new World(new BeeMessage(SendMessage));
            if (!timer1.Enabled)
                toolStrip1.Items[0].Text = "Start simulation";

        }

        private void SendMessage(int ID, string Message)
        {
            statusStrip1.Items[0].Text = "Bee #" + ID + ": " + Message;
            var beeGroups =
                from bee in world.Bees
                group bee by bee.CurrentState into beeGroup
                orderby beeGroup.Key
                select beeGroup;
            listBox1.Items.Clear();
            foreach(var group in beeGroups) {
                string s;
                if (group.Count() == 1)
                    s = "";
                else
                    s = "s";
                listBox1.Items.Add(group.Key.ToString() + ": " + group.Count() + " bee" + s);
                if (group.Key == BeeState.Idle && group.Count() == world.Bees.Count() && framesRun > 0)
                {
                    listBox1.Items.Add("Simulation has ended: all bees are idle");
                    toolStrip1.Items[0].Text = "Simulation ended";
                    statusStrip1.Items[0].Text = "Simulation ended";
                    timer1.Enabled = false;

                }
            }  
        }
    }
}
