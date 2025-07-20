using UnityEngine;
using System.Collections.Generic;

public class EnemyProximity : MonoBehaviour
{
    public GameController game_controller;
    public PlayerHealth player_health;

    [Tooltip("Layer dos inimigos")]
    public LayerMask enemyLayer;

    private HashSet<GameObject> enemiesInRange = new HashSet<GameObject>();
    private bool isInCombat = false;

    private void OnTriggerEnter(Collider other)
    {

        if (IsInEnemyLayer(other.gameObject))
        {
            enemiesInRange.Add(other.gameObject);

            if (!isInCombat)
            {
                isInCombat = true;
                game_controller.setParametroEmCombate(true);
                player_health.setInCombat(true);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (IsInEnemyLayer(other.gameObject))
        {
            enemiesInRange.Remove(other.gameObject);

            if (enemiesInRange.Count == 0 && isInCombat)
            {
                isInCombat = false;
                game_controller.setParametroEmCombate(false);
                player_health.setInCombat(false);
                player_health.StartRegeneration();
            }
        }
    }


    private bool IsInEnemyLayer(GameObject obj)
    {
        return ((1 << obj.layer) & enemyLayer) != 0;
    }
}
