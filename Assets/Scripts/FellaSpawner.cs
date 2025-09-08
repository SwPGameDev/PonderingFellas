using UnityEngine;

public class FellaSpawner : MonoBehaviour
{
    public GameObject fellaPrefab;
    public Transform spawnPoint;
    public float spawnCooldown;
    public float _spawnTimer = 0;

    private void Update()
    {
        if (_spawnTimer < spawnCooldown)
        {
            _spawnTimer += Time.deltaTime;
        }
        else
        {
            _spawnTimer = 0;
            SpawnFella();
        }
    }

    void SpawnFella()
    {
        Instantiate(fellaPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
