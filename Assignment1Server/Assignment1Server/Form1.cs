using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment1Server
{
    public partial class Form1 : Form
    {
        String predeterminedPath = "";
        List<String> names = new List<String>();
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientSockets = new List<Socket>();
        List<string> clientNames = new List<string>();
        List<int> clientFileCounts = new List<int>();

        bool terminating = false;
        bool listening = false;
        static string cwd = Directory.GetCurrentDirectory();
        static string fullPathDb = Path.Combine(cwd, "information.txt");



        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            if (!File.Exists(fullPathDb))
            {
                File.Create(fullPathDb).Close();
            }
        }

        private void button_listen_Click(object sender, EventArgs e)
        {
            int serverPort;

            if (Int32.TryParse(textBox_port.Text, out serverPort))
            {
                if (predeterminedPath == "")
                {
                    logs.AppendText("First choose the database folder\n");
                }
                else
                {

                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                    serverSocket.Bind(endPoint);
                    serverSocket.Listen(3);

                    listening = true;
                    button_listen.Enabled = false;

                    Thread acceptThread = new Thread(Accept);
                    acceptThread.Start();

                    logs.AppendText("Started listening on port: " + serverPort + "\n");
                }

            }
            else
            {
                logs.AppendText("Please check port number \n");
            }

        }

        private bool NameExists(string name)
        {
            for(int i = 0; i < clientNames.Count; i++)
            {
                if (clientNames[i] == name)
                {
                    return true;
                }
            }
            return false;
        }

        private int filenameExists(string client_name, string filename)
        {
           
            string line;
            int count = -1;
            StreamReader file = new StreamReader(fullPathDb);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    if (words[0] == client_name && words[1] == filename)
                    {
                        string strCount = words[2];
                        if (Int32.TryParse(strCount, out count))
                        {
                            count = count + 1;
                        }
                    }
                }
                return count;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally
            {
                file.Close();
                file.Dispose();
            }
            return -1;
        }


        private void Accept()
        {
            while (listening)
            {
                try
                {
                    Socket newClient = serverSocket.Accept();
                    clientSockets.Add(newClient);
                    Thread receiveThread = new Thread(() => Receive(newClient)); // updated
                    receiveThread.Start();
                    
                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        logs.AppendText("The socket stopped working.\n");
                    }

                }
            }
        }

        private string getFileNameFromString(string message, int index)
        {
            return message.Substring(5, index - 6);
        }

        private void Receive(Socket thisClient) // updated
        {
            bool connected = true;

            bool firstMessage = false;

            int ioc = 0;
            while (connected && !terminating)
            {
                try   //This will be the implementation of file acceptance and writing to the system
                {
                    if (!firstMessage)
                    {
                        Byte[] clientNameBuffer = new Byte[64];
                        thisClient.Receive(clientNameBuffer);
                        string newclientName = Encoding.Default.GetString(clientNameBuffer);
                        newclientName = newclientName.Substring(0, newclientName.IndexOf('\0'));
                        firstMessage = true;

                        if (NameExists(newclientName))
                        {
                            clientSockets.Remove(thisClient);
                            logs.AppendText("The client with " + newclientName +  " exists, socket not connected\n");

                            Byte[] acceptanceBuffer = new Byte[64];
                            acceptanceBuffer = Encoding.Default.GetBytes("No");
                            thisClient.Send(acceptanceBuffer);
                        }
                        else
                        {
                            logs.AppendText("Client with name " + newclientName + " is joined to server \n");
                            clientNames.Add(newclientName);
                            clientFileCounts.Add(0);
                            Byte[] acceptanceBuffer = new Byte[64];
                            acceptanceBuffer = Encoding.Default.GetBytes("Yes");
                            thisClient.Send(acceptanceBuffer);
                        }

                    }
                    else
                    {
                        int counter = 0;

                        int num = 1024 * 500;
                        Byte[] buffer = new Byte[num];                              //Düzelt burayı
                        thisClient.Receive(buffer);
                        string incomingMessage = Encoding.Default.GetString(buffer);
                        //incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                        string file_message = "";
                        string filename = "";

                        if(incomingMessage.Length > 0)
                        {
                            int index = incomingMessage.IndexOf('.');
                            filename = getFileNameFromString(incomingMessage, index);
                            int message_size = incomingMessage.Length;
                            incomingMessage = incomingMessage.Substring(index + 1);

                            //logs.AppendText("incomming message is " + incomingMessage + "\n");
                            file_message += incomingMessage;

                        }


                        while (incomingMessage.Length > 0)
                        {
                            


                            
                            Byte[] buffer2 = new Byte[num];

                            thisClient.Receive(buffer2);
                            counter++;
                            string partialdata = Encoding.Default.GetString(buffer2);
                            if(partialdata.IndexOf("\0") >= 0)
                            {
                                partialdata = partialdata.Substring(0, partialdata.IndexOf("\0"));
                            }
                            bool ends = partialdata.EndsWith("!end!");
                            string end_string = partialdata.Substring(partialdata.Length - 5, 5);
                            logs.AppendText("Counter is " + counter + "\n");
                            //logs.AppendText("end string is " + end_string + "\n");
                            if (partialdata.Length > 0)
                            {
                                file_message += partialdata;
                            }
                            if (end_string == "!end!")
                            {
                                logs.AppendText("partial data ends with !end!  with counter "+ counter + "\n");
                                break;
                            }


/*
                            else if(partialdata == "!end!")
                            {
                                logs.AppendText("end data is received \n");
                                break;
                            }
*/

                        }

                        logs.AppendText("I am here \n");
                        ioc = clientSockets.IndexOf(thisClient);
                        string clientName = clientNames[ioc];
                        int fileCountNumber = filenameExists(clientName, filename);
                        string filename_to_write = "";
                        logs.AppendText("I am here 2\n");

                        if (fileCountNumber != -1)
                        {

                            filename_to_write = clientNames[ioc] + filename + fileCountNumber.ToString() +  ".txt";            //olması gereken clientNames[ioc] + clientFiles[ioc]+ ".txt"
                        }
                        else
                        {
                            filename_to_write = clientNames[ioc] + filename  + ".txt";
                        }
                        logs.AppendText("I am here 3\n");

                        using (StreamWriter sw = File.AppendText(fullPathDb))
                        {
                            string to_send = "";
                            if(fileCountNumber != -1)
                            {
                                to_send = clientNames[ioc] + " " + filename + " " + fileCountNumber + " " + filename_to_write;
                            }
                            else
                            {
                                to_send = clientNames[ioc] + " " + filename + " " +  "0" + " " + filename_to_write;
                            }
                            sw.WriteLine(to_send); // Write text to .txt file
                        }
                        logs.AppendText("I am here 4\n");

                        using (StreamWriter sw = File.AppendText(predeterminedPath + "\\" + filename_to_write))
                        {
                            sw.WriteLine(file_message); // Write text to .txt file
                        }

                        logs.AppendText("Client " + clientNames[ioc] + " send a file called " + filename_to_write + "\n");
                        

                    }
                    
                }
                catch (Exception error)
                {
                    logs.AppendText("The process failed: " +  error.ToString());
                    if (!terminating)
                    {
                        logs.AppendText("A client has disconnected\n");
                    }
                    thisClient.Close();
                    clientSockets.Remove(thisClient);
                    clientNames.Remove(clientNames[ioc]);
                    connected = false;
                }
            }
        }

        private void button_file_Click(object sender, EventArgs e)
        {
            
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            //dialog.Filter = "Text files | *.txt"; // file types, that will be allowed to upload
            //dialog.Multiselect = false; // allow/deny user to upload more than one file at a time
            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                predeterminedPath = dialog.SelectedPath; // get name of file
                logs.AppendText("Database folder set to: " + predeterminedPath + "\n");
            }
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            terminating = true;
            Environment.Exit(0);

        }
    }
}
