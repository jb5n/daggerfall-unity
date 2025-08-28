using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using Mirror;
using UnityEngine;

public class NetConnectionManager : NetSingleton<NetConnectionManager>
{
    // These are all the components that depend on the player existing
    [Header("Player Dependencies")]
    public FloatingOrigin floatingOrigin;
    public StreamingWorld streamingWorld;
    public SunlightManager sunlightManager;
    public WeatherManager weatherManager;
    [Space]
    public GameObject[] objectsToDelayWaking;
    [Header("Scene References")]
    public GameObject interiorParent;
    public GameObject exteriorParent;
    public GameObject dungeonParent;

    protected override void Awake()
    {
        base.Awake();
        NetworkPlayerIdentity.ExecuteOnLocalPlayerInitialized(OnPlayerLoaded);
    }

    private void OnPlayerLoaded(GameObject localPlayer)
    {
        floatingOrigin.Player = localPlayer;
        floatingOrigin.gameObject.SetActive(true);
        streamingWorld.LocalPlayerGPS = localPlayer.GetComponentInChildren<PlayerGPS>();
        streamingWorld.gameObject.SetActive(true);
        sunlightManager.LocalPlayer = localPlayer;
        sunlightManager.gameObject.SetActive(true);
        weatherManager.PlayerWeather = localPlayer.GetComponentInChildren<PlayerWeather>();
        weatherManager.gameObject.SetActive(true);

        foreach (GameObject obj in objectsToDelayWaking)
        {
            obj.SetActive(true);
        }
    }

    [ContextMenu("Disable Delayed Wake GameObjects")]
    void DisableDelayedWakeGameObjects()
    {
        foreach (GameObject obj in objectsToDelayWaking)
        {
            obj.SetActive(false);
        }
    }
}
