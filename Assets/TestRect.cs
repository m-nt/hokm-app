using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRect : MonoBehaviour
{
    public RectTransform parent;
    public Text text;
    public InputField input;
    public int Ypos;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = Screen.height.ToString();
    }
    public void OnInput()
    {
        StartCoroutine(OnDelay());
    }
    public IEnumerator OnDelay()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (KeyboardHeight.height > 0)
            {
                parent.anchoredPosition = new Vector2(parent.anchoredPosition.x, KeyboardHeight.height);
                break;
            }
        }
    }
    public void OnInputBack()
    {
        parent.anchoredPosition = new Vector2(parent.anchoredPosition.x,0);
    }
    public void OnDelayLayout(ContentSizeFitter csf)
    {
        StartCoroutine(DeleyInputLayout(csf));
    }
    public IEnumerator DeleyInputLayout(ContentSizeFitter csf)
    {
        csf.enabled = false;
        yield return new WaitForSeconds(0.1f);
        csf.enabled = true;
    }

}
