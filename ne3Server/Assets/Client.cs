using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lec04
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;


public class Client : MonoBehaviour
{
    public GameObject myCube;
    private static byte[] outBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static Socket clientSoc;

    public static void StartClient()
    {
        try
        {
            IPAddress ip = IPAddress.Parse("10.160.35.168");
            remoteEP = new IPEndPoint(ip, 8888);

            clientSoc = new Socket(AddressFamily.InterNetwork, 
                SocketType.Dgram, ProtocolType.Udp);



        } catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myCube = GameObject.Find("Cube");
        StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        outBuffer = Encoding.ASCII.GetBytes(myCube.transform.position.x.ToString());

        clientSoc.SendTo(outBuffer, remoteEP);
        
    }
}
