using UnityEngine;
using TMPro; 

public class MesaMejoras : MonoBehaviour
{
    [Header("UI de la Tienda")]
    public GameObject panelTienda;       // El panel visual del menú (Canvas)
    public TextMeshProUGUI textoAviso;   // Texto para decir "Compra exitosa" o "Falta dinero"

    [Header("Precios y Mejoras")]
    public int precioMunicion = 100;
    public int precioMejora = 500;
    public int balasAComprar = 30;
    public float dañoAumentado = 5f;

    private bool jugadorCerca = false;
    private bool tiendaAbierta = false;

    // Referencias a LOS TRES scripts implicados
    private Player scriptJugador; // Tu script de vida y puntos
    private Weapon scriptArma;    // Tu script de armas

    void Start()
    {
        panelTienda.SetActive(false); // Empezamos con la tienda cerrada
    }

    void Update()
    {
        // Si el jugador está en el trigger y pulsa E
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            AlternarTienda();
        }
    }

    public void AlternarTienda()
    {
        tiendaAbierta = !tiendaAbierta;
        panelTienda.SetActive(tiendaAbierta);

        if (tiendaAbierta)
        {
            // PAUSAR JUEGO
            Time.timeScale = 0f; 
            
            // MOSTRAR RATÓN
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (textoAviso != null) textoAviso.text = "Bienvenido a la Armería";
        }
        else
        {
            // REANUDAR JUEGO
            Time.timeScale = 1f;
            
            // OCULTAR RATÓN
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // --- BOTONES ---

    public void Boton_ComprarBalas()
    {
        // 1. Comprobamos si el jugador tiene puntos
        if (scriptJugador.GastarPuntos(precioMunicion))
        {
            // 2. Si pagó, le damos las balas
            WeaponEntry arma = scriptArma.GetArmaActual();
            arma.currentReserve += balasAComprar;
            
            // 3. Actualizamos texto
            scriptArma.RefrescarUI(); 
            if (textoAviso != null) textoAviso.text = "¡Munición comprada!";
        }
        else
        {
            if (textoAviso != null) textoAviso.text = "¡No tienes puntos suficientes!";
        }
    }

    public void Boton_MejorarDaño()
    {
        if (scriptJugador.GastarPuntos(precioMejora))
        {
            WeaponEntry arma = scriptArma.GetArmaActual();
            arma.damage += dañoAumentado;

            if (textoAviso != null) textoAviso.text = "¡Arma mejorada! Daño: " + arma.damage;
        }
        else
        {
            if (textoAviso != null) textoAviso.text = "¡No tienes puntos suficientes!";
        }
    }
    
    public void Boton_Salir()
    {
        AlternarTienda(); // Cierra el menú y reanuda el juego
    }

    // --- DETECCIÓN DE COLISIÓN ---
    
    private void OnTriggerEnter(Collider other)
    {
        // Importante: Tu jugador debe tener el Tag "Player"
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            // Buscamos automáticamente tus scripts en el objeto que entró (el jugador)
            scriptJugador = other.GetComponent<Player>();
            scriptArma = other.GetComponent<Weapon>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (tiendaAbierta) AlternarTienda(); // Cerrar si te alejas
        }
    }
}