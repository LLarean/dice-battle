using System;
using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceBattle
{
    public class DiceRollAnimation : MonoBehaviour
    {
        [Header("Dice Settings")]
        [SerializeField] private List<Dice> _dices = new();

        [Header("Roll Area")]
        [SerializeField] private GameObject _rollAreaObject;
        [SerializeField] private Vector2 _rollAreaMin = new Vector2(-3f, -2f);
        [SerializeField] private Vector2 _rollAreaMax = new Vector2(3f, 2f);

        [Header("Animation Settings")]
        [SerializeField] private float _throwDuration = 0.8f;
        [SerializeField] private float _throwHeight = 3f;
        [SerializeField] private float _rotationSpeed = 720f;
        [SerializeField] private float _diceSize = 0.5f;

        private List<Vector2> _finalPositions = new();

        public event Action OnDiceRollComplete;

        private void Start()
        {
            LeanTween.init(1000);

            // Check if dices are assigned
            if (_dices.Count == 0)
            {
                Debug.LogError("Add dice to the list!");
            }
        }

        public void RollDices(List<Dice> dicesToRoll)
        {
            UpdateAreaBounds();
            GenerateNonOverlappingPositions(dicesToRoll.Count);

            for (int i = 0; i < dicesToRoll.Count; i++)
            {
                AnimateDice(dicesToRoll[i].gameObject, _finalPositions[i], i);
            }
        }

        // Roll dice by indices (e.g., [0, 2, 4] for 1st, 3rd and 5th dice)
        public void RollDicesByIndices(List<int> indices)
        {
            List<Dice> dicesToRoll = new List<Dice>();

            foreach (int index in indices)
            {
                if (index >= 0 && index < _dices.Count)
                {
                    dicesToRoll.Add(_dices[index]);
                }
            }

            RollDices(dicesToRoll);
        }

        private void UpdateAreaBounds()
        {
            if (_rollAreaObject != null)
            {
                // Get bounds from SpriteRenderer, Collider2D or RectTransform
                SpriteRenderer sprite = _rollAreaObject.GetComponent<SpriteRenderer>();
                Collider2D col = _rollAreaObject.GetComponent<Collider2D>();
                RectTransform rect = _rollAreaObject.GetComponent<RectTransform>();

                if (sprite != null)
                {
                    Bounds bounds = sprite.bounds;
                    _rollAreaMin = new Vector2(bounds.min.x, bounds.min.y);
                    _rollAreaMax = new Vector2(bounds.max.x, bounds.max.y);
                }
                else if (col != null)
                {
                    Bounds bounds = col.bounds;
                    _rollAreaMin = new Vector2(bounds.min.x, bounds.min.y);
                    _rollAreaMax = new Vector2(bounds.max.x, bounds.max.y);
                }
                else if (rect != null)
                {
                    // For UI elements
                    Vector3[] corners = new Vector3[4];
                    rect.GetWorldCorners(corners);
                    _rollAreaMin = new Vector2(corners[0].x, corners[0].y);
                    _rollAreaMax = new Vector2(corners[2].x, corners[2].y);
                }
                else
                {
                    Debug.LogWarning("Roll Area Object should have SpriteRenderer, Collider2D or RectTransform!");
                }

                // Add margin from edges (so dice don't go outside bounds)
                float margin = _diceSize;
                _rollAreaMin += new Vector2(margin, margin);
                _rollAreaMax -= new Vector2(margin, margin);
            }
        }

        private void GenerateNonOverlappingPositions(int diceCount)
        {
            _finalPositions.Clear();
            int maxAttempts = 100;

            for (int i = 0; i < diceCount; i++)
            {
                Vector2 newPos = Vector2.zero;
                bool validPosition = false;
                int attempts = 0;

                while (!validPosition && attempts < maxAttempts)
                {
                    // Generate random position in area
                    newPos = new Vector2(
                        Random.Range(_rollAreaMin.x, _rollAreaMax.x),
                        Random.Range(_rollAreaMin.y, _rollAreaMax.y)
                    );

                    // Check overlap with already placed dice
                    validPosition = true;
                    foreach (Vector2 existingPos in _finalPositions)
                    {
                        float distance = Vector2.Distance(newPos, existingPos);
                        if (distance < _diceSize * 2.2f) // 2.2f for small gap
                        {
                            validPosition = false;
                            break;
                        }
                    }

                    attempts++;
                }

                _finalPositions.Add(newPos);
            }
        }

        private void AnimateDice(GameObject dice, Vector2 targetPos, int index)
        {
            Vector3 startPos = dice.transform.position;
            Vector3 endPos = new Vector3(targetPos.x, targetPos.y, dice.transform.position.z);

            // Small delay for each dice
            float delay = index * 0.05f;

            // Movement animation with parabola using moveLocal and separate Y animation
            LeanTween.move(dice, endPos, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutQuad);

            // Separate height animation (parabola)
            float currentY = startPos.y;
            LeanTween.value(dice, currentY, currentY + _throwHeight, _throwDuration * 0.5f)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate((float val) => {
                    Vector3 pos = dice.transform.position;
                    pos.y = val;
                    dice.transform.position = pos;
                });

            LeanTween.value(dice, currentY + _throwHeight, endPos.y, _throwDuration * 0.5f)
                .setDelay(delay + _throwDuration * 0.5f)
                .setEase(LeanTweenType.easeInQuad)
                .setOnUpdate((float val) => {
                    Vector3 pos = dice.transform.position;
                    pos.y = val;
                    dice.transform.position = pos;
                });

            // Rotation on all axes for roll effect
            float randomRotX = Random.Range(2, 5) * 360f;
            float randomRotY = Random.Range(2, 5) * 360f;
            float randomRotZ = Random.Range(2, 5) * 360f;

            LeanTween.rotateX(dice, randomRotX, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad);

            LeanTween.rotateY(dice, randomRotY, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad);

            LeanTween.rotateZ(dice, randomRotZ, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => DiceRollComplete(dice, index));

            // Small scale effect on landing
            LeanTween.scale(dice, dice.transform.localScale * 1.1f, _throwDuration * 0.3f)
                .setDelay(delay + _throwDuration * 0.7f)
                .setEase(LeanTweenType.easeOutQuad)
                .setLoopPingPong(1);
        }

        private void DiceRollComplete(GameObject dice, int index)
        {
            // Here you can add logic after animation completion
            if (index == _dices.Count - 1)
            {
                Debug.Log("All dice have landed!");

                OnDiceRollComplete?.Invoke();
                // Call your callback or event
            }
        }

        // Visualize roll area in editor
        private void OnDrawGizmos()
        {
            if (_rollAreaObject != null)
            {
                UpdateAreaBounds();
            }

            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (_rollAreaMin.x + _rollAreaMax.x) / 2f,
                (_rollAreaMin.y + _rollAreaMax.y) / 2f,
                0
            );
            Vector3 size = new Vector3(
                _rollAreaMax.x - _rollAreaMin.x,
                _rollAreaMax.y - _rollAreaMin.y,
                0.1f
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}
