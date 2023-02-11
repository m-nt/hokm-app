using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Farsiinput : MonoBehaviour
{
    public InputField input;
    public Text text;
    public GameController GC;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputField>();
    }
    public void CallTheFunction(string function)
    {
        Invoke(function, 0f);
    }
    public void Chat()
    {
        GC.OnSendChat(input.text.faConvert());
    }
    public void Report()
    {
        GC.OnReportSubmit(input.text.faConvert());
    }

    // Update is called once per frame
    void Update()
    {
        text.text = input.text.faConvert();
    }
}
