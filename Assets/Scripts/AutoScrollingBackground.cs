
using UnityEngine;


namespace RipsBigun
{
    [RequireComponent(typeof(Renderer))]
    public class AutoScrollingBackground : MonoBehaviour
    {
        [SerializeField]
        private float scrollSpeed;
        Renderer _renderer;

        // Start is called before the first frame update
        void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        // Update is called once per frame
        void Update()
        {
            float x = Mathf.Repeat(Time.time * scrollSpeed, 1);
            Vector2 offset = new Vector2(x, 0);
            _renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
    }
}