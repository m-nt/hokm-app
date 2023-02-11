using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class CurencyItem
{
    public string name;
    public int amount;
}
[Serializable]
public class SettingObject
{
    public Menu menu;
    public Profile profile;
    public Cards cards;
    public Backgrounds background;
    public LeaderBoard leaderboard;
    public VIPObject vip;
    public VIPbadge vipBadge;
    public About about;
    public GameObject status,AdRemove;
    public Lobby random, lobby;
    public SettingObject()
    {

    }

    public void resetHome()
    {
        menu.parent.gameObject.SetActive(true);
        profile.parent.gameObject.SetActive(false);
        leaderboard.parent.gameObject.SetActive(false);
        about.parent.gameObject.SetActive(false);
    }
    public void OnDeSetBackgroundAspect()
    {
        menu.background.type = Image.Type.Simple;
        profile.background.type = Image.Type.Simple;
        leaderboard.background.type = Image.Type.Simple;
        vip.background.type = Image.Type.Simple;
        about.background.type = Image.Type.Simple;
        lobby.background.type = Image.Type.Simple;
        random.background.type = Image.Type.Simple;
    }
    public void OnSetBackgroundAspect()
    {
        menu.background.type = Image.Type.Tiled;
        menu.background.pixelsPerUnitMultiplier = 0.8f;
        profile.background.type = Image.Type.Tiled;
        profile.background.pixelsPerUnitMultiplier = 0.8f;
        leaderboard.background.type = Image.Type.Tiled;
        leaderboard.background.pixelsPerUnitMultiplier = 0.8f;
        vip.background.type = Image.Type.Tiled;
        vip.background.pixelsPerUnitMultiplier = 0.8f;
        about.background.type = Image.Type.Tiled;
        about.background.pixelsPerUnitMultiplier = 0.8f;
        lobby.background.type = Image.Type.Tiled;
        lobby.background.pixelsPerUnitMultiplier = 0.8f;
        random.background.type = Image.Type.Tiled;
        random.background.pixelsPerUnitMultiplier = 0.8f;

    }
    public void setBackgrounds()
    {
        int index = int.Parse(background.index);
        if (index < 50 )
        {
            menu.background.sprite = background.texure.sprite;
            profile.background.sprite = background.texure.sprite;
            leaderboard.background.sprite = background.texure.sprite;
            vip.background.sprite = background.texure.sprite;
            about.background.sprite = background.texure.sprite;
            random.background.sprite = background.texure.sprite;
            lobby.background.sprite = background.texure.sprite;
            OnSetBackgroundAspect();
        }
        else
        {
            menu.background.sprite = menu.defback;
            profile.background.sprite = profile.defback;
            leaderboard.background.sprite = leaderboard.defback;
            vip.background.sprite = vip.defback;
            about.background.sprite = about.defback;
            random.background.sprite = random.defback;
            lobby.background.sprite = lobby.defback;
            OnDeSetBackgroundAspect();
        }
    }
    public void setBackgrounds(Sprite sprite)
    {
        int index = int.Parse(background.index);
        if (index < 50)
        {
            menu.background.sprite = sprite;
            profile.background.sprite = sprite;
            leaderboard.background.sprite = sprite;
            vip.background.sprite = sprite;
            about.background.sprite = sprite;
            random.background.sprite = sprite;
            lobby.background.sprite  = sprite;

        }
        else
        {
            menu.background.sprite = menu.defback;
            profile.background.sprite = profile.defback;
            leaderboard.background.sprite = leaderboard.defback;
            vip.background.sprite = vip.defback;
            about.background.sprite = about.defback;
            random.background.sprite = random.defback;
            lobby.background.sprite = lobby.defback;

        }
    }

}
[Serializable]
public class BackGround
{
    public Backgrounds background;
    public Image[] images;
    public Sprite[] defaults;

    public void OnSetBackgroundAspect()
    {
        foreach (Image item in images)
        {
            item.type = Image.Type.Tiled;
            item.pixelsPerUnitMultiplier = 0.8f;
        }
    }
    public void OnDeSetBackgroundAspect()
    {
        foreach (Image item in images)
        {
            item.type = Image.Type.Simple;
        }
    }

    public void setBackground()
    {
        if (int.Parse(background.index) < 50)
        {
            foreach (Image item in images)
            {
                item.sprite = background.texure.sprite;
            }
            OnSetBackgroundAspect();
        }
        else
        {
            for (int i = 0; i < images.Length;i++)
            {
                images[i].sprite = defaults[i];
            }
            OnDeSetBackgroundAspect();
        }
    }
}
[Serializable]
public class Lobby
{
    public Transform parent;
    public Button ready, leave;
    public Text leaveLobby;
    public Image background;
    public Sprite defback;
}
[Serializable]
public class Menu
{
    public Image background;
    public Sprite defback;
    public Transform parent;
    public Image avatar;
    public Text name;
    public Text curency;
    public GameObject loading;

