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
        // Inicializar la UI al inicio
        textoVitalidad.text = "Vitalidad: " + vitalidad;
        textoPuntos.text = "Puntos: " + puntos;
    }

    public void Update()
    {
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
        puntos += cantidad;
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntos;
    }

    public void RecibirDano(float cantidad){
        vitalidad -= cantidad;
        textoVitalidad.text = "Vitalidad: " + vitalidad;
        
        // Actualizar UI aquí si es necesario
        
        if (vitalidad <= 0)
        {
            vitalidad = 0;
            estaVivo = false; // El jugador ha muerto
            SistemaGuardado.BorrarPartida(1); 


            if (GameManager.instancia != null)
            {
                GameManager.instancia.TriggerGameOver();
            }
        }
    }

    public void Curar(float cantidad)
    {
        vitalidad += cantidad;
        
        // Tope máximo de 100 de vida
        if (vitalidad > 100f) 
        {
            vitalidad = 100f;
        }

        // Actualizamos el texto de la pantalla
        if (textoVitalidad != null)
            textoVitalidad.text = "Vitalidad: " + vitalidad;
            
        Debug.Log("Jugador curado. Vida actual: " + vitalidad);
    }


    public bool GastarPuntos(int cantidad){
        if (puntos >= cantidad)
        {
            puntos -= cantidad;
            return true; // Devuelve VERDADERO: La compra se realizó
        }
        else
        {
            Debug.Log("No tienes suficientes puntos.");
            return false; // Devuelve FALSO: No se pudo comprar
        }
    }
}
