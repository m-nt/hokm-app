using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyCat : MonoBehaviour
{
    public Text copy;
    public float delay = 0.2f;
    public bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        active = false;
    }
    IEnumerator Loop()
    {
        while (active)
        {
            yield return new WaitForSeconds(delay);
            this.GetComponent<Text>().text = copy.text;
        }
    }
}
