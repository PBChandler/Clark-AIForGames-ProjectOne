using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class StockPointer : MonoBehaviour
{
    public LineRenderer rend;
    public List<StockDataPoint> stockPoints = new List<StockDataPoint>();
    public GameObject start, end;
    int index = 0;
    public Gradient activeGradient; List<Vector3> VectoredPositions = new List<Vector3>();
    private float tempTime = 155, tempValue = 0;

    public struct StockDataPoint
    {
        public float Time;
        public float Value;
        public Vector3 position;
    }

    public void Start()
    {
        AddDataPoint(0, 0, false);
        AddDataPoint(100, 0, false);
        AddDataPoint(155, -50, false);
    }
    /// <summary>
    /// stopRecursion is to prevent the points added to smooth out the graph from creating an infinite loop;
    /// </summary>
    /// <param name="time"></param>
    /// <param name="value"></param>
    /// <param name="stopRecursion"></param>
    /// 
    public void Update()
    {
        if (Input.mouseScrollDelta.magnitude > 0.2f)
        {
            float sign = Mathf.Sign(Input.mouseScrollDelta.y);
            tempValue = Random.Range(2f*sign, 40f*sign);
            AddDataPoint(tempTime += 50, tempValue, false);
        }
    }
    public void AddDataPoint(float time, float value, bool stopRecursion)
    {
   
        Gradient g = new Gradient();
        StockDataPoint newPoint = new StockDataPoint();
        newPoint.position = new Vector3(start.transform.position.x+time, value-200, 0);
        index++;
        VectoredPositions.Add(newPoint.position);
        List<GradientColorKey> key = new List<GradientColorKey>();
        List<GradientAlphaKey> key2 = new List<GradientAlphaKey>();
        StockDataPoint lastPoint = newPoint;//stop issues with it not being initialized
        for (int i = 0; i < stockPoints.Count; i++)
        {
            GradientColorKey n = new GradientColorKey();

            if (i == 0)
                n.color = Color.green;
            else if (stockPoints[i - 1].Value > value)
            {
                n.color = Color.green;
            }
            else
            {
                n.color = Color.red;
            }
            

        }
        if (stockPoints.Count > 0)
        {
            lastPoint = stockPoints[stockPoints.Count - 1];
            if (value < lastPoint.Value)
            {
                rend.startColor = Color.red;
                rend.endColor = Color.red;
            }
            if (value >= lastPoint.Value)
            {
                rend.startColor = Color.green;
                rend.endColor = Color.green;
            }
        }
            
        
        activeGradient.SetKeys(key.ToArray(), key2.ToArray());
        stockPoints.Add(newPoint);
        rend.positionCount = VectoredPositions.Count;
        rend.SetPositions(VectoredPositions.ToArray());
       
        start.transform.position = stockPoints[0].position;
        end.transform.position = stockPoints[stockPoints.Count-1].position;
       
    }
}
