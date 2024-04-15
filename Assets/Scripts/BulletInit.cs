using Unity.VisualScripting;
using UnityEngine;

public class BulletInit{

    public int angle;
    public GameObject character;
    public GameObject weapon;

    public BulletInit(int angle, GameObject character, GameObject weapon){

        this.angle = angle;
        this.character = character;
        this.weapon = weapon;

    }

}