using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class FreeLookZoom : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineCamera cineCam;

    [Header("Zoom Settings")]
    public float zoomSpeed = 150f;
    public float minFOV = 10f;
    public float maxFOV = 100f;

    private float targetFOV;

    void Start()
    {
        if (cineCam == null)
            cineCam = GetComponent<CinemachineCamera>();

        targetFOV = cineCam.Lens.FieldOfView;
    }

    void Update()
    {
        float scrollValue = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            targetFOV -= scrollValue * zoomSpeed * Time.deltaTime;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        }

        cineCam.Lens.FieldOfView = Mathf.Lerp(
            cineCam.Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * 10f
        );
    }
}
