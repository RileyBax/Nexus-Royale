using System;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private NetworkObject RiflePrefab;
    [SerializeField] private NetworkObject ShotgunPrefab;
    [SerializeField] private NetworkObject SMGPrefab;
    [SerializeField] private Tilemap Tilemap;
    private NetworkObject BotPrefab;
    private System.Random rand = new System.Random();

    [Networked, Capacity(20)] public NetworkDictionary<PlayerRef, Player> Players => default;
    public Dictionary<NetworkId, int> BotSprite = new Dictionary<NetworkId, int>();
    public Dictionary<NetworkId, int> PlayerSprite = new Dictionary<NetworkId, int>();
    [Networked] private float timer {get; set;}
    private bool gameStarted = false;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private PolygonCollider2D zoneCollider;
    private int circlePoints = 60;
    [Networked] private float radius {get; set;}
    private Vector3 circlePointPos;
    [Networked] private Vector3 zone {get; set;}
    private float width = 0.25f;
    private Vector2[] colliderPoints;
    [SerializeField] private AudioManager am;
        
    public void Start(){

        BotPrefab = Resources.Load<NetworkObject>("Prefabs/Bot");
        am = Resources.Load<AudioManager>("Prefabs/Audio Manager");
        lr = this.AddComponent<LineRenderer>();
        zoneCollider = this.AddComponent<PolygonCollider2D>();

        lr.positionCount = circlePoints;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.startColor = Color.white;
        lr.material = new Material (Shader.Find ("Sprites/Default"));
        lr.material.color = Color.white; 
        lr.loop = true;
        lr.sortingOrder = 1;

        colliderPoints = new Vector2[circlePoints];

        this.transform.position -= new Vector3(0,0,-5);

        zoneCollider.isTrigger = true;

        this.tag = "Game Manager";

        timer = 30.0f;

        radius = 400.0f;

    }

    public override void Spawned()
    {
        timer = 30.0f;
        radius = 400.0f;
        Debug.Log("Spawned");

        zone = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), 0);
        
        Instantiate(am);

        //Runner.Spawn(SMGPrefab, new Vector3(0,0,0));

    }

    public void PlayerJoined(PlayerRef player)
    {
        // dont let join if game started
        if (HasStateAuthority)
        {
            NetworkObject playerObject = Runner.Spawn(playerPrefab, new Vector3(0,0, 0), Quaternion.identity, player);
            Players.Add(player, playerObject.GetComponent<Player>());
            PlayerSprite.Add(playerObject.Id, PlayerInfo.Skin);
            playerObject.SendMessage("setSprite", PlayerInfo.Skin);
            playerObject.SendMessage("setTimer", timer - 2);
            playerObject.SendMessage("setZone", (Vector2) zone);
            
        }
    }

    public void PlayerLeft(PlayerRef player) {
        if (!HasStateAuthority)
        {
            return;
        }

        if (Players.TryGet(player, out Player playerScript)) {
            Players.Remove(player);
            Runner.Despawn(playerScript.Object);
            Destroy(playerScript.Object);
        }
    }

    private void SpawnWeapons()
    {
        Tilemap.CompressBounds();
        Vector3 tilemapSize = Tilemap.size;
        int x1 = -(int)(tilemapSize.x / 2);
        int x2 = (int)(tilemapSize.x / 2);
        int y1 = -(int)(tilemapSize.y / 2);
        int y2 = (int)(tilemapSize.y / 2);

        int maxRifles = 5;
        int maxShotguns = 20;
        int maxSMGs = 10;
        int spawned = 0;

        while (spawned < maxRifles)
        {
            var position = new Vector3(UnityEngine.Random.Range(x1, x2), UnityEngine.Random.Range(y1, y2));

            if (!Physics.CheckBox(position, new Vector3(1,1,1)))
            {
                Runner.Spawn(RiflePrefab, position);
                spawned++;
            }
        }

        spawned = 0;

        while (spawned < maxShotguns)
        {
            var position = new Vector3(UnityEngine.Random.Range(x1, x2), UnityEngine.Random.Range(y1, y2));

            if (!Physics.CheckBox(position, new Vector3(1, 1, 1)))
            {
                Runner.Spawn(ShotgunPrefab, position);
                spawned++;
            }
        }

        spawned = 0;

        while (spawned < maxSMGs)
        {
            var position = new Vector3(UnityEngine.Random.Range(x1, x2), UnityEngine.Random.Range(y1, y2));

            if (!Physics.CheckBox(position, new Vector3(1, 1, 1)))
            {
                Runner.Spawn(SMGPrefab, position);
                spawned++;
            }
        }

    }

    private void SpawnBots(){

        Tilemap.CompressBounds();
        Vector3 tilemapSize = Tilemap.size;
        int x1 = -(int)(tilemapSize.x / 2);
        int x2 = (int)(tilemapSize.x / 2);
        int y1 = -(int)(tilemapSize.y / 2);
        int y2 = (int)(tilemapSize.y / 2);

        int spawned = 0;
        int max = 20 - Players.Count;

        while(spawned < max){

            var position = new Vector3(UnityEngine.Random.Range(x1, x2), UnityEngine.Random.Range(y1, y2));

            if (!Physics.CheckBox(position, new Vector3(1, 1, 1))){

                int sprite = rand.Next(1, 10);

                NetworkObject botObject = Runner.Spawn(BotPrefab, position);
                BotSprite.Add(botObject.Id, sprite);
                botObject.SendMessage("setSprite", BotSprite[botObject.Id]);
                botObject.SendMessage("setZone", (Vector2) zone);

                spawned++;

            }

        }

    }

    public override void FixedUpdateNetwork()
    {

        if(timer <= 0.0f && !gameStarted) {
            gameStarted = true;
            RpcStartGame();
        }


        for(int i = 0; i < circlePoints; i++){

            circlePointPos.x = (float)(zone.x + Math.Sin((2 * Math.PI) / circlePoints * i + 1) * radius);
            circlePointPos.y = (float)(zone.y + Math.Cos((2 * Math.PI) / circlePoints * i + 1) * radius);

            lr.SetPosition(i, circlePointPos);

            colliderPoints[i] = new Vector2(circlePointPos.x, circlePointPos.y) - (Vector2) zone;

        }

        zoneCollider.SetPath(0, colliderPoints);
        zoneCollider.transform.position = this.transform.position;

    }

    void FixedUpdate(){

        timer -= Runner.DeltaTime;

        if(radius >= 10) radius -= Runner.DeltaTime;

        if(timer <= 0.0f && !gameStarted) {
            gameStarted = true;
            RpcStartGame();
        }

        for(int i = 0; i < circlePoints; i++){

            circlePointPos.x = (float)(zone.x + Math.Sin((2 * Math.PI) / circlePoints * i + 1) * radius);
            circlePointPos.y = (float)(zone.y + Math.Cos((2 * Math.PI) / circlePoints * i + 1) * radius);

            lr.SetPosition(i, circlePointPos);

            colliderPoints[i] = new Vector2(circlePointPos.x, circlePointPos.y) - (Vector2) zone;

        }

        zoneCollider.SetPath(0, colliderPoints);
        transform.position = zone;

    }

    [Rpc]
    public void RpcStartGame(){

        if(Runner.IsServer){

            // UnityEngine.Randomize all player positions
            // Spawn remainer empty spaces with bots
            var sessionInfo = Runner.SessionInfo;

            if (sessionInfo.IsOpen) 
            {
                sessionInfo.IsOpen = false;
            }

            Tilemap.CompressBounds();
            Vector3 tilemapSize = Tilemap.size;
            int x1 = -(int)(tilemapSize.x / 2);
            int x2 = (int)(tilemapSize.x / 2);
            int y1 = -(int)(tilemapSize.y / 2);
            int y2 = (int)(tilemapSize.y / 2);

            foreach(KeyValuePair<PlayerRef, Player> p in Players){

                int spawned = 0;

                while(spawned < 1){

                    var position = new Vector3(UnityEngine.Random.Range(x1, x2), UnityEngine.Random.Range(y1, y2));

                    if (!Physics.CheckBox(position, new Vector3(1, 1, 1))){

                        p.Value.transform.position = position;
                        spawned++;

                    }

                }

            }

            if (Runner.IsServer) {
                SpawnWeapons();
                SpawnBots();
            }

            transform.position = zone;

        }

    }

    public void CloseServer(){

        if(Runner.IsServer) {
            
            if(Players.Count <= 1){

                SceneManager.LoadScene("Nexus Royale");
                Runner.Shutdown();

            } 
            
        }
        else{
            PlayerLeft(Runner.LocalPlayer);
            SceneManager.LoadScene("Nexus Royale");
            Runner.Shutdown();
        }

    }

    public void Quit(){

        if(Runner.IsServer) {
            
            if(Players.Count <= 1){

                SceneManager.LoadScene("Nexus Royale");
                Runner.Shutdown();
                Application.Quit();

            } 
            
        }
        else{
            PlayerLeft(Runner.LocalPlayer);
            SceneManager.LoadScene("Nexus Royale");
            Runner.Shutdown();
            Application.Quit();
        }

    }

}
