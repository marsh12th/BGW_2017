﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HeroesRace.Effectors 
{
	public class SideKnock : NetworkBehaviour 
	{
		public float kickForce;
		public float upForce;

		[ServerCallback] 
		private void OnTriggerEnter (Collider other) 
		{
			var hero = other.GetComponent<Hero> ();
			if (hero != null) 
			{
				// Stop Hero before knocking
				hero.driver.body.velocity = Vector3.zero;
				hero.driver.body.angularVelocity = Vector3.zero;

				// Apply computed force
				var f = KnockForce (hero.transform.position);
				hero.driver.body.AddForceAtPosition (f, transform.position, ForceMode.Impulse);

				// Apply CC to Hero
				hero.blocks.Add ("Knocked ", CCs.Locomotion, 1.5f, unique: false);
			}
		}

		private Vector3 KnockForce (Vector3 heroPos) 
		{
			// Create a helper RTS matrix that looks at the Tower's center
			var position = transform.position;
			var rotation = Quaternion.LookRotation (-position);
			var this2world = Matrix4x4.TRS (position, rotation, Vector3.one);

			// Compare against hero position to get the force direction
			var transPos = this2world.inverse.MultiplyPoint3x4 (heroPos);
			float sign = Mathf.Sign (transPos.x);

			// Get matrix right and comput final force
			var kickDir = this2world.MultiplyVector (Vector3.right);
			kickDir *= sign * kickForce;
			kickDir += Vector3.up * upForce;

			return kickDir;
		}
	} 
}
