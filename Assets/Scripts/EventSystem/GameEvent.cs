using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Events", menuName = "ScriptableObjects/gameEvents", order = 2)]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise()
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].onEventRaised();
        }
    }

    public void RegListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnRegListener(GameEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}
