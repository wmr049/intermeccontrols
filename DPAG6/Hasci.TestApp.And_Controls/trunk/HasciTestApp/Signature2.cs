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
    public partial class Signature2 : BaseForm
    {
        [Import(typeof(ISignatureControl))]
        private ISignatureControl conSignature;

        BaseForm prevForm;

        public Signature2(BaseForm sig1)
        {
            InitializeComponent();
            prevForm = sig1;
            menuItemBack.Click += new EventHandler(menuItemBack_Click);
            menuItemSave.Click += new EventHandler(menuItemSave_Click);
        }

        void menuItemSave_Click(object sender, EventArgs e)
        {
            if (conSignature.SaveAsJpg(Savepath.GetPath("FotoSignature")))
            {
                GotoForm(prevForm);
            }
            else
                labelSaveMessage.Text = "Fehler beim Speichern";
        }

        protected override void menuItemBack_Click(object sender, EventArgs e)
        {
            prevForm.Close();
            GotoForm(startForm);
            this.Close();
        }

        private void Signature2_Load(object sender, EventArgs e)
        {
            Control ctrSign = conSignature as Control;
            if (ctrSign != null)
            {
                this.Controls.Add(ctrSign);
                ctrSign.Left = 0;
                ctrSign.Top = 120;
                ctrSign.BackColor = this.BackColor;
            }
            conSignature.SignatureReady += new EventHandler(Signature2_Update);
            menuItemSave.Text = "Ok";

        }

        private void Signature2_Update(object sender, EventArgs e)
        {
            base.menuItemSave.Enabled = true;
        }
    }
}

