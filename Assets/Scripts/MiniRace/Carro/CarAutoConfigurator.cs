using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CarAutoConfigurator : MonoBehaviour
{
    [Header("Wheel Meshes (Visual)")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;

    [Header("Wheel Colliders")]
    public WheelCollider colFL;
    public WheelCollider colFR;
    public WheelCollider colRL;
    public WheelCollider colRR;

    [Header("Wheel Settings")]
    public float wheelRadius = 4.5f;
    public float suspensionDistance = 0.3f;
    public float suspensionSpring = 35000;
    public float suspensionDamper = 4500;

    [Header("Center of Mass Offset")]
    public Vector3 comOffset = new Vector3(0, -0.4f, 0);

    [Header("Ground Layer")]
    public LayerMask groundLayer = ~0;

#if UNITY_EDITOR
    [ContextMenu("AUTO CONFIGURE CAR")]
    public void AutoConfigure()
    {
        Debug.Log("🚗 Iniciando Auto Setup do Carro...", this);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody não encontrado no carro!");
            return;
        }

        // --------------------------------------------------------------------
        // 1) AJUSTAR CENTER OF MASS
        // --------------------------------------------------------------------
        rb.centerOfMass = comOffset;
        Debug.Log("✔ Center of Mass ajustado.");

        // --------------------------------------------------------------------
        // 2) CONFIGURAR TODAS AS RODAS
        // --------------------------------------------------------------------
        SetupWheel(colFL, wheelFL);
        SetupWheel(colFR, wheelFR);
        SetupWheel(colRL, wheelRL);
        SetupWheel(colRR, wheelRR);

        Debug.Log("✔ Todas as rodas configuradas.");

        // --------------------------------------------------------------------
        // 3) CONFIGURAR SUSPENSÃO
        // --------------------------------------------------------------------
        ApplySuspension(colFL);
        ApplySuspension(colFR);
        ApplySuspension(colRL);
        ApplySuspension(colRR);

        Debug.Log("✔ Suspensão aplicada.");

        Debug.Log("🎉 AUTO CONFIGURAÇÃO FINALIZADA!");
    }
#endif

    // ========================================================================
    // CONFIGURAÇÃO DE CADA RODA
    // ========================================================================
private void SetupWheel(WheelCollider wc, Transform mesh)
{
    if (wc == null || mesh == null)
        return;

    //------------------------------
    // 1) Alinha posição do Collider ao Mesh
    //------------------------------
    wc.transform.position = mesh.position;

    //------------------------------
    // 2) Alinha rotação do Collider ao Mesh
    //------------------------------
    wc.transform.rotation = mesh.rotation;

    //------------------------------
    // 3) Calcula automaticamente o raio
    //------------------------------
    // Busca o MeshFilter original
    MeshFilter mf = mesh.GetComponent<MeshFilter>();

    if (mf != null && mf.sharedMesh != null)
    {
        // O raio é metade do tamanho Y do mesh em espaço local
        float meshRadius = mf.sharedMesh.bounds.extents.y;

        wc.radius = meshRadius;
    }
    else
    {
        Debug.LogWarning($"⚠ MeshFilter não encontrado em {mesh.name}. Raio não calculado.");
    }

#if UNITY_EDITOR
    UnityEditor.EditorUtility.SetDirty(wc);
#endif
}



    // ========================================================================
    // CONFIGURAÇÃO DA SUSPENSÃO
    // ========================================================================
    private void ApplySuspension(WheelCollider wc)
    {
        JointSpring spr = wc.suspensionSpring;
        spr.spring = suspensionSpring;
        spr.damper = suspensionDamper;
        spr.targetPosition = 0.5f;
        wc.suspensionSpring = spr;
    }
}
