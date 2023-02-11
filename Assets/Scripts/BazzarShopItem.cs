using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bazaar.Poolakey.Data;
using RTLTMPro;
using System;


public class BazzarShopItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private RTLTextMeshPro titleText, descriptionText, priceText;
    private SKUDetails skuDetails;
    private PurchaseInfo purchaseInfo;
    private Action<SKUDetails> onSelect;
    private Button button;

    public BazzarShopItem Init(BazzarProduct product, Action<SKUDetails> onSelect)
    {
        this.onSelect = onSelect;
        if (IsConsumable(product.id))
        {
            iconImage.enabled = false;
        }
        else
        {
            iconImage.sprite = product.icon;
            switch (product.id)
            {
                case "1MM":
                    iconImage.color = new Color(0.584f, 0.823f, 0.918f, 1);
                    break;
                case "2MM":
                    iconImage.color = new Color(0.341f, 0.871f, 0.365f, 1);
                    break;
                case "3MM":
                    iconImage.color = new Color(0.945f, 0.733f, 0.141f, 1);
                    break;
                default:
                    break;
            }
        }
        button = GetComponent<Button>();
        button.interactable = false;
        return this;
    }
    public void CommitData(SKUDetails skuDetails, PurchaseInfo purchaseInfo)
    {
        button.interactable = false;
        this.purchaseInfo = purchaseInfo;
        this.skuDetails = skuDetails;
        titleText.text = skuDetails.title;
        string[] price = skuDetails.price.Split(' ');
        string[] pricenumber = price[0].Split(',');
        string t = pricenumber.Length == 3 ? pricenumber[2] + "," : "";
        string priceReal = t + pricenumber[1] + "," + pricenumber[0] + " " + price[1]; 
        priceText.text = priceReal;
        descriptionText.text = skuDetails.description;

        button.interactable = purchaseInfo == null;
    }
    public void OnClick()
    {
        onSelect?.Invoke(skuDetails);
    }
    private bool IsConsumable(string productId) => productId != "1MM" && productId != "2MM" && productId != "3MM";
}
