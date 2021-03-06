﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Autobus
{
    public partial class Form1 : Form
    {
        private VirtualGPSSensor gps = new VirtualGPSSensor();
        private string backup;
        private int nPersone=0;
        private int idMezzo = Convert.ToInt32(new AppSettingsReader().GetValue("idMezzo", typeof(string)));
        private bool aperta = false;
        DataSender redis = new DataSender();
        public Form1()
        {
            InitializeComponent();
            timer2.Start();
        }
        public void StartGPS(object sender,EventArgs e)
        {
            if(btnStop.Enabled==false && btnChiudi.Enabled==false)
            {
                timer1.Start();
                btnPartenza.Enabled = false;
                btnStop.Enabled = true;
                btnApri.Enabled = false;
                btnChiudi.Enabled = false;
            }
        }

        public void StopGPS(object sender,EventArgs e)
        {
            if (btnPartenza.Enabled == false)
            {
                timer1.Stop();
                btnPartenza.Enabled = true;
                btnStop.Enabled = false;
                btnApri.Enabled = true;
            }
        }

        public void ApriPorta(object sender ,EventArgs e)
        {
            Random random = new Random();
            nPersone = random.Next(0, 70);
            txtPersone.Text = nPersone.ToString();
            btnApri.Enabled = false;
            btnChiudi.Enabled = true;
            btnPartenza.Enabled = false;
            aperta = true;
            string pippo = aperta.ToString().ToLower();
            redis.Read(backup + ",\"Aperto\":"+pippo+",\"Persone\":"+nPersone+"}");
        }

        public void ChiudiPorta(object sender,EventArgs e)
        {
            btnPartenza.Enabled = true;
            btnChiudi.Enabled = false;
            btnApri.Enabled = true;
            aperta = false;
            string pippo = aperta.ToString().ToLower();
            redis.Read(backup + ",\"Aperto\":" + pippo + ",\"Persone\":" + nPersone + "}");
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            string temp = (gps.ToJson(idMezzo));
            backup = temp;
            string pippo = aperta.ToString().ToLower();
            temp =(temp + ",\"Aperto\":" + pippo + ",\"Persone\":" + nPersone + "}");
            redis.Read(temp);
            txtGPS.Text += temp+"\r\n";    
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
             redis.Send();
        }
    }
}
