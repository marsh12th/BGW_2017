﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Networker : NetworkManager 
{
	#region DATA
	public static Networker i;
	public static List<Game> players;

	public static bool DedicatedServer 
	{
		get { return (NetworkServer.active && !NetworkServer.localClientActive); }
	}
	public static bool DedicatedClient 
	{
		get { return (NetworkClient.active && !NetworkServer.localClientActive); }
	}
	public static bool IsHost 
	{
		get { return NetworkServer.localClientActive; }
	}
	#endregion

	#region SERVER
	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) 
	{
		// Spawn player object over the net
		var player = Instantiate (playerPrefab).GetComponent<Game> ();
		NetworkServer.AddPlayerForConnection (conn, player.gameObject, playerControllerId);
		player.SetName ("Player");
		players.Add (player);

		#region SCENE BEHAVIOUR
		// Behaviour on what scene where at
		string scene = SceneManager.GetActiveScene ().name;
		if (scene == "Menu")
		{
			// Assign authority to selector
			var selector = GameObject.Find ("Selector_" + conn.connectionId).GetComponent<Selector> ();
			selector.id.AssignClientAuthority (conn);
			selector.SetName ("Selector");
		}
		else
		// If bypassing the Selection Menu
		if (scene == "Tower") 
		{
			// Spawn a different hero for each player & start
			var asignedHero = (Game.Heroes) conn.connectionId;
			player.playingAs = asignedHero;
			player.SpawnHero ();
		} 
		#endregion
	}
	#endregion

	#region CALLBACKS
	private void OnSceneLoad (Scene scene, LoadSceneMode mode) 
	{
		// nothing yet...
	}

	private void Awake () 
	{
		if (DedicatedServer)
			SceneManager.sceneLoaded += OnSceneLoad;
	}
	private void OnDestroy () 
	{
		if (DedicatedServer)
			SceneManager.sceneLoaded -= OnSceneLoad;
	}

	// Creates this object on every scene no matter where
	[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void InitizalizeSingleton () 
	{
		i = Extensions.SpawnSingleton<Networker> ();
		players = new List<Game> (3);
	}
	#endregion
}