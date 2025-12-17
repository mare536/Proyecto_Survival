using UnityEngine;
using UnityEngine.SceneManagement; //Importante

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa; 
    private bool estaPausado = false;

    void Start()
    {
        //InicioNivelAjustes
        Time.timeScale = 1f; //AjustarTimeScale
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
    
    //IrAlMenuPrincipal
    public void IrAlMenuPrincipal()
    {
        //GuardarAntesSalir
        //GameManager maneja el slot
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