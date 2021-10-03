using TMPro;
using UnityEngine;

namespace Assets.TekkTech.Scripts.Utility
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    public class UiTextSetter : MonoBehaviour
    {
        public LocalizedString localizedText;

        private TMP_Text uiText;

        private void Start()
        {
            localizedText.SetAction(SetText);
            uiText = this.GetComponent<TMP_Text>();
            SetText();
        }

        public void SetText()
        {
            if(uiText is null)
                uiText = this.GetComponent<TMP_Text>();

            uiText.text = localizedText;
        }
    }
}
