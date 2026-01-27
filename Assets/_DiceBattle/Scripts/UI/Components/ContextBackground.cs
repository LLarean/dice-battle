using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class ContextBackground : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}
