using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{
    A, B, X, Y, LeftStick, RightStick, LeftTrigger, RightTrigger, LeftButton, RightButton, Start, Select
}

public class ButtonIcon : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField]
    private Sprite[] sprites;

	// Use this for initialization
	public void Initialize (ButtonType bt)
    {
        sr = GetComponent<SpriteRenderer>();

        int i = 0;
        switch (bt)
        {
            case ButtonType.A:
                i = 0;
                break;
            case ButtonType.B:
                i = 1;
                break;
            case ButtonType.X:
                i = 2;
                break;
            case ButtonType.Y:
                i = 3;
                break;
            case ButtonType.LeftStick:
                i = 4;
                break;
            case ButtonType.RightStick:
                i = 5;
                break;
            case ButtonType.LeftTrigger:
                i = 6;
                break;
            case ButtonType.RightTrigger:
                i = 7;
                break;
            case ButtonType.LeftButton:
                i = 8;
                break;
            case ButtonType.RightButton:
                i = 9;
                break;
            case ButtonType.Start:
                i = 10;
                break;
            case ButtonType.Select:
                i = 11;
                break;
        }
        sr.sprite = sprites[i];
	}
}
