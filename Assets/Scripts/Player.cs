using HelloWorld;
using System.Collections;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour
{
    // Player Info
    public NetworkVariable<FixedString128Bytes> Name = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> Character = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<int> ID = new NetworkVariable<int>();
    public NetworkVariable<int> Team = new NetworkVariable<int>();

    //KillDeathAssist - Las stats de kills(K), muertes(D) y asistencias(A) (a priori solamente implementaremos K y D)
    public NetworkVariable<Vector3Int> KDA = new NetworkVariable<Vector3Int>();

    public NetworkVariable<int> Health = new NetworkVariable<int>();

    private TeamDeathmatchManager teamDeathmatchManager;
    [SerializeField] Animator animator;
    [SerializeField] Rig rig;

    public override string ToString()
    {
        return "PLAYER "+ID.Value+": "+Name.Value+"(Equipo "+Team.Value+") / HEALTH: "+Health.Value+" / KILLS: "+ KDA.Value.x +" / DEATHS: " + KDA.Value.y;
    }

    public override void OnNetworkSpawn()
    {
        ApplyName(Name.Value);
        ApplyID(ID.Value);
        ApplyTeam(Team.Value);
        ApplyCharacter(Character.Value);
        ApplyKDA(KDA.Value);
        ApplyHealth(Health.Value);

        Name.OnValueChanged += OnNameChanged;
        ID.OnValueChanged += OnIDChanged;
        Team.OnValueChanged += OnTeamChanged;
        Character.OnValueChanged += OnCharacterChanged;
        KDA.OnValueChanged += OnKDAChanged;
        Health.OnValueChanged += OnHealthChanged;

        SetName(/*provisional*/"pedro");
        SetCharacter(DataBetweenScenes.instance.GetNombre());
        SetKDA(new Vector3Int(0,0,0));
        SetHealth(100);
    }

    public override void OnNetworkDespawn()
    {
        Name.OnValueChanged -= OnNameChanged;
        ID.OnValueChanged -= OnIDChanged;
        Character.OnValueChanged -= OnCharacterChanged;
        KDA.OnValueChanged -= OnKDAChanged;
        Health.OnValueChanged -= OnHealthChanged;
    }

    private void Start()
    {
        teamDeathmatchManager = GameObject.Find("@TeamDeathmatchManager").GetComponent<TeamDeathmatchManager>();
    }

    public void Damage(int damage, Player player)
    {
        Health.Value -= damage;
        if (Health.Value <= 0)
        {
            GetComponent<PlayerInput>().enabled = false;
            //GetComponent<PlayerController>().selectedGun.SetActive(false);
            rig.weight = 0;
            DeadAnimationServerRpc();
            StartCoroutine(RespawnCooldown());
            teamDeathmatchManager.Respawn(gameObject);
            player.KDA.Value = new Vector3Int(player.KDA.Value.x+1, player.KDA.Value.y, player.KDA.Value.z);
            KDA.Value = new Vector3Int(KDA.Value.x, KDA.Value.y + 1, KDA.Value.z);
        }
    }

    [ServerRpc]
    public void DeadAnimationServerRpc()
    {
        animator.SetTrigger("dead");
    }
    [ServerRpc]
    public void RespawnAnimationServerRpc()
    {
        animator.SetTrigger("respawn");
    }

    IEnumerator RespawnCooldown()
    {
        yield return new WaitForSeconds(6);
        teamDeathmatchManager.Respawn(gameObject);
        RespawnAnimationServerRpc();
        rig.weight = 1;
        //GetComponent<PlayerController>().selectedGun.SetActive(true);
        GetComponent<PlayerInput>().enabled = true;
    }


    #region NAME
    void OnNameChanged(FixedString128Bytes previous, FixedString128Bytes current)
    {
        //nameTag.SetNameTag(Name.Value); // actualiza la etiqueta del nombre cuando el nombre cambia
    }

    public void SetName(FixedString128Bytes newX) // metodo para cambiar el nombre
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetNameServerRpc(newX); // se modifica el nombre en el servidor si el objeto existe y es el propietario
        }
    }

    [ServerRpc]
    private void SetNameServerRpc(FixedString128Bytes newX)
    {
        Name.Value = newX;
    }


    // Aplicamos a cada x su valor cuando se conecten nuevos
    private void ApplyName(FixedString128Bytes X)
    {

        if (Name != null)
        {
            Name.Value = X;
        }
    }
    #endregion

    #region ID
    void OnIDChanged(int previous, int current)
    {
        // Lógica para manejar cambios en el ID
    }

    public void SetID(int newID) // Método para cambiar el ID
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetIDServerRpc(newID); // Se modifica el ID en el servidor si el objeto existe y es el propietario
        }
    }

    [ServerRpc]
    private void SetIDServerRpc(int newID) // Establecer el ID en el servidor
    {
        ID.Value = newID;
    }

    private void ApplyID(int X)
    {
        if (ID != null)
        {
            ID.Value = X;
        }
    }
    #endregion

    #region Team
    void OnTeamChanged(int previous, int current)
    {
        // Lógica para manejar cambios en el ID
    }

    public void SetTeam(int newTeam) // Método para cambiar el ID
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetTeamServerRpc(newTeam); // Se modifica el ID en el servidor si el objeto existe y es el propietario
        }
    }

    [ServerRpc]
    private void SetTeamServerRpc(int newTeam) // Establecer el ID en el servidor
    {
        Team.Value = newTeam;
    }

    private void ApplyTeam(int X)
    {
        if (Team != null)
        {
            Team.Value = X;
        }
    }
    #endregion

    #region CHARACTER
    void OnCharacterChanged(FixedString128Bytes previous, FixedString128Bytes current)
    {
        // Lógica para manejar cambios en el personaje 
    }

    public void SetCharacter(FixedString128Bytes newCharacter) // Método para cambiar el personaje
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetCharacterServerRpc(newCharacter); // Se modifica el personaje en el servidor si el objeto existe y es el propietario
        }
    }

    [ServerRpc]
    private void SetCharacterServerRpc(FixedString128Bytes newCharacter) // Establecer el personaje en el servidor
    {
        Character.Value = newCharacter;
    }

    private void ApplyCharacter(FixedString128Bytes X)
    {
        if (Character != null)
        {
            Character.Value = X;
        }
    }
    #endregion

    #region KDA
    void OnKDAChanged(Vector3Int previous, Vector3Int current)
    {
        // Lógica para manejar cambios en KDA
        if (previous.x != current.x)
        {
            teamDeathmatchManager.SumarMuertes(Team.Value);
        }
    }

    public void SetKDA(Vector3Int newKDA) // Método para cambiar el KDA
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetKDAServerRpc(newKDA); // Se modifica el KDA en el servidor si el objeto existe y es el propietario
        }
    }

    [ServerRpc]
    private void SetKDAServerRpc(Vector3Int newKDA) // Establecer el KDA en el servidor
    {
        KDA.Value = newKDA;
    }

    private void ApplyKDA(Vector3Int X)
    {
        if (KDA != null)
        {
            KDA.Value = X;
        }
    }
    #endregion

    #region HEALTH
    void OnHealthChanged(int previous, int current)
    {
        // Lógica para manejar cambios en la salud
        //si llega a cero la palmas y tienes que reespawnear y sumar muertes y kills
    }

    public void SetHealth(int newHealth) // Método para cambiar la salud
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetHealthServerRpc(newHealth); // Se modifica la salud en el servidor si el objeto existe y es el propietario
        }
    }

    [ServerRpc]
    private void SetHealthServerRpc(int newHealth) // Establecer la salud en el servidor
    {
        Health.Value = newHealth;
    }

    private void ApplyHealth(int X)
    {
        if (Health != null)
        {
            Health.Value = X;
        }
    }
    #endregion
}