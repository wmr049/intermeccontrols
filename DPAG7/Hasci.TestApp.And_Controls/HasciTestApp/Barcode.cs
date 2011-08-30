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
    public partial class Barcode : BaseForm
    {
        [Import(typeof(IBarcodeScanControl))]
        private IBarcodeScanControl conScan;

        [Import(typeof(IUtilityControl))]
        private IUtilityControl conUt;

        public Barcode()
        {
            InitializeComponent();
            //Control ctrScan = conScan as Control;
            if (bFirstLoad)
                initBarcode();
            Control ctrUtil = conUt as Control;
            //if (ctrScan != null)
            //{
            //    this.Controls.Add(ctrScan);
            //    ctrScan.Visible = false;
            //}
            if (ctrUtil != null)
            {
                this.Controls.Add(ctrUtil);
                ctrUtil.Visible = false;
            }

            //conScan.ScanReady += ScanControl_Ready;
            conScan.ScanReady += new BarcodeEventHandler(conScan_ScanReady);    //using new eventargs driven interface
            base.menuItemSave.Text = string.Empty;

        }

       private static bool bFirstLoad = true;
        private void initBarcode()
        {
            Control ctrScan = conScan as Control;
            if (ctrScan != null)
            {
                this.Controls.Add(ctrScan);
                ctrScan.Visible = false;
            }
            bFirstLoad = false;
        }
        private void deInitBarcode()
        {
            if (conScan != null)
            {
                conScan.Dispose();
                conScan = null;
                bFirstLoad = true;
            }
        }

        void conScan_ScanReady(object sender, BarcodeEventArgs e)
        {
            textBoxScanResult.Text = e.Text;
            if (e._bSuccess)
            {
                if (checkBoxVibro.Checked)
                    conUt.Vibration(200);
                conUt.GoodSound(200);
                pictureBoxOK.Visible = true;
                pictureBoxNotOk.Visible = false;
            }
            else
            {
                conUt.BadSound(200);
                pictureBoxOK.Visible = false;
                pictureBoxNotOk.Visible = true;
            }
        }
        
        private void ScanControl_Ready(object sender, EventArgs e)
        {
            textBoxScanResult.Text = conScan.BarcodeText;
            if (conScan.IsSuccess)
            {
                if (checkBoxVibro.Checked)
                    conUt.Vibration(200);
                conUt.GoodSound(200);
                pictureBoxOK.Visible = true;
                pictureBoxNotOk.Visible = false;
            }
            else
            {
                conUt.BadSound(200);
                pictureBoxOK.Visible = false;
                pictureBoxNotOk.Visible = true;
            }
        }
        private void Barcode_Deactivate(object sender, EventArgs e)
        {
            if (!bFirstLoad)
                deInitBarcode();

        }

        private void Barcode_Activated(object sender, EventArgs e)
        {
            if (bFirstLoad)
                initBarcode();

        }

    }
}

