using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
using System;

[Serializable]
public static class KeyboardHeight
{
    public static float height
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

                using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                {
                    View.Call("getWindowVisibleDisplayFrame", Rct);

                    return (((float)Screen.height - (float)Rct.Call<int>("height")) / (float)Screen.height)*1920.0f;
                }
            }
#else
            return (((float)Screen.height / 2)/(float)Screen.height) * 1920;
#endif
        }
    }
}
[Serializable]
public class URLS
{
    public string server;
    public string auth;
}
[Serializable]
public class CredentialPref
{
    public string Username;
    public string Password;
    public CredentialPref()
    {

    }
    public CredentialPref(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
[Serializable]
public class Message
{
    public string message;
    public string code;
    public string err;
    public User user;
    public User[] users;
    public VIP vip;
    public ReportMsg[] reports;
}
[Serializable]
public class Leaderboard
{
    public User[] message;
    public string code;

}
[Serializable]
public class User
{
    public string Curency;
    public string _id;
    public string Username;
    public string Password;
    public string DeckOfCard;
    public string Background;
    public AvatarData Avatar;
    public string DeviceInfo;
    public int __v;
    public int Debt;
    public string Status;
    public string token;
    public void Destroy()
    {
        Curency = "";
        _id = "";
        Username = "";
        Debt = 0;
        Password = "";
        DeviceInfo = "";
        __v = 0;
    }
}
[Serializable]
public class SingleMessage
{
    public string message;
    public string code;
    public string error;
}
[Serializable]
public class VIP
{
    public string name;
    public string _id;
    public string user_pk;
    public string expires;
    public DateTime expire;

    public void StringToDate()
    {
        string expi = expires.Replace(".000Z", "");
        expi = expi.Replace("T","-");
        expire = DateTime.ParseExact(expi, "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
    }
    public void DateToString()
    {
        expires = expire.Year.ToString()+"-"+
                  expire.Month.ToString()+"-"+
                  expire.Day.ToString()+"T"+
                  expire.Hour.ToString()+":"+
                  expire.Minute.ToString()+":"+
                  expire.Second.ToString();
    }
    public void Destroy()
    {
        name = "";
        _id = "";
        user_pk = "";
        expires = "";
        expire = new DateTime();
    }
}