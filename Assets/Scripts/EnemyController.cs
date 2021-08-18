using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RipsBigun
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        float _giveDamageAmount = 10f;
        public float GiveDamageAmount
        {
            get
            {
                return _giveDamageAmount;
            }
        }
    }

}
