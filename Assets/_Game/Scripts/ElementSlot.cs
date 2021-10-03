using _Game.Scripts.Classes;
using UnityEngine;

public class ElementSlot : MonoBehaviour
{
    public Element _element; 
    
    // Start is called before the first frame update
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
    
}
