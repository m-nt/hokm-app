using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarsiFixer : MonoBehaviour
{
    private Text _text;
    private string TextHolder;
    public void Start()
    {
        _text = this.gameObject.GetComponent<Text>();
    }
    public void OnTextEdit()
    {
        if (_text != null)
        {
            TextHolder = _text.text;
            TextHolder = TextHolder.faConvert();
            _text.text = TextHolder;
            Debug.Log(TextHolder);
        }

    }
}