    public void init(User user)
    {
        avatar.sprite = user.Avatar.data.sprite;
        name.text = user.Username;
        curency.text = user.Curency;
    }
}
[Serializable]
public class VIPbadge
{
    public enum VIPtype
    {
        NonVIP,
        OneMonth,
        TwoMonth,
        ThreeMonth
    }
    public VIP obj;
    public bool isVIP;
    public Image badge;
    public Text expire;
    public VIPtype type;
    public Color noVIP,oneM, twoM, threeM;
    public void init(VIP vip)
    {
        obj = new VIP();
        obj = vip;
        obj.StringToDate();
        expire.text = Math.Floor((obj.expire - DateTime.UtcNow).TotalDays).ToString();
        isVIP = true;
        switch (vip.name)
        {
            case "1M":
                type = VIPtype.OneMonth;
                badge.color = oneM;
                break;
            case "2M":
                type = VIPtype.TwoMonth;
                badge.color = twoM;
                break;
            case "3M":
                type = VIPtype.ThreeMonth;
                badge.color = threeM;
                break;
            default:
                break;
        }
    }
    public void destroy()
    {
        obj.Destroy();
        isVIP = false;
        badge.color = noVIP;
        expire.text = "0";
        type = VIPtype.NonVIP;
    }
}
[Serializable]
public class VIPObject
{
    public Image background;
    public Sprite defback;
    public Transform parent;
    public GameObject[] items;
}
[Serializable]
public class About
{
    public Image background;
    public Sprite defback;
    public Transform parent;
}
[Serializable]
public class Profile
{
    public Image background;
    public Sprite defback;
    public Transform parent;
    public Image _avatar;
    public Image little_avatar;
    public Text username,password;
    public TextureM avatar;
    public Button acceptAndEnter;
    
    public void init(User user)
    {
        TextureM little = new TextureM(user.Avatar.data.texture, new Vector2(50, 50));
        _avatar.sprite = user.Avatar.data.sprite;
        little_avatar.sprite = little.sprite;
        username.text = user.Username;
        //avatar = new TextureM(user.Avatar.texture, new Vector2(200, 200));
        acceptAndEnter.interactable = true;
    }
    public void ButtonPressed()
    {
        acceptAndEnter.interactable = false;
    }
    public void ButtonReleased()
    {
        acceptAndEnter.interactable = true;
    }
    public void setAvatar()
    {
        avatar = new TextureM(_avatar.sprite.texture,new Vector2(200,200),false);
    }
}
[Serializable]
public class Cards
{

    public int selectedCardIndex;
    public bool newCard;
    public GameObject[] cards;

    public void selectNewCard(int index)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(false);
        }
        cards[index].SetActive(true);
        selectedCardIndex = index;
        newCard = true;
    }
    public void Destroy()
    {
        newCard = false;
    }
}
[Serializable]
public class Backgrounds
{
    public TextureM texure;
    public string index;

    private bool Iscashed = false;
    public bool IsCashed
    {
        get { return Iscashed; }
        set { Iscashed = value; }
    }

    public void init(Texture2D texture,string inx)
    {
        texure = new TextureM(texture,new Vector2(1080,1920));
        index = inx;
    }
    public void Destroy()
    {
        texure = null;
        index = "404";
    }
}
[Serializable]
public class LeaderBoard
{
    public Image background;
    public int limit;
    public Sprite defback;
    public Transform parent;
    public Transform root;
    public GameObject playerPrefab,loading;
    public GameObject[] players;

    public GameObject initialize(GameObject playerInstance,User playerData,string number)
    {
        GameObject player = playerInstance;
        player.transform.GetChild(0).GetComponent<Text>().text = playerData.Username;
        player.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = playerData.Curency;
        player.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = number;
        player.transform.GetChild(2).GetComponent<Image>().sprite = playerData.Avatar.data.sprite;
        return player;
    }
}
[Serializable]
public class Curency
{
    public int amount;
    public bool newCurency;
    public int newAmount;
    public void init(int N)
    {
        newAmount = amount + N;
        newCurency = true;
    }
    public void init(User user)
    {
        amount = int.Parse(user.Curency);
        newCurency = false;
        newAmount = 0;
    }
    public void Destroy()
    {
        newAmount = 0;
        newCurency = false;
    }
}
[Serializable]
public class PurchaseReqBody
{
    public string merchant_id, amount, description, callback_url;
    public PurchaseReqBody(string MerchID, string Amount,string Desc, string CallBk)
    {
        merchant_id = MerchID;
        amount = Amount;
        description = Desc;
        callback_url = CallBk;
    }
}
[Serializable]
public class ReqPurchMessage
{
    public ReqPurchDataMessage data;
    public string error;
    public string message;
    public string code;
}
[Serializable]
public class ReqPurchDataMessage
{
   public int code;
   public string message;
   public string authority;
   public string fee_type;
   public int fee;
}