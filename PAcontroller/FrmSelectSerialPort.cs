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
            cboxSerialPort.SelectedIndex = cboxSerialPort.FindStringExact(Properties.Settings.Default.COMx);
        }

        private void cboxSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            COMx = cboxSerialPort.SelectedValue.ToString();
        }


    }
}
