using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment1Client
{

   

    public partial class Form1 : Form
    {
        bool terminating = false;
        bool connected = false;
        Socket clientSocket;
        string name;


        public Form1()

        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_ip.Text;
            string nameinput = textBox_name.Text;
            name = nameinput;

            

            int portNum;
            if (Int32.TryParse(textBox_port.Text, out portNum))
            {
                try
                {
                    clientSocket.Connect(IP, portNum);
                    button_connect.Enabled = false;
                    connected = true;


                    Byte[] namebuffer = new Byte[64];
                    namebuffer = Encoding.Default.GetBytes(name);
                    clientSocket.Send(namebuffer);
                    logs.AppendText("Sended server the name. Trying to connect.. \n");

                    Thread receiveThread = new Thread(Receive);
                    receiveThread.Start();

                }
                catch
                {
                    logs.AppendText("Could not connect to the server!\n");
                }
            }
            else
            {
                logs.AppendText("Check the port\n");
            }
        }

        private void Receive()
        {
            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    clientSocket.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));

                    
                    if (incomingMessage == "Yes")
                    {
                        logs.AppendText("Connected to the server\n");
                    }
                    else if (incomingMessage == "No")
                    {
                        logs.AppendText("Not connected to the server. There exists a client with same name. Enter another name \n");
                        button_connect.Enabled = true;
                        connected = false;
                    }
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("The server has disconnected\n");
                        button_connect.Enabled = true;
                        button_send.Enabled = false;
                    }

                    clientSocket.Close();
                    connected = false;
                }

            }
        }


        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);

        }

        private string getFileNameFromPath(string fullpath)
        {
            int last_index = fullpath.LastIndexOf('\\');
            string result = fullpath.Substring(last_index + 1);
            return result.Substring(0, result.IndexOf('.'));
        }

        private void button_send_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.txt"; // file types, that will be allowed to upload
            dialog.Multiselect = false; // allow/deny user to upload more than one file at a time
            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                String path = dialog.FileName; // get name of file
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open), new UTF8Encoding())) // do anything you want, e.g. read it
                {
                    string filename = getFileNameFromPath(path);        //extract filename from path
                    string message = reader.ReadToEnd();
                    message = filename + "." + message;
                    int length_message = message.Length;
                    logs.AppendText("You sent a file called " +filename +  ".txt\n");
                    Byte[] buffer = new Byte[message.Length];
                    buffer = Encoding.Default.GetBytes(message);
                    clientSocket.Send(buffer);
                    
                }
            }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {
            connected = false;
            terminating = true;
            clientSocket.Close();
            button_send.Enabled = false;
            button_disconnect.Enabled = false;
        }
    }
}
