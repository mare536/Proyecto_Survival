using UnityEngine;
using System.Collections; // Necesario para la Corrutina
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Referencias")]
    public Player scriptJugador;
    public Weapon scriptArmas;
    public RoundManager scriptRondas;

    private int slotActual = -1;

    void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Recuperamos el slot que elegimos en el menú principal
        int slotACargar = PlayerPrefs.GetInt("SlotSeleccionado", -1);
        
        if (slotACargar != -1)
        {
            slotActual = slotACargar;
            Debug.Log($"⏳ Esperando para cargar Slot {slotActual}...");
            // AQUÍ ESTÁ EL TRUCO: Usamos una corrutina para esperar un frame
            StartCoroutine(ProcesoCarga(slotActual));
            
            // Reseteamos el PlayerPrefs para que no cargue siempre al reiniciar la escena normal
            PlayerPrefs.SetInt("SlotSeleccionado", -1); 
        }
    }

    // Esta rutina espera a que Weapon.cs y Player.cs terminen sus Start()
    IEnumerator ProcesoCarga(int slot)
    {
        yield return new WaitForEndOfFrame(); // Espera al final del primer fotograma
        CargarJuego(slot);
    }

    public void GuardarJuego()
    {
        if (slotActual == -1) slotActual = 1;

        DatosJuego datos = new DatosJuego();

        if (scriptJugador != null)
        {
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
        if (datos == null)
        {
            Debug.LogWarning("No se encontró archivo de guardado.");
            return;
        }

        Debug.Log("📂 CARGANDO DATOS...");

        // 1. Cargar Jugador
        if (scriptJugador != null)
        {
            scriptJugador.vitalidad = datos.vidaJugador;
            scriptJugador.puntos = datos.puntosJugador;
            
            // Actualizar la UI del jugador manualmente para que se vea el cambio
            // Asegúrate de tener métodos públicos en Player para esto o llama a recibirDaño(0) como truco
            scriptJugador.agregarPuntos(0); // Truco para refrescar texto puntos
            scriptJugador.recibirDaño(0);   // Truco para refrescar texto vida

            // Mover jugador (apagando CharacterController momentáneamente si existe)
            CharacterController cc = scriptJugador.GetComponent<CharacterController>();
            if(cc != null) cc.enabled = false;
            scriptJugador.transform.position = new Vector3(datos.posicionJugador[0], datos.posicionJugador[1], datos.posicionJugador[2]);
            if(cc != null) cc.enabled = true;

            Debug.Log($"✅ Jugador cargado: Vida {scriptJugador.vitalidad}, Puntos {scriptJugador.puntos}");
        }

        // 2. Cargar Rondas
        if(scriptRondas != null)
        {
            scriptRondas.rondaActual = datos.rondaActual;
            // Si tienes un método para actualizar texto de ronda, llámalo aquí
            Debug.Log($"✅ Ronda cargada: {scriptRondas.rondaActual}");
        }

        // 3. Cargar Armas
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
            // Equipar el arma guardada y refrescar la UI del arma
            scriptArmas.EquipWeapon(datos.indiceArmaEquipada);
            scriptArmas.RefrescarUI(); 
            Debug.Log("✅ Armas cargadas y munición establecida.");
        }
    }
}