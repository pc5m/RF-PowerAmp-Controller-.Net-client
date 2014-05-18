using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Configuration;
using System.Diagnostics;


namespace PAcontroller
{
    public partial class FrmSelectSerialPort : Form
    {

        public string COMx { get; set; }
        
        
        public FrmSelectSerialPort()
        {
            InitializeComponent();
        }

        private void FrmSelectSerialPort_Load(object sender, EventArgs e)
        {
            cboxSerialPort.DataSource = SerialPort.GetPortNames();
            if (cboxSerialPort.Items.Count != 0)  //some ports found, try to select stored one, otherwise show first one in selection
            {
                int COMportFound = cboxSerialPort.FindStringExact(Properties.Settings.Default.COMx);
                if (COMportFound != -1) cboxSerialPort.SelectedIndex = COMportFound; else cboxSerialPort.SelectedIndex = 0;
            }
            else
            {   //disable combobox and set COMx property to NULL
                cboxSerialPort.Enabled = false;  
                COMx = null;
            }
        }

        private void cboxSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            COMx = cboxSerialPort.SelectedValue.ToString();
        }


    }
}
