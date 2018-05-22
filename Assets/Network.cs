using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Network : MonoBehaviour
{
    public Text uText;

    private StringBuilder _sender = new StringBuilder("");
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

                //Array.Copy(buffer, 0, incData, 0, length);
                //string jsonData = Encoding.ASCII.GetString(incData);
                //print(jsonData);

                int space = 0;
                while (space<99)
                {
                    if (Encoding.ASCII.GetString(new byte[] { buffer[space] }) == "{")
                    {
                        break;
                    }
                    space++;
                }



                if (space > 0 && space < 99)
                {
                    Array.Copy(buffer, space, incData, 0, length);
                    string jsonData = Encoding.ASCII.GetString(incData);

                    byte[] spaceBytes = new byte[space];
                    for (int i = 0; i <= space-1; i++)
                    {
                        spaceBytes[i] = buffer[i];
                    }

                    int iJson = Convert.ToInt32(Encoding.ASCII.GetString(spaceBytes));
                    var jsonType = (JsonTypes)iJson;

                    ExtractData(jsonType, jsonData);
                }
                else if (space == 0)
                {
                    print("Not found type income JSON object");
                }
                else
                    print("Bad incoming data from server");
            }
        }
    }
    
    private void ExtractData(JsonTypes jType, string data)
    {
        switch (jType)
        {
            case JsonTypes.Command:
                break;
            case JsonTypes.MediaPlan:
                break;
            case JsonTypes.MediaTemplate:
                break;
            case JsonTypes.Message:
                var msg = JsonUtility.FromJson<Message>(data);
                print(msg.Text);
                break;
        }
    }

    public void SendMessage(Message msgOut)
    {
        msgOut.Name = "Unity";
        var json = JsonUtility.ToJson(msgOut);

        byte[] buffer = new byte[json.Length];
        buffer = Encoding.ASCII.GetBytes(json);
        _tcpClient.Client.Send(buffer);

        //var json = new DataContractJsonSerializer(typeof(Message));
        //json.WriteObject(_stream, msg);
    }

    // Update is called once per frame
    void Update()
    {
        if (_msg.Length != 0)
        {
            uText.text += "\n" + _sender.ToString() + ": " + _msg.ToString();
            print(_msg.ToString());
            _msg.Length = 0;
            _sender.Length = 0;
        }
    }

    // Dispose resources pre exit application
    void OnApplicationQuit()
    {
        _tcpClient.Close();
        _stream.Close();
        _stream.Dispose();
    }
}
