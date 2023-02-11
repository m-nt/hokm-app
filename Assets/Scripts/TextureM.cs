using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

[Serializable]
public class EncodeType
{
    public enum encodeType
    {
        PNG,
        JPG,
        EXR,
        TGA
    }

}
[Serializable]
public class AvatarData
{
    public TextureM data;
    public string contentType;
}
[Serializable]
public class TextureM
{
    public Texture2D texture;
    public Sprite sprite;
    public string type;
    public byte[] data;
    public void Destroy()
    {
        texture = null;
        sprite = null;
        type = "";
        data = new byte[0];
    }
    public TextureM()
    {
    }
    public TextureM(byte[] Data,Vector2 size)
    {
        data = ResizeImage(Data,size);
        texture = CreateTexture(data);
        sprite = CreateSprite(texture);
    }
    public TextureM(byte[] Data)
    {
        data = Data;
        texture = CreateTexture(data);
        sprite = CreateSprite(texture);
    }

    public TextureM(byte[] Data, Vector2 size,string name)
    {
        data = ResizeImage(Data, size,name);
        texture = CreateTexture(data);
        sprite = CreateSprite(texture);
    }
    public TextureM(Texture2D textre, Vector2 size)
    {
        texture = Resize(textre, Convert.ToInt32(size.x), Convert.ToInt32(size.y));
        data = ConvertToByte(texture);
        sprite = CreateSprite(texture);
    }
    public TextureM(Texture2D textre, Vector2 size,bool compress)
    {
        if (compress)
        {
            texture = Resize(textre, Convert.ToInt32(size.x), Convert.ToInt32(size.y));
        }
        else
        {
            texture = ResizeUnCompresed(textre, Convert.ToInt32(size.x), Convert.ToInt32(size.y));
        }
        data = ConvertToByte(texture);
        sprite = CreateSprite(texture);
    }

    public TextureM(Texture2D textre, Vector2 size,string name)
    {
        texture = Resize(textre, Convert.ToInt32(size.x), Convert.ToInt32(size.y));
        texture.name = name;
        data = ConvertToByte(texture);
        sprite = CreateSprite(texture);
    }
    public TextureM(Texture2D textre, string name,EncodeType.encodeType type)
    {
        texture = textre;
        texture.name = name;
        data = ConvertToByte(texture,type);
        sprite = CreateSprite(texture);
    }
    public TextureM(Texture2D textre, int x,int y)
    {
        texture = Resize(textre, x, y);
        data = ConvertToByte(texture);
        sprite = CreateSprite(texture);
    }
    Sprite CreateSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0,0,texture.width,texture.height), new Vector2(0, 0));
    }


    byte[] ResizeImage(byte[] data, Vector2 size)
    {
        Texture2D t = new Texture2D(1,1);
        t.LoadImage(data);
        t = ResizeUnCompresed(t, Convert.ToInt32(size.x), Convert.ToInt32(size.y));
        return t.EncodeToPNG();
    }
    byte[] ResizeImage(byte[] data, Vector2 size,string name)
    {
        Texture2D t = new Texture2D(1, 1);
        t.name = name;
        t.LoadImage(data);
        t = ResizeUnCompresed(t, Convert.ToInt32(size.x), Convert.ToInt32(size.y));
        return t.EncodeToPNG();
    }

    byte[] ConvertToByte(Texture2D texture)
    {
        Texture2D tx = ResizeUnCompresed(texture, texture.width, texture.height);
        return tx.EncodeToPNG();
    }
    byte[] ConvertToByte(Texture2D texture, EncodeType.encodeType type)
    {
        Texture2D tx = ResizeUnCompresed(texture, texture.width, texture.height);
        switch (type)
        {
            case EncodeType.encodeType.PNG:
                return tx.EncodeToPNG();
            case EncodeType.encodeType.JPG:
                return tx.EncodeToJPG();
            case EncodeType.encodeType.EXR:
                return tx.EncodeToEXR();
            case EncodeType.encodeType.TGA:
                return tx.EncodeToTGA();
            default:
                return tx.EncodeToPNG();
        }
    }
    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = RenderTexture.GetTemporary(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Compress(false);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }
    Texture2D ResizeUnCompresed(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = RenderTexture.GetTemporary(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        //result.Compress(false);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }
    Texture2D CreateTexture(byte[] data)
    {
        Texture2D t = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        t.LoadImage(data);
        t.Compress(false);
        return t;
    }

}
