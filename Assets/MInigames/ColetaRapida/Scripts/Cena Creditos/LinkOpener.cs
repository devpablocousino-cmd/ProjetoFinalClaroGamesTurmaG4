using UnityEngine;

public class LinkOpener : MonoBehaviour
{
    public void AbrirLink(string url)
    {
        Application.OpenURL(url);
    }
}
