using TMPro;
using UnityEngine;

namespace DiceBattle
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;

        public void ShowAttempts(int attemptCount)
        {
            gameObject.SetActive(true);
            _message.text = $"There are {attemptCount} attempts left";
        }

        public void ShowRoll()
        {
            _message.text = "Click the Roll button!";
        }

        public void Show()
        {
             _message.text = "You can choose the dice to avoid throwing them";
        }
    }
}