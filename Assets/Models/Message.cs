using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
[Serializable]
public class Message
{
    [DataMember]
    public int Request;
    [DataMember]
    public int Response;
    [DataMember]
    public string Name;
    [DataMember]
    public string Text;
    [DataMember]
    public string Receiver;
}
