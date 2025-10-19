using UnityEngine;

// Script da BB: aplica arrasto quadrático e efeito Magnus (backspin).
// Coloque este script no prefab da BB.
public class BBPhysics : MonoBehaviour
{
    [Header("Propriedades físicas da BB")]
    public float massKg = 0.0002f;   // 0.2 g
    public float radiusM = 0.003f;   // 3 mm
    public float Cd = 0.47f;         // coef. arrasto esfera
    public float airDensity = 1.225f;

    [Header("Magnus / Backspin")]
    [HideInInspector] public float BackspinDrag = 1.0f; // setado pelo lançador
    [HideInInspector] public Vector3 spinAxis = Vector3.right;
    public bool useAdvancedMagnus = false;
    [Tooltip("Multiplicador para Cl quando useAdvancedMagnus = true")]
    public float magnusCoefficient = 0.8f;

    [Header("Opções")]
    public bool applyQuadraticDrag = true;
    public float linearDragExtra = 0f; // força linear extra se quiser

    Rigidbody rb;
    float area;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        area = Mathf.PI * radiusM * radiusM;
        if (rb != null) rb.mass = massKg;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 vel = rb.linearVelocity;
        float speed = vel.magnitude;
        if (speed <= 0.0001f) return;

        // Arrasto quadrático: Fd = -0.5 * rho * Cd * A * v^2 * v_hat
        if (applyQuadraticDrag)
        {
            float dragMag = 0.5f * airDensity * Cd * area * speed * speed;
            Vector3 dragForce = -dragMag * vel.normalized;
            rb.AddForce(dragForce, ForceMode.Force);
        }

        // arrasto linear extra (opcional)
        if (linearDragExtra > 0f)
            rb.AddForce(-linearDragExtra * vel, ForceMode.Force);

        // Magnus / lift
        Vector3 liftForce = Vector3.zero;
        if (!useAdvancedMagnus)
        {
            // versão simples: sqrt(v) * BackspinDrag
            float liftMag = Mathf.Sqrt(speed) * BackspinDrag;
            Vector3 liftDir = Vector3.Cross(spinAxis, vel.normalized).normalized;
            liftForce = liftDir * liftMag;
        }
        else
        {
            // versão física aproximada: Fl = 0.5 * rho * Cl * A * v^2
            float Cl = Mathf.Max(0f, magnusCoefficient * BackspinDrag);
            float liftMag = 0.5f * airDensity * Cl * area * speed * speed;
            Vector3 liftDir = Vector3.Cross(spinAxis, vel.normalized).normalized;
            liftForce = liftDir * liftMag;
        }

        rb.AddForce(liftForce, ForceMode.Force);
    }
}
