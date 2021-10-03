using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;


namespace _Game.Scripts.Util.Editor
{
    
    #if UNITY_EDITOR

    public class PNGGenerator : MonoBehaviour
    {
        public List<GameObject> _gameObjects = new List<GameObject>();

        [Button]
        public void GenerateTextures()
        {
       
            for (var i = 0; i < _gameObjects.Count; i++)
            {
                var texture2D = AssetPreview.GetAssetPreview(_gameObjects[i]);

                string path = Path.Combine(Application.dataPath, string.Format("{0}/{1}.png", "", _gameObjects[i].name));
     
                File.WriteAllBytes(Application.dataPath+"\\GeneratedTextures\\"+_gameObjects[i].name+".png",  texture2D.EncodeToPNG ());
                AssetDatabase.Refresh();
           
            }
     
        }
    }
#endif
}
