using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Player; //Acceder varieble estaVivo de Player.cs


public class PlayerMovementSimple : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    // Velocidad de movimiento en el suelo (m/s)
    [SerializeField] private float VelocidadMovimiento;
    // Sensibilidad del ratón para rotación
    [SerializeField] private float VelocidadRotacion;
    // Referencia al CharacterController que gestiona colisiones y movimiento básico
    [SerializeField] private CharacterController characterController;
    // Transform del jugador (se usa para girar el cuerpo en Y)
    [SerializeField] private Transform playerTransform;
    // Cámara que mira desde la cabeza del jugador (se rota en X)
    [SerializeField] private Camera cameraPersonaje;

    [Header("Salto")]
    // Altura objetivo del salto (metros)
    [SerializeField] private float alturaSalto = 1.5f;
    // Gravedad aplicada verticalmente (debe ser negativa, p.ej. -9.81)
    [SerializeField] private float gravedad = -9.81f;

    // Variables internas
    private Vector3 movimiento; // usado si quieres almacenar movimiento general
    private float rotacionX; // ángulo vertical actual de la cámara
    private float velocidadVertical; // velocidad en Y (para salto/gravedad)

    [Header("Cursor / Puntero")]
    // Imagen para el punto central (opcional)
    [SerializeField] private Texture2D punteroTexture;
    // Tamaño en píxeles del puntero
    [SerializeField] private Vector2 tamañoPuntero = new Vector2(16f, 16f);
    // Estado si el cursor está bloqueado/oculto
    private bool cursorBloqueado = true;
    private Player playerScript;



    private void Update()
    {
        // Si playerScript no está asignado, intentar obtenerlo (por si Player está añadido en runtime)
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
        // Intentar obtener el script Player y avisar si falta
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
        // Pulsar Escape para desbloquear y mostrar cursor
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
        // Movimiento horizontal basado en la orientación del jugador
        Vector3 movimientoHorizontal = transform.right * movX + transform.forward * movZ;

        // Si estamos en tierra y la velocidad vertical es negativa, ajustarla ligeramente para mantener contacto
        if (characterController.isGrounded)
        {
            // Pequeño ajuste para evitar que la velocidad vertical crezca en negativo
            if (velocidadVertical < 0f) velocidadVertical = -1f;

            // Si pulsas la tecla Jump (por defecto Espacio), calculamos la velocidad
            // inicial para alcanzar la altura deseada.
            if (Input.GetButtonDown("Jump"))
            {
                // Fórmula física para salto: v = sqrt(2 * g * h)
                // gravedad es negativa, por eso usamos -gravedad.
                velocidadVertical = Mathf.Sqrt(-2f * gravedad * alturaSalto);
            }
        }

        // Aplicar gravedad
        velocidadVertical += gravedad * Time.deltaTime;

        // Vector final con movimiento horizontal y vertical
        Vector3 movimientoFinal = movimientoHorizontal * VelocidadMovimiento + Vector3.up * velocidadVertical;

        // Usar Move para controlar la componente Y manualmente
        characterController.Move(movimientoFinal * Time.deltaTime);

    }

    void MovimientoDeLaCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * VelocidadRotacion;
        float mouseY = Input.GetAxis("Mouse Y") * VelocidadRotacion;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

    // Rotar la cámara en X (mirar arriba/abajo) y el cuerpo en Y (giro horizontal)
    cameraPersonaje.transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
    playerTransform.Rotate(Vector3.up * mouseX);
    }

    private void OnGUI()
    {
        // Dibujar puntero en el centro de la pantalla si existe la textura
        if (punteroTexture != null && cursorBloqueado)
        {
            float x = (Screen.width - tamañoPuntero.x) / 2f;
            float y = (Screen.height - tamañoPuntero.y) / 2f;
            Rect rect = new Rect(x, y, tamañoPuntero.x, tamañoPuntero.y);
            GUI.DrawTexture(rect, punteroTexture);
        }
        else if (cursorBloqueado)
        {
            // Si no hay textura, dibujar un simple punto con GUI
            float size = 4f;
            float x = (Screen.width - size) / 2f;
            float y = (Screen.height - size) / 2f;
            GUI.Box(new Rect(x, y, size, size), GUIContent.none);
        }
    }
}
