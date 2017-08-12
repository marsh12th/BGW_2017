﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// Rota el selector de personaje
/// y permite seleccionar uno
public class Selector : NetworkBehaviour
{
	#region INTERNAL DATA
	[SyncVar]
	public PJs pj;                      // El personaje selccionado
	int charId
	{
		get { return ( int ) pj; }
		set { pj = ( PJs ) value; }
	}
	Sprite[] personajes;                // El orden tiene que coincidir con la enum!

	public Vector2 pos;
	public Image current;
	public Image next;
	public GameObject focus;            // Marca de cual es nuestro personje
	public GameObject selected;         // Indicador de seleccion

	[SyncVar]
	bool done;              // Personaje seleccionado?
	[SyncVar]
	bool sliding;               // Animacion en marcha?
	RectTransform rect;
	Animator anim;
	#endregion

	#region SELECTING CHAR
	static int playersDone;             // Cantidad de jugadores listos
	[Command]
	void Cmd_Select( bool done ) 
	{
		playersDone += done ? +1 : -1;
		if (playersDone == 3)
		{
			UI.manager.currentScreen = UI.Pantallas.TodosListos;
			NetworkManager.singleton.ServerChangeScene ("Torre");
		}

		selected.SetActive (done);      // Mostrar indicador de seleccion (servidor)
		this.done = done;               // Bloquear sliding
		Rpc_Select (done);              // Mostrar indicador de seleccion (cliente)
	}
	[ClientRpc]
	void Rpc_Select( bool done ) 
	{
		selected.SetActive (done);
	}
	#endregion

	#region SLIDING
	public void CorrectSprite() 
	{
		/// Esto se ejecuta para intercambiar los splasharts
		/// y que no se note el cambio (animacion)
		current.sprite = personajes[charId];
		if (isServer) sliding = false;
	}

	[Command]
	void Cmd_CorrectSlideID( int dir ) 
	{
		sliding = true;                     // Bloquea el sliding (porque ya se esta ejecutando)
		var max = personajes.Length;

		/// Cambia los splasharts en base al movimiento
		charId += dir;
		// Asegura el loop
		if (charId == -1) charId = max-1;
		else
		if (charId == max) charId = 0;

		next.sprite = personajes[charId];   // Cambiar el splashart siguiente (servidor)
		Rpc_CorrectSlideID (charId);        // Cambiar el splashart siguiente (cliente)
	}
	[ClientRpc]
	void Rpc_CorrectSlideID( int id ) 
	{
		next.sprite = personajes[id];
	}
	#endregion

	#region CALLBACKS
	private void Update()
	{
		if (rect.anchoredPosition != pos) rect.anchoredPosition = pos;
		if (!hasAuthority || isServer) return;

		/// En caso de que se pulse tecla de mover
		/// deslizar targeta de personaje.
		var dir = InputX.GetMovement ();
		if (!sliding)
		{
			/// Animacion
			if (dir != 0 && !done)
			{
				sliding = true;
				Cmd_CorrectSlideID (( int ) dir);
				anim.SetTrigger ((dir == -1) ? "SlideLeft" : "SlideRight");
			}

			/// Seleccionar personaje
			if (InputX.GetKeyDown (PlayerActions.GreenBtn)
			&& UI.manager.currentScreen == UI.Pantallas.SeleccionPersonaje)
			{
				if (!done)
				{
					done = true;   // auto avoid player from sliding
					Cmd_Select (true);
				}
				else
				{
					// only can slide again when server provides
					selected.SetActive (false);
					Cmd_Select (false);
				}
			}
		}
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority ();
		if (isClient) focus.SetActive (true);
	}
	private void Start()
	{
		personajes = UI.manager.personajes;
		rect = GetComponent<RectTransform> ();
		anim = GetComponent<Animator> ();
		current.sprite = personajes[charId];
	} 
	#endregion
}

public enum PJs 
{
	/// Lista de todos los personajes
	/// que se pueden seleccionar
	NONE,   // => Espectador
	Gatete,
	Sir,
	Random_0,
	Random_1
	// En un futuro cuidado con
	// cambiar los nombres! => El orden tiene que coincidir con la array!
}