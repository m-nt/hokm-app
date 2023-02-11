using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OtherPlayerCardHandler : MonoBehaviour
{
    public enum Side
    {
        right,
        left,
        top
    }
    public Card[] card;
    public CardHandler handle;
    public Transform place;
    public Transform center;
    public GameObject cardPrefab;
    public Side side;
    public float pickUpScale, offsetFactor, rotationFactor, curveFactor;
    public int border;
    

    private int index;
    // Start is called before the first frame update
    void Start()
    {
        //GenerateCards(handle.gameCtrl.sockCon.stage.playerCards.P1);
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
                card[i].init(cards[i - startPoint], this.transform,
                    obj.GetComponent<RectTransform>(), obj.GetComponent<Image>());
                
            }
            SetCards();
        }
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnCardActionMsg(106);

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnCardActionInit(106);
        }
    }
    public int GetIndex(Card[] array,int id)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].id == id)
            {
                return i;
            }

        }
        return -1;
    }
    public void OnCardActionMsg(int id)
    {
        int index = handle.gameCtrl.sockCon.stage.GetIndex(handle.gameCtrl.sockCon.stage.playerCards.P1, id);
        card[index].transform.localScale = new Vector3(pickUpScale,pickUpScale,1);
    }
    public void OnCardActionInit(int id)
    {
        int index = GetIndex(card, id);
        if (index != -1)
        {

            card[index].transform.localScale = new Vector3(1, 1, 1);
            if (place.childCount > 0)
            {
                for (int i = 0; i < place.childCount; i++)
                {
                    Destroy(place.GetChild(0).gameObject);
                }
            }
            card[index].setDeck(handle.deck);
            card[index].transform.parent = place;
            card[index].SetZero();
            card[index].transform = null;
        }
        SetCards();
    }
    public void SetCards()
    {
        switch (side)
        {
            case Side.right:
                SetCardRight();
                break;
            case Side.left:
                SetCardLeft();
                break;
            case Side.top:
                SetCardTop();
                break;
        }
    }
    public void SetCardRight()
    {
        int counter = 0;
        foreach (Card item in card)
        {
            if (item.transform != null)
            {
                int number = this.transform.childCount;
                float offset = (Mathf.Pow(number - 13, 2) / offsetFactor) + offsetFactor;
                float y = (center.position.y - ((number - 1) / 2) * offset) + offset * counter;
                float x = ((Mathf.Pow(y - center.position.y, 2) / curveFactor)) + center.position.x;
                float Rz = 90+(-(((number - 1f) / 2f) * (offset * rotationFactor)) + (offset * rotationFactor) * counter);
                item.transform.position = new Vector3(x, y, item.transform.position.z);
                item.transform.rotation = Quaternion.Euler(0, 0,Rz);
                counter++;
            }
        }
    }
    public void SetCardLeft()
    {
        int counter = 0;
        foreach (Card item in card)
        {
            if (item.transform != null)
            {
                int number = this.transform.childCount;
                float offset = (Mathf.Pow(number - 13, 2) / offsetFactor) + offsetFactor;
                float y = (center.position.y - ((number - 1) / 2) * offset) + offset * counter;
                float x = (-1*(Mathf.Pow(y - center.position.y, 2) / curveFactor)) + center.position.x;
                float Rz = 90-(-(((number - 1f) / 2f) * (offset * rotationFactor)) + (offset * rotationFactor) * counter);
                item.transform.position = new Vector3(x, y, item.transform.position.z);
                item.transform.rotation = Quaternion.Euler(0, 0, Rz);
                counter++;
            }
        }
    }
    public void SetCardTop()
    {
        int counter = 0;
        foreach (Card item in card)
        {
            if (item.transform != null)
            {
                int number = this.transform.childCount;
                float offset = (Mathf.Pow(number - 13, 2) / offsetFactor) + offsetFactor;
                float x = (center.position.x - ((number - 1) / 2) * offset) + offset * counter;
                float y = ((Mathf.Pow(x - center.position.x, 2) / curveFactor)) + center.position.y;
                float Rz = (-(((number - 1f) / 2f) * (offset * rotationFactor)) + (offset * rotationFactor) * counter);
                item.transform.position = new Vector3(x, y, item.transform.position.z);
                item.transform.rotation = Quaternion.Euler(0, 0, Rz);
                counter++;
            }
        }
    }
}
