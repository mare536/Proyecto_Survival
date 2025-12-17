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

    //Runtime
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public int currentReserve;
}

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    //---Configuracion---
    [Header("Referencias")]
    [SerializeField] private Camera camara;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private TextMeshProUGUI textoInfo;

    [Header("Inventario")]
    //Cambio1 publico para guardar balas GameManager
    public WeaponEntry[] weapons; 
    [SerializeField] private int startIndex = 0;

    //---Estado---
    //publico para guardar indice arma en GameManager
    public int currentIdx = 0; 
    
    private float lastShotTime;
    private bool isReloading = false;
    private AudioSource audioSource;
    private GameObject currentModel;

    public WeaponEntry CurrentWeapon => weapons[currentIdx];

    //Inicializar Armas y Municion
    void Start()
    {
        if (camara == null) camara = Camera.main;
        if (weaponHolder == null && camara != null) weaponHolder = camara.transform;
        audioSource = GetComponent<AudioSource>();

        if (weapons != null && weapons.Length > 0)
        {
            foreach (var w in weapons)
            {
                //Inicializar municion
                w.currentAmmo = w.magazineSize;
                w.currentReserve = w.reserveAmmo;
            }
            currentIdx = Mathf.Clamp(startIndex, 0, weapons.Length - 1);
            EquipWeapon(currentIdx);
        }
    }

    //Actualizar Entrada y Estado
    void Update()
    {
        if (Time.timeScale == 0) return;

        if (weapons == null || weapons.Length == 0) return;

        HandleWeaponSwitch();
        HandleReload();
        HandleShooting();
    }

    //Cambiar Arma por Numero o Scroll
    private void HandleWeaponSwitch()
    {
        for (int i = 0; i < weapons.Length; i++)
            if (Input.GetKeyDown((i + 1).ToString())) EquipWeapon(i);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) EquipWeapon((currentIdx + 1) % weapons.Length);
        else if (scroll < 0f) EquipWeapon((currentIdx - 1 + weapons.Length) % weapons.Length);
    }

    //Iniciar Recarga Si Se Solicita
    private void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            WeaponEntry w = CurrentWeapon;
            if (!isReloading && w.currentAmmo < w.magazineSize && w.currentReserve > 0)
                StartCoroutine(ReloadRoutine());
        }
    }

    //Gestionar Disparo Segun Tipo y Cadencia
    private void HandleShooting()
    {
        if (isReloading) return;
        WeaponEntry w = CurrentWeapon;
        bool triggerPulled = w.automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (!triggerPulled || Time.time < lastShotTime + w.fireRate) return;

        if (w.currentAmmo <= 0)
        {
            if (w.currentReserve > 0) StartCoroutine(ReloadRoutine());
            return;
        }

        PerformShoot();
        w.currentAmmo--;
        lastShotTime = Time.time;
        UpdateUI();
    }

    //Equipar Arma y Crear Modelo
    public void EquipWeapon(int index)
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

    //Realizar Raycasts y hacer daño
    private void PerformShoot()
    {
        WeaponEntry w = CurrentWeapon;
        if (w.shotSound != null) audioSource.PlayOneShot(w.shotSound);
        int totalShots = Mathf.Max(1, w.pellets);
        for (int i = 0; i < totalShots; i++)
        {
            Vector3 dir = camara.transform.forward;
            if (w.spread > 0)
            {
                float x = Random.Range(-w.spread, w.spread);
                float y = Random.Range(-w.spread, w.spread);
                dir = Quaternion.Euler(x, y, 0f) * dir;
            }

            if (!Physics.Raycast(camara.transform.position, dir, out RaycastHit hit, w.range)) continue;

            var enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null) enemy.recibirDaño(w.damage);

            if (w.impactPrefab != null)
            {
                var impact = Instantiate(w.impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }

    //Recarga
    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        WeaponEntry w = CurrentWeapon;
        if (textoInfo != null) textoInfo.text = "Recargando...";
        yield return new WaitForSeconds(w.reloadTime);
        
        if (weapons[currentIdx] == w)
        {
            int needed = w.magazineSize - w.currentAmmo;
            int taken = Mathf.Min(needed, w.currentReserve);
            w.currentAmmo += taken;
            w.currentReserve -= taken;
            UpdateUI();
        }
        isReloading = false;
        UpdateUI(); 
    }

    public void UpdateUI()
    {
        //Actualizar Texto de Municion
        if (textoInfo == null) return;
        WeaponEntry w = CurrentWeapon;
        textoInfo.text = $"{w.weaponName} {w.currentAmmo}/{w.currentReserve}";
    }

    public WeaponEntry GetArmaActual()
    {
        //Obtener Entrada Arma Actual
        return weapons[currentIdx];
    }
    
    public void RefrescarUI()
    {
        UpdateUI();
    }
}