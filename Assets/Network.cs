using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

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
            byte[] buffer = new byte[1024];
            int length = _stream.Read(buffer, 0, buffer.Length);
            if (length > 0)
            {
                var incData = new byte[length];

                var incFullData = new byte[length];
                Array.Copy(buffer, 0, incFullData, 0, length);
                string stringData = Encoding.ASCII.GetString(incFullData);

                if (stringData[0] == '!')
                {
                    // Получили простой сигнал-опрос от сервера. Так он знает что мы не отвалились или не ушли в Оффлайн
                }
                else
                {
                    int space = 0;
                    while (space < 99)
                    {
                        if (incFullData[space] == '{')
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
                        for (int i = 0; i <= space - 1; i++)
                        {
                            spaceBytes[i] = buffer[i];
                        }

                        int iJson = Convert.ToInt32(Encoding.ASCII.GetString(spaceBytes));
                        var jsonType = (JsonTypes)iJson;

                        print(jsonType);

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
                _sender.Append(msg.Name);
                _msg.Append(msg.Text);
                break;
        }
    }

    [Obsolete("Устаревший метод, который отправляет <<строгий>> тип данных. Используйте альтернативу" +
        "SendData")]
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

    public void SendData(System.Object obj)
    {
        var fields = new List<FieldInfo>(obj.GetType().GetFields());
        foreach (var field in fields)
        {
            if (field != null && field.Name == "JsonType")
            {
                var sJson = (JsonTypes)field.GetValue(obj);
                var iJson = (int)sJson;
                var json = JsonUtility.ToJson(obj);
                byte[] buffer = new byte[json.Length];
                buffer = Encoding.ASCII.GetBytes(iJson + json);
                _tcpClient.Client.Send(buffer);

                return;
            }
        }
        throw new Exception("SendData not found field with JSON enum");
    }

    void RenderIncommingMessage(string author, string message)
    {
        uText.text += "\n" + _sender.ToString() + ": " + _msg.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (_msg.Length != 0)
        {
            RenderIncommingMessage(_sender.ToString(), _msg.ToString());
            print("update");
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
