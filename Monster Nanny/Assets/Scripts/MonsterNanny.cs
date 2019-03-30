using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNanny : MonoBehaviour {

    public LCDScreen screen;

    int maxSpace = 5;
    int minSpace = 1;
    int currentSpace = 1;
    const string circleTag = "circle";

    void Awake() {
        InputManager.OnLCDInput += MoveLeft;
        InputManager.OnLCDInput += MoveRight;
    }
    void Start() {
        this.screen.OffAll();
        this.screen.OnOne(MonsterNanny.circleTag + this.currentSpace);
    }

    void MoveLeft(InputType inp) {
        if (inp == InputType.Left && currentSpace > minSpace) {
            this.screen.OffOne(MonsterNanny.circleTag + this.currentSpace);
            this.currentSpace -= 1;
            this.screen.OnOne(MonsterNanny.circleTag + this.currentSpace);
        }
    }
    void MoveRight(InputType inp) {
        if (inp == InputType.Right && currentSpace < maxSpace) {
            this.screen.OffOne(MonsterNanny.circleTag + this.currentSpace);
            this.currentSpace += 1;
            this.screen.OnOne(MonsterNanny.circleTag + this.currentSpace);
        }
    }
}
