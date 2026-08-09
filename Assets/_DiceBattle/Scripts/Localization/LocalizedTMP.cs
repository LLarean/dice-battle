using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

namespace DiceBattle.Localization
{
    /// <summary>
    /// Localize TMP_Text component.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTMP : MonoBehaviour
    {
        [SerializeField] private string _localizationKey;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            _text.text = LocalizationManager.Localize(_localizationKey);
        }
    }
}
