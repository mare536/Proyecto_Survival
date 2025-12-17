using UnityEngine;
using UnityEngine.SceneManagement; //NecesarioCambiarEscena

public class MainMenu : MonoBehaviour
{
    //AsignarABotonJugar
    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    //AsignarABotonSalir
    public void Salir()
    {
        Application.Quit();
    }
}