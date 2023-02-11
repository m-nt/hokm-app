using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextCounter : MonoBehaviour
{
    public Text text;
    public int count;
    private void OnEnable()
    {
        StartCoroutine(Counter());
    }
    IEnumerator Counter()
    {
        while (count >= 0)
        {
            yield return new WaitForSeconds(1);
            text.text = count.ToString();
            count--;
        }
    }
}
