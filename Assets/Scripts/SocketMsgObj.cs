using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class SocketMsgObj : MonoBehaviour
{
}
[Serializable]
public class ReadyMsg
{
    public string name;
    public string id;
    public ReadyMsg(string Name,string _id)
    {
        name = Name;
        id = _id;
    }
}