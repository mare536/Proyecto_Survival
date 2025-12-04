using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa; // El panel UI que dice "PAUSA"
    private bool estaPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
    }

    public void TogglePausa()
    {
        estaPausado = !estaPausado;
        panelPausa.SetActive(estaPausado);

        if (estaPausado)
        {
            Time.timeScale = 0f; // Tiempo congelado
            Cursor.lockState = CursorLockMode.None; // Ratón libre
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f; // Tiempo normal
            Cursor.lockState = CursorLockMode.Locked; // Ratón atrapado (para FPS)
            Cursor.visible = false;
        }
    }
    
    // Función para el botón "Salir del Juego"
    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo...");
    }
}