using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    #region EDITOR_VALS
    /// <summary> Transforms of the points to be spawned</summary>
    [SerializeField] private Coin[] points;
    #endregion

    #region PRIVATE_CLASSMEMBER
    /// <summary>Point counter</summary>
    private static int currentPoints = 0;

    /// <summary>max number of points</summary>
    private static int maxPoints;
    #endregion

    #region EVENTS
    /// <summary>Called whenever the points change</summary>
    public static Action OnPointsChanged;

    /// <summary>Called when the max points were reached</summary>
    public static Action OnMaxPointReached;
    #endregion

    #region LIFECYCLE
    void Start()
    {
        maxPoints = points.Length;
        foreach (Coin point in points) point.OnPointAchieved += IncreasePoints;
    }
    void OnDestroy()
    {
        currentPoints = 0;
    }
    #endregion
    
    private void IncreasePoints()
    {
        currentPoints++;
        if (currentPoints == maxPoints) OnMaxPointReached?.Invoke();
        else OnPointsChanged?.Invoke();
    }

    public static int GetPoints() { return currentPoints;}   
}
