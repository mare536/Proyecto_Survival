using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Referencias")]
    public Player scriptJugador;
    public Weapon scriptArmas;
    public RoundManager scriptRondas; // Asumo que tienes este script por tu mensaje anterior

    private int slotActual = -1; // -1 significa que no hemos cargado ninguno aún

    void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Al empezar la escena, miramos si hay que cargar algo
        // Usamos PlayerPrefs temporalmente para saber qué slot eligió el usuario en el Menú
        int slotACargar = PlayerPrefs.GetInt("SlotSeleccionado", -1);
        
        if (slotACargar != -1)
        {
            slotActual = slotACargar;
            CargarJuego(slotActual);
            // Reseteamos para que si reinicia la escena normal no cargue siempre
            PlayerPrefs.SetInt("SlotSeleccionado", -1); 
        }
    }

    public void GuardarJuego()
    {
        if (slotActual == -1) 
        {
            Debug.LogError("No se ha seleccionado slot. Guardando en Slot 1 por defecto.");
            slotActual = 1;
        }

        DatosJuego datos = new DatosJuego();

        // 1. Guardar Jugador
        datos.vidaJugador = scriptJugador.vitalidad; // Asumiendo que 'vitalidad' sea public
        datos.puntosJugador = scriptJugador.puntos;  // Asumiendo que 'puntos' sea public
        datos.posicionJugador = new float[] { 
            scriptJugador.transform.position.x, 
            scriptJugador.transform.position.y, 
            scriptJugador.transform.position.z 
        };

        // 2. Guardar Rondas
        if(scriptRondas != null) datos.rondaActual = scriptRondas.rondaActual;

        // 3. Guardar Armas
        datos.indiceArmaEquipada = scriptArmas.currentIdx; // Necesitas hacer 'currentIdx' public en Weapon.cs
        foreach (var arma in scriptArmas.weapons) // Necesitas hacer 'weapons' public en Weapon.cs
        {
            DatosArmaGuardada infoArma = new DatosArmaGuardada();
            infoArma.nombreID = arma.weaponName;
            infoArma.municionCargador = arma.currentAmmo;
            infoArma.municionReserva = arma.currentReserve;
            infoArma.dañoActual = arma.damage;
            datos.armas.Add(infoArma);
        }

        SistemaGuardado.GuardarPartida(datos, slotActual);
    }

    public void CargarJuego(int slot)
    {
        DatosJuego datos = SistemaGuardado.CargarPartida(slot);
        if (datos == null) return;

        // 1. Cargar Jugador
        scriptJugador.vitalidad = datos.vidaJugador;
        scriptJugador.puntos = datos.puntosJugador;
        // Importante: Desactivar CharacterController antes de mover si lo usas
        CharacterController cc = scriptJugador.GetComponent<CharacterController>();
        if(cc != null) cc.enabled = false;
        
        scriptJugador.transform.position = new Vector3(datos.posicionJugador[0], datos.posicionJugador[1], datos.posicionJugador[2]);
        
        if(cc != null) cc.enabled = true;
        
        // Actualizar UI del jugador
        // (Aquí deberías llamar a una función en Player.cs tipo 'ActualizarUI()' si las variables no lo hacen solas)

        // 2. Cargar Rondas
        if(scriptRondas != null) scriptRondas.rondaActual = datos.rondaActual;

        // 3. Cargar Armas
        // Recorremos las armas guardadas y actualizamos las del juego
        for (int i = 0; i < datos.armas.Count; i++)
        {
            if (i < scriptArmas.weapons.Length)
            {
                var armaJuego = scriptArmas.weapons[i];
                var armaDatos = datos.armas[i];

                // Verificamos por nombre para estar seguros (opcional pero recomendado)
                if (armaJuego.weaponName == armaDatos.nombreID)
                {
                    armaJuego.currentAmmo = armaDatos.municionCargador;
                    armaJuego.currentReserve = armaDatos.municionReserva;
                    armaJuego.damage = armaDatos.dañoActual;
                }
            }
        }
        // Equipar el arma que tenía
        scriptArmas.EquipWeapon(datos.indiceArmaEquipada); // Hacer EquipWeapon public en Weapon.cs
    }
}