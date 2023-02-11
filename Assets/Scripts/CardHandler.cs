using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CardHandler : MonoBehaviour
{
    public Card[] card;
    public CardDecks deck;
    public GameObject cardPrefab;
    public Transform place;
    public Transform center;
    public float pickUpScale, offsetFactor,rotationFactor,curveFactor;
    public int border;
    public GameController gameCtrl;

    private bool enableTouch;
    private int index;
    // Start is called before the first frame update
    void Start()
    {
        //GenerateCards(stage.playerCards.P0);
    }
    public bool IsCard(int cd)
    {
        for (int i = 0; i < card.Length; i++)
        {
            if (card[i].id == cd)
            {
                return true;
            }
        }
        return false;
    }
    public void GenerateCards(int[] cards)
    {
        int startPoint = -1;
        for (int i = 0; i < card.Length; i++)
        {
            if (card[i].id == 0)
            {
                startPoint = i;
                break;
            }
        }
        if (startPoint != -1)
        {
            for (int i = startPoint; i < cards.Length + startPoint; i++)
            {
                int j = i;
                GameObject obj;
                obj = Instantiate(cardPrefab, this.transform, false);
                card[i].init(this.transform,
                    obj.GetComponent<RectTransform>(),
                    obj.GetComponent<Image>(),
                    cards[i - startPoint],
                    deck);
                EventTrigger trigger = obj.GetComponent<EventTrigger>();
                EventTrigger.Entry entry1 = new EventTrigger.Entry();
                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry1.eventID = EventTriggerType.PointerDown;
                entry1.callback.AddListener((data) => { OnImageTouched(j); });
                entry2.eventID = EventTriggerType.PointerUp;
                entry2.callback.AddListener((data) => { OnImageUntouched(); });
                trigger.triggers.Add(entry1);
                trigger.triggers.Add(entry2);
            }
            SetCards();
        }
    }
    public Card getCardsByIndex(int index)
    {
        foreach (Card item in card)
        {
            if (index == item.id)
            {
                return item;
            }
        }
        return null;
    }
    public void SetCards()
    {
        SortArray sorted = new SortArray(gameCtrl.sockCon.stage.playerCards.GetCard(gameCtrl.sockCon.seq[0]));
        int[] sortedindexs = sorted.Sort;
        //int counter = 0;
        for (int i = 0;i < sortedindexs.Length; i++)
        {
            Card item = getCardsByIndex(sortedindexs[i]);
            if (item != null)
            {
                int number = this.transform.childCount;
                float offset = (Mathf.Pow(number-13,2)/offsetFactor)+offsetFactor;
                float x = (center.position.x - ((number-1)/2)*offset) + offset * i;
                float y = (-1 * (Mathf.Pow(x - center.position.x, 2) / curveFactor)) + center.position.y;
                float Rz = (-(((number - 1f) / 2f) * (offset* rotationFactor)) + (offset* rotationFactor) * i);
                item.image.gameObject.transform.SetSiblingIndex(i);  
                item.transform.position = new Vector3(x, y, item.transform.position.z);
                item.transform.rotation = Quaternion.Euler(0,0,Rz);
            }
            //counter++;
        }
    }
    public void OnImageTouched(int indx)
    {
        index = indx;
        enableTouch = true;
    }
    public void OnImageUntouched()
    {
        enableTouch = false;
        card[index].transform.localScale = new Vector3(1, 1, 1);

        if (Input.mousePosition.y > border
            && gameCtrl.sockCon.seq[0] == gameCtrl.sockCon.stage.nextPlayer
            && (DetectType(gameCtrl.sockCon.stage.hand)?card[index].CardType == gameCtrl.sockCon.stage.hand
            || gameCtrl.sockCon.stage.nextPlayer == gameCtrl.sockCon.stage.playedPlayer:true)
            && gameCtrl.sockCon.stage.GetStageNumber == 5)
        {
            // this is where you send a message to server for player move
            //DestroyChildes(place);
            gameCtrl.sockCon.OnSetStage(new SocketStageMsg(int.Parse(gameCtrl.sockCon.opponents.users[0].number),card[index].id));
            if (gameCtrl.sockCon.stage.nextPlayer == 3)
            {
                gameCtrl.lastPlayer = 0;
            }
            else
            {
            gameCtrl.lastPlayer = gameCtrl.sockCon.stage.nextPlayer + 1;
            }
            Camera.main.GetComponent<AudioSource>().Play();
            card[index].transform.parent = place;
            card[index].SetZero();
            card[index].transform = null;
        }
        SetCards();
    }
    public bool DetectType(int type)
    {
        for (int i = 0; i < card.Length; i++)
        {
            if (card[i].CardType == type)
            {
                return true;
            }
        }
        return false;
    }
    public void OnCardPlayed(int cd)
    {
        //DestroyChildes(place);
        // this is where you send a message to server for player move
        for (int i = 0; i < card.Length; i++)
        {
            if (card[i].id == cd)
            {
                index = i;
                break;
            }
        }
        int sn = gameCtrl.sockCon.stage.GetStageNumber;
        if (sn == 5)
        {
            card[index].transform.parent = place;
            card[index].SetZero();
            card[index].transform = null;
        }
    }
    public void DestroyChildes(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

    }
    public void DestroyWithAnimation(Animator parent,int id)
    {
        parent.SetInteger("id", id);
    }
    public void SetZero()
    {
        foreach (Card item in card)
        {
            item.Destroy();
        }
    }
    // Update is called once per frame
    void Update()
    {
       
        if (enableTouch)
        {
            card[index].transform.localScale = new Vector3(pickUpScale, pickUpScale, 1);
            card[index].transform.position = Input.mousePosition;
        }

    }

}
[Serializable]
public class Card
{
    public Transform parent;
    public RectTransform transform;
    public Image image;
    public int id;

