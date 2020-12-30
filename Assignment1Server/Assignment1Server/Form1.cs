using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Assignment1Server
{
    public partial class Form1 : Form
    {
        String predeterminedPath = @"C:\Users\ASUS\Desktop\foldertowrite";
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientSockets = new List<Socket>();
        List<string> clientNames = new List<string>();

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

        private string parseFilename(string filename)
        {
            int indexOfFileCount = -1;
            int filenameLength = filename.Length;
            for (int i = filenameLength - 1; i >= 0; i--)
            {
                // if the char of the filename can be an integer, it reflects count
                if (Int32.TryParse(filename[i].ToString(), out int temp))
                {
                    indexOfFileCount = filenameLength - i;
                }
                else { break; }
            }
            // If there exists any file count in the filename, extract this part from the filename.
            if (indexOfFileCount != -1)
            {
                filename = filename.Substring(0, filename.Length - indexOfFileCount);
            }
            return filename;
        }

        private int filenameExists(string client_name, string filename, out bool fullFilenameExists)
        {

            // fullFilenameExists = true indicates that given file has <client_name>-<filenameWithFileCount> structure.
            fullFilenameExists = false;
            string line;
            int count = -1;
            StreamReader file = new StreamReader(fullPathDb);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    string fullFileName = words[3];
                    if (fullFileName.EndsWith(".txt"))
                    {
                        fullFileName = fullFileName.Substring(0, fullFileName.Length - 4);
                    }
                    bool recordEqualsFileInput = fullFileName == filename;
                    // The previously found fullFilename returns count as 01. (After finding first one, it never goes inner loop which increments count)
                    // Therefore, I check that if the current line's filename == input file name by taking substring of it
                    if (fullFilenameExists && filename.Length <= fullFileName.Length)
                    {
                        bool temp = parseFilename(filename) == words[0] + "-" + words[1];
                        if (fullFileName.Substring(0, filename.Length) == filename
                            || parseFilename(filename) == words[0] + "-" + words[1])
                        {
                            recordEqualsFileInput = true;
                        }
                    }
                    if (words[0] == client_name && (words[1] == filename || recordEqualsFileInput))
                    {
                        if (fullFileName == filename)
                        {
                            fullFilenameExists = true;
                        }
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

        private string getFileStatus(string filename)
        {
            string line = "";
            StreamReader file = new StreamReader(fullPathDb);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    if (words[1] == filename && words[6] == "public")
                    {
                        return "public";
                    }
                }
                return "private";
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

        // getFieldFromDB returns a field of the specified file for given clientName
        private string getFieldFromDB(int fieldIndex, string filename, string clientName = "")
        {
            string line = "";
            StreamReader file = new StreamReader(fullPathDb);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    if (clientName.Length > 0)
                    {
                        if (words[1] == filename && words[0] == clientName)
                        {
                            return words[fieldIndex];
                        }
                    }
                    else
                    {
                        if (words[1] == filename)
                        {
                            return words[fieldIndex];
                        }
                    }
                }
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

        // createFileCopy creates a copy of the given file.
        private bool createFileCopy(string filename, string clientName, out bool exist)
        {
            try
            {
                // now, filename does not contain any file extension
                if (filename.EndsWith(".txt"))
                {
                    filename = filename.Substring(0, filename.Length - 4);
                }

                int fileCountNumber = filenameExists(clientName, filename, out bool fullFilenameInput);
                if (fileCountNumber == -1)
                {
                    logs.AppendText(filename + " couldn't found for the user " + clientName + "\n");
                    exist = false;
                    return false;
                }

                // if fullFilenameInput is true, we have <client_name>-<filenameCount> file input.
                // therefore, we need to parse it to obtain real file name.
                if (fullFilenameInput)
                {
                    // - Until the dash, we have client name. We are going to extract client name from the input 
                    // - indexOfFileCount indicates the starting index of the file count within the filename input given as a parameter.
                    int indexOfDash = filename.IndexOf('-');

                    // Now we obtained a filename without client name.
                    filename = filename.Substring(indexOfDash + 1, filename.Length - 1 - indexOfDash);

                    // However, we may have a file count in the end of the filename.
                    // Iterate through the filename to find the first occurrences of the count.
                    // Increment the value of the indexOfFileCount based on iteration
                    filename = parseFilename(filename);
                }

                // filename_to_write will be used as a filename for the copy of the actual file.
                string filename_to_write = "";
                if (fileCountNumber < 10)
                {
                    filename_to_write = clientName + "-" + filename + "0" + fileCountNumber.ToString() + ".txt";
                }
                else
                {
                    filename_to_write = clientName + "-" + filename + fileCountNumber.ToString() + ".txt";
                }

                string sourceFilename = getFieldFromDB(3, filename, clientName);
                if(sourceFilename == "")
                {
                    logs.AppendText("Couldn't get any file from DB to create copy.\n");
                    exist = true;
                    return false;
                }
                string sourceFileDest = Path.Combine(predeterminedPath, sourceFilename);
                string copiedFileDest = Path.Combine(predeterminedPath, filename_to_write);

                File.Copy(sourceFileDest, copiedFileDest, true);

                // In order to create a proper record, we need file size and the status(public or priv.) of the file
                string fileSize = getFieldFromDB(5, filename, clientName);
                string status = getFileStatus(filename);

                // create a record in the db for the copied file
                using (StreamWriter sw = File.AppendText(fullPathDb))
                {
                    string to_send = "",
                        currDate = DateTime.Now.ToString("dd/MM/yyyy.HH:mm:ss");

                    if (fileSize == "")
                    {
                        logs.AppendText("File size for the " + filename + " couldn't found in the DB.\n");
                    }

                    if (fileCountNumber != -1)
                    {
                        to_send = clientName + " " + filename + " " + fileCountNumber
                            + " " + filename_to_write + " " + currDate + " " + fileSize
                            + " " + status;
                    }
                    sw.WriteLine(to_send); // Write text to .txt file
                }

                exist = true;
                return true;
            }
            catch (Exception err)
            {
                logs.AppendText("Error in creating file copy " + err.ToString() + "\n");
                exist = true;
                return false;
            }
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
                        if (fileName.Length > 0)
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

        private bool deleteFile(string filename, string clientName, out bool exist)
        {
            try
            {
                // now, filename does not contain any file extension
                if (filename.EndsWith(".txt"))
                {
                    filename = filename.Substring(0, filename.Length - 4);
                }

                int fileCountNumber = filenameExists(clientName, filename, out bool fullFilenameInput);
                if (fileCountNumber == -1)
                {
                    logs.AppendText(filename + " couldn't found for the user " + clientName + "\n");
                    exist = false;
                    return false;
                }

                string parsedFilename = "";
                // if fullFilenameInput is true, we have <client_name>-<filenameCount> file input.
                // therefore, we need to parse it to obtain real file name.
                if (fullFilenameInput)
                {
                    // - Until the dash, we have client name. We are going to extract client name from the input 
                    // - indexOfFileCount indicates the starting index of the file count within the filename input given as a parameter.
                    int indexOfDash = filename.IndexOf('-');

                    // Now we obtained a filename without client name.
                    parsedFilename = filename.Substring(indexOfDash + 1, filename.Length - 1 - indexOfDash);

                    // However, we may have a file count in the end of the filename.
                    // Iterate through the filename to find the first occurrences of the count.
                    // Increment the value of the indexOfFileCount based on iteration
                    parsedFilename = parseFilename(parsedFilename);
                }

                string tempFilename = "";
                if(fullFilenameInput)
                {
                    tempFilename = filename + ".txt";
                }
                else
                {
                    tempFilename = getFieldFromDB(3, filename, clientName);
                }

                string filePath = Path.Combine(predeterminedPath, tempFilename);
                if (fileCountNumber == -1 || !File.Exists(filePath))
                {
                    exist = false;
                    return false;
                }

                List<string> fileLinesList = File.ReadAllLines(fullPathDb).ToList();
                string line = "";
                int counter = 0;
                using (StreamReader reader = new StreamReader(fullPathDb))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] words = line.Split(' ');
                        if ((fullFilenameInput && words[3] == filename + ".txt")
                            || (!fullFilenameInput && words[1] == filename)
                            && (words[0] == clientName))
                        {
                            break;
                        }
                        counter++;
                    }
                }
                fileLinesList.RemoveAt(counter);
                File.WriteAllLines(fullPathDb, fileLinesList);

                File.Delete(filePath);
                exist = true;
                return true;
            }
            catch (Exception err)
            {
                logs.AppendText("Error in deleting file " + err.ToString() + "\n");
                exist = true;
                return false;
            }
        }

        private bool downloadFile(string filename, string clientName, out bool exist, Socket thisSocket)
        {
            try
            {
                bool temp = false;
                int fileCountNumber = filenameExists(clientName, filename, out temp);

                string fileVisibility = getFileStatus(filename);
                if (fileVisibility == "private")
                {
                    if (fileCountNumber == -1)
                    {
                        exist = false;
                        return false;
                    }
                }
                else
                {
                    clientName = getFieldFromDB(0, filename);
                    if (clientName == "")
                    {
                        exist = false;
                        return false;
                    }
                }

                string tmpFilename = clientName + "-" + filename + ".txt";
                string tmpFilePath = Path.Combine(predeterminedPath, tmpFilename);

                using (StreamReader reader = new StreamReader(new FileStream(tmpFilePath, FileMode.Open), new UTF8Encoding()))
                {
                    string message = "!df!" + reader.ReadToEnd() + "!end!";
                    int length_message = message.Length;
                    int bufferSize = 1000000;
                    int howmany = (length_message / bufferSize) + 1;
                    logs.AppendText("how many is " + howmany + "\n");
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
                        thisSocket.Send(buffer);
                    }
                    logs.AppendText("You sent a file called " + filename + ".txt\n");
                }


                exist = true;
                return true;
            }
            catch (Exception err)
            {
                //thisSocket.Send(Encoding.Default.GetBytes("!err!" + filename + " is not found"));
                logs.AppendText("Error in deleting file " + err.ToString() + "\n");
                exist = true;
                return false;
            }
        }

        private bool changeFileVisibility(string filename, string clientName, out bool exist, string status)
        {
            try
            {
                bool temp = false;
                int fileCountNumber = filenameExists(clientName, filename, out temp);
                string tempFilename = "";

                if (fileCountNumber == -1)
                {
                    exist = false;
                    return false;
                }
                if (fileCountNumber - 1 == 0)
                {
                    tempFilename = clientName + "-" + filename + ".txt";
                }
                else if (fileCountNumber - 1 < 10)
                {
                    tempFilename = clientName + "-" + filename + "0" + (fileCountNumber - 1).ToString() + ".txt";
                }
                else if (fileCountNumber - 1 >= 10)
                {
                    tempFilename = clientName + "-" + filename + (fileCountNumber - 1).ToString() + ".txt";
                }

                string filePath = Path.Combine(predeterminedPath, tempFilename);
                if (fileCountNumber == -1 || !File.Exists(filePath))
                {
                    exist = false;
                    return false;
                }

                string line = "";
                List<string> fileLinesList = File.ReadAllLines(fullPathDb).ToList();
                int counter = 0;
                using (StreamReader reader = new StreamReader(fullPathDb))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] words = line.Split(' ');
                        if (words[1] == filename && words[0] == clientName)
                        {
                            words[6] = status;
                            string newRecord = String.Join(" ", words[0], words[1], words[2], words[3],
                                                            words[4], words[5], words[6]);
                            fileLinesList[counter] = newRecord;
                        }
                        counter++;
                    }
                }
                File.WriteAllLines(fullPathDb, fileLinesList);
                exist = true;
                return true;
            }
            catch (Exception err)
            {
                logs.AppendText("Error in deleting file " + err.ToString() + "\n");
                exist = true;
                return false;
            }
        }

        private string getPublicFileList(string filename)
        {
            string line, resultList = "!pll!";
            StreamReader file = new StreamReader(fullPathDb);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    if (words[6] == "public")
                    {
                        resultList += words[0] + ", " + words[3] + ", "
                            + words[5] + ", " + words[4] + "~";
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

        private void Receive(Socket thisClient)
        {
            bool connected = true;
            bool filenameExtracted = false;
            bool firstMessage = false;

            int ioc = 0;
            while (connected && !terminating)
            {
                // This will be the implementation of file acceptance and writing to the system
                try
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
                        // clear the empty bytes of incomingMessage
                        if (incomingMessage.EndsWith("\0"))
                        {
                            incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                        }

                        ioc = clientSockets.IndexOf(thisClient);
                        // List the files of the user
                        if (incomingMessage.StartsWith("!ll!"))
                        {
                            string clientName = clientNames[ioc];
                            string fileList = getFileList(clientName);
                            thisClient.Send(Encoding.Default.GetBytes(fileList));
                        }
                        // create copy of the file
                        else if (incomingMessage.StartsWith("!cc!"))
                        {
                            string clientName = clientNames[ioc];
                            string filename = incomingMessage.Substring(4, incomingMessage.Length - 4);

                            bool doesCopied = createFileCopy(filename, clientName, out bool fileExistsForUser);
                            string responseMessage = "!cc!";

                            // if file ends with .txt, adding .txt to the end of the filename
                            // causes duplicate .txt extension error.
                            if (filename.EndsWith(".txt"))
                            {
                                filename = filename.Substring(0, filename.Length - 4);
                            }

                            if (!fileExistsForUser)
                            {
                                responseMessage += filename + ".txt is not found for " + clientName;
                                logs.AppendText(responseMessage.Substring(4, responseMessage.Length - 4) + "\n");
                            }
                            else if (doesCopied)
                            {
                                responseMessage += filename + ".txt is copied for user " + clientName;
                                logs.AppendText(responseMessage.Substring(4, responseMessage.Length - 4) + "\n");
                            }
                            else
                            {
                                responseMessage += filename + ".txt couldn't copied for user " + clientName;
                                logs.AppendText(responseMessage.Substring(4, responseMessage.Length - 4) + "\n");
                            }
                            thisClient.Send(Encoding.Default.GetBytes(responseMessage));
                        }
                        // delete a file
                        else if (incomingMessage.StartsWith("!del!"))
                        {
                            string clientName = clientNames[ioc],
                                responseMessage = "!del!",
                                filename = incomingMessage.Substring(5, incomingMessage.Length - 5);

                            // now, filename does not include any extension
                            if (filename.EndsWith(".txt"))
                            {
                                filename = filename.Substring(0, filename.Length - 4);
                            }

                            bool doesDeleted = deleteFile(filename, clientName, out bool fileExist);
                            if (!fileExist)
                            {
                                responseMessage += filename + ".txt is not found for " + clientName;
                                logs.AppendText(responseMessage.Substring(5, responseMessage.Length - 5) + "\n");
                            }
                            else if (doesDeleted)
                            {
                                responseMessage += filename + ".txt is deleted for user " + clientName;
                                logs.AppendText(responseMessage.Substring(5, responseMessage.Length - 5) + "\n");
                            }
                            else
                            {
                                responseMessage += filename + ".txt couldn't deleted for user " + clientName;
                                logs.AppendText(responseMessage.Substring(5, responseMessage.Length - 5) + "\n");
                            }
                            thisClient.Send(Encoding.Default.GetBytes(responseMessage));
                        }
                        else if (incomingMessage.StartsWith("!df!"))
                        {
                            string clientName = clientNames[ioc],
                                    responseMessage = "!df!",
                                    filename = incomingMessage.Substring(4, incomingMessage.Length - 4);

                            // now, filename does not include any extension
                            if (filename.EndsWith(".txt"))
                            {
                                filename = filename.Substring(0, filename.Length - 4);
                            }

                            bool fileExist = true;
                            bool isDownloaded = downloadFile(filename, clientName, out fileExist, thisClient);
                            if (!fileExist)
                            {
                                responseMessage = "!err!" + filename + ".txt is not found for " + clientName;
                                logs.AppendText(responseMessage + "\n");
                                thisClient.Send(Encoding.Default.GetBytes(responseMessage));
                            }
                            else if (isDownloaded)
                            {
                                responseMessage += filename + ".txt is downloaded for user " + clientName;
                                logs.AppendText(responseMessage + "\n");
                            }
                            else
                            {
                                responseMessage = "!err!" + filename + ".txt couldn't downloaded for user " + clientName;
                                logs.AppendText(responseMessage + "\n");
                                thisClient.Send(Encoding.Default.GetBytes(responseMessage));
                            }
                        }
                        else if (incomingMessage.StartsWith("!mp!"))
                        {
                            string clientName = clientNames[ioc],
                               responseMessage = "!mp!",
                               filename = incomingMessage.Substring(4, incomingMessage.Length - 4);
                            bool fileExist = true;

                            if (filename.EndsWith(".txt"))
                            {
                                filename = filename.Substring(0, filename.Length - 4);
                            }
                            bool isChanged = changeFileVisibility(filename, clientName, out fileExist, "public");

                            if (!fileExist)
                            {
                                responseMessage += filename + ".txt is not found for " + clientName;
                                logs.AppendText(responseMessage.Substring(4, responseMessage.Length - 4) + "\n");
                            }
                            else if (isChanged)
                            {
                                responseMessage += filename + ".txt becomes public";
                                logs.AppendText(responseMessage.Substring(4, responseMessage.Length - 4) + "\n");
                            }
                            else
                            {
                                responseMessage += filename + ".txt couldn't changed";
                                logs.AppendText(responseMessage.Substring(4, responseMessage.Length - 4) + "\n");
                            }
                            thisClient.Send(Encoding.Default.GetBytes(responseMessage));
                        }
                        else if (incomingMessage.StartsWith("!pll!"))
                        {
                            string clientName = clientNames[ioc];
                            string filename = incomingMessage.Substring(5, incomingMessage.Length - 5);
                            string fileList = getPublicFileList(filename);
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
                            string filename_to_write = "", status = "private";

                            bool temp = false;
                            int fileCountNumber = filenameExists(clientName, filename, out temp);

                            // if file exist before.
                            if (fileCountNumber != -1)
                            {
                                status = getFieldFromDB(6, filename, clientName);
                                if (fileCountNumber < 10)
                                {
                                    filename_to_write = clientNames[ioc] + "-" + filename + "0" + fileCountNumber.ToString() + ".txt";
                                }
                                else
                                {
                                    filename_to_write = clientNames[ioc] + "-" + filename + fileCountNumber.ToString() + ".txt";
                                }
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
                                        + " " + filename_to_write + " " + currDate + " " + fileMessage.Length
                                        + " " + status;
                                }
                                else
                                {
                                    to_send = clientNames[ioc] + " " + filename + " " + "0"
                                        + " " + filename_to_write + " " + currDate + " " + fileMessage.Length
                                        + " private";
                                }
                                sw.WriteLine(to_send); // Write text to .txt file
                            }

                            if (fileMessage.IndexOf("\0") >= 0)
                            {
                                fileMessage.Substring(0, fileMessage.IndexOf("\0"));
                            }

                            using (StreamWriter sw = File.AppendText(predeterminedPath + "\\" + filename_to_write))
                            {
                                fileMessage = fileMessage.Substring(0, fileMessage.Length - 5);
                                sw.WriteLine(fileMessage); // Write text to .txt file
                            }
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
