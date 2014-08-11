using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections.ObjectModel;

namespace PAcontroller
{
    public class Communication
    {

        private SerialPort port;

        private enum State
        {
            sof,
            sync,
            id,
            dlc,
            eof,
            unknown
        }

        const byte SOF_BYTE  = 0xAA;
        const byte SYNC_BYTE = 0x55;
        const byte EOF_BYTE  = 0x66;

        private State status = State.unknown;
        private byte id, dlc,bytesToRead;


        public bool PortIsOpen { get; set; }

        Message currentMessage;

        public ObservableCollection<Message> messageList;

        public void SerialCommunicationInit()
        {
            port = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            currentMessage = new Message();
            port.Open();
        }

        public void SerialCommunicationInit(String COMx)
        {
            try
            {
                port = new SerialPort(COMx, 115200, Parity.None, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                currentMessage = new Message();
                port.Open();
                PortIsOpen = true;
            }
            catch (Exception e)
            {
                PortIsOpen = false;
            }
        }

        public void SerialCommunicationClose()
        {
            port.DataReceived -= port_DataReceived;
            System.Threading.Thread.Sleep(100);  //needed to not get exception
            port.Close();
        }

        


        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            while (port.BytesToRead != 0)
            {
                byte temp = (byte)port.ReadByte();
                switch (status)
                {
                    case State.unknown:
                        if (temp == 0xAA)
                            status = State.sof;
                        break;
                    case State.sof:
                        if (temp == 0x55)
                            status = State.sync;
                        else
                            status = State.unknown;
                        break;
                    case State.sync:
                        currentMessage.id = temp;
                        status = State.id;
                        break;
                    case State.id:
                        currentMessage.dlc = temp;
                        bytesToRead = currentMessage.dlc;
                        if (bytesToRead == 0)
                            status = State.eof;
                        else
                            status = State.dlc;
                        break;
                    case State.dlc:
                        currentMessage.addData(temp);
                        bytesToRead--;
                        if (bytesToRead == 0)
                            status = State.eof;
                        break;
                    case State.eof:
                        //switch (rx.id)
                        //{
                        //    case 0:
                        //        sendDate();
                        //        break;
                        //    case 1:
                        //        readDate();
                        //        break;
                        //    default:
                        //        break;
                        //}
                        if (temp == 0x66)
                        {
                            lock (messageList)
                            {
                                messageList.Add(currentMessage);
                            }
                            currentMessage = new Message();
                        }
                        status = State.unknown;
                        break;
                    default:
                        status = State.unknown;
                        break;
                }
            }
        }

        public void port_sendSerialMessage(Message TxMessage)
        {
            if (port.IsOpen)
            {
                byte[] buffer = new byte[TxMessage.dlc + 4];
                buffer[0] = SOF_BYTE;
                port.Write(buffer, 0, 1);
                buffer[0] = SYNC_BYTE;
                port.Write(buffer, 0, 1);
                buffer[0] = TxMessage.id;
                port.Write(buffer, 0, 1);
                buffer[0] = TxMessage.dlc;
                port.Write(buffer, 0, 1);
                port.Write(TxMessage.messageData.ToArray(), 0, TxMessage.dlc);
                buffer[0] = EOF_BYTE;
                port.Write(buffer, 0, 1);
            }
        }

        public void SendMessage(Main.MsgIDsToMCU Id)
        {
            Message msg = new Message();
            msg.ConstructMessage(Id);
            port_sendSerialMessage(msg);
        }


        public void SendMessage(Main.MsgIDsToMCU Id, byte val)
        {
            Message msg = new Message();
            msg.ConstructMessage(Id, val);
            port_sendSerialMessage(msg);
        }

        //public void SendMessage(Main.MsgIDsToMCU Id, Int16 val)
        //{
        //    Message msg = new Message();
        //    msg.ConstructMessage(Id, val);
        //    port_sendSerialMessage(msg);
        //}

        public void SendMessage(Main.MsgIDsToMCU Id, Byte ModId, Int16 val)
        {
            Message msg = new Message();
            msg.ConstructMessage(Id, ModId, val);
            port_sendSerialMessage(msg);
        }

        public void SendMessage(Main.MsgIDsToMCU Id, Byte ModId, UInt16 val)
        {
            Message msg = new Message();
            msg.ConstructMessage(Id, ModId, val);
            port_sendSerialMessage(msg);
        }

        public void SendMessage(Main.MsgIDsToMCU Id, Byte PowerType, Byte NrOfPoints, UInt16[] CalPoints, Single[] RCvals, Single[] Bvals)
        {
            Message msg = new Message();
            msg.ConstructMessage(Id, PowerType, NrOfPoints, CalPoints, RCvals, Bvals);
            port_sendSerialMessage(msg);
        }

        //public void SendMessage(Main.MsgIDsToMCU Id, Single val)
        //{
        //    Message msg = new Message();
        //    msg.ConstructMessage(Id, val);
        //    port_sendSerialMessage(msg);
        //}

        public void SendMessage(Main.MsgIDsToMCU Id, Byte ModId, Single val)
        {
            Message msg = new Message();
            msg.ConstructMessage(Id, ModId, val);
            port_sendSerialMessage(msg);
        }
    }
}
