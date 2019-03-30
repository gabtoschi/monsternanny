using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCDObject : MonoBehaviour {

    public static float OffAlphaSprite = 0.2f;

    private SpriteRenderer sprite;

    private Color offColor;
    private Color onColor;

    void Awake() {
        this.sprite = this.gameObject.GetComponent<SpriteRenderer>();

        this.onColor = this.sprite.color;
        this.offColor = this.sprite.color;
        this.offColor.a = LCDObject.OffAlphaSprite;
    }

    public void On() {
        this.sprite.color = this.onColor;
    }

    public void Off() {
        this.sprite.color = this.offColor;
    }
}
