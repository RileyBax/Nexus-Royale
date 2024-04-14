using System;
using UnityEngine;

public class BotScript : MonoBehaviour
{

    private GameObject weaponNear;
    private GameObject weapon;
    private Vector2 weaponPos;
    private float angle = 0;
    private bool weaponNearEquipped;
    private float weaponNearAngle;
    private float moveAngle;
    [SerializeField] Rigidbody2D rb;
    private Vector3 movePoint;
    private float directionTimer = 0.0f;
    private System.Random rand = new System.Random();
    private int state = 0; // 0 = searching for weapon, 1 = has weapon searching for player, 2 = attacking player
    private int health = 100;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(state == 0){

            if(weaponNear == null){
                    
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 10);
                
                for(int i = 0; i < hitColliders.Length; i++){

                    if(weaponNear == null && hitColliders[i].tag.Equals("Weapon")) weaponNear = hitColliders[i].gameObject;
                    else if(weaponNear != null && Vector3.Distance(weaponNear.transform.position, transform.position) > Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position) 
                    && hitColliders[i].tag.Equals("Weapon")){
                        
                        hitColliders[i].gameObject.SendMessage("getEquipped", transform.gameObject);

                        if(!weaponNearEquipped) weaponNear = hitColliders[i].gameObject;

                    }

                }

            }

            // find closest angle to in 45deg, move towards object on that axis until collide

            if(directionTimer <= 0.0f){

                directionTimer = (float) rand.NextDouble();

                weaponNearAngle = -(float) Math.Atan2(transform.position.x - weaponNear.transform.position.x - 0.5f, transform.position.y - weaponNear.transform.position.y) + 3.14f;

                movePoint = new Vector3();
                float tempMoveAngle = 10.0f;

                for(int i = 0; i < 8; i++){

                    if(Math.Abs(weaponNearAngle - i * 0.785f) < Math.Abs(weaponNearAngle - tempMoveAngle)) tempMoveAngle = i * 0.785f;

                }

                moveAngle = -tempMoveAngle;

                movePoint.x = (float) (transform.position.x + Math.Sin(moveAngle) * 5);
                movePoint.y = (float) (transform.position.y + Math.Cos(moveAngle) * 5);

            }

            Debug.DrawLine(transform.position, movePoint);

            // set timer for change direction of bot movement

            directionTimer -= Time.deltaTime;

            // check if weapon is picked up by another bot/player

            weaponNear.SendMessage("getEquipped", transform.gameObject);
            if(weaponNearEquipped) weaponNear = null;

        }
        else if(state == 1){

            updateWeapon();

            //  search for player ------------------------

        }

    }

    void FixedUpdate(){

        Vector2 moveDir = new Vector2(movePoint.x - transform.position.x, movePoint.y - transform.position.y).normalized;

        rb.MovePosition(rb.position + moveDir * Time.deltaTime * 5);

    }

    void setIsEquipped(bool e){

        weaponNearEquipped = e;

    }

    void OnTriggerEnter2D(Collider2D col){

        if(col.tag.Equals("Weapon")){

            col.gameObject.SendMessage("getEquipped", transform.gameObject);

            if(!weaponNearEquipped){

                // create variable for weapon
                // set weapon to follow bot, use player code
                // find angle and aim at near player/bot

                weapon = col.gameObject;
                col.gameObject.SendMessage("setEquipped", true);

                state = 1;

            }

        }

    }

    void updateWeapon(){

        weaponPos.x = (float) (transform.position.x + Math.Sin(angle) * 1);
        weaponPos.y = (float) (transform.position.y + Math.Cos(angle) * 1);

        weapon.transform.position = weaponPos;
        weapon.transform.up = new Vector2(weaponPos.x - transform.position.x, weaponPos.y - transform.position.y);

    }

    void updateHealth(int damage){

        health -= damage;

        if(health <= 0) {

            transform.gameObject.SetActive(false);
            weapon.SendMessage("setEquipped", false);
            
        }

    }

}
