using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillFeed : MonoBehaviour
{
    public GameObject killfeedEntryPrefab;

    // Method to add a kill entry
    public void AddKillEntry(string username)
    {
        GameObject entry = Instantiate(killfeedEntryPrefab, transform);
        TMP_Text entryText = entry.GetComponent<TMP_Text>();
        entryText.text = $"{username} was killed!";
        Destroy(entry, 6.0f);
    }
}
