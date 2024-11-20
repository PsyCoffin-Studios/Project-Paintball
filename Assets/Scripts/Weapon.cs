using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using TMPro;


public class Weapon : NetworkBehaviour
{
    public float desviacion;
    public float desviacionAcumuluada = 1;
    public float retroceso;
    public int balasPorDisparo;

    public int chestDamage;
    public int legsDamage;

    public Quaternion[] desviaciones;
    [SerializeField] private LayerMask shootLayer;

    [SerializeField] private GameObject playerHead;
    private bool coroutineStartPoint = true;
    private Vector3 shootStartPoint;

    /// <summary>
    public const int killAfter = 150;
    public int velocity; //m/s
    /// </summary>

    public IEnumerator RecoilCoroutine;

    public CinemachineCamera cinemachineCamera; //referencia a la camara
    [SerializeField] private GameObject _impactPrefab;

    public float reduccionDeDesvio;

    public int municion;
    private int municionActual;
    [SerializeField]private int TIEMPO_RECARGA;
    [SerializeField] TextMeshProUGUI textoDebug;


    Player playerStats; 

    public bool Stop_ = false;
    public NetworkVariable<bool> Stop = new NetworkVariable<bool>();
    #region Stop
    void OnStopChanged(bool previous, bool current)
    {
        // Lógica para manejar cambios en el ID
    }

