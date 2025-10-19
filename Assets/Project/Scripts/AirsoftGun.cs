using System.Collections;
using UnityEngine;

public class AirsoftGun : MonoBehaviour
{
    [Header("Prefabs & refs")]
    public GameObject bbPrefab;
    public Transform muzzle;
    public Collider gunCollider; // opcional, para ignorar colisão inicial

    [Header("Energia / BB")]
    public float energyJoules = 1.49f;    // energia fixa do disparo (J)
    public float bbMassKg = 0.0002f;      // massa padrão (kg)

    [Header("Spawn & offsets")]
    public float spawnExtra = 0.001f;     // mm extras além do raio (m)
    public float ignoreCollisionTime = 0.05f;

    [Header("Spin / Backspin")]
    public float BackspinDrag = 1f;
    public Vector3 spinAxis = Vector3.right;
    public float initialAngularSpeed = 200f;

    [Header("Destruição automática")]
    public float bbLifetime = 5f; // tempo até a BB sumir (em segundos)

    [Header("Debug")]
    public bool logInitialSpeed = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Fire();
    }

    public void Fire()
    {
        if (bbPrefab == null || muzzle == null) return;

        float v = Mathf.Sqrt(2f * energyJoules / bbMassKg);
        if (logInitialSpeed)
            Debug.Log($"[AirsoftGun] Initial speed = {v:F3} m/s ({v * 3.280839895f:F1} fps)");

        float bbRadius = 0.003f;
        float offset = bbRadius + spawnExtra;
        Vector3 spawnPos = muzzle.position + muzzle.forward * offset;

        GameObject bb = Instantiate(bbPrefab, spawnPos, muzzle.rotation);

        // 💣 Destrói a BB após o tempo configurado
        Destroy(bb, bbLifetime);

        Rigidbody rb = bb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = bbMassKg;
            rb.linearVelocity = muzzle.forward * v;
            rb.maxAngularVelocity = 1000f;
            rb.angularVelocity = spinAxis.normalized * initialAngularSpeed;
        }

        BBPhysics bbPhysics = bb.GetComponent<BBPhysics>();
        if (bbPhysics != null)
        {
            bbPhysics.BackspinDrag = BackspinDrag;
            bbPhysics.spinAxis = spinAxis.normalized;
            bbPhysics.massKg = bbMassKg;
        }

        Collider bbCol = bb.GetComponent<Collider>();
        if (gunCollider != null && bbCol != null)
        {
            Physics.IgnoreCollision(bbCol, gunCollider, true);
            StartCoroutine(ReenableCollisionAfter(bbCol, gunCollider, ignoreCollisionTime));
        }
    }

    private IEnumerator ReenableCollisionAfter(Collider a, Collider b, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (a != null && b != null)
            Physics.IgnoreCollision(a, b, false);
    }
}
