using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextRoom : MonoBehaviour
{
  public void NextTimeSpawnNextRoom()
    {
        FindFirstObjectByType<GameManger>().ChangeRoomPrefab();
    }
}
