using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCD7Seg : MonoBehaviour {

    private int currentValue = 0;

    private int maxValue;
    private string numberMask;

    LCDScreen[] digitObjects;

    public void ConfigDisplay() {
        List<LCDScreen> digits = new List<LCDScreen>();

        foreach (Transform child in transform) {
            var lcdScreen = child.gameObject.GetComponent<LCDScreen>();

            if (lcdScreen) {
                lcdScreen.ConfigObjects();
                digits.Add(lcdScreen);
            }
        }

        this.digitObjects = digits.ToArray();
        
        var maxValueStr = "";
        for (int i = 0; i < this.digitObjects.Length; i++) {
            maxValueStr += "9";
            this.numberMask += "0";
        }
        this.maxValue = int.Parse(maxValueStr);
    }

    void Start() {
        ResetDisplay();
    }

    public void ResetDisplay() {
        foreach (LCDScreen digit in this.digitObjects) {
            digit.OffAll();
        }
    }

    public void UpdateValue(int newValue) {
        this.currentValue = newValue;
        if (this.currentValue > this.maxValue) this.currentValue = 0;

        UpdateLCDDisplay();
    }

    void UpdateLCDDisplay() {
        char[] valueStr = this.currentValue.ToString(this.numberMask).ToCharArray();

        for (int i = 0; i < this.digitObjects.Length; i++) {
            this.digitObjects[i].OffAll();
            this.digitObjects[i].OnObject(valueStr[i].ToString());
        }
    }

}
