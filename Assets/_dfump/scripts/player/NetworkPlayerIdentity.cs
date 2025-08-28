using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayerIdentity : NetworkBehaviour
{
    public static NetworkPlayerIdentity localPlayer = null;

    private static Action<GameObject> onLocalPlayerInitialized;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (localPlayer == null)
        {
            localPlayer = this;
            onLocalPlayerInitialized?.Invoke(gameObject);
        }
        else
        {
            Debug.LogWarning("Network player already spawned!");
        }
    }

    public static void ExecuteOnLocalPlayerInitialized(Action<GameObject> action)
    {
        if (localPlayer != null)
        {
            action(localPlayer.gameObject);
        }
        else
        {
            onLocalPlayerInitialized += action;
        }
    }
}
