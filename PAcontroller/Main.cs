using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Configuration;


namespace PAcontroller
{
    public partial class Main : Form
    {
        // Observerable collection support eventhandling when adding new message to list<T>
        ObservableCollection<Message> messageList = new ObservableCollection<Message>();
        
        Communication communication = new Communication();

        FrmCalibrationAndDetail frmCalibrationAndDetails;

        // delegate needed for updating GUI from serial port thread
        private delegate void SetGuiDeleg(Message data);




        public enum MsgIDsFromMCU
        {
            Status = 0,
            CurrentsADCVals = 1,
            CurrentVals = 2,
            CurrentTripADC = 3,
            CurrentTripVal = 4,
            CurrentCalibrationADC2AMP = 5,
            CurrentCalibrationAMP2ADC = 6,
            PowerADCVals = 7,
            PowerTripADC = 8,
            PowerTripVals = 9,
            PowerCalibrationADC2W = 10,
            PowerCalibrationW2ADC = 11,
            PowerVals = 12,
            Temperature = 13,
            TemperatureTrip = 14,
            FW_VERSION = 15,
            PowerCalibrationADC2W_RC_B = 16,
            PowerCalibrationW2ADC_RC_B = 17
        }

        public enum MsgIDsToMCU
        {
            REQ_FW_VERSION = 0, 
            SET_STATUS_AUTOTX_STATUS = 1,
            SET_STATUS_AUTOTX_CURRENTS_ADC= 2,
            SET_STATUS_AUTOTX_CURRENTS_VALS = 3,
            REQ_CURRENT_TRIP_ADC= 4,
            REQ_CURRENT_TRIP_VAL= 5,
            SET_CURRENT_TRIP_ADC= 6,
            SET_CURRENT_TRIP_VALS= 7,
            SET_CAL_CURRENTS_AMP2ADC= 8,
            SET_CAL_CURRENTS_ADC2AMP= 9,
            REQ_CAL_CURRENTS_AMP2ADC= 10,
            REQ_CAL_CURRENTS_ADC2AMP= 11,
            SET_STATUS_AUTOTX_POWERS_ADC= 12,
            SET_STATUS_AUTOTX_POWERS_VALS= 13,
            REQ_POWERS_TRIP_ADC= 14,
            REQ_POWERS_TRIP_VALS= 15,
            SET_POWERS_TRIP_ADC= 16,
            SET_POWERS_TRIP_VAL= 17,
            SET_SWR_TRIP_VAL = 18,
            SET_CAL_POWERS_W2ADC= 19,
            SET_CAL_POWERS_ADC2W= 20,
            REQ_CAL_POWERS_W2ADC= 21,
            REQ_CAL_POWERS_ADC2W= 22,
            SET_STATUS_AUTOTX_TEMPERATURE = 23,
            REQ_TEMP_TRIP= 24,
            SET_TEMP_TRIP = 25,
            SET_CAL_POWERS_W2ADC_RC_B = 26,
            SET_CAL_POWERS_ADC2W_RC_B = 27,
            REQ_CAL_POWERS_W2ADC_RC_B = 28,
            REQ_CAL_POWERS_ADC2W_RC_B = 29
        }




        //   ErrorClearedPreviously  is needed to clear textforms only once when no error exists
        static private bool ErrorClearedPreviously = false;  
        public enum ErrorStates
        {
            NoError = 0,
            ImodA = 1,
            ImodB = 2,
            ImodC = 3,
            ImodD = 4,
            Pfwrd = 5,
            Prefl = 6,
            Pin = 7,
            SWR = 8,
            TempA = 9,
            TempB = 10,
            TempC = 11,
            TempD = 12
        }

        const byte FLAG_ERROR   = 7;
        const byte FLAG_TX_ON   = 6;
        const byte FLAG_PSU_ON  = 5;
        const byte FLAG_BIAS_ON = 4;

        public const byte MODULE_A = 0;
        public const byte MODULE_B = 1;
        public const byte MODULE_C = 2;
        public const byte MODULE_D = 3;

        public const byte POWER_FWRD = 0;
        public const byte POWER_REFL = 1;
        public const byte POWER_IN = 2;
        public const byte SWR = 3;

        MsgIDsFromMCU msgIDfromMCU;
        MsgIDsToMCU msgIDToMCU;
        ErrorStates SSPAError;

        bool SSPAConnected;

        string COMx;

        public Main()
        {
            InitializeComponent();
            communication.messageList = messageList;
        }

        

