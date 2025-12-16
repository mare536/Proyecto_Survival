using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class MesaMejoras : MonoBehaviour
{
    [Header("UI de la Tienda")]
    public GameObject panelTienda;       
    public TextMeshProUGUI textoAviso;   

    [Header("Referencias a los Botones REALES")]
    public Button btnComprarBalas;
    public Button btnMejorarDano;
    public Button btnCurar; 
    public Button btnSalir;

    [Header("Precios y Mejoras")]
    public int precioMunicion = 100;
    public int precioMejora = 500;
    public int precioCura = 250;     
    public int balasAComprar = 30;
    public float dañoAumentado = 5f;
    public float cantidadCura = 50f; 

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

    void ConfigurarBotones()
    {
        if(btnComprarBalas != null)
        {
            btnComprarBalas.onClick.RemoveAllListeners();
            btnComprarBalas.onClick.AddListener(Boton_ComprarBalas);
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

        if(btnCurar != null)
        {
            btnCurar.onClick.RemoveAllListeners();
            btnCurar.onClick.AddListener(Boton_Curar);
        }
    }

    // --- FUNCIONES LÓGICAS ---

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

    public void Boton_Curar()
    {
        if (scriptJugador == null) return;

        // Comprobar si ya tiene la vida llena
        if (scriptJugador.vitalidad >= 100f)
        {
            if (textoAviso) textoAviso.text = "¡Tu salud ya está al máximo!";
            return;
        }

        if (scriptJugador.GastarPuntos(precioCura))
        {
            scriptJugador.Curar(cantidadCura);
            if (textoAviso) textoAviso.text = "¡Salud recuperada!";
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