using UnityEngine;

public class GameController : MonoBehaviour
{

    private int player_points = 0;

    struct ranks
    {
        
    }


    void Start()
    {

    }

    void Update()
    {

    }


    public void addPlayerPoints(int points)
    {
        player_points += points;

        checkPlayerRank();
    }

    private void checkPlayerRank()
    {
        
    }

    public int getPlayerPoints()
    {
        return player_points;
    }
}
