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
        string downloadFolder = "";


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
                    enableUserInputFields(true);
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

        // enableUserInputFields change the visibility of the input fields.
        private void enableUserInputFields(bool option)
        {
            button_disconnect.Enabled = option;
            button_list_files.Visible = option;
            label_filename.Visible = option;
            textBox_filename.Visible = option;
            button_delete.Visible = option;
            button_create_copy.Visible = option;
            button_download.Visible = option;
            button_make_public.Visible = option;
            button_get_public.Visible = option;
            checkBox_public.Visible = option;
        }

        private void Receive()
        {
            while (connected)
            {
                try
                {
                    int bufferSize = 1000000;
                    Byte[] buffer = new Byte[bufferSize];
                    clientSocket.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    if (incomingMessage.Contains("\0"))
                    {
                        incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));              //The message about the connection to server.
                    }

                    //Yes means, we connected successfully. 
                    if (incomingMessage == "Yes")
                    {
                        logs.AppendText("Connected to the server\n");
                    }

                    else if (incomingMessage == "No")               //No means we do not connected. There exists a name.
                    {
                        button_connect.Enabled = true;
                        button_send.Enabled = false;
                        button_disconnect.Enabled = false;
                        enableUserInputFields(false);
                        connected = false;
                        logs.AppendText("Not connected to the server. There exists a client with same name. Enter another name \n");
                    }
                    // !resp! is used for listing command
                    else if (incomingMessage.StartsWith("!resp!"))
                    {
                        incomingMessage = incomingMessage.Substring(6, incomingMessage.Length - 6);
                        displayFileList(incomingMessage);
                    }
                    // !cc! is used for creating copy 
                    else if (incomingMessage.StartsWith("!cc!"))
                    {
                        incomingMessage = incomingMessage.Substring(4, incomingMessage.Length - 4);
                        logs.AppendText(incomingMessage + "\n");
                    }
                    else if (incomingMessage.StartsWith("!del!"))
                    {
                        incomingMessage = incomingMessage.Substring(5, incomingMessage.Length - 5);
                        logs.AppendText(incomingMessage + "\n");
                    }
                    else if (incomingMessage.StartsWith("!err!"))
                    {
                        incomingMessage = incomingMessage.Substring(5, incomingMessage.Length - 5);
                        logs.AppendText(incomingMessage + "\n");
                    }
                    else if (incomingMessage.StartsWith("!df!"))
                    {
                        string filename = textBox_filename.Text;
                        if (!filename.EndsWith(".txt"))
                        {
                            filename += ".txt";
                        }
                        int counter = 1;
                        incomingMessage = incomingMessage.Substring(4, incomingMessage.Length - 4);
                        int incomingSize = incomingMessage.Length;
                        if (incomingMessage.Length == bufferSize - 4)
                        {
                            while (true)
                            {
                                Byte[] buffer2 = new Byte[bufferSize];
                                clientSocket.Receive(buffer2);
                                string partialdata = Encoding.Default.GetString(buffer2);
                                if (partialdata.IndexOf("\0") >= 0)
                                {
                                    partialdata = partialdata.Substring(0, partialdata.IndexOf("\0"));
                                }
                                logs.AppendText("Counter is " + counter + "\n");
                                counter++;

                                if (partialdata.EndsWith("!end!"))
                                {
                                    partialdata = partialdata.Substring(0, partialdata.Length - 5);
                                    incomingMessage += partialdata;
                                    break;
                                }
                                if (partialdata.Length > 0)
                                {
                                    incomingMessage += partialdata;
                                }
                            }
                        }
                        string downloadedFilePath = Path.Combine(downloadFolder, filename);
                        if (incomingMessage.EndsWith("!end!"))
                        {
                            incomingMessage = incomingMessage.Substring(0, incomingMessage.Length - 5);
                        }
                        logs.AppendText(filename + " is saving.\n");
                        using (StreamWriter sw = File.CreateText(downloadedFilePath))
                        {
                            sw.WriteLine(incomingMessage);
                        }
                        logs.AppendText(filename + " is downloaded into " + downloadFolder + "\n");
                    }
                    else if (incomingMessage.StartsWith("!mp!"))
                    {
                        incomingMessage = incomingMessage.Substring(4, incomingMessage.Length - 4);
                        logs.AppendText(incomingMessage + "\n");
                    }
                    else if (incomingMessage.StartsWith("!pll!"))
                    {
                        incomingMessage = incomingMessage.Substring(5, incomingMessage.Length - 5);
                        string[] publicFilesArr = incomingMessage.Split('~');
                        logs.AppendText("PUBLIC FILES\n");
                        foreach (string fileInfo in publicFilesArr)
                        {
                            string[] eachInfo = fileInfo.Split(' ');
                            if (eachInfo.Length > 0 && eachInfo[0].Length > 0)
                            {
                                logs.AppendText("--------------------\n");
                                logs.AppendText("OWNER:" + eachInfo[0] + "\n" +
                                    "FILENAME:" + eachInfo[1] + " \n" +
                                    "CREATED AT: " + eachInfo[3] + " \n" +
                                    "SIZE:" + eachInfo[2] + "\n");
                                logs.AppendText("--------------------\n");
                            }
                        }
                    }
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("The server has disconnected\n");
                        button_connect.Enabled = true;
                        button_send.Enabled = false;
                        enableUserInputFields(false);
                    }

                    clientSocket.Close();
                    connected = false;
                }
            }
        }

        // displayFileList displays files in the db for the given user. 
        private void displayFileList(string files)
        {
            string[] fileListInfoArray = files.Split('~'), fileRecords;
            int counter = 1;
            logs.AppendText("FILES OF " + name + "\n");
            foreach (string fileInfoLine in fileListInfoArray)
            {
                fileRecords = fileInfoLine.Split(' ');
                if (fileRecords[0].Length > 0)
                {
                    string fileName = fileRecords[0],
                        fileDateStr = fileRecords[1],
                        fileSize = fileRecords[2];

                    logs.AppendText(counter.ToString() + ") " +
                        "FILENAME:" + fileName + " " +
                        "CREATED AT: " + fileDateStr + " " +
                        "SIZE:" + fileSize + "\n");

                    counter++;
                }
            }
            logs.AppendText("---------------------------\n");
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
                    int bufferSize = 1000000;
                    int howmany = (length_message / bufferSize) + 1;

                    int cur_index = 0;
                    for (int i = 0; i < howmany; i++)
                    {
                        Byte[] buffer = new Byte[bufferSize];
                        if (cur_index + bufferSize < length_message)
                        {
                            buffer = Encoding.Default.GetBytes(message.Substring(cur_index, bufferSize));
                        }
                        else
                        {
                            string last_message = message.Substring(cur_index, length_message - cur_index);
                            buffer = Encoding.Default.GetBytes(last_message);
                        }
                        cur_index = cur_index + bufferSize;
                        clientSocket.Send(buffer);
                    }
                    logs.AppendText("You sent a file called " + filename + ".txt\n");
                }
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
            enableUserInputFields(false);
        }

        private void Button_list_files_Click(object sender, EventArgs e)
        {
            string listingCommand = "!ll!";
            try
            {
                Byte[] commandBuffer = new Byte[64];
                commandBuffer = Encoding.Default.GetBytes(listingCommand);
                clientSocket.Send(commandBuffer);
            }
            catch (Exception error)
            {
                Console.WriteLine("The process failed: {0}", error.ToString());
                if (!terminating)
                {
                    logs.AppendText("The server has disconnected\n");
                    button_connect.Enabled = true;
                    button_send.Enabled = false;
                    enableUserInputFields(false);
                }

                clientSocket.Close();
                connected = false;
            }
        }

        private void Button_create_copy_Click(object sender, EventArgs e)
        {
            if (checkBox_public.Checked)
            {
                logs.AppendText("You cannot create copy of the public files.\n");
                return;
            }

            string inputFilename = textBox_filename.Text;
            string createCopyCommand = "!cc!" + inputFilename;
            try
            {
                Byte[] commandBuffer = new Byte[512];
                commandBuffer = Encoding.Default.GetBytes(createCopyCommand);
                clientSocket.Send(commandBuffer);
            }
            catch (Exception error)
            {
                Console.WriteLine("The process failed: {0}", error.ToString());
                if (!terminating)
                {
                    logs.AppendText("The server has disconnected\n");
                    button_connect.Enabled = true;
                    button_send.Enabled = false;
                    enableUserInputFields(false);
                }
                clientSocket.Close();
                connected = false;
            }
        }

        private void Button_delete_Click(object sender, EventArgs e)
        {
            if (checkBox_public.Checked)
            {
                logs.AppendText("You cannot delete public files.\n");
                return;
            }

            string inputFilename = textBox_filename.Text;
            string deleteFileCommand = "!del!" + inputFilename;
            try
            {
                Byte[] commandBuffer = new Byte[512];
                commandBuffer = Encoding.Default.GetBytes(deleteFileCommand);
                clientSocket.Send(commandBuffer);
                logs.AppendText("Deletion is started! \n");
            }
            catch (Exception error)
            {
                Console.WriteLine("The process failed: {0}", error.ToString());
                if (!terminating)
                {
                    logs.AppendText("The server has disconnected\n");
                    button_connect.Enabled = true;
                    button_send.Enabled = false;
                    enableUserInputFields(false);
                }
                clientSocket.Close();
                connected = false;
            }
        }

        private void Button_download_Click(object sender, EventArgs e)
        {

            if (checkBox_public.Checked && textBox_public_owner.Text == "")
            {
                logs.AppendText("If requested file is public, specify the owner name.\n");
                return;
            }

            //using (var fbd = new FolderBrowserDialog())
            //{
            //    DialogResult result = fbd.ShowDialog();
            //    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            //    {
            //        string[] files = Directory.GetFiles(fbd.SelectedPath);
            //        logs.AppendText("Selected path: " + fbd.SelectedPath + "\n");
            //        downloadFolder = fbd.SelectedPath;
            //    }
            //    else
            //    {
            //        downloadFolder = "";
            //    }
            //}

            downloadFolder = @"C:\Users\ASUS\Desktop\downloaded";

            if (downloadFolder == "" || downloadFolder.Length == 0)
            {
                logs.AppendText("You need to specify valid path!\n");
                return;
            }

            string owner = textBox_public_owner.Text;
            try
            {
                string inputFilename = textBox_filename.Text, downloadFileCommand;
                if (owner != "" || owner.Trim() != "")
                {
                    downloadFileCommand = "!df!~" + owner + "`" + inputFilename;
                }
                else
                {
                    downloadFileCommand = "!df!" + inputFilename;
                }

                Byte[] commandBuffer = new Byte[512];
                commandBuffer = Encoding.Default.GetBytes(downloadFileCommand);
                clientSocket.Send(commandBuffer);
                logs.AppendText("Download started ...\n");
            }
            catch (Exception error)
            {
                Console.WriteLine("The process failed: {0}", error.ToString());
                if (!terminating)
                {
                    logs.AppendText("The server has disconnected\n");
                    button_connect.Enabled = true;
                    button_send.Enabled = false;
                    enableUserInputFields(false);
                }
                clientSocket.Close();
                connected = false;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (checkBox_public.Checked)
            {
                logs.AppendText("You cannot change the visibility of public files.\n");
                return;
            }
            string inputFilename = textBox_filename.Text;
            string makePublicCommand = "!mp!" + inputFilename;
            try
            {

                Byte[] commandBuffer = new Byte[512];
                commandBuffer = Encoding.Default.GetBytes(makePublicCommand);
                clientSocket.Send(commandBuffer);
            }
            catch (Exception error)
            {
                Console.WriteLine("The process failed: {0}", error.ToString());
                if (!terminating)
                {
                    logs.AppendText("The server has disconnected\n");
                    button_connect.Enabled = true;
                    button_send.Enabled = false;
                    enableUserInputFields(false);
                }
                clientSocket.Close();
                connected = false;
            }

        }

        private void Button_get_public_Click(object sender, EventArgs e)
        {
            string filename = textBox_filename.Text;
            string listingCommand = "!pll!" + filename;
            try
            {
                Byte[] commandBuffer = new Byte[64];
                commandBuffer = Encoding.Default.GetBytes(listingCommand);
                clientSocket.Send(commandBuffer);
            }
            catch (Exception error)
            {
                Console.WriteLine("The process failed: {0}", error.ToString());
                if (!terminating)
                {
                    logs.AppendText("The server has disconnected\n");
                    button_connect.Enabled = true;
                    button_send.Enabled = false;
                    enableUserInputFields(false);
                }
                clientSocket.Close();
                connected = false;
            }
        }

        private void CheckBox_public_CheckedChanged(object sender, EventArgs e)
        {
            label_public_owner.Visible = !label_public_owner.Visible;
            textBox_public_owner.Visible = !textBox_public_owner.Visible;
            if (!textBox_public_owner.Visible)
            {
                textBox_public_owner.Text = "";
            }
        }
    }
}
