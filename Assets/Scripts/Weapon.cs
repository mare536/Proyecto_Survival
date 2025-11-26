using UnityEngine;
using TMPro;

[System.Serializable]
public class WeaponEntry
{
    // Datos configurables por arma (se editan en el Inspector)
    public string weaponName = "Arma";   // nombre mostrado en UI
    public float damage = 25f;          // daño que aplica al enemigo
    public float range = 50f;           // alcance del raycast
    public float fireRate = 1f;         // disparos por segundo
    public bool automatic = false;      // si mantener clic dispara continuamente
    public int pellets = 1;             // >1 = escopeta (múltiples raycasts)
    public float spread = 5f;           // dispersión en grados para pellets
    public AudioClip shotSound;         // sonido de disparo (opcional)
    public GameObject impactPrefab;     // prefab de efecto en punto de impacto (opcional)
    public LayerMask layerMask = ~0;    // layers que puede golpear el arma
}

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private Camera camara;               // cámara desde la que se dispara (arrastrar o usa Main)
    [SerializeField] private WeaponEntry[] weapons;       // lista de armas configurables en el Inspector
    [SerializeField] private int startIndex = 0;          // arma inicial
    [Header("UI (opcional)")]
    [SerializeField] private TextMeshProUGUI textoArma;   // texto UI para mostrar el arma actual

    private int current = 0;          // índice del arma seleccionada
    private float lastShotTime;       // tiempo del último disparo (para controlar fireRate)
    private AudioSource audioSource;  // AudioSource usado para reproducir shotSound

    void Start()
    {
        // fallback a Camera.main si no se asignó
        if (camara == null) camara = Camera.main;

        // obtener AudioSource (se requiere por el atributo RequireComponent, pero por seguridad lo pedimos)
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // asegurar índice válido y actualizar UI
        current = Mathf.Clamp(startIndex, 0, Mathf.Max(0, weapons.Length - 1));
        UpdateUI();
    }

    void Update()
    {
        // sin armas configuradas no hacemos nada
        if (weapons == null || weapons.Length == 0) return;

        // cambiar arma pulsando 1..n (fácil y directo)
        for (int i = 0; i < weapons.Length; i++)
            if (Input.GetKeyDown((i + 1).ToString())) SetWeapon(i);

        // cambiar arma con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SetWeapon((current + 1) % weapons.Length);
        else if (scroll < 0f) SetWeapon((current - 1 + weapons.Length) % weapons.Length);

        // lógica de disparo según configuración del arma actual
        var w = weapons[current];
        bool wantFire = w.automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        // controlar la cadencia: comprobar tiempo desde lastShotTime
        if (wantFire && Time.time >= lastShotTime + 1f / Mathf.Max(0.0001f, w.fireRate))
        {
            Shoot(w);
            lastShotTime = Time.time;
        }
    }

    // Cambia el arma seleccionada y actualiza UI
    private void SetWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        current = index;
        UpdateUI();
    }

    // Actualiza el texto de UI (si está asignado)
    private void UpdateUI()
    {
        if (textoArma != null && weapons != null && weapons.Length > 0)
            textoArma.text = weapons[current].weaponName;
    }

    // Dispara: realiza raycasts desde la cámara y aplica daño/effects
    private void Shoot(WeaponEntry w)
    {
        // asegurar cámara
        if (camara == null) camara = Camera.main;
        if (camara == null) return;

        // reproducir sonido de disparo si hay
        if (w.shotSound != null && audioSource != null) audioSource.PlayOneShot(w.shotSound);

        // Usamos la posición y forward de la cámara para el raycast (más consistente)
        Vector3 origin = camara.transform.position;
        Vector3 forward = camara.transform.forward;

        // obtener mask usable (si el usuario dejó 0)
        int mask = w.layerMask.value;
        if (mask == 0) mask = Physics.DefaultRaycastLayers;

        // Si pellets == 1 hacemos un único raycast
        if (w.pellets <= 1)
        {
            if (Physics.Raycast(origin, forward, out RaycastHit hit, w.range, mask, QueryTriggerInteraction.Collide))
                ApplyHit(hit, w);
        }
        else
        {
            // Escopeta: varios raycasts con dispersión aleatoria
            for (int i = 0; i < w.pellets; i++)
            {
                Vector3 dir = Quaternion.Euler(Random.Range(-w.spread, w.spread),
                                               Random.Range(-w.spread, w.spread),
                                               0) * forward;
                if (Physics.Raycast(origin, dir, out RaycastHit hit, w.range, mask, QueryTriggerInteraction.Collide))
                    ApplyHit(hit, w);
            }
        }
    }

    // Aplica el resultado del impacto: daño y efecto visual
    private void ApplyHit(RaycastHit hit, WeaponEntry w)
    {
        // Buscar EnemyHealth en el collider o en sus padres y aplicar daño
        var eh = hit.collider.GetComponentInParent<EnemyHealth>();
        if (eh != null)
            eh.recibirDaño(w.damage);

        // Instanciar efecto de impacto si existe
        if (w.impactPrefab != null)
        {
            var go = Instantiate(w.impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(go, 1.5f);
        }
    }
}