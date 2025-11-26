using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Spawner sencillo: controla una ola por ronda. Genera "count" enemigos uno a uno
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;   // prefab (desde Project)
    [SerializeField] private float spawnRadius = 8f;   // radio alrededor del spawner
    [SerializeField] private float delayBetweenSpawns = 0.5f; // tiempo entre cada spawn de la ola

    private bool waveActive = false;
    private List<GameObject> spawnedThisWave = new List<GameObject>();

    // Llamar desde RoundManager para iniciar la ola en este spawner
    public void StartWave(int count)
    {
        if (enemyPrefab == null) return;
        if (waveActive) return; // ya en ola, ignorar
        StartCoroutine(WaveRoutine(count));
    }

    // Genera "count" enemigos de uno en uno (con delay); después espera a que todos mueran
    private IEnumerator WaveRoutine(int count)
    {
        waveActive = true;
        spawnedThisWave.Clear();

        for (int i = 0; i < count; i++)
        {
            // posición aleatoria alrededor del spawner y ajustada a NavMesh si hay
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.y = 0f;
            Vector3 candidate = transform.position + offset;

            Vector3 spawnPos = candidate;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
                spawnPos = hit.position;

            // Instanciar enemigo
            GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            var agent = go.GetComponent<NavMeshAgent>();
            if (agent != null) agent.Warp(spawnPos);

            spawnedThisWave.Add(go);

            // esperar antes de generar el siguiente (evita spawnear todos a la vez)
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        // Esperar hasta que TODOS los enemigos de esta ola sean destruidos
        while (true)
        {
            // limpiar elementos ya destruidos
            spawnedThisWave.RemoveAll(x => x == null);

            if (spawnedThisWave.Count == 0)
                break;

            yield return new WaitForSeconds(0.5f);
        }

        // Ola finalizada
        waveActive = false;
    }

    // opcional: para saber si hay ola en curso
    public bool IsWaveActive() => waveActive;
}