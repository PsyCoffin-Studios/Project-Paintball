using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MobileChecker : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
            [System.Runtime.InteropServices.DllImport("__Internal")]
            private static extern void DetectDevice();
#endif

    public GameObject canvasInGameSmartPhone;
    public bool playingInSmartPhone = false;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                            DetectDevice();
#endif
        if (transform.parent.GetComponent<PlayerController>().IsOwner)
        {

            if (!playingInSmartPhone)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                canvasInGameSmartPhone.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnMobileDetected()
    {
            playingInSmartPhone = true;
    }
}
