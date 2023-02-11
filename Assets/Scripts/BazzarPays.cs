using System;
using UnityEngine;
using Bazaar.Data;
using Bazaar.Poolakey;
using System.Collections;
using Bazaar.Poolakey.Data;
using System.Collections.Generic;
using UnityEngine.UI;
public class BazzarPays : MonoBehaviour
{
    //public:
    public List<BazzarProduct> products;
    public Transform parent;
    public Text debugText;
    public Authentication auth;
    public GameObject status;
    //private:    
    [SerializeField] private BazzarShopItem shopItemTemplate;
    private static string RSA = "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwCj3WJF8MuTM625IdLUzJF6AzGnUXqCC/OlSGE4vwiPJb5hSN3VRzo6z7egQr/MUwm0gdTKec3FeK8fI8QUAnQOMa2C4eT/3NZSbyiUUbXxdqQIGf3ou4sa01rUH36ftvFV9tmSn14psw3YYJ4/+FO1vZSTFgRb5lhQd4Pzgs4ebEJLZRiqdyG5X1kEzNiRlJV8EFlq1nwBKO1GU6As/mGhYiTqUld9hjawf2dBl20CAwEAAQ==";
    private Payment payment;
    private Dictionary<string, BazzarShopItem> shopItems;
    public void Start()
    {
        SecurityCheck securityCheck = SecurityCheck.Enable(RSA);
        PaymentConfiguration paymentConfiguration = new PaymentConfiguration(securityCheck);
        payment = new Payment(paymentConfiguration);
        auth = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Authentication>();
        Connect();
    }
    public void CreateShopItems()
    {
        shopItems = new Dictionary<string, BazzarShopItem>();
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
        foreach (var p in products)
        {
            shopItems.Add(p.id, Instantiate<BazzarShopItem>(shopItemTemplate, parent,false).Init(p, Purchase));
        }
        GetSkuDetails();
    }
    private async void GetSkuDetails()
    {
        var productIds = "";
        foreach (var p in products)
        {
            productIds += p.id + ",";
        }
        var result = await payment.GetSkuDetails(productIds);
        //Log(result.status.ToString(), result.message);
        if (result.status == Status.Success)
        {
            GetPurchases(result.data);
        }
    }
    private async void GetPurchases(List<SKUDetails> skuDetailsList)
    {
        var result = await payment.GetPurchases();
        //Log(result.status.ToString(), result.message);
        if (result.status == Status.Success)
        {
            var purchases = result.data;
            foreach (var skuDetails in skuDetailsList)
            {
                var purchaseInfo = purchases.Find(pi => pi.productId == skuDetails.sku);

                if (purchaseInfo != null)
                {
                    StartCoroutine(ConsumeProduct(purchaseInfo));
                }
                shopItems[skuDetails.sku].CommitData(skuDetails, purchaseInfo);
            }

        }
    }
    private async void Consume(PurchaseInfo purchaseInfo)
    {
        if (purchaseInfo == null)
        {
            //Log("You must purchase an item pefore!");
            return;
        }

        var result = await payment.Consume(purchaseInfo.purchaseToken);
        //Log(result.ToString());
        //Log(result.status.ToString(), result.message);
        //if (result.status == Status.Success)
        //{
            //Log($"=={purchaseInfo.productId}");
            //purchaseInfo.purchaseState = PurchaseInfo.State.Consumed;
            //UpdateStats(purchaseInfo);
            //GetSkuDetails();
        //}
    }
    private void UpdateStats(PurchaseInfo purchaseInfo)
    {
        // send data to server to validate purchase and update database
        //Log(purchaseInfo.purchaseState.ToString(), purchaseInfo.productId);
    }
    private async void Connect()
    {
        var result = await payment.Connect();
        //Debug.Log(result.status);
        //Log(result.status.ToString(), result.message);
        if (result.status == Status.Success)
        {
            // connection seccussful
             GetSkuDetails();
        }
    }
    private async void Purchase(SKUDetails skuDetails)
    {
        if (auth.vip.expires == "")
        {

            var result = await payment.Purchase(skuDetails.sku, skuDetails.type);
            // Log(result.ToString());
            //Log(result.status.ToString(), result.message);

            if (result.status == Status.Success)
            {
                StartCoroutine(ConsumeProduct(result.data));
            }
        }
        else if(IsConsumable(skuDetails.sku))
        {
            var result = await payment.Purchase(skuDetails.sku, skuDetails.type);
            // Log(result.ToString());
            //Log(result.status.ToString(), result.message)
            if (result.status == Status.Success)
            {
                StartCoroutine(ConsumeProduct(result.data));
            }
        }
        else
        {
            auth.SetStatus(status, "Player already has vip", auth.colorWarning);
        }

    }
    IEnumerator ConsumeProduct(PurchaseInfo info)
    {
        CoroutineWithData co = new CoroutineWithData(this, auth.OnConsumeBazzarProduct(info.productId,info.purchaseToken));
        yield return co.coroutine;
        Message message = null;
        string error = null;
        try
        {
            message = (Message)co.result;
        }
        catch (Exception e)
        {
            error = (string)co.result;
        }
        if (message != null)
        {
            if (message.code == "ok")
            {
                Consume(info);
            }
            else
            {
                auth.SetStatus(status, message.message, auth.colorWarning);
            }
        }
        else
        {
            auth.SetStatus(status,error, auth.colorError);
        }
    }
    public void Log(string status,string message)
    {
        debugText.text = debugText.text + "Status[" + status + "]: " + message + "\n";
    }

    private bool IsConsumable(string productId) => productId != "1MM" && productId != "2MM" && productId != "3MM";
    void OnApplicationQuit()
    {
        payment.Disconnect();
    }

}
[Serializable]
public class BazzarProduct
{
    public string id;
    public Sprite icon;
}