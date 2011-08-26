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
    public partial class Imager :BaseForm
    {
        [Import(typeof(IImagerControl))]
        private IImagerControl conImager;

        public Imager()
        {
            InitializeComponent();
        }

        private void Imager_Preview(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Red;
            menuItemSave.Enabled = false;
        }

        private void Imager_Image(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Black;
            menuItemSave.Enabled = true;
        }

        private void Imager_Load(object sender, EventArgs e)
        {
            if (bFirstLoad)
                initImager();

            //Control ctrImager = conImager as Control;
            //if (ctrImager != null)
            //{
            //    panelImage.Controls.Add(ctrImager);
            //    ctrImager.Left = 1;
            //    ctrImager.Top = 0;
            //    ctrImager.BackColor = this.BackColor;
            //}
            //conImager.InPreview += new EventHandler(Imager_Preview);
            //conImager.ImageReady += new EventHandler(Imager_Image);
            //menuItemSave.Click += new EventHandler(menuItemSave_Click);
        }
        private static bool bFirstLoad = true;
        private void initImager()
        {
            Control ctrImager = conImager as Control;
            if (ctrImager != null)
            {
                panelImage.Controls.Add(ctrImager);
                ctrImager.Left = 1;
                ctrImager.Top = 0;
                ctrImager.BackColor = this.BackColor;
            }
            conImager.InPreview += new EventHandler(Imager_Preview);
            conImager.ImageReady += new EventHandler(Imager_Image);
            menuItemSave.Click += new EventHandler(menuItemSave_Click);
            //this.Resize += new EventHandler(Imager_Resize);
            bFirstLoad = false;
        }
        private void deInitImager()
        {
            if (conImager != null)
            {
                conImager.Dispose();
                conImager = null;
                bFirstLoad = true;
            }
        }

        void menuItemSave_Click(object sender, EventArgs e)
        {
            if (conImager.SaveAsJpg(Savepath.GetPath("FotoImager")))
            {
                labelSaveMessage.Text = "Erfolgreich gespeichert";
                menuItemSave.Enabled = false;
            }
            else
                labelSaveMessage.Text = "Fehler beim Speichern";
        }

        private void Imager_Activated(object sender, EventArgs e)
        {
            if (bFirstLoad)
                initImager();
        }

        private void Imager_Deactivate(object sender, EventArgs e)
        {
            if (!bFirstLoad)
                deInitImager();
        }

    }
}

