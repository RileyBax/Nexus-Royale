using UnityEngine;
using UnityEngine.UI;

public class UIWeapons : MonoBehaviour
{
    [SerializeField]
    public Image[] WeaponSlots;

    public void UpdateWeapons(Weapons weapons)
    {
        // Should only be three weapon slots
        for (int i = 0; i < 3; i++)
        {
            int selectedSlot = weapons.GetSelectedSlot() - 1;
            if (i != selectedSlot) WeaponSlots[i].color = Color.grey;
            else WeaponSlots[i].color = Color.white;

            Weapon weapon = weapons.GetSlot(i + 1);
            if (WeaponSlots[i].sprite == null && (weapon != null))
            {
                WeaponSlots[i].sprite = weapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            }
            if (WeaponSlots[i].sprite != null && weapon == null)
            {
                WeaponSlots[i].sprite = null;
            }
        }
    }
}
