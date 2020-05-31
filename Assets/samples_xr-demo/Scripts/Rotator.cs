using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DilmerGames.xrdemo
{
public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 speed;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime, Space.Self);
    }
}
}