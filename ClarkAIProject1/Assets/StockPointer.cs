using UnityEngine;
using System.Collections.Generic;

public class StockPointer : MonoBehaviour
{
    [Header("Line Settings")]
    public LineRenderer rend;
    public Gradient activeGradient;

    [Header("Graph Settings")]
    public float xStep = 5f;          // spacing between data points on X-axis
    public float yScale = 10f;        // how tall each cube height appears
    public int maxPoints = 50;        // how many points to keep in the graph (rolling window)

    [Header("Anchors (optional)")]
    public GameObject start, end;     // references for debugging/anchoring graph ends

    private List<StockDataPoint> stockPoints = new List<StockDataPoint>();
    private List<Vector3> vectoredPositions = new List<Vector3>();
    private float timeCounter = 0f;

    public struct StockDataPoint
    {
        public float Time;
        public float Value;
        public Vector3 Position;
    }

    private void OnEnable()
    {
        
        StackManager.OnStackHeightChanged += HandleStackHeightChanged;
    }

    private void OnDisable()
    {
    
        StackManager.OnStackHeightChanged -= HandleStackHeightChanged;
    }

    private void Start()
    {
       
        AddDataPoint(0, 0, false);
    }

    /// <summary>
    /// Called whenever the stack height changes.
    /// Adds a new data point to the graph.
    /// </summary>
    private void HandleStackHeightChanged(StackHeightChangedEventArgs args)
    {
        timeCounter += xStep; // move X-axis forward
        AddDataPoint(timeCounter, args.NewHeight, false);
    }

    /// <summary>
    /// Add a new point to the graph
    /// </summary>
    public void AddDataPoint(float time, float value, bool stopRecursion)
    {
        StockDataPoint newPoint = new StockDataPoint
        {
            Time = time,
            Value = value,
            Position = new Vector3(time, value * yScale, 0)
        };

        // Add to lists
        stockPoints.Add(newPoint);
        vectoredPositions.Add(newPoint.Position);

        // Keep only the latest maxPoints
        if (stockPoints.Count > maxPoints)
        {
            stockPoints.RemoveAt(0);
            vectoredPositions.RemoveAt(0);

            // Shift X positions so the graph "scrolls" left
            for (int i = 0; i < vectoredPositions.Count; i++)
            {
                vectoredPositions[i] = new Vector3(
                    (i * xStep),
                    vectoredPositions[i].y,
                    vectoredPositions[i].z
                );
            }
        }

        // Apply to LineRenderer
        rend.positionCount = vectoredPositions.Count;
        rend.SetPositions(vectoredPositions.ToArray());

        // Update line color based on trend
        if (stockPoints.Count > 1)
        {
            var last = stockPoints[stockPoints.Count - 2];
            if (value >= last.Value)
                rend.startColor = rend.endColor = Color.green;
            else
                rend.startColor = rend.endColor = Color.red;
        }

        if (start != null) start.transform.position = vectoredPositions[0];
        if (end != null) end.transform.position = vectoredPositions[vectoredPositions.Count - 1];
    }
}
