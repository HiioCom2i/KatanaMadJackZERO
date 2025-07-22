using UnityEngine;

public class EnemySpriteShake : MonoBehaviour
{
    public float shakeAngle = 5f;           // Ângulo do balanço (positivo e negativo)
    public float shakeInterval = 0.1f;      // Intervalo entre trocas (em segundos)

    private Quaternion baseRotation;
    private bool isShaking = false;
    private float timer = 0f;
    private bool tiltRight = true;

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        if (isShaking)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                float angle = tiltRight ? shakeAngle : -shakeAngle;
                transform.localRotation = baseRotation * Quaternion.Euler(0, 0, angle);
                tiltRight = !tiltRight;
                timer = shakeInterval;
            }
        }
        else
        {
            transform.localRotation = baseRotation;
        }
    }

    public void SetShaking(bool value)
    {
        if (!isShaking && value)
            timer = 0f; // Reinicia shake ao ligar

        isShaking = value;

        if (!value)
        {
            transform.localRotation = baseRotation;
        }
    }

    public void SetShakeSpeed(bool isChasing)
    {
        shakeInterval = isChasing ? 0.07f : 0.25f;
    }
}
