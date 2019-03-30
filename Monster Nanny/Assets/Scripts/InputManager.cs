using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType {
    Left,
    Right,
    Action,
}

public class InputManager : MonoBehaviour {
    public delegate void InputPressed(InputType input);
    public static event InputPressed OnLCDInput;

    void Update() {
        if (Input.GetKeyDown("a")) {
            OnLCDInput(InputType.Left);
        }

        if (Input.GetKeyDown("d")) {
            OnLCDInput(InputType.Right);
        }

        if (Input.GetKeyDown("p")) {
            OnLCDInput(InputType.Action);
        }
    }
}
