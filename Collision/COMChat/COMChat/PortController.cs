using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace COMChat
{
    class PortController
    {
        private SerialPort _serialPort;
        public delegate void ShowDataHandler(string data, bool sender);
        private readonly ShowDataHandler _handler;
        private const int Attempts = 10;
        private const byte JamByte = 0x8C;
        private const byte EndByte = 0x8E;
        private const int CollisionWindowTimeout = 10;
        private const int SlotTimeLength = 30;

        public PortController(ShowDataHandler handler)
        {
            _handler = handler;
        }

        public void Connect(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Open();
            _serialPort.DataReceived += DataReceived;
        }

        public void Disconnect()
        {
            _serialPort.Close();
        }

        public void SendData(byte[] data)
        {
            foreach (var _byte in data)
            {
                SendByte(_byte);
            }
            _serialPort.Write(new[] { EndByte }, 0, 1);
        }

        private void SendByte(byte _byte)
        {
            int i = 0;
            while(true)
            {
                while (!IsChannelFreeOrCollisionIs()) { }
                _serialPort.Write(new[] { _byte }, 0, 1);
                Thread.Sleep(CollisionWindowTimeout);
                if (!IsChannelFreeOrCollisionIs())
                {
                    return;
                }
                _serialPort.Write(new[] { JamByte }, 0, 1);
#if DEBUG
                Console.WriteLine(JamByte);
#endif
                if (++i == Attempts)
                    throw new Exception("Error!!!");
                Thread.Sleep(new Random().Next(0, (int)Math.Pow(2, Math.Min(i, 10))) * SlotTimeLength);
            }
            
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var _message = new List<byte>();
            while (true)
            {
                if (_serialPort.BytesToRead == 0)
                {
                    continue;
                }
                var _byte = (byte)_serialPort.ReadByte();
                if (_byte == EndByte)
                {
                    break;
                }
                if (_byte == JamByte)
                {
                    _message.RemoveAt(_message.Count - 1);
                }
                else
                {
                    _message.Add(_byte);
                }
            }
            _handler.Invoke(System.Text.Encoding.Default.GetString(_message.ToArray()), false);
        }

        private bool IsChannelFreeOrCollisionIs()
        {
            return DateTime.Now.TimeOfDay.Milliseconds % 2 == 0;
        }
    }
}
