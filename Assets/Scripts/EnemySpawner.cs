using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//SpawnerSencillo
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;   //Prefab
    [SerializeField] private float spawnRadius = 8f;   //SpawnRadius
    [SerializeField] private float delayBetweenSpawns = 0.5f; //DelayBetweenSpawns

    private bool waveActive = false;
    private List<GameObject> spawnedThisWave = new List<GameObject>();

    //IniciarOlaDesdeRoundManager
    public void StartWave(int count)
    {
        if (enemyPrefab == null) return;
        if (waveActive) return; // ya en ola, ignorar
        StartCoroutine(WaveRoutine(count));
    }

    //GenerarEnemigosYEsperar
    private IEnumerator WaveRoutine(int count)
    {
        waveActive = true;
        spawnedThisWave.Clear();

        for (int i = 0; i < count; i++)
        {
            //CalcularPosicionSpawn
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.y = 0f;
            Vector3 candidate = transform.position + offset;

            Vector3 spawnPos = candidate;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
                spawnPos = hit.position;

            //InstanciarEnemigo
            GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            var agent = go.GetComponent<NavMeshAgent>();
            if (agent != null) agent.Warp(spawnPos);

            spawnedThisWave.Add(go);

            //DelayEntreSpawns
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        //EsperarFinOla
        while (true)
        {
            //LimpiarDestruidos
            spawnedThisWave.RemoveAll(x => x == null);

            if (spawnedThisWave.Count == 0)
                break;

            yield return new WaitForSeconds(0.5f);
        }

        //OlaFinalizada
        waveActive = false;
    }

    //IsWaveActive
    public bool IsWaveActive() => waveActive;
}