using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
public class PlayersObject
{
    
}
[Serializable]
public class ImageSeries
{
    public Transform parent;
    public GameObject prefab,selectPrefab;
    public Image image;
    public string presetURI,presentLowURL;
    public int count;
    
    public void init()
    {

    }
    public void Destroy()
    {
        foreach (Transform item in parent)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
    public void DesableChilds()
    {
        foreach (Transform item in parent)
        {
            item.gameObject.GetComponent<Image>().sprite = null;
            item.gameObject.GetComponent<Button>().interactable = false;
        }
    }
}

[Serializable]
public class ReportObject
{
    public Transform parent;
    public Toggle rulebreak,rudeness;
    public InputField message;
    public Button submit;
}
[Serializable]
public class ReportMsg
{
    public string user_pk,user_pk_sender;
    public int type;
    public string message;
    public bool issued;
    public string _id;
    public int __v;
    public ReportMsg(string User_pk,int Type,string Message)
    {
        user_pk = User_pk;
        user_pk_sender = "";
        type = Type;
        message = Message;
        issued = false;
        _id = "";
        __v = 0;
    }
}
[Serializable]
public class ChangePosition
{
    public int index, toIndex;
    public ChangePosition(int Index,int ToIndex)
    {
        index = Index;
        toIndex = ToIndex;
    }
}
[Serializable]
public class FriendRequest
{
    public Transform parent;
    public GameObject prefab;
    public void Destroy()
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
[Serializable]
public class FriendListLobbie
{
    public Transform parentparent,parent;
    public RectTransform button;
    public Vector2 open, close;
    public bool isOpen = false;
    public GameObject prefab;
    public Color statusOFFLINE,statusONLINE,statusAWAIT;

