using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouOnlyLabelOnce
{
    public partial class YouOnlyLabelOnce : Form
    {
        byte[] bytes = new byte[1048576];
        public static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12345);
        Socket sendr = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        public static int coords = 0;
        public int x1;
        public int y1;
        public int x2;
        public int y2;

        public Color clr;
        Dictionary<string, int> objs = new Dictionary<string, int>() { { "Red", 4 }, { "Blue", 5 }};

        public YouOnlyLabelOnce()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            write("\nconnecting to server");

            // request image from server
            sendr.Connect(remoteEP);
            receive_img();
        }

        private void write(string text)
        {
            textBox1.AppendText(text);
            textBox1.AppendText(Environment.NewLine);
        }

        private void receive_img()
        {
            byte[] msg = Encoding.ASCII.GetBytes("r");
            int bytesSent = sendr.Send(msg);

            // receive image and decode bytes
            int bytesRec = sendr.Receive(bytes);
            using (var ms = new MemoryStream(bytes))
            {
                pictureBox1.Image = Image.FromStream(ms);
            }
            connect.Enabled = false;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // grab current mouse coords on click
            if (coords != 1)
            {
                submit.Enabled = false;
                x1 = e.X;
                y1 = e.Y;
                coords = 1;
            }
            else if (coords == 1)
            {
                x2 = e.X;
                y2 = e.Y;
                coords++;
                
                // if we have two coords, draw a rectangle as bounding box
                Pen pen = new Pen(clr);
                using (Graphics G = Graphics.FromImage(pictureBox1.Image))
                {
                    G.DrawRectangle(pen, new Rectangle(x1, y1, x2 - x1, y2 - y1));
                }
                pictureBox1.Refresh();
                if (listBox1.SelectedItem != null)
                {
                    submit.Enabled = true;
                }
            }
            write("coordinate " + coords + " grabbed");
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            clr = Color.FromName(listBox1.SelectedItem.ToString());
            if (coords == 2)
            {
                submit.Enabled = true;
            }
        }
        
        private void submit_MouseUp(object sender, MouseEventArgs e)
        {
            float width = pictureBox1.Image.Size.Width;
            float height = pictureBox1.Image.Size.Height;

            String line = objs[listBox1.SelectedItem.ToString()] + " " + Math.Abs(x1 + x2) / 2 / width + " " + Math.Abs(y1 + y2) / 2 / height + " " + Math.Abs(x1 - x2) / width + " " + Math.Abs(y1 - y2) / height;
            byte[] msg = Encoding.ASCII.GetBytes(line);
            int bytesSent = sendr.Send(msg);
            write(line + " submitted to server");

            receive_img();
        }
    }
}
