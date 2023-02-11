using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class GameDataController : MonoBehaviour
{

    public Authentication authenticate;
    public SocketConnection sockConn;
    public SettingObject gameObject;
    public TextCounter txtCounter,txtCounterCustom;
    public Opponents[] opponents,opponentsCustom;
    public Notifications notification;
    public FriendListLobbie friendlist;
    public FriendRequest joinRequest;
    public ImageSeries avatar, background;

    int LeaveLobbyCounter = 3;
    float notificationTimer = 15f;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        sockConn = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<SocketConnection>();
        sockConn.GDC = this.GetComponent<GameDataController>();
        authenticate = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Authentication>();
        authenticate.GDC = this.GetComponent<GameDataController>();
        PlayerPrefs.SetString("cashedBackground", "false");
        PlayerPrefs.SetString("cashedAvatar", "false");
        //gameObject.OnSetBackgroundAspect();
        gameObject.menu.init(authenticate.user);
        if (authenticate.vip.expires != "")
        {
            gameObject.vipBadge.init(authenticate.vip);
            StartCoroutine(OnGetBackground(authenticate.user.Background));
        }
        gameObject.cards.selectNewCard(int.Parse(authenticate.user.DeckOfCard));
        OnDetectNotification();
        StartCoroutine(authenticate.OnChangeStatus("ONLINE"));
        gameObject.menu.loading.SetActive(true);
        //OnCardsSelect(int.Parse(authenticate.user.DeckOfCard));
        authenticate.adobject.ShowStanBanner();
        if (authenticate.vip.expires == "")
        {
            authenticate.adobject.StanBannerAd(open => { }, err => 
            { 
                authenticate.SetStatus(gameObject.status, err, authenticate.colorError); 
            });
            authenticate.adobject.OnRandomAd(
              open => 
            {
                gameObject.AdRemove.SetActive(true);
            },reward=> {
                gameObject.AdRemove.SetActive(true);
            }, close => 
            {
                //nothing happenning
                gameObject.AdRemove.SetActive(true);
                authenticate.SetStatus(gameObject.status, close, authenticate.colorWarning);
            }, err =>
            {
                authenticate.SetStatus(gameObject.status, err, authenticate.colorError);
                authenticate.adobject.active = true;
            });
        }
        if (authenticate.logged)
        {
            gameObject.menu.loading.SetActive(false);
        }
#if UNITY_WEBGL
        gameObject.menu.loading.SetActive(false);
#endif
    }
    public void OnRewardVideo(int amount)
    {
        authenticate.adobject.RewardVideo(open => {
            gameObject.AdRemove.SetActive(true);
            authenticate.SetStatus(gameObject.status, open, authenticate.colorWarning);
        }, reward => {
            gameObject.profile.init(authenticate.user);
            gameObject.profile.setAvatar();
            authenticate.curency.init(amount);
            StartCoroutine(authenticate.OnUpdateUserEnum(gameObject));

        }, close => {
            gameObject.profile.init(authenticate.user);
            gameObject.profile.setAvatar();
            authenticate.curency.init(amount);
            StartCoroutine(authenticate.OnUpdateUserEnum(gameObject));
            authenticate.SetStatus(gameObject.status, close, authenticate.colorWarning);
        }, err => {
            authenticate.SetStatus(gameObject.status, err, authenticate.colorError);
            authenticate.adobject.active = true;
        });
    }
    public void UpdateProfile()
    {
        StartCoroutine(OnUpdateProfile());
    }
    public IEnumerator OnUpdateProfile()
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.OnGetUser());
        yield return co.coroutine;
        User user = new User();
        string err = "";
        try
        {
            user = (User)co.result;
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (err.Length == 0)
        {
            user.Avatar.data = new TextureM(user.Avatar.data.data, new Vector2(200, 200),"avatar");
            gameObject.menu.init(user);
            if (authenticate.vip.expires != "")
            {
                gameObject.vipBadge.init(authenticate.vip);
                StartCoroutine(OnGetBackground(authenticate.user.Background));
            }
            gameObject.cards.selectNewCard(int.Parse(user.DeckOfCard));
            OnDetectNotification();
            StartCoroutine(authenticate.OnChangeStatus("ONLINE"));

        }

    }
    public void OnPurchaseItem(string data)
    {
        string[] result = data.Split(',');
        string description = result[0];
        int amount = int.Parse(result[1]);
        StartCoroutine(OnPurchese(description, amount));
    }
    IEnumerator OnPurchese(string desc,int amount)
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.OnPerchase(desc,amount));
        yield return co.coroutine;
        string result = co.result.ToString();
        Debug.Log(result);
    }
    IEnumerator OnGetBackground(string id)
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.GetTexture(background.presetURI, id));
        yield return co.coroutine;
        string result = co.result.ToString();
        Texture2D texture = new Texture2D(1,1);
        string err = "";
        try
        {
            texture = (Texture2D)co.result;
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (err == "")
        {
            
            gameObject.background.init(texture, id);
            gameObject.setBackgrounds();
            GameObject obj = Instantiate(background.selectPrefab, background.parent.GetChild(int.Parse(id) - 1), false);

        }
        else
        {
            Debug.Log(err);
        }

    }
    public void OnOpenGallery()
    {
#if UNITY_ANDROID

        if (!NativeGallery.IsMediaPickerBusy())
        {
            NativeGallery.GetImageFromGallery((path) => {
                Debug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    TextureM texture = new TextureM(NativeGallery.LoadImageAtPath(path, 200), new Vector2(200, 200));
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }
                    else
                    {
                        //set the avatar image 
                        authenticate.user.Avatar.data = texture;
                        OnAvatar(texture.sprite);
                        OnProfileSelect();
                    }
                }
            }, "Avatar", "image/*");
        }

