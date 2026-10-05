using System;
using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceBattle.Animations
{
    public static class DiceAnimation
    {
        private static RectTransform _safeArea;
        private static readonly List<Vector2> _finalPositions = new();

        private static Vector2 _rollAreaMin;
        private static Vector2 _rollAreaMax;

        private const float _throwDuration = 0.8f;
        private const float _throwScale = 1.35f;
        private const float _spacing = 1.1f;
        private const float _landSquash = 0.85f;
        private const float _landDuration = 0.08f;
        private const float _settleDelay = 0.45f;

        private static Vector2 _diceSize;

        private static List<Dice> _dicesToRoll;

        public static event Action OnDiceRollComplete;

        public static void Animate(List<Dice> dices, RectTransform safeArea)
        {
            if (dices.Count == 0)
            {
                return;
            }

            _safeArea = safeArea;
            _dicesToRoll = dices;

            DisableIcons();
            UpdateAreaBounds();
            GenerateNonOverlappingPositions(dices.Count);

            for (int i = 0; i < dices.Count; i++)
            {
                AnimateDice(dices[i], _finalPositions[i], i);
            }
        }

        private static void DisableIcons()
        {
            foreach (Dice dice in _dicesToRoll)
            {
                dice.ResetToEmpty();
            }
        }

        private static void UpdateAreaBounds()
        {
            var corners = new Vector3[4];
            _safeArea.GetWorldCorners(corners);
            _rollAreaMin = new Vector2(corners[0].x, corners[0].y);
            _rollAreaMax = new Vector2(corners[2].x, corners[2].y);

            // The canvas is Screen Space - Overlay, so world units are pixels and depend on the canvas scale.
            var diceRect = (RectTransform)_dicesToRoll[0].transform;
            _diceSize = Vector2.Scale(diceRect.rect.size, diceRect.lossyScale);

            _rollAreaMin += _diceSize * 0.5f;
            _rollAreaMax -= _diceSize * 0.5f;
        }

        private static void GenerateNonOverlappingPositions(int diceCount)
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
                        Vector2 offset = newPos - existingPos;
                        if (Mathf.Abs(offset.x) < _diceSize.x * _spacing && Mathf.Abs(offset.y) < _diceSize.y * _spacing)
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

        private static void AnimateDice(Dice dice, Vector2 targetPos, int index)
        {
            Vector3 endPos = new Vector3(targetPos.x, targetPos.y, dice.transform.position.z);

            // Small delay for each dice
            float delay = index * 0.05f;

            LeanTween.move(dice.gameObject, endPos, _throwDuration)
                .setDelay(delay)
                .setEase(LeanTweenType.easeOutCubic);

            // Top-down throw: the dice grows while in the air and shrinks back on landing
            LeanTween.value(dice.gameObject, 0f, Mathf.PI, _throwDuration)
                .setDelay(delay)
                .setOnUpdate((float angle) => dice.transform.localScale = Vector3.one * (1f + (_throwScale - 1f) * Mathf.Sin(angle)));

            AnimateFaceCycling(dice, delay);

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
        }

        private static void AnimateFaceCycling(Dice dice, float delay)
        {
            const float faceChangeInterval = 0.07f;
            int steps = Mathf.CeilToInt(_throwDuration / faceChangeInterval);
            float lastStep = -1f;

            LeanTween.value(dice.gameObject, 0f, steps, _throwDuration)
                .setDelay(delay)
                .setOnUpdate((float val) =>
                {
                    float step = Mathf.Floor(val);
                    if (step != lastStep)
                    {
                        lastStep = step;
                        dice.ShowRandomFace();
                    }
                });
        }

        private static void DiceRollComplete(int index)
        {
            Dice dice = _dicesToRoll[index];
            dice.Roll();

            LeanTween.scale(dice.gameObject, Vector3.one * _landSquash, _landDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => LeanTween.scale(dice.gameObject, Vector3.one, _landDuration).setEase(LeanTweenType.easeOutQuad));

            if (index >= _dicesToRoll.Count - 1)
            {
                LeanTween.delayedCall(_settleDelay, () => OnDiceRollComplete?.Invoke());
            }
        }
    }
}
