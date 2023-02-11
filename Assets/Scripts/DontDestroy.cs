using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    static bool IsUndestroid = true;
	// Use this for initialization
	void Start () {
        if (IsUndestroid)
        {
            DontDestroyOnLoad(this);
            IsUndestroid = false;
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
}
