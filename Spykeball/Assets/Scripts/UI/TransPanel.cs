using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransPanel : Panel
{
    // delegates and events
    public delegate void transitionEvent(string eventName);
    public static event transitionEvent doOnTransitionEvent;

    public void fireTransitionEvent(string eventName) {
        doOnTransitionEvent(eventName);
    }
}
