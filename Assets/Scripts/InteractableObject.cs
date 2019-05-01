using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
    }

    public abstract void Interact();

    // void OnMouseDown()
    // { // right-click = inspect
    // }
}
