using System;
using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceBattle.Animations
{
    public class DiceAnimation
    {
        private readonly RectTransform _safeArea;
        private readonly List<Vector2> _finalPositions = new();

        private Vector2 _rollAreaMin;
        private Vector2 _rollAreaMax;

        private const float _throwDuration = 0.8f;
        private const float _throwHeight = 3f;
        private const float _diceSize = 0.5f;

        private List<Dice> _dicesToRoll;

        public event Action OnDiceRollComplete;

        public DiceAnimation(RectTransform safeArea)
        {
            _safeArea = safeArea;
            // LeanTween.init(1000);
        }

        public void Animate(List<Dice> dices)
        {
            _dicesToRoll = dices;

            UpdateAreaBounds();
            GenerateNonOverlappingPositions(dices.Count);

            for (int i = 0; i < dices.Count; i++)
            {
                AnimateDice(dices[i], _finalPositions[i], i);
            }
        }

        private void UpdateAreaBounds()
        {
            var corners = new Vector3[4];
            _safeArea.GetWorldCorners(corners);
            _rollAreaMin = new Vector2(corners[0].x, corners[0].y);
            _rollAreaMax = new Vector2(corners[2].x, corners[2].y);

            float margin = _diceSize;
            _rollAreaMin += new Vector2(margin, margin);
            _rollAreaMax -= new Vector2(margin, margin);
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
                    newPos = new Vector2(
                        Random.Range(_rollAreaMin.x, _rollAreaMax.x),
                        Random.Range(_rollAreaMin.y, _rollAreaMax.y)
                    );

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

        private void AnimateDice(Dice dice, Vector2 targetPos, int index)
        {
            Vector3 startPos = dice.transform.position;
            Vector3 endPos = new Vector3(targetPos.x, targetPos.y, dice.transform.position.z);

            // Small delay for each dice
            float delay = index * 0.05f;

            // Movement animation with parabola using moveLocal and separate Y animation
            LeanTween.move(dice.gameObject, endPos, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutQuad);

            // Separate height animation (parabola)
            float currentY = startPos.y;
            LeanTween.value(dice.gameObject, currentY, currentY + _throwHeight, _throwDuration * 0.5f)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate((float val) =>
                {
                    Vector3 pos = dice.transform.position;
                    pos.y = val;
                    dice.transform.position = pos;
                });

            LeanTween.value(dice.gameObject, currentY + _throwHeight, endPos.y, _throwDuration * 0.5f)
                .setDelay(delay + _throwDuration * 0.5f)
                .setEase(LeanTweenType.easeInQuad)
                .setOnUpdate((float val) =>
                {
                    Vector3 pos = dice.transform.position;
                    pos.y = val;
                    dice.transform.position = pos;
                });

            // Rotation on all axes for roll effect
            float randomRotX = Random.Range(2, 5) * 360f;
            float randomRotY = Random.Range(2, 5) * 360f;
            float randomRotZ = Random.Range(2, 5) * 360f;

            LeanTween.rotateX(dice.gameObject, randomRotX, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad);

            LeanTween.rotateY(dice.gameObject, randomRotY, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad);

            LeanTween.rotateZ(dice.gameObject, randomRotZ, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => DiceRollComplete(index));

            LeanTween.scale(dice.gameObject, dice.transform.localScale * 1.1f, _throwDuration * 0.3f)
                .setDelay(delay + _throwDuration * 0.7f)
                .setEase(LeanTweenType.easeOutQuad)
                .setLoopPingPong(1);
        }

        private void DiceRollComplete(int index)
        {
            _dicesToRoll[index].Roll();

            if (index >= _dicesToRoll.Count - 1)
            {
                LeanTween.delayedCall(1f, () => OnDiceRollComplete?.Invoke());
            }
        }
    }
}
