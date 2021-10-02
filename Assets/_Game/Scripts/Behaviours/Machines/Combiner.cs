using _Game.Scripts.Behaviours;
using _Game.Scripts.Classes;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combiner : Machine
{
    [SerializeField] Transform AnchorA, AnchorB;

    [ReadOnly]
    [SerializeField] ElementComponent A, B;

    public override void Use(ElementComponent elementComponent)
    {
        //Objekte kombinieren
        //TODO

        //ElementSpawner.Spawn(ElementType type, Vector3 position);
    }
}
