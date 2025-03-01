using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RapportiRLC
{
    public partial class Form1 : Form
    {
        Globali globali = new Globali();
        public class Globali
        {
            static public string[] OrariCombobox { get; set; } = new string[24 * 4 + 1];

            public void globali()
            {
                int idx = 0;
                string str = string.Empty;
                for (int i = 0; i < 24; i++)
                {
                    for (int ii = 0; ii <= 45; ii = ii + 15)
                    {
                        str = i.ToString("D2") + ":" + ii.ToString("D2");
                        Globali.OrariCombobox[idx] = str;
                        idx = idx + 1;
                    }

                }

            }
        }
        public Form1()
        {
            InitializeComponent();
            globali.globali();
            foreach (string s in Globali.OrariCombobox)
            {
                if (s != null)
                {
                    comboBox3.Items.Add(s);
                    comboBox4.Items.Add(s);
                    comboBox5.Items.Add(s);
                    comboBox6.Items.Add(s);
                    comboBox7.Items.Add(s);
                    comboBox8.Items.Add(s);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ClienteBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AperturaConn_Click(object sender, EventArgs e)
        {

        }
    }
}
