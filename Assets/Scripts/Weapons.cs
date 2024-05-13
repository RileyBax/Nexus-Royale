using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : NetworkBehaviour
{
    private Weapon Slot1;
    private Weapon Slot2;
    private Weapon Slot3;
    private int SelectedWeapon;

    public Weapons()
    {
        this.SelectedWeapon = 1;
    }
    
    public void ChangeWeapon(int slotNum)
    {
        Weapon weapon = this.GetSlot(SelectedWeapon);
        if (weapon != null)
        {
        weapon.SetVisible(false);
        }
        SelectedWeapon = slotNum;
        weapon = this.GetSlot(SelectedWeapon);
        if (weapon != null)
        {
            weapon.SetVisible(true);
        }
    }

    public void PickupWeapon(Weapon weapon)
    {
        Weapon currentWeapon = this.GetSlot(SelectedWeapon);
        if (currentWeapon != null) 
        {
            currentWeapon.RemovePlayer();
        }

        SetSlotWeapon(SelectedWeapon, weapon);
    }

    public void SetSlotWeapon(int slotNum, Weapon weapon)
    {
        switch (slotNum)
        {
            case 1: 
                Slot1 = weapon;
                break;
            case 2: 
                Slot2 = weapon;
                break;
            case 3: 
                Slot3 = weapon;
                break;
        }
    }

    public Weapon GetSlot(int slotNum)
    {
        switch (slotNum)
        {
            case 1: return Slot1;
            case 2: return Slot2;
            case 3: return Slot3;
        }

        return null;
    }

    public Weapon GetSelectedWeapon()
    {
        return this.GetSlot(SelectedWeapon);
    }

    public int GetSelectedSlot()
    {
        return this.SelectedWeapon;
    }
}
