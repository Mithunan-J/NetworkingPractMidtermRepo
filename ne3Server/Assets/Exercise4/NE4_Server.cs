using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NE4_Server : MonoBehaviour
{
    private static byte[] buffer = new byte[512];
    private static Socket server;

    private static byte[] outBuffer = new byte[512];
    private static string outMsg = "";

    private static float[] pos;

    public GameObject serverCube;

    //public GameObject serverCube;

    // Start is called before the first frame update
    void Start()
    {
        serverCube = GameObject.Find("Cube");

        Debug.Log("Starting the server...");

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress ip = IPAddress.Parse("127.0.0.1");

        server.Bind(new IPEndPoint(ip, 8888));

        server.Listen(10);

        server.BeginAccept(new AsyncCallback(AcceptCallback), null);

        //Console.Read();
    }

    // Update is called once per frame
    void Update()
    {
        if(pos != null)
        {
            serverCube.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        }
        
    }

    private static void AcceptCallback(IAsyncResult result)
    {
        Socket client = server.EndAccept(result);
        Debug.Log("Client connected!!!    IP:");

        client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), client);
    }

    private static void ReceiveCallback(IAsyncResult result)
    {
        Socket socket = (Socket)result.AsyncState;
        int rec = socket.EndReceive(result);
        //byte[] data = new byte[rec];
        //Array.Copy(buffer, data, rec);
        pos = new float[rec]; //rec == data.length
        Buffer.BlockCopy(buffer, 0, pos, 0, rec);

        //string msg = Encoding.ASCII.GetString(data);
        //Console.WriteLine("Recv: " + msg);

        //socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), socket);
        socket.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendCallback), socket);
        Debug.Log("Receive X:" + pos[0] + " Y: " + pos[1] + "Z: " + pos[2]);
        //GameObject serverCube = GameObject.Find("Cube");
        //serverCube.transform.position = new Vector3(pos[0], pos[1], pos[2]);

        socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), socket);
    }

    private static void SendCallback(IAsyncResult result)
    {
        Socket socket = (Socket)result.AsyncState;
        socket.EndSend(result);
    }
}
