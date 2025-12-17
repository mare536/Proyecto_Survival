using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuSeleccionSlot : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelPrincipal; //PanelPrincipal
    public GameObject panelSlots;     //PanelSlots

    [Header("Info Slots")]
    public TextMeshProUGUI[] textosInfoSlots; //TextosInfoSlots

    void Start()
    {
        //InicioMostrarPaneles
        panelPrincipal.SetActive(true);
        panelSlots.SetActive(false);
        
        ActualizarInfoSlots();
    }

    //---LogicaVisual---

    public void MostrarSelectorDeSlots()
    {
        panelPrincipal.SetActive(false); //OcultarPrincipal
        panelSlots.SetActive(true);      //MostrarSlots
        ActualizarInfoSlots();           //ActualizarInfoSlots
    }

    public void VolverAlMenuPrincipal()
    {
        panelSlots.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    //---LogicaCarga---

    public void JugarSlot(int slot)
    {
        //GuardarSlotSeleccionado
        PlayerPrefs.SetInt("SlotSeleccionado", slot);
        PlayerPrefs.Save(); // Aseguramos que se guarde el dato

        //CargarEscenaJuego
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
                //EjemploTextoSlot
                textosInfoSlots[i].text = $"SLOT {numSlot}\nRonda {datos.rondaActual} - {datos.puntosJugador} pts";
            }
            else
            {
                textosInfoSlots[i].text = $"SLOT {numSlot}\nVacío (Nueva Partida)";
            }
        }
    }
}