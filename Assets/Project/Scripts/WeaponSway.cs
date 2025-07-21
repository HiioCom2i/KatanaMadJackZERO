using UnityEngine;

public class KatanaSway : MonoBehaviour
{
    public float swayAmount = 0.05f;
    public float swaySpeed = 5f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (isMoving)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(Time.time * swaySpeed * 2f) * swayAmount * 0.5f;

            transform.localPosition = initialPosition + new Vector3(swayX, swayY, 0);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);
        }
    }
}
