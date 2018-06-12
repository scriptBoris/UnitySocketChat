using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSendMessage : MonoBehaviour
{
    public InputField uInputField;
    public Network uNetwork;

    // Use this for initialization
    void Start()
    {

    }

    private void OnClick()
    {
        string text = uInputField.text;
        if (text != "")
        {
            uInputField.text = "";
            var msg = new Message { Name="Unity",Text = text };
            uNetwork.SendData(msg);
            //uNetwork.SendMessage(msg);
        }
    }
}
