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

    //VariableEstaticMultiplicador
    public static float CurrentMultiplier { get; private set; } = 1f;

    //VariablePrivada
    [SerializeField] private int _rondaActual = 0; 
    private bool esperandoRonda = false;

    //PropiedadRondaPublica
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
        //InicioSincronizar
        ActualizarTodo();
        StartCoroutine(MonitorRondas());
    }

    //ActualizarTextoYDificultad
    public void ActualizarTodo()
    {
        // 1. Actualizar Texto UI
        if (textoRonda != null) 
            textoRonda.text = "Ronda: " + _rondaActual;

        // 2. Actualizar Dificultad (Multiplicador)
        //CalcularMultiplicador
        if (_rondaActual > 0)
            CurrentMultiplier = 1f + (_rondaActual - 1) * 0.15f;
        else
            CurrentMultiplier = 1f;
    }

    //IEnumerator: método usado como corutina para pausar sin bloquear el juego.
    //Se inicia con StartCoroutine(...) y utiliza 'yield return' para esperar.
    private IEnumerator MonitorRondas()
    {
        while (true)
        {
                //ComprobarEnemigosYEsperando
            if (!esperandoRonda && Object.FindAnyObjectByType<EnemyHealth>() == null)
            {
                StartCoroutine(StartNextRound());
            }
            yield return new WaitForSeconds(1f);
        }
    }

    //WaitForSeconds depende de Time.timeScale
    private IEnumerator StartNextRound()
    {
        esperandoRonda = true;
        
        // Subimos de ronda usando la propiedad pública para que se actualice el texto solo
        rondaActual++; 

        // Calcular enemigos para esta oleada
        int spawnsThisRound = spawnsBasePorSpawner + (rondaActual - 1) * incrementoPorRonda;
        spawnsThisRound = Mathf.Min(spawnsThisRound, maxPorSpawner);

        yield return new WaitForSeconds(tiempoEntreRondas);

        // Avisar a los spawners
        var spawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var s in spawners)
        {
            s.StartWave(spawnsThisRound);
        }

        esperandoRonda = false;
    }
}