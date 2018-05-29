using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emedia_Program2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string nazwa;
            nazwa = textBox1.Text;
            RSA kodowanie = new RSA();
            //kodowanie.start();
            //kodowanie.startdecode(nazwa);
            kodowanie.start();
            pictureBox1.Image = new Bitmap(@"..\..\obrazy\zakodowane.jpg");
            pictureBox2.Image = new Bitmap(@"..\..\obrazy\odkodowane.jpg");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RSA kodowanie = new RSA(); 
            kodowanie.startundecode();
            pictureBox2.Image = new Bitmap(@"..\..\obrazy\odkodowane.jpg");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
