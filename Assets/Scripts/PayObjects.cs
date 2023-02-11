using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PayObjects : MonoBehaviour
{
    public enum Type
    {
        VIP,
        CurencyItem,
    }
    public Type type;
    [SerializeField]
    private VIP vip;
    [SerializeField]
    private CurencyItem curency;

    public object GetData(Type _type)
    {
        if (_type == type)
        {
            return GetVIP;
        }
        else
        {
            return null;
        }
    }

    public VIP GetVIP
    {
        get { return vip; } 
    }
    public CurencyItem GetCurency
    {
        get { return curency; }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
