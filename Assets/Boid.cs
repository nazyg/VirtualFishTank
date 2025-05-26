using UnityEngine;

public class Boid : MonoBehaviour
{
    public Transform waterTransform;
    public Vector3 waterSize = new Vector3(1f, 1.5f, 1f); // suyun scale değeri
    public float turnSpeed = 3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Su hacminin merkezi ve sınır boyutları
        Vector3 center = waterTransform.position;
        Vector3 extents = waterSize * 0.5f;

        Vector3 pos = transform.position;
        Vector3 vel = rb.linearVelocity;
        Vector3 steering = Vector3.zero;

        // Sınır kontrolü
        if (pos.x > center.x + extents.x) steering.x = -1;
        if (pos.x < center.x - extents.x) steering.x = 1;
        if (pos.y > center.y + extents.y) steering.y = -1;
        if (pos.y < center.y - extents.y) steering.y = 1;
        if (pos.z > center.z + extents.z) steering.z = -1;
        if (pos.z < center.z - extents.z) steering.z = 1;

        // Yön değiştirme
        if (steering != Vector3.zero)
        {
            Vector3 desired = (vel.normalized + steering.normalized).normalized;
            rb.linearVelocity = desired * vel.magnitude;
        }

        // Y ekseninde yavaşlama (süzülme hissi)
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Clamp(rb.linearVelocity.y, -0.2f, 0.2f),
            rb.linearVelocity.z
        );

        // Hız varsa yavaşça dön
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }
}
