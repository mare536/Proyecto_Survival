using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Camera camara;       // arrastra la cámara o dejar vacío para usar Camera.main
    [SerializeField] private float daño = 25f;    // daño por disparo
    [SerializeField] private float alcance = 50f; // alcance del disparo
    [SerializeField] private LayerMask layerMask = ~0; // layers que puede golpear
    [SerializeField] private GameObject impactoPrefab; // opcional: efecto al impactar

    // --- Sonido de disparo ---
    [SerializeField] private AudioClip shotSound;    // asignar en el Inspector
    [SerializeField] private float shotVolume = 1f;
    private AudioSource audioSource;

    void Start()
    {
        if (camara == null) camara = Camera.main;

        // Obtener o crear AudioSource para reproducir el sonido de disparo
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Disparar();
    }

    private void Disparar()
    {
        if (camara == null) return;

        // reproducir sonido (si hay)
        if (shotSound != null && audioSource != null)
            audioSource.PlayOneShot(shotSound, shotVolume);

        Ray r = camara.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(r, out RaycastHit hit, alcance, layerMask))
        {
            // Si el objeto o alguno de sus padres tiene EnemyHealth, aplicarle daño
            EnemyHealth eh = hit.collider.GetComponentInParent<EnemyHealth>();
            if (eh != null) eh.recibirDaño(daño);

            if (impactoPrefab != null)
            {
                GameObject go = Instantiate(impactoPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(go, 1.5f);
            }
        }
    }
}