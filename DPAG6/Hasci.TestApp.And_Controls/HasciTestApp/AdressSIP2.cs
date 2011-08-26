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
    public partial class AdressSIP2 : BaseForm
    {
        [Import(typeof(IUtilityControl))]
        private IUtilityControl conUt;

        BaseForm prevForm;
        public AdressSIP2(BaseForm aSip1, BaseForm startForm)
        {
            InitializeComponent();
            vKeyboard1.KeyBeep = conUt.GoodSound;
            prevForm = aSip1; 
            this.menuItemSave.Enabled = true;
            this.menuItemSave.Text = "Ok";
            this.menuItemSave.Click += new EventHandler(menuItemSave_Click);

        }

        void inputPanel1_EnabledChanged(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }

        private void menuItemSave_Click(object sender, EventArgs e)
        {
            ((AdressSIP1)prevForm).DeleteAll();
            this.textBox1.Text = string.Empty;
            GotoForm(prevForm);
        }

        protected override void menuItemBack_Click(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
            prevForm.Close();
            if (startForm != null)
                GotoForm(startForm);
            this.Close();
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            vKeyboard1.CurrentTextBox = textBox1;
        }
    }
}