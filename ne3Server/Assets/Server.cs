using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lec04
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;

public class Server : MonoBehaviour
{
    private static Socket server;
    byte[] buffer = new byte[512];
    private static EndPoint remoteClient;
    public static void StartServer()
    {
        

        IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());

        IPAddress ip = hostInfo.AddressList[1];

        //Console.WriteLine("Server name: {0}  IP: {1}", hostInfo.HostName, ip);
        Debug.Log("Server name: " + hostInfo.HostName + "IP: " + ip);

        IPEndPoint localEP = new IPEndPoint(ip, 8888);

        server = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        remoteClient = new IPEndPoint(IPAddress.Any, 0);

        server.ReceiveTimeout = 1000;
        server.SendTimeout = 1000;

        try
        {
            server.Bind(localEP);

            //Console.WriteLine("Waiting for data....");
            Debug.Log("Waiting for data....");

            //while (true)
            //{
            //    int recv = server.ReceiveFrom(buffer, ref remoteClient);
            //    // server.SendTo()

                

            //    //Console.WriteLine("Recv from: {0}   Data: {1}", remoteClient.ToString(), Encoding.ASCII.GetString(buffer, 0, recv));
            //    Debug.Log("Recv from: " + remoteClient.ToString() + " Data: " + Encoding.ASCII.GetString(buffer, 0, recv));

                
            //}

            //server shutdown
            //server.Shutdown(SocketShutdown.Both);
            //server.Close();

        }
        catch (Exception e)
        {
            //Console.WriteLine(e.ToString());
            Debug.Log(e.ToString());
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        StartServer();
    }

    // Update is called once per frame
    void Update()
    {
        int recv = server.ReceiveFrom(buffer, ref remoteClient);
        // server.SendTo()



        //Console.WriteLine("Recv from: {0}   Data: {1}", remoteClient.ToString(), Encoding.ASCII.GetString(buffer, 0, recv));
        //Debug.Log("Recv from: " + remoteClient.ToString() + " Data: " + Encoding.ASCII.GetString(buffer, 0, recv));

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Recv from: " + remoteClient.ToString() + " Data: " + Encoding.ASCII.GetString(buffer, 0, recv));
            UpdateCube(Encoding.ASCII.GetString(buffer, 0, recv));
        }
        UpdateCube(Encoding.ASCII.GetString(buffer, 0, recv));
    }

    public void UpdateCube(string _text)
    {
        StringBuilder sb = new StringBuilder();
        float[] posValues = new float[3];
        int i = 0;

        foreach(char c in _text)
        {
            if(char.IsNumber(c) || c == '.' || c == '-')
            {
                sb.Append(c);
            }
            else
            {
                if(sb.Length > 0)
                {
                    //Debug.Log(sb.ToString()); //paste the coordinate value
                    posValues[i] = Single.Parse(sb.ToString());
                    i++;
                }
                sb.Clear(); //clear the string builder for the next coordinate value
            }
        }
        GameObject myCube = GameObject.Find("Cube");
        myCube.transform.position = new Vector3(posValues[0], posValues[1], posValues[2]);
        Debug.Log("Server Cube Position: " + myCube.transform.position);
    }
}
