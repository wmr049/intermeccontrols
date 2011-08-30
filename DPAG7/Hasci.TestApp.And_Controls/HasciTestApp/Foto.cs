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
    public partial class Foto : BaseForm
    {
        [Import(typeof(IPhotoControl))]
        private IPhotoControl conPhoto;

        public Foto()
        {
            InitializeComponent();
        }
        private static bool bFirstLoad = true;
        void initCamera()
        {
            Control ctrPhoto = conPhoto as Control;
            if (ctrPhoto != null)
            {
                panelImage.Controls.Add(ctrPhoto);
                ctrPhoto.Left = 0;
                ctrPhoto.Top = 0;
                ctrPhoto.Visible = true;
                ctrPhoto.BackColor = this.BackColor;
                panelImage.Visible = true;
            }
            conPhoto.InPreview += new EventHandler(Photo_Preview);
            conPhoto.ImageReady += new EventHandler(Photo_Image);
            menuItemSave.Click += new EventHandler(menuItemSave_Click);
            labelPhotoHint.Text = "Fokussieren Sie mit gedrückter " + conPhoto.PhotoKey.ToUpper() +
                "-Taste. Beim Loslassen der " + conPhoto.PhotoKey.ToUpper() + "-Taste wird das Foto gemacht";
            bFirstLoad = false;

        }
        void deInitCamera()
        {
            try
            {
                if (conPhoto != null)
                {
                    conPhoto.Dispose();
                    conPhoto = null;
                    bFirstLoad = true;
                }

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void Photo_Preview(object sender, EventArgs e)
        {
            labelPhotoHint.ForeColor = Color.Red;
            menuItemSave.Enabled = false;
        }

        private void Photo_Image(object sender, EventArgs e)
        {
            labelPhotoHint.ForeColor = Color.Black;
            menuItemSave.Enabled = true;
        }

        private void Photo_Load(object sender, EventArgs e)
        {
            if (bFirstLoad)
                initCamera();

            //Control ctrPhoto = conPhoto as Control;
            //if (ctrPhoto != null)
            //{
            //    panelImage.Controls.Add(ctrPhoto);
            //    ctrPhoto.Left = 0;
            //    ctrPhoto.Top = 0;
            //    ctrPhoto.Visible = true;
            //    ctrPhoto.BackColor = this.BackColor;
            //    panelImage.Visible = true;
            //}
            //conPhoto.InPreview += new EventHandler(Photo_Preview);
            //conPhoto.ImageReady += new EventHandler(Photo_Image);
            //menuItemSave.Click += new EventHandler(menuItemSave_Click);
            //labelPhotoHint.Text = "Fokussieren Sie mit gedrückter " + conPhoto.PhotoKey.ToUpper() + 
            //    "-Taste. Beim Loslassen der " + conPhoto.PhotoKey.ToUpper() + "-Taste wird das Foto gemacht";
        }

        void menuItemSave_Click(object sender, EventArgs e)
        {
            if (conPhoto.SaveAsJpg(Savepath.GetPath("FotoKamera")))
            {
                labelSaveMessage.Text = "Erfolgreich gespeichert";
                menuItemSave.Enabled = false;
            }
            else
                labelSaveMessage.Text = "Fehler beim Speichern";
        }

        private void Foto_Deactivate(object sender, EventArgs e)
        {
            if (!bFirstLoad)
                deInitCamera();
        }

        private void Foto_Activated(object sender, EventArgs e)
        {
            if (bFirstLoad)
                initCamera();
        }

    }

}

