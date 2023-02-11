using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class TestRenderTexure : MonoBehaviour
{
    public string baseURL = "http://localhost:8080/";
    public string number,parent;
    public Image image;
    public Vector2 size;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(OnGetTexture());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(OnGetTexture());
        }
    }
    public IEnumerator OnGetTexture()
    {
        CoroutineWithData co = new CoroutineWithData(this, GetTexture(parent, number));
        yield return co.coroutine;
        TextureM texture = new TextureM();
        string err = "";
        try
        {
            //Texture2D usersTemp = (Texture2D)co.result;
            //usersTemp.name = "UserTemp Texture";
            //usersTemp.Compress(false);
            //usersTemp.Apply();
            texture = new TextureM((byte[])co.result, size,"Converted Texure:"+number);
            
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (texture != null)
        {
            image.sprite = texture.sprite;
        }
        else
        {
            Debug.Log(err);
        }
    }
    public IEnumerator GetTexture(string type, string id)
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(baseURL + type + id + ").jpg"))
        {
            yield return req.SendWebRequest();

            if (!req.isHttpError && !req.isNetworkError)
            {

                //yield return ((DownloadHandlerTexture)req.downloadHandler).texture;
                yield return req.downloadHandler.data;
            }
            else
            {
                yield return req.error;
            }
        }

    }

}
