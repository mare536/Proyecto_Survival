using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuSeleccionSlot : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelPrincipal; // Arrastra aquí el Panel_Principal
    public GameObject panelSlots;     // Arrastra aquí el Panel_Slots

    [Header("Info Slots")]
    public TextMeshProUGUI[] textosInfoSlots; // Los textos de los botones Slot 1, 2, 3

    void Start()
    {
        // Al empezar, nos aseguramos de que se vea el principal y no los slots
        panelPrincipal.SetActive(true);
        panelSlots.SetActive(false);
        
        ActualizarInfoSlots();
    }

    // --- LÓGICA VISUAL ---

    public void MostrarSelectorDeSlots()
    {
        panelPrincipal.SetActive(false); // Ocultamos menú principal
        panelSlots.SetActive(true);      // Mostramos slots
        ActualizarInfoSlots();           // Refrescamos la info por si acaso
    }

    public void VolverAlMenuPrincipal()
    {
        panelSlots.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    // --- LÓGICA DE CARGA ---

    public void JugarSlot(int slot)
    {
        // 1. Guardamos en la memoria temporal qué slot ha elegido el jugador
        PlayerPrefs.SetInt("SlotSeleccionado", slot);
        PlayerPrefs.Save(); // Aseguramos que se guarde el dato

        // 2. Cargamos la escena del juego
        SceneManager.LoadScene("SampleScene");
    }

    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo...");
    }

    private void ActualizarInfoSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            int numSlot = i + 1;
            if (SistemaGuardado.ExistePartida(numSlot))
            {
                DatosJuego datos = SistemaGuardado.CargarPartida(numSlot);
                // Ejemplo: "SLOT 1 \n Ronda 5 - 1500 pts"
                textosInfoSlots[i].text = $"SLOT {numSlot}\nRonda {datos.rondaActual} - {datos.puntosJugador} pts";
            }
            else
            {
                textosInfoSlots[i].text = $"SLOT {numSlot}\nVacío (Nueva Partida)";
            }
        }
    }
}