using UnityEngine;
using TMPro; 
using UnityEngine.UI; // <--- NECESARIO PARA TOCAR LOS BOTONES

public class MesaMejoras : MonoBehaviour
{
    [Header("UI de la Tienda")]
    public GameObject panelTienda;       
    public TextMeshProUGUI textoAviso;   

    [Header("Referencias a los Botones REALES")]
    // Arrastra aquí los botones del Canvas
    public Button btnComprarBalas;
    public Button btnMejorarDano;
    public Button btnSalir;

    [Header("Precios y Mejoras")]
    public int precioMunicion = 100;
    public int precioMejora = 500;
    public int balasAComprar = 30;
    public float dañoAumentado = 5f;

    private bool jugadorCerca = false;
    private bool tiendaAbierta = false;

    private Player scriptJugador;
    private Weapon scriptArma;

    void Start()
    {
        if(panelTienda != null) 
            panelTienda.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            // Solo abrimos si tenemos las referencias
            if (scriptJugador != null && scriptArma != null)
            {
                AlternarTienda();
            }
            else
            {
                Debug.LogError("Error: Jugador cerca, pero referencias perdidas.");
            }
        }
    }

    public void AlternarTienda()
    {
        tiendaAbierta = !tiendaAbierta;
        if(panelTienda != null) panelTienda.SetActive(tiendaAbierta);

        if (tiendaAbierta)
        {
            // --- AQUI ESTA LA MAGIA ---
            // Le decimos a los botones que olviden a la mesa anterior y nos escuchen a nosotros
            ConfigurarBotones();

            Time.timeScale = 0f; 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (textoAviso != null) textoAviso.text = "Bienvenido a la Armería";
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Función nueva para conectar los cables dinámicamente
    void ConfigurarBotones()
    {
        if(btnComprarBalas != null)
        {
            btnComprarBalas.onClick.RemoveAllListeners(); // Borrar conexiones viejas
            btnComprarBalas.onClick.AddListener(Boton_ComprarBalas); // Conectar a ESTA mesa
        }

        if(btnMejorarDano != null)
        {
            btnMejorarDano.onClick.RemoveAllListeners();
            btnMejorarDano.onClick.AddListener(Boton_MejorarDaño);
        }

        if(btnSalir != null)
        {
            btnSalir.onClick.RemoveAllListeners();
            btnSalir.onClick.AddListener(Boton_Salir);
        }
    }

    // --- FUNCIONES LÓGICAS --- (Igual que antes)

    public void Boton_ComprarBalas()
    {
        if (scriptJugador == null || scriptArma == null) return;

        if (scriptJugador.GastarPuntos(precioMunicion))
        {
            WeaponEntry arma = scriptArma.GetArmaActual();
            arma.currentReserve += balasAComprar;
            scriptArma.RefrescarUI(); 
            if (textoAviso) textoAviso.text = "¡Munición comprada!";
        }
        else
        {
            if (textoAviso) textoAviso.text = "¡No tienes puntos suficientes!";
        }
    }

    public void Boton_MejorarDaño()
    {
        if (scriptJugador == null || scriptArma == null) return;

        if (scriptJugador.GastarPuntos(precioMejora))
        {
            WeaponEntry arma = scriptArma.GetArmaActual();
            arma.damage += dañoAumentado;
            if (textoAviso) textoAviso.text = "¡Daño mejorado!";
        }
        else
        {
            if (textoAviso) textoAviso.text = "¡No tienes puntos suficientes!";
        }
    }
    
    public void Boton_Salir()
    {
        AlternarTienda(); 
    }

    // --- DETECCION ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Player>() || other.GetComponentInParent<Player>())
        {
            jugadorCerca = true;
            scriptJugador = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
            
            scriptArma = other.GetComponent<Weapon>();
            if (scriptArma == null) scriptArma = other.GetComponentInChildren<Weapon>();
            if (scriptArma == null) scriptArma = other.GetComponentInParent<Weapon>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<Player>())
        {
            jugadorCerca = false;
            if (tiendaAbierta) AlternarTienda();
        }
    }
}