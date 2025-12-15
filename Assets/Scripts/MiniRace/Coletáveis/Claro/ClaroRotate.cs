using UnityEngine;

public class CoinIdleAnimation : MonoBehaviour
{
    public float rotateSpeed = 180f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.25f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        //transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}
