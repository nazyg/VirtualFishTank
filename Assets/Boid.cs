
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float maxSpeed = 2f;
    public float maxAccel = 3f;
    public float slowingRadius = 1.5f;
    public float turnSpeed = 3f;

    public float limitX = 1.4f;
    public float limitY = 1.4f;
    public float limitZ = 0.9f;

    public string foodTag = "Food";

    private Rigidbody rb;
    private Vector3 targetPosition;
    private Vector3 currentSteering = Vector3.zero;

    private bool seekActive = false;
    private bool fleeActive = false;
    private bool pursueActive = false;
    private bool evadeActive = false;
    private bool arrivalActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleInput();

        Vector3 steering = Vector3.zero;

        if (seekActive)
            steering = Seek(targetPosition);
        else if (fleeActive)
            steering = Flee(targetPosition);
        else if (pursueActive)
            steering = Pursue(targetPosition);
        else if (evadeActive)
            steering = Evade(targetPosition);
        else if (arrivalActive)
        {
            GameObject food = GameObject.FindGameObjectWithTag(foodTag);
            if (food != null)
                targetPosition = food.transform.position;

            steering = Arrive(targetPosition);
        }

        currentSteering = steering;
        ApplySteering();
        ClampAndOrient();
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetTarget();
                seekActive = true;
                fleeActive = pursueActive = evadeActive = arrivalActive = false;
                Debug.Log("SEEK triggered at: " + targetPosition);
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetTarget();
                fleeActive = true;
                seekActive = pursueActive = evadeActive = arrivalActive = false;
                Debug.Log("FLEE triggered at: " + targetPosition);
            }
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetTarget();
                pursueActive = true;
                seekActive = fleeActive = evadeActive = arrivalActive = false;
                Debug.Log("PURSUE triggered at: " + targetPosition);
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetTarget();
                evadeActive = true;
                seekActive = fleeActive = pursueActive = arrivalActive = false;
                Debug.Log("EVADE triggered at: " + targetPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            arrivalActive = true;
            seekActive = fleeActive = pursueActive = evadeActive = false;
            Debug.Log("ARRIVAL mode activated");
        }
    }

    private void SetTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
            targetPosition = hit.point;
    }

    private Vector3 Seek(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized * maxSpeed;
        Vector3 steer = desired - rb.linearVelocity;
        return Vector3.ClampMagnitude(steer, maxAccel);
    }

    private Vector3 Flee(Vector3 target)
    {
        Vector3 desired = (transform.position - target).normalized * maxSpeed;
        Vector3 steer = desired - rb.linearVelocity;
        return Vector3.ClampMagnitude(steer, maxAccel);
    }

    private Vector3 Arrive(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        float dist = toTarget.magnitude;

        if (dist < 0.1f)
            return -rb.linearVelocity;

        float speed = Mathf.Min(maxSpeed, maxSpeed * (dist / slowingRadius));
        Vector3 desired = toTarget.normalized * speed;
        Vector3 steer = desired - rb.linearVelocity;
        return Vector3.ClampMagnitude(steer, maxAccel);
    }

    private Vector3 Pursue(Vector3 target)
    {
        float distance = (target - transform.position).magnitude;
        float prediction = distance / maxSpeed;
        Vector3 future = target + rb.linearVelocity * prediction;
        return Seek(future);
    }

    private Vector3 Evade(Vector3 target)
    {
        float distance = (target - transform.position).magnitude;
        float prediction = distance / maxSpeed;
        Vector3 future = target + rb.linearVelocity * prediction;
        return Flee(future);
    }

    private void ApplySteering()
    {
        rb.linearVelocity += currentSteering * Time.deltaTime;
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    private void ClampAndOrient()
    {
        Vector3 pos = transform.position;
        Vector3 vel = rb.linearVelocity;

        if (pos.x > limitX || pos.x < -limitX) vel.x = -vel.x;
        if (pos.y > limitY || pos.y < 0) vel.y = -vel.y;
        if (pos.z > limitZ || pos.z < -limitZ) vel.z = -vel.z;

        rb.linearVelocity = vel;

        if (vel.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(vel.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            Debug.Log("Food eaten!");
        }
    }
}
