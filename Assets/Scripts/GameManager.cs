using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
// Gestor del juego: guardado, carga y fin de partida
public class GameManager : MonoBehaviour
{
    // Singleton de GameManager
    public static GameManager instancia;

    [Header("Referencias")]
    // Referencias a componentes (jugador, armas, rondas)
    public Player scriptJugador;
    public Weapon scriptArmas;
    public RoundManager scriptRondas;

    [Header("UI Game Over")]
    // Panel que se muestra al perder
    public GameObject panelGameOver; 

    // Slot de guardado actual
    private int slotActual = -1;
    // Indica si el juego terminó (bloquea guardado)
    private bool juegoTerminado = false;

    // Configura el singleton
    void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    // Comprueba si hay un slot para cargar al iniciar
    void Start()
    {
        int slotACargar = PlayerPrefs.GetInt("SlotSeleccionado", -1);
        
        if (slotACargar != -1)
        {
            slotActual = slotACargar;
                Debug.Log($"⏳ Esperando para cargar Slot {slotActual}...");
            StartCoroutine(ProcesoCarga(slotActual));
            
            PlayerPrefs.SetInt("SlotSeleccionado", -1); 
        }
    }

    // Espera un frame y luego llama a cargar el slot
    IEnumerator ProcesoCarga(int slot)
    {
        yield return new WaitForEndOfFrame(); 
        CargarJuego(slot);
    }

    // Guarda el estado actual en el slot
    public void GuardarJuego()
    {
        // No guardar si el juego terminó
        if (juegoTerminado) 
        {
            Debug.LogWarning("❌ Intento de guardar bloqueado porque el jugador ha muerto.");
            return; 
        }

        if (slotActual == -1) slotActual = 1;

        DatosJuego datos = new DatosJuego();

        if (scriptJugador != null)
        {
            // Guardar datos del jugador
            datos.vidaJugador = scriptJugador.vitalidad;
            datos.puntosJugador = scriptJugador.puntos;
            datos.posicionJugador = new float[] { 
                scriptJugador.transform.position.x, 
                scriptJugador.transform.position.y, 
                scriptJugador.transform.position.z 
            };
        }

        // Guardar ronda actual
        if (scriptRondas != null) datos.rondaActual = scriptRondas.rondaActual;

        if (scriptArmas != null)
        {
            // Guardar datos de cada arma
            datos.indiceArmaEquipada = scriptArmas.currentIdx;
            foreach (var arma in scriptArmas.weapons)
            {
                DatosArmaGuardada infoArma = new DatosArmaGuardada();
                infoArma.nombreID = arma.weaponName;
                infoArma.municionCargador = arma.currentAmmo;
                infoArma.municionReserva = arma.currentReserve;
                infoArma.dañoActual = arma.damage;
                datos.armas.Add(infoArma);
            }
        }

        SistemaGuardado.GuardarPartida(datos, slotActual);
    }

    // Carga datos desde un slot de guardado
    public void CargarJuego(int slot)
    {
        DatosJuego datos = SistemaGuardado.CargarPartida(slot);
        
        // Si no hay datos, iniciar partida nueva
        if (datos == null)
        {
            Debug.LogWarning("No se encontró archivo de guardado. Iniciando Partida Nueva.");
            return;
        }

        // Si la vida cargada es 0, borrar el guardado (evitar cargar muerte)
        if (datos.vidaJugador <= 0)
        {
            Debug.LogError("☠️ Se detectó una partida guardada con el jugador muerto. Eliminando y reiniciando.");
            SistemaGuardado.BorrarPartida(slot); //AsegurarBorrado
            return; // Salimos de la función para NO cargar los stats de muerte
        }

        Debug.Log("📂 CARGANDO DATOS...");

        // Restaurar vida, puntos y posición del jugador
        if (scriptJugador != null)
        {
            scriptJugador.vitalidad = datos.vidaJugador;
            scriptJugador.puntos = datos.puntosJugador;
            
            scriptJugador.agregarPuntos(0); 
            scriptJugador.RecibirDano(0);   

            CharacterController cc = scriptJugador.GetComponent<CharacterController>();
            if(cc != null) cc.enabled = false;
            scriptJugador.transform.position = new Vector3(datos.posicionJugador[0], datos.posicionJugador[1], datos.posicionJugador[2]);
            if(cc != null) cc.enabled = true;

            Debug.Log($"✅ Jugador cargado: Vida {scriptJugador.vitalidad}");
        }

        // Restaurar estado de rondas
        if(scriptRondas != null)
        {
            scriptRondas.rondaActual = datos.rondaActual;
        }

        // Restaurar armas y munición
        if (scriptArmas != null)
        {
            for (int i = 0; i < datos.armas.Count; i++)
            {
                if (i < scriptArmas.weapons.Length)
                {
                    var armaJuego = scriptArmas.weapons[i];
                    var armaDatos = datos.armas[i];

                    armaJuego.currentAmmo = armaDatos.municionCargador;
                    armaJuego.currentReserve = armaDatos.municionReserva;
                    armaJuego.damage = armaDatos.dañoActual;
                }
            }
            scriptArmas.EquipWeapon(datos.indiceArmaEquipada);
            scriptArmas.RefrescarUI(); 
        }
    }

    // Acciones al producirse Game Over: borrar guardado y mostrar UI
    public void TriggerGameOver()
    {
        if (juegoTerminado) return; //EvitarDuplicado

        Debug.Log("☠️ GAME OVER - Eliminando partida...");
        
        juegoTerminado = true; //JuegoTerminado

        //BorrarPartidaActual
        SistemaGuardado.BorrarPartida(slotActual);

        //MostrarGameOver
        if(panelGameOver != null) panelGameOver.SetActive(true);

        //CongelarJuegoYSoltarCursor
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}