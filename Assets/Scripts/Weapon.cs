using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;


public class Weapon : NetworkBehaviour
{
    public float desviacion;
    public float desviacionAcumuluada = 1;
    public float retroceso;
    public int balasPorDisparo;
    public Quaternion[] desviaciones;

    [SerializeField] private GameObject playerHead;
    private bool coroutineStartPoint = true;
    private Vector3 shootStartPoint;

    /// <summary>
    public const int killAfter = 150;
    public int velocity; //m/s
    /// </summary>

    public IEnumerator RecoilCoroutine;

    public CinemachineCamera cinemachineCamera; //referencia a la camara
    [SerializeField]private GameObject _impactPrefab;


    public float reduccionDeDesvio;


    void Start() {

            desviaciones = new Quaternion[balasPorDisparo];
            for (int i = 0; i < balasPorDisparo; i++)
            {
                desviaciones[i] = Quaternion.Euler(
                        UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio, UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio, UnityEngine.Random.Range(-desviacion, desviacion) * reduccionDeDesvio
                );


            }
            playerHead = transform.parent.gameObject;

    }

    private void Update()
    {
        
    }

    public void ShootBullet()
    {
        // Calcular la dirección de disparo en el cliente
        Vector3 shootDirection = cinemachineCamera.transform.forward;

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
            // Llamar al RPC con la dirección calculada
            ShootBulletServerRpc(shootDirection);
        }
    }

    [ServerRpc]
    public void ShootBulletServerRpc(Vector3 shootDirection) //Le pasamos la dirección de disparo y dejamos que la calcule el cliente
    {

                /*
                if (RecoilCoroutine != null)
                {
                    StopCoroutine(RecoilCoroutine);
                    RecoilCoroutine = null;
                }
                */
                if (coroutineStartPoint)
                {
                    shootStartPoint = playerHead.transform.localEulerAngles;
                    coroutineStartPoint = false;
                }
                else
                {
                    StopAllCoroutines();
                }
                RecoilCoroutine = LerpValue(shootStartPoint, playerHead.transform.localEulerAngles, playerHead.transform.localEulerAngles - new Vector3(retroceso, 0, 0), 0.05f);
                StartCoroutine(RecoilCoroutine);

                RaycastHit hit;
                /////
                Vector3[] vectors = new Vector3[killAfter];
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
                            //sphereHit.transform.SetParent(hit.transform.GetChild(0));
                            sphereHit.GetComponent<NetworkObject>().Spawn(); // Spawn solo para el servidor

                            // Notificar a todos los clientes
                            ShootBulletClientRpc(sphereHit.transform.position);

                            //fin impactoooo
                        }
                    
                        break;
                    }
                    Debug.DrawRay(ray.origin, ray.direction, Color.blue);
                    ray = new Ray(ray.origin + ray.direction, ray.direction + (Physics.gravity / killAfter / velocity));

                }


    }

    [ClientRpc]
    void ShootBulletClientRpc(Vector3 hitPosition)
    {
        var sphereHit = Instantiate(_impactPrefab);
        sphereHit.transform.position = hitPosition;

    }

    IEnumerator LerpValue(Vector3 origin, Vector3 start, Vector3 end, float duration)
    {

        if (IsOwner) {
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
            t = t * t * (3f-2f*t);

            playerHead.transform.localEulerAngles = new Vector3(Mathf.Lerp(start.x, end.x, t), playerHead.transform.localEulerAngles.y, playerHead.transform.localEulerAngles.z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);


        desviacionAcumuluada = 1;
        timeElapsed = 0;
        //duration = (Mathf.Abs(NormalizeAngle(playerHead.transform.localEulerAngles.x) - origin.x) * duration )/ Mathf.Abs(end.x - start.x);

        float rate = 1/Mathf.Abs(NormalizeAngle(playerHead.transform.localEulerAngles.x) - origin.x);

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


    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180) angle -= 360;
        else if (angle < -180) angle += 360;
        return angle;
    }


}

