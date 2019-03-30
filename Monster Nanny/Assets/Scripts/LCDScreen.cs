using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCDScreen : MonoBehaviour {

    Dictionary<string, LCDObject> lcdObjects = new Dictionary<string, LCDObject>();

    void Awake() {
        foreach (Transform child in transform) {
            this.lcdObjects.Add(child.gameObject.name, 
                child.gameObject.GetComponent<LCDObject>());
        }
    }

    void Start() {
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

    public void OnOne(string lcdObject) {
        this.lcdObjects[lcdObject].On();
    }

    public void OffOne(string lcdObject) {
        this.lcdObjects[lcdObject].Off();
    }
}
