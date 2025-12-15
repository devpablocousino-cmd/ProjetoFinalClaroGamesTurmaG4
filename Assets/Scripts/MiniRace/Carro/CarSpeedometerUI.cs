using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarSpeedometerUI : MonoBehaviour
{
    [Header("References")]
    public Rigidbody carRigidbody;
    public TextMeshProUGUI speedText;
    public Image speedNeedle;

    [Header("Speed Settings")]
    public float maxSpeed = 220f;   // km/h
    public float warningSpeed = 160f;

    [Header("Needle Settings")]
    public float minNeedleAngle = -90f;
    public float maxNeedleAngle = 90f;
    public float needleSmooth = 8f;

    [Header("Colors")]
    public Color slowColor = Color.green;
    public Color mediumColor = Color.yellow;
    public Color fastColor = new Color(1f, 0.4f, 0f);
    public Color warningColor = Color.red;

    private float currentNeedleAngle;

    void Update()
    {
//        if (carRigidbody == null || speedText == null || speedNeedle == null)
        if (carRigidbody == null)
            return;

        // ✅ Velocidade em km/h
        float speedKmh = carRigidbody.linearVelocity.magnitude * 3.6f;

        speedKmh = Mathf.Clamp(speedKmh, 0, maxSpeed);

        if (speedText != null)
        {   
            
            // ✅ Texto
            speedText.text = Mathf.RoundToInt(speedKmh) + " km/h";

            // ✅ Cor por faixa
            if (speedKmh < maxSpeed * 0.4f)
                speedText.color = slowColor;
            else if (speedKmh < maxSpeed * 0.7f)
                speedText.color = mediumColor;
            else if (speedKmh < warningSpeed)
                speedText.color = fastColor;
            else
                speedText.color = warningColor;

            // ✅ ALERTA DE LIMITE
            if (speedKmh >= warningSpeed)
                speedText.fontStyle = FontStyles.Bold;
            else
                speedText.fontStyle = FontStyles.Normal;            
        }

        // ✅ Cálculo do ponteiro
        float t = speedKmh / maxSpeed;
        float targetAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, t);
 
        currentNeedleAngle = Mathf.Lerp(
            currentNeedleAngle,
            targetAngle,
            Time.deltaTime * needleSmooth
        );

        speedNeedle.rectTransform.localRotation =
            Quaternion.Euler(0f, 0f, currentNeedleAngle);
 

    }
}
