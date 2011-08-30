#pragma warning disable 0114,0219
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace Hasci.TestApp
{
    public partial class Signature : BaseForm
    {
        public Signature()
        {
            InitializeComponent();
        }

        private void Signature_Load(object sender, EventArgs e)
        {
            menuItemSave.Text = "OK";
            menuItemSave.Enabled = true;
            menuItemBack.Enabled = false;
            menuItemBack.Text = string.Empty;
            menuItemSave.Click += new EventHandler(menuItemSave_Click);
            menuItemBack.Click +=new EventHandler(menuItemBack_Click);
        }

        void menuItemSave_Click(object sender, EventArgs e)
        {
            if (nextForm == null)
                nextForm = new Signature2(this);
            GotoForm(nextForm);
       }

        private void menuItemBack_Click(object sender, EventArgs e)
        {
            if (nextForm != null)
                nextForm.Close();
            GotoForm(startForm);
        }

    }
}

