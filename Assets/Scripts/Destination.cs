using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    // Complete this class. See Additional Steps 2.

    private void OnTriggerEnter(Collider other)
    {
        
        GridGameEventBus.Publish(MovementEventType.ARRIVED_AT_DESTINATION);
    }


}
