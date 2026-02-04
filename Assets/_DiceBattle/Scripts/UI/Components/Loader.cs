using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class Loader : MonoBehaviour
    {
        private const float _loadDuration = 6f;
        private const float _spriteChangeTime = 0.5f;

        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _sprites;

        private int _currentIndex;
        private bool _isAnimating;

        public void StartAnimation()
        {
            _isAnimating = true;
            _currentIndex = 0;
            _image.sprite = _sprites[0];

            StartCoroutine(PlayAnimation());
        }

        private void StopAnimation()
        {
            _isAnimating = false;
            LeanTween.cancel(_image.gameObject);
            StopAllCoroutines();
        }

        private IEnumerator PlayAnimation()
        {
            for (int i = 0; i <= _loadDuration; i++)
            {
                int spriteIndex = i % _sprites.Length;
                _image.sprite = _sprites[spriteIndex];

                if (i < _loadDuration)
                {
                    yield return new WaitForSeconds(_spriteChangeTime);
                }
            }

            SceneManager.LoadScene("SampleScene");
        }
    }
}
