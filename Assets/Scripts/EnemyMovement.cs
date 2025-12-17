using UnityEngine;
using UnityEngine.AI;

//MovimientoSimpleEnemigo
public class EnemyMovement : MonoBehaviour
{
    [Tooltip("Si no se asigna, busca el GameObject con tag 'Player'")]
    public Transform player;

    private NavMeshAgent navMeshAgent;
    [SerializeField] private float rangoAtaque = 1.2f;
    [SerializeField] private float cooldownAtaque = 1f;
    private float ultimoAtaque = -999f;
    [SerializeField] private float dañoAlJugador = 10f;

    //---SonidosZombie---
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoGruñido;      //SonidoIdle
    [SerializeField] private AudioClip sonidoAtaque;       //SonidoAtaque
    [SerializeField] private float volumenSonido = 1f;
    [SerializeField] private float tiempoGruñidoMin = 3f;  //TiempoGruñidoMin
    [SerializeField] private float tiempoGruñidoMax = 8f;  //TiempoGruñidoMax
    private AudioSource audioSource;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        //EscalarDanoSegunRonda
        dañoAlJugador *= RoundManager.CurrentMultiplier;

        // Si no hay referencia al player en el Inspector, buscar por tag
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }

        //PrepararAudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound 

        //IniciarCoroutineSonidos
        if (sonidoGruñido != null)
            StartCoroutine(IdleGroanLoop());
    }

    void Update()
    {
        //ComprobarPlayerYAgente
        if (player == null || navMeshAgent == null) return;

        //ProtegerAccesoAgente
        if (!navMeshAgent.isOnNavMesh || !navMeshAgent.enabled) return;

        //SeguirJugador
        navMeshAgent.SetDestination(player.position);

        //EsperarPathListo
        if (navMeshAgent.pathPending) return;

        //ComprobarDistanciaYAtacar
        if (navMeshAgent.remainingDistance <= rangoAtaque)
        {
            if (Time.time - ultimoAtaque >= cooldownAtaque)
            {
                // Intentar obtener el componente Player desde el transform apuntado (cámara/hijo posible)
                Player jugador = player.GetComponentInParent<Player>();
                if (jugador == null) jugador = player.GetComponent<Player>();

                if (jugador != null && jugador.estaVivo)
                {
                    //ReproducirSonidoAtaque
                    if (sonidoAtaque != null && audioSource != null)
                        audioSource.PlayOneShot(sonidoAtaque, volumenSonido);

                    jugador.RecibirDano(dañoAlJugador);
                    ultimoAtaque = Time.time;
                }
            }
        }
    }

    //CoroutineGruñidos
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
