﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCDObject : MonoBehaviour {

    private SpriteRenderer sprite;

    void Start() {
        this.sprite = this.gameObject.GetComponent<SpriteRenderer>();
        this.gameObject.SetActive(false);
    }

    public void On() {
        this.gameObject.SetActive(true);
    }

    public void Off() {
        this.gameObject.SetActive(false);
    }
}
