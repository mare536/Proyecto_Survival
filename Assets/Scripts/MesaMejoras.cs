using UnityEngine;
using TMPro; 

public class MesaMejoras : MonoBehaviour
{
    [Header("UI de la Tienda")]
    public GameObject panelTienda;       
    public TextMeshProUGUI textoAviso;   

    [Header("Precios y Mejoras")]
    public int precioMunicion = 100;
    public int precioMejora = 500;
    public int balasAComprar = 30;
    public float dañoAumentado = 5f;

    private bool jugadorCerca = false;
    private bool tiendaAbierta = false;

    //Variables para guardar quién es el jugador y su arma
    private Player scriptJugador;
    private Weapon scriptArma;

    void Start()
    {
        //Al iniciar, aseguramos que el panel no moleste
        if(panelTienda != null) 
            panelTienda.SetActive(false);
    }

    void Update()
    {
        //Solo funciona si el jugador está cerca Y hemos detectado sus scripts correctamente
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (scriptJugador != null && scriptArma != null)
            {
                AlternarTienda();
            }
            else
            {
                Debug.LogError("ERROR: Estoy cerca, pero no encuentro el script 'Player' o 'Weapon' en tu personaje.");
            }
        }
    }

    public void AlternarTienda()
    {
        tiendaAbierta = !tiendaAbierta;
        if(panelTienda != null) panelTienda.SetActive(tiendaAbierta);

        if (tiendaAbierta)
        {
            Time.timeScale = 0f; //Pausa
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (textoAviso != null) textoAviso.text = "Bienvenido a la Armería";
        }
        else
        {
            Time.timeScale = 1f; //Reanudar
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //--- FUNCIÓN DEL BOTÓN COMPRAR BALAS ---
    public void Boton_ComprarBalas()
    {
        //1. Seguridad: Si no hay scripts, salimos para que no explote el juego
        if (scriptJugador == null || scriptArma == null)
        {
            Debug.LogError("Error Crítico: Las referencias al jugador se han perdido.");
            return;
        }

        //2. Intentamos gastar el dinero del JUGADOR (Player.cs)
        if (scriptJugador.GastarPuntos(precioMunicion))
        {
            //3. Si pagó, accedemos al arma ACTUAL que tiene en la mano
            WeaponEntry armaActual = scriptArma.GetArmaActual();
            
            //4. Le sumamos las balas a la reserva
            armaActual.currentReserve += balasAComprar;
            
            //5. Actualizamos el texto de la pantalla del arma
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
        if (scriptJugador == null || scriptArma == null) return;

        if (scriptJugador.GastarPuntos(precioMejora))
        {
            WeaponEntry armaActual = scriptArma.GetArmaActual();
            armaActual.damage += dañoAumentado;

            if (textoAviso != null) textoAviso.text = "¡Arma mejorada! Daño: " + armaActual.damage;
        }
        else
        {
            if (textoAviso != null) textoAviso.text = "¡No tienes puntos suficientes!";
        }
    }
    
    public void Boton_Salir()
    {
        AlternarTienda(); 
    }

    //--- DETECCIÓN "TODO TERRENO" DEL JUGADOR ---
    private void OnTriggerEnter(Collider other)
    {
        //Verificamos si es el jugador mirando el Tag o si tiene el script Player
        if (other.CompareTag("Player") || other.GetComponent<Player>() || other.GetComponentInParent<Player>())
        {
            jugadorCerca = true;

            //BUSCAMOS EL SCRIPT 'PLAYER' (Vida y Puntos)
            scriptJugador = other.GetComponent<Player>();
            if (scriptJugador == null)
            {
                scriptJugador = other.GetComponentInParent<Player>();
            }
            if (scriptJugador == null) {
                scriptJugador = other.GetComponentInChildren<Player>();}

            //BUSCAMOS EL SCRIPT 'WEAPON' (Armas) - Aquí es donde fallaba antes
            scriptArma = other.GetComponent<Weapon>();
            if (scriptArma == null)
            {
              scriptArma = other.GetComponentInChildren<Weapon>();  
            }  //Busca en la Cámara (Hijos)
            if (scriptArma == null)
            {
                scriptArma = other.GetComponentInParent<Weapon>();
            }
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