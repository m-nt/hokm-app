using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using System;
using System.Text.RegularExpressions;

public class SocketConnection : MonoBehaviour
{
    public string URL;
    //public PlayerJoinMsg player;
    public PlayersJoinedMsg opponents;
    public Authentication auth;
    public GameDataController GDC;
    public GameController gameCtrl;
    public PlayersObjectMsg stage;
    public int[] seq = new int[4],seqCustom = new int[5];
    public AvatarData defaultAvatar;
    //private string prefix = "ws://", postfix = "/socket.io/?EIO=4&transport=websocket";
    private SocketManager manager;
    private bool isCustom = false;
    private string pattern = @"<body>(.*)</body>";
    // Start is called before the first frame update

    private void Awake()
    {
        OnConnect();
    }
    void Start()
    {
        auth = GetComponent<Authentication>();
        auth.SockConn = GetComponent<SocketConnection>();
        
    }
    public IEnumerator OnGetURLs()
    {
        URLS urls;
        using (UnityWebRequest req = UnityWebRequest.Get("https://hokm-url.herokuapp.com/urls.json"))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                Regex rg = new Regex(pattern, RegexOptions.Singleline);
                Match mc = rg.Match(req.downloadHandler.text);
                string json = mc.Value;
                urls = JsonUtility.FromJson<URLS>(json);
                URL = urls.server;
            }
        }

    }

    public void OnConnect()
    {
        //SocketOptions options = new SocketOptions();
        //options.Timeout = new TimeSpan(0,1,0);
        manager = new SocketManager(new Uri( URL));
        manager.Socket.On<SockMessage>("hi", hellosocket);
        manager.Socket.On<PlayersJoinedMsg>("playersjoined", playersJoined);
        manager.Socket.On<PlayersJoinedMsg>("playerpositionchange", playersJoinedCustom);
        manager.Socket.On("playerDCed", OnDCed);
        manager.Socket.On<PlayerJoinMsg>("playerDCedLobby", OnDCedLobby);
        manager.Socket.On<PlayersObjectMsg>("GetStage", OnGetStage);
        manager.Socket.On<ChatMsgObj>("GetChat", OnGetChat);
        manager.Socket.On("customlobbyready", OnCustomLobbyReady);
        manager.Socket.On("customlobbyNOTready", OnCustomLobbyNOTReady);
        manager.Socket.On<ChatMsgObj>("RequestToJoin", OnGetRequestToJoin);
        manager.Socket.On("StartTheMatch", OnStartTheMatch);
        manager.Socket.On<PlayersObjectMsg>("EndTheMatch", OnEndTheGame);
        manager.Socket.On(SocketIOEventTypes.Disconnect, OnDisconnect);
        manager.Socket.On(SocketIOEventTypes.Connect, OnConnecting);

    }
    public void OnDisconnect()
    {
        //show the player we are disconnected
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void OnEndTheGame(PlayersObjectMsg msg)
    {
        stage = msg;
        stage.playerCards.init();
        gameCtrl.OnEndTheMatch();
    }
    public void OnConnecting()
    {
        manager.Socket.Emit("init", new ReadyMsg(auth.user.Username, auth.user._id));
    }
    public void OnCustomLobbyReady()
    {
        GDC.gameObject.lobby.ready.interactable = false;
        GDC.gameObject.lobby.ready.transform.GetChild(0).GetComponent<Text>().text = "...";
    }
    public void OnCustomLobbyNOTReady()
    {
        PlayerJoinMsg plyr = opponents.GetPlayerById(auth.user._id);
        if (plyr != null && int.Parse(plyr.number) == 0)
        {
            GDC.gameObject.lobby.ready.interactable = true;
        }
        GDC.gameObject.lobby.ready.transform.GetChild(0).GetComponent<Text>().text = "یﺯﺎﺑ یﺍﺮﺑ نﺪﺷ هﺩﺎﻣﺁ";
    }
    public void OnChangePosition(int toIndex)
    {
        manager.Socket.Emit("ChangePosition",new ChangePosition(int.Parse(opponents.GetPlayerById(auth.user._id).number),toIndex));
    }
    public void OnGetRequestToJoin(ChatMsgObj Msg)
    {
        GDC.OnGetRequestToJoin(Msg.getChatMsg());
    }
    public void OnJoinCustomLobby(string room)
    {
        // last line of code :)
        isCustom = true;
        manager.Socket.Emit("PlayerJoinCustomLobby", new ChatMsg(auth.user.Username, auth.user._id, "msg", room));
        // get the player to custom lobby
        GDC.gameObject.menu.parent.gameObject.SetActive(false);
        GDC.gameObject.lobby.parent.gameObject.SetActive(true);
    }
    public void OnSendRequest(string id,string room)
    {
        //last line of code :)
        manager.Socket.Emit("RequestToJoin", new ChatMsg(auth.user.Username,id,"msg",room));
    }
    public void OnGetChat(ChatMsgObj msg)
    {
        gameCtrl.NewChat(msg.getChatMsg());
    }
    public void OnSendChat(ChatMsg msg)
    {
        manager.Socket.Emit("SendChat",msg);
    }
    public void OnStartTheMatch()
    {
        //start the match
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    public void OnDCedLobby(PlayerJoinMsg msg)
    {
        for (int i = 0; i < seq.Length; i++)
        {
            if (seq[i] == int.Parse(msg.number))
            {
                GDC.opponents[i].Destroy();
                break;
            }
        }
        opponents.DestroyIndivisual(msg);
    }
    public void OnGetStage(PlayersObjectMsg msg)
    {
        stage = msg;
        stage.playerCards.init();
        gameCtrl.next();
    }
    public void OnSetStage(SocketStageMsg msg) 
    {
        manager.Socket.Emit("setStage",msg);
    }
    public void OnDCed()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void OnReadyLobby(int LobbyID)
    {
        ReadyMsg msg = new ReadyMsg(auth.user.Username, auth.user._id);
        switch (LobbyID)
        {
            case 0:
                isCustom = false;
                manager.Socket.Emit("PlayerReadyLobby", msg);
                foreach (Opponents item in GDC.opponents)
                {
                    item.parent.gameObject.SetActive(false);
                }
                break;
            case 1:
                isCustom = true;
                manager.Socket.Emit("PlayerReadyCustomLobby", msg);
                foreach (Opponents item in GDC.opponentsCustom)
                {
                    item.parent.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }

    }
    public void OnReadyCustomLobbyGame()
    {
        manager.Socket.Emit("ReadyToPlayCustom");
    }
    public void OnLeaveLobby()
    {
        manager.Socket.Emit("PlayerLeaveLobby");
    }
    public void OnReadySignal()
    {
        manager.Socket.Emit("ReadySignal");
    }
    void hellosocket(SockMessage msg)
    {
    }

    void playersJoinedCustom(PlayersJoinedMsg msg)
    {
        // Clear the lobby items
        foreach (Opponents item in GDC.opponentsCustom)
        {
            item.Destroy();
        }
        foreach (PlayerJoinMsg item in opponents.users)
        {
            item.Destroy();
        }

        PlayerJoinMsg me = msg.GetPlayerById(auth.user._id);
        if (int.Parse(me.number) != 0)
        {
            GDC.gameObject.lobby.ready.interactable = false;
        }
        seqCustom = opponents.SeqNumberCustom(int.Parse(me.number));
        seq = opponents.SeqNumber(int.Parse(me.number));
        opponents.room = msg.room;


        for (int i = 0; i < seqCustom.Length; i++)
        {
            PlayerJoinMsg player = msg.GetPlayerByNumber(seqCustom[i]);
            if (player != null)
            {
                opponents.users[i] = player;
                if (player.active == false)
                {
                    player.Curency = "0";
                    player.Avatar = defaultAvatar;
                    GDC.opponentsCustom[seqCustom[i]].init(opponents.users[i]);
                }
                else
                {
                    StartCoroutine(GetPlayerDataCustom(player, i));
                }
            }
        }
        if (msg.users.Length == 4)
        {
        //start the timer for match
            GDC.txtCounterCustom.gameObject.SetActive(true);
            GDC.txtCounterCustom.enabled = true;
            StartCoroutine(ReadyToStartTheMatch());
        }

    }

    void playersJoined(PlayersJoinedMsg msg)
    {
        if (!isCustom)
        {
            foreach (PlayerJoinMsg item in opponents.users)
            {
                item.Destroy();
            }
            foreach (Opponents item in GDC.opponents)
            {
                item.Destroy();
            }

            PlayerJoinMsg me = msg.GetPlayerById(auth.user._id);
            opponents.room = msg.room;
            opponents.users[0] = me;
            opponents.users[0].Avatar = auth.user.Avatar;
            opponents.users[0].Curency = auth.user.Curency;
            opponents.users[0].VIP = auth.vip.name;
            GDC.opponents[0].init(opponents.users[0]);
            seq = opponents.SeqNumber(int.Parse(me.number));
            for (int i = 1; i < seq.Length; i++)
            {
                PlayerJoinMsg player = msg.GetPlayerByNumber(seq[i]);
                if (player != null)
                {
                    opponents.users[i] = player;
                    if (player.active == false)
                    {
                        player.Curency = "0";
                        player.Avatar = defaultAvatar;
                        GDC.opponents[i].init(opponents.users[i]);
                    }
                    else
                    {
                        StartCoroutine(GetPlayerData(player, i));
                    }
                }
            }
            if (msg.users.Length == 4)
            {
                //start the timer for match
                GDC.txtCounter.gameObject.SetActive(true);
                GDC.txtCounter.enabled = true;
                StartCoroutine(ReadyToStartTheMatch());
            }
        }
        else
        {
            playersJoinedCustom(msg);
        }
    }
    IEnumerator ReadyToStartTheMatch()
    {
        yield return new WaitForSeconds(5);
        OnReadySignal();
    }
    IEnumerator GetPlayerDataCustom(PlayerJoinMsg msg, int index)
    {

        CoroutineWithData co = new CoroutineWithData(this, auth.OnGetUserByIdEnum(msg,index));
        yield return co.coroutine;

        string result = co.result.ToString();
        if (result == "ok")
        {
            // set Game Data in waiting lobie
            GDC.opponentsCustom[seqCustom[index]].init(opponents.users[index]);

        }
        else
        {
            // idk whf will happens
            // its means a player joind but neither a network error occurred or player data isn't correct
            // nok : user id doesn't exists - user id doesn't set in request body - user id is fake
            Debug.Log(result);
            auth.SetStatus(GDC.gameObject.status, result, auth.colorError);
        }
    }
    public void onDisconnect()
    {
        manager.Socket.Disconnect();
    }
    IEnumerator GetPlayerData(PlayerJoinMsg msg,int index)
    {

        CoroutineWithData co = new CoroutineWithData(this, auth.OnGetUserByIdEnum(msg,index));
        yield return co.coroutine;

        string result = co.result.ToString();
        if (result == "ok")
        {
            // set Game Data in waiting lobie
            GDC.opponents[index].init(opponents.users[index]);

        }
        else
        {
            // idk whf will happens
            // its means a player joind but neither a network error occurred or player data isn't correct
            // nok : user id doesn't exists - user id doesn't set in request body - user id is fake
            Debug.Log(result);
            auth.SetStatus(GDC.gameObject.status, result, auth.colorError);
        }
    }

}

