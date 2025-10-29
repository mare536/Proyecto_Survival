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
        // Obtiene el componente NavMeshAgent del enemigo
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Si existe una referencia al jugador, el enemigo lo sigue
        if (player != null)
        {
            // Establece la posición del jugador como destino del enemigo
            navMeshAgent.SetDestination(player.position);
            // Si el agente ha llegado suficientemente cerca y ha pasado el cooldown, atacar
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= rangoAtaque)
            {
                if (Time.time - ultimoAtaque >= cooldownAtaque)
                {
                    Player jugador = player.GetComponent<Player>();
                    if (jugador != null && Player.estaVivo)
                    {
                        jugador.recibirDaño(dañoAlJugador); // daño por defecto (ajusta si quieres usar otro valor)
                        ultimoAtaque = Time.time;
                    }
                }
            }
        }   
    }
}
