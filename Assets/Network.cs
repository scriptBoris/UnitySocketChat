using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Network : MonoBehaviour
{
    private StringBuilder _msg = new StringBuilder("");
    private bool _isEnabled;
    private TcpClient _tcpClient;
    private NetworkStream _stream;
    private Thread _thread;
    // Use this for initialization
    void Start()
    {
        _tcpClient = new TcpClient();
        _tcpClient.Connect("localhost", 8000);

        if (_tcpClient.Connected)
        {
            _isEnabled = true;
            _stream = _tcpClient.GetStream();
            _thread = new Thread(DataEngine);
            _thread.Start();
            print("success connect to server");
        }
        else
            print("bad try connect to server");
    }

    private void DataEngine()
    {
        while (_isEnabled)
        {
            if (_stream.DataAvailable)
            {
                byte[] buffer = new byte[1024];
                int length = _stream.Read(buffer, 0, buffer.Length);
                var incData = new byte[length];

                Array.Copy(buffer, 0, incData, 0, length);
                string serverMessage = Encoding.ASCII.GetString(incData);

                _msg.Append(serverMessage);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_msg.Length != 0)
        {
            print(_msg.ToString() );
            _msg.Length = 0;
        }
    }
}
