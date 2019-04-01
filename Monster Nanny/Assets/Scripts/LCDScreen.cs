using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCDScreen : MonoBehaviour {

    Dictionary<string, LCDObject> lcdObjects = new Dictionary<string, LCDObject>();
    Dictionary<string, LCDScreen> lcdChildScreens = new Dictionary<string, LCDScreen>();
    Dictionary<string, LCD7Seg> lcd7Segs = new Dictionary<string, LCD7Seg>();

    public bool isHeadScreen = false;

    void Awake() {
        if (this.isHeadScreen) {
            this.ConfigObjects();
        }
    }

    public void ConfigObjects() {
        foreach (Transform child in transform) {
            var lcdScreen = child.gameObject.GetComponent<LCDScreen>();

            if (lcdScreen) {
                this.lcdChildScreens.Add(child.gameObject.name, lcdScreen);

                if (!lcdScreen.isHeadScreen) {
                    lcdScreen.ConfigObjects();
                }

                foreach(KeyValuePair<string, LCDObject> data in lcdScreen.GetAllObjects()) {
                    this.lcdObjects.Add(data.Key, data.Value);
                }

                foreach(KeyValuePair<string, LCD7Seg> data in lcdScreen.GetAll7Segs()) {
                    this.lcd7Segs.Add(data.Key, data.Value);
                }
            } else {
                var lcd7Seg = child.gameObject.GetComponent<LCD7Seg>();

                if (lcd7Seg) {
                    this.lcd7Segs.Add(child.gameObject.name, lcd7Seg);
                    lcd7Seg.ConfigDisplay();
                } else {
                    var lcdComp = child.gameObject.GetComponent<LCDObject>();

                    if (lcdComp) {
                        this.lcdObjects.Add(child.gameObject.name, lcdComp);
                    }
                }
            }   
        }
    }

    public void OnAll() {
        foreach(KeyValuePair<string, LCDObject> data in this.lcdObjects) {
            data.Value.On();
        }
    }

    public void OffAll() {
        foreach(KeyValuePair<string, LCDObject> data in this.lcdObjects) {
            data.Value.Off();
        }
    }

    public void OnObject(string lcdObject) {
        if (this.lcdObjects.ContainsKey(lcdObject))
            this.lcdObjects[lcdObject].On();
    }

    public void OffObject(string lcdObject) {
        if (this.lcdObjects.ContainsKey(lcdObject))
            this.lcdObjects[lcdObject].Off();
    }

    public void OnScreen(string lcdScreen) {
        if (this.lcdChildScreens.ContainsKey(lcdScreen))
            this.lcdChildScreens[lcdScreen].OnAll();
    }

    public void OffScreen(string lcdScreen) {
        if (this.lcdChildScreens.ContainsKey(lcdScreen))
            this.lcdChildScreens[lcdScreen].OffAll();
    }

    public LCDScreen GetChildScreen(string lcdScreen) {
        if (this.lcdChildScreens.ContainsKey(lcdScreen)) return null;
        return this.lcdChildScreens[lcdScreen];
    }

    public Dictionary<string, LCDObject> GetAllObjects() {
        return this.lcdObjects;
    }

    public Dictionary<string, LCD7Seg> GetAll7Segs() {
        return this.lcd7Segs;
    }

    public void Reset7Seg(string lcd7Seg) {
        if (this.lcd7Segs.ContainsKey(lcd7Seg)) {
            this.lcd7Segs[lcd7Seg].ResetDisplay();
        }
    }

    public void Update7Seg(string lcd7Seg, int value) {
        if (this.lcd7Segs.ContainsKey(lcd7Seg)) {
            this.lcd7Segs[lcd7Seg].UpdateValue(value);
        }
    }
}
