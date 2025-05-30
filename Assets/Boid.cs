using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Environment")]
    public Transform waterTransform;
    public Vector3 waterSize = new Vector3(1f, 1.5f, 1f);
    public float turnSpeed = 3f;

    [Header("Targeting")]
    public Transform target;
    public float maxSpeed = 1.5f;
    public float maxAccel = 5f;

    [Header("Obstacle Avoidance")]
    public float avoidDistance = 2f;
    public float avoidForce = 5f;
    public LayerMask obstacleMask;

    [Header("Blending Weights")]
    public float pursueWeight = 1f;
    public float evadeWeight = 1f;
    public float matchVelocityWeight = 0.5f;
    public float obstacleAvoidWeight = 3f;

    public enum Mode { Blending, Arbitration }
    public Mode controlMode = Mode.Blending;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 steering = (controlMode == Mode.Blending)
            ? GetBlendedSteering()
            : GetArbitratedSteering();

        rb.linearVelocity += steering * Time.deltaTime;
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Clamp(rb.linearVelocity.y, -0.2f, 0.2f),
            rb.linearVelocity.z
        );

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }

    Vector3 GetBlendedSteering()
    {
        Vector3 accel = Vector3.zero;

        if (target != null)
        {
            accel += pursueWeight * Pursue();
            accel += evadeWeight * Evade();
            accel += matchVelocityWeight * MatchVelocity();
        }

        accel += obstacleAvoidWeight * ObstacleAvoidance();
        return Vector3.ClampMagnitude(accel, maxAccel);
    }

    Vector3 GetArbitratedSteering()
    {
        Vector3 accel = ObstacleAvoidance();
        if (accel.magnitude > 0.01f) return Vector3.ClampMagnitude(accel, maxAccel);

        accel = Evade();
        if (accel.magnitude > 0.01f) return Vector3.ClampMagnitude(accel, maxAccel);

        accel = Pursue();
        if (accel.magnitude > 0.01f) return Vector3.ClampMagnitude(accel, maxAccel);

        accel = MatchVelocity();
        return Vector3.ClampMagnitude(accel, maxAccel);
    }

    Vector3 Pursue()
    {
        if (target == null) return Vector3.zero;

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        float speed = rb.linearVelocity.magnitude;
        float lookAheadTime = (speed < 0.01f) ? 0 : distance / speed;

        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 futureTargetPos = target.position;
        if (targetRb != null)
            futureTargetPos += targetRb.linearVelocity * lookAheadTime;

        Vector3 desiredVelocity = (futureTargetPos - transform.position).normalized * maxSpeed;
        return Vector3.ClampMagnitude(desiredVelocity - rb.linearVelocity, maxAccel);
    }

    Vector3 Evade()
    {
        if (target == null) return Vector3.zero;

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        float speed = rb.linearVelocity.magnitude;
        float lookAheadTime = (speed < 0.01f) ? 0 : distance / speed;

        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 futureTargetPos = target.position;
        if (targetRb != null)
            futureTargetPos += targetRb.linearVelocity * lookAheadTime;

        Vector3 desiredVelocity = (transform.position - futureTargetPos).normalized * maxSpeed;
        return Vector3.ClampMagnitude(desiredVelocity - rb.linearVelocity, maxAccel);
    }

    Vector3 MatchVelocity()
    {
        if (target == null) return Vector3.zero;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb == null) return Vector3.zero;

        Vector3 desiredVelocity = targetRb.linearVelocity;
        return Vector3.ClampMagnitude(desiredVelocity - rb.linearVelocity, maxAccel);
    }

    Vector3 ObstacleAvoidance()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, avoidDistance, obstacleMask))
        {
            Vector3 avoidDir = hit.normal + Vector3.up * 0.2f;
            Vector3 desiredVelocity = avoidDir.normalized * maxSpeed;
            return Vector3.ClampMagnitude(desiredVelocity - rb.linearVelocity, avoidForce);
        }
        return Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * avoidDistance);
    }
}
