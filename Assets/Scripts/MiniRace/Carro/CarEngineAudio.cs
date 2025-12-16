using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarEngineAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource idleSource;
    public AudioSource midSource;
    public AudioSource highSource;

    [Header("RPM Settings")]
    public float maxRPM = 6000f;
    public float idleRPM = 800f;

    [Header("Pitch Settings")]
    public float idlePitch = 1f;
    public float maxPitch = 2f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        idleSource.loop = true;
        midSource.loop = true;
        highSource.loop = true;

        idleSource.Play();
        midSource.Play();
        highSource.Play();
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude * 3.6f; // km/h
        float rpm = Mathf.Lerp(idleRPM, maxRPM, speed / 200f);
        rpm = Mathf.Clamp(rpm, idleRPM, maxRPM);

        float t = rpm / maxRPM;

        // 🔊 VOLUME
        idleSource.volume = Mathf.Clamp01(1f - t * 2f);
        midSource.volume  = Mathf.Clamp01(1f - Mathf.Abs(t - 0.5f) * 3f);
        highSource.volume = Mathf.Clamp01(t * 1.5f);

        // 🎵 PITCH
        float pitch = Mathf.Lerp(idlePitch, maxPitch, t);

        idleSource.pitch = pitch;
        midSource.pitch  = pitch;
        highSource.pitch = pitch;
    }
}
