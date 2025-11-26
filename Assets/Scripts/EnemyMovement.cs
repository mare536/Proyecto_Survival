using UnityEngine;
using UnityEngine.AI;

// Movimiento simple del enemigo: sigue al jugador y aplica daño al llegar
public class EnemyMovement : MonoBehaviour
{
    [Tooltip("Si no se asigna, busca el GameObject con tag 'Player'")]
    public Transform player;

    private NavMeshAgent navMeshAgent;
    [SerializeField] private float rangoAtaque = 1.2f;
    [SerializeField] private float cooldownAtaque = 1f;
    private float ultimoAtaque = -999f;
    [SerializeField] private float dañoAlJugador = 10f;

    // --- Sonidos del zombie ---
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoGruñido;      // sonido ocasional (idle)
    [SerializeField] private AudioClip sonidoAtaque;       // sonido al atacar
    [SerializeField] private float volumenSonido = 1f;
    [SerializeField] private float tiempoGruñidoMin = 3f;  // intervalo aleatorio mínimo entre gruñidos
    [SerializeField] private float tiempoGruñidoMax = 8f;  // intervalo aleatorio máximo
    private AudioSource audioSource;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // escalar daño según la ronda actual
        dañoAlJugador *= RoundManager.CurrentMultiplier;

        // Si no hay referencia al player en el Inspector, buscar por tag
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }

        // Preparar AudioSource (se crea si no existe)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound (ajusta en Inspector si quieres 2D)

        // Lanzar coroutine de sonidos idle si hay clip asignado
        if (sonidoGruñido != null)
            StartCoroutine(IdleGroanLoop());
    }

    void Update()
    {
        // Requerimos player y agente válidos
        if (player == null || navMeshAgent == null) return;

        // Proteger accesos al agente: solo usar si está sobre la NavMesh y habilitado
        if (!navMeshAgent.isOnNavMesh || !navMeshAgent.enabled) return;

        // Seguir al jugador
        navMeshAgent.SetDestination(player.position);

        // Esperar a que el path esté listo antes de consultar remainingDistance
        if (navMeshAgent.pathPending) return;

        // Comprobar distancia y atacar si corresponde
        if (navMeshAgent.remainingDistance <= rangoAtaque)
        {
            if (Time.time - ultimoAtaque >= cooldownAtaque)
            {
                // Intentar obtener el componente Player desde el transform apuntado (cámara/hijo posible)
                Player jugador = player.GetComponentInParent<Player>();
                if (jugador == null) jugador = player.GetComponent<Player>();

                if (jugador != null && jugador.estaVivo)
                {
                    // reproducir sonido de ataque si existe
                    if (sonidoAtaque != null && audioSource != null)
                        audioSource.PlayOneShot(sonidoAtaque, volumenSonido);

                    jugador.recibirDaño(dañoAlJugador);
                    ultimoAtaque = Time.time;
                }
            }
        }
    }

    // Coroutine simple que reproduce gruñidos ocasionales para dar vida al zombie
    private System.Collections.IEnumerator IdleGroanLoop()
    {
        while (true)
        {
            float wait = Random.Range(tiempoGruñidoMin, tiempoGruñidoMax);
            yield return new WaitForSeconds(wait);

            if (audioSource != null && sonidoGruñido != null)
                audioSource.PlayOneShot(sonidoGruñido, volumenSonido);
        }
    }
}
