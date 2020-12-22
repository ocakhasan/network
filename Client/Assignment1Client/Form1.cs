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
                    button_disconnect.Enabled = true;
                    button_send.Enabled = true;
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
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));              //The message about the connection to server.

                    //Yes means, we connected successfully. 
                    if (incomingMessage == "Yes")
                    {
                        logs.AppendText("Connected to the server\n");
                    }
                    
                    else if (incomingMessage == "No")               //No means we do not connected. There exists a name.
                    {
                        logs.AppendText("Not connected to the server. There exists a client with same name. Enter another name \n");
                        button_connect.Enabled = true;
                        button_send.Enabled = false;
                        button_disconnect.Enabled = false;
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

        /*
            This function gets the filename from the complete path.
        */

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
                    message = "dosya" + filename + "." + message + "!end!";                 //add the file name to file content. Will be parsed by the server.
                    int length_message = message.Length;

                    logs.AppendText("Dosya boyutu " + length_message + "\n");
                    int num = 500 * 1024;

                    int howmany = (length_message / num) + 1;

                    logs.AppendText("how many is " + howmany + "\n");
                    int cur_index = 0;
                    for(int i = 0; i < howmany; i++)
                    {
                        Byte[] buffer = new Byte[num];
                        if(cur_index + num < length_message)
                        {
                            buffer = Encoding.Default.GetBytes(message.Substring(cur_index, num));
                        }
                        else
                        {
                            string last_message = message.Substring(cur_index, length_message - cur_index);
                            buffer = Encoding.Default.GetBytes(last_message);
                        }
                        cur_index = cur_index + num;
                        clientSocket.Send(buffer);
                        
                    }
                    logs.AppendText("You sent a file called " + filename + ".txt\n");
                }
/*
                Byte[] buffer2 = new Byte[1024];
                buffer2 = Encoding.Default.GetBytes("!end!");
                clientSocket.Send(buffer2);
*/
                logs.AppendText("End data is sent\n");
            }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {
            connected = false;
            terminating = true;
            clientSocket.Close();

            // change the status of corresponding buttons.
            button_send.Enabled = false;
            button_disconnect.Enabled = false;
            button_connect.Enabled = true;
        }
    }
}
