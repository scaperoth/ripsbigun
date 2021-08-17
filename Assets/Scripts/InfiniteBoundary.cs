using UnityEngine;

namespace RipsBigun
{
    public class InfiniteBoundary : MonoBehaviour
    {
        [SerializeField]
        private Transform _followTarget;
        private Transform _transform;
        // Start is called before the first frame update
        void Start()
        {
            _transform = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            _transform.position = new Vector3(_followTarget.localPosition.x, _transform.position.y, _transform.position.z);
        }
    }

}