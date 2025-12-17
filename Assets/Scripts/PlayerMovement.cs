using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Player; //AccederEstaVivo


public class PlayerMovementSimple : MonoBehaviour
{
    [Header("ConfiguracionMovimiento")]
    //VelocidadMovimiento
    [SerializeField] private float VelocidadMovimiento;
    //VelocidadRotacion
    [SerializeField] private float VelocidadRotacion;
    //ReferenciaCharacterController
    [SerializeField] private CharacterController characterController;
    //TransformJugador
    [SerializeField] private Transform playerTransform;
    //CameraPersonaje
    [SerializeField] private Camera cameraPersonaje;

    [Header("Salto")]
    //AlturaSalto
    [SerializeField] private float alturaSalto = 1.5f;
    //Gravedad
    [SerializeField] private float gravedad = -9.81f;

    //VariablesInternas
    private Vector3 movimiento; //MovimientoGeneral
    private float rotacionX; //AnguloVerticalCamara
    private float velocidadVertical; //VelocidadVertical

    [Header("Cursor / Puntero")]
    //PunteroTexture
    [SerializeField] private Texture2D punteroTexture;
    //TamanoPuntero
    [SerializeField] private Vector2 tamañoPuntero = new Vector2(16f, 16f);
    //CursorBloqueado
    private bool cursorBloqueado = true;
    private Player playerScript;



    private void Update()
    {
        //ObtenerPlayerSiNecesario
        if (playerScript == null)
        {
            playerScript = GetComponent<Player>();
        }
        if (Time.timeScale == 0f) return;

        if (playerScript != null && playerScript.estaVivo)
        {
            MovimientoDelPersonaje();
            MovimientoDeLaCamara();
            GestionCursor();
        }
        else
        {
            BloquearCursor(false);
        }
    }

    private void Start()
    {
        //BuscarPlayerAlInicio
        playerScript = GetComponent<Player>();
        if (playerScript == null)
        {
            Debug.LogWarning("PlayerMovement: no se encontró Player en el mismo GameObject. Si Player está en otro objeto, asigna la referencia correcta o añade tag 'Player' y modifica el código.");
        }

        // Bloquear cursor después de comprobar el Player
        BloquearCursor(true);
    }

    void GestionCursor()
    {
        //GestionInputCursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BloquearCursor(false);
        }

        // Si el cursor está desbloqueado y hacemos clic izquierdo, volver a bloquear
        if (!cursorBloqueado && Input.GetMouseButtonDown(0))
        {
            BloquearCursor(true);
        }
    }

    void BloquearCursor(bool bloquear)
    {
        cursorBloqueado = bloquear;
        Cursor.lockState = bloquear ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !bloquear;
    }

    void MovimientoDelPersonaje()
    {
        float movX = Input.GetAxis("Horizontal");
        float movZ = Input.GetAxis("Vertical");
        //CalcularMovimientoHorizontal
        Vector3 movimientoHorizontal = transform.right * movX + transform.forward * movZ;

        //ComprobarSueloYSalto
        if (characterController.isGrounded)
        {
            //AjusteVelocidadVertical
            if (velocidadVertical < 0f) velocidadVertical = -1f;

            //ComprobarJump
            if (Input.GetButtonDown("Jump"))
            {
                //CalcularVelocidadSalto
                velocidadVertical = Mathf.Sqrt(-2f * gravedad * alturaSalto);
            }
        }

        //AplicarGravedad
        velocidadVertical += gravedad * Time.deltaTime;

        //MovimientoFinal
        Vector3 movimientoFinal = movimientoHorizontal * VelocidadMovimiento + Vector3.up * velocidadVertical;

        //MoverCharacterController
        characterController.Move(movimientoFinal * Time.deltaTime);

    }

    void MovimientoDeLaCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * VelocidadRotacion;
        float mouseY = Input.GetAxis("Mouse Y") * VelocidadRotacion;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

    //RotarCamaraYCuerpo
    cameraPersonaje.transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
    playerTransform.Rotate(Vector3.up * mouseX);
    }

    private void OnGUI()
    {
        //DibujarPuntero
        if (punteroTexture != null && cursorBloqueado)
        {
            float x = (Screen.width - tamañoPuntero.x) / 2f;
            float y = (Screen.height - tamañoPuntero.y) / 2f;
            Rect rect = new Rect(x, y, tamañoPuntero.x, tamañoPuntero.y);
            GUI.DrawTexture(rect, punteroTexture);
        }
        else if (cursorBloqueado)
        {
            //DibujarPuntoPredeterminado
            float size = 4f;
            float x = (Screen.width - size) / 2f;
            float y = (Screen.height - size) / 2f;
            GUI.Box(new Rect(x, y, size, size), GUIContent.none);
        }
    }
}
