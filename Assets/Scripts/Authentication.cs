using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TapsellPlusSDK;
using System.Text.RegularExpressions;
using System.IO;
public class Authentication : MonoBehaviour
{
    //public properties
    public string baseURL,merchID,reqPurch,verifyPurch,startPurch;
    public int StatusDelay = 3;
    public Color colorError, colorWarning, colorLog;
    public User user;
    public VIP vip;
    public Curency curency;
    public User[] leaderboard;
    public GameDataController GDC;
    public GameController gameCtrl;
    public SocketConnection SockConn;
    public AuthObject authObject;
    public OtherPlayerTemp opTwo, opThree, opFour;
    public AdObject adobject;
    public bool logged = false;
    //Private properties

    private string pattern = "{\".*\"}";
    private string Username = "", Password = "", CPassword = "", DeviceInfo = "";
    public string SetDeviceInfo { set { DeviceInfo = value; } }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        adobject.init();
#if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        Username = "";
        Password = "";
        CPassword = "";
        DeviceInfo = "";
        
    }
    public IEnumerator OnGetURLs()
    {
        URLS urls;
#if UNITY_ANDROID
        //File.Exists(Application.persistentDataPath);
        string url = "http://hokm-urls.blogfa.com/page/urls";
#endif
#if UNITY_WEBGL
        string url = "https://hokm-url.herokuapp.com/urls.json";
#endif

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
#if UNITY_ANDROID
            req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
#endif

            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
#if UNITY_ANDROID

                Regex rg = new Regex(pattern, RegexOptions.Singleline);
                Match mc = rg.Match(req.downloadHandler.text);
                string json = mc.Value;
                Debug.Log(json);
                urls = JsonUtility.FromJson<URLS>(json);
                baseURL = urls.auth;
                SockConn.URL = urls.server;
#endif
#if UNITY_WEBGL
                urls = JsonUtility.FromJson<URLS>(req.downloadHandler.text);
                baseURL = urls.auth;
                SockConn.URL = urls.server;
#endif

                OnCheckLoggin();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }

    }
    public void OnDialogFileAvatar(Texture2D txt)
    {
        user.Avatar.data = new TextureM(txt, new Vector2(200, 200));
        authObject.avatar.image.sprite = user.Avatar.data.sprite;
    }
    public void OnAvatar(TextureM file)
    {
        user.Avatar.data = file;
    }
    public void OnImageSetAvatar(Image avatar, Sprite image)
    {
        avatar.sprite = image;
    }
    public void OnUsername(InputField txt)
    {
        Username = txt.text;
    }
    public void OnPassword(InputField txt)
    {
        Password = txt.text;
    }
    public void OnCPassword(InputField txt)
    {
        CPassword = txt.text;
    }
    public void OnCheckLoggin()
    {
        StartCoroutine(OnCheckLogginEnum());
    }
    public void OnLoggin()
    {
        StartCoroutine(OnLogginEnum());
        authObject.authobj.loggin.ButtonPressed();
    }
    public void OnRegister()
    {
        StartCoroutine(OnRegisterEnum());
        authObject.authobj.register.ButtonPressed();
    }
    public void SetStatus(GameObject status, string msg, Color color)
    {
        StartCoroutine(OnSetStatusEnum(status, msg, color));
    }
    public IEnumerator OnUpdateCurrency()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL+ "users/updatecurency",RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    curency.init(user);
                    yield return data.user;
                }
                else
                {
                    OnMessageProcess(data.message);
                    yield return data.message;
                }
            }
            else
            {
                yield return req.error;
            }
        }
    }
    public IEnumerator GetTexture(string type,string id)
    {
        Texture2D textureCash = ImgMng.loadTexture(type + id + ").jpg");
        if (textureCash != null)
        {
            yield return textureCash;
        }
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(baseURL + type + id +").jpg"))
        {
            yield return req.SendWebRequest();

            if (!req.isHttpError && !req.isNetworkError)
            {
                Texture2D texture = ((DownloadHandlerTexture)req.downloadHandler).texture;
                ImgMng.save(type + id + ").jpg", texture);
                yield return texture;
            }
            else
            {
                yield return req.error;
            }
        }
        
    }
    public IEnumerator OnPerchase(string desc,int amount)
    {
        WWWForm RForm = new WWWForm();
        ReqPurchMessage data;
        PurchaseReqBody reqdata = new PurchaseReqBody(merchID, amount.ToString(), desc, baseURL + "pay/callback");
        //RForm.AddField("merchant_id", merchID);
        //RForm.AddField("amount", amount);
        //RForm.AddField("description", desc);
        //RForm.AddField("callback_url", baseURL+"/pay/callback");
        using (UnityWebRequest req = UnityWebRequest.Put(reqPurch,JsonUtility.ToJson(reqdata)))
        {
            req.method = UnityWebRequest.kHttpVerbPOST;
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<ReqPurchMessage>(req.downloadHandler.text);
                if (data.data.code == 100 || data.data.code == 101)
                {
                    CoroutineWithData co = new CoroutineWithData(this, OnInitiatePurchase(desc,amount,data.data.authority));
                    yield return co.coroutine;
                    ReqPurchMessage result = new ReqPurchMessage();
                    string err = "";
                    try
                    {
                        result = (ReqPurchMessage)co.result;
                    }
                    catch (System.Exception)
                    {
                        err = co.result.ToString();
                    }
                    if (result.code == "ok")
                    {
                        Application.OpenURL(startPurch + data.data.authority);
                        yield return data.data.code;
                    }
                    else
                    {
                        SetStatus(GDC.gameObject.status, result.message, colorWarning);
                        yield return data.data.code;
                    }
                    
                }
                else
                {
                    SetStatus(GDC.gameObject.status, data.error, colorWarning);
                    yield return data.error;
                }
            }
            else
            {
                SetStatus(GDC.gameObject.status, req.error, colorError);
                yield return req.error;
            }
        }
    }
    public IEnumerator OnInitiatePurchase(string desc, int amount,string authority)
    {
        WWWForm RForm = new WWWForm();
        ReqPurchMessage data;
        RForm.AddField("amount", amount);
        RForm.AddField("description", desc);
        RForm.AddField("authority", authority);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL+"pay/init", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<ReqPurchMessage>(req.downloadHandler.text);
                OnMessageProcess(data.message);
                yield return data;
            }
            else
            {
                SetStatus(GDC.gameObject.status, req.error, colorError);
                yield return req.error;
            }
        }

    }
    public IEnumerator OnUpdateUserEnum(SettingObject obj)
    {
        obj.profile.setAvatar();
        Message data;
        CredentialPref pref = new CredentialPref();
        if (PlayerPrefs.HasKey("creds"))
        {
            pref = JsonUtility.FromJson<CredentialPref>(PlayerPrefs.GetString("creds"));
        }
        WWWForm RForm = new WWWForm();
        if (obj.profile.username.text != "") { RForm.AddField("Username", obj.profile.username.text);
            if (pref.Username != obj.profile.username.text)
            {
                pref.Username = obj.profile.username.text;
            }
        }
        if (obj.profile.password.text != "") { RForm.AddField("Password", obj.profile.password.text);
            if (pref.Password != obj.profile.password.text)
            {
                pref.Password = obj.profile.password.text;
            }
        }
        PlayerPrefs.SetString("creds",JsonUtility.ToJson(pref));
        RForm.AddBinaryData("Avatar", obj.profile.avatar.data);
        if (obj.background.index != "404") { RForm.AddField("Background", obj.background.index); }
        if (obj.cards.newCard) { RForm.AddField("DeckOfCard", obj.cards.selectedCardIndex.ToString()); }
        if (curency.newCurency) { RForm.AddField("Curency", curency.newAmount); }

        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/updateuser", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {

                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    user = data.user;
                    if (data.vip.user_pk != null)
                    {
                        vip = data.vip;
                        GDC.gameObject.vipBadge.init(vip);
                    }
                    else
                    {
                        vip.Destroy();
                        GDC.gameObject.vipBadge.destroy();
                    }
                    user.Avatar.data = new TextureM(data.user.Avatar.data.data);
                    curency.init(user);
                    GDC.gameObject.menu.init(user);
                    GDC.gameObject.profile.init(user);
                    obj.profile.username.transform.parent.GetComponent<InputField>().text = "";
                    obj.profile.password.transform.parent.GetComponent<InputField>().text = "";
                    GDC.UpdateProfile();
                }
                else
                {
                    OnMessageProcess(data.message);
                    GDC.gameObject.profile.ButtonReleased();
                    SetStatus(GDC.gameObject.status, data.message, colorWarning);
                }
            }
            else
            {
                GDC.gameObject.profile.ButtonReleased();
                SetStatus(GDC.gameObject.status, req.error, colorError);
            }

        }

    }
    public IEnumerator OnLeaderBoardEnum(int limit)
    {
        Message data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("limit", limit.ToString());
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/leaderboard", RForm))
        {

            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {

                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    leaderboard = data.users;
                    GDC.gameObject.leaderboard.players = new GameObject[leaderboard.Length];
                    GDC.gameObject.leaderboard.loading.SetActive(false);
                    for (int i = 0; i < leaderboard.Length; i++)
                    {

                        leaderboard[i].Avatar.data = new TextureM(leaderboard[i].Avatar.data.data, new Vector2(200, 200));
                        // instantiating leaderboard objects
                        GameObject player = Instantiate(GDC.gameObject.leaderboard.initialize(GDC.gameObject.leaderboard.playerPrefab, leaderboard[i], (i + 1).ToString()), GDC.gameObject.leaderboard.root);
                        GDC.gameObject.leaderboard.players[i] = player;
                    }
                }
                else if (data.code == "nok")
                {
                    // Go back to loggin page
                    //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                    SingleMessage msg = JsonUtility.FromJson<SingleMessage>(req.downloadHandler.text);
                    OnMessageProcess(msg.message);
                    SetStatus(GDC.gameObject.status, msg.message, colorError);
                }
                else
                {
                    // Data isnt valid - doesnt get any players
                    SingleMessage message = JsonUtility.FromJson<SingleMessage>(req.downloadHandler.text);
                    SetStatus(GDC.gameObject.status, message.message, colorError);
                }
            }
            else
            {
                SetStatus(GDC.gameObject.status, req.error, colorError);
            }

        }

    }
    IEnumerator OnRegisterEnum()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        if (Username != "") { RForm.AddField("Username", Username); }
        if (Password != "") { RForm.AddField("Password", Password); }
        if (CPassword != "") { RForm.AddField("CPassword", CPassword); }
        if (user.Avatar.data.data.Length == 0)
        {
            user.Avatar.data = new TextureM(user.Avatar.data.texture, new Vector2(200, 200));
        }
        RForm.AddBinaryData("Avatar", user.Avatar.data.data);
        RForm.AddField("DeviceInfo", DeviceInfo);

        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/register", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {

                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    // Go to loggin section
                    user = data.user;
                    user.Avatar.data = new TextureM(data.user.Avatar.data.data, new Vector2(200, 200));
                    authObject.authobj.SwitchPage();
                    curency.init(user);
                    SetStatus(authObject.authobj.status, data.message, colorLog);
                }
                else
                {
                    // show that user is not logged in
                    authObject.authobj.register.ButtonReleased();
                    SetStatus(authObject.authobj.status, data.message, colorError);
                }
            }
            else
            {
                authObject.authobj.register.ButtonPressed();
                SetStatus(authObject.authobj.status, req.error, colorError);
            }

        }

    }
    IEnumerator OnSetStatusEnum(GameObject status, string msg, Color color)
    {
        status.SetActive(true);
        status.GetComponentInChildren<Image>().color = color;
        status.GetComponent<Text>().text = msg;
        yield return new WaitForSeconds(StatusDelay);
        status.GetComponent<Text>().text = "";
        status.GetComponentInChildren<Image>().color = Color.white;
        status.SetActive(false);
    }
    public IEnumerator OnLoggoutEnum()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/loggout", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                StartCoroutine(OnChangeStatus("OFFLINE"));
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    //go back to Authentication page
                    //user = data.user;
