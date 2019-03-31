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
        "fluff",
        "bomb",
        "wind",
    };

    private int randomMonstersCounter = 0;
    private int randomMonstersMax = 5;

    void RandomizeMonsters() {
        Utils.ShuffleArray<string>(monsterOrder);
        UpdateMonsterLCD();
    }

    void UpdateMonsterCounter() {
        randomMonstersCounter += 1;

        if (randomMonstersCounter >= randomMonstersMax){
            RandomizeMonsters();
            randomMonstersCounter = 0;
        }
    }

    void UpdateMonsterLCD() {
        screen.OffScreen(monsterScreenTag);

        for (int i = 0; i < monsterOrder.Length; i++) {
            screen.OnObject(monsterTag + i + monsterOrder[i]);
        }
    }

    /* ITEMS */
    private string[] itemsSequence = new string[25];
    private const int itemPopulateQuantity = 17;

    private string itemsScreenTag = "items";
    private string itemTag = "item";

    private float updateItemsCounter = 0f;
    private float updateItemsMax = 1f;

    private int[] createItemsPositions = {0, 3, 6};
    private char createItemsToken = '$';

    void PopulateItems() {
        for (int i = 0; i < itemsSequence.Length; i++) {
            itemsSequence[i] = (i < itemPopulateQuantity) 
                ? monsterOrder[Random.Range(0, monsterOrder.Length)]
                : null;                
        }
    }

    void UpdateItems() {
        ShiftItems();
        UpdateItemsLCD();
    }

    void ShiftItems() {
        for (int i = itemsSequence.Length - 1; i > 0; i--) {
            itemsSequence[i] = itemsSequence[i-1];

            if (itemsSequence[i] != null
                && itemsSequence[i].ToCharArray()[0] == createItemsToken
                && itemsSequence[i].ToCharArray()[1].ToString() == i.ToString()) {
                    CreateScheduledItem(i);
                }
        }

        ScheduleNewItem();
    }

    void ScheduleNewItem() {
        var scheduledPositionId = Random.Range(0, createItemsPositions.Length * 9) % createItemsPositions.Length;

        if (createItemsPositions[scheduledPositionId] == 0) {
            CreateScheduledItem(0);
        } else {
            itemsSequence[0] = createItemsToken.ToString();
            itemsSequence[0] += createItemsPositions[scheduledPositionId];
        }
    }

    void CreateScheduledItem(int itemPosition) {
        for (int i = 0; i < createItemsPositions.Length; i++) {
            if (createItemsPositions[i] == itemPosition) {
                itemsSequence[itemPosition] = monsterOrder[i];
                return;
            }
        }
    }

    void UpdateItemsLCD() {
        screen.OffScreen(itemsScreenTag);

        for (int i = 0; i < itemsSequence.Length; i++) {
            if (itemsSequence[i] != null && itemsSequence[i].ToCharArray()[0] != createItemsToken) {
                screen.OnObject(itemTag + i + itemsSequence[i]);
            }
        }
    }

    void UpdateItemsCounter() {
        updateItemsCounter += Time.deltaTime;

        if (updateItemsCounter >= updateItemsMax){
            UpdateItems();
            UpdateMonsterCounter();
            updateItemsCounter = 0f;
        }
    }

    /* GAME LOOP */
    public LCDScreen screen;
    public bool pause = false;

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

        PopulateItems();
        UpdateItemsLCD();
    }

    void Update() {
        if (!pause) {
            UpdateItemsCounter();
        }
        
    }
   
}
