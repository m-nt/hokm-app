using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoColoring : MonoBehaviour
{
    //public float amplifier;
    public int uper, lower, x, y;
    public Text[] texts;
    public int[] array;
    //public bool reverse;
    Text target;
    TextureM background;
    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Text>();
        //Image bgtexture = GameObject.FindGameObjectWithTag("background").GetComponent<Image>();
        //background = new TextureM(bgtexture.sprite.texture, new Vector2(x, y));
        ////bgtexture.sprite = background.sprite;
        //StartCoroutine(SetColorFromTexture(target, background.data));
        SortArray sorted = new SortArray(array);
        int counter = 0;
        foreach (int item in sorted.Sort)
        {
            texts[counter].text = item.ToString();
            texts[counter].transform.parent.SetSiblingIndex(counter);
            counter++;
        }
    }
    IEnumerator SetColorFromTexture(Text target,byte[] texture)
    {
        CoroutineWithData co = new CoroutineWithData(this, FindColorFromTexture(texture));
        yield return co.coroutine;

        Color color = (Color)co.result;
        target.color = color;
    }
    IEnumerator FindColorFromTexture(byte[] data)
    {
        float expos = 0f;
        int light = 0, dark = 0,mid = 0;
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < data.Length; i ++)
        {         
            float ind = i % 3;
            float R = 0f, G = 0f, B = 0f;
            if (ind == 0 && i <= data.Length - 3)
            {
                R = data[i];
                G = data[i + 1];
                B = data[i + 2];
                float max = Mathf.Max(new float[3] { R,G,B });
                //(0.299f * R + 0.587f * G + 0.144f * B)
                //Debug.Log("["+i.ToString()+"]RGB:(" + R.ToString() + "," + G.ToString() + "," + B.ToString());
                if (max < lower)
                {
                    dark++;
                }else if (max > uper)
                {
                    light++;
                }
                else
                {
                    mid++;
                }
            }
        }

        expos = (Mathf.Min(light,dark))*1f/(Mathf.Max(light,dark)*1f);
        if (light < dark)
        {
            expos = 1;
        }
        else
        {
            expos = 0;
        }

        //float exp = (middle - expos)

        Debug.Log("Exposer:(" + expos.ToString() + ") dark:"+dark.ToString()+"---- light:"+light.ToString()+"---- mid:"+mid.ToString());
        yield return new Color(expos, expos, expos, 1);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
