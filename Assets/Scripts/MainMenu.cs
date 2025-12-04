using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MainMenu : MonoBehaviour
{
    // Asigna esta función al botón "JUGAR"
    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Asigna esta función al botón "SALIR"
    public void Salir()
    {
        Application.Quit();
    }
}