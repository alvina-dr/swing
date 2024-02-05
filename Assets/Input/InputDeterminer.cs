using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class InputDeterminer : MonoBehaviour
{
    public InputActionReference XYaxis;
    public CinemachineInputProvider inputProvider;

    private void Update()
    {
        if (Input.GetJoystickNames().Length == 0 && inputProvider != null)
        {
            Destroy(transform.GetComponent<CinemachineInputProvider>());
            inputProvider = null;
        } else if (Input.GetJoystickNames().Length > 0 && inputProvider == null)
        {
            inputProvider = gameObject.AddComponent<CinemachineInputProvider>();
            inputProvider.XYAxis = XYaxis;
        }
    }
}