#endif
    }

    public void OnOpenMenuSetting()
    {
        if (PlayerPrefs.HasKey("cashedAvatar"))
        {
            if (PlayerPrefs.GetString("cashedAvatar") == "false")
            {
                avatar.Destroy();
                for (int i = 1; i <= avatar.count; i++)
                {
                    StartCoroutine(OnGetAvatars(i.ToString()));
                }

            }
        }
        else
        {
            avatar.Destroy();
            for (int i = 1; i <= avatar.count; i++)
            {
                StartCoroutine(OnGetAvatars(i.ToString()));
            }
        }
    }
    public IEnumerator OnGetAvatars(string id)
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.GetTexture(avatar.presetURI, id.ToString()));
        yield return co.coroutine;
        string result = co.result.ToString();
        TextureM texture = new TextureM();
        string err = "";
        try
        {
            Texture2D usersTemp = (Texture2D)co.result;
            texture = new TextureM(usersTemp, new Vector2(200, 200),false);
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (texture != null)
        {
            GameObject obj = Instantiate(avatar.prefab, avatar.parent, false);
            obj.GetComponent<Image>().sprite = texture.sprite;
            obj.GetComponent<Button>().onClick.AddListener(delegate { OnAvatar(texture.sprite); });

            if (int.Parse(id) == avatar.count)
            {
                PlayerPrefs.SetString("cashedAvatar", "true");
            }
        }
        else
        {
            PlayerPrefs.SetString("cashedAvatar", "false");
            Debug.Log(err);
        }
    }
    public void OnOpenBackroundSetting(Setting setting)
    {
        //gameObject.menu.background
        if (PlayerPrefs.HasKey("cashedBackground"))
        {
            if (PlayerPrefs.GetString("cashedBackground") == "false")
            {
                for (int i = 1; i <= background.count; i++)
                {
                    StartCoroutine(OnGetBackgrounds(i));
                }

            }
        }
        else
        {

            for (int i = 1; i <= background.count; i++)
            {
                StartCoroutine(OnGetBackgrounds(i));
            }

        }

    }
    public IEnumerator OnGetBackgrounds(int id)
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.GetTexture(background.presentLowURL, id.ToString()));
        yield return co.coroutine;
        string result = co.result.ToString();
        TextureM texture = new TextureM();
        string err = "";
        try
        {
            Texture2D usersTemp = (Texture2D)co.result;
            usersTemp.name = "Downloaded Texture";
            usersTemp.Compress(false);
            usersTemp.Apply();
            texture = new TextureM(usersTemp,"Converted Texture: ("+id.ToString()+")",EncodeType.encodeType.JPG);
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (texture != null)
        {
            background.parent.GetChild(id - 1).GetComponent<Image>().sprite = texture.sprite;
            background.parent.GetChild(id - 1).GetComponent<Button>().interactable = true;
            if (id == background.count)
            {
                PlayerPrefs.SetString("cashedBackground", "true");
            }
        }
        else
        {
            PlayerPrefs.SetString("cashedBackground", "false");
            Debug.Log(err);
        }
        yield return new WaitForSeconds(0.01f);
    }

    private void Update()
    {
        notificationTimer -= Time.deltaTime;
        if (notificationTimer <= 0)
        {
            notificationTimer = 15f;
            //calculate every 2 seconds
            OnDetectNotification();
        }
    }
    public void OnOpenRefrenceURL(string url)
    {
        Application.OpenURL(url);
    }
    public void OnChangePosition(int toIndex)
    {
        sockConn.OnChangePosition(toIndex);
    }
    public void OnGetRequestToJoin(ChatMsg msg)
    {
        // here to show player to join a lobby
        // msg.name = the name of user requested to join
        // msg.room = the room to join
        GameObject obj = Instantiate(joinRequest.prefab, joinRequest.parent, false);
        obj.transform.GetChild(0).GetComponent<Text>().text = msg.name;
        obj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { OnRequestToJoin(msg.room,obj); });
        obj.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { OnCloseJoinRequest(obj); });
    }
    public void OnCloseJoinRequest(GameObject obj)
    {
        Destroy(obj);
    }
    public void OnRequestToJoin(string room,GameObject obj)
    {
        sockConn.OnJoinCustomLobby(room);
        Destroy(obj);
    }
    public void OnOpenFriendList()
    {
        if (friendlist.isOpen)
        {
            friendlist.Destroy();
            friendlist.isOpen = false;
            // Close FriendList
            //friendlist.button.rect.Set(0, friendlist.close, friendlist.button.rect.width, friendlist.button.rect.height);
            friendlist.button.anchoredPosition = friendlist.close;
            // replace with animation
            friendlist.parentparent.gameObject.SetActive(false);
        }
        else
        {
            // Open FriendList
            friendlist.isOpen = true;
            //friendlist.button.rect.Set(0, friendlist.open,friendlist.button.rect.width,friendlist.button.rect.height);
            friendlist.button.anchoredPosition = friendlist.open;
            // replace with animation
            friendlist.parentparent.gameObject.SetActive(true);
            StartCoroutine(OnOpenFriendListEnum());
        }
    }
    IEnumerator OnOpenFriendListEnum()
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.OnGetFriends());
        yield return co.coroutine;
        User[] users = new User[0];
        string err = "";
        try
        {
            User[] usersTemp = (User[])co.result;
            users = new User[usersTemp.Length];
            users = usersTemp;
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (users.Length > 0)
        {
            foreach (User item in users)
            {
                
                GameObject obj = Instantiate(friendlist.prefab, friendlist.parent, false);
                TextureM avatar = new TextureM(item.Avatar.data.data, new Vector2(100, 100));
                obj.transform.GetChild(0).GetComponent<Image>().sprite = avatar.sprite;
                obj.transform.GetChild(1).GetComponent<Text>().text = item.Username;
                obj.transform.GetChild(2).GetComponent<Text>().text = item.Status;
                obj.transform.GetChild(2).GetComponent<Text>().color = friendlist.GetStatusColor(item.Status);
                obj.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { OnSendRequestToJoinGame(item._id,sockConn.opponents.room); });
            }
        }
        else
        {
            //show the error 
        }
    }
    public void OnSendRequestToJoinGame(string id,string room)
    {
        sockConn.OnSendRequest(id,room);
    }
    public void ReadyLobby(int LobbyID)
    {
        sockConn.OnReadyLobby(LobbyID);
        authenticate.adobject.HideStanBanner();
    }
    public void ReadyCustomLobbyGame()
    {
        sockConn.OnReadyCustomLobbyGame();
    }
    public void OnLeaveLobby(int LobbyID)
    {
        if (LeaveLobbyCounter > 0)
        {
            LeaveLobbyCounter--;
            if (LobbyID == 0)
            {
                gameObject.random.leaveLobby.text = "(" + LeaveLobbyCounter.ToString() + ")";
            }
            else
            {
                gameObject.lobby.leaveLobby.text = "(" + LeaveLobbyCounter.ToString() + ")";
            }
        }
        else
        {

            if (LobbyID == 0)
            {
                gameObject.random.leaveLobby.text = "(3)";
                gameObject.random.parent.gameObject.SetActive(false);
                gameObject.menu.parent.gameObject.SetActive(true);
            }
            else
            {
                gameObject.lobby.leaveLobby.text = "(3)";
                gameObject.lobby.parent.gameObject.SetActive(false);
                gameObject.menu.parent.gameObject.SetActive(true);
            }
            sockConn.OnLeaveLobby();
            LeaveLobbyCounter = 3;
            authenticate.adobject.ShowStanBanner();
        }
    }
    public void OnCardsSelect(int index)
    {
        StartCoroutine(OnCardsSelectEnum(index));
    }
    public void OnBackgroundSelect(string index)
    {
        StartCoroutine(OnBackgroundSelectEnum(index));
    }
    public void OnLeaderboard()
    {
        StartCoroutine(authenticate.OnLeaderBoardEnum(gameObject.leaderboard.limit));
    }
    public void OnLeaderboardBack()
    {
        StartCoroutine(OnLeaderBoardBack());
    }
    public void OnProfileSelect()
    {
        gameObject.profile.init(authenticate.user);
    }
    public void OnAvatar(Sprite sprite)
    {
        gameObject.profile._avatar.sprite = sprite;
        gameObject.profile.setAvatar();
    }
    public void OnProfileAvatarChange()
    {

        gameObject.profile.setAvatar();
    }
    public void OnEnterAndUpdate()
    {
        //gameObject.profile.init(authenticate.user);
        StartCoroutine(authenticate.OnUpdateUserEnum(gameObject));
        gameObject.profile.ButtonPressed();
    }
    public void OnLoggOut()
    {
        StartCoroutine(authenticate.OnLoggoutEnum());
    }
    public void OnCloseNotification()
    {
        notification.Destroy();
        notification.parent.gameObject.SetActive(false);
    }
    public void OnDetectNotification()
    {
        notification.Destroy();
        StartCoroutine(OnGetPendingRequests());
        StartCoroutine(OnGetReports());
    }
    public void OnOpenNotification()
    {
        if (notification.items[0].parent.transform.childCount > 0)
        {
            notification.parent.gameObject.SetActive(true);
        }
        
    }
    IEnumerator OnGetReports()
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.OnGetReports());
        yield return co.coroutine;

        ReportMsg[] reports = new ReportMsg[0];
        string err = "";
        try
        {
            ReportMsg[] reportsTemp = (ReportMsg[])co.result;
            reports = new ReportMsg[reportsTemp.Length];
            reports = reportsTemp;
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (reports.Length > 0)
        {
            foreach (ReportMsg item in reports)
            {
                GameObject obj = Instantiate(notification.items[item.type].prefab, notification.items[item.type].parent, false);
                obj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { OnCloseReportNotify(obj); });
            }
            notification.notify.GetComponent<Animator>().SetTrigger("Bell");
        }
        else
        {
            // show whats happend 
        }
        if (notification.items[0].parent.transform.childCount > 0)
        {
            notification.count.text = notification.items[0].parent.transform.childCount.ToString();
            //OnOpenNotification();
        }
        else
        {
            OnCloseNotification();
            notification.count.text = "0";
        }
    }
    public void OnCloseReportNotify(GameObject obj)
    {
        Destroy(obj);
        notification.count.text = notification.items[0].parent.transform.childCount.ToString();
    }
    IEnumerator OnGetPendingRequests()
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.OnGetPendingRequests());
        yield return co.coroutine;
        User[] users = new User[0];
        string err = "";
        try
        {
            User[] usersTemp = (User[])co.result;
            users = new User[usersTemp.Length];
            users = usersTemp;
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }


        if (users.Length > 0)
        {
            foreach (User item in users)
            {
                GameObject obj = Instantiate(notification.items[0].prefab, notification.items[0].parent, false);
                TextureM avatar = new TextureM(item.Avatar.data.data,new Vector2(100,100));
                obj.transform.GetChild(0).GetComponent<Image>().sprite = avatar.sprite;
                obj.transform.GetChild(2).GetComponent<Text>().text = item.Username;
                obj.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { OnAcceptReject(obj,item._id, "ACCEPTED"); });
                obj.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { OnAcceptReject(obj,item._id, "REJECTED"); });
            }
            notification.notify.GetComponent<Animator>().SetTrigger("Bell");
        }
        else
        {
            // show whats happend 
        }
        if (notification.items[0].parent.transform.childCount > 0)
        {
            notification.count.text = notification.items[0].parent.transform.childCount.ToString();
            //OnOpenNotification();
        }
        else
        {
            OnCloseNotification();
            notification.count.text = "0";
        }
    }
    public void OnAcceptReject(GameObject obj, string id, string accrej)
    {
        StartCoroutine(OnAcceptRejectEnum(obj,id, accrej));
    }
    IEnumerator OnAcceptRejectEnum(GameObject obj, string id,string accrej)
    {
        CoroutineWithData co = new CoroutineWithData(this, authenticate.OnAcceptReject(id,accrej));
        yield return co.coroutine;
        string result = co.result.ToString();
        if (result == "ok")
        {
            //if friendship set seccussfuly
            Destroy(obj);
            OnDetectNotification();
            authenticate.SetStatus(gameObject.status, result, authenticate.colorError);
        }
        else if(result != "")
        {
            //if got error of friendship already is set
            authenticate.SetStatus(gameObject.status, result, authenticate.colorError);
        }
        else
        {
            Destroy(obj);
        }
    }
    IEnumerator OnLeaderBoardBack()
    {
        for (int i = 0;i < gameObject.leaderboard.players.Length; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject.Destroy(gameObject.leaderboard.players[i].gameObject);
        }
        gameObject.leaderboard.players = new GameObject[0];
        gameObject.leaderboard.loading.SetActive(true);
    }
    IEnumerator OnCardsSelectEnum(int index)
    {
        yield return authenticate.OnVerifyVIPEnum();
        if (gameObject.vipBadge.isVIP)
        {
            gameObject.cards.selectNewCard(index);
            yield return authenticate.OnUpdateUserEnum(gameObject);
        }
        else
        {
            //go to vip
            gameObject.cards.selectedCardIndex = int.Parse(authenticate.user.DeckOfCard);
            gameObject.cards.Destroy();
            gameObject.profile.parent.gameObject.SetActive(false);
            gameObject.vip.parent.gameObject.SetActive(true);
        }

    }
    IEnumerator OnBackgroundSelectEnum(string index)
    {
        yield return authenticate.OnVerifyVIPEnum();
        if (gameObject.vipBadge.isVIP)
        {
            int previusIndex = int.Parse(gameObject.background.index);
            if (previusIndex < 50)
            {
                if (background.parent.GetChild(previusIndex-1).childCount > 0)
                {
                    foreach (Transform item in background.parent.GetChild(previusIndex - 1))
                    {
                        Destroy(item.gameObject);
                    }
                }
            }
            gameObject.background.index = index;
            gameObject.setBackgrounds(background.parent.GetChild(int.Parse(index)-1).GetComponent<Image>().sprite);
            GameObject obj = Instantiate(background.selectPrefab, background.parent.GetChild(int.Parse(index) - 1), false);
            yield return authenticate.OnUpdateUserEnum(gameObject);
        }
        else
        {
            //go to vip
            gameObject.background.Destroy();
            gameObject.setBackgrounds();
            gameObject.profile.parent.gameObject.SetActive(false);
            gameObject.vip.parent.gameObject.SetActive(true);
        }

    }
}
