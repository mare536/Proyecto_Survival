using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Para mostrar texto en la UI

public class Player : MonoBehaviour
{
    public bool estaVivo = false;
    [SerializeField] private float vitalidad = 100f;
    [SerializeField] private int puntos = 0;
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
    public void recibirDaño(float cantidad)
    {
        vitalidad -= cantidad;
        if (vitalidad < 0) vitalidad = 0;
        if (textoVitalidad != null)
            textoVitalidad.text = "Vitalidad: " + vitalidad;

        if (vitalidad <= 0 && estaVivo)
        {
            estaVivo = false;
            Debug.Log("Jugador muerto.");
        }
    }
}
