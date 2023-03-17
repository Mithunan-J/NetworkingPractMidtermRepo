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

public class NE4_client : MonoBehaviour
{
    public GameObject myCube;

    private static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static byte[] buffer = new byte[512];

    private static byte[] bpos;
    private static float[] pos;

    float x, y, z = 0;
    // Start is called before the first frame update
    void Start()
    {
        myCube = GameObject.Find("Cube");

        client.Connect(IPAddress.Parse("127.0.0.1"), 8888);
        Debug.Log("Connected to server!!!");

        client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), client);

        //SyncSend();

        //Console.Read();
    }

    // Update is called once per frame
    void Update()
    {
        bool positionChanged = false;
        if(x != myCube.transform.position.x)
        {
            positionChanged = true;
            
        }
        else if(y != myCube.transform.position.y)
        {
            positionChanged = true;
        }
        else if(z != myCube.transform.position.z)
        {
            positionChanged = true;
        }

        //x = y = ++z;
        if(positionChanged == true)
        {
            x = myCube.transform.position.x;
            y = myCube.transform.position.y;
            z = myCube.transform.position.z;

            pos = new float[] { x, y, z };
            bpos = new byte[pos.Length * 4];
            Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);

            //byte[] buffer = Encoding.ASCII.GetBytes(c.ToString()); //change this to send as an array of floats for the exercise
            //client.Send(buffer);

            client.Send(bpos);
            //Thread.Sleep(1000);
        }
        

        
        positionChanged = false;
    }

    private static void ReceiveCallback(IAsyncResult results)
    {
        Socket socket = (Socket)results.AsyncState;
        int rec = socket.EndReceive(results);

        pos = new float[rec]; //rec == data.length
        Buffer.BlockCopy(buffer, 0, pos, 0, rec);

        socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), socket);
        Debug.Log("Receive X:" + pos[0] + " Y: " + pos[1] + " Z: " + pos[2]);
    }

    private static void SyncSend() //MOVE TO UPDATE LOOP
    {
        //int c = 0;
        float x, y, z = 0;

        while (true)
        {
            x = y = ++z;
            pos = new float[] { x, y, z };
            bpos = new byte[pos.Length * 4];
            Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);

            //byte[] buffer = Encoding.ASCII.GetBytes(c.ToString()); //change this to send as an array of floats for the exercise
            //client.Send(buffer);

            client.Send(bpos);

            Thread.Sleep(1000);
            //c += 2;
        }
    }
}
