using UnityEngine;

namespace RipsBigun
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        FloatVariable _score;

        // controllers/managers
        PlayerController _player;

        // events
        PlayerEvent _onPlayerDeath;
        PlayerEvent _onPlayerHit;
        EnemyEvent _onEnemyDeath;
    }
}