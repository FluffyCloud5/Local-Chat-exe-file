using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace sendRecieve
{
    internal class Program
    {


        static void Main(string[] args)
        {
            RunMessager();
        }


        static void RunMessager()
        {
            

            // Declearing Threads
            Thread listener = new Thread(() =>
            {
                UDPListener.StartListener();
            });

            Thread PrintQueue = new Thread(() =>
            {
                UDPListener.PrintQueue();
            });
            Thread showStats = new Thread(() =>
            {
                UDPListener.ShowStats();
            });



            // GETTING INFO ABOUT NEW USER
            bool gotNickname = false;
            
            while (!gotNickname)
            {
                Console.Write("Nickname: ");


                nickname = Console.ReadLine();
                if (nickname != "")
                    gotNickname = true;
                else
                {
                    deletePage();
                }

            }

            deletePage();

            //STARTING THREADS
            listener.Start();
            PrintQueue.Start();
            showStats.Start();

            listener.IsBackground = true;
            PrintQueue.IsBackground = true;
            showStats.IsBackground = true;


            //GIVING THREADS TIME TO GET READY
            Thread.Sleep(1000);

            if (!broadcastAll)
            {
                UDPSend.send("/joinrequest", UDPSend.broadcastIPV4);
            }

            Console.WriteLine("People in chat: -1");

            Console.WriteLine("Messages in queue: 0");


            
            //Waiting for inputs
            while (!stop)
            {
                Thread.Sleep(50);

                string message = Console.ReadLine();



                if (!commands(message))
                {
                    if (broadcastAll)
                    {
                        deleteLine(Console.CursorTop - 1);
                        UDPSend.send($"{nickname}: {message}", UDPSend.broadcastIPV4);
                    }
                    else
                    {
                        foreach (string ip in UDPSend.connectedIPs)
                        {
                            deleteLine(Console.CursorTop - 1);
                            UDPSend.send($"{nickname}: {message}", ip);
                        }
                    }     
                }

                else
                    Console.WriteLine("DONE");
            }
        }

        
        public static bool stop = false;

        //User settings
        public static string nickname = "";
        public static bool lockChat = false;
        public static bool broadcastAll = false;


        public static bool commands(string command)
        {
            switch(command)
            {
                case ("/close"):
                    stop = true;
                    return true;
                case ("/rename"):
                    bool gotNickname = false;
                    while (!gotNickname)
                    {
                        Console.Write("Nickname: ");

                        nickname = Console.ReadLine();
                        if (nickname != "")
                            gotNickname = true;
                        else
                        {
                            deleteLine(Console.CursorTop-1);
                        }

                    }
                    return true;
                case ("/lockchat"):
                    lockChat = true;
                    return true;

            }
            return false;



        }

        public static void deleteLine(int row = -1, int lineLength = -1)
        {
            if (lineLength < 0)
                lineLength = Console.WindowWidth;
            if (row < 0)
                row = Console.CursorTop;

            Console.SetCursorPosition(0, row);

            for (int i = 0; i < lineLength; i++)
            {

                Console.Write(" ");
            }
            Console.SetCursorPosition(0, row);
        }

        public static void deletePage()
        {
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            for (int i = 0; i < windowHeight; i++)
            {
                deleteLine(i);
            }
            Console.SetCursorPosition(0, 0);
            
        }
    }
}