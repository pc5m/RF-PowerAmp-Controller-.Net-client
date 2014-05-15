using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace PAcontroller
{
    public partial class FrmCalibrationAndDetail : Form
    {
        public Communication comm;

        public FrmCalibrationAndDetail()
        {
            InitializeComponent();
        }

        private void FormDetails_Load(object sender, EventArgs e)
        {
            // messageList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Messagelist_changed);
            btnIA_SendCal.Enabled = false;
            btnIB_SendCal.Enabled = false;
            btnIC_SendCal.Enabled = false;
            btnID_SendCal.Enabled = false;
            btnPfwrdSendCal.Enabled = false;
            btnPreflSendCal.Enabled = false;
            btnPinpSendCal.Enabled = false;
            btnSWRSendCal.Enabled = false;
        }

        public void processMsg(Message message)
        {
            switch ((Main.MsgIDsFromMCU)message.id)
            {
                case Main.MsgIDsFromMCU.Temperature:
                    txtAdegreeC.Text = Convert.ToString(message.messageData[0]);
                    txtBdegreeC.Text = Convert.ToString(message.messageData[1]);
                    txtCdegreeC.Text = Convert.ToString(message.messageData[2]);
                    txtDdegreeC.Text = Convert.ToString(message.messageData[3]);
                    break;
                case Main.MsgIDsFromMCU.TemperatureTrip:
                    txtTripTemperature.Text = Convert.ToString(message.messageData[0]);
                    break;
                case Main.MsgIDsFromMCU.CurrentVals:
                    txtIA_ActualAmp.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange( 0,4).ToArray(),0));
                    txtIB_ActualAmp.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange( 4,4).ToArray(),0));
                    txtIC_ActualAmp.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange( 8,4).ToArray(),0));
                    txtID_ActualAmp.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(12,4).ToArray(),0));
                    break;
                case Main.MsgIDsFromMCU.CurrentsADCVals:
                    txtIA_ActualADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(0, 2).ToArray(), 0));
                    txtIB_ActualADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(2, 2).ToArray(), 0));
                    txtIC_ActualADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(4, 2).ToArray(), 0));
                    txtID_ActualADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(6, 2).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.CurrentTripADC:
                    txtIA_TripADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(0, 2).ToArray(), 0));
                    txtIB_TripADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(2, 2).ToArray(), 0));
                    txtIC_TripADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(4, 2).ToArray(), 0));
                    txtID_TripADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(6, 2).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.CurrentTripVal:
                    txtI_TripAmp.Text = String.Format("{0:F1}", BitConverter.ToInt16(message.messageData.GetRange(0, 2).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.CurrentCalibrationADC2AMP:
                    txtIA_ADC2Amp.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(0, 4).ToArray(), 0));
                    txtIB_ADC2Amp.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(4, 4).ToArray(), 0));
                    txtIC_ADC2Amp.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(8, 4).ToArray(), 0));
                    txtID_ADC2Amp.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(12, 4).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.CurrentCalibrationAMP2ADC:
                    txtIA_Amp2ADC.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(0, 4).ToArray(), 0));
                    txtIB_Amp2ADC.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(4, 4).ToArray(), 0));
                    txtIC_Amp2ADC.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(8, 4).ToArray(), 0));
                    txtID_Amp2ADC.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(12, 4).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.PowerVals:
                    txtPfwrdWatt.Text = String.Format("{0:F0}", BitConverter.ToSingle(message.messageData.GetRange( 0,4).ToArray(),0));
                    txtPreflWatt.Text = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange( 4,4).ToArray(),0));
                    txtPinpWatt.Text  = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange( 8,4).ToArray(),0));
                    txtSWR.Text       = String.Format("{0:F1}", BitConverter.ToSingle(message.messageData.GetRange(12,4).ToArray(),0));
                    break;
                case Main.MsgIDsFromMCU.PowerADCVals:
                    txtPfwrdADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(0, 2).ToArray(), 0));
                    txtPreflADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(2, 2).ToArray(), 0));
                    txtPinpADC.Text =  String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(4, 2).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.PowerTripADC:
                    txtPfwrdTripADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(0, 2).ToArray(), 0));
                    txtPreflTripADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(2, 2).ToArray(), 0));
                    txtPinpTripADC.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(4, 2).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.PowerTripVals:
                    txtPfwrdTripWatt.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(0, 2).ToArray(), 0));
                    txtPreflTripWatt.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(2, 2).ToArray(), 0));
                    txtPinpTripWatt.Text = String.Format("{0:F0}", BitConverter.ToInt16(message.messageData.GetRange(4, 2).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.PowerCalibrationADC2W:
                    txtPfwrdADC2W.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(0, 4).ToArray(), 0));
                    txtPreflADC2W.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(4, 4).ToArray(), 0));
                    txtPinpADC2W.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(8, 4).ToArray(), 0));
                    break;
                case Main.MsgIDsFromMCU.PowerCalibrationW2ADC:
                    txtPfwrdW2ADC.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(0, 4).ToArray(), 0));
                    txtPreflW2ADC.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(4, 4).ToArray(), 0));
                    txtPinpW2ADC.Text = String.Format("{0:F4}", BitConverter.ToSingle(message.messageData.GetRange(8, 4).ToArray(), 0));
                    break;
                default:
                    break;
            }
        }

        private void btnRefreshTemperatureTrip_Click(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.REQ_TEMP_TRIP);
        }

        private void btnSendTemperatureCal_Click(object sender, EventArgs e)
        {
            Byte val;
            if (!Byte.TryParse(txtTripTemperatureCal.Text, out val))
            {
                errorProvider1.SetError(txtTripTemperatureCal, "Only integer values between 0 and 255 allowed");
                return;
            }
            else errorProvider1.SetError(txtTripTemperatureCal, "");
            comm.SendMessage(Main.MsgIDsToMCU.SET_TEMP_TRIP,val);
            btnRefreshTemperatureTrip_Click(sender, null); //refresh trip temperature textbox
        }

        private void tabPageTemperature_Enter(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.REQ_TEMP_TRIP);
        }

        private void tabPagePower_Enter(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.REQ_POWERS_TRIP_ADC);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_POWERS_TRIP_VALS);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_W2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_STATUS_AUTOTX_POWERS_ADC, 1);
        }

        private void tabPageCurrent_Enter(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CURRENT_TRIP_VAL);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CURRENT_TRIP_ADC);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_CURRENTS_AMP2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_CURRENTS_ADC2AMP);
            comm.SendMessage(Main.MsgIDsToMCU.SET_STATUS_AUTOTX_CURRENTS_ADC,1);
        }

        private void btnIA_Check_Click(object sender, EventArgs e)
        {
            int ADCcal, TripAmpCal;
            float AmpCal, MaxCurrent;
            if (CheckCurrentTextBoxes(txtIA_AmpCal, txtIA_ADCcal, txtI_TripAmpCal, out AmpCal,  out ADCcal, out TripAmpCal ) == true)
            {
                 MaxCurrent = AmpCal / ADCcal * 1023;
                 txtIA_MaxAmp.Text = MaxCurrent.ToString("F0");
                 btnIA_SendCal.Enabled = true;
            } 
            else btnIA_SendCal.Enabled = false;
        }

        private void btnIB_Check_Click(object sender, EventArgs e)
        {
            int ADCcal, TripAmpCal;
            float AmpCal, MaxCurrent;
            if (CheckCurrentTextBoxes(txtIB_AmpCal, txtIB_ADCcal, txtI_TripAmpCal, out AmpCal, out ADCcal, out TripAmpCal) == true)
            {
                MaxCurrent = AmpCal / ADCcal * 1023;
                txtIB_MaxAmp.Text = MaxCurrent.ToString("F0");
                btnIB_SendCal.Enabled = true;
            }
            else btnIB_SendCal.Enabled = false;
        }

        private void btnIC_Check_Click(object sender, EventArgs e)
        {
            int ADCcal, TripAmpCal;
            float AmpCal, MaxCurrent;
            if (CheckCurrentTextBoxes(txtIC_AmpCal, txtIC_ADCcal, txtI_TripAmpCal, out AmpCal, out ADCcal, out TripAmpCal) == true)
            {
                MaxCurrent = AmpCal / ADCcal * 1023;
                txtIC_MaxAmp.Text = MaxCurrent.ToString("F1");
                btnIC_SendCal.Enabled = true;
            }
            else btnIC_SendCal.Enabled = false;
        }

        private void btnID_Check_Click(object sender, EventArgs e)
        {
            int ADCcal, TripAmpCal;
            float AmpCal, MaxCurrent;
            if (CheckCurrentTextBoxes(txtID_AmpCal, txtID_ADCcal, txtI_TripAmpCal, out AmpCal, out ADCcal, out TripAmpCal) == true)
            {
                MaxCurrent = AmpCal / ADCcal * 1023;
                txtID_MaxAmp.Text = MaxCurrent.ToString("F1");
                btnID_SendCal.Enabled = true;
            }
            else btnID_SendCal.Enabled = false;
        }

        private bool CheckCurrentTextBoxes(TextBox txt_AmpCal, TextBox txt_ADCcal, TextBox txtI_TripAmpCal, out float AmpCal, out int ADCcal, out int TripAmpCal)
        {
            int _ADCcal, _TripAmpCal;
            float _AmpCal;
            bool succesAmpCal, succesADCcal, succesTripAmpCal;
            succesAmpCal = float.TryParse(txt_AmpCal.Text, out _AmpCal);
            if (succesAmpCal & (_AmpCal > 0)) errorProvider1.SetError(txt_AmpCal, ""); else errorProvider1.SetError(txt_AmpCal, "numeric value must be bigger than 0");

            succesADCcal = int.TryParse(txt_ADCcal.Text, out _ADCcal);
            if (succesADCcal & (_ADCcal > 0) & (_ADCcal <= 1023)) errorProvider1.SetError(txt_ADCcal, ""); else errorProvider1.SetError(txt_ADCcal, "integer value must be bigger than 0 and smaller than 1023");

            succesTripAmpCal = int.TryParse(txtI_TripAmpCal.Text, out _TripAmpCal);
            if (succesTripAmpCal & (_TripAmpCal > 0)) errorProvider1.SetError(txtI_TripAmpCal, ""); else errorProvider1.SetError(txtI_TripAmpCal, "value must be bigger than 0");

            AmpCal = _AmpCal;
            ADCcal = _ADCcal;
            TripAmpCal = _TripAmpCal;
            return (succesADCcal & succesAmpCal & succesTripAmpCal);
        }

        private bool CheckPowerTextBoxes(TextBox txt_PCal, TextBox txt_ADCcal, TextBox txt_TripPCal, out UInt16 PCal, out UInt16 ADCcal, out UInt16 TripPCal)
        {
            UInt16 _ADCcal, _TripPCal;
            UInt16 _PCal;
            bool succesPCal, succesADCcal, succesTripPCal;
            succesPCal = UInt16.TryParse(txt_PCal.Text, out _PCal);
            if (succesPCal & (_PCal > 0)) errorProvider1.SetError(txt_PCal, ""); else errorProvider1.SetError(txt_PCal, "numeric value must be bigger than 0");

            succesADCcal = UInt16.TryParse(txt_ADCcal.Text, out _ADCcal);
            if (succesADCcal & (_ADCcal > 0) & (_ADCcal <= 1023)) errorProvider1.SetError(txt_ADCcal, ""); else errorProvider1.SetError(txt_ADCcal, "integer value must be bigger than 0 and smaller than 1023");

            succesTripPCal = UInt16.TryParse(txt_TripPCal.Text, out _TripPCal);
            if (succesTripPCal & (_TripPCal > 0)) errorProvider1.SetError(txt_TripPCal, ""); else errorProvider1.SetError(txt_TripPCal, "value must be bigger than 0");

            PCal = _PCal;
            ADCcal = _ADCcal;
            TripPCal = _TripPCal;
            return (succesADCcal & succesPCal & succesTripPCal);
        }

        private void btnIA_SendCal_Click(object sender, EventArgs e)
        {
            float IA_CalAmp2ADC, IA_CalADC2Amp;
            byte CalTripAmp;
            CalTripAmp = Convert.ToByte(txtI_TripAmpCal.Text);
            IA_CalAmp2ADC = Convert.ToSingle(txtIA_ADCcal.Text) / Convert.ToSingle(txtIA_AmpCal.Text);
            IA_CalADC2Amp = Convert.ToSingle(txtIA_AmpCal.Text) / Convert.ToSingle(txtIA_ADCcal.Text);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_ADC2AMP, Main.MODULE_A, IA_CalADC2Amp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_AMP2ADC, Main.MODULE_A, IA_CalAmp2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_VALS, CalTripAmp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_ADC, Main.MODULE_A, Convert.ToInt16(IA_CalAmp2ADC * CalTripAmp));
            tabPageCurrent_Enter(null, null); // request refresh data
        }

        private void btnIB_SendCal_Click(object sender, EventArgs e)
        {
            float IB_CalAmp2ADC, IB_CalADC2Amp;
            byte CalTripAmp;
            CalTripAmp = Convert.ToByte(txtI_TripAmpCal.Text);
            IB_CalAmp2ADC = Convert.ToSingle(txtIB_ADCcal.Text) / Convert.ToSingle(txtIB_AmpCal.Text);
            IB_CalADC2Amp = Convert.ToSingle(txtIB_AmpCal.Text) / Convert.ToSingle(txtIB_ADCcal.Text);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_ADC2AMP, Main.MODULE_B, IB_CalADC2Amp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_AMP2ADC, Main.MODULE_B, IB_CalAmp2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_VALS, CalTripAmp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_ADC, Main.MODULE_B, Convert.ToInt16(IB_CalAmp2ADC * CalTripAmp));
            tabPageCurrent_Enter(null, null); // request refresh data
        }

        private void btnIC_SendCal_Click(object sender, EventArgs e)
        {
            float IC_CalAmp2ADC, IC_CalADC2Amp;
            byte CalTripAmp;
            CalTripAmp = Convert.ToByte(txtI_TripAmpCal.Text);
            IC_CalAmp2ADC = Convert.ToSingle(txtIC_ADCcal.Text) / Convert.ToSingle(txtIC_AmpCal.Text);
            IC_CalADC2Amp = Convert.ToSingle(txtIC_AmpCal.Text) / Convert.ToSingle(txtIC_ADCcal.Text);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_ADC2AMP, Main.MODULE_C, IC_CalADC2Amp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_AMP2ADC, Main.MODULE_C, IC_CalAmp2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_VALS, CalTripAmp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_ADC, Main.MODULE_C, Convert.ToInt16(IC_CalAmp2ADC * CalTripAmp));
            tabPageCurrent_Enter(null, null); // request refresh data
        }

        private void btnID_SendCal_Click(object sender, EventArgs e)
        {
            float ID_CalAmp2ADC, ID_CalADC2Amp;
            byte CalTripAmp;
            CalTripAmp = Convert.ToByte(txtI_TripAmpCal.Text);
            ID_CalAmp2ADC = Convert.ToSingle(txtID_ADCcal.Text) / Convert.ToSingle(txtID_AmpCal.Text);
            ID_CalADC2Amp = Convert.ToSingle(txtID_AmpCal.Text) / Convert.ToSingle(txtID_ADCcal.Text);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_ADC2AMP, Main.MODULE_D, ID_CalADC2Amp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_CURRENTS_AMP2ADC, Main.MODULE_D, ID_CalAmp2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_VALS, CalTripAmp);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CURRENT_TRIP_ADC, Main.MODULE_D, Convert.ToInt16(ID_CalAmp2ADC * CalTripAmp));
            tabPageCurrent_Enter(null, null); // request refresh data
        }

        private void btnPfwrdCheck_Click(object sender, EventArgs e)
        {
            UInt16 ADCcal, TripPwrdCal;
            UInt16 PCal, MaxPower;
            if (CheckPowerTextBoxes(txtPfwrdWattCal, txtPfwrdADCcal, txtPfwrdTripWattCal ,out PCal,  out ADCcal, out TripPwrdCal ) == true)
            {
                 MaxPower = Convert.ToUInt16((float) PCal / ADCcal * 1023);
                 txtPfwrdMaxWatt.Text = MaxPower.ToString();
                 btnPfwrdSendCal.Enabled = true;
            }
            else btnPfwrdSendCal.Enabled = false;
        }

        private void btnPreflCheck_Click(object sender, EventArgs e)
        {
            UInt16 ADCcal, TripPwrdCal;
            UInt16 PCal, MaxPower;
            if (CheckPowerTextBoxes(txtPreflWattCal, txtPreflADCcal, txtPreflTripWattCal, out PCal, out ADCcal, out TripPwrdCal) == true)
            {
                MaxPower = Convert.ToUInt16((float)PCal / ADCcal * 1023);
                txtPreflWattMax.Text = MaxPower.ToString();
                btnPreflSendCal.Enabled = true;
            }
            else btnPreflSendCal.Enabled = false;

        }

        private void btnPinpCheck_Click(object sender, EventArgs e)
        {
            UInt16 ADCcal, TripPwrdCal;
            UInt16 PCal, MaxPower;
            if (CheckPowerTextBoxes(txtPinpWattCal, txtPinpADCcal, txtPinpTripWattCal, out PCal, out ADCcal, out TripPwrdCal) == true)
            {
                MaxPower = Convert.ToUInt16((float)PCal / ADCcal * 1023);
                txtPinpWattMax.Text = MaxPower.ToString();
                btnPinpSendCal.Enabled = true;
            }
            else btnPinpSendCal.Enabled = false;
        }

        private void btnSWRCheck_Click(object sender, EventArgs e)
        {

        }

        private void bntPfwrdSendCal_Click(object sender, EventArgs e)
        {
            float P_CalW2ADC, P_CalADC2W;
            UInt16 P_CalTripW;
            P_CalTripW = Convert.ToUInt16(txtPfwrdTripWattCal.Text);
            P_CalW2ADC = Convert.ToSingle(txtPfwrdADCcal.Text) / Convert.ToSingle(txtPfwrdWattCal.Text);
            P_CalADC2W = Convert.ToSingle(txtPfwrdWattCal.Text) / Convert.ToSingle(txtPfwrdADCcal.Text);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_POWERS_ADC2W, Main.POWER_FWRD, P_CalADC2W);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_POWERS_W2ADC, Main.POWER_FWRD, P_CalW2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_VAL, Main.POWER_FWRD, P_CalTripW);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_ADC, Main.POWER_FWRD, Convert.ToInt16(P_CalW2ADC * P_CalTripW));
            tabPagePower_Enter(null, null); // request refresh data
        }

        private void btnPreflSendCal_Click(object sender, EventArgs e)
        {
            float P_CalW2ADC, P_CalADC2W;
            UInt16 P_CalTripW;
            P_CalTripW = Convert.ToUInt16(txtPreflTripWattCal.Text);
            P_CalW2ADC = Convert.ToSingle(txtPreflADCcal.Text) / Convert.ToSingle(txtPreflWattCal.Text);
            P_CalADC2W = Convert.ToSingle(txtPreflWattCal.Text) / Convert.ToSingle(txtPreflADCcal.Text);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_POWERS_ADC2W, Main.POWER_REFL, P_CalADC2W);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_POWERS_W2ADC, Main.POWER_REFL, P_CalW2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_VAL, Main.POWER_REFL, P_CalTripW);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_ADC, Main.POWER_REFL, Convert.ToInt16(P_CalW2ADC * P_CalTripW));
            tabPagePower_Enter(null, null); // request refresh data
        }

        private void btnPinpSendCal_Click(object sender, EventArgs e)
        {
            float P_CalW2ADC, P_CalADC2W;
            UInt16 P_CalTripW;
            P_CalTripW = Convert.ToUInt16(txtPinpTripWattCal.Text);
            P_CalW2ADC = Convert.ToSingle(txtPinpADCcal.Text) / Convert.ToSingle(txtPinpWattCal.Text);
            P_CalADC2W = Convert.ToSingle(txtPinpWattCal.Text) / Convert.ToSingle(txtPinpADCcal.Text);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_POWERS_ADC2W, Main.POWER_IN, P_CalADC2W);
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_POWERS_W2ADC, Main.POWER_IN, P_CalW2ADC);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_VAL, Main.POWER_IN, P_CalTripW);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_ADC, Main.POWER_IN, Convert.ToInt16(P_CalW2ADC * P_CalTripW));
            tabPagePower_Enter(null, null); // request refresh data
        }

        private void btnSWRSendCal_Click(object sender, EventArgs e)
        {

        }

        private void tabPageCurrent_Leave(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.SET_STATUS_AUTOTX_CURRENTS_ADC,0);
        }

        private void tabPagePower_Leave(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.SET_STATUS_AUTOTX_POWERS_ADC, 0);
        }
    }
}
