using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System;

public class CameraUIManager : MonoBehaviour
{
    #region EDITOR_VALS
    /// <summary>UI Text Ouput for points</summary>
    [SerializeField] private Text pointCounterUI;

    /// <summary>UI Text Ouput for playing time</summary>
    [SerializeField] private Text timeCounterUI;
    #endregion

    /// <summary> Save the time passed </summary>
    private float timePassed = 0;

    #region LIFECYCLE
    // Start is called before the first frame update
    void Start()
    {
        PointManager.OnPointsChanged += UpdatePoints;
        UpdatePoints();
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        int min = (int) (timePassed / 60);
        int sec = (int) (timePassed - min * 60);
        timeCounterUI.text = "" + min + "m" + sec + "s";
    }

    void OnDestroy()
    {
        PointManager.OnPointsChanged -= UpdatePoints;
    }
    #endregion

    private void UpdatePoints()
    {
        pointCounterUI.text = "" + PointManager.GetPoints();
    }
}
