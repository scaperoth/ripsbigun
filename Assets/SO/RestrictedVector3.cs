using System.Collections.Generic;
using UnityEngine;

namespace RipsBigun
{
	[CreateAssetMenu]
	public class RestrictedVector3 : ScriptableObject
	{
		[SerializeField]
		private Vector3 _min = Vector3.zero;
		public Vector3 Min
        {
			get
            {
				return _min;
            }
        }
		[SerializeField]
		private Vector3 _max = Vector3.zero;
		public Vector3 Max
		{
			get
			{
				return _max;
			}
		}
	}
}