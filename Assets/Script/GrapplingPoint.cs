using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingPoint : MonoBehaviour
{
    public enum GrapplingType
    {
        OnPoint = 0,
        Acceleration = 1
    }

    public GrapplingType grapplingType;
}
