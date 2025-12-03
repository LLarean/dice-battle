using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle
{
    /// <summary>
    /// Manages dice roll animations
    /// </summary>
    public class DiceRollAnimation : MonoBehaviour
    {
        [Header("Dice Settings")]
        [SerializeField] private List<GameObject> _dices = new List<GameObject>();
    
        [Header("Roll Area")]
        [SerializeField] private GameObject _rollAreaObject;
        [SerializeField] private Vector2 _rollAreaMin = new Vector2(-3f, -2f);
        [SerializeField] private Vector2 _rollAreaMax = new Vector2(3f, 2f);
    
        [Header("Animation Settings")]
        [SerializeField] private float _throwDuration = 0.8f;
        [SerializeField] private float _throwHeight = 3f;
        [SerializeField] private float _rotationSpeed = 720f;
        [SerializeField] private float _diceSize = 0.5f;
    
        private List<Vector2> _finalPositions = new List<Vector2>();
    
        /// <summary>
        /// List of dice game objects
        /// </summary>
        public List<GameObject> Dices => _dices;
    
        /// <summary>
        /// Roll area object reference
        /// </summary>
        public GameObject RollAreaObject => _rollAreaObject;
    
        /// <summary>
        /// Minimum bounds of roll area
        /// </summary>
        public Vector2 RollAreaMin => _rollAreaMin;
    
        /// <summary>
        /// Maximum bounds of roll area
        /// </summary>
        public Vector2 RollAreaMax => _rollAreaMax;
    
        /// <summary>
        /// Duration of throw animation
        /// </summary>
        public float ThrowDuration => _throwDuration;
    
        /// <summary>
        /// Height of throw arc
        /// </summary>
        public float ThrowHeight => _throwHeight;
    
        /// <summary>
        /// Rotation speed in degrees per second
        /// </summary>
        public float RotationSpeed => _rotationSpeed;
    
        /// <summary>
        /// Dice size for overlap checking
        /// </summary>
        public float DiceSize => _diceSize;

        /// <summary>
        /// Initialization
        /// </summary>
        private void Start()
        {
            // Check if dices are assigned
            if (_dices.Count == 0)
            {
                Debug.LogError("Add dice objects to the list!");
            }
        }

        /// <summary>
        /// Roll all dice in the list
        /// </summary>
        public void RollAllDices()
        {
            RollDices(_dices);
        }
    
        /// <summary>
        /// Roll specific dice from the list
        /// </summary>
        /// <param name="dicesToRoll">List of dice game objects to roll</param>
        public void RollDices(List<GameObject> dicesToRoll)
        {
            if (dicesToRoll == null || dicesToRoll.Count == 0)
            {
                Debug.LogWarning("No dice to roll!");
                return;
            }
        
            UpdateAreaBounds();
            GenerateNonOverlappingPositions(dicesToRoll.Count);
        
            for (int i = 0; i < dicesToRoll.Count; i++)
            {
                AnimateDice(dicesToRoll[i], _finalPositions[i], i);
            }
        }
    
        /// <summary>
        /// Roll dice by their indices
        /// </summary>
        /// <param name="indices">List of dice indices to roll</param>
        public void RollDicesByIndices(List<int> indices)
        {
            List<GameObject> dicesToRoll = new List<GameObject>();
        
            foreach (int index in indices)
            {
                if (index >= 0 && index < _dices.Count)
                {
                    dicesToRoll.Add(_dices[index]);
                }
            }
        
            RollDices(dicesToRoll);
        }

        /// <summary>
        /// Update roll area bounds from the roll area object
        /// </summary>
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

        /// <summary>
        /// Generate non-overlapping positions for dice
        /// </summary>
        /// <param name="diceCount">Number of dice to position</param>
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
                    // Generate random position within area
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

        /// <summary>
        /// Animate a single dice
        /// </summary>
        /// <param name="dice">Dice game object</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="index">Dice index</param>
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
                .setOnComplete(() => OnDiceRollComplete(dice, index));
        
            // Small scale effect on landing
            LeanTween.scale(dice, dice.transform.localScale * 1.1f, _throwDuration * 0.3f)
                .setDelay(delay + _throwDuration * 0.7f)
                .setEase(LeanTweenType.easeOutQuad)
                .setLoopPingPong(1);
        }

        /// <summary>
        /// Called when dice roll animation completes
        /// </summary>
        /// <param name="dice">Dice game object</param>
        /// <param name="index">Dice index</param>
        private void OnDiceRollComplete(GameObject dice, int index)
        {
            // Add logic after animation completion here
            if (index == _dices.Count - 1)
            {
                Debug.Log("All dice have landed!");
                // Call your callback or event here
            }
        }

        /// <summary>
        /// Editor testing
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RollAllDices();
            }
        
            // Test: roll only first 3 dice
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RollDicesByIndices(new List<int> { 0, 1, 2 });
            }
        
            // Test: roll only 2nd and 4th dice
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                RollDicesByIndices(new List<int> { 1, 3 });
            }
        }

        /// <summary>
        /// Visualize roll area in editor
        /// </summary>
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