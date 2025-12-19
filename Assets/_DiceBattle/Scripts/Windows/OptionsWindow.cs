using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private Transform _substrate;
        [Space]
        [SerializeField] private Button _close;

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
            gameObject.SetActive(false);
        }

        private void OnDestroy() => _close.onClick.RemoveAllListeners();

        private void HandleCloseClick() => gameObject.SetActive(false);
    }
}
