using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Game.Scripts.Behaviours;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class CarriedElementsDisplay : MonoBehaviour
{
   public List<ElementIcons> allIconsList = new List<ElementIcons>();
   public ElementContainerComponent ElementContainerComponent;
   public Image topSlotImage; 
   public List<Image> images;

   private Sprite defaultSprite;

   public void Awake()
   {
       defaultSprite = topSlotImage.sprite; 
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
       topSlotImage.sprite = allIconsList.Find(icons => icons._elementComponent._element.elementType == topElement._element.elementType).sprite;


       // set all ElementDisplays to disable





       // ElementContainerComponent.carryingElements
   }

#if UNITY_EDITOR
    
   [Button]
   public void GenerateTextures()
   {
       
       for (var i = 0; i < allIconsList.Count; i++)
       {
           var texture2D = AssetPreview.GetAssetPreview(allIconsList[i]._elementComponent.gameObject);

           string path = Path.Combine(Application.dataPath, string.Format("{0}/{1}.png", "", allIconsList[i]._elementComponent.name));
     
           File.WriteAllBytes(Application.dataPath+"\\GeneratedTextures\\"+allIconsList[i]._elementComponent.name+".png",  texture2D.EncodeToPNG ());
           AssetDatabase.Refresh();
           
       }
     
   }
#endif


    [System.Serializable]
    public struct ElementIcons
    {
        public ElementComponent _elementComponent;
        public Sprite sprite;
    }

}
