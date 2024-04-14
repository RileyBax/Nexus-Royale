using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    private float fireRate = 0.0f;
    private bool isEquipped = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(fireRate >= 0) fireRate -= Time.deltaTime;

    }

    void FireWeapon(){

        if(fireRate <= 0.0f){

            fireRate = 1.5f;

            Instantiate(bullet);
            Instantiate(bullet).SendMessage("init", -10);
            Instantiate(bullet).SendMessage("init", 10);

        }

    }

    void setEquipped(bool e){

        isEquipped = e;

    }

    void getEquipped(GameObject bot){

        bot.SendMessage("setIsEquipped", isEquipped);

    }

}
