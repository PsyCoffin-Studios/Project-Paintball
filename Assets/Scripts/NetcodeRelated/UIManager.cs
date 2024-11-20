using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.AudioSettings;


namespace HelloWorld
{
    public class UIManager : MonoBehaviour
    {
        private string joinCode;
        private const int maxConnections = 3;

        public Sprite[] characterSprites;
        private GameObject personajeElegido;

        private GameObject canvasUI;
        public GameObject canvasInGame;
        public TextMeshProUGUI errorText;



        async void Start()
        {
            

            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn +=
                () => print($"New player {AuthenticationService.Instance.PlayerId} connected");

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            personajeElegido = GameObject.Find("PersonajeElegidoUI");
            canvasUI = GameObject.Find("CanvasNETCODE");
            errorText = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();

            mostrarPersonaje();

        }

        private void mostrarPersonaje()
        {
            Image imagen = personajeElegido.GetComponent<Image>();

            switch (DataBetweenScenes.instance.GetNombre())
            {
                case "Joker":

                    imagen.sprite = characterSprites[0];

                    break;
                case "Outcast":

                    imagen.sprite = characterSprites[1];

                    break;
                case "Revenant":

                    imagen.sprite = characterSprites[2];

                    break;
                case "Hex":

                    imagen.sprite = characterSprites[3];

                    break;

            }

            imagen.preserveAspect = false;
            if(imagen.sprite != null) {
                imagen.enabled = true;
            }


        }

        void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ? "Host" :
                NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);

            GUILayout.Label("Room: " + joinCode);
        }

        public async void StartHost()
        {
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(new RelayServerData(allocation, "wss"));
                joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                // Copiar el código de unión al portapapeles
                GUIUtility.systemCopyBuffer = joinCode;

                NetworkManager.Singleton.StartHost();

                canvasUI.SetActive(false);
                canvasInGame.SetActive(true);
                canvasInGame.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text
                    = "PROTOTYPE VERSION\r\nROOM CODE: " + joinCode;
            }
            catch (RelayServiceException e)
            {
                print(e);
                ShowErrorMessage("Error: No se ha podido iniciar una partida.");
            }
        }

        public async void StartClient(string joinCode)
        {
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(new RelayServerData(joinAllocation, "wss"));
                NetworkManager.Singleton.StartClient();

                canvasUI.SetActive(false);
                canvasInGame.SetActive(true);
                canvasInGame.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text
                    = "PROTOTIPE VERSION\r\nROOM CODE: "+joinCode;
            }
            catch (RelayServiceException e)
            {
                print(e);
                ShowErrorMessage("Error: No se pudo unir al juego. Verifica el codigo e intentalo de nuevo.");
            }
        }

       public async void volverAlInicioAsync()
        {
            AuthenticationService.Instance.SignedOut +=
                () => print($"Player {AuthenticationService.Instance.PlayerId} disconnected");

            AuthenticationService.Instance.SignOut();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        private void ShowErrorMessage(string message)
        {
            if (errorText != null)
            {
                errorText.text = message;  // Actualiza el textoDebug del mensaje de error
                errorText.gameObject.SetActive(true);  // Asegúrate de que el textoDebug esté visible
            }
            else
            {
                Debug.LogWarning("No se ha asignado el componente de textoDebug para los errores.");
            }
        }

        public string GetCode()
        {
            return joinCode;
        }

        public string GetID()
        {
            return AuthenticationService.Instance.PlayerId;
        }

    }
}