using UnityEngine;

public class RevolverHitscan : MonoBehaviour
{
    public Camera playerCamera; // câmera do jogador
    public float range = 100f;
    public float fireRate = 0.5f;
    public LayerMask hitMask;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            Debug.Log("Acertou: " + hit.collider.name);

            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Se o objeto tiver um script de vida, podemos causar dano:
            var target = hit.collider.GetComponent<TargetHealth>();
            if (target != null)
            {
                target.TakeDamage(10f); // você pode mudar esse valor
            }
        }
    }
}
