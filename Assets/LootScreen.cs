using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class LootScreen : MonoBehaviour
    {
        [SerializeField] private Button _next;
        
        private void Start() => _next.onClick.AddListener(StartClick);

        private void OnDestroy() => _next.onClick.RemoveAllListeners();

        private void StartClick()
        {
            gameObject.SetActive(false);
            // TODO: SignalSystem.Raise - The effect is clicked (select sound)
        }
    }
}