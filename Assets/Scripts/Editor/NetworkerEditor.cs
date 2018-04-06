﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CustomEditor (typeof(Networker))]
public class NetworkerEditor : Editor
{
	private bool[] folds;
	Editor editor;

	private bool spawned;
	private Heroes heroToSpawn = Heroes.Indiana;

	/// Adding some utils as buttons
	public override void OnInspectorGUI () 
	{
		/// Draw inherited inspector
		editor.OnInspectorGUI ();

		/// If Hosting, spawn a Hero to play it
		if (!spawned && Networker.IsHost) 
		{
			/// Draw a header
			EditorGUILayout.LabelField ("Play as Hero", EditorStyles.boldLabel);

			/// Select hero and spawn it
			heroToSpawn = (Heroes)EditorGUILayout.EnumPopup ("Hero to spawn", heroToSpawn);
			if (GUILayout.Button ("Spawn & assign to host"))
			{
				Character.Spawn (heroToSpawn, Networker.localPlayer);
				spawned = true;
			}
		}
	}

	private void OnEnable () 
	{
		folds = new bool[3];
		editor = CreateEditor (target as NetworkManager, typeof(NetworkManagerEditor));
	}
}
