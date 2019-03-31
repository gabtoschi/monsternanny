using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCDScreen : MonoBehaviour {

    Dictionary<string, LCDObject> lcdObjects = new Dictionary<string, LCDObject>();
    Dictionary<string, LCDScreen> lcdChildScreens = new Dictionary<string, LCDScreen>();

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
            } else {
                var lcdComp = child.gameObject.GetComponent<LCDObject>();

                if (lcdComp) {
                    this.lcdObjects.Add(child.gameObject.name, lcdComp);
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
}
