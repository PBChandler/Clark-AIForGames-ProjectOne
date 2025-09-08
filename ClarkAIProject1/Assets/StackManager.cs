using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    
    public static event Action<StackHeightChangedEventArgs> OnStackHeightChanged;

    // Singleton instance
    public static StackManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("How often (in seconds) to check the stack height.")]
    [SerializeField] private float checkInterval = 0.25f;

    [Tooltip("The size of the grid cells for grouping stacks.")]
    [SerializeField] private float bucketSize = 1.0f;

    // The list of all active cubes in the scene.
    private readonly List<Transform> _activeCubes = new List<Transform>();
    private int _lastKnownMaxHeight = 0;
    private readonly Dictionary<Vector2Int, int> _stackCounts = new Dictionary<Vector2Int, int>();

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // periodic but not on update
        StartCoroutine(CheckHeightRoutine());
    }

    
    public void RegisterCube(Transform cubeTransform)
    {
        if (!_activeCubes.Contains(cubeTransform))
        {
            _activeCubes.Add(cubeTransform);
        }
    }

    public void DeregisterCube(Transform cubeTransform)
    {
        _activeCubes.Remove(cubeTransform);
    }

    
    private IEnumerator CheckHeightRoutine()
    {
        while (true)
        {
            // Wait for the specified interval before running the check.
            yield return new WaitForSeconds(checkInterval);
            CalculateAndCheckHeight();
        }
    }

    private void CalculateAndCheckHeight()
    {
        if (_activeCubes.Count == 0)
        {
            // If there are 0 cubes, the height is 0.
            if (_lastKnownMaxHeight != 0)
            {
                _lastKnownMaxHeight = 0;
                OnStackHeightChanged?.Invoke(new StackHeightChangedEventArgs(0, HeightChangeDirection.Decreased));
            }
            return;
        }

        // 1. Clear previous counts
        _stackCounts.Clear();
        int currentMaxHeight = 0;

        // 2. Iterate through all cubes and place them in buckets
        foreach (var cube in _activeCubes)
        {
            // Create a 2D grid coordinate based on the cube's world position
            int bucketX = Mathf.RoundToInt(cube.position.x / bucketSize);
            int bucketZ = Mathf.RoundToInt(cube.position.z / bucketSize);
            var bucket = new Vector2Int(bucketX, bucketZ);

            // Increment the count for that bucket
            if (!_stackCounts.ContainsKey(bucket))
            {
                _stackCounts[bucket] = 0;
            }
            _stackCounts[bucket]++;
        }

        // 3. Find the tallest stack among all buckets
        foreach (var count in _stackCounts.Values)
        {
            if (count > currentMaxHeight)
            {
                currentMaxHeight = count;
            }
        }

        // 4. Compare with the last known height and fire the event if it changed
        if (currentMaxHeight != _lastKnownMaxHeight)
        {
            HeightChangeDirection direction = currentMaxHeight > _lastKnownMaxHeight
                ? HeightChangeDirection.Increased
                : HeightChangeDirection.Decreased;

            Debug.Log($"Height changed from {_lastKnownMaxHeight} to {currentMaxHeight}");
            OnStackHeightChanged?.Invoke(new StackHeightChangedEventArgs(currentMaxHeight, direction));

            _lastKnownMaxHeight = currentMaxHeight;
        }
    }
}