using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeaponUI : MonoBehaviour
{
    public Image image;
    public Sprite shotgun;
    public Sprite dagger_sprite;
    public Player player;

    void Update()
    {
        if (player.currentWeapon == Player.WeaponState.Dagger)
        {
            image.sprite = dagger_sprite;
        }
        else
        {
            image.sprite = shotgun;
        }
    }
}