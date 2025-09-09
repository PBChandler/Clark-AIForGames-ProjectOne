using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    public static event Action<StackHeightChangedEventArgs> OnStackHeightChanged;
    public static StackManager Instance { get; private set; }

    [SerializeField] private float checkInterval = 0.25f;
    [SerializeField] private float bucketSize = 1.0f;

    private readonly List<Transform> _activeCubes = new List<Transform>();
    private int _lastKnownMaxHeight = 0;
    private readonly Dictionary<Vector2Int, int> _stackCounts = new Dictionary<Vector2Int, int>();

    public GameObject player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        StartCoroutine(CheckHeightRoutine());
    }

    public void RegisterCube(Transform cubeTransform)
    {
        if (!_activeCubes.Contains(cubeTransform))
            _activeCubes.Add(cubeTransform);
    }

    private IEnumerator CthulhuEnding()
    {
       
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;

                float floatDuration = 5f;
                float elapsed = 0f;
                Vector3 floatDirection = Vector3.up * 10f; // upward velocity

                while (elapsed < floatDuration)
                {
                    rb.MovePosition(rb.position + floatDirection * Time.deltaTime);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScreen");
    }


    public void DeregisterCube(Transform cubeTransform)
    {
        _activeCubes.Remove(cubeTransform);
    }

    private IEnumerator CheckHeightRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            CalculateAndCheckHeight();
        }
    }

    private void CalculateAndCheckHeight()
    {
        if (_activeCubes.Count == 0)
        {
            if (_lastKnownMaxHeight != 0)
            {
                _lastKnownMaxHeight = 0;
                OnStackHeightChanged?.Invoke(new StackHeightChangedEventArgs(0, HeightChangeDirection.Decreased));
            }
            return;
        }

        _stackCounts.Clear();
        int currentMaxHeight = 0;

        foreach (var cube in _activeCubes)
        {
            SpawnableCube cubeData = cube.GetComponent<SpawnableCube>();
            if (cubeData == null || !cubeData.HasLanded) continue;

            int bucketX = Mathf.RoundToInt(cube.position.x / bucketSize);
            int bucketZ = Mathf.RoundToInt(cube.position.z / bucketSize);
            var bucket = new Vector2Int(bucketX, bucketZ);

            if (!_stackCounts.ContainsKey(bucket))
                _stackCounts[bucket] = 0;

            _stackCounts[bucket]++;
        }

        foreach (var count in _stackCounts.Values)
        {
            if (count > currentMaxHeight)
                currentMaxHeight = count;
        }

        if (currentMaxHeight != _lastKnownMaxHeight)
        {
            HeightChangeDirection direction = currentMaxHeight > _lastKnownMaxHeight
                ? HeightChangeDirection.Increased
                : HeightChangeDirection.Decreased;

            OnStackHeightChanged?.Invoke(new StackHeightChangedEventArgs(currentMaxHeight, direction));
            _lastKnownMaxHeight = currentMaxHeight;

            if (currentMaxHeight >= 20)
            {
                StartCoroutine(CthulhuEnding());
            }
        }
    }

}
