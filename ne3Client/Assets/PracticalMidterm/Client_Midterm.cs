using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

public class Client_Midterm : MonoBehaviour
{
    public GameObject myCube;
    public GameObject remoteCube;
    public GameObject chatBox;
    public GameObject logInTextField;
    public GameObject loginPanel;

    public bool login = false;

    public static bool textReceived = false;
    public static string textMessage = "";

    private static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static byte[] buffer = new byte[512];

    private static byte[] bpos;
    private static float[] pos;

    float x, y, z = 0;

    public string tcpMsg = "2";


    //UDP Position Updates
    private static byte[] outBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static Socket clientSoc;
    public Vector3 cubePos;

    private static EndPoint remoteServer; //test
    //UDP Position Updates


    // Start is called before the first frame update
    void Start()
    {
        myCube = GameObject.Find("Cube");
        cubePos = myCube.transform.position;

        //UDP Position Updates
        //StartClient();
        //UDP Position Updates

        //TCP Position Updates
        //client.Connect(IPAddress.Parse("127.0.0.1"), 8888);
        //Debug.Log("Connected to server!!!");

        //client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), client);
        //TCP Position Updates

        //SyncSend();

        //Console.Read();
    }

    // Update is called once per frame
    void Update()
    {
        if(login == true) //testing if statement login stuff
        {
            //UDP Position Updates
            string pos = myCube.transform.position.x.ToString() + "," + myCube.transform.position.y.ToString() + "," + myCube.transform.position.z.ToString();
            //outBuffer = Encoding.ASCII.GetBytes(myCube.transform.position.ToString());
            outBuffer = Encoding.ASCII.GetBytes(pos); //TEST MIGHT REMOVE LATER
            if (cubePos != myCube.transform.position)
            {
                clientSoc.SendTo(outBuffer, remoteEP);
                cubePos = myCube.transform.position;
            }

            if (clientSoc.Available > 0)
            {
                int recv = clientSoc.ReceiveFrom(buffer, ref remoteServer);
                Debug.Log("Recv from: " + remoteServer.ToString() + "Data: " + Encoding.ASCII.GetString(buffer, 0, recv));

                string remoteCubePosStr = Encoding.ASCII.GetString(buffer, 0, recv);
                var remoteCubePos = remoteCubePosStr.Split(':');
                string remoteCubePos2 = remoteCubePos[1];
                var remoteCubePos3 = remoteCubePos2.Split(',');
                //Debug.Log(remoteCubePos3[0] + " " + remoteCubePos3[1] + " " + remoteCubePos3[2]);
                float x, y, z;
                x = float.Parse(remoteCubePos3[0]);
                y = float.Parse(remoteCubePos3[1]);
                z = float.Parse(remoteCubePos3[2]);

                remoteCube.transform.position = new Vector3(x, y, z);
            }
        }
        
        
        //UDP Position Updates

        //TCP Position Updates
        /*bool positionChanged = false;
        if (x != myCube.transform.position.x)
        {
            positionChanged = true;

        }
        else if (y != myCube.transform.position.y)
        {
            positionChanged = true;
        }
        else if (z != myCube.transform.position.z)
        {
            positionChanged = true;
        }

        //x = y = ++z;
        if (positionChanged == true)
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

        positionChanged = false;*/
        //TCP Position Updates

        if(textReceived == true)
        {
            TextChat.GetInstance().CreateTextBox(textMessage);
            textReceived = false;
        }
    }

    private static void ReceiveCallback(IAsyncResult results)
    {
        Socket socket = (Socket)results.AsyncState;
        int rec = socket.EndReceive(results);

        byte[] data = new byte[rec];
        Array.Copy(buffer, data, rec);

        string msg = Encoding.ASCII.GetString(data);
        Debug.Log("Received from Server: " + msg);
        textReceived = true;
        textMessage = msg;

        //pos = new float[rec]; //rec == data.length
        //Buffer.BlockCopy(buffer, 0, pos, 0, rec);

        socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), socket);
        //Debug.Log("Receive X:" + pos[0] + " Y: " + pos[1] + " Z: " + pos[2]);
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

            //Thread.Sleep(1000);
            //c += 2;
        }
    }


    public static void StartUDPClient(string _ip) //UDP Position Updates
    {
        try
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            remoteEP = new IPEndPoint(ip, 8889);

            clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        
            remoteServer = new IPEndPoint(ip, 8889); //test

            //More Tests
            byte[] connectionMessage = Encoding.ASCII.GetBytes("Hi from ne3Client");
            clientSoc.SendTo(connectionMessage, remoteEP);
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    public static void StartTCPClient(string _ip)
    {
        client.Connect(IPAddress.Parse(_ip), 8888);
        Debug.Log("Connected to server!!!");

        client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), client);
    }

    public void SendToTCPServer()
    {
        tcpMsg = chatBox.GetComponent<TMP_InputField>().text;
        byte[] buffer = Encoding.ASCII.GetBytes(tcpMsg);
        client.Send(buffer);
    }

    public void LoginToServer()
    {
        StartUDPClient(logInTextField.GetComponent<TMP_InputField>().text);
        StartTCPClient(logInTextField.GetComponent<TMP_InputField>().text);
        login = true;
        loginPanel.SetActive(false);
    }
    public void Quit()
    {
        tcpMsg = "quit";
        byte[] buffer = Encoding.ASCII.GetBytes(tcpMsg);
        client.Send(buffer);
        client.Close();
    }
}
