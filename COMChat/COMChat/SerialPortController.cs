using System.IO.Ports;
using System.Threading;

namespace COMChat
{
    class SerialPortController
    {
        public delegate void ShowDataHandler(string data, bool sender);

        private SerialPort serialPort;
        private ShowDataHandler handler;
        private Packer packer;

        public SerialPortController(ShowDataHandler _handler)
        {
            handler = _handler;
        }

        public void Connect(string portName, int baudRate, byte sourceByte, byte destinationByte)
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.Open();
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            packer = new Packer(sourceByte, destinationByte);
        }

        public void Disconnect()
        {
            serialPort.Close();
        }

        public void SendData(byte[] data)
        {
            byte[] dataForSend = packer.Packing(data);
            serialPort.RtsEnable = true;
            serialPort.Write(dataForSend, 0, dataForSend.Length);
            Thread.Sleep(100);
            serialPort.RtsEnable = false;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[serialPort.BytesToRead];            
            serialPort.Read(data, 0, data.Length);
            var acceptData = packer.Unpacking(data);
            if (acceptData == null)
                return;
            handler.Invoke(System.Text.Encoding.Default.GetString(acceptData), false);
        }
    }
}