#if UNITY_ANDROID
                    PlayerPrefs.DeleteKey("cookie");
                    PlayerPrefs.DeleteKey("creds");
                    UnityWebRequest.ClearCookieCache();
#endif
                    logged = false;
                    user.Destroy();
                    vip.Destroy();
                    SockConn.onDisconnect();
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);

                }
                else
                {
                    // show that user is not logged in
                    SetStatus(GDC.gameObject.status, data.message, colorWarning);
                }
            }

        }

    }
    IEnumerator OnStelthLogin()
    {
        Message data;
        CredentialPref pref = new CredentialPref();
        WWWForm RForm = new WWWForm();
#if UNITY_ANDROID || UNITY_WEBGL
        if (PlayerPrefs.HasKey("creds"))
        {
            pref = JsonUtility.FromJson<CredentialPref>(PlayerPrefs.GetString("creds"));
            RForm.AddField("Username", pref.Username);
            RForm.AddField("Password", pref.Password);
        }
#endif
        if (GDC != null)
        {
            GDC.gameObject.menu.loading.SetActive(true);
            GDC.gameObject.resetHome();
        }
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/stelthloggin", RForm))
        {

            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                curency.init(user);
                //SockConn.OnConnect();
                StartCoroutine(OnChangeStatus("ONLINE"));
                StartCoroutine(OnUpdateCurrency());
                if (GDC != null)
                {
                    SetStatus(GDC.gameObject.status, data.message, colorLog);
                    GDC.gameObject.menu.loading.SetActive(false);
                }
                adobject.active = true;
                logged = true;
            }
            else
            {
                if (GDC != null) { 
                    SetStatus(GDC.gameObject.status, req.error, colorError);
                }
            }
        }
    }
    IEnumerator OnLogginEnum()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("Username", Username);
        RForm.AddField("Password", Password);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/loggin", RForm))
        {
            //SetHeader(req);
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    //go to home page
#if UNITY_ANDROID
                    if (req.GetResponseHeader("Set-Cookie") != null)
                    {
                        PlayerPrefs.SetString("cookie", req.GetResponseHeader("Set-Cookie"));
                    }
                    CredentialPref pref = new CredentialPref(Username, Password);
                    PlayerPrefs.SetString("creds", JsonUtility.ToJson(pref));
#endif
                    user = data.user;
                    Debug.Log("From Loggin: " + req.GetResponseHeader("Set-Cookie"));
                    if (data.vip.user_pk != null)
                    {
                        // public vip ezafe shavad va in code montaghel shavad be GameDataController
                        vip = data.vip;
                        //GDC.gameObject.vipBadge.init(data.vip);
                    }
                    else
                    {
                        vip.Destroy();
                        //GDC.gameObject.vipBadge.destroy();
                    }
                    user.Avatar.data = new TextureM(data.user.Avatar.data.data);
                    curency.init(user);
                    //SockConn.OnConnect();
                    StartCoroutine(OnChangeStatus("ONLINE"));
                    StartCoroutine(OnUpdateCurrency());
                    logged = true;
                    yield return new WaitForSeconds(0.1f);
                    UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                }
                else
                {
                    authObject.authobj.loggin.ButtonReleased();
                    // show that user is not logged in
                    SetStatus(authObject.authobj.status, data.message, colorWarning);
                }
            }
            else
            {
                authObject.authobj.loggin.ButtonReleased();
                SetStatus(authObject.authobj.status, req.error, colorError);
            }

        }

    }
    IEnumerator OnCheckLogginEnum()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/checkloggin", RForm))
        {
#if UNITY_ANDROID
            if (PlayerPrefs.HasKey("cookie"))
            {
                req.SetRequestHeader("Cookie", PlayerPrefs.GetString("cookie"));
            }
#endif
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                if (authObject != null)
                {
                    authObject.authobj.loading.SetActive(false);
                }
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {

                    //go to home page
                    StartCoroutine(OnStelthLogin());
                    user = data.user;
                    if (data.vip.user_pk != null)
                    {
                        vip = data.vip;
                        //GDC.gameObject.vipBadge.init(data.vip);
                    }
                    else
                    {
                        vip.Destroy();
                        //GDC.gameObject.vipBadge.destroy();
                    }
                    user.Avatar.data = new TextureM(data.user.Avatar.data.data);
                    curency.init(user);
                    //SockConn.OnConnect();
                    StartCoroutine(OnChangeStatus("ONLINE"));
                    StartCoroutine(OnUpdateCurrency());
                    yield return new WaitForSeconds(0.1f);
                    UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                }
                else
                {
                    // show that user is not logged in
                    authObject.authobj.loggin.parent.GetComponent<Animator>().SetTrigger("Go");
                    SetStatus(authObject.authobj.status, data.message, colorError);
                }
            }
            else
            {
                yield return new WaitForSeconds(5);
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }
    public IEnumerator OnGetUser()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/getuser", RForm))
        {

            yield return req.SendWebRequest();

            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    //go to home page
                    yield return data.user;
                }
                else
                {
                    // show that user is not logged in
                    OnMessageProcess(data.message);
                    SetStatus(GDC.gameObject.status, data.message, colorError);
                    yield return data.message;
                }
            }
            else
            {
                SetStatus(GDC.gameObject.status, req.error, colorError);
                yield return req.error;
            }
        }
    }

    public IEnumerator OnVerifyVIPEnum()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/verifyvip", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {

                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    vip = data.vip;
                    GDC.gameObject.vipBadge.init(vip);
                }
                else
                {
                    // show that user is not logged in
                    vip.Destroy();
                    GDC.gameObject.vipBadge.destroy();
                    OnMessageProcess(data.message);
                    SetStatus(GDC.gameObject.status, data.message, colorWarning);
                }
            }
            else
            {
                SetStatus(GDC.gameObject.status, req.error, colorError);
            }
        }

    }
    public IEnumerator OnGetUserByIdEnum(PlayerJoinMsg user, int index)
    {
        PlayerGetMessage data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("id", user.id);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/getuserbyid", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {

                data = JsonUtility.FromJson<PlayerGetMessage>(req.downloadHandler.text);

                if (data.code == "ok")
                {
                    SockConn.opponents.users[index].init(data);
                    yield return data.code;
                }
                else
                {
                    OnMessageProcess(data.message);
                    SetStatus(GDC.gameObject.status, data.message, colorWarning);
                    yield return data.code;
                }
            }
            else
            {
                SetStatus(GDC.gameObject.status, req.error, colorError);
                yield return req.error;
            }
        }

    }
    public IEnumerator OnFriendRequest(string id)
    {
        Message data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("id", id);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/friendrequest", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    yield return data.code;
                }
                else
                {
                    SetStatus(gameCtrl.status, data.err, colorWarning);
                    yield return data.err;
                }
            }
            else
            {
                SetStatus(gameCtrl.status, req.error, colorError);
                yield return req.error;
            }
        }
    }
    public IEnumerator OnGetPendingRequests()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/pendingrequests", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {  
                    yield return data.users;
                }
                else
                {
                    OnMessageProcess(data.message);
                    yield return data.message;
                }
            }
            else
            {
                yield return req.error;
            }
        }
    }
    public IEnumerator OnAcceptReject(string id,string accrej)
    {
        Message data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("id",id);
        RForm.AddField("accrej", accrej);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/accept-reject", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                yield return data.code;
                OnMessageProcess(data.message);
            }
            else
            {
                yield return req.error;
            }
        }
    }
    public IEnumerator OnGetFriends()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/getfriends", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    yield return data.users;
                }
                else
                {
                    OnMessageProcess(data.message);
                    yield return data.err;
                }
            }
            else
            {
                yield return req.error;
            }
        }
    }
    public void OnApplicationQuit()
    {
        StartCoroutine(OnChangeStatus("OFFLINE"));
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            StartCoroutine(OnChangeStatus("ONLINE"));
        }
        else
        {
            StartCoroutine(OnChangeStatus("OFFLINE"));
        }
    }
    public IEnumerator OnChangeStatus(string status)
    {
        Message data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("status", status);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/changestatus", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                yield return data;
                if (status != "AWAIT")
                {
                    OnMessageProcess(data.message);
                }
            }
            else
            {
                yield return req.error;
            }
        }
    }
    public IEnumerator OnSetReport(ReportMsg msg)
    {
        Message data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("id", msg.user_pk);
        if (msg.message != "")
        {
            RForm.AddField("message", msg.message);
        }
        RForm.AddField("type", msg.type);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/setreport", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                yield return data.code;
            }
            else
            {
                yield return req.error;
            }
        }
    }
    public IEnumerator OnConsumeBazzarProduct(string product_id,string purchase_token)
    {
        Message data;
        WWWForm RForm = new WWWForm();
        RForm.AddField("product_id", product_id);
        RForm.AddField("purchase_token", purchase_token);
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "pay/bazzarpay", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);

                SetStatus(GDC.gameObject.status, data.message, colorWarning);
                yield return data;
            }
            else
            {
                SetStatus(GDC.gameObject.status, req.error, colorError);
                yield return req.error;
            }
        }
    }
    public IEnumerator OnGetReports()
    {
        Message data;
        WWWForm RForm = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(baseURL + "users/getreports", RForm))
        {
            yield return req.SendWebRequest();
            if (!req.isHttpError && !req.isNetworkError)
            {
                data = JsonUtility.FromJson<Message>(req.downloadHandler.text);
                if (data.code == "ok")
                {
                    yield return data.reports;
                }
                else
                {
                    OnMessageProcess(data.message);
                    yield return data.message;
                }
            }
            else
            {
                yield return req.error;
            }
        }
    }

    public void OnMessageProcess(string message)
    {
        if (message == "You are not logged in !")
        {
            StartCoroutine(OnStelthLogin());
        }
    }
}

