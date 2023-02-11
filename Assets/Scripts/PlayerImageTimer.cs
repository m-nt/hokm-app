using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerImageTimer : MonoBehaviour
{
    public Image image;
    [Range(0.001f,0.9f)]
    public float amount;
    public float wait;
    // Start is called before the first frame update
    public void OnStart(int timeout)
    {
        wait = timeout;
        StartCoroutine(OnImageFill());
        
    }
    public void OnStop()
    {
        image.enabled = false;
        image.fillAmount = 1;
        StopAllCoroutines();
    }
    IEnumerator OnImageFill()
    {
        image.enabled = true;
        image.fillAmount = 1;
        while (image.fillAmount > 0)
        {
            image.fillAmount -= amount;
            yield return new WaitForSeconds(wait*amount);
        }
        image.enabled = false;
        image.fillAmount = 1;
    }
}
