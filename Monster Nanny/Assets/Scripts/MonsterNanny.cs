using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNanny : MonoBehaviour {

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

    void StartAlwaysOn() {
        foreach (string tag in objectsAlwaysOnTags) {
            screen.OnObject(tag);
        }

        foreach (string tag in screensAlwaysOnTags) {
            screen.OnScreen(tag);
        }
    }

    /* PLAYER */
    private enum PlayerDirection {
        Up,
        Down
    }

    const string playerTag = "player";
    const string playerBaseTag = "base";
    const string playerUpTag = "up";
    const string playerDownTag = "down";
    
    private int playerPosition = 0;
    private int playerMaxPos = 5;
    private PlayerDirection playerFacing = PlayerDirection.Up;

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

    /* MONSTERS */

    private string monsterTag = "monster";
    private string monsterScreenTag = "monsters";

    private string[] monsterOrder = {
        "fluffy",
        "bomby",
        "windy",
    };

    void RandomizeMonsters() {
        Utils.ShuffleArray<string>(monsterOrder);
    }

    void UpdateMonsterLCD() {
        screen.OffScreen(monsterScreenTag);

        for (int i = 0; i < monsterOrder.Length; i++) {
            screen.OnObject(monsterTag + i + monsterOrder[i]);
        }
    }

    /* GAME LOOP */
    public LCDScreen screen;

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
        RandomizeMonsters();
        UpdateMonsterLCD();
    }
   
}
