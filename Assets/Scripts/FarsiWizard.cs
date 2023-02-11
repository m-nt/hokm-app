using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[AddComponentMenu("Farsi Saaz/fa converter",-1000)]
[ExecuteInEditMode]
public class FarsiWizard : MonoBehaviour 
{
	[TextArea(2,20)]
	public string text = "";
	public bool Convert;
	public bool Clear;
	public void Update()
	{
		if(Convert)
		{
			text = text.faConvert ();
			Convert = false;
		}
		if(Clear)
		{
			text = "";
			Clear = false;
		}

	}

}
