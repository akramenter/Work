using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconChange : MonoBehaviour
{
    public GameObject ItemIcon;
    public Sprite[] sprites;
    public ActorManager am;
    private Image iconSprite;
    int index = 0;
    void Start()
    {
        iconSprite = ItemIcon.GetComponent<Image>();
        iconSprite.sprite = sprites[index];
        //am = GetComponentInParent<ActorManager>();

    }

    void Update()
    {
        //index = weaponChange.inHandNo;
        NextSprite();
    }

    public void NextSprite()
    {
        if (am.sm.haveOil)
            iconSprite.sprite = sprites[1];
        else
            iconSprite.sprite = sprites[0];
    }
}
