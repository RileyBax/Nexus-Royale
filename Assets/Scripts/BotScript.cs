using System;
using UnityEngine;

public class BotScript : MonoBehaviour
{

    private GameObject weaponNear;
    private GameObject weapon;
    private Vector2 weaponPos;
    private float angle = 0;
    private bool weaponNearEquipped;
    private float destAngle;
    private float moveAngle;
    [SerializeField] Rigidbody2D rb;
    private Vector3 movePoint;
    private float directionTimer = 0.0f;
    private System.Random rand = new System.Random();
    public int state = 0; // 0 = searching for weapon, 1 = has weapon searching for player, 2 = attacking player
    private int health = 100;
    private Collider2D[] hitColliders;
    private GameObject target;
    private Vector2 zone;
    private float wanderAngle;
    private float waitTimer;
    private float offsetTimer = 2.0f;
    private int offset;
    private float damageTimer;
    private SpriteRenderer sr;
    private Color baseColor = new Color(1.0f, 0.823f, 0.506f, 255);
    private float zoneDamageTimer = 2.0f;
    private bool insideZone = true;
    private float healTimer;
    private Vector3 dest;

    // Start is called before the first frame update
    void Start()
    {

        sr = transform.gameObject.GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
        // Searching for weapon
        if(state == 0){

            if(weaponNear == null){
                
                // Check for weapon in circle around character
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
            // Changes direction the bot is moving in, stops bot jittering
            else if(directionTimer <= 0.0f){

                directionTimer = (float) rand.NextDouble();

                destAngle = -(float) Math.Atan2(transform.position.x - weaponNear.transform.position.x + 0.5f, transform.position.y - weaponNear.transform.position.y) + 3.14f;

                movePoint = new Vector3();
                float tempMoveAngle = 10.0f;

                for(int i = 0; i < 8; i++){

                    if(Math.Abs(destAngle - i * 0.785f) < Math.Abs(destAngle - tempMoveAngle)) tempMoveAngle = i * 0.785f;

                }

                moveAngle = -tempMoveAngle;

                movePoint.x = (float) (transform.position.x + Math.Sin(moveAngle) * 5);
                movePoint.y = (float) (transform.position.y + Math.Cos(moveAngle) * 5);

            }

            // set timer for change direction of bot movement

            directionTimer -= Time.deltaTime;

            // check if weapon is picked up by another bot/player

            if(weaponNear != null){

                weaponNear.SendMessage("getEquipped", transform.gameObject);
                if(weaponNearEquipped) weaponNear = null;

            }

            // TODO: Make bot wander to center of zone if it cant find a weapon
            if(target == null && Vector3.Distance(movePoint, transform.position) < 0.1f && waitTimer <= 0.0f){

                // change this to pick one of 8 45 deg angles to walk in
                // -------------------------------------------------------------------------------

                wanderAngle = -(float) Math.Atan2(rand.Next((int)(transform.position.x - zone.x - 30), (int)(transform.position.x - zone.x + 30)), 
                rand.Next((int)(transform.position.y - zone.y - 30), (int)(transform.position.y - zone.y + 30))) + 3.14f;

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
            else if(Vector3.Distance(movePoint, transform.position) < 0.1f && waitTimer > 0.0f) movePoint = transform.position;
            waitTimer -= Time.deltaTime;

        }
        // Has weapon, searching for target
        else if(state == 1){

            if(target == null || !target.activeSelf){

                // Check for target in circle
                hitColliders = Physics2D.OverlapCircleAll(transform.position, 20);

                for(int i = 0; i < hitColliders.Length; i++){

                    if(target == null && hitColliders[i].tag.Equals("Character") && hitColliders[i].gameObject != transform.gameObject) target = hitColliders[i].gameObject;
                    else if(target != null && Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position)
                    && hitColliders[i].tag.Equals("Character") && hitColliders[i].gameObject != transform.gameObject) target = hitColliders[i].gameObject;

                }

                if(health < 100 && target == null){

                    hitColliders = Physics2D.OverlapCircleAll(transform.position, 10);

                    for(int i = 0; i < hitColliders.Length; i++){

                        if(hitColliders[i].tag.Equals("Health")) dest = hitColliders[i].gameObject.transform.position;

                    }

                }

                if(dest != Vector3.zero) {
                    moveTo(dest);
                    directionTimer -= Time.deltaTime;
                }

                if(target != null) state = 2;

            }

            // If target not found, wander towards center of zone
            if(target == null && Vector3.Distance(movePoint, transform.position) < 0.1f || waitTimer <= 0.0f){

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
            else if(Vector3.Distance(movePoint, transform.position) < 0.1f && waitTimer > 0.0f) movePoint = transform.position;
            waitTimer -= Time.deltaTime;
            
            updateWeapon();
            hasWeaponCheck();

        }
        // Target found, starts firing and moving around target position
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
        //TODO: Add check for bot stuck, set timer then choose new position to walk to
        // fix bot jitter
        // check if point moving to is inside of wall collider
   
        if(damageTimer > 0.0f) {

            sr.color = baseColor - new Color(0, 2 * damageTimer, 2 * damageTimer);
            damageTimer -= Time.deltaTime;

        }

        if(healTimer > 0.0f){

            sr.color = baseColor - new Color(healTimer, 0, healTimer);
            healTimer -= Time.deltaTime;

        }

        if(!insideZone && zoneDamageTimer <= 0.0f){

            zoneDamageTimer = 2.0f;
            updateHealth(10);

        }
        else if(!insideZone && zoneDamageTimer > 0.0f) zoneDamageTimer -= Time.deltaTime;

    }

    void FixedUpdate(){

        // Moves character in direction

        Vector2 moveDir = new Vector2(movePoint.x - transform.position.x, movePoint.y - transform.position.y).normalized;

        rb.MovePosition(rb.position + moveDir * Time.deltaTime * 5);

    }

    // Sets currently equiped weapon, called from weapon script
    void setIsEquipped(bool e){

        weaponNearEquipped = e;

    }

    // Handles collisions with weapons
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

        if(col.tag.Equals("Game Manager")) insideZone = true;

        if(col.tag.Equals("Health") && health < 100) {

            updateHealth(-(Math.Min(100 - health, 50)));
            Destroy(col.gameObject);
            
        }

    }

    void OnTriggerExit2D(Collider2D col){

        if(col.tag.Equals("Game Manager")) insideZone = false;

    }

    void OnCollisionStay2D(Collision2D col){

        if(offsetTimer <= 0.0f) {
            offset = UnityEngine.Random.Range(0, 1) * 2 - 1;
            offsetTimer = 2.0f;
        }

        if(col.gameObject.tag.Equals("Object") || col.gameObject.tag.Equals("Water")){
            movePoint.x = (float) (transform.position.x + Math.Sin(moveAngle += 0.1f * offset) * 5);
            movePoint.y = (float) (transform.position.y + Math.Cos(moveAngle += 0.1f * offset) * 5);
        }

        offsetTimer -= Time.deltaTime;
    }

    // Updates rotation of equipped weapon
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

    // Updates health, called from bullets script
    void updateHealth(int damage){

        health -= damage;

        if(health <= 0) {

            transform.gameObject.SetActive(false);
            
            if(weapon != null) {

                weapon.SendMessage("setEquipped", false);
                weapon.SendMessage("setCharacterNull");

            }
            
        }

        if(damage > 0) damageTimer = 0.5f;
        else {
            dest = Vector3.zero;
            healTimer = 0.5f;
        }

    }

    void hasWeaponCheck(){

        //incase bot gets stuck with no weapon
        if(weapon == null || !weapon.activeSelf) {
                
            state = 0;
            weaponNear = null;

        }

    }

    void moveTo(Vector3 destination){
        
        if(directionTimer <= 0.0f){

            directionTimer = (float) rand.NextDouble();

            destAngle = -(float) Math.Atan2(transform.position.x - destination.x + 0.5f, transform.position.y - destination.y) + 3.14f;

            movePoint = new Vector3();
            float tempMoveAngle = 10.0f;

            for(int i = 0; i < 8; i++){

                if(Math.Abs(destAngle - i * 0.785f) < Math.Abs(destAngle - tempMoveAngle)) tempMoveAngle = i * 0.785f;

            }

            moveAngle = -tempMoveAngle;

            movePoint.x = (float) (transform.position.x + Math.Sin(moveAngle) * 5);
            movePoint.y = (float) (transform.position.y + Math.Cos(moveAngle) * 5);
            
        }

    }

    void setZone(Vector3 z){

        zone = (Vector2) z;

    }

}
