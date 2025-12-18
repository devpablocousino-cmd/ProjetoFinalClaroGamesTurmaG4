using UnityEngine;
using UnityEngine.EventSystems;

public class MobileSprintButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerMoviment player;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player != null)
            player.MobileSprint(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (player != null)
            player.MobileSprint(false);
    }
}
