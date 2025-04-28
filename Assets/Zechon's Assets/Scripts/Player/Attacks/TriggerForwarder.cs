using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerForwarder : MonoBehaviour
{
    [System.Serializable]
    public class TriggerEvent : UnityEvent<Collider> { }

    public TriggerEvent onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
        Debug.Log("Hit");
    }
}
