using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float vidaMaxima = 50f;
    [SerializeField] private int puntosAlMorir = 10;
    [SerializeField] private float tiempoDestruccion = 0.5f; // retraso antes de destruir el objeto

    private float vidaActual;
    private bool estaVivo = true;
    private NavMeshAgent agent;

    void Start()
    {
        vidaActual = vidaMaxima;
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

        // Parar el agente en vez de desactivarlo para evitar excepciones al leer remainingDistance
        if (agent != null) agent.isStopped = true;

        // Dar puntos al jugador si existe 
        Player jugador = Object.FindFirstObjectByType<Player>();
        if (jugador != null)
        {
            jugador.agregarPuntos(puntosAlMorir);
        }

        // Aquí podrías reproducir animación/sfx antes de destruir
        Destroy(gameObject, tiempoDestruccion);
    }
}