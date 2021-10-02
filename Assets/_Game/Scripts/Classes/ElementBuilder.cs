namespace _Game.Scripts.Classes
{
    public class ElementBuilder
    {
        private Element.ElementType _type;
        private Element.ElementStatus _status;

        public ElementBuilder OfType(Element.ElementType type)
        {
            this._type = type;
            return this; 
        }
        
        public ElementBuilder WithStatus(Element.ElementStatus status)
        {
            _status = status;
            return this; 
        }

        public Element Build()
        {
            return new Element(this._type, this._status);
        }

        public static implicit operator Element(ElementBuilder builder)
        {
            return builder.Build();
        }
    }
}