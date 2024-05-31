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
        if (Manager.Players.TryGet(Runner.LocalPlayer, out var player) == false)
        {

                    WeaponsUI.UpdateWeapons(player.Weapons);
        }
    }
}
