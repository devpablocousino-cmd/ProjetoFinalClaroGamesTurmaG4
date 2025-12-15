using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    [Header("Wheel Meshes")]
    public Transform meshFL;
    public Transform meshFR;
    public Transform meshRL;
    public Transform meshRR;

    [Header("Drive Settings")]
    public float motorTorque = 1500f;
    public float brakeTorque = 3000f;
    public float steerAngle = 30f;

    private Vector2 moveInput;
    private float brakeInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        brakeInput = context.ReadValue<float>();
    }

    void FixedUpdate()
    {
        float steering = moveInput.x;
        float accel = moveInput.y;

        // Direção (apenas dianteiras)
        wheelFL.steerAngle = steerAngle * steering;
        wheelFR.steerAngle = steerAngle * steering;

        // Tração traseira
        wheelRL.motorTorque = motorTorque * accel;
        wheelRR.motorTorque = motorTorque * accel;

        // Freio
        float bt = brakeTorque * brakeInput;
        wheelFL.brakeTorque = bt;
        wheelFR.brakeTorque = bt;
        wheelRL.brakeTorque = bt;
        wheelRR.brakeTorque = bt;

        // Atualiza os meshes visuais
        UpdateWheelVisual(wheelFL, meshFL);
        UpdateWheelVisual(wheelFR, meshFR);
        UpdateWheelVisual(wheelRL, meshRL);
        UpdateWheelVisual(wheelRR, meshRR);
    }

    void UpdateWheelVisual(WheelCollider collider, Transform mesh)
    {
        if (collider == null || mesh == null)
            return;

        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
