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
    const string playerHalfChargedTag = "charge1";
    const string playerFullChargedTag = "charge2";
    const string playerUpTag = "up";
    const string playerDownTag = "down";
    
    private int playerPosition = 0;
    private int playerMaxPos = 5;
    private PlayerDirection playerFacing = PlayerDirection.Up;

    private bool playerHalfCharged = true;
    private bool playerFullCharged = true;
    private float chargePlayerCounter = 0f;
    private float chargePlayerMax = 1.75f;

    private int[] actionPositionsUp = {14, 13, 12, 11, 10, 9};
    private int[] actionPositionsDown = {18, 19, 20, 21, 22, 23};

    void UpdatePlayerCounter() {
        if (!playerFullCharged) {
            chargePlayerCounter += Time.deltaTime;

            if (!playerHalfCharged && chargePlayerCounter >= chargePlayerMax / 2f) {
                playerHalfCharged = true;
                UpdatePlayerLCD();
            }

            if (chargePlayerCounter >= chargePlayerMax){
                playerFullCharged = true;
                UpdatePlayerLCD();
                chargePlayerCounter = 0f;
            }
        }
    }

    void UpdatePlayerLCD() {
        screen.OffScreen(playerTag);
        screen.OnObject(playerTag + playerPosition + playerBaseTag);

        if (playerHalfCharged)
            screen.OnObject(playerTag + playerPosition + playerHalfChargedTag);

        if (playerFullCharged)
            screen.OnObject(playerTag + playerPosition + playerFullChargedTag);

        screen.OnObject(playerTag + playerPosition + 
            (playerFacing == PlayerDirection.Up ? playerUpTag : playerDownTag));
    }

    void PlayerMoveLeft(InputType inp) {
        if (!pause && inp == InputType.Left && playerPosition > 0) {
            playerPosition -= 1;
            UpdatePlayerLCD();
        }
    }

    void PlayerMoveRight(InputType inp) {
        if (!pause && inp == InputType.Right && playerPosition < playerMaxPos) {
            playerPosition += 1;
            UpdatePlayerLCD();
        }
    }

    void PlayerMoveUp(InputType inp) {
        if (!pause && inp == InputType.Up && playerFacing != PlayerDirection.Up) {
            playerFacing = PlayerDirection.Up;
            UpdatePlayerLCD();
        }
    }

    void PlayerMoveDown(InputType inp) {
        if (!pause && inp == InputType.Down && playerFacing != PlayerDirection.Down) {
            playerFacing = PlayerDirection.Down;
            UpdatePlayerLCD();
        }
    }

    void PlayerAction(InputType inp) {
        if (!pause && inp == InputType.Action && playerFullCharged) {
            playerHalfCharged = false;
            playerFullCharged = false;
            UpdatePlayerLCD();

            int pos = playerFacing == PlayerDirection.Up
                    ? actionPositionsUp[playerPosition]
                    : actionPositionsDown[playerPosition];

            RemoveItem(pos, true);
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
    private float updateItemsMax = 1.2f;
    private float updateItemsMaxBase = 1.2f;
    
    private float updateItemsSpeedUp = 0.8f;
    private float updateItemsSpeedUpPoints = 300;
    private float updateItemsSpeedUpPointsBase = 300;

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
        if (itemsSequence[itemsSequence.Length - 1] != null) {
            MissGame();
            return;
        }

        ShiftItems();
        UpdateItemsLCD();
    }

    void SpeedUpItems() {
        if (points > updateItemsSpeedUpPoints) {
            updateItemsMax *= updateItemsSpeedUp;
            updateItemsSpeedUpPoints += updateItemsSpeedUpPointsBase;
        }
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

    void RemoveItem(int position, bool haveCollision) {
        AddPoints(this.pointPerItem);
        RewindItems(position);
        UpdateItemsLCD();

        if (haveCollision && itemsSequence[position] != null) {
            CheckCollision(position);
        }
    }

    void CheckCollision(int pos){
        /* FLUFFY */
        if (itemsSequence[pos] == "fluff" && itemsSequence[pos-1] == "fluff") {
            AddPoints(this.pointPerCollision);
            RemoveItem(pos, false);
            return;
        }

        /* BOMBY */
        if (itemsSequence[pos] == "bomb" && itemsSequence[pos-1] == "bomb") {
            AddPoints(this.pointPerCollision);
            RemoveItem(pos, false);
            RemoveItem(pos-1, true);
            return;
        }

        /* WINDY */
        if (itemsSequence[pos] == "wind" && itemsSequence[pos-1] == "wind") {
            AddPoints(this.pointPerCollision);
            string tmp = itemsSequence[pos-1];
            itemsSequence[pos-1] = itemsSequence[pos-2];
            itemsSequence[pos-2] = tmp;

            RemoveItem(pos, true);
            return;
        }
    }

    void RewindItems(int position) {
        for (int i = position; i < itemsSequence.Length - 1; i++) {
            itemsSequence[i] = itemsSequence[i+1];
        }

        itemsSequence[itemsSequence.Length - 1] = null;
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

    /* MISS */
    public int misses = 0;
    public int missMax = 3;
    private string missTag = "missicon";
    private string citymissTag = "citymiss";

    void MissGame() {
        misses++;
        UpdateMissLCD();
        screen.OnObject(citymissTag);
        pause = true;
    }

    void UpdateMissLCD() {
        for (int i = 1; i <= missMax; i++) {
            if (i <= misses) screen.OnObject(missTag + i);
        }
    }

    /* POINTS */
    private string pointsDisplayTag = "pointsdisplay";
    private int points = 0;

    private int pointPerItem = 1;
    private int pointPerCollision = 10;

    void AddPoints(int value) {
        points += value;
        this.screen.Update7Seg(pointsDisplayTag, points);

        SpeedUpItems();
    }

    void UpdatePointsLCD() {
        screen.Reset7Seg("pointsdisplay");
    }

    /* GAME LOOP */
    public LCDScreen screen;
    public bool pause = false;

    void UnpauseGame(InputType inp) {
        if (pause && inp == InputType.Action) {          
            pause = false;

            if (misses == missMax) RetryMachine();
            else RetryGame();
        }
    }
   
    void RetryGame() {
        screen.OffAll();

        StartAlwaysOn();

        UpdatePlayerLCD();

        RandomizeMonsters();

        PopulateItems();
        UpdateItemsLCD();

        UpdateMissLCD();
    }

    void RetryMachine() {
        misses = 0;
        points = 0;

        updateItemsMax = updateItemsMaxBase;
        updateItemsSpeedUpPoints = updateItemsSpeedUpPointsBase;

        UpdatePointsLCD();
        RetryGame();
    }

    void Awake() {
        InputManager.OnLCDInput += PlayerMoveLeft;
        InputManager.OnLCDInput += PlayerMoveRight;
        InputManager.OnLCDInput += PlayerMoveUp;
        InputManager.OnLCDInput += PlayerMoveDown;
        InputManager.OnLCDInput += PlayerAction;
        InputManager.OnLCDInput += UnpauseGame;
    }

    void Start() {
        RetryGame();
    }

    void Update() {
        if (!pause) {
            UpdateItemsCounter();
            UpdatePlayerCounter();
        }
    }
   
}
