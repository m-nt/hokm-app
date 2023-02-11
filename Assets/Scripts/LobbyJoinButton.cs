using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyJoinButton : MonoBehaviour
{
    private bool enableEnter = false,enableGen = false;
    public Animator animator;
    public Text editableText;
    public string mainValue, secondValue;
    public void OnClickEnterLobby()
    {
        if (enableEnter)
        {
            animator.SetBool("enter", false);
            editableText.text = mainValue;
            enableEnter = false;
        }
        else
        {
            animator.SetBool("enter", true);
            editableText.text = secondValue;
            enableEnter = true;
        }
    }
    public void OnClickGenLobby()
    {
        if (enableGen)
        {
            animator.SetBool("enter", false);
            editableText.text = mainValue;
            enableGen = false;
        }
        else
        {
            animator.SetBool("enter", true);
            editableText.text = secondValue;
            enableGen = true;
        }
    }
}
