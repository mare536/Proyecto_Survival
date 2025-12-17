using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float vidaMaxima = 50f;
    [SerializeField] private int puntosAlMorir = 10;
    [SerializeField] private float tiempoDestruccion = 0.2f; // retraso antes de destruir el objeto

    private float vidaActual;
    private bool estaVivo = true;
    private NavMeshAgent agent;

    void Start()
    {
        //AplicarMultiplicadorRonda
        vidaActual = vidaMaxima * RoundManager.CurrentMultiplier;

        //EscalarPuntosPorRonda
        puntosAlMorir = Mathf.Max(1, Mathf.RoundToInt(puntosAlMorir * RoundManager.CurrentMultiplier));

        agent = GetComponent<NavMeshAgent>();
    }

    public void recibirDaño(float cantidad)
    {
        if (!estaVivo) return;

        vidaActual -= cantidad;
        if (vidaActual <= 0f)
        {
            vidaActual = 0f;
            Morir();
        }
    }

    private void Morir()
    {
        if (!estaVivo) return;
        estaVivo = false;

        //PararAgente
        if (agent != null) agent.isStopped = true;

        //DarPuntosJugador
        Player jugador = Object.FindFirstObjectByType<Player>();
        if (jugador != null)
        {
            jugador.agregarPuntos(puntosAlMorir);
        }

        //ReproducirSfxAntesDestruir
        Destroy(gameObject, tiempoDestruccion);
    }
}