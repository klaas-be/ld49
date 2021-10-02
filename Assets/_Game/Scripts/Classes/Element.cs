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
            Electrocuted,
            Compressed,
        }

        [SerializeField]
        public ElementType elementType = ElementType.Steel;

        [ReadOnly]
        [SerializeField]
        public ElementStatus elementStatus = ElementStatus.Default;

        public Element(ElementType type, ElementStatus status = ElementStatus.Default)
        {
            elementType = type;
            switch (status)
            {
                case ElementStatus.Burned:
                    Burn();
                    break;
                case ElementStatus.Electrocuted:
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
        //star + burning = shooting star
        public void Burn()
        {
            if (elementType == ElementType.Star)
            {
                elementType = ElementType.Shootingstar;
                return;
            }

            elementStatus = ElementStatus.Burned;
        }
        public void Electrocute()
        {
            elementStatus = ElementStatus.Electrocuted;
        }
        public void Compress()
        {
            elementStatus = ElementStatus.Compressed;
        }
    }
}