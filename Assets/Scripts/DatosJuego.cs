using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class DatosJuego
{
    // Datos Generales
    public int rondaActual;
    public int puntosJugador;
    public float vidaJugador;
    public float[] posicionJugador; //GuardarPosicionXYZ

    // Datos de Armas (Lista para guardar cada una)
    public int indiceArmaEquipada;
    public List<DatosArmaGuardada> armas = new List<DatosArmaGuardada>();

    // Constructor vacío necesario para JSON
    public DatosJuego() {}
}

[System.Serializable]
public class DatosArmaGuardada
{
    public string nombreID;
    public int municionCargador;
    public int municionReserva;
    public float dañoActual; //DanoActual
}
