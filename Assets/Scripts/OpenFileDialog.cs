using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections;

public class OpenFileDialog : MonoBehaviour, IPointerDownHandler
{

    public Renderer preview;
    public Authentication auth;
    void Start()
    {
        auth = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Authentication>();
        Application.ExternalEval(
            @"
document.addEventListener('click', function() {

    var fileuploader = document.getElementById('fileuploader');
    if (!fileuploader) {
        fileuploader = document.createElement('input');
        fileuploader.setAttribute('style','display:none;');
        fileuploader.setAttribute('type', 'file');
        fileuploader.setAttribute('id', 'fileuploader');
        fileuploader.setAttribute('class', 'focused');
        document.getElementsByTagName('body')[0].appendChild(fileuploader);

        fileuploader.onchange = function(e) {
        var files = e.target.files;
            for (var i = 0, f; f = files[i]; i++) {
                window.alert(URL.createObjectURL(f));
                SendMessage('" + gameObject.name + @"', 'FileDialogResult', URL.createObjectURL(f));
            }
        };
    }
    if (fileuploader.getAttribute('class') == 'focused') {
        fileuploader.setAttribute('class', '');
        fileuploader.click();
    }
});
            ");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Application.ExternalEval(
            @"
var fileuploader = document.getElementById('fileuploader');
if (fileuploader) {
    fileuploader.setAttribute('class', 'focused');
}
            ");
    }

    public void FileDialogResult(string fileUrl)
    {
        StartCoroutine(PreviewCoroutine(fileUrl));
    }

    public IEnumerator PreviewCoroutine(string url)
    {
        var www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        auth.OnDialogFileAvatar(texture);
    }
}
