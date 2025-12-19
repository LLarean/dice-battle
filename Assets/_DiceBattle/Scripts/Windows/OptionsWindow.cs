using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Windows
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private Button _close;

        private void Start() => _close.onClick.AddListener(HandleCloseClick);

        private void OnDestroy() => _close.onClick.RemoveAllListeners();

        private void HandleCloseClick() => gameObject.SetActive(false);
    }
}
