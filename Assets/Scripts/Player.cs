using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Para mostrar texto en la UI

public class Player : MonoBehaviour
{
    public bool estaVivo = false;
    public float vitalidad = 100f;
    public int puntos   = 0;
    [SerializeField] private TextMeshProUGUI textoVitalidad;
    [SerializeField] private TextMeshProUGUI textoPuntos;



    public void Start()
    {
        estaVivo = true;
        //InicializarUI
        textoVitalidad.text = "Vitalidad: " + vitalidad;
        textoPuntos.text = "Puntos: " + puntos;
    }

    public void Update()
    {
        //ComprobarVida
        if (estaVivo==true)
        {
            if (vitalidad <= 0)
            {
                estaVivo = false;
            }
        }
        
    }

    public void agregarPuntos(int cantidad)
    {
        //AgregarPuntosYActualizarUI
        puntos += cantidad;
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntos;
    }

    public void RecibirDano(float cantidad){
        //RestarVida
        vitalidad -= cantidad;
        textoVitalidad.text = "Vitalidad: " + vitalidad;

        if (vitalidad <= 0)
        {
            vitalidad = 0;
            estaVivo = false; //JugadorMuerto
            SistemaGuardado.BorrarPartida(1);

            if (GameManager.instancia != null)
            {
                GameManager.instancia.TriggerGameOver();
            }
        }
    }

    public void Curar(float cantidad)
    {
        //CurarJugador
        vitalidad += cantidad;

        //LimiteVida100
        if (vitalidad > 100f) 
        {
            vitalidad = 100f;
        }

        //ActualizarVidaUI
        if (textoVitalidad != null)
            textoVitalidad.text = "Vitalidad: " + vitalidad;

        Debug.Log("Jugador curado. Vida actual: " + vitalidad);
    }


    public bool GastarPuntos(int cantidad){
        //IntentarGastarPuntos
        if (puntos >= cantidad)
        {
            puntos -= cantidad;
            return true; //CompraRealizada
        }
        else
        {
            Debug.Log("No tienes suficientes puntos.");
            return false; //CompraFallida
        }
    }
}
