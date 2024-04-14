using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGScript : MonoBehaviour
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

            fireRate = 0.2f;

            Instantiate(bullet);

        }

    }

    void setEquipped(bool e){

        isEquipped = e;

    }

    void getEquipped(GameObject bot){

        bot.SendMessage("setIsEquipped", isEquipped);

    }

}
