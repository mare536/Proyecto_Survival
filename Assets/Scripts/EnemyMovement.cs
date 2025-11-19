using UnityEngine; // Importa el espacio de nombres principal de Unity
using UnityEngine.AI; // Importa el sistema de navegación de Unity

public class EnemyMovement : MonoBehaviour
{
    // Referencia al transform del jugador para poder seguirlo
    public Transform player;
    // Referencia al componente NavMeshAgent que permite mover al enemigo usando la malla de navegación
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float rangoAtaque = 1.2f;
    [SerializeField] private float cooldownAtaque = 1f;
    private float ultimoAtaque = -999f;
    [SerializeField] private float dañoAlJugador = 10f;

    // Start se llama una vez antes de la primera actualización del frame
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Si no se asignó el Transform del jugador en el Inspector, intentar buscar por tag
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
            else Debug.LogWarning("EnemyMovement: 'player' no asignado y no se encontró GameObject con tag 'Player'.");
        }
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Si existe una referencia al jugador, el enemigo lo sigue
        if (player != null)
        {
            // PROTECCIÓN: solo usar el NavMeshAgent si está inicializado, habilitado y en la NavMesh
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.enabled)
            {
                // Establece la posición del jugador como destino del enemigo
                navMeshAgent.SetDestination(player.position);

                // Si el agente ha llegado suficientemente cerca y ha pasado el cooldown, atacar
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= rangoAtaque)
                {
                    if (Time.time - ultimoAtaque >= cooldownAtaque)
                    {
                        Player jugador = player.GetComponent<Player>();
                        if (jugador != null && jugador.estaVivo)
                        {
                            jugador.recibirDaño(dañoAlJugador); // daño por defecto (ajusta si quieres usar otro valor)
                            ultimoAtaque = Time.time;
                        }
                    }
                }
            }
            // Si el agente no está disponible (p.ej. el enemigo murió o se desactivó), no intentar acceder a remainingDistance
        }   
    }
}
