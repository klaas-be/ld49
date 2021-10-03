using Assets.TekkTech.Scripts.Language;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.TekkTech.Scripts.Utility
{
    [System.Serializable]
    public class LocalizedString
    {
        public LanguageTags languageTag;
        public string Text {
            get => LocalizationManager.GetTextForTag(languageTag);
            private set { }
        }
        private UnityAction callToSet;

        public LocalizedString(LanguageTags tag, UnityAction callToSetString = null)
        {
            languageTag = tag;
            callToSet = callToSetString;

            LocalizationManager.instance.RegisterLanguageString(this);
        }

        public void SetAction(UnityAction callToSetString)
        {
            LocalizationManager.instance.UnregisterLanguageString(this);
            callToSet = callToSetString;
            LocalizationManager.instance.RegisterLanguageString(this);
        }

        public void LanguageSwitch()
        {
            Text = LocalizationManager.GetTextForTag(languageTag);
            callToSet?.Invoke();
        }

        public static implicit operator string(LocalizedString t) => t.Text;
        public static implicit operator LocalizedString(LanguageTags tag) => new LocalizedString(tag);

        public override string ToString() => Text;

        public override bool Equals(object obj)
        {
            return obj is LocalizedString localizedString && this.languageTag == localizedString.languageTag;
        }

        public override int GetHashCode()
        {
            return languageTag.GetHashCode();
        }
    }
}
