using _Game.Scripts.Behaviours;
using NaughtyAttributes;
using UnityEngine;

namespace _Game.Scripts.Classes
{
    [System.Serializable]
    public  class Element
    {
        public enum ElementType
        {
            Steel,
            Uran,
            Star,
            Helium,
            Starsteel,
            Shootingstar,
            Bomb,
            Lightsteel,
            Whitestar,
        }
        public enum ElementStatus
        {
            Default,
            Burned,
            Shocked,
            Compressed,
        }

        [SerializeField]
        public ElementType elementType = ElementType.Steel;

        [ReadOnly]
        [SerializeField]
        public ElementStatus elementStatus = ElementStatus.Default;

        [ReadOnly]
        [SerializeField]
        private GameObject burnParticles;
        [ReadOnly]
        [SerializeField]
        private GameObject shockerGO;

        public Element(ElementType type, ElementStatus status = ElementStatus.Default)
        {
            elementType = type;
            switch (status)
            {
                case ElementStatus.Burned:
                    Burn();
                    break;
                case ElementStatus.Shocked:
                    Electrocute();
                    break;
                case ElementStatus.Compressed:
                    Compress();
                    break;
                case ElementStatus.Default:
                default:
                    break;
            }
        }

        //Mechanismen
        public void ResetStatus()
        {
            elementStatus = ElementStatus.Default;
            burnParticles.SetActive(false);
            shockerGO.SetActive(false);
        }
        public void Burn()
        {
            elementStatus = ElementStatus.Burned;
            burnParticles.SetActive(true);
        }
        public void Electrocute()
        {
            elementStatus = ElementStatus.Shocked;
            shockerGO.SetActive(true);
        }
        public void Compress()
        {
            elementStatus = ElementStatus.Compressed;
        }
    }
}