using System;
using UnityEngine;
using UnityEngine.Events;

namespace DilmerGames.xrdemo
{
[Serializable]
public class ARObjectPlacementEvent : UnityEvent<ARPlacementInteractableSingle, GameObject> 
{ 
    
}
}