    public Color GetStatusColor(string status)
    {
        switch (status)
        {
            case "OFFLINE":
                return statusOFFLINE;
            case "ONLINE":
                return statusONLINE;
            case "AWAIT":
                return statusAWAIT;
            default:
                return Color.black;
        }
    }
    public void Destroy()
    {
        foreach (Transform item in parent)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
}

[Serializable]
public class Notifications
{
    public Transform parent;
    public Image notify;
    public Text count;
    public Notification[] items;
    public void Destroy()
    {
        foreach (Transform child in items[0].parent)
        {
            if (child.transform.childCount > 3)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
[Serializable]
public class Notification
{
    public string name;
    public Transform parent;
    public GameObject prefab;

}

[Serializable]
public class PlayersOptions
{
    public PlayerOptions[] options;
}
[Serializable]
public class PlayerOptions
{
    public string name;
    public bool isOpen;
    public Transform parent;
    public Text addfriend;
    public Button addfriendbutt, reportbutt;
}
[Serializable]
public class Chat
{
    public Transform parent;
    public RectTransform inputParent;
    public GameObject mePrefab, otherPrefab,newMessageIcon;
    public Dictionary<int, MeChat> meDic = new Dictionary<int, MeChat>();
    public Dictionary<int, OtherChat> otherDic = new Dictionary<int, OtherChat>();
    public InputField input;

    public void createMeChat(Transform Parent,Text Text,ChatMsg msg)
    {
        meDic.Add(meDic.Count, new MeChat(Parent, Text));
        meDic[meDic.Count-1].init(msg);
    }
    public void createOtherChat(Transform Parent, Text Text,Text Username,Image Avatar,ChatMsg msg,Sprite SAvatar)
    {
        otherDic.Add(otherDic.Count, new OtherChat(Parent, Text,Username,Avatar));
        otherDic[otherDic.Count-1].init(msg, SAvatar);
    }
}
[Serializable]
public class ChatMsg
{
    public string name,userID,msg,room;

    public ChatMsg(string Name,string UserID,string Msg,string Room)
    {
        name = Name;
        userID = UserID;
        msg = Msg;
        room = Room;
    }
}
[Serializable]
public class ChatMsgObj
{
    public string name, userID, msg,room;

    public ChatMsg getChatMsg()
    {
        return new ChatMsg(name, userID, msg,room);
    }
    public void setChatMsg(ChatMsg Msg)
    {
        name = Msg.name;
        userID = Msg.userID;
        msg = Msg.msg;
        room = Msg.room;
    }
}
[Serializable]
public class MeChat
{
    public string name,userID;
    public Transform parent;
    public Text text;
    
    public MeChat(Transform Parent,Text Text)
    {
        parent = Parent;
        text = Text;
    }
    public void init(ChatMsg msg)
    {
        name = msg.name;
        userID = msg.userID;
        text.text = msg.msg;
    }
}
[Serializable]
public class OtherChat
{
    public string name,userID;
    public Transform parent;
    public Text text,username;
    public Image avatar;

    public OtherChat(Transform Parent, Text Text,Text Username,Image Avatar)
    {
        parent = Parent;
        text = Text;
        username = Username;
        avatar = Avatar;
    }
    public void init(ChatMsg msg,Sprite Avatar)
    {
        name = msg.name;
        userID = msg.userID;
        text.text = msg.msg;
        username.text = msg.name;
        avatar.sprite = Avatar;
    }

}
[Serializable]
public class EndTeams 
{
    public Transform parent;
    public EndTeam winner, losser;
    public void SetUpResult(string result,int yourScore,int oppScore,int Score)
    {
        parent.gameObject.SetActive(true);
        switch (result)
        {
            case "you":
                winner.init(yourScore,oppScore,Score,true);
                break;
            case "other":
                losser.init(yourScore, oppScore, Score,false);
                break;
            default:
                break;
        }
    }
}
[Serializable]
public class EndTeam
{
    public Transform parent;
    public Text YourTeamScore, OppTeamScore, YourScore;
    public void init(int you,int opp,int score,bool winer)
    {
        parent.gameObject.SetActive(true);
        YourTeamScore.text = you.ToString();
        OppTeamScore.text = opp.ToString();
        if (winer)
        {
            YourScore.text = score.ToString();
        }
        else
        {
            YourScore.text = "-" + score.ToString();
        }
    }
}
[Serializable]
public class Teams
{
    public Team[] teams;

    public void Update(PlayersObjectMsg msg,int[] seq)
    {
        int playerTeam = WitchTeam(seq[0]);
        for (int i = 0; i < teams.Length; i++)
        {
            for (int j = 0; j < teams[i].count.Length; j++)
            {
                teams[i].count[j].enabled = false;
            }
        }
        if (playerTeam == 0)
        {
            teams[0].score.text = msg.teamScore.team0.ToString();
            teams[1].score.text = msg.teamScore.team1.ToString();
            for (int i = 0; i < msg.teamCount.team0; i++)
            {
                teams[0].count[i].enabled = true;
            }
            for (int i = 0; i < msg.teamCount.team1; i++)
            {
                teams[1].count[i].enabled = true;
            }
        }
        else
        {
            teams[0].score.text = msg.teamScore.team1.ToString();
            teams[1].score.text = msg.teamScore.team0.ToString();
            for (int i = 0; i < msg.teamCount.team0; i++)
            {
                teams[1].count[i].enabled = true;
            }
            for (int i = 0; i < msg.teamCount.team1; i++)
            {
                teams[0].count[i].enabled = true;
            }
        }
    }
    public int WitchTeam(int pn)
    {
        if (pn == 0 || pn == 2)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}
[Serializable]
public class Team
{
    public string name;
    public Text score;
    public Image[] count;

}
[Serializable]
public class SocketStageMsg
{
    public int pn;
    public int cd;

    public SocketStageMsg(int Pn,int Cd)
    {
        pn = Pn;
        cd = Cd;
    }
}
[Serializable]
public class PlayersObjectMsg
{
    public string msg,stage;
    public int nextPlayer,playedPlayer,hokm,ruler,hand,timeout, cardsOnIndex;
    public TeamCountScore teamCount, teamScore;
    public CardsOnGround cardsOnGround;
    public PlayerCards playerCards;
    
    public int GetIndex(int[] playerCards,int id)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (playerCards[i] == id)
            {
                return i;
            }

        }
        return -1;
    }
    public int GetStageNumber
    {
        get{ return int.Parse(stage.Split(',')[0]); }
    }
}
[Serializable]
public class TeamCountScore
{
    public int team0, team1;

    public string WichTeam(string number)
    {
        int nmbr = int.Parse(number);
        if (nmbr == 0 || nmbr == 2)
        {
            return "team0";
        }
        else
        {
            return "team1";
        }
    }
}
[Serializable]
public class CardsOnGround
{
    public int C0, C1,C2,C3;
    public int GetCard(int pn)
    {
        switch (pn)
        {
            case 0:
                return C0;
            case 1:
                return C1;
            case 2:
                return C2;
            case 3:
                return C3;
            default:
                return 0;
        }
    }
    public int cardsIndex
    {
        get { return howmany(); }
    }
    private int howmany()
    {
        int res = 0;
        if (C0 != 0)
        {
            res++;
        }
        if (C1 != 0)
        {
            res++;
        }
        if (C2 != 0)
        {
            res++;
        }
        if (C3 != 0)
        {
            res++;
        }
        return res;
    }
}
[Serializable]
public class PlayerCards
{
    public int[] P0, P1, P2, P3;
    public Dictionary<int,int[]> cards;
    public void init()
    {
        cards = new Dictionary<int, int[]>();
        cards.Add(0, P0);
        cards.Add(1, P1);
        cards.Add(2, P2);
        cards.Add(3, P3);
    }
    public int[] GetCard(int P,int start,int end)
    {
        int[] result = new int[end - start];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = cards[P][i + start];
        }
        return result;
    }
    public int[] GetCard(int P)
    {
        int[] result = new int[cards[P].Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = cards[P][i];
        }
        return result;
    }

}
public class SortArray
{
    public int[] array;
    public SortArray(int[] Array)
    {
        array = Array;
    }
    public int[] Sort
    {
        get { return sort(); }
    }
    private int[] sort()
    {
        int[] res = new int[array.Length];
        IEnumerable<int> query =
            array.OrderBy(id => id);
        int counter = 0; 
        foreach (int item in query)
        {
            res[counter] = item;
            counter++;
        }
        return res;
    }
}
[Serializable]
public class Players
{
    public Player player;
    public OtherPlayers[] opponents;
    public void init(PlayersJoinedMsg opp,int[] seq)
    {
        player.avatar.sprite = opp.users[0].Avatar.data.sprite;
        player.cardhandler.center.position = player.cardhandler.transform.position;
        for (int i = 1; i < seq.Length; i++)
        {
            PlayerJoinMsg plyr = opp.GetPlayerByNumber(seq[i]);
            opponents[i-1].avatar.sprite = plyr.Avatar.data.sprite;
            opponents[i-1].username.text = plyr.name;
            opponents[i-1].cardhandler.center.position = opponents[i-1].cardhandler.transform.position;
        }
    }
    public void SetZero()
    {
        player.cardhandler.SetZero();
        foreach (OtherPlayers item in opponents)
        {
            item.cardhandler.SetZero();
        }
    }
}

[Serializable]
public class Player
{
    public CardHandler cardhandler;
    public Image avatar;

}
[Serializable]
public class OtherPlayers
{
    public OtherPlayerCardHandler cardhandler;
    public Image avatar;
    public Text username;
}
[Serializable]
public class OtherPlayerTemp
{
    public Sprite avatar;
    public string username;
}
[Serializable]
public class ChatRoom
{
    public Transform parent;
    public GameObject meChatPrefab, otherChatPrefab;

}
[Serializable]
public class Opponents
{
    public Transform parent;
    public Text name;
    public Text curency;
    public Image avatar;
    public SimpleVIPBadge badge;
    public void Destroy()
    {
        name.text = "";
        curency.text = "";
        avatar.sprite = null;
        badge.destroy();
        parent.gameObject.SetActive(false);
    }
    public void init(PlayerJoinMsg p)
    {
        name.text = p.name;
        curency.text = p.Curency;
        avatar.sprite = p.Avatar.data.sprite;
        if (p.VIP == null)
        {
        badge.init("");
        }
        else
        {
            badge.init(p.VIP);
        }
        parent.gameObject.SetActive(true);
    }
}
[Serializable]
public class SimpleVIPBadge
{
    public bool isVIP;
    public Image badge;
    public void init(string vip)
    {
        isVIP = true;
        badge.enabled = true;
        switch (vip) 
        {
            case "1M":
                badge.color = new Color(149,210,234,255);
                break;
            case "2M":
                badge.color = new Color(87, 222, 93, 255);
                break;
            case "3M":
                badge.color = new Color(241, 187, 36, 255);
                break;
            default:
                badge.enabled = false;
                break;
        }
    }
    public void destroy()
    {
        isVIP = false;
        badge.enabled = false;
    }
}
[Serializable]
public class SockMessage
{
    public string message;
}
[Serializable]
public class PlayersJoinedMsg
{
    public PlayerJoinMsg[] users;
    public string room;
    public void DestroyIndivisual(PlayerJoinMsg pmsg)
    {
        foreach (PlayerJoinMsg item in users)
        {
            if (item.sockID == pmsg.sockID)
            {
                item.Destroy();
                break;
            }
        }
    }
    public int[] SeqNumber(int number)
    {
        //int userZero = int.Parse(users[0].number);
        switch (number)
        {
            case 0:
                return NumberSeter(0, 1, 2, 3);
            case 1:
                return NumberSeter(1, 2, 3, 0);
            case 2:
                return NumberSeter(2, 3, 0, 1);
            case 3:
                return NumberSeter(3, 0, 1, 2);
            default:
                return NumberSeter(0, 1, 2, 3); ;
        }

    }
    public int[] SeqNumberCustom(int number)
    {
        //int userZero = int.Parse(users[0].number);
        switch (number)
        {
            case 0:
                return NumberSeter(0, 1, 2, 3, 4);
            case 1:
                return NumberSeter(1, 2, 3, 0, 4);
            case 2:
                return NumberSeter(2, 3, 0, 1, 4);
            case 3:
                return NumberSeter(3, 0, 1, 2, 4);
            case 4:
                return NumberSeter(0, 1, 2, 3, 4);
            default:
                return null;
        }

    }
    private int[] NumberSeter(int i0,int i1,int i2,int i3)
    {
        int[] numbers = new int[4] { i0, i1, i2, i3 };
        return numbers;
    }
    private int[] NumberSeter(int i0, int i1, int i2, int i3,int i4)
    {
        int[] numbers = new int[5] { i0, i1, i2, i3,i4 };
        return numbers;
    }
    public PlayerJoinMsg GetPlayerById(string id)
    {

        foreach (PlayerJoinMsg item in users)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;

    }
    public PlayerJoinMsg GetPlayerByNumber(int number)
    {

        foreach (PlayerJoinMsg item in users)
        {
            if (int.Parse(item.number) == number)
            {
                return item;
            }
        }
        return null;

    }
}
[Serializable]
public class PlayerJoinMsg
{
    public string name;
    public string id;
    public string sockID;
    public string number;
    public string Curency;
    public AvatarData Avatar;
    public string VIP;
    public bool active;

    public void init(PlayerJoinMsg player)
    {
        name = player.name;
        id = player.id;
        sockID = player.sockID;
        number = player.number;
        active = player.active;
    }
    public void init(PlayerGetMessage msg)
    {
        if (msg.user.VIP != null && msg.user.VIP != "")
        {
            VIP = msg.user.VIP;
        }
        Curency = msg.user.Curency;
        Avatar = msg.user.Avatar;
        Avatar.data = new TextureM(Avatar.data.data, new Vector2(200, 200));
    }
    public void Destroy()
    {
        name = "";
        id = "";
        sockID = "";
        number = "";
        Curency = "";
        //Avatar.Destroy();
        VIP = "";
        active = false;
    }
}
[Serializable]
public class PlayerGetMessage
{
    public string message;
    public string code;
    public PlayerJoinMsg user;
}
[Serializable]
public class CoroutineWithData
{
    public Coroutine coroutine { get; private set; }
    public object result;
    private IEnumerator target;
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = target.Current;
            yield return result;
        }
    }
}