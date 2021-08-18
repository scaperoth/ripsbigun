using UnityEngine;

namespace RipsBigun
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField]
        protected float _giveDamageAmount = 10f;

        public float GiveDamageAmount
        {
            get
            {
                return _giveDamageAmount;
            }
        }
    }

}