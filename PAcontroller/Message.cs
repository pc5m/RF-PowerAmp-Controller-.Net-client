using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAcontroller
{
    public class Message
    {
        private byte _id;
        private byte _dlc;
        private List<byte> _messageData = new List<byte>();

        public byte id
        {
            set { this._id = value; }
            get { return this._id; }
        }
        public byte dlc
        {
            set { this._dlc = value; }
            get { return this._dlc; }
        }

        public List<byte> messageData
        {
            set { this._messageData = value; }
            get { return this._messageData; }
        }

        public byte getData(int index)
        {
            return messageData[index];
        }


        public void addData(byte data_in)
        {
            messageData.Add(data_in);
        }

        public void ConstructMessage(Main.MsgIDsToMCU Id, Single val)
        {
            id = (byte)Id;
            messageData.AddRange(BitConverter.GetBytes(val));
            dlc = (byte)messageData.Count();
        }

        public void ConstructMessage(Main.MsgIDsToMCU Id, Byte ModId, Single val)
        {
            id = (byte)Id;
            messageData.Add(ModId);
            messageData.AddRange(BitConverter.GetBytes(val));
            dlc = (byte)messageData.Count();
        }

        public void ConstructMessage(Main.MsgIDsToMCU Id, Int16 val)
        {
            id = (byte)Id;
            messageData.AddRange(BitConverter.GetBytes(val));
            dlc = (byte)messageData.Count();
        }


        public void ConstructMessage(Main.MsgIDsToMCU Id, Byte ModId, Int16 val)
        {
            id = (byte)Id;
            messageData.Add(ModId);
            messageData.AddRange(BitConverter.GetBytes(val));
            dlc = (byte)messageData.Count();
        }


        public void ConstructMessage(Main.MsgIDsToMCU Id, Byte ModId, UInt16 val)
        {
            id = (byte)Id;
            messageData.Add(ModId);
            messageData.AddRange(BitConverter.GetBytes(val));
            dlc = (byte)messageData.Count();
        }

        public void ConstructMessage(Main.MsgIDsToMCU Id, byte val)
        {
            id = (byte)Id;
            messageData.Add(val);
            dlc = (byte)messageData.Count();
        }

        public void ConstructMessage(Main.MsgIDsToMCU Id)
        {
            id = (byte)Id;
            dlc = 0;
        }

        public void ConstructMessage(Main.MsgIDsToMCU Id, Byte PowerType, Byte NrOfPoints, UInt16[] CalPoints, Single[] RCvals, Single[] Bvals)
        {
            id = (byte)Id;
            messageData.Add(PowerType);
            messageData.Add(NrOfPoints);
            foreach (UInt16 data in CalPoints)
                messageData.AddRange(BitConverter.GetBytes(data));
            foreach (Single data in RCvals)
                messageData.AddRange(BitConverter.GetBytes(data));
            foreach (Single data in Bvals)
                messageData.AddRange(BitConverter.GetBytes(data));
            dlc = (byte)messageData.Count();
        }

    }
}
