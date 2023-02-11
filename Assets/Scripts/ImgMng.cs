using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public static class ImgMng
{

    public static bool save(string filename,Texture2D texture)
    {
        if (!filename.ToLower().EndsWith(".jpg"))
        {
            filename += ".jpg";
        }
#if UNITY_STANDALONE || UNITY_EDITOR
		var filepath = Path.Combine(Application.dataPath, filename);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        var filepath = Application.persistentDataPath + filename;
#endif
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        try
        {
            File.WriteAllBytes(filepath, texture.EncodeToJPG());
            return true;
        }
        catch (Exception e)
        {

            return false;
        }
    }
    public static bool save(string filename, byte[] data)
    {
        if (!filename.ToLower().EndsWith(".jpg"))
        {
            filename += ".jpg";
        }
#if UNITY_STANDALONE || UNITY_EDITOR
        var filepath = Path.Combine(Application.dataPath, filename);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        var filepath = Application.persistentDataPath + filename;
#endif
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        try
        {
            File.WriteAllBytes(filepath, data);
            return true;
        }
        catch (Exception e)
        {

            return false;
        }
    }

    public static Texture2D loadTexture(string filename)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        bool dataExist = File.Exists(Application.dataPath + filename);
        if (dataExist)
        {
            byte[] data = File.ReadAllBytes(Application.dataPath + filename);
            TextureM texture = new TextureM(data);
            return texture.texture;
        }

#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        bool dataExist = File.Exists(Application.persistentDataPath + filename);
        if (dataExist)
        {
            byte[] data = File.ReadAllBytes(Application.persistentDataPath + filename);
            TextureM texture = new TextureM(data);
            return texture.texture;
        }
#endif
        return null;
    }
    public static byte[] loadByte(string filename)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        bool dataExist = File.Exists(Application.dataPath + filename);
        if (dataExist)
        {
            byte[] data = File.ReadAllBytes(Application.dataPath + filename);
            return data;
        }

#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        bool dataExist = File.Exists(Application.persistentDataPath + filename);
        if (dataExist)
        {
            byte[] data = File.ReadAllBytes(Application.persistentDataPath + filename);
            return data;
        }
#endif
        return null;
    }

}
