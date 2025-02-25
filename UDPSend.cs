using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;


namespace sendRecieve
{
    public class UDPSend
    {

        public static void send(string message, string ipv4Address)
        {
            int port = 11000;

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);



            IPAddress destination = IPAddress.Parse(ipv4Address); 

            //school broadcast: 10.174.63.255


            byte[] sendbuf = Encoding.ASCII.GetBytes(message);
            IPEndPoint ep = new IPEndPoint(destination, port);


            s.SendTo(sendbuf, ep);
        }

        
        //all to do with IPs and getting them
        public static string broadcastIPV4 = GetBroadcastAddress();

        public static List<string> connectedIPs = new List<string>();

        public static string GetLocalIPV4()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            if (ipAddress == null)
                throw new Exception("No IPv4 address found for this machine.");

            return ipAddress.ToString();

        }

        public static string GetLocalIPV6()
        {
            string strHostName = System.Net.Dns.GetHostName(); ;
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            if (addr[0].AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return addr[0].ToString(); //ipv6
            }
            else
                throw new Exception("No IPV6");
        }

        public static string GetSubnetMask()
        {
            IPAddress address = IPAddress.Parse(UDPSend.GetLocalIPV4());
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask.ToString();
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        public static string GetBroadcastAddress()
        {
            IPAddress address = IPAddress.Parse(GetLocalIPV4());
            IPAddress subnetMask = IPAddress.Parse(GetSubnetMask());
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress).ToString();
        }

        

    }
}