using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Referencias")]
    public Player scriptJugador;
    public Weapon scriptArmas;
    public RoundManager scriptRondas;

    [Header("UI Game Over")]
    public GameObject panelGameOver; 

    private int slotActual = -1;
    private bool juegoTerminado = false; //VariableBloquearGuardado

    void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

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

    IEnumerator ProcesoCarga(int slot)
    {
        yield return new WaitForEndOfFrame(); 
        CargarJuego(slot);
    }

    public void GuardarJuego()
    {
        //ProhibirGuardarSiJuegoTerminado
        if (juegoTerminado) 
        {
            Debug.LogWarning("❌ Intento de guardar bloqueado porque el jugador ha muerto.");
            return; 
        }

        if (slotActual == -1) slotActual = 1;

        DatosJuego datos = new DatosJuego();

        if (scriptJugador != null)
        {
            //GuardarDatosJugador
            datos.vidaJugador = scriptJugador.vitalidad;
            datos.puntosJugador = scriptJugador.puntos;
            datos.posicionJugador = new float[] { 
                scriptJugador.transform.position.x, 
                scriptJugador.transform.position.y, 
                scriptJugador.transform.position.z 
            };
        }

        if (scriptRondas != null) datos.rondaActual = scriptRondas.rondaActual;

        if (scriptArmas != null)
        {
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

    public void CargarJuego(int slot)
    {
        DatosJuego datos = SistemaGuardado.CargarPartida(slot);
        
        //ComprobarDatosCargados
        if (datos == null)
        {
            Debug.LogWarning("No se encontró archivo de guardado. Iniciando Partida Nueva.");
            return;
        }

        //FiltroAntiMuerte
        //Si la vida es 0, no cargamos los datos
        if (datos.vidaJugador <= 0)
        {
            Debug.LogError("☠️ Se detectó una partida guardada con el jugador muerto. Eliminando y reiniciando.");
            SistemaGuardado.BorrarPartida(slot); //AsegurarBorrado
            return; // Salimos de la función para NO cargar los stats de muerte
        }

        Debug.Log("📂 CARGANDO DATOS...");

        //CargarJugador
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

        //CargarRondas
        if(scriptRondas != null)
        {
            scriptRondas.rondaActual = datos.rondaActual;
        }

        //CargarArmas
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