        private void Main_Load(object sender, EventArgs e)
        {
            //serialPortToolStripComboBox1.ComboBox.DataSource = SerialPort.GetPortNames();
            //serialPortToolStripComboBox1.SelectedIndex = serialPortToolStripComboBox1.FindStringExact(COMx);
            // communication.SerialCommunicationInit();
            lblPSUOnOff.BackColor = Color.Snow;
            lblRxTx.BackColor = Color.Snow;
            lblBiasOnOff.BackColor = Color.Snow;
            lblError.Visible = false;
            SSPAConnected = false;
            messageList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Messagelist_changed);
        }

        private void Messagelist_changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
               // Debug.WriteLine(((Message)e.NewItems[0]).id.ToString());
                lock (messageList)
                {
                    // not sure if we need invoke or begininvoke ??, RemoveAt(0) ? also do we need to check for more
                    // than one item at position 0 ?Maybe more are added before item 0 is being processed and removed
                    this.BeginInvoke(new SetGuiDeleg(processMsg), new object[] { messageList[0] }); 
                    messageList.RemoveAt(0);
                    int count = messageList.Count();
                    if (count > 0) Debug.WriteLine("nr of items: {0}",messageList.Count());
                }
            }
        }

        private bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        private void processStatusCode (byte status)
        {
            const byte ErrorMask = 0x0F;
            if (IsBitSet(status, FLAG_BIAS_ON) == true)
            {
                lblBiasOnOff.BackColor = Color.Red;
                lblBiasOnOff.Text = "BIAS ON";
            }
            else
            {
                lblBiasOnOff.BackColor = Color.Green;
                lblBiasOnOff.Text = "BIAS OFF";
            }
            if (IsBitSet(status, FLAG_PSU_ON) == true)
            {
                lblPSUOnOff.BackColor = Color.Red;
                lblPSUOnOff.Text = "PSU ON";
            }
            else
            {
                lblPSUOnOff.BackColor = Color.Green;
                lblPSUOnOff.Text = "PSU OFF";

            }
            if (IsBitSet(status, FLAG_TX_ON) == true)
            {
                lblRxTx.BackColor = Color.Red;
                lblRxTx.Text = "TX";
            }
            else
            {
                lblRxTx.BackColor = Color.Green;
                lblRxTx.Text = "RX";
            }
            if (IsBitSet(status, FLAG_ERROR) == true)
            {
                ErrorClearedPreviously = false; // make sure text boxes are cleared when no error
                lblError.Visible = true;
                // Notice from this point on, status is being changed, masked for only LSB 4 bits
                status &= ErrorMask;
                switch ((ErrorStates)status)
                {
                    case ErrorStates.NoError:
                        break;
                    case ErrorStates.ImodA: txtI_A.BackColor = Color.Red;
                        break;
                    case ErrorStates.ImodB: txtI_B.BackColor = Color.Red;
                        break;
                    case ErrorStates.ImodC: txtI_C.BackColor = Color.Red;
                        break;
                    case ErrorStates.ImodD: txtI_D.BackColor = Color.Red;
                        break;
                    case ErrorStates.Pfwrd: txtPfwrd.BackColor = Color.Red;
                        break;
                    case ErrorStates.Prefl: txtPrefl.BackColor = Color.Red;
                        break;
                    case ErrorStates.Pin: txtPin.BackColor = Color.Red;
                        break;
                    case ErrorStates.SWR: txtSWR.BackColor = Color.Red;
                        break;
                    case ErrorStates.TempA: txtTemp_A.BackColor = Color.Red;
                        break;
                    case ErrorStates.TempB: txtTemp_B.BackColor = Color.Red;
                        break;
                    case ErrorStates.TempC: txtTemp_C.BackColor = Color.Red;
                        break;
                    case ErrorStates.TempD: txtTemp_D.BackColor = Color.Red;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (ErrorClearedPreviously == false)
                {
                    ErrorClearedPreviously = true;  //text boxes only cleared once when no error
                    lblError.Visible = false;
                    txtI_A.BackColor = System.Drawing.SystemColors.Window;
                    txtI_B.BackColor = System.Drawing.SystemColors.Window;
                    txtI_C.BackColor = System.Drawing.SystemColors.Window;
                    txtI_D.BackColor = System.Drawing.SystemColors.Window;
                    txtPfwrd.BackColor = System.Drawing.SystemColors.Window;
                    txtPrefl.BackColor = System.Drawing.SystemColors.Window;
                    txtPin.BackColor = System.Drawing.SystemColors.Window;
                    txtSWR.BackColor = System.Drawing.SystemColors.Window;
                    txtTemp_A.BackColor = System.Drawing.SystemColors.Window;
                    txtTemp_B.BackColor = System.Drawing.SystemColors.Window;
                    txtTemp_C.BackColor = System.Drawing.SystemColors.Window;
                    txtTemp_D.BackColor = System.Drawing.SystemColors.Window;
                }
            }
       }


        private void processMsg(Message message)
        {
            switch ((MsgIDsFromMCU)message.id)
            {
                case MsgIDsFromMCU.Status:
                    processStatusCode(message.messageData[0]);
                    break;
                case MsgIDsFromMCU.FW_VERSION:
                    lblConnectStatus.Text = "Connected, FW version:" + (message.messageData[0]).ToString();
                    btnConnect.Text = "Disconnect";
                     communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_CURRENTS_VALS, 1);
                     communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_POWERS_VALS, 1);
                     communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_TEMPERATURE, 1);
                    communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_STATUS, 1);
                    SSPAConnected = true;
                    lblPSUOnOff.Enabled = true;
                    lblRxTx.Enabled = true;
                    lblError.Enabled = true;
                    break;
                case MsgIDsFromMCU.Temperature:
                    txtTemp_A.Text = Convert.ToString(message.messageData[0]);
                    txtTemp_B.Text = Convert.ToString(message.messageData[1]);
                    txtTemp_C.Text = Convert.ToString(message.messageData[2]);
                    txtTemp_D.Text = Convert.ToString(message.messageData[3]);
                    break;
                case MsgIDsFromMCU.CurrentVals:
                    txtI_A.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(0, 4).ToArray(), 0));
                
                    txtI_B.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(4, 4).ToArray(), 0));
                    txtI_C.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(8, 4).ToArray(), 0));
                    txtI_D.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(12, 4).ToArray(), 0));
                    break;
                case MsgIDsFromMCU.PowerVals:
                    txtPfwrd.Text = String.Format("{0:F0}", BitConverter.ToSingle(message.messageData.GetRange(0, 4).ToArray(), 0));
                    txtPrefl.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(4, 4).ToArray(), 0));
                    txtPin.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(8, 4).ToArray(), 0));
                    txtSWR.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(12, 4).ToArray(), 0));
                    break;
                default:
                    break;
            }
            // if Calibration and details form is active, also process the message for that form
            if (frmCalibrationAndDetails != null) frmCalibrationAndDetails.processMsg(message);     
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutForm = new AboutBox1();
            aboutForm.ShowDialog();
        }

        private void calibrationDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCalibrationAndDetails = new FrmCalibrationAndDetail();
            frmCalibrationAndDetails.comm = communication;
            frmCalibrationAndDetails.ShowDialog();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!communication.PortIsOpen)
            {
                 communication.SerialCommunicationInit(Properties.Settings.Default.COMx);
                communication.ThresholdReached += new EventHandler<MyEventArgs> (communication_ThresholdReached);
            }
            if (communication.PortIsOpen)
            {
                if (SSPAConnected == false)
                {
                    communication.SerialCommunicationClose();
                    communication.SerialCommunicationInit(Properties.Settings.Default.COMx);
                    communication.SendMessage(MsgIDsToMCU.REQ_FW_VERSION);
                }
                else
                {
                    communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_CURRENTS_VALS, 0);
                    communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_POWERS_VALS, 0);
                    communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_TEMPERATURE, 0);
                    communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_STATUS, 0);
                    btnConnect.Text = "Connect";
                    lblConnectStatus.Text = "Disconnected, press CONNECT to connect to SSPA";
                    lblPSUOnOff.BackColor = Color.Snow;
                    lblRxTx.BackColor = Color.Snow;
                    lblBiasOnOff.BackColor = Color.Snow;
                    SSPAConnected = false;
                    System.Threading.Thread.Sleep(100);  // make sure serial communication had time to process transmit
                    communication.SerialCommunicationClose();
                }
            }

        }

        void communication_ThresholdReached(object sender, MyEventArgs e)
        {
            // MessageBox.Show (e.BufferLength.ToString());
            //   textBox1.Text = e.BufferLength.ToString();

            this.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = e.BufferLength.ToString(); // runs on UI thread
            });
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            messageList.CollectionChanged -= Messagelist_changed;
            if (communication.PortIsOpen)
            {
                communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_CURRENTS_VALS, 0);
                communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_POWERS_VALS, 0);
                communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_TEMPERATURE, 0);
                communication.SendMessage(MsgIDsToMCU.SET_STATUS_AUTOTX_STATUS, 0);
                System.Threading.Thread.Sleep(100);  // make sure serial communication had time to process transmit
                communication.SerialCommunicationClose();
            }
        }

        private void serialPortTsMenuItem_Click(object sender, EventArgs e)
        {
            FrmSelectSerialPort SelectSerialPort = new FrmSelectSerialPort();
            if (DialogResult.OK == SelectSerialPort.ShowDialog())
            {
                if (SelectSerialPort.COMx != null && SelectSerialPort.COMx != String.Empty)
                {
                    Properties.Settings.Default.COMx = SelectSerialPort.COMx;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}