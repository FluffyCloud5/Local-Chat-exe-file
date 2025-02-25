using Microsoft.VisualBasic.FileIO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace sendRecieve
{
    public class UDPListener
    {
        private const int listenPort = 11000;

        public static void StartListener()
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    
                    byte[] bytes = listener.Receive(ref groupEP);


                    string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);


                    if (!RecivedCommands(message, groupEP))
                    {
                        recived.Add(message);
                    }
                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {

                listener.Close();
                Console.WriteLine("Listener Status: OFF");
            }
        }

        public static List<string> recived = new List<string>();

        private static int messagesInQueueStat = 0;

        private static int peopleInChatStat = -1;

        public static void PrintQueue()
        {
            while (true)
            {
                Thread.Sleep(100);
                while(recived.Count > 0 && Console.CursorLeft == 0)
                {
                    Console.WriteLine(recived[0]);

                    recived.RemoveAt(0);
                }
                
            }
        }

        public static void ShowStats()
        {
            while (true)
            {
                Thread.Sleep(50);
                if (recived.Count != messagesInQueueStat || UDPSend.connectedIPs.Count - 1 != peopleInChatStat) 
                {
                    messagesInQueueStat = recived.Count;

                    peopleInChatStat = UDPSend.connectedIPs.Count - 1;

                    int row = Console.CursorTop;
                    int column = Console.CursorLeft;

                    Console.SetCursorPosition(0, 1);

                    Console.Write("Messages in Queue: " + messagesInQueueStat + "      ");

                    Console.SetCursorPosition(0, 0);

                    Console.Write("People in chat: " + peopleInChatStat + "      ");

                    Console.SetCursorPosition(column, row);
                }
            }
            
        }



        public static bool RecivedCommands(string message, IPEndPoint groupEP)
        {
            switch(message)
            {
                case ("/joinrequest"):
                    
                    if(!Program.lockChat)
                    {
                        string ip = groupEP.ToString().Remove(groupEP.ToString().IndexOf(':'));
                        UDPSend.connectedIPs.Add(ip);
                        UDPSend.send("/joinaccepted", ip);
                    }
                    return true;
                case ("/joinaccepted"):
                    if (!Program.lockChat)
                    {

                        string ip = groupEP.ToString().Remove(groupEP.ToString().IndexOf(':'));
                        if (ip != UDPSend.GetLocalIPV4())
                            UDPSend.connectedIPs.Add(ip);
                    }
                    return true;

            }


            return false;
        }
    }
}
