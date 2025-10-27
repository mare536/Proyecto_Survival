using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Para mostrar texto en la UI

public class Player : MonoBehaviour
{
    public static bool estaVivo = false;
    [SerializeField]private float vitalidad = 100f;
    [SerializeField] private int puntos = 0;
    [SerializeField] private TextMeshProUGUI textoVitalidad;
    [SerializeField] private TextMeshProUGUI textoPuntos;
    

    public void Start()
    {
        estaVivo = true;
    }

    public void Update()
    {
        if (estaVivo)
        {
            // Lógica del jugador cuando está vivo
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                agregarPuntos(10);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                recibirDaño(15f);
            }
            if (vitalidad <= 0)
            {
                estaVivo = false;
            }
        }
        
    }

    public void agregarPuntos(int cantidad)
    {
        puntos += cantidad;
        textoPuntos.text = "Puntos: " + puntos;

    }
    public void recibirDaño(float cantidad)
    {
        vitalidad -= cantidad;
        if (vitalidad < 0)
        {
            vitalidad = 0;
        }
        textoVitalidad.text = "Vitalidad: " + vitalidad;
    }

}
