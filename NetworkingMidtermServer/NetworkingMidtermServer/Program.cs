using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkingMidtermServer
{
    class Program
    {
        private static byte[] buffer = new byte[512];
        private static Socket serverTcp;
        private static Socket server2Tcp;

        private static byte[] outBuffer = new byte[512];
        private static string outMsg = "";

        private static float[] pos;

        private static List<Socket> clientSockets = new List<Socket>();

        //Test Endpoints
        public static EndPoint client1Endpoint;
        public static EndPoint client2Endpoint;
        public static string c1S = "";
        public static string c2S = "";
        public static bool udp1 = false;
        public static bool udp2 = false;

        static void Main(string[] args)
        {
            

            //TCP Code
            Console.WriteLine("Starting the server....");

            serverTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server2Tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ip = IPAddress.Parse("127.0.0.1");

            serverTcp.Bind(new IPEndPoint(ip, 8888));

            serverTcp.Listen(10);

            serverTcp.BeginAccept(new AsyncCallback(AcceptCallback), null);

            server2Tcp.Bind(new IPEndPoint(ip, 8887));

            server2Tcp.Listen(10);

            server2Tcp.BeginAccept(new AsyncCallback(AcceptCallback2), null);

            //server.ReceiveTimeout = 1000;
            //server.SendTimeout = 1000;

            //server2.ReceiveTimeout = 1000;
            //server2.SendTimeout = 1000;
            //TCP Code

            //Console.Read();

            StartServer(); //UDP position updates //this one has a while loop that's why it is at the bottom
        }

        private static void AcceptCallback(IAsyncResult result)
        {
            Socket client = serverTcp.EndAccept(result);

            Console.WriteLine("Client connected!!!   IP:" + client.RemoteEndPoint.ToString());

            clientSockets.Add(client);

            client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), client);
        }

        private static void AcceptCallback2(IAsyncResult result)
        {
            Socket client2 = server2Tcp.EndAccept(result);

            Console.WriteLine("Client connected!!!   IP:" + client2.RemoteEndPoint.ToString());

            clientSockets.Add(client2);

            client2.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), client2);
        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            int rec = socket.EndReceive(result);
            byte[] data = new byte[rec];
            Array.Copy(buffer, data, rec);
            //pos = new float[rec]; //rec == data.length
            //Buffer.BlockCopy(buffer, 0, pos, 0, rec); //receiving data from client

            string msg = Encoding.ASCII.GetString(data);
            int clientId = 0;
            for(int i = 0; i < clientSockets.Count; i++) //finding which client sent the message
            {
                if(socket == clientSockets[i])
                {
                    clientId = i + 1;
                }
            }
            int remoteId;
            if(clientId == 1)
            {
                remoteId = 1;
            }
            else
            {
                remoteId = 0;
            }
            if(msg == "quit") //handling voluntary disconnection
            {
                Console.WriteLine(socket.RemoteEndPoint.ToString() + " has disconnected");
                clientSockets[clientId - 1].Close();
                clientSockets.RemoveAt(clientId - 1);
            }
            else //testing massive else statement //purpose: prevents access to disconnected server and thus avoiding an error.
            {
                //Console.WriteLine(clientId);
                Console.WriteLine("Received message: " + msg + " From: " + socket.RemoteEndPoint.ToString());
                if(clientSockets.Count > 1) //basically only sends the second message if there is a second client.
                {
                    Console.WriteLine("Sent message: " + msg + " To " + clientSockets[remoteId].RemoteEndPoint.ToString());
                }

                string newMsg = "Client " + clientId + ":" + msg;
                byte[] newData = Encoding.ASCII.GetBytes(newMsg);
                foreach (Socket soc in clientSockets)
                {
                    soc.BeginSend(newData, 0, newData.Length, 0, new AsyncCallback(SendCallback), soc);
                }

                //socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), socket); //TEST UNCOMMENT LATER
                //socket.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendCallback), socket);
                //Console.WriteLine("Received X:" + pos[0] + " Y: " + pos[1] + "Z: " + pos[2] + "From: " + socket.RemoteEndPoint.ToString());

                socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), socket);
            }           

        }

        

        private static void SendCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);
        }



        public static void StartServer() //for the udp send position data of cubes  //UDP
        {
            byte[] buffer = new byte[512];
            byte[] buffer2 = new byte[512];

            //IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName()); //mental note: irrelevant

            IPAddress ip = IPAddress.Parse("127.0.0.1");

            Console.WriteLine("Server name: Server  IP: {0}", ip);

            IPEndPoint localEP = new IPEndPoint(ip, 8889);

            Socket server = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            EndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);

            IPEndPoint localEP2 = new IPEndPoint(ip, 8890);
            Socket server2 = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            EndPoint remoteClient2 = new IPEndPoint(IPAddress.Any, 0);


            try
            {
                server.Bind(localEP);
                server2.Bind(localEP2);

                
                Console.WriteLine("Waiting for data....");

                while (true)
                {
                    if(server.Available > 0) //test if statement
                    {
                        int recv = server.ReceiveFrom(buffer, ref remoteClient);
                        float[] pos = new float[recv / 4];
                        Buffer.BlockCopy(buffer, 0, pos, 0, recv);

                        if(udp1 == false)
                        {
                            client1Endpoint = remoteClient; //test statement
                            udp1 = true;
                        }

                        // server.SendTo()
                        //Console.WriteLine("Recv from: {0}   Data: {1}", remoteClient.ToString(), Encoding.ASCII.GetString(buffer, 0, recv));
                        Console.WriteLine("Recv from: " + remoteClient.ToString() + " Data: " + pos[0] + " " + pos[1] + " " + pos[2]);
                        if(udp2 == true)
                        {
                            //Console.WriteLine("Sent: {0}    to   {1}", Encoding.ASCII.GetString(buffer, 0, recv), client2Endpoint.ToString());
                            Console.WriteLine("Sent: " + pos[0] + " " + pos[1] + " " + pos[2] + " to " + client2Endpoint.ToString());
                        }
                        
                        //string pos = Encoding.ASCII.GetString(buffer, 0, recv); //old string stuff
                        //var result = pos.Split(',');
                        //Console.WriteLine("X position: " + result[0] + " Y position: " + result[1] + " Z position: " + result[2]);

                        //foreach (Socket socket in clientSockets)
                        //{
                        //    string newPos = "Client 1: " + pos;
                        //    byte[] outBuffer = Encoding.ASCII.GetBytes(newPos);
                        //    server.SendTo(outBuffer, remoteClient); //send message back to clients //only remote client currently works
                        //}

                        string newPos = "Client 1: " + pos;
                        //byte[] outBuffer = Encoding.ASCII.GetBytes(newPos);
                        byte[] outBuffer = new byte[pos.Length * 4];
                        Buffer.BlockCopy(pos, 0, outBuffer, 0, outBuffer.Length);
                        //server.SendTo(outBuffer, remoteClient); //send message back to the owner.
                        if (udp2 == true)
                        {
                            server.SendTo(outBuffer, client2Endpoint); //send message to other client
                        }
                        
                    }

                    if (server2.Available > 0) //test if statement
                    {
                        int recv2 = server2.ReceiveFrom(buffer2, ref remoteClient2);
                        float[] pos2 = new float[recv2 / 4];
                        Buffer.BlockCopy(buffer2, 0, pos2, 0, recv2);

                        if (udp2 == false)
                        {
                            client2Endpoint = remoteClient2; //test statement
                            udp2 = true;
                        }

                        // server.SendTo()
                        //Console.WriteLine("Recv from: {0}   Data: {1}", remoteClient2.ToString(), Encoding.ASCII.GetString(buffer2, 0, recv2));
                        Console.WriteLine("Recv from: " + remoteClient2.ToString() + " Data: " + pos2[0] + " " + pos2[1] + " " + pos2[2]);
                        if (udp1 == true)
                        {
                            //Console.WriteLine("Sent: {0}    to   {1}", Encoding.ASCII.GetString(buffer, 0, recv2), client1Endpoint.ToString());
                            Console.WriteLine("Sent: " + pos2[0] + " " + pos2[1] + " " + pos2[2] + " to " + client1Endpoint.ToString());
                        }
                        //string pos2 = Encoding.ASCII.GetString(buffer2, 0, recv2);
                        //var result2 = pos2.Split(',');
                        //Console.WriteLine("X position: " + result2[0] + " Y position: " + result2[1] + " Z position: " + result2[2]);


                        //foreach (Socket socket in clientSockets)
                        //{
                        //    string newPos = "Client 2: " + pos2;
                        //    byte[] outBuffer = Encoding.ASCII.GetBytes(newPos);
                        //    server2.SendTo(outBuffer, remoteClient2); //send message back to clients
                        //}

                        string newPos = "Client 2: " + pos2;
                        //byte[] outBuffer = Encoding.ASCII.GetBytes(newPos);
                        byte[] outBuffer = new byte[pos2.Length * 4];
                        Buffer.BlockCopy(pos2, 0, outBuffer, 0, outBuffer.Length);
                        //server2.SendTo(outBuffer, remoteClient2); //send message back to owner.
                        if (udp1 == true)
                        {
                            server2.SendTo(outBuffer, client1Endpoint); //send message to the other client
                        }

                    }

                }

                //server shutdown

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public static void SendMessageToClient(Socket _client, string _msg)
        {
            byte[] data = Encoding.ASCII.GetBytes(_msg);
            _client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), _client);
        }
    }
}
