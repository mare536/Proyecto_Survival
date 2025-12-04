using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class WeaponEntry
{
    public string weaponName = "Arma"; 
    
    [Header("Combate")]
    public float damage = 25f;
    public float range = 50f;
    public float fireRate = 0.1f;
    public bool automatic = false;

    [Header("Escopeta")]
    public int pellets = 1;
    public float spread = 0f;

    [Header("Munición")]
    public int magazineSize = 12;
    public int reserveAmmo = 36;
    public float reloadTime = 1.5f;

    [Header("Visuales")]
    public AudioClip shotSound;
    public GameObject impactPrefab;
    public GameObject modelPrefab;
    public Vector3 modelLocalPosition = Vector3.zero;

    // Runtime
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public int currentReserve;
}

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    // --- CONFIGURACIÓN ---
    [Header("Referencias")]
    [SerializeField] private Camera camara;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private TextMeshProUGUI textoInfo;

    [Header("Inventario")]
    [SerializeField] private WeaponEntry[] weapons;
    [SerializeField] private int startIndex = 0;

    // --- ECONOMÍA (NUEVO) ---
    [Header("Jugador")]
    public int puntos = 1000; 
    // --- ESTADO ---
    private int currentIdx = 0;
    private float lastShotTime;
    private bool isReloading = false;
    private AudioSource audioSource;
    private GameObject currentModel;

    // Propiedad útil
    public WeaponEntry CurrentWeapon => weapons[currentIdx];

    void Start()
    {
        if (camara == null) camara = Camera.main;
        if (weaponHolder == null) weaponHolder = camara.transform;
        audioSource = GetComponent<AudioSource>();

        if (weapons != null && weapons.Length > 0)
        {
            foreach (var w in weapons)
            {
                w.currentAmmo = w.magazineSize;
                w.currentReserve = w.reserveAmmo;
            }
            currentIdx = Mathf.Clamp(startIndex, 0, weapons.Length - 1);
            EquipWeapon(currentIdx);
        }
    }

    void Update()
    {
        // IMPORTANTE: Si el juego está en pausa (Time.timeScale == 0), no hacemos nada
        if (Time.timeScale == 0) return;

        if (weapons == null || weapons.Length == 0) return;

        HandleWeaponSwitch();
        HandleReload();
        HandleShooting();
    }


    private void HandleWeaponSwitch()
    {
        for (int i = 0; i < weapons.Length; i++)
            if (Input.GetKeyDown((i + 1).ToString())) EquipWeapon(i);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) EquipWeapon((currentIdx + 1) % weapons.Length);
        else if (scroll < 0f) EquipWeapon((currentIdx - 1 + weapons.Length) % weapons.Length);
    }

    private void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            WeaponEntry w = CurrentWeapon;
            if (!isReloading && w.currentAmmo < w.magazineSize && w.currentReserve > 0)
                StartCoroutine(ReloadRoutine());
        }
    }

    private void HandleShooting()
    {
        if (isReloading) return;
        WeaponEntry w = CurrentWeapon;
        bool triggerPulled = w.automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (triggerPulled && Time.time >= lastShotTime + w.fireRate)
        {
            if (w.currentAmmo <= 0)
            {
                if (w.currentReserve > 0) StartCoroutine(ReloadRoutine());
            }
            else
            {
                PerformShoot();
                w.currentAmmo--;
                lastShotTime = Time.time;
                UpdateUI();
            }
        }
    }

    private void EquipWeapon(int index)
    {
        if (index == currentIdx && currentModel != null) return;
        currentIdx = index;
        isReloading = false;
        if (currentModel != null) Destroy(currentModel);
        if (CurrentWeapon.modelPrefab != null)
        {
            currentModel = Instantiate(CurrentWeapon.modelPrefab, weaponHolder);
            currentModel.transform.localPosition = CurrentWeapon.modelLocalPosition;
            currentModel.transform.localRotation = Quaternion.identity;
        }
        UpdateUI();
    }

    private void PerformShoot()
    {
        WeaponEntry w = CurrentWeapon;
        if (w.shotSound != null) audioSource.PlayOneShot(w.shotSound);
        int totalShots = Mathf.Max(1, w.pellets);
        for (int i = 0; i < totalShots; i++)
        {
            Vector3 direction = camara.transform.forward;
            if (w.spread > 0)
            {
                float x = Random.Range(-w.spread, w.spread);
                float y = Random.Range(-w.spread, w.spread);
                direction = Quaternion.Euler(x, y, 0) * direction;
            }
            if (Physics.Raycast(camara.transform.position, direction, out RaycastHit hit, w.range))
            {
                EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
                if (enemy != null) enemy.recibirDaño(w.damage);
                if (w.impactPrefab != null)
                {
                    GameObject impact = Instantiate(w.impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 2f);
                }
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        WeaponEntry w = CurrentWeapon;
        if (textoInfo != null) textoInfo.text = "Recargando...";
        yield return new WaitForSeconds(w.reloadTime);
        if (CurrentWeapon == w)
        {
            int needed = w.magazineSize - w.currentAmmo;
            int taken = Mathf.Min(needed, w.currentReserve);
            w.currentAmmo += taken;
            w.currentReserve -= taken;
            UpdateUI();
        }
        isReloading = false;
        if (CurrentWeapon == w) UpdateUI(); 
    }

    // --- FUNCIONES PÚBLICAS PARA LA TIENDA (NUEVO) ---

    public void ComprarMunicion(int cantidad, int coste)
    {
        if (puntos >= coste)
        {
            puntos -= coste;
            CurrentWeapon.currentReserve += cantidad;
            UpdateUI();
        }
    }

    public void MejorarDaño(float cantidadMejora, int coste)
    {
        if (puntos >= coste)
        {
            puntos -= coste;
            CurrentWeapon.damage += cantidadMejora;
            UpdateUI(); // Para actualizar los puntos en pantalla si los muestras
        }
    }

    public void UpdateUI()
    {
        if (textoInfo == null) return;
        WeaponEntry w = CurrentWeapon;
        // Ahora muestra también el Dinero ($)
        textoInfo.text = $"{w.weaponName} {w.currentAmmo}/{w.currentReserve} ";
    }
    // Método público para que la mesa sepa qué arma tienes en la mano
    public WeaponEntry GetArmaActual()
    {
        return weapons[currentIdx];
    }
    
    // Método para recargar interfaz desde fuera (cuando compras balas)
    public void RefrescarUI()
    {
        UpdateUI();
    }
}