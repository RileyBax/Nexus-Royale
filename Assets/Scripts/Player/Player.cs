using Fusion;
using Fusion.StatsInternal;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public Weapons Weapons;
    private List<GameObject> nearbyWeapons;
    private UnityEngine.UI.Image[] inventoryUI;
    [SerializeField] private GameObject hud;
    // Camera target (the main camera) reference
    [SerializeField] private Transform camTarget;
    [Networked] private int Health {get; set;}
    [Networked] public string Name { get; private set; }
    private int lastHealth = 100;
    [SerializeField] SpriteRenderer sr;

    // Is the player ready to play
    private bool isReady;

    private Rigidbody2D rigidBody;
    [SerializeField] private Sprite[] spriteIdle;
    [SerializeField] private Sprite[] spriteWalk;
    [Networked] private int selectedSprite {get; set;} // 1 - 10
    [Networked] private float timer {get; set;}
    private float frameTime = 0;
    private int frame;
    private int startFrame;
    public bool isMoving = false;
    public bool up = false;
    public bool down = true;
    public bool side = false;
    private float damageTimer;
    private Color baseColor = new Color(1, 1, 1, 1);
    [SerializeField] NetworkObject deathEmitter;
    [SerializeField] GameObject healthbarObject;
    [SerializeField] GameObject LobbyTimerObject;
    private RectTransform healthbar;
    private float healthbarLength;
    private LobbyTimerScript lobbyTimerScript;
    [SerializeField] GameObject minimapObject;
    private GameObject minimap;
    [SerializeField] GameObject minimapCamObject;
    private GameObject minimapCam;
    private float zoneDamageTimer = 2.0f;
    private bool insideZone = true;
    [SerializeField] GameObject arrowObject;
    private ArrowScript arrow;
    [Networked] private Vector2 zone {get;set;}
    private float stepTimer = 0.0f;
    [SerializeField] AudioManager am;
    [SerializeField] GameObject ePopupObject;
    private GameObject ePopup;
    private float arrowTimer;
    private bool hasSeen;
    private int clientInvSelect = 0;
    private bool[] clientEquipped = new bool[3];
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject deathScreen;
    private bool gameStarted = false;
    private bool hasWon = false;
    private float activePlayerTimer = 5.0f;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < clientEquipped.Length; i++){

            clientEquipped[i] = false;

        }

        for(int i = 0; i < inventoryUI.Length; i++) inventoryUI[i] = GameObject.Find("UI").transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        
    }

    public override void Spawned()
    {
        
        Weapons = new Weapons();

        this.nearbyWeapons = new List<GameObject>();
        this.inventoryUI = new UnityEngine.UI.Image[3];

        try{
            am = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();
        }
        catch{
            am = Instantiate(Resources.Load<AudioManager>("Prefabs/Audio Manager"));
        }

        am.mVolume = PlayerInfo.MusicVolume;
        am.sVolume = PlayerInfo.SoundVolume;

        if (HasInputAuthority)
        {

            Name = PlayerInfo.Username;
            RPC_PlayerName(Name);
            selectedSprite = PlayerInfo.Skin;
            RPC_PlayerSprite(selectedSprite);

            CameraFollower.Singleton.SetTarget(camTarget);

            hud = GameObject.Find("UI");
            hud.transform.GetChild(0).transform.position = new Vector3(5.2f, -3.5f);
            for(int i = 0; i < inventoryUI.Length; i++) inventoryUI[i] = hud.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<UnityEngine.UI.Image>();
            

            healthbar = Instantiate(healthbarObject, hud.transform).GetComponentsInChildren<RectTransform>()[1];
            healthbarLength = 570;
            healthbar.SetAnchors(0.025f, 0.975f, 0.5f, 0.5f);
            healthbar.offsetMin = new Vector2(0, healthbar.offsetMin.y);
            healthbar.offsetMax = new Vector2(0, healthbar.offsetMax.y);

            lobbyTimerScript = Instantiate(LobbyTimerObject, hud.transform).GetComponent<LobbyTimerScript>();

            hud.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 32;
            hud.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            hud.transform.GetChild(1).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;

            minimap = Instantiate(minimapObject, hud.transform);

            minimapCam = Instantiate(minimapCamObject);

            // arrow doesnt look good but works, is very jittery to host
            arrow = Instantiate(arrowObject).GetComponent<ArrowScript>();
            arrow.zone = zone;
            arrow.gameObject.SetActive(false);

            am.AuthPlayer = this.gameObject;

            ePopup = Instantiate(ePopupObject, this.transform);
            ePopup.SetActive(false);

            deathScreen = GameObject.Find("DeathScreen");
            winScreen = GameObject.Find("WinScreen");

            deathScreen.SetActive(false);
            winScreen.SetActive(false);

            am.PlayMusic("Game");

        }

        spriteIdle = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " idle");
        spriteWalk = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " walk");

        setTimer(timer);

        Health = 100;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetInput data))
        {
            rigidBody.velocity = data.Velocity * 5;

            if (data.WeaponChange != 0)
            {
                Weapons.ChangeWeapon(data.WeaponChange);
            }

            if (data.PickupWeapon && nearbyWeapons.Count > 0)
            {
                PickupWeapon(nearbyWeapons[0]);
            }

            Weapon weapon = Weapons.GetSelectedWeapon();
            if (weapon != null)
            {
                Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
                float angle = -(float)Math.Atan2(data.MousePos.y - transform.position.y, data.MousePos.x - transform.position.x) + 1.55f;

                Vector3 weaponPos = Vector3.zero;

                weaponPos.x = (float)(transform.position.x + Math.Sin(angle) * 1);
                weaponPos.y = (float)(transform.position.y + Math.Cos(angle) * 1);

                weapon.transform.position = weaponPos;
                weapon.transform.up = new Vector2(data.MousePos.x - transform.position.x, data.MousePos.y - transform.position.y);
                
                if (data.FireWeapon)
                {
                    weapon.FireWeapon();
                }
            }

            RpcUpdateSpriteState(data);

        }

        double ping = Runner.GetPlayerRtt(PlayerRef.None) * 1000;

        if (hud != null)
        {

        hud.transform.GetChild(1).GetComponent<TMP_Text>().text = ping.ToString("0") + "ms";
        }

        RpcUpdateSprite();

    }

    void FixedUpdate(){

        UpdateHealthBar();

        if(timer > 0) timer -= Runner.DeltaTime;

        if(lastHealth > Health) {
            damageTimer = 1.0f;
            if(am != null) am.PlaySFX("Hit", this.gameObject);
        }
        lastHealth = Health;

        if(Health <= 0) {
            Instantiate(deathEmitter, this.transform.position, Quaternion.identity);
            if(am != null) am.PlaySFX("Death", this.gameObject);
            if (HasInputAuthority)
            {
            gameManager.RPC_PlayerKilled(PlayerInfo.Username);
                deathScreen.SetActive(true);
            }
                transform.gameObject.SetActive(false);
        }

        if(minimapCam != null) minimapCam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        if(!insideZone && zoneDamageTimer <= 0.0f){

            zoneDamageTimer = 2.0f;
            updateHealth(10);

        }
        else if(!insideZone && zoneDamageTimer > 0.0f) zoneDamageTimer -= Time.deltaTime;

        if(isMoving && HasInputAuthority){

            if(stepTimer <= 0.0f) {
                if(this.gameObject != null) am.PlaySFX("Walk", this.gameObject);
                stepTimer = 0.4f;
            }
            else stepTimer -= Runner.DeltaTime;

        }

        if(arrow != null && arrowTimer <= 0.0f) arrow.gameObject.SetActive(true);
        else arrowTimer -= Runner.DeltaTime;

        for(int i = 0; i < 3; i++){

            if (i != clientInvSelect) inventoryUI[i].color = Color.grey;
            else inventoryUI[i].color = Color.white;

        }

        if(timer <= 0.0f && !gameStarted){

            gameStarted = true;

        }

        if(gameStarted && activePlayerTimer <= 0.0f){

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            int activePlayer = 0;
            for(int i = 0; i < allObjects.Length; i++){

                if(allObjects[i].activeInHierarchy  && allObjects[i].tag.Equals("Character")) activePlayer++;

            }

            if(activePlayer <= 1 && !hasWon) {

                hasWon = true;

                if(HasInputAuthority){

                    winScreen.SetActive(true);

                }

                Debug.Log("Winner");
            }

            activePlayerTimer = 5.0f;

        }
        else activePlayerTimer -= Runner.DeltaTime;

    }

    void Update(){

        if(arrow != null) arrow.playerPos = transform.position;

        if(Input.GetKeyDown(KeyCode.Alpha1)){
            clientInvSelect = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            clientInvSelect = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            clientInvSelect = 2;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.8f);
        if(ePopup != null){

            ePopup.SetActive(false);
            if(!hasSeen) {

                for(int i = 0; i < hitColliders.Length; i++){

                    if(hitColliders[i].tag.Equals("Weapon") && !hitColliders[i].gameObject.GetComponent<Weapon>().GetEquipped() && !hasSeen) ePopup.SetActive(true);

                }

            }
            else ePopup.SetActive(false);

            if(Input.GetKeyDown(KeyCode.E) && !hasSeen) hasSeen = true;

        }

        hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.8f);
        if(Input.GetKey(KeyCode.E) && hitColliders.Length > 0 && !clientEquipped[clientInvSelect]){
            for(int i = 0; i < hitColliders.Length; i++){
                if(hitColliders[i].tag.Equals("Weapon")) {
                    if(!hitColliders[i].gameObject.GetComponent<Weapon>().clientEquipped){
                        inventoryUI[clientInvSelect].sprite = hitColliders[i].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                        clientEquipped[clientInvSelect] = true;
                        hitColliders[i].gameObject.GetComponent<Weapon>().clientEquipped = true;
                    }
                }
            }

        }

    }

    public void PickupWeapon(GameObject w)
    {
        Weapon weapon = w.GetComponent<Weapon>();
        if(!weapon.GetEquipped()){
            Weapons.PickupWeapon(weapon);
            weapon.SetPlayer(this.gameObject);
            weapon.setEquipped(true);
            
            nearbyWeapons.Remove(w);
        }
    }

    public void AddNearbyWeapon(GameObject weapon)
    {
        nearbyWeapons.Add(weapon);
        Debug.Log(nearbyWeapons.Count);
    }

    public void RemoveNeabyWeapon(GameObject weapon)
    {
        nearbyWeapons.Remove(weapon);
    }

    // Updates health, called from bullet script
    public void updateHealth(int damage){

        Health -= damage;

    }

    [Rpc]
    void RpcUpdateSprite(){

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

    [Rpc]
    void RpcUpdateSpriteState(NetInput data){

        // Handles animations states

        if(data.Velocity.x == 0.0f && data.Velocity.y == 0.0f) isMoving = false;
        else isMoving = true;

        if(data.Velocity.y >= 1 && up != true){
            frame = 1;
            startFrame = frame;
            up = true;
            down = false;
            side = false;
        }
        else if(data.Velocity.y <= -1 && down != true){
            frame = 0;
            startFrame = frame;
            up = false;
            down = true;
            side = false;
        }
        else if(data.Velocity.x >= 1 && data.Velocity.y >= -0.1 && data.Velocity.y <= 0.1 && side != true 
        || data.Velocity.x <= -1 && data.Velocity.y >= -0.1 && data.Velocity.y <= 0.1 && side != true){
            frame = 2;
            startFrame = frame;
            up = false;
            down = false;
            side = true;
        }

        if(data.Velocity.x <= -1 && sr.flipX != true) sr.flipX = true;
        else if(data.Velocity.x >= 1 && sr.flipX != false) sr.flipX = false;

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_PlayerName(string name)
    {
        Name = name;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_PlayerSprite(int skin)
    {
        selectedSprite = skin;
    }

    public void setSprite(int s){

        selectedSprite = s;

        spriteIdle = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " idle");
        spriteWalk = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " walk");

    }

    public void UpdateHealthBar(){

        if(healthbar != null) healthbar.offsetMax = new Vector2(-(healthbarLength + (healthbarLength * (Health / -100.0f))), healthbar.offsetMax.y);

    }

    public void setTimer(float t){

        if(timer < 60) timer = t - 4;
        else timer = t;

        if(lobbyTimerScript != null) {
            lobbyTimerScript.timer = t;
            arrowTimer = t;
        }

        //timer += 20;

    }

    void OnTriggerEnter2D(Collider2D col){

        if(col.tag.Equals("Game Manager")) insideZone = true;

    }

    void OnTriggerExit2D(Collider2D col){

        if(col.tag.Equals("Game Manager")) insideZone = false;

    }

    void setZone(Vector2 z){

        try{
            zone = z;
            arrow.zone = zone;
        }
        catch(Exception e) {
            Debug.Log(e);
        }

    }

}
