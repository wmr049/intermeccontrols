using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Hasci.TestApp.DeviceControlContracts;

namespace Hasci.TestApp
{
    public partial class Form1 : BaseForm
    {


        [Import(typeof(IUtilityControl))]
        private IUtilityControl conUt;

        private int exitKey = 0;

        public Form1()
        {
            InitializeComponent();
            startForm = this;
            Control ctrUtil = conUt as Control;
            if (ctrUtil != null)
            {
                this.Controls.Add(ctrUtil);
                ctrUtil.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conUt.SipState = false;
            labelPerCent.Text = "Akku: " + conUt.AccuState.ToString("D") + " %";
            this.Activated += new EventHandler(Form1_Activated);
        }

        void Form1_Deactivate(object sender, EventArgs e)
        {
        }

        void Form1_Activated(object sender, EventArgs e)
        {
            exitKey = 0;
            labelPerCent.Text = "Akku: " + conUt.AccuState.ToString("D") + " %";
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Got KeyDown, KeyValue=" + e.KeyValue);
            switch (e.KeyValue) 
            {
                case 153:   //ignore orange key of Intermec Keyboards
                    e.Handled = true;
                    break;
                case 66:
                case 155:
                    if (exitKey == 0)
                        exitKey = 1;
                    else
                        exitKey = 0;
                    break;
                case 57:
                case 86:
                    if (exitKey > 0)
                    {
                        exitKey++;
                        if (exitKey == 3)
                            this.Close();
                    }
                    else
                        exitKey = 0;
                    break;
                case 144:
                    break;
                default:
                    exitKey = 0;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            GotoMenuItem(new Barcode());
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GotoMenuItem(new Foto());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GotoMenuItem(new Imager());

        }

        private void button6_Click(object sender, EventArgs e)
        {
            GotoMenuItem(new Signature());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GotoMenuItem(new Adress());

        }

        private void button4_Click(object sender, EventArgs e)
        {
            GotoMenuItem(new AdressSIP1());
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            this.KeyDown -= new KeyEventHandler(Form1_KeyDown);
            this.Deactivate -= new EventHandler(Form1_Deactivate);
            conUt.SipState = true;
            Application.Exit(); //HGO
        }

        private void GotoMenuItem(BaseForm targetForm)
        {
            exitKey = 0;
            this.KeyDown -= new KeyEventHandler(Form1_KeyDown);
            GotoForm(targetForm);
        }

    }
}