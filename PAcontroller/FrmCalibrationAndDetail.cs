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

        public enum PowerType
        {
            Forward = 0,
            Reflected = 1,
            Input = 2,
            SWR = 3,
        }

        BindingSource bsPfwrdCalValues = new BindingSource();
        BindingSource bsPfwrd_RC_B_ADC2W = new BindingSource();
        BindingSource bsPfwrd_RC_B_W2ADC = new BindingSource();

        BindingSource bsPreflCalValues = new BindingSource();
        BindingSource bsPrefl_RC_B_ADC2W = new BindingSource();
        BindingSource bsPrefl_RC_B_W2ADC = new BindingSource();

        BindingSource bsPinpCalValues = new BindingSource();
        BindingSource bsPinp_RC_B_ADC2W = new BindingSource();
        BindingSource bsPinp_RC_B_W2ADC = new BindingSource();

        dsCalibration dsCal = new dsCalibration();  
      
        

        public FrmCalibrationAndDetail()
        {
            InitializeComponent();
        }

        private void FormDetails_Load(object sender, EventArgs e)
        {
            btnIA_SendCal.Enabled = false;
            btnIB_SendCal.Enabled = false;
            btnIC_SendCal.Enabled = false;
            btnID_SendCal.Enabled = false;

            bsPfwrdCalValues.DataSource = dsCal.dtPfwrdCalVals;
            bsPfwrd_RC_B_ADC2W.DataSource = dsCal.dtPfwrd_RC_B_ADC2W;
            bsPfwrd_RC_B_W2ADC.DataSource = dsCal.dtPfwrd_RC_B_W2ADC;


            bsPreflCalValues.DataSource = dsCal.dtPreflCalVals;
            bsPrefl_RC_B_ADC2W.DataSource = dsCal.dtPrefl_RC_B_ADC2W;
            bsPrefl_RC_B_W2ADC.DataSource = dsCal.dtPrefl_RC_B_W2ADC;


            bsPinpCalValues.DataSource = dsCal.dtPinpCalVals;
            bsPinp_RC_B_ADC2W.DataSource = dsCal.dtPinp_RC_B_ADC2W;
            bsPinp_RC_B_W2ADC.DataSource = dsCal.dtPinp_RC_B_W2ADC;

            dgvPfwrd.DataSource = bsPfwrdCalValues;
       //     dgvPfwrd_RC_B_ADC2W.DataSource = bsPfwrd_RC_B_ADC2W;
       //     dgvPfwrd_RC_B_W2ADC.DataSource = bsPfwrd_RC_B_W2ADC;

            dgvPrefl.DataSource = bsPreflCalValues;
        //    dgvPrefl_RC_B_ADC2W.DataSource = bsPrefl_RC_B_ADC2W;
        //    dgvPrefl_RC_B_W2ADC.DataSource = bsPrefl_RC_B_W2ADC;

            dgvPinp.DataSource = bsPinpCalValues;
            //    dgvPinp_RC_B_ADC2W.DataSource = bsPinp_RC_B_ADC2W;
            //    dgvPinp_RC_B_W2ADC.DataSource = bsPinp_RC_B_W2ADC;

            btnUpdateControllerPfwrd.Enabled = false;
            btnUpdateControllerPrefl.Enabled = false;
            btnUpdateControllerPinp.Enabled = false;
        }

        float Interpolate(Int16 x, List<Int16> xArray, List<float> RCArray, List<float> BArray, Int16 nrOfVals)
        {
            int i;
            for (i = nrOfVals - 2; i > 0; i--)
            {
                if (x >= xArray[i]) break;
            }
            return (RCArray[i] * x + BArray[i]);
        }

        void LoadCalibrationTables(Message message, DataTable dtCalVals, DataTable dt_RC_B_ADC2W, DataTable dt_RC_B_W2ADC)
        {
            Byte NrofDatapoints = message.messageData[1];
            // txtBoxNrPoints.Text = NrofDatapoints.ToString();
            List<float> RClist = new List<float>();
            List<float> Blist = new List<float>();
            List<Int16> Xlist = new List<Int16>();

            dtCalVals.Clear();
            dt_RC_B_ADC2W.Clear();
            dt_RC_B_W2ADC.Clear();
            for (int i = 0; i < NrofDatapoints - 1; i++)
            {
                float RC, B;
                RC = BitConverter.ToSingle(message.messageData.GetRange(12 + i * 4, 4).ToArray(), 0);
                B = BitConverter.ToSingle(message.messageData.GetRange(28 + i * 4, 4).ToArray(), 0);
                RClist.Add(RC);
                Blist.Add(B);
                dt_RC_B_ADC2W.Rows.Add(RC, B);
            }
            for (int i = 0; i < NrofDatapoints; i++)
            {
                Int16 x;
                x = BitConverter.ToInt16(message.messageData.GetRange(2 + i * 2, 2).ToArray(), 0);
                Xlist.Add(x);
                dtCalVals.Rows.Add(0, x);
            }
            for (int i = 0; i < NrofDatapoints; i++)
            {
                Int16 x;
                x = BitConverter.ToInt16(message.messageData.GetRange(2 + i * 2, 2).ToArray(), 0);
                Xlist.Add(x);
                // DataRow rw;
                // dsCalibration.dtPfwrdCalValsRow PforwardRow;
                // PforwardRow = dsCal.dtPfwrdCalVals[i];
                // rw = dtCalVals.Rows[i];
                dtCalVals.Rows[i]["Power"] = (Int16)Interpolate(x, Xlist, RClist, Blist, NrofDatapoints);
            }

        }


        private void FillCalibration(Message message)
        {
            switch (message.messageData[0])
            {
                case (int)PowerType.Forward:
                    LoadCalibrationTables(message, dsCal.dtPfwrdCalVals, dsCal.dtPfwrd_RC_B_ADC2W, dsCal.dtPfwrd_RC_B_W2ADC);

                break;
                case (int)PowerType.Reflected:
                    LoadCalibrationTables(message, dsCal.dtPreflCalVals, dsCal.dtPrefl_RC_B_ADC2W, dsCal.dtPrefl_RC_B_W2ADC);
                break;
                case (int)PowerType.Input:
                    LoadCalibrationTables(message, dsCal.dtPinpCalVals, dsCal.dtPinp_RC_B_ADC2W, dsCal.dtPinp_RC_B_W2ADC);
                break;
                case (int)PowerType.SWR:
                    // to be implemented
                break;
            }
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
                case Main.MsgIDsFromMCU.PowerCalibrationW2ADC_RC_B:
                    // not implemented yet , needed ?
                    break;
                case Main.MsgIDsFromMCU.PowerCalibrationADC2W_RC_B:
                    FillCalibration(message);
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

            //request power values
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W_RC_B,(byte)PowerType.Forward);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W_RC_B,(byte)PowerType.Reflected);
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W_RC_B,(byte)PowerType.Input);

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

        private void tabPageCurrent_Leave(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.SET_STATUS_AUTOTX_CURRENTS_ADC,0);
        }

        private void tabPagePower_Leave(object sender, EventArgs e)
        {
            comm.SendMessage(Main.MsgIDsToMCU.SET_STATUS_AUTOTX_POWERS_ADC, 0);
        }


        private float CheckCalValuesAndTrip(BindingSource bsCalValues, BindingSource bsRC_B_ADC2W, BindingSource bsRC_B_W2ADC, Int16 Wtrip)
        {
            List<Int16> adcList = new List<Int16>();
            List<Int16> powerList = new List<Int16>();

            // sort ascending using the ADC column
            bsCalValues.Sort = "ADC ASC";

            ((DataTable)bsRC_B_ADC2W.DataSource).Clear();
            ((DataTable)bsRC_B_W2ADC.DataSource).Clear();

            foreach (DataRowView row in bsCalValues)
            {
                adcList.Add((Int16)row["ADC"]);
                powerList.Add((Int16)row["Power"]);
            }
            // txtBoxNrPoints.Text = bsPfwrdCalValues.Count.ToString();

            FillRCandBvals((DataTable)bsRC_B_ADC2W.DataSource, adcList, powerList);
            FillRCandBvals((DataTable)bsRC_B_W2ADC.DataSource, powerList, adcList);

            return LinearInterpolate(Wtrip, powerList, (DataTable)bsRC_B_W2ADC.DataSource);
        }

        private void btnCheckPfrwd_Click(object sender, EventArgs e)
        {
            Int16 PfwrdTrip;
            if (Int16.TryParse(txtBoxPfwrdTrip.Text, out PfwrdTrip))
            {
                txtBoxPfwrdTripADC.Text = Convert.ToString(Convert.ToInt16(CheckCalValuesAndTrip(bsPfwrdCalValues, bsPfwrd_RC_B_ADC2W, bsPfwrd_RC_B_W2ADC, PfwrdTrip)));
                btnUpdateControllerPfwrd.Enabled = true;
            }
            else btnUpdateControllerPfwrd.Enabled = false;
        }

        private void btnCheckPrefl_Click(object sender, EventArgs e)
        {
            Int16 PreflTrip;
            if (Int16.TryParse(txtBoxPreflTrip.Text, out PreflTrip))
            {
                txtBoxPreflTripADC.Text = Convert.ToString(Convert.ToInt16(CheckCalValuesAndTrip(bsPreflCalValues, bsPrefl_RC_B_ADC2W, bsPrefl_RC_B_W2ADC, PreflTrip)));
                btnUpdateControllerPrefl.Enabled = true;
            }
            else btnUpdateControllerPrefl.Enabled = false;
        }

        private void btnCheckPinput_Click(object sender, EventArgs e)
        {
            Int16 PinpTrip;
            if (Int16.TryParse(txtBoxPinpTrip.Text, out PinpTrip))
            {
                txtBoxPinpTripADC.Text = Convert.ToString(Convert.ToInt16(CheckCalValuesAndTrip(bsPinpCalValues, bsPinp_RC_B_ADC2W, bsPinp_RC_B_W2ADC, PinpTrip)));
                btnUpdateControllerPinp.Enabled = true;
            }
            else btnUpdateControllerPinp.Enabled = false;
        }


        private float LinearInterpolate(Int16 x, List<Int16> XvaluesList, DataTable RC_B_table)
        {
            int IndexFound = XvaluesList.FindIndex(delegate(Int16 xx)
            {
                return x <= xx;
            }
            );
            if (IndexFound == 0) IndexFound = 1;
            if (IndexFound == -1) IndexFound = XvaluesList.Count() - 1; //above last datapoint
            return (Convert.ToSingle(RC_B_table.Rows[IndexFound - 1]["RC"]) * x + Convert.ToSingle(RC_B_table.Rows[IndexFound - 1]["B"]));
        }

        private void FillRCandBvals(DataTable RC_B_table, List<Int16> xArray, List<Int16> yArray)
        {
            RC_B_table.Clear();
            float RC;
            // for (int i = 1; i < xArray.Count() && i < 5; i++)
            for (int i = 1; i < xArray.Count() ; i++)
            {
                RC = ((float)yArray[i] - yArray[i - 1]) / ((float)xArray[i] - xArray[i - 1]);
                RC_B_table.Rows.Add(RC, (yArray[i] - xArray[i] * RC));
            }
        }

        private void UpdateControllerPowerCal(PowerType powerType, BindingSource bsCalValues, BindingSource bsRC_B_vals)
        {
            UInt16[] CalList = new UInt16[5];
            Single[] RCList = new  Single[4];
            Single[] BList = new   Single[4];

            // sort ascending using the ADC column
            bsCalValues.Sort = "ADC ASC";

            int i = 0;

            foreach (DataRowView row in bsRC_B_vals)
            {
                i++;
                RCList[i-1] = Convert.ToSingle(row["RC"]);
                BList[i-1] =  Convert.ToSingle(row["B"]);
                if (i == 4) break;
            }

            i = 0;
            foreach (DataRowView row in bsCalValues)
            {
                i++; 
                CalList[i-1] = Convert.ToUInt16(row["ADC"]);
                if (i == 5) break;
            }
            comm.SendMessage(Main.MsgIDsToMCU.SET_CAL_POWERS_ADC2W_RC_B, (byte)powerType, (byte)i , CalList, RCList, BList );
            tabPagePower_Enter(null, null); // request refresh data
        }

        private void btnUpdateControllerPfwrd_Click(object sender, EventArgs e)
        {
            UpdateControllerPowerCal(PowerType.Forward, bsPfwrdCalValues, bsPfwrd_RC_B_ADC2W);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_VAL, Main.POWER_FWRD, Convert.ToInt16(txtBoxPfwrdTrip.Text));
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_ADC, Main.POWER_FWRD, Convert.ToInt16(txtBoxPfwrdTripADC.Text));

        }

        private void btnUpdateControllerPrefl_Click(object sender, EventArgs e)
        {
            UpdateControllerPowerCal(PowerType.Reflected, bsPreflCalValues, bsPrefl_RC_B_ADC2W);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_VAL, Main.POWER_REFL, Convert.ToInt16(txtBoxPreflTrip.Text));
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_ADC, Main.POWER_REFL, Convert.ToInt16(txtBoxPreflTripADC.Text));
            comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W_RC_B, (byte)PowerType.Reflected);
        }

        private void btnUpdateControllerPinp_Click(object sender, EventArgs e)
        {
            UpdateControllerPowerCal(PowerType.Input, bsPinpCalValues, bsPinp_RC_B_ADC2W);
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_VAL, Main.POWER_IN, Convert.ToInt16(txtBoxPinpTrip.Text));
            comm.SendMessage(Main.MsgIDsToMCU.SET_POWERS_TRIP_ADC, Main.POWER_IN, Convert.ToInt16(txtBoxPinpTripADC.Text));
        }


        //private void btnReloadPwrd_Click(object sender, EventArgs e)
        //{
        //    comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W_RC_B, (byte)PowerType.Forward);
        //}

        //private void btnReloadPrefl_Click(object sender, EventArgs e)
        //{
        //    comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W_RC_B, (byte)PowerType.Reflected);
        //}

        //private void btnReloadPinput_Click(object sender, EventArgs e)
        //{
        //    comm.SendMessage(Main.MsgIDsToMCU.REQ_CAL_POWERS_ADC2W_RC_B, (byte)PowerType.Input);
        //}




    }
}