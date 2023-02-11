using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public RectTransform ItemToDrop,Arrow;
    public GameObject ItemToHide;
    private bool IsDroped = false;
    public bool isDroped
    {
        get { return IsDroped; }
        set { IsDroped = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDropDown(string data)
    {
        string[] dataArray = data.Split(',');

        float dropValue = float.Parse(dataArray[1]);
        float undropValue = float.Parse(dataArray[0]);
        int IsDroping = -1;
        if (dataArray.Length > 2)
        {
            IsDroping = int.Parse(dataArray[2]);
        }

        if (!IsDroped && IsDroping != 0) {
            IsDroped = true;
            ItemToDrop.sizeDelta = new Vector2(ItemToDrop.rect.width,dropValue);
            Arrow.rotation = Quaternion.Euler(0,0,270);
            ItemToHide.SetActive(true);
        }
        else
        {
            IsDroped = false;
            ItemToDrop.sizeDelta = new Vector2(ItemToDrop.rect.width, undropValue);
            Arrow.rotation = Quaternion.Euler(0, 0, 90);
            ItemToHide.SetActive(false);
        }
    }
}
