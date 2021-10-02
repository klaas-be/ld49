using _Game.Scripts.Behaviours;
using UnityEngine;

public abstract class Machine : MonoBehaviour
{
    public bool canBeUsed = true;
    public abstract void Use(ElementComponent elementComponent);
}
