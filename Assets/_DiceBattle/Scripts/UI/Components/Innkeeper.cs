using DiceBattle.Global;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class Innkeeper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;

        public void ShowMessage()
        {
            AnimateIn();
            int completedLevels = GameProgress.CompletedLevels;

            // TODO Separate it into a separate logic
            // Add translation to other languages
            if (completedLevels == 0)
            {
                _message.text = "Добро пожаловать в таверну!";
            }
            else
            {
                _message.text = "Как ваши приключения?";
            }
        }

        private void AnimateIn()
        {
            // TODO Add animation
        }

    }
}
