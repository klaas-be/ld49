using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Game.Scripts.Behaviours;
using NaughtyAttributes;

using UnityEngine;
using UnityEngine.UI;



public class CarriedElementsDisplay : MonoBehaviour
{
    public ElementContainerComponent ElementContainerComponent;
   public List<Image> elementDisplays = new List<Image>(); 
   public Image topSlotImage; 
   public List<Image> images;

   private Sprite defaultSprite;
   

   public void Awake()
   {
       defaultSprite = topSlotImage.sprite;
       if(ElementContainerComponent == null)
            ElementContainerComponent = GameObject.FindWithTag("Player").GetComponent<ElementContainerComponent>();
   }

   public void Update()
   {
       if (ElementContainerComponent.carryingElements.Count == 0)
       {
           topSlotImage.sprite = defaultSprite; 
           return;
       }

       var iconImages = FindObjectsOfType<Image>().Where(image => image.gameObject.name == "Icon").ToArray();
       
       //display the first element in main display
       var topElement = ElementContainerComponent.carryingElements.Last();
       topSlotImage.sprite = ElementSpawner.Instance.ElementSettings.Find(settings => settings.Type == topElement._element.elementType).icon;


       // set all ElementDisplays to disable
       foreach (var ed in elementDisplays)
       {
           ed.gameObject.transform.parent.gameObject.SetActive(false);
       }

       for (int i = 0; i < ElementContainerComponent.carryingElements.Count-1; i++)
       {
           var element = ElementContainerComponent.carryingElements[i]._element;
           elementDisplays[i].sprite = ElementSpawner.Instance.ElementSettings.Find(settings => settings.Type == element.elementType).icon;
           elementDisplays[i].gameObject.transform.parent.gameObject.SetActive(true);
       }
   }

}
