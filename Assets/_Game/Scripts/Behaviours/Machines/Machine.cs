using _Game.Scripts.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Machine : MonoBehaviour
{
    public abstract void Use(ElementComponent elementComponent);
}