    public void SetStop(bool newStop) // Método para cambiar el ID
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetStopServerRpc(newStop); // Se modifica el ID en el servidor si el objeto existe y es el propietario
        }
    }

    [ServerRpc]
    private void SetStopServerRpc(bool newStop) // Establecer el ID en el servidor
    {
        Stop.Value = newStop;
    }

    private void ApplyStop(bool X)
    {
        if (Stop != null)
        {
            Stop.Value = X;
        }
    }
    #endregion

    public override void OnNetworkSpawn()
    {
        ApplyStop(Stop.Value);
        Stop.OnValueChanged += OnStopChanged;
        SetStop(false);
    }

    public override void OnNetworkDespawn()
    {
        Stop.OnValueChanged -= OnStopChanged;
    }

        void Start()
    {
        municionActual = municion;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "AMMO: " + municionActual + " / " + municion;

        desviaciones = new Quaternion[balasPorDisparo];
        for (int i = 0; i < balasPorDisparo; i++)
        {
            desviaciones[i] = Quaternion.Euler(
                    UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio, UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio, UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio
            );


        }
        playerHead = transform.parent.gameObject;

        playerStats = transform.root.gameObject.GetComponent<Player>();
    }

    private void Update()
    {

    }

    public bool GetCoroutineStartPoint()
    {
        return coroutineStartPoint;
    }

    public void ShootBullet()
    {
        if(municionActual>0)
        {// Calcular la dirección de disparo en el cliente


            municionActual--;
            textoDebug.GetComponent<TextMeshProUGUI>().text = "AMMO: " + municionActual + " / " + municion;

            Vector3 shootDirection = cinemachineCamera.transform.forward;
            ShootBulletServerRpc(shootDirection);


            if (municionActual <= 0)
            {
                //corutina para recargar, aunque mas adelante podría ser con una animación de recarga
                StartCoroutine(Recarga());
            }
        }
    }

    IEnumerator Recarga()
    {
        textoDebug.GetComponent<TextMeshProUGUI>().text = "AMMO: recargando...";
        Debug.Log("Arma recargando");
        yield return new WaitForSeconds(TIEMPO_RECARGA);
        Debug.Log("Arma cargada");
        municionActual = municion;
        textoDebug.GetComponent<TextMeshProUGUI>().text = "AMMO: " + municionActual + " / " + municion;
    }

    [ServerRpc]
    public void ShootBulletServerRpc(Vector3 shootDirection) //Le pasamos la dirección de disparo y dejamos que la calcule el cliente
    {
        if (coroutineStartPoint)
        {
            shootStartPoint = playerHead.transform.localEulerAngles;
            coroutineStartPoint = false;
        }
        else
        {
            StopAllCoroutines();
        }
        //RecoilCoroutine = LerpValue(shootStartPoint, playerHead.transform.localEulerAngles, playerHead.transform.localEulerAngles - new Vector3(retroceso, 0, 0), 0.05f);
        RecoilCoroutine = LerpValue2(shootStartPoint);
        StartCoroutine(RecoilCoroutine);

        // Añadir desviación
        for (int i = 0; i < balasPorDisparo; i++)
        {
            desviaciones[i] = Quaternion.Euler(
                UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio,
                UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio,
                UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio
                );

            // Aplicar la desviación a la dirección de disparo
            shootDirection = desviaciones[i] * shootDirection;
            RaycastHit hit;
            /////
            Ray ray = new Ray(cinemachineCamera.transform.position, shootDirection);

            for (int j = 0; j < killAfter; j++)
            {
                if (Physics.Raycast(ray, out hit, 1f))
                {
                    if (IsServer)
                    {
                        //impactooo
                        Debug.Log(hit.collider.name);
                        var sphereHit = Instantiate(_impactPrefab);
                        sphereHit.transform.position = hit.point;
                        sphereHit.GetComponent<NetworkObject>().Spawn();
                        //ShootBulletClientRpc(sphereHit.transform.position);
                        //fin impactoooo
                    }

                    if (((1 << hit.collider.gameObject.layer) & shootLayer) != 0)
                    {
                        Player playerHit = hit.collider.transform.root.gameObject.GetComponent<Player>();
                        if (playerHit != null && playerHit.Team.Value != playerStats.Team.Value && playerHit.Health.Value>0)
                        //esta linea determina si hay fuego amigo o no xd 
                        {
                            string bodyPart = hit.collider.tag;
                            Debug.LogWarning("DISPARO A :" + playerHit.ID.Value + "-" + playerHit.Name.Value + "(" + playerHit.Character.Value + ")" + "EN: " + bodyPart);
                            if (bodyPart == "collider_cabeza")
                            {
                                Debug.LogWarning("oo_cabeza");
                                playerHit.Damage(100, playerStats);
                                return;
                            }
                            else if (bodyPart == "collider_piernas")
                            {
                                Debug.LogWarning("oo_piernas");
                                playerHit.Damage(legsDamage, playerStats);
                            }
                            else if (bodyPart == "collider_tronco")
                            {
                                Debug.LogWarning("oo_tronco");
                                playerHit.Damage(chestDamage, playerStats);
                            }
                            
                        }
                    }
                    break;
                }
                Debug.DrawRay(ray.origin, ray.direction, Color.blue, 5);
                ray = new Ray(ray.origin + ray.direction, ray.direction + (Physics.gravity / killAfter / velocity));
            }
        }
    }

    [ClientRpc]
    void ShootBulletClientRpc(Vector3 hitPosition)
    {
        //var sphereHit = Instantiate(_impactPrefab);
        //sphereHit.transform.position = hitPosition;
        //sphereHit.GetComponent<NetworkObject>().Spawn();
        //sphereHit.GetComponent<DestroyOnAnimationEnd>().Splash(this);
    }

    IEnumerator LerpValue(Vector3 origin, Vector3 start, Vector3 end, float duration)
    {

        if (IsOwner)
        {
            for (int i = 0; i < balasPorDisparo; i++)
            {
                desviaciones[i] = Quaternion.Euler(
                        UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio,
                        UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio,
                        UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio
                );
            }

            desviacionAcumuluada += 0.15f;
        }

        float timeElapsed = 0;

        start.x = NormalizeAngle(start.x);
        end.x = NormalizeAngle(end.x);
        origin.x = NormalizeAngle(origin.x);



        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);

            playerHead.transform.localEulerAngles = new Vector3(Mathf.Lerp(start.x, end.x, t), playerHead.transform.localEulerAngles.y, playerHead.transform.localEulerAngles.z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);


        desviacionAcumuluada = 1;
        timeElapsed = 0;
        //duration = (Mathf.Abs(NormalizeAngle(playerHead.transform.localEulerAngles.x) - origin.x) * duration )/ Mathf.Abs(end.x - start.x);

        float rate = 1 / Mathf.Abs(NormalizeAngle(playerHead.transform.localEulerAngles.x) - origin.x);

        /*
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);

            playerHead.transform.localEulerAngles = new Vector3(Mathf.Lerp(NormalizeAngle(playerHead.transform.localEulerAngles.x), origin.x, t), playerHead.transform.localEulerAngles.y, playerHead.transform.localEulerAngles.z);

            timeElapsed += Time.deltaTime;
            Debug.LogWarning("ddddd");
            yield return null;
        }*/

        while (timeElapsed <= duration)
        {

            timeElapsed += Time.deltaTime * rate;
            playerHead.transform.localEulerAngles = new Vector3(Mathf.Lerp(NormalizeAngle(playerHead.transform.localEulerAngles.x), origin.x, timeElapsed), playerHead.transform.localEulerAngles.y, playerHead.transform.localEulerAngles.z);

            yield return null;
        }


        /*
        float rateTiempo = 1f / TiempoEnHacer;
        float rateVelocity = 1f / Vector3.Distance(startPos, endPos) * Velocidad;
        float t = 0.0f;

        if (t <= 1f)
        {
            t += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(startPos, endPos, t);
        }*/


        coroutineStartPoint = true;
    }


    IEnumerator LerpValue2(Vector3 origin)
    {

        if (IsOwner)
        {
            for (int i = 0; i < balasPorDisparo; i++)
            {
                desviaciones[i] = Quaternion.Euler(
                        UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio,
                        UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio,
                        UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio
                );
            }

        }

        playerHead.transform.Rotate(new Vector3(1f, 0f, 0f), -retroceso);

        origin.x = NormalizeAngle(origin.x);
        var a = NormalizeAngle(playerHead.transform.localEulerAngles.x);
        float distance = Mathf.Abs(origin.x - a);
        float progress = 0;


        while (progress < distance)
        {
            /*
            if (IsOwner)
            {
                for (int i = 0; i < balasPorDisparo; i++)
                {
                    desviaciones[i] = Quaternion.Euler(
                            UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio,
                            UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio,
                            UnityEngine.Random.Range(-desviacion, desviacion) * desviacionAcumuluada * reduccionDeDesvio
                    );
                }
                desviacionAcumuluada += 0.05f;
            }*/

            playerHead.transform.Rotate(new Vector3(1f, 0f, 0f), 10 * Time.deltaTime);
            progress += 10 * Time.deltaTime;
            yield return null;
        }

        Debug.Log("MECACHISSSSSSS");
        desviacionAcumuluada = 1;
        coroutineStartPoint = true;
    }


    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180) angle -= 360;
        else if (angle < -180) angle += 360;
        return angle;
    }


}

