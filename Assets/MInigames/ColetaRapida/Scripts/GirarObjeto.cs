using UnityEngine;

public class GirarObjeto : MonoBehaviour
{
    void Update()
    {
        // Gira 50 graus por segundo no eixo Y
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}