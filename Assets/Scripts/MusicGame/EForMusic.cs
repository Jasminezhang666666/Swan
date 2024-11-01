using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EForMusic : EInteractable 
{
    public override void Interact()
    {
        gameObject.GetComponent<MusicTransition>().TransitionToScene();
    }
}
