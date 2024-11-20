using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerInit : NetworkBehaviour
{
    #region mecagoendios
    /*
    //VARIABLES DE PERSONAJE Y PISTOLA// (AÑADIR VELOCIDAD Y OTRAS COSAS FACILMENTE!!!!!!!!!!!!!

    //////////DATOS COMPARTIDOS ENTRE ESCENAS PARA SABER EL PERSONAJE Y EL ARMA//////////

    private NetworkVariable<FixedString128Bytes> nombrePersonaje = new NetworkVariable<FixedString128Bytes>( // Nombre del jugador
        "nombre",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<FixedString128Bytes> arma = new NetworkVariable<FixedString128Bytes>( // Nombre del jugador
        "arma",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<FixedString128Bytes> hab = new NetworkVariable<FixedString128Bytes>( // Nombre del jugador
        "hab",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);



    // escribir el tipo de habilidad que tendra el player
    [SerializeField] private IHabilidad habilidad;


    public GameObject[] personajes;
    public GameObject[] armas;
    public GameObject[] habilidades;


    //Transform face;
    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {

        ApplyInfo(nombrePersonaje.Value, arma.Value, hab.Value);

        nombrePersonaje.OnValueChanged += OnNameChanged;
        arma.OnValueChanged += OnArmaChanged;
        hab.OnValueChanged += OnHabChanged;

        SetInfo(DataBetweenScenes.instance.GetNombre(), DataBetweenScenes.instance.GetArma(), DataBetweenScenes.instance.GetHabilidad());
    }


    //network variable
    public override void OnNetworkDespawn()
    {
        nombrePersonaje.OnValueChanged -= OnNameChanged;
        arma.OnValueChanged -= OnArmaChanged;
        hab.OnValueChanged -= OnHabChanged;
    }
    void OnNameChanged(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        Transform face = null;

        //nombrePersonaje.Value = newValue;
        switch (nombrePersonaje.Value.ToString())
        {
            case "Hex":
                personajes[0].SetActive(true);
                personajes[1].SetActive(false);
                personajes[2].SetActive(false);
                personajes[3].SetActive(false);
                GetComponent<PlayerController>().SetAnimator(personajes[0].GetComponent<Animator>());
                face = FindChildByName(personajes[0].transform, "face");
                break;
            case "Joker":
                personajes[1].SetActive(true);
                personajes[0].SetActive(false);
                personajes[2].SetActive(false);
                personajes[3].SetActive(false);
                GetComponent<PlayerController>().SetAnimator(personajes[1].GetComponent<Animator>());
                face = FindChildByName(personajes[1].transform, "face");
                break;
            case "Revenant":
                personajes[2].SetActive(true);
                personajes[0].SetActive(false);
                personajes[1].SetActive(false);
                personajes[3].SetActive(false);
                GetComponent<PlayerController>().SetAnimator(personajes[2].GetComponent<Animator>());
                face = FindChildByName(personajes[2].transform, "face");
                break;
            case "Outcast":
                personajes[3].SetActive(true);
                personajes[0].SetActive(false);
                personajes[1].SetActive(false);
                personajes[2].SetActive(false);
                GetComponent<PlayerController>().SetAnimator(personajes[3].GetComponent<Animator>());
                face = FindChildByName(personajes[3].transform, "face");
                break;
        }
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = face;
            source.weight = 1.0f;
            List<ConstraintSource> a = new List<ConstraintSource> { source };
            GetComponent<PlayerController>().GetPlayerHead().GetComponent<PositionConstraint>().SetSources(a);

    }
    void OnArmaChanged(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        //arma.Value = newValue;
        if (arma != null)
        {
            //arma.Value = textWeapon;
            switch (arma.Value.ToString())
            {
                case "Pistola":
                    armas[0].SetActive(true);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[0];
                    GetComponent<InputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[0].GetComponent<WeaponController>());

                    break;
                case "Rifle":
                    armas[1].SetActive(true);
                    armas[0].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[1];
                    GetComponent<InputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[1].GetComponent<WeaponController>());

                    break;
                case "Escopeta":
                    armas[2].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[2];
                    GetComponent<InputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[2].GetComponent<WeaponController>());

                    break;
                case "Francotirador":
                    armas[3].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    //selectedGun = armas[3];
                    GetComponent<InputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[3].GetComponent<WeaponController>());

                    break;
            }
        }
    }
    void OnHabChanged(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        switch (hab.Value.ToString())
        {
            case "Dash":
                //GetComponent<InputController>().SetHab(hability.GetComponent<DashHab>());
                //habilidad = new DashHab();
                //DashHab hD = gameObject.AddComponent(typeof(DashHab)) as DashHab;
                //hD.player = this;
                Destroy(habilidades[1]);
                Destroy(habilidades[2]);
                Destroy(habilidades[3]);
                GetComponent<InputController>().SetHab(habilidades[0].GetComponent<DashHab>());

                break;

            case "Explosiones":
                Destroy(habilidades[0]);
                Destroy(habilidades[2]);
                Destroy(habilidades[3]);
                GetComponent<InputController>().SetHab(habilidades[1].GetComponent<ExplosionHab>());

                break;

            case "Invisibilidad":
                Destroy(habilidades[0]);
                Destroy(habilidades[1]);
                Destroy(habilidades[3]);
                GetComponent<InputController>().SetHab(habilidades[2].GetComponent<InvisibleHab>());

                break;

            case "BombasDeHumo":
                Destroy(habilidades[0]);
                Destroy(habilidades[1]);
                Destroy(habilidades[2]);
                GetComponent<InputController>().SetHab(habilidades[3].GetComponent<SmokeHab>());

                break;
        }
    }

    public void SetInfo(FixedString128Bytes newValueN, FixedString128Bytes newValueW, FixedString128Bytes newValueH)
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetInfoServerRpc(newValueN, newValueW, newValueH);
        }
    }

    [ServerRpc]
    private void SetInfoServerRpc(FixedString128Bytes newValueN, FixedString128Bytes newValueW, FixedString128Bytes newValueH)
    {
        nombrePersonaje.Value = newValueN;
        arma.Value = newValueW;
        hab.Value = newValueH;
    }


    // Aplicamos a cada coche su nombre cuando se conecten nuevos para que todos los jugadores puedan ver el nombre del resto
    private void ApplyInfo(FixedString128Bytes textName, FixedString128Bytes textWeapon, FixedString128Bytes textHability)
    {
        if (nombrePersonaje != null)
        {
            Transform face = null;

            //nombrePersonaje.Value = textName;
            switch (nombrePersonaje.Value.ToString())
            {
                case "Hex":
                    personajes[0].SetActive(true);
                    personajes[1].SetActive(false);
                    personajes[2].SetActive(false);
                    personajes[3].SetActive(false);
                    GetComponent<PlayerController>().SetAnimator(personajes[0].GetComponent<Animator>());
                    face = FindChildByName(personajes[0].transform, "face");
                    break;
                case "Joker":
                    personajes[1].SetActive(true);
                    personajes[0].SetActive(false);
                    personajes[2].SetActive(false);
                    personajes[3].SetActive(false);
                    GetComponent<PlayerController>().SetAnimator(personajes[1].GetComponent<Animator>());
                    face = FindChildByName(personajes[1].transform, "face");
                    break;
                case "Revenant":
                    personajes[2].SetActive(true);
                    personajes[0].SetActive(false);
                    personajes[1].SetActive(false);
                    personajes[3].SetActive(false);
                    GetComponent<PlayerController>().SetAnimator(personajes[2].GetComponent<Animator>());
                    face = FindChildByName(personajes[2].transform, "face");
                    break;
                case "Outcast":
                    personajes[3].SetActive(true);
                    personajes[0].SetActive(false);
                    personajes[1].SetActive(false);
                    personajes[2].SetActive(false);
                    GetComponent<PlayerController>().SetAnimator(personajes[3].GetComponent<Animator>());
                    face = FindChildByName(personajes[3].transform, "face");
                    break;
            }
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = face;
            source.weight = 1.0f;
            List<ConstraintSource> a = new List<ConstraintSource> { source };
            GetComponent<PlayerController>().GetPlayerHead().GetComponent<PositionConstraint>().SetSources(a);

        }
        if (arma != null)
        {
            //arma.Value = textWeapon;
            switch (arma.Value.ToString())
            {
                case "Pistola":
                    armas[0].SetActive(true);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[0];
                    GetComponent<InputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    break;
                case "Rifle":
                    armas[1].SetActive(true);
                    armas[0].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[1];
                    GetComponent<InputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    break;
                case "Escopeta":
                    armas[2].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[2];
                    GetComponent<InputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    break;
                case "Francotirador":
                    armas[3].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    //selectedGun = armas[3];
                    GetComponent<InputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    break;
            }
        }
        if (hab != null)
        {
            switch (hab.Value.ToString())
            {
                case "Dash":
                    //GetComponent<InputController>().SetHab(hability.GetComponent<DashHab>());
                    //habilidad = new DashHab();
                    //DashHab hD = gameObject.AddComponent(typeof(DashHab)) as DashHab;
                    //hD.player = this;
                    Destroy(habilidades[1]);
                    Destroy(habilidades[2]);
                    Destroy(habilidades[3]);
                    GetComponent<InputController>().SetHab(habilidades[0].GetComponent<DashHab>());
                    break;

                case "Explosiones":
                    Destroy(habilidades[0]);
                    Destroy(habilidades[2]);
                    Destroy(habilidades[3]);
                    GetComponent<InputController>().SetHab(habilidades[1].GetComponent<ExplosionHab>());
                    break;

                case "Invisibilidad":
                    Destroy(habilidades[0]);
                    Destroy(habilidades[1]);
                    Destroy(habilidades[3]);
                    GetComponent<InputController>().SetHab(habilidades[2].GetComponent<InvisibleHab>());
                    break;

                case "BombasDeHumo":
                    Destroy(habilidades[0]);
                    Destroy(habilidades[1]);
                    Destroy(habilidades[2]);
                    GetComponent<InputController>().SetHab(habilidades[3].GetComponent<SmokeHab>());
                    break;
            }
        }
    }



    // Función para buscar un hijo en toda la jerarquía del objeto padre
    Transform FindChildByName(Transform parent, string childName)
    {
        // Revisamos si el nombre del objeto actual coincide
        if (parent.name == childName)
            return parent;

        // Recorremos los hijos del objeto actual
        foreach (Transform child in parent)
        {
            // Llamada recursiva para buscar en el siguiente nivel
            Transform result = FindChildByName(child, childName);
            if (result != null)
                return result; // Retorna el primer hijo encontrado
        }

        // Si no se encuentra, devuelve null
        return null;
    }
    */
    #endregion


    private NetworkVariable<FixedString128Bytes> arma = new NetworkVariable<FixedString128Bytes>( // Nombre del jugador
        "arma",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);


    public GameObject[] armas;


    public override void OnNetworkSpawn()
    {

        ApplyInfo(arma.Value);

        arma.OnValueChanged += OnArmaChanged;

        SetInfo(DataBetweenScenes.instance.GetArma());
    }


    //network variable
    public override void OnNetworkDespawn()
    {
        arma.OnValueChanged -= OnArmaChanged;
    }
    void OnArmaChanged(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        //arma.Value = newValue;
        if (arma != null)
        {
            //arma.Value = textWeapon;
            switch (arma.Value.ToString())
            {
                case "Pistola":
                    armas[0].SetActive(true);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[0];
                    GetComponent<InputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[0].GetComponent<WeaponController>());

                    break;
                case "Rifle":
                    armas[1].SetActive(true);
                    armas[0].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[1];
                    GetComponent<InputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[1].GetComponent<WeaponController>());

                    break;
                case "Escopeta":
                    armas[2].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[2];
                    GetComponent<InputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[2].GetComponent<WeaponController>());

                    break;
                case "Francotirador":
                    armas[3].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    //selectedGun = armas[3];
                    GetComponent<InputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[3].GetComponent<WeaponController>());

                    break;
            }
        }
    }

    public void SetInfo(FixedString128Bytes newValueW)
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetInfoServerRpc(newValueW);
        }
    }

    [ServerRpc]
    private void SetInfoServerRpc(FixedString128Bytes newValueW)
    {
        arma.Value = newValueW;
    }


    // Aplicamos a cada coche su nombre cuando se conecten nuevos para que todos los jugadores puedan ver el nombre del resto
    private void ApplyInfo(FixedString128Bytes textWeapon)
    {
        if (arma != null)
        {
            //arma.Value = textWeapon;
            switch (arma.Value.ToString())
            {
                case "Pistola":
                    armas[0].SetActive(true);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[0];
                    GetComponent<InputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    break;
                case "Rifle":
                    armas[1].SetActive(true);
                    armas[0].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[1];
                    GetComponent<InputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    break;
                case "Escopeta":
                    armas[2].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[2];
                    GetComponent<InputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    break;
                case "Francotirador":
                    armas[3].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    //selectedGun = armas[3];
                    GetComponent<InputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    break;
            }
        }
    }
}
