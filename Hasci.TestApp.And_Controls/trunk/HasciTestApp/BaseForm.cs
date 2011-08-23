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


namespace Hasci.TestApp
{
    public partial class BaseForm : Form
    {
        protected static BaseForm startForm;
        protected bool Is653 = false;
        protected BaseForm nextForm;
        public BaseForm()
        {
            InitializeComponent();
            DirectoryCatalog catalog = new DirectoryCatalog(".","*.*.*Control*.dll");
            CompositionContainer container = new CompositionContainer(catalog);
            try
            {
                container.ComposeParts(this);
            }
            catch (Exception ex)
            {
                if (ex is ChangeRejectedException)
                    MessageBox.Show("HW-Components not found!");
                else
                    MessageBox.Show("Exception in ComposeParts: " + ex.Message + "\n" + ex.StackTrace);
                this.Close();
            }
        }

        protected virtual void menuItemBack_Click(object sender, EventArgs e)
        {
            if (startForm != null)
                GotoForm(startForm);
            this.Close();
        }

        protected void BaseForm_Deactivate(object sender, EventArgs e)
        {
            this.Activate();
        }

        protected void GotoForm(BaseForm targetForm)
        {
            //HGO
            try
            {
                this.Deactivate -= new EventHandler(BaseForm_Deactivate);
                targetForm.Activate();
                targetForm.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kann Dialog " + targetForm.Text + " nicht öffnen. \n" + ex.Message);
            }
        }

        private void BaseForm_Activated(object sender, EventArgs e)
        {
            this.Deactivate -= new EventHandler(BaseForm_Deactivate);
            this.Deactivate += new EventHandler(BaseForm_Deactivate);
        }

    }
}