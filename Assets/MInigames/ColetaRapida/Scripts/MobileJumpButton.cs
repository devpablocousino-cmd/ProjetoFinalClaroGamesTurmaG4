using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJumpButton : MonoBehaviour, IPointerClickHandler
{
    public PlayerMoviment player;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (player != null)
            player.MobileJump();
    }
}
