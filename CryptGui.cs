﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LetMeCrypterU
{
    public partial class CryptGui : Form
    {
        private crypt cp { get; set; }

        public CryptGui()
        {
            InitializeComponent();

            try
            {
                cp = new crypt(lblSti.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                txtOutput.Text = cp.Decrypt(txtInput.Text);
            }
            catch(UnknownFormatException uke)
            {
                MessageBox.Show(uke.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            txtOutput.Text = cp.Encrypt(txtInput.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
           var result=  folderBrowserDialog1.ShowDialog();

            if (result.ToString().Equals("OK"))
            {
                lblSti.Text = folderBrowserDialog1.SelectedPath.ToString();
                try
                {
                    cp = new crypt(lblSti.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            crypt cp = new crypt(@"c:\key\");

            OpenFileDialog of = new OpenFileDialog();

            DialogResult dr = of.ShowDialog();
            if (dr == DialogResult.OK)
            {

       
              //  cp.cryptFile(of.FileName);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            crypt cp = new crypt(@"c:\key\");

            OpenFileDialog of = new OpenFileDialog();
            DialogResult dr = of.ShowDialog();
            if (dr == DialogResult.OK)
            {

                
            //    cp.decryptFile(of.FileName);
            }

        }
    }
}
