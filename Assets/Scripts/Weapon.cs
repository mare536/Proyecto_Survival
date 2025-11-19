using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Configuración del arma")]
    [SerializeField] private Camera camara; // cámara desde la que se dispara
    [SerializeField] private float daño = 25f;
    [SerializeField] private float alcance = 50f;
    [SerializeField] private float cadenciaPorSegundo = 3f; // disparos por segundo

    [Header("Opcional")]
    [SerializeField] private GameObject impactoPrefab; // efecto al impactar (opcional)
    [SerializeField] private LayerMask layerMask = ~0; // por defecto colisionar con todo

    private float ultimoDisparo = 0f;

    void Start()
    {
        if (camara == null) camara = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float intervalo = 1f / Mathf.Max(0.0001f, cadenciaPorSegundo);
            if (Time.time - ultimoDisparo >= intervalo)
            {
                Disparar();
                ultimoDisparo = Time.time;
            }
        }
    }

    private void Disparar()
    {
        if (camara == null) return;

        Ray r = camara.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(r, out RaycastHit hit, alcance, layerMask))
        {
            // Buscar componente EnemyHealth en el collider o en sus padres
            EnemyHealth eh = hit.collider.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                eh.recibirDaño(daño);
            }

            // Instanciar efecto de impacto (si se asignó)
            if (impactoPrefab != null)
            {
                GameObject go = Instantiate(impactoPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(go, 1.5f);
            }
        }
    }
}