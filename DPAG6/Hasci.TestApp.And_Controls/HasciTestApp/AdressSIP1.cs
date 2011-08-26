using System;
using System.Linq;
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
    public partial class AdressSIP1 : BaseForm
    {
        [Import(typeof(IUtilityControl))]
        private IUtilityControl conUt;

        public AdressSIP1()
        {
            InitializeComponent();
            vKeyboard1.KeyBeep = conUt.GoodSound;
            this.menuItemSave.Enabled = true;
            this.menuItemSave.Text = "Weiter";
            this.menuItemSave.Click += new EventHandler(menuItemSave_Click);
        }

        internal void DeleteAll()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox1.Focus();
        }

        void inputPanel1_EnabledChanged(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }

        protected override void menuItemBack_Click(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
            if (nextForm != null)
                nextForm.Close();
            if (startForm != null)
                GotoForm(startForm);
            this.Close();
        }

        private void menuItemSave_Click(object sender, EventArgs e)
        {
            if (nextForm == null)
                nextForm = new AdressSIP2(this, startForm);
            GotoForm(nextForm);
        }

        private void textBox_GotFocus(object sender, EventArgs e)
        {
            vKeyboard1.CurrentTextBox = sender as TextBox;
        }
    }
}