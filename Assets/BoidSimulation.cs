
using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numBoidsToSpawn = 4;

    public GameObject rockPrefab;
    public GameObject foodPrefab;

    private bool placingRock = false;
    private bool placingFood = false;

    void Start()
    {
        SpawnBoids();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResetInputModes();
            placingRock = true;
            Debug.Log("Rock placement mode activated");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ResetInputModes();
            placingFood = true;
            Debug.Log("Food placement mode activated");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetInputModes();
            Debug.Log("Input modes reset.");
        }

        if (placingRock && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Instantiate(rockPrefab, hit.point + Vector3.up * 0.1f, Quaternion.identity);
                ResetInputModes();
                Debug.Log("Rock placed at: " + hit.point);
            }
        }

        if (placingFood && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Instantiate(foodPrefab, hit.point + Vector3.up * 0.1f, Quaternion.identity);
                ResetInputModes();
                Debug.Log("Food placed at: " + hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Cleaning up Rock and Food objects only...");
            DestroyAllWithTag("Rock");
            DestroyAllWithTag("Food");
        }
    }

    private void SpawnBoids()
    {
        for (int i = 0; i < numBoidsToSpawn; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-1.4f, 1.4f),
                Random.Range(0f, 1.4f),
                Random.Range(-0.9f, 0.9f)
            );

            Quaternion rotation = Random.rotation;

            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation);
            spawnedBoid.transform.localScale = Vector3.one * Random.Range(0.9f, 2f);

            Renderer rend = spawnedBoid.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material);
                rend.material.color = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f);
            }

            Rigidbody rb = spawnedBoid.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = Random.onUnitSphere * Random.Range(0.5f, 1.2f);
        }
    }

    private void DestroyAllWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    private void ResetInputModes()
    {
        placingRock = false;
        placingFood = false;
    }
}
