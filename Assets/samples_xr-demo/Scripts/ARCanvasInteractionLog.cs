﻿using TMPro;
using UnityEngine;

namespace DilmerGames.xrdemo
{
public class ARCanvasInteractionLog : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI details;


    public void Toggle()
    {
        details.gameObject.SetActive(!details.gameObject.activeSelf);
    }
}
}