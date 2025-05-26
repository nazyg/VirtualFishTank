using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numBoidsToSpawn = 10;

    void Start()
    {
        for (int i = 0; i < numBoidsToSpawn; i++)
        {
            // Spawn konumu (su kutusunun içinde)
            Vector3 position = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0.2f, 1.3f),
                Random.Range(-0.9f, 0.9f)
            );

            // Rastgele yön
            Quaternion rotation = Random.rotation;

            // Instantiate prefab
            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation);

            // Rastgele boyut
            spawnedBoid.transform.localScale = Vector3.one * Random.Range(0.9f, 1.5f);

            // Renk ata
            Renderer rend = spawnedBoid.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material); // Her boid’e ayrı materyal
                rend.material.color = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f);
            }

            // Rigidbody ile rastgele hız ver
            Rigidbody rb = spawnedBoid.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearVelocity = Random.onUnitSphere * Random.Range(0.5f, 1.2f);
            }
        }
    }
}

