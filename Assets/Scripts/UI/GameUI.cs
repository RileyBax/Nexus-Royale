using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameManager Manager;
    [HideInInspector]
    public NetworkRunner Runner;
    public UIWeapons WeaponsUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Runner = Manager.Runner;

        Player playerObject = Manager.Players.Get(Runner.LocalPlayer);
        if (playerObject != null)
        {
            WeaponsUI.UpdateWeapons(playerObject.Weapons);
        }
    }
}
