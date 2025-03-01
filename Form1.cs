using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RapportiRLC
{
    public partial class Form1 : Form
    {
        //Questa area è terra di nessuno, creiamo qui istanze di oggetti accessibili pubblicamente
        Globals globals = new Globals();
        SqlConnection conn = new SqlConnection(); //La connessione è una no? perchè usarne 20
        SqlCommand cmd = new SqlCommand();

        public class Cliente //Classe cliente, serve a contenere i dati dei clienti a Database
        {
            public int UID;
            public string NomeCliente;
            public string Via;
            public string Città;
            public string Civico;
            public string CAP;
            public string PIVA; 
        }

        public class Globals //Classe che contiene variabili e metodi accessibili globalmente da tutto.
        {
            static public string[] OrariCombobox { get; set; } = new string[24 * 4 + 1];
            static public string Server { get; set; }
            static public string Database { get; set; }
            static public string User { get; set; }
            static public string Pwd { get; set; }

            public Cliente[] Clienti { get; set; } = new Cliente[0]; //Inizializzo a 0 l'array, MEMORY COSTS $$$

            public void InitOrariComboBox() //Popola array per gli span orari delle attività/viaggi (formato 00:00 ... 23:45)
            {
                int idx = 0;
                string str = string.Empty;
                for (int i = 0; i < 24; i++)
                {
                    for (int ii = 0; ii <= 45; ii = ii + 15)
                    {
                        str = i.ToString("D2") + ":" + ii.ToString("D2");
                        Globals.OrariCombobox[idx] = str;
                        idx = idx + 1;
                    }

                }

            }

           public void CreaClienti(int Quantità) //Alla Creazione della connessione col database tramite il pulsante, questa funzione viene chiamata, viene creato l'array della dimensione giusta
            {
                int i = 0;
                Clienti = new Cliente[Quantità];
                for (i = 0;i < Quantità; i++)
                {
                    Clienti[i] = new Cliente();
                }
                
            }
        
            public void MappaClientiInMemoria(DataTable dt)
            {
                string tmp;
                int idx = 1;
                CreaClienti(dt.Rows.Count + 1);
                Cliente[] Cl = Clienti;
                foreach (DataRow dr in dt.Rows)
                {
                    if (Cl[idx] != null)
                    {
                        tmp = dr[0].ToString();
                        Cl[idx].UID = Int32.Parse(tmp);
                        Cl[idx].NomeCliente = dr[1].ToString();
                        Cl[idx].Via = dr[2].ToString();
                        Cl[idx].Città = dr[3].ToString();
                        Cl[idx].Civico = dr[4].ToString();
                        Cl[idx].CAP = dr[5].ToString();
                        Cl[idx].PIVA = dr[6].ToString();
                        idx++;
                    }
                }
                Clienti = Cl;
            }   
            public void ClientiToCBox(Cliente[] CL,ComboBox CB) //popola l'oggetto Combobox passato al metodo con tutti i nomi 
            {
                CB.Items.Clear();
                int idx = 0;
                CB.Items.Add("---");
                foreach (Cliente c in CL)
                {
                    if (c != null && c.NomeCliente != null)
                    {
                        CB.Items.Add(c.NomeCliente);
                    }
                    idx++;
                }
                CB.SelectedIndex = 0;
            }
        }
        public Form1() // Questo è come se Fosse il Main() di tutto, Hai presente lo script OnStartup di intoutch? ecco, butta i tuoi startup qui, attento però, gli item a video esistono solo dopo "initialisecomponent()"
        {
            InitializeComponent();
            globals.InitOrariComboBox();
            //Inizializzo Dati Database Di Default (Editabili anche a video).
            ServerTextBox.Text = @"192.168.1.239\SQLEXPRESS";
            UserTextBox.Text = "sa";
            PwdTextBox.Text = "io";
            NameTextBox.Text = "Rapporti_RLC";

            foreach (string s in Globals.OrariCombobox)
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
            if (ClienteBox.SelectedItem != null && ClienteBox.SelectedItem.ToString() != "---")
            {
                bool gotcha = false;
                Cliente ClienteTrovato = new Cliente();
                foreach (Cliente cl in globals.Clienti)
                {
                    if (cl != null && cl.NomeCliente == ClienteBox.SelectedItem.ToString())
                    {
                        gotcha = true;
                        ClienteTrovato = cl;
                        break;
                    }
                }
                if (gotcha)
                {
                    CittàText.Text = ClienteTrovato.Città;
                    ViaText.Text = ClienteTrovato.Via;
                    CivicoText.Text = ClienteTrovato.Civico;
                    CapText.Text = ClienteTrovato.CAP;
                }
            } 
        }

        private void AperturaConn_Click(object sender, EventArgs e)
        {
            Globals.Server = ServerTextBox.Text;
            Globals.User = UserTextBox.Text;
            Globals.Pwd = PwdTextBox.Text;
            Globals.Database = NameTextBox.Text;

            string sConn = "server =" + Globals.Server + "; user id = " + Globals.User + "; password = " + Globals.Pwd + " ; database = " + Globals.Database + ";";
            string Query;
            if (conn.State != ConnectionState.Open) 
            { 
                conn.ConnectionString = sConn; 
            }

            if ((Globals.Server != null) && (Globals.User != null) && (Globals.Pwd != null) && (Globals.Database != null))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    if (conn != null)
                    {
                        Query = "SELECT * from Clienti";
                        cmd.CommandText = Query;
                        cmd.Connection = conn;
                        using (SqlDataReader reader = cmd.ExecuteReader()) { 
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            globals.MappaClientiInMemoria(dt);
                            globals.ClientiToCBox(globals.Clienti,ClienteBox);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Almeno uno dei Parametri Di connessione è nullo");
            }



        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (conn != null && (conn.State == ConnectionState.Open)) 
            {
                conn.Close();  
            }
        }
    }
}
