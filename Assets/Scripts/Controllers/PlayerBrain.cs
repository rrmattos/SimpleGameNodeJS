using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : Brain
{
    AudioBehaviour audioBehaviour;
    //public override List<Component> components { get; set; } = new List<Component>();
    

    private void Start()
    {
        GetObjectComponents();

        if (HasComponent(typeof(PlayerBrain)))
        {
            Debug.Log($"{gameObject.name} - count: {currentComponentsCount}");
        }       
    }


}
