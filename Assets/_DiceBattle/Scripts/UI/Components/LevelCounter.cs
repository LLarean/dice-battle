using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class LevelCounter : MonoBehaviour
    {
        [SerializeField] private Image _state;
        [SerializeField] private TextMeshProUGUI _label;
        [Space]
        [SerializeField] private Sprite _empty;
        [SerializeField] private Sprite _success;
        [SerializeField] private Sprite _defeat;

        public void SetEmptyState(string label)
        {
            _state.sprite = _empty;
            _label.text = label;
            _label.gameObject.SetActive(true);
        }

        public void SetSuccessState()
        {
            _state.sprite = _success;
            _label.gameObject.SetActive(false);
        }

        public void SetDefeatState()
        {
            _state.sprite = _defeat;
            _label.gameObject.SetActive(false);
        }
    }
}
