using UnityEngine;

public class TriggerCollectClaro : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MenuManager.Instance.LoadCollectClaro();
        }
    }
}
