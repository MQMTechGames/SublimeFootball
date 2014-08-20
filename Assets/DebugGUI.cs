using UnityEngine;
using System.Collections;

public class DebugGUI : MonoBehaviour 
{
    void OnGUI()
    {
        GUI.Label(new Rect(10f, 10f, 1500f, 40f), "Current Time: " + Time.timeSinceLevelLoad    );
    }
}
