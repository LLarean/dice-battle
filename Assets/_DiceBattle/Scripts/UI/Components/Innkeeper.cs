using DiceBattle.Data;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class Innkeeper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;
        [Space]
        [SerializeField] private InnkeeperConfig _config;

        public void ShowMessage()
        {
            AnimateIn();

            // TODO Add translation to other languages
            int randomCount = Random.Range(0, _config.Messages.Count);
            _message.text =  _config.Messages[randomCount];
        }

        private void AnimateIn()
        {
            // TODO Add animation
        }

    }
}
