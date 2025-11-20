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

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Si no hay referencia al player en el Inspector, buscar por tag
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }
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
                    jugador.recibirDaño(dañoAlJugador);
                    ultimoAtaque = Time.time;
                }
            }
        }
    }
}
