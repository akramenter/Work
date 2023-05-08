using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconChange : MonoBehaviour
{
    public GameObject WeaponIcon;
    public Sprite[] sprites;
    public WeaponChange weaponChange;
    private Image iconSprite;
    int index = 0;


    void Start()
    {
        iconSprite = WeaponIcon.GetComponent<Image>();
        iconSprite.sprite = sprites[index];

    }

    void Update()
    {
        //index = weaponChange.inHandNo;
        NextSprite();
    }

    public void NextSprite()
    {
        if (weaponChange != null)
            iconSprite.sprite = sprites[weaponChange.inHandNo];
    }
}
