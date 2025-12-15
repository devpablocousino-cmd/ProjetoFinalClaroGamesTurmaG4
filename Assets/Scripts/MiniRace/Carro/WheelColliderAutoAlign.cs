using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class WheelColliderAutoAlign : MonoBehaviour
{
    [Header("Wheel Mesh (visual)")]
    public Transform wheelMesh;

    [Header("Wheel Collider")]
    public WheelCollider wheelCollider;

    [Header("Ajustes Automáticos")]
    public bool adjustRadius = true;
    public bool adjustPosition = true;
    public bool snapToGround = false;

    [Header("Offset Opcional")]
    public float verticalOffset = 0f;

    private void Reset()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

#if UNITY_EDITOR
    [ContextMenu("Align WheelCollider")]
    public void AlignNow()
    {
        if (wheelCollider == null || wheelMesh == null)
        {
            Debug.LogWarning("WheelColliderAutoAlign: Atribua o WheelCollider e o WheelMesh!");
            return;
        }

        // 1️⃣ CALCULAR RAIO (usando bounds do MeshRenderer)
        if (adjustRadius)
        {
            var renderer = wheelMesh.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                float diameter = renderer.bounds.size.y;
                wheelCollider.radius = diameter / 2f;
            }
        }

        // 2️⃣ CENTRALIZAR WheelCollider NA RODA
        if (adjustPosition)
        {
            Vector3 pos = wheelMesh.position;
            pos.y += verticalOffset;

            wheelCollider.transform.position = pos;
        }

        // 3️⃣ AJUSTAR PARA TOCAR O CHÃO
        if (snapToGround)
        {
            if (Physics.Raycast(wheelMesh.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f))
            {
                wheelCollider.transform.position = hit.point + Vector3.up * wheelCollider.radius;
            }
        }

        Debug.Log($"WheelCollider alinhado: {name}", this);
    }
#endif
}
