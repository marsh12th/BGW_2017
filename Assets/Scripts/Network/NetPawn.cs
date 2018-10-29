﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HeroesRace 
{
	public class NetPawn : NetBehaviour 
	{
		[Info] public Player owner;

		internal virtual void OnStartOwnership (){ }
		internal virtual void OnStopOwnership () { }

		public void UpdateName () 
		{
			string name = SharedName;
			if (NetworkClient.active)
			{
				if (isLocalPlayer || owner == Net.me)
					name = name.Insert (0, "[OWN] ");
				else
				{
					if (owner) name = name.Insert (0, "[OTHER] ");
					else name = name.Insert (0, "[-PAWN-] ");
				}
			}
			else
			if (NetworkServer.active) 
			{
				if (GetComponent<NetworkIdentity> ().serverOnly)
					name = name.Insert (0, "[SERVER-ONLY] ");
				else
				{
					if (owner != null)
					{
						name = name.Insert (0, "CLIENT] ");
						name = name.Insert (0, "[" + owner.ID + ":");
					}
					else name = name.Insert (0, "[-PAWN-] ");
				}
			}
			// Show on inspector
			this.name = name;
		}

	}
}