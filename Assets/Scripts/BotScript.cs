using System;
using Fusion;
using UnityEngine;

public class BotScript : NetworkBehaviour
{

    public GameObject weaponNear;
    private GameObject weapon;
    private Weapon weaponScript;
    private Vector3 weaponPos;
    private float angle = 0;
    private bool weaponNearEquipped;
    private float weaponNearAngle;
    private float moveAngle;
    [SerializeField] Rigidbody2D rb;
    private Vector3 movePoint;
    private float directionTimer = 0.0f;
    private System.Random rand = new System.Random();
    public int state = 0; // 0 = searching for weapon, 1 = has weapon searching for player, 2 = attacking player
    [Networked] private int Health {get; set;}
    private int lastHealth = 100;
    private Collider2D[] hitColliders;
    private GameObject target;
    private Vector2 zone;
    private float wanderAngle;
    public float waitTimer;
    public float offsetTimer = 2.0f;
    private int offset;
    private Vector2 moveDir;
    private float frameTime = 0;
    private int frame;
    private int startFrame;
    public bool isMoving = false;
    public bool up = false;
    public bool down = true;
    public bool side = false;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] private Sprite[] spriteIdle;
    [SerializeField] private Sprite[] spriteWalk;
    [Networked] private int selectedSprite {get; set;}
    private Vector2 lastPos;
    private float lastPosTimer = 0.1f;
    [SerializeField] private NetworkTransform nt;
    private float damageTimer;
    private Color baseColor = new Color(1, 1, 1, 1);
    [SerializeField] NetworkObject deathEmitter;
    private bool alive = true;
    private float zoneDamageTimer = 2.0f;
    private bool insideZone = true;

    // Start is called before the first frame update
    public override void Spawned()
    {
        
        // have game manager set zone center position
        zone = new Vector2(0, 0);

        spriteIdle = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " idle");
        spriteWalk = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " walk");

        Health = 100;

    }

    public override void FixedUpdateNetwork(){
        
        // Searching for weapon
        if(state == 0){

            if(weaponNear == null){
                
                // Check for weapon in circle around character
                hitColliders = Physics2D.OverlapCircleAll(transform.position, 20);
                
                for(int i = 0; i < hitColliders.Length; i++){

                    if(weaponNear == null && hitColliders[i].tag.Equals("Weapon")) weaponNear = hitColliders[i].gameObject;
                    else if(weaponNear != null && Vector3.Distance(weaponNear.transform.position, transform.position) > Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position) 
                    && hitColliders[i].tag.Equals("Weapon")){
                        
                        hitColliders[i].GetComponent<Weapon>().SendMessage("getEquipped", transform.gameObject);

                        if(!weaponNearEquipped) weaponNear = hitColliders[i].gameObject;

                    }

                }

            }
            // Changes direction the bot is moving in, stops bot jittering
            else if(Vector3.Distance(movePoint, transform.position) < 0.1f || directionTimer <= 0.0f){

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

            // set timer for change direction of bot movement

            directionTimer -= Time.deltaTime;

            // check if weapon is picked up by another bot/player

            if(weaponNear != null){

                weaponNear.GetComponent<Weapon>().SendMessage("getEquipped", transform.gameObject);
                if(weaponNearEquipped) weaponNear = null;

            }

            // TODO: Make bot wander to center of zone if it cant find a weapon
            if(weaponNear == null && Vector3.Distance(movePoint, transform.position) < 0.1f || waitTimer <= 0.0f){

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

            if(target == null){

                // Check for target in circle
                hitColliders = Physics2D.OverlapCircleAll(transform.position, 20);

                for(int i = 0; i < hitColliders.Length; i++){

                    if(target == null && hitColliders[i].tag.Equals("Character") && hitColliders[i].gameObject != transform.gameObject) target = hitColliders[i].gameObject;
                    else if(target != null && Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position)
                    && hitColliders[i].tag.Equals("Character") && hitColliders[i].gameObject != transform.gameObject) target = hitColliders[i].gameObject;

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
            waitTimer -= Runner.DeltaTime;
            
            updateWeapon();
            hasWeaponCheck();

        }
        // Target found, starts firing and moving around target position
        else if(state == 2){

            if(target != null) {
                
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

        moveDir = new Vector2(movePoint.x - transform.position.x, movePoint.y - transform.position.y).normalized;

        rb.MovePosition(rb.position + moveDir * Runner.DeltaTime * 10);

        UpdateSpriteState();
        UpdateSprite();

        if(lastPosTimer <= 0.0f){

            lastPos = nt.transform.position;
            lastPosTimer = 0.1f;
            

        }
        else lastPosTimer -= Runner.DeltaTime;

        if(lastHealth > Health) {
            damageTimer = 1.0f;
        }
        lastHealth = Health;

        if(!insideZone && zoneDamageTimer <= 0.0f){

            zoneDamageTimer = 2.0f;
            updateHealth(10);

        }
        else if(!insideZone && zoneDamageTimer > 0.0f) zoneDamageTimer -= Time.deltaTime;

    }

    // Runs visuals client sided
    void FixedUpdate(){

        moveDir = new Vector2(movePoint.x - transform.position.x, movePoint.y - transform.position.y).normalized;

        rb.MovePosition(rb.position + moveDir * Runner.DeltaTime * 10);

        UpdateSpriteState();
        UpdateSprite();

        if(lastPosTimer <= 0.0f){

            lastPos = nt.transform.position;
            lastPosTimer = 0.1f;
            

        }
        else lastPosTimer -= Runner.DeltaTime;

        if(lastHealth > Health) {
            damageTimer = 1.0f;
        }
        lastHealth = Health;

        if(Health <= 0 && alive){

            Instantiate(deathEmitter, this.nt.transform.position, Quaternion.identity);
            Runner.Despawn(this.GetComponent<NetworkObject>());
            transform.gameObject.SetActive(false);

            alive = false;

        }

        if(!insideZone && zoneDamageTimer <= 0.0f){

            zoneDamageTimer = 2.0f;
            updateHealth(10);

        }
        else if(!insideZone && zoneDamageTimer > 0.0f) zoneDamageTimer -= Time.deltaTime;

    }

    void UpdateSprite(){

        // Visual feed back for health change
        if(damageTimer > 0.0f) {

            sr.color = baseColor - new Color(0, damageTimer, damageTimer, 0);
            damageTimer -= Runner.DeltaTime;

        }

        frameTime += Runner.DeltaTime;

        if(isMoving == false){

            if(frameTime >= 1){

                frame += 3;
                frameTime = 0.5f;

            }
            
            sr.sprite = spriteIdle[frame % 6];

        }
        else{

            if(frameTime >= 1){

                frame += 3;
                frameTime = 0.7f;

            }

            sr.sprite = spriteWalk[frame % 12];

        }

    }

    void UpdateSpriteState(){

        // Handles animations states
        moveDir = ((Vector2) nt.transform.position - lastPos) * 5;

        if(moveDir.x == 0.0f && moveDir.y == 0.0f) isMoving = false;
        else isMoving = true;

        if(moveDir.y >= 1 && up != true){
            frame = 1;
            startFrame = frame;
            up = true;
            down = false;
            side = false;
        }
        else if(moveDir.y <= -1 && down != true){
            frame = 0;
            startFrame = frame;
            up = false;
            down = true;
            side = false;
        }
        else if(moveDir.x >= 1 && moveDir.y >= -0.1 && moveDir.y <= 0.1 && side != true 
        || moveDir.x <= -1 && moveDir.y >= -0.1 && moveDir.y <= 0.1 && side != true){
            frame = 2;
            startFrame = frame;
            up = false;
            down = false;
            side = true;
        }

        if(moveDir.x <= -1 && sr.flipX != true) sr.flipX = true;
        else if(moveDir.x >= 1 && sr.flipX != false) sr.flipX = false;

    }

    // Sets currently equiped weapon, called from weapon script
    void setIsEquipped(bool e){

        weaponNearEquipped = e;

    }

    // Handles collisions with weapons
    void OnTriggerEnter2D(Collider2D col){

        if(col.tag.Equals("Weapon") && state == 0){

            weaponScript = col.gameObject.GetComponent<Weapon>();

            weaponScript.SendMessage("getEquipped", transform.gameObject);

            if(!weaponNearEquipped){

                weapon = col.gameObject;
                weaponScript.setEquipped(true);
                weaponScript.SetPlayer(this.gameObject);

                state = 1;

            }

        }

        if(col.tag.Equals("Game Manager")) insideZone = true;

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
            movePoint.x = (float) (transform.position.x + Math.Sin(moveAngle += 0.2f * offset) * 5);
            movePoint.y = (float) (transform.position.y + Math.Cos(moveAngle += 0.2f * offset) * 5);
        }

        offsetTimer -= Time.deltaTime;
    }

    // Updates rotation of equipped weapon
    void updateWeapon(){

        if(target != null) {

            weaponScript.transform.up = new Vector2(target.transform.position.x - nt.transform.position.x, target.transform.position.y - nt.transform.position.y);

            angle = -(float) Math.Atan2(target.transform.position.y - nt.transform.position.y, target.transform.position.x - nt.transform.position.x) + 1.55f;

            weaponPos = Vector3.zero;

            if(Vector3.Distance(transform.position, target.transform.position) < 10) weaponScript.SendMessage("FireWeapon", new Vector3(target.transform.position.x, target.transform.position.y, 0));

        }

        weaponPos.x = (float) (nt.transform.position.x + Math.Sin(angle) * 1);
        weaponPos.y = (float) (nt.transform.position.y + Math.Cos(angle) * 1);

        weaponScript.transform.position = weaponPos;

    }

    // Updates health, called from bullets script
    void updateHealth(int damage){

        Health -= damage;

        if(Health <= 0) {
            
            if(weapon != null) {

                weapon.SendMessage("setEquipped", false);
                weapon.SendMessage("RemovePlayer");

            }
            
        }

        //if(damage > 0) damageTimer = 1f;

    }

    void hasWeaponCheck(){

        //incase bot gets stuck with no weapon
        if(weapon == null || !weapon.activeSelf) {
                
            state = 0;
            weaponNear = null;

        }

    }

    public void setSprite(int s){

        selectedSprite = s;

        spriteIdle = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " idle");
        spriteWalk = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " walk");

    }

    public void setZone(Vector2 z){

        zone = z;

    }

}
