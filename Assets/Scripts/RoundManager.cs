using System.Collections;
using UnityEngine;
using TMPro;

// RoundManager sencillo: calcula cuántos enemigos por spawner y lanza la ola.
// La ronda empieza cuando no queda ningún EnemyHealth vivo en escena.
public class RoundManager : MonoBehaviour
{
    [SerializeField] private float tiempoEntreRondas = 2f;
    [SerializeField] private int spawnsBasePorSpawner = 10; // ejemplo: 10 en ronda 1
    [SerializeField] private int incrementoPorRonda = 1;     // +1 por ronda
    [SerializeField] private int maxPorSpawner = 100;        // tope por spawner (seguridad)
    [SerializeField] private TextMeshProUGUI textoRonda;     // opcional: muestra ronda en UI

    // Multiplicador público que usan EnemyHealth/EnemyMovement
    public static float CurrentMultiplier { get; private set; } = 1f;

    public int CurrentRound { get; private set; } = 0;

    private bool esperandoRonda = false;

    private void Start()
    {
        if (textoRonda != null) textoRonda.text = "Ronda: " + CurrentRound;
        StartCoroutine(MonitorRondas());
    }

    private IEnumerator MonitorRondas()
    {
        while (true)
        {
            // usar la nueva API rápida para comprobar si queda algún EnemyHealth
            if (!esperandoRonda && Object.FindAnyObjectByType<EnemyHealth>() == null)
            {
                StartCoroutine(StartNextRound());
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator StartNextRound()
    {
        esperandoRonda = true;
        CurrentRound++;

        // calcular cuántos spawns por spawner: base + incremento*(ronda-1)
        int spawnsThisRound = spawnsBasePorSpawner + (CurrentRound - 1) * incrementoPorRonda;
        spawnsThisRound = Mathf.Min(spawnsThisRound, maxPorSpawner);

        // actualizar multiplicador (ej. 15% por ronda)
        CurrentMultiplier = 1f + (CurrentRound - 1) * 0.15f;

        // actualizar UI
        if (textoRonda != null) textoRonda.text = "Ronda: " + CurrentRound;

        // pequeña espera antes de arrancar la ola
        yield return new WaitForSeconds(tiempoEntreRondas);

        // pedir a todos los spawners que inicien su ola (usar la nueva API más rápida)
        var spawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var s in spawners)
        {
            s.StartWave(spawnsThisRound);
        }

        esperandoRonda = false;
    }
}