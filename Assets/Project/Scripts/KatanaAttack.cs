using UnityEngine;

public class KatanaAttack : MonoBehaviour
{
    public Animator animator;
    private int comboStep = 0;
    private float lastAttackTime;
    public float comboResetTime = 1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // botão esquerdo do mouse
        {
            if (Time.time - lastAttackTime > comboResetTime)
            {
                comboStep = 0; // reinicia combo se demorar
            }

            lastAttackTime = Time.time;

            animator.SetInteger("ComboIndex", comboStep);
            animator.SetTrigger("DoAttack");

            comboStep = (comboStep + 1) % 4; // 0 até 3
        }
    }
}
