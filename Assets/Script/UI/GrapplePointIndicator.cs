using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplePointIndicator : MonoBehaviour
{
    [SerializeField] private GameObject sprite;

    public void ShowAtWorldPosition(Vector3 _worldPosition)
    {
        sprite.SetActive(true);
        transform.position = _worldPosition + (Camera.main.transform.position - _worldPosition).normalized * 0.05f;
    }
    public void Hide()
    {
        sprite.SetActive(false);
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
