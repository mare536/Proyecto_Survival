using UnityEngine;
using UnityEngine.SceneManagement; // ¡Importante!

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa; 
    private bool estaPausado = false;

    void Start()
    {
        // Al empezar el nivel, aseguramos cursor bloqueado y panel oculto
        Time.timeScale = 1f; // Asegurar que el tiempo corre al entrar en la escena
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if(panelPausa != null) panelPausa.SetActive(false);
    }

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
        
        if(panelPausa != null) panelPausa.SetActive(estaPausado);

        if (estaPausado)
        {
            Time.timeScale = 0f; 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f; 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    // CAMBIO: Ahora esta función carga el menú principal
    public void IrAlMenuPrincipal()
    {
        // 1. ANTES de irnos, guardamos la partida automáticamente
        // GameManager ya sabe en qué slot estamos porque lo cargó al inicio
        if (GameManager.instancia != null)
        {
            GameManager.instancia.GuardarJuego();
            Debug.Log("Partida guardada automáticamente al salir.");
        }

        // 2. Reactivamos el tiempo y cargamos el menú
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainScene");
    }
}