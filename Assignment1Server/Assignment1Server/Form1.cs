﻿using System;
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
        String predeterminedPath = @"C:\Users\ASUS\Desktop\foldertowrite";

        List<String> names = new List<String>();
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientSockets = new List<Socket>();
        List<string> clientNames = new List<string>();
        List<int> clientFileCounts = new List<int>();

        bool terminating = false;
        bool listening = false;
        static string cwd = Directory.GetCurrentDirectory();
        static string fullPathDb = Path.Combine(cwd, "information.txt");
        int currentUserIdx = 0;
        Dictionary<Socket, int> clientIndexes = new Dictionary<Socket, int>();



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
            for (int i = 0; i < clientNames.Count; i++)
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
                    //clientSockets.Add(newClient);
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
            return message.Substring(5, index - 5);
        }

        private string getFileList(string userName)
        {
            string line, resultList = "!resp!";
            StreamReader file = new StreamReader(fullPathDb);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    if (words[0] == userName)
                    {
                        string fileName = words[1];
                        if (!resultList.Contains(fileName) && fileName.Length > 0)
                        {
                            resultList += fileName + " " + words[4] + " " + words[5] + "~";
                        }
                    }
                }
                return resultList;
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
            return "";
        }

        private bool checkFileEnd(string message)
        {
            if (message.EndsWith("!end!"))
            {
                return true;
            }
            int messageLenght = message.Length;
            if ((messageLenght == 1 && message == "!")
                || (messageLenght == 2 && message == "d!")
                || (messageLenght == 3 && message == "nd!")
                || (messageLenght == 4 && message == "end!"))
            {
                return true;
            }
            return false;
        }

        private void Receive(Socket thisClient)
        {
            bool connected = true;
            bool filenameExtracted = false;
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

                        if (NameExists(newclientName))
                        {
                            logs.AppendText("The client with " + newclientName + " exists, socket not connected\n");

                            connected = false;
                            Byte[] acceptanceBuffer = new Byte[64];
                            acceptanceBuffer = Encoding.Default.GetBytes("No");
                            thisClient.Send(acceptanceBuffer);
                        }
                        else
                        {
                            logs.AppendText("Client with name " + newclientName + " is joined to server \n");

                            clientIndexes[thisClient] = currentUserIdx;
                            currentUserIdx++;

                            // add new user
                            clientNames.Add(newclientName);
                            clientSockets.Add(thisClient);
                            firstMessage = true;

                            Byte[] acceptanceBuffer = new Byte[64];
                            acceptanceBuffer = Encoding.Default.GetBytes("Yes");
                            thisClient.Send(acceptanceBuffer);
                        }

                    }
                    else
                    {
                        int counter = 0;
                        int bufferSize = 1000000;

                        Byte[] buffer = new Byte[bufferSize];
                        thisClient.Receive(buffer);
                        filenameExtracted = false;

                        string incomingMessage = Encoding.Default.GetString(buffer);

                        ioc = clientSockets.IndexOf(thisClient);
                        // List the files of the user
                        if (incomingMessage.StartsWith("!ll!"))
                        {
                            string clientName = clientNames[ioc];
                            string fileList = getFileList(clientName);
                            thisClient.Send(Encoding.Default.GetBytes(fileList));
                        }
                        else if (!incomingMessage.StartsWith("\0"))
                        {

                            string fileMessage = "";
                            string filename = "";

                            if (!filenameExtracted)
                            {
                                int index = incomingMessage.IndexOf('.');
                                filename = getFileNameFromString(incomingMessage, index);
                                incomingMessage = incomingMessage.Substring(index + 1);
                                if (incomingMessage.EndsWith("\0"))
                                {
                                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                                }
                                fileMessage += incomingMessage;
                                filenameExtracted = true;
                            }

                            if (incomingMessage.EndsWith("\0"))
                            {
                                incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                            }
                            bool doesEndWith = incomingMessage.EndsWith("!end!");
                            while (!doesEndWith)
                            {
                                Byte[] buffer2 = new Byte[bufferSize];
                                thisClient.Receive(buffer2);

                                counter++;
                                string partialdata = Encoding.Default.GetString(buffer2);
                                if (partialdata.IndexOf("\0") >= 0)
                                {
                                    partialdata = partialdata.Substring(0, partialdata.IndexOf("\0"));
                                }
                                logs.AppendText("Counter is " + counter + "\n");
                                if (partialdata.Length > 0)
                                {
                                    fileMessage += partialdata;
                                }

                                doesEndWith = checkFileEnd(partialdata);
                                if (doesEndWith)
                                {
                                    logs.AppendText("partial data ends with !end!  with counter " + counter + "\n");
                                    break;
                                }
                            }

                            ioc = clientSockets.IndexOf(thisClient);
                            string clientName = clientNames[ioc];
                            string filename_to_write = "";

                            int fileCountNumber = filenameExists(clientName, filename);

                            // if file exist before.
                            if (fileCountNumber != -1)
                            {
                                filename_to_write = clientNames[ioc] + "-" + filename + fileCountNumber.ToString() + ".txt";
                            }
                            else
                            {
                                filename_to_write = clientNames[ioc] + "-" + filename + ".txt";
                            }

                            // create a record in the db for the incoming file
                            using (StreamWriter sw = File.AppendText(fullPathDb))
                            {
                                string to_send = "",
                                    currDate = DateTime.Now.ToString("dd/MM/yyyy.HH:mm:ss");

                                if (fileCountNumber != -1)
                                {
                                    to_send = clientNames[ioc] + " " + filename + " " + fileCountNumber
                                        + " " + filename_to_write + " " + currDate + " " + fileMessage.Length;
                                }
                                else
                                {
                                    to_send = clientNames[ioc] + " " + filename + " " + "0"
                                        + " " + filename_to_write + " " + currDate + " " + fileMessage.Length;
                                }
                                sw.WriteLine(to_send); // Write text to .txt file
                            }
                            logs.AppendText("I am here 4\n");

                            if (fileMessage.IndexOf("\0") >= 0)
                            {
                                fileMessage.Substring(0, fileMessage.IndexOf("\0"));
                            }

                            using (StreamWriter sw = File.AppendText(predeterminedPath + "\\" + filename_to_write))
                            {
                                fileMessage = fileMessage.Substring(0, fileMessage.Length - 5);
                                sw.WriteLine(fileMessage); // Write text to .txt file
                            }
                            logs.AppendText("Client " + clientNames[ioc] + " send a file called " + filename_to_write + "\n");
                        }
                    }

                }
                catch (Exception error)
                {
                    logs.AppendText("Errro: " + error.ToString() + "\n");
                    if (!terminating)
                    {
                        logs.AppendText("A client has disconnected\n");
                    }
                    int thisClientIdx = clientIndexes[thisClient];

                    clientIndexes.Remove(thisClient);
                    currentUserIdx--;

                    clientSockets.Remove(thisClient);
                    clientNames.Remove(clientNames[thisClientIdx]);

                    connected = false;
                    thisClient.Close();
                }
            }
        }

        private void button_file_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                predeterminedPath = dialog.SelectedPath; // get name of file
                Console.WriteLine(predeterminedPath);
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
