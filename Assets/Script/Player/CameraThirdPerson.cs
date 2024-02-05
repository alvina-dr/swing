using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform playerMesh;
    [SerializeField] Transform orientation;
    [SerializeField] float rotationSpeed;

    private void Update()
    {
        Vector3 _viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = _viewDir.normalized;

        float _inputHorizontal = Input.GetAxisRaw("Horizontal");
        float _inputVertical = Input.GetAxisRaw("Vertical");
        Vector3 _inputDir = orientation.forward * _inputVertical + orientation.right * _inputHorizontal;
        if (_inputDir != Vector3.zero) {
            playerMesh.forward = Vector3.Slerp(playerMesh.forward, _inputDir.normalized, Time.deltaTime * rotationSpeed);
        }    }
}
