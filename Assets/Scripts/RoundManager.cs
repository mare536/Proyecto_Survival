using System.Collections;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float tiempoEntreRondas = 2f;
    [SerializeField] private int spawnsBasePorSpawner = 10;
    [SerializeField] private int incrementoPorRonda = 1;
    [SerializeField] private int maxPorSpawner = 100;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoRonda;

    // Variable estática para que los enemigos sepan cuánto daño hacen
    public static float CurrentMultiplier { get; private set; } = 1f;

    // --- MAGIA DE PROGRAMACIÓN ---
    // Variable privada real
    [SerializeField] private int _rondaActual = 0; 
    private bool esperandoRonda = false;

    // Variable PÚBLICA que usa el GameManager.
    // Cuando cambiamos esta variable, ejecuta el código de dentro (set).
    public int rondaActual
    {
        get { return _rondaActual; }
        set 
        { 
            _rondaActual = value;
            ActualizarTodo(); // ¡Se actualiza solo al cargar!
        }
    }

    private void Start()
    {
        // Al empezar, nos aseguramos de que todo esté sincronizado
        ActualizarTodo();
        StartCoroutine(MonitorRondas());
    }

    // Esta función actualiza texto y dificultad basándose en el número de ronda
    public void ActualizarTodo()
    {
        // 1. Actualizar Texto UI
        if (textoRonda != null) 
            textoRonda.text = "Ronda: " + _rondaActual;

        // 2. Actualizar Dificultad (Multiplicador)
        // Si es ronda 0 o 1, es normal. Si es más, sube 15% por ronda.
        if (_rondaActual > 0)
            CurrentMultiplier = 1f + (_rondaActual - 1) * 0.15f;
        else
            CurrentMultiplier = 1f;
    }

    private IEnumerator MonitorRondas()
    {
        while (true)
        {
            // Comprueba si no hay enemigos y no estamos ya esperando
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
        
        // Subimos de ronda usando la propiedad pública para que se actualice el texto solo
        rondaActual++; 

        // Calcular enemigos para esta oleada
        int spawnsThisRound = spawnsBasePorSpawner + (rondaActual - 1) * incrementoPorRonda;
        spawnsThisRound = Mathf.Min(spawnsThisRound, maxPorSpawner);

        yield return new WaitForSeconds(tiempoEntreRondas);

        // Avisar a todos los spawners
        var spawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var s in spawners)
        {
            s.StartWave(spawnsThisRound);
        }

        esperandoRonda = false;
    }
}