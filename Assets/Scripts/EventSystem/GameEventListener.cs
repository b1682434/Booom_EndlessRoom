using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent response;

    private void OnEnable()
    {
        gameEvent.RegListener(this);
    }
    private void Start()
    {
        gameEvent.RegListener(this);
    }
    private void OnDisable()
    {
        gameEvent.UnRegListener(this);
    }
    private void OnDestroy()
    {
        gameEvent.UnRegListener(this);
    }
    public void onEventRaised()
    {
       
        response.Invoke();
    }
}
