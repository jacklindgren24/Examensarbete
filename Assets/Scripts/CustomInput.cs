using System.Collections.Generic;
using UnityEngine;

public class CustomInput : MonoBehaviour {

    enum AxisState
    {
        Idle,
        Pressed,
        Held
    }

    static Dictionary<string, AxisState> axesStates = new Dictionary<string, AxisState>
    {
        { "Fire1", AxisState.Idle }
    };

    void Update()
    {
        string[] keys = new string[axesStates.Count];
        axesStates.Keys.CopyTo(keys, 0);
        for (int i = 0; i < keys.Length; i++)
        {
            bool isPressed = Input.GetAxisRaw(keys[i]) != 0;

            if (axesStates[keys[i]] == AxisState.Idle)
            {
                axesStates[keys[i]] = isPressed ? AxisState.Pressed : AxisState.Idle;
            }
            else
            {
                axesStates[keys[i]] = isPressed ? AxisState.Held : AxisState.Idle;
            }
        }
    }

    public static bool GetAxisDown(string axisName)
    {
        return axesStates[axisName] == AxisState.Pressed;
    }

    public static bool GetAxis(string axisName)
    {
        return axesStates[axisName] == AxisState.Pressed || axesStates[axisName] == AxisState.Held;
    }
}