    public void init(Transform prnt,RectTransform trns,Image img,int _id,CardDecks deck)
    {
        id = _id;
        parent = prnt;
        transform = trns;
        image = img;
        image.sprite = deck.GetTexture(id) != null ? deck.GetTexture(id):img.sprite;
    }
    public void init(int _id,Transform prnt, RectTransform trns, Image img)
    {
        id = _id;
        parent = prnt;
        transform = trns;
        image = img;
    }

    public void setDeck(CardDecks deck)
    {
        image.sprite = deck.GetTexture(id);
    }
    public void SetZero()
    {
        transform.GetComponent<EventTrigger>().enabled = false;
        transform.offsetMax = Vector2.zero;
        transform.offsetMin = Vector2.zero;
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;

        id = 0;
        parent = null;
        image = null;

    }
    public void Destroy()
    {
        id = 0;
        parent = null;
        transform = null;
        image = null;
    }
    public int CardType
    {
        get { 
            if (id > 400)
            {
                return 400;
            }
            else if (id > 300)
            {
                return 300;
            }
            else if (id > 200)
            {
                return 200;
            }
            else if (id > 100)
            {
                return 100;
            }
            else
            {
                return 0;
            }
        }
    }
    public int index
    {
        get
        {
            if ((id - 400) < 100 && (id - 400) > 0)
            {
                return id - 400;
            }
            else if ((id - 300) < 100 && (id - 300) > 0)
            {
                return id - 300;
            }
            else if ((id - 200) < 100 && (id - 200) > 0)
            {
                return id - 200;
            }
            else
            {
                return id - 100;
            }
        }
    }
}
[Serializable]
public class CardDecks
{
    public enum Deck
    {
        one,
        two,
        three,
        four,
        five,
        six
    }
    public Deck deck;
    public Sprite[] deck0,deck1,deck2,deck3,deck4,deck5;

    public void ChangeDeck(string deckID)
    {
        switch (deckID)
        {
            case "0":
                deck = Deck.one;
                break;
            case "1":
                deck = Deck.two;
                break;
            case "2":
                deck = Deck.three;
                break;
            case "3":
                deck = Deck.four;
                break;
            case "4":
                deck = Deck.five;
                break;
            case "5":
                deck = Deck.six;
                break;
            default:
                deck = Deck.one;
                break;
        }
    }
    public Sprite GetTexture(int id)
    {
        int index = GetIndex(id);
        switch (deck)
        {
            case Deck.one:
                return deck0[index];
            case Deck.two:
                return deck1[index];
            case Deck.three:
                return deck2[index];
            case Deck.four:
                return deck3[index];
            case Deck.five:
                return deck4[index];
            case Deck.six:
                return deck5[index];
            default:
                return null;
        }
    }
    public int GetIndex(int cn)
    {
        if (cn > 400)
        {
            return cn - 363;
        }else if (cn > 300)
        {
            return cn - 276;
        }else if (cn > 200)
        {
            return cn - 189;
        }else
        {
            return cn - 102;
        }
    }
}