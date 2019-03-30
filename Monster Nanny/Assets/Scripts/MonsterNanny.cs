using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNanny : MonoBehaviour {

    private enum PlayerDirection {
        Up,
        Down
    }

    public LCDScreen screen;

    /* ALWAYS ON */
    static readonly string[] objectsAlwaysOnTags = {
        "pointslabel",
        "misslabel",
        "path",
        "citybase",
    };

    static readonly string[] screensAlwaysOnTags = {
        "monsterbases",
    };

    /* PLAYER */
    const string playerTag = "player";
    const string playerBaseTag = "base";
    const string playerUpTag = "up";
    const string playerDownTag = "down";
    
    
    private int playerPosition = 0;
    private int playerMaxPos = 5;
    private PlayerDirection playerFacing = PlayerDirection.Up;

    void Awake() {
        InputManager.OnLCDInput += PlayerMoveLeft;
        InputManager.OnLCDInput += PlayerMoveRight;
        InputManager.OnLCDInput += PlayerMoveUp;
        InputManager.OnLCDInput += PlayerMoveDown;
    }

    void Start() {
        screen.OffAll();

        StartAlwaysOn();
        UpdatePlayerLCD();
    }

    void StartAlwaysOn() {
        foreach (string tag in objectsAlwaysOnTags) {
            screen.OnObject(tag);
        }

        foreach (string tag in screensAlwaysOnTags) {
            screen.OnScreen(tag);
        }
    }

    void UpdatePlayerLCD() {
        screen.OffScreen(playerTag);
        screen.OnObject(playerTag + playerPosition + playerBaseTag);
        screen.OnObject(playerTag + playerPosition + 
            (playerFacing == PlayerDirection.Up ? playerUpTag : playerDownTag));
    }

    void PlayerMoveLeft(InputType inp) {
        if (inp == InputType.Left && playerPosition > 0) {
            playerPosition -= 1;
            UpdatePlayerLCD();
        }
    }

    void PlayerMoveRight(InputType inp) {
        if (inp == InputType.Right && playerPosition < playerMaxPos) {
            playerPosition += 1;
            UpdatePlayerLCD();
        }
    }

    void PlayerMoveUp(InputType inp) {
        if (inp == InputType.Up && playerFacing != PlayerDirection.Up) {
            playerFacing = PlayerDirection.Up;
            UpdatePlayerLCD();
        }
    }

    void PlayerMoveDown(InputType inp) {
        if (inp == InputType.Down && playerFacing != PlayerDirection.Down) {
            playerFacing = PlayerDirection.Down;
            UpdatePlayerLCD();
        }
    }
}
