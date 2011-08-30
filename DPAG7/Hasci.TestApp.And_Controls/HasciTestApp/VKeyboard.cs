using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Hasci.TestApp
{
    public partial class VKeyboard : UserControl
    {
        private TextBox currTxtBox;
        private Panel AlphaKeys;
        private Panel DigitKeys;
        private bool isNumeric = false;
        public delegate void Beep(int duration);
        private Beep keyBeep;


        public VKeyboard()
        {
            InitializeComponent();
            CreateAlpha();
            CreateNumeric();
        }

        private void CreateAlpha()
        {
            const int wNumA = 6;
            const int wNumN = 3;
            const int hNum = 5;
            int pWidth = this.Width;
            int pHeight = this.Height;
            int bWidth = pWidth / hNum;
            int bHeight = pHeight / wNumA;
            int wLoc = bWidth / 12;
            int hLoc = bHeight * 2 / 15;

            this.SuspendLayout();
            AlphaKeys = new Panel();
            this.Controls.Add(AlphaKeys);
            AlphaKeys.Location = new System.Drawing.Point(0, 0);
            AlphaKeys.Size = new System.Drawing.Size(pWidth, pHeight);
            AlphaKeys.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(153)))));
            AlphaKeys.Visible = false;

            for(int bIndex = 0; bIndex < wNumA * hNum; bIndex++)
            {

                Button btnKey = new System.Windows.Forms.Button();
                btnKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(180)))), ((int)(((byte)(0)))));
                btnKey.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
                btnKey.Location = new System.Drawing.Point(wLoc + (bIndex * bWidth) % pWidth, hLoc + bHeight * (int)(bIndex / hNum));
                if (bIndex < 26)
                {
                    btnKey.Text = ((char)(bIndex + 97)).ToString();
                    btnKey.Name = "button" + btnKey.Text;
                }
                else
                    switch (bIndex)
                    {
                        case 26:
                            btnKey.Text = ".";
                            btnKey.Name = "buttonPointa";
                            break;
                        case 27:
                            btnKey.Text = " ";
                            btnKey.Name = "buttonSpace" ;
                            break;
                        case 28:
                            btnKey.Text = "<-";
                            btnKey.Name = "buttonBsp";
                            break;
                        case 29:
                            btnKey.Text = "123";
                            btnKey.Name = "button" + btnKey.Text;
                            break;
                        default:
                        btnKey.Name = "button";
                        break;
                    }
                btnKey.Size = new System.Drawing.Size(bWidth - 2 * wLoc, bHeight - 2 * hLoc);
                btnKey.TabIndex = bIndex;
                btnKey.TabStop = false;
                btnKey.Click += new System.EventHandler(this.buttonAny_Click);
                AlphaKeys.Controls.Add(btnKey);
            }

            this.ResumeLayout(false);
            AlphaKeys.Visible = true;

        }

        private void CreateNumeric()
        {
            const int wNumA = 6;
            const int wNumN = 3;
            const int hNum = 5;
            int pWidth = this.Width;
            int pHeight = this.Height;
            int bWidth = pWidth / hNum;
            int bHeight = pHeight / wNumA;
            int wLoc = bWidth / 12;
            int hLoc = bHeight * 2 / 15;

            this.SuspendLayout();
            DigitKeys = new Panel();
            this.Controls.Add(DigitKeys);
            DigitKeys.Location = new System.Drawing.Point(0, 0);
            DigitKeys.Size = new System.Drawing.Size(pWidth, pHeight);
            DigitKeys.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(153)))));
            DigitKeys.Visible = false;
            DigitKeys.Enabled = false;

            for (int bIndex = 15; bIndex < wNumA * hNum; bIndex++)
            {

                Button btnKey = new System.Windows.Forms.Button();
                btnKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(180)))), ((int)(((byte)(0)))));
                btnKey.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
                btnKey.Location = new System.Drawing.Point(wLoc + (bIndex * bWidth) % pWidth, hLoc + bHeight * (int)(bIndex / hNum));
                if (bIndex < 25)
                {
                    btnKey.Text = ((char)(bIndex + 33)).ToString();
                    btnKey.Name = "button" + btnKey.Text;
                }
                else
                    switch (bIndex)
                    {
                        case 25:
                            btnKey.Text = ".";
                            btnKey.Name = "buttonPoint";
                            break;
                        case 26:
                            btnKey.Text = ",";
                            btnKey.Name = "buttonComma";
                            break;
                        case 27:
                            btnKey.Text = " ";
                            btnKey.Name = "buttonSpace";
                            break;
                        case 28:
                            btnKey.Text = "<-";
                            btnKey.Name = "buttonBsp";
                            break;
                        case 29:
                            btnKey.Text = "abc";
                            btnKey.Name = "button" + btnKey.Text;
                            break;
                        default:
                            btnKey.Name = "button";
                            break;
                    }
                btnKey.Size = new System.Drawing.Size(bWidth - 2 * wLoc, bHeight - 2 * hLoc);
                btnKey.TabIndex = bIndex;
                btnKey.TabStop = false;
                btnKey.Click += new System.EventHandler(this.buttonAny_Click);
                DigitKeys.Controls.Add(btnKey);
            }

            this.ResumeLayout(false);

        }

        public bool Numeric
        {
            get
            {
                return isNumeric;
            }
            set
            {
                isNumeric = value;
                if (isNumeric)
                {
                    AlphaKeys.Visible = false;
                    DigitKeys.Visible = true;
                    AlphaKeys.Enabled = false;
                    DigitKeys.Enabled = true;
                }
                else
                {
                    DigitKeys.Visible = false;
                    AlphaKeys.Visible = true;
                    DigitKeys.Enabled = false;
                    AlphaKeys.Enabled = true;
                }
            }
        }

        public TextBox CurrentTextBox
        {
            get
            {
                return currTxtBox;
            }
            set
            {
                currTxtBox = value;
            }
        }

        public Beep KeyBeep
        {
            set
            {
                keyBeep = value;
            }
        }

        private void buttonAny_Click(object sender, EventArgs e)
        {
            if (currTxtBox == null)
                this.Visible = false;
            else
            {
                int currTbCursorPos = currTxtBox.SelectionStart;
                Button currBtn = (Button)sender;
                currTxtBox.Focus();
                if (currBtn.Text.Length == 1)
                {
                    currTxtBox.Text = currTxtBox.Text.Insert(currTbCursorPos, currBtn.Text);
                    currTxtBox.SelectionStart = currTbCursorPos + 1;
                }
                else
                switch (currBtn.Text)
                {
                    case "<-":
                    if (currTbCursorPos > 0)
                    {
                        currTxtBox.Text = currTxtBox.Text.Remove(currTbCursorPos - 1, 1);
                        currTxtBox.SelectionStart = currTbCursorPos - 1;
                    }
                    break;
                    case "abc":
                        Numeric = false;
                        currTxtBox.SelectionStart = currTbCursorPos;
                        break;
                    case "123":
                        Numeric = true;
                        currTxtBox.SelectionStart = currTbCursorPos;
                        break;
                    default:
                    break;
                }
                currBtn.Update();
                if (keyBeep != null)
                    keyBeep(50);
            }

        }
    }
}
