using TMPro;
using UnityEngine;

namespace Assets.TekkTech.Scripts.Utility
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UiTextSetter : MonoBehaviour
    {
        public LocalizedString localizedText;

        private TextMeshProUGUI uiText;

        private void Start()
        {
            localizedText.SetAction(SetText);
            uiText = this.GetComponent<TextMeshProUGUI>();
            SetText();
        }

        public void SetText()
        {
            if(uiText is null)
                uiText = this.GetComponent<TextMeshProUGUI>();

            uiText.text = localizedText;
        }
    }
}
