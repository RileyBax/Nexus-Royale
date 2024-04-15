using System;
using System.Threading;
using TreeEditor;
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
    private Collider2D[] hitColliders;
    private GameObject target;
    private Vector2 zone;
    private float wanderAngle;
    private float waitTimer;

    // Start is called before the first frame update
    void Start()
    {
        
        // have game manager set zone center position
        zone = new Vector2(0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        
        if(state == 0){

            if(weaponNear == null){
                    
                hitColliders = Physics2D.OverlapCircleAll(transform.position, 10);
                
                for(int i = 0; i < hitColliders.Length; i++){

                    if(weaponNear == null && hitColliders[i].tag.Equals("Weapon")) weaponNear = hitColliders[i].gameObject;
                    else if(weaponNear != null && Vector3.Distance(weaponNear.transform.position, transform.position) > Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position) 
                    && hitColliders[i].tag.Equals("Weapon")){
                        
                        hitColliders[i].gameObject.SendMessage("getEquipped", transform.gameObject);

                        if(!weaponNearEquipped) weaponNear = hitColliders[i].gameObject;

                    }

                }

            }
            else if(directionTimer <= 0.0f){

                directionTimer = (float) rand.NextDouble();

                weaponNearAngle = -(float) Math.Atan2(transform.position.x - weaponNear.transform.position.x + 0.5f, transform.position.y - weaponNear.transform.position.y) + 3.14f;

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

            if(weaponNear != null){

                weaponNear.SendMessage("getEquipped", transform.gameObject);
                if(weaponNearEquipped) weaponNear = null;

            }

        }
        else if(state == 1){

            if(target == null || !target.activeSelf){

                hitColliders = Physics2D.OverlapCircleAll(transform.position, 10);

                for(int i = 0; i < hitColliders.Length; i++){

                    if(target == null && hitColliders[i].tag.Equals("Character") && hitColliders[i].gameObject != transform.gameObject) target = hitColliders[i].gameObject;
                    else if(target != null && Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position)
                    && hitColliders[i].tag.Equals("Character") && hitColliders[i].gameObject != transform.gameObject) target = hitColliders[i].gameObject;

                }

                if(target != null) state = 2;

            }

            if(target == null && Vector3.Distance(movePoint, transform.position) < 0.1f && waitTimer <= 0.0f){

                // expand search, make bot wander
                // wander towards center of zone


                // change this to pick one of 8 45 deg angles to walk in
                // -------------------------------------------------------------------------------

                wanderAngle = -(float) Math.Atan2(rand.Next((int)(transform.position.x - zone.x - 10), (int)(transform.position.x - zone.x + 10)), 
                rand.Next((int)(transform.position.y - zone.y - 10), (int)(transform.position.y - zone.y + 10))) + 3.14f;

                movePoint = new Vector3();
                float tempMoveAngle = 10.0f;

                for(int i = 0; i < 8; i++){

                    if(Math.Abs(wanderAngle - i * 0.785f) < Math.Abs(wanderAngle  - tempMoveAngle)) tempMoveAngle = i * 0.785f;

                }

                moveAngle = -tempMoveAngle;

                movePoint.x = (float) (transform.position.x + Math.Sin(moveAngle) * 5);
                movePoint.y = (float) (transform.position.y + Math.Cos(moveAngle) * 5);

                waitTimer = (float)(rand.NextDouble() * 5);
                // -------------------------------------------------------------------------------

            }

            waitTimer -= Time.deltaTime;
            
            updateWeapon();
            hasWeaponCheck();

        }
        else if(state == 2){

            if(target.activeSelf) {
                
                updateWeapon();

                if(Vector3.Distance(movePoint, transform.position) < 0.1f){

                    wanderAngle = -(float) Math.Atan2(rand.Next((int)(transform.position.x - target.transform.position.x - 5), (int)(transform.position.x - target.transform.position.x + 10)), 
                    rand.Next((int)(transform.position.y - target.transform.position.y - 5), (int)(transform.position.y - target.transform.position.y + 5))) + 3.14f;

                    movePoint = new Vector3();
                    float tempMoveAngle = 10.0f;

                    for(int i = 0; i < 8; i++){

                        if(Math.Abs(wanderAngle - i * 0.785f) < Math.Abs(wanderAngle  - tempMoveAngle)) tempMoveAngle = i * 0.785f;

                    }

                    moveAngle = -tempMoveAngle;

                    movePoint.x = (float) (transform.position.x + Math.Sin(moveAngle) * 5);
                    movePoint.y = (float) (transform.position.y + Math.Cos(moveAngle) * 5);

                }

            }
            else{

                state = 1;
                target = null;

            }

            hasWeaponCheck();

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

        if(col.tag.Equals("Weapon") && state == 0){

            col.gameObject.SendMessage("getEquipped", transform.gameObject);

            if(!weaponNearEquipped){

                weapon = col.gameObject;
                col.gameObject.SendMessage("setEquipped", true);
                col.gameObject.SendMessage("setCharacter", transform.gameObject);

                state = 1;

            }

        }

    }

    void updateWeapon(){

        if(target != null) {

            weapon.transform.up = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);

            angle = -(float) Math.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) + 1.55f;

            weapon.SendMessage("FireWeapon");

        }

        weaponPos.x = (float) (transform.position.x + Math.Sin(angle) * 1);
        weaponPos.y = (float) (transform.position.y + Math.Cos(angle) * 1);

        weapon.transform.position = weaponPos;

    }

    void updateHealth(int damage){

        health -= damage;

        if(health <= 0) {

            transform.gameObject.SetActive(false);
            
            if(weapon != null) {

                weapon.SendMessage("setEquipped", false);
                weapon.SendMessage("setCharacterNull");

            }
            
        }

    }

    void hasWeaponCheck(){

        //incase bot gets stuck with no weapon
        if(weapon == null || !weapon.activeSelf) {
                
            state = 0;
            weaponNear = null;

        }

    }

}
