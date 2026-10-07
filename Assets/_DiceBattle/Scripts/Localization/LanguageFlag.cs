using System;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Localization
{
    [RequireComponent(typeof(Image))]
    public class LanguageFlag : MonoBehaviour
    {
        [Serializable]
        private class Flag
        {
            public SystemLanguage Language;
            public Sprite Sprite;
        }

        [SerializeField] private List<Flag> _flags;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            Refresh();
            LocalizationManager.OnLocalizationChanged += Refresh;
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Refresh;
        }

        private void Refresh()
        {
            Flag flag = _flags.Find(item => item.Language.ToString() == LocalizationManager.Language);

            if (flag != null)
            {
                _image.sprite = flag.Sprite;
            }
        }
    }
}
