using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lapCheckpoint : MonoBehaviour
{
    public Transform[] checkpoints;
    public Text lapCounterText;

    private int lapCount = 0;
    private int currentCheckpointIndex = 0;

    void Start()
    {
        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentCheckpointIndex == checkpoints.Length - 1)
            {
                // Car passed through the last checkpoint
                print("ran a lap");
                currentCheckpointIndex = 0;
                lapCount += 1;
                UpdateUI();
            }
            else
            {
                // Car passed through a checkpoint, move to the next one
                print("here");
                currentCheckpointIndex++;
            }
        }
    }

    void UpdateUI()
    {
        if (lapCounterText != null)
        {
            lapCounterText.text = "Lap: " + lapCount;
        }
    }
}
