using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;   // Asigna el prefab desde la carpeta Project
    [SerializeField] private int maxConcurrent = 5;    // Máx enemigos simultáneos
    [SerializeField] private float spawnRadius = 8f;   // Radio alrededor del spawner
    [SerializeField] private float spawnInterval = 2f; // Segundos entre spawns

    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: asigna enemyPrefab desde la carpeta Project.");
            enabled = false;
            return;
        }
        InvokeRepeating(nameof(TrySpawn), spawnInterval, spawnInterval);
    }

    private void TrySpawn()
    {
        CleanList();

        if (activeEnemies.Count >= maxConcurrent) return;

        // Posición aleatoria en círculo alrededor del spawner
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0f;
        Vector3 candidate = transform.position + randomOffset;

        // Intentar ajustar la posición sobre la NavMesh (si hay NavMesh)
        NavMeshHit hit;
        Vector3 spawnPos = candidate;
        if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
            spawnPos = hit.position;

        GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(go);
    }

    private void CleanList()
    {
        activeEnemies.RemoveAll(item => item == null);
    }

    public void StopSpawner()
    {
        CancelInvoke(nameof(TrySpawn));
    }

    public void StartSpawner()
    {
        CancelInvoke(nameof(TrySpawn));
        InvokeRepeating(nameof(TrySpawn), spawnInterval, spawnInterval);
    }
}