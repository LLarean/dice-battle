using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class Loader : MonoBehaviour
    {
        private const float _minimumDuration = 2f;
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

        // The main scene loads behind the animation; the minimum leaves time to read the tip on a fast load.
        // deltaTime is capped per frame, so a startup hitch does not eat that time.
        private IEnumerator PlayAnimation()
        {
            AsyncOperation loading = SceneManager.LoadSceneAsync("Main");
            loading.allowSceneActivation = false;

            float elapsed = 0f;
            float nextSpriteChange = _spriteChangeTime;

            while (elapsed < _minimumDuration || loading.progress < 0.9f)
            {
                yield return null;
                elapsed += Time.deltaTime;

                if (elapsed >= nextSpriteChange)
                {
                    _currentIndex = (_currentIndex + 1) % _sprites.Length;
                    _image.sprite = _sprites[_currentIndex];
                    nextSpriteChange += _spriteChangeTime;
                }
            }

            loading.allowSceneActivation = true;
        }
    }
}
