using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AuthObject : MonoBehaviour
{
    public AuthDataObject authobj;
    public Authentication auth;
    public ImageSeries avatar;
    public Text TestText;
    // Start is called before the first frame update
    public void Awake()
    {
        
    }
    void Start()
    {
        auth = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Authentication>();
        auth.authObject = this.GetComponent<AuthObject>();
        auth.SetDeviceInfo = SystemInfo.deviceModel + "[div]"
                        + SystemInfo.deviceName + "[div]"
                        + SystemInfo.deviceType + "[div]"
                        + SystemInfo.deviceUniqueIdentifier + "[div]"
                        + SystemInfo.operatingSystem;
        auth.user.Avatar.data = new TextureM(authobj.avatarRef.sprite.texture, new Vector2(200, 200));
        authobj.init(auth);
        //StartCoroutine(auth.OnGetURLs());
        //StartCoroutine(auth.SockConn.OnGetURLs());
        PlayerPrefs.SetString("cashedAvatar", "false");
        auth.OnCheckLoggin();
        //TestText.text = Application.persistentDataPath;

    }
    public void OnOpenGallery()
    {
#if UNITY_ANDROID

        if (!NativeGallery.IsMediaPickerBusy())
        {
            NativeGallery.GetImageFromGallery((path)=> {
                Debug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    TextureM texture = new TextureM(NativeGallery.LoadImageAtPath(path, 200),new Vector2(200,200));
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }
                    else
                    {
                        //set the avatar image 
                        authobj.avatarRef.sprite = texture.sprite;
                        auth.user.Avatar.data = texture;
                    }
                }
            }, "Avatar", "image/*");
        }
        
#endif
    }
    public void OnOpenAvatarSection()
    {
        if (PlayerPrefs.HasKey("cashedAvatar"))
        {
            if (PlayerPrefs.GetString("cashedAvatar") == "false")
            {
                avatar.Destroy();
                for (int i = 1; i <= avatar.count; i++)
                {
                    StartCoroutine(OnGetAvatarTexure(i));
                }

            }
        }
        else
        {
            avatar.Destroy();
            for (int i = 1; i <= avatar.count; i++)
            {
                StartCoroutine(OnGetAvatarTexure(i));
            }
        }
    }
    IEnumerator OnGetAvatarTexure(int id)
    {
        CoroutineWithData co = new CoroutineWithData(this, auth.GetTexture(avatar.presetURI, id.ToString()));
        yield return co.coroutine;
        string result = co.result.ToString();
        TextureM texture = new TextureM();
        string err = "";
        try
        {
            Texture2D usersTemp = (Texture2D)co.result;
            texture = new TextureM(usersTemp, new Vector2(200, 200));
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (texture != null)
        {
            GameObject obj = Instantiate(avatar.prefab, avatar.parent, false);
            obj.name = "Avatar image(" + id.ToString() + ")";
            obj.GetComponent<Image>().sprite = texture.sprite;
            obj.GetComponent<Button>().onClick.AddListener(delegate { auth.OnAvatar(texture); });
            obj.GetComponent<Button>().onClick.AddListener(delegate { auth.OnImageSetAvatar(avatar.image,texture.sprite); });
            if (id == avatar.count)
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

    // Update is called once per frame
    void Update()
    {
    }
}
[Serializable]
public class AuthDataObject
{
    public GameObject status,loading;
    public Image avatarRef;
    public RegisterObject register;
    public LogginObject loggin;

    public void init(Authentication auth)
    {
        register.acceptAndEnter.onClick.AddListener(auth.OnRegister);
        register.Username.onEndEdit.AddListener(delegate{ auth.OnUsername(register.Username); });
        register.Password.onEndEdit.AddListener(delegate { auth.OnPassword(register.Password); });
        register.Cpassword.onEndEdit.AddListener(delegate { auth.OnCPassword(register.Cpassword); });
        loggin.acceptAndEnter.onClick.AddListener(auth.OnLoggin);
        loggin.Username.onEndEdit.AddListener(delegate { auth.OnUsername(loggin.Username); });
        loggin.Password.onEndEdit.AddListener(delegate { auth.OnPassword(loggin.Password); });

    }

    public void SwitchPage()
    {
        loggin.parent.GetComponent<Animator>().SetTrigger("Go");
        register.parent.GetComponent<Animator>().SetTrigger("Go");
    }
}
[Serializable]
public class RegisterObject
{
    public Transform parent;
    public Button acceptAndEnter,loggin;
    public InputField Username, Password, Cpassword;
    public Animator buttonAnim;

    public void ButtonPressed()
    {
        buttonAnim.SetBool("start", true);
        acceptAndEnter.interactable = false;
    }
    public void ButtonReleased()
    {
        buttonAnim.SetBool("start", false);
        acceptAndEnter.interactable = true;
    }
}
[Serializable]
public class LogginObject
{
    public Transform parent;
    public Button acceptAndEnter,register;
    public InputField Username, Password;
    public Animator buttonAnim;
    public void ButtonPressed()
    {
        buttonAnim.SetBool("start", true);
        acceptAndEnter.interactable = false;
    }
    public void ButtonReleased()
    {
        buttonAnim.SetBool("start", false);
        acceptAndEnter.interactable = true;
    }
}
