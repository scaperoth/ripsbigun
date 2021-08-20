using System.Collections.Generic;
using UnityEngine;

namespace RipsBigun
{
	[CreateAssetMenu]
	public class PlayerEvent : ScriptableObject
	{
		private List<PlayerEventListener> listeners =
			new List<PlayerEventListener>();

		public void Raise(PlayerController player)
		{
			for (int i = listeners.Count - 1; i >= 0; i--)
				listeners[i].OnEventRaised(player);
		}

		public void RegisterListener(PlayerEventListener listener)
		{ listeners.Add(listener); }

		public void UnregisterListener(PlayerEventListener listener)
		{ listeners.Remove(listener); }
	}
}