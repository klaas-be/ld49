using static _Game.Scripts.Classes.Element;

namespace _Game.Scripts.Classes
{
    public static class CombineRecipes
    {
        public static Element GetResultOf(Element A, Element B, bool recursive = false)
        {
            //burned steel + star = starsteel
            if (A.elementType == ElementType.Steel      && A.elementStatus == ElementStatus.Burned &&
                B.elementType == ElementType.Star       && B.elementStatus == ElementStatus.Default)
            {
                return new Element(ElementType.Starsteel);
            }

            //helium + uran = bomb
            if (A.elementType == ElementType.Helium     && A.elementStatus == ElementStatus.Default &&
                B.elementType == ElementType.Uran       && B.elementStatus == ElementStatus.Default)
            {
                return new Element(ElementType.Bomb);
            }

            //elec steel + helium = lightsteel
            if (A.elementType == ElementType.Steel      && A.elementStatus == ElementStatus.Shocked &&
                B.elementType == ElementType.Helium     && B.elementStatus == ElementStatus.Default)
            {
                return new Element(ElementType.Lightsteel);
            }

            //star comp +star comp = whitestar
            if (A.elementType == ElementType.Star       && A.elementStatus == ElementStatus.Compressed &&
                B.elementType == ElementType.Star       && B.elementStatus == ElementStatus.Compressed)
            {
                return new Element(ElementType.Whitestar);
            }
            //star burn +star elec = whitestar
            if (A.elementType == ElementType.Star       && A.elementStatus == ElementStatus.Burned &&
                B.elementType == ElementType.Star       && B.elementStatus == ElementStatus.Shocked)
            {
                return new Element(ElementType.Shootingstar);
            }

            //TEST STEEL STEEL 
            if (A.elementType == ElementType.Steel      && A.elementStatus == ElementStatus.Default &&
                B.elementType == ElementType.Steel      && B.elementStatus == ElementStatus.Default)
            {
                return new Element(ElementType.Steel);
            }


            if (recursive)
                return null;

            //Falls es andersrum passt
            return GetResultOf(B,A, true);
        }
    }
}