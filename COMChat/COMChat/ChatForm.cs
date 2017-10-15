using System;
using System.Text;
using System.Windows.Forms;

namespace COMChat
{
    public partial class ChatForm : Form
    {
        private SerialPortController serialPort;

        public ChatForm()
        {
            InitializeComponent();
            serialPort = new SerialPortController(ShowData);
        }

        private void canConnect(object sender, EventArgs e)
        {
            button1.Enabled = (textBox1.Text.Length != 0) && (comboBox1.SelectedIndex > -1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (button1.Text)
            {
                case "Connect": Connect(); break;
                case "Disconnect": Disconnect(); break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort.SendData(Encoding.Default.GetBytes(richTextBox2.Text));
            ShowData(richTextBox2.Text, true);
            richTextBox2.Text = "";
        }

        private void ShowData(string data, bool sender)
        {
            richTextBox1.Text += ((sender ? "↑" : "↓") + " [" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + data + "\n");
        }

        private void Connect()
        {
            serialPort.Connect(textBox1.Text, Int32.Parse(comboBox1.SelectedItem.ToString()),
                (byte)SourceByte.Value,(byte)DestinationByte.Value);
            button1.Text = "Disconnect";
            textBox1.Enabled = false;
            comboBox1.Enabled = false;
            SourceByte.Enabled = false;
            DestinationByte.Enabled = false;
            button2.Enabled = true;        
        }

        private void Disconnect()
        {
            serialPort.Disconnect();
            button1.Text = "Connect";
            textBox1.Enabled = true;
            comboBox1.Enabled = true;
            SourceByte.Enabled = true;
            DestinationByte.Enabled = true;
            button2.Enabled = false;
        }
    }
}
