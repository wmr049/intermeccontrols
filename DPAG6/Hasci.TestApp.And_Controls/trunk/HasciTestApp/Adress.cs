using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Hasci.TestApp.DeviceControlContracts;

namespace Hasci.TestApp
{
    public partial class Adress : BaseForm
    {
        [Import(typeof(IUtilityControl))]
        private IUtilityControl conUt;

        public Adress()
        {
            InitializeComponent();
            Control ctrUtil = conUt as Control;
            if (ctrUtil != null)
            {
                this.Controls.Add(ctrUtil);
                ctrUtil.Visible = false;
            }
            this.Activated += new EventHandler(Adress_Activated);
            this.Deactivate += new EventHandler(Adress_Deactivate);
            this.menuItemSave.Enabled = true;
            this.menuItemSave.Text = "Ok";
            this.menuItemSave.Click += new EventHandler(menuItemSave_Click);
        }

        void menuItemSave_Click(object sender, EventArgs e)
        {
            foreach (Control ctr in this.Controls)
                if (ctr is TextBox)
                    ((TextBox)ctr).Text = string.Empty;
            textBox2.Focus();
        }

        void Adress_Deactivate(object sender, EventArgs e)
        {
            conUt.KeyboardBacklightState = false;
        }

        void Adress_Activated(object sender, EventArgs e)
        {
            conUt.KeyboardBacklightState = true;
        }

        private void inputPanel1_EnabledChanged(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }
    }
}

