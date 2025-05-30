using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numBoidsToSpawn = 10;

    void Start()
    {
        // Sahnedeki ilk Food nesnesini bul
        Food food = Object.FindFirstObjectByType<Food>();
        Transform foodTransform = food != null ? food.transform : null;

        for (int i = 0; i < numBoidsToSpawn; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0.2f, 1.3f),
                Random.Range(-0.9f, 0.9f)
            );

            Quaternion rotation = Random.rotation;
            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation);

            spawnedBoid.transform.localScale = Vector3.one * Random.Range(0.9f, 1.5f);

            Renderer rend = spawnedBoid.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material);
                rend.material.color = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f);
            }

            Rigidbody rb = spawnedBoid.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearVelocity = Random.onUnitSphere * Random.Range(0.5f, 1.2f);
            }

            // 🎯 Hedef ataması
            Boid boidScript = spawnedBoid.GetComponent<Boid>();
            if (boidScript != null && foodTransform != null)
            {
                boidScript.target = foodTransform;
            }
        }
    }
}
