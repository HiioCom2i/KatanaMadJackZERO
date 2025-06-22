using UnityEngine;

public class GameController : MonoBehaviour
{

    public int player_points = 0;
    private double player_HP = 200;


    private double player_katana_base_dmg = 50;
    private double player_pistol_base_dmg = 35;  
    private double damage_multiplier = 1; 

    private double player_katana_dmg;  
    private double player_pistol_dmg;



    void Start()
    {
        player_katana_dmg = damage_multiplier * player_katana_base_dmg;
        player_pistol_dmg = damage_multiplier * player_pistol_base_dmg;

        InvokeRepeating("losePointsWithTime", 5f, 5f); // Player perde pontos a cada 5 segundos, quando fora de combate (TODO!)
        InvokeRepeating("playerHPRegen", 5f, 5f); // Player ganha vida a cada 5 segundos, quando fora de combate (TODO!)
    }

    public void losePointsWithTime()
    {
        addPlayerPoints(-5);
    }

    public void playerHPRegen()
    {
        if (player_HP < 200)
        {
            player_HP += 5;
        }
    }

    void Update()
    {

    }


    public void addPlayerPoints(int points)
    {
        player_points += points;

        if (player_points < 0){ player_points = 0; }

        checkPlayerRank();
    }

    private void checkPlayerRank()
    {
        switch (player_points)
        {
            case >= 560:  //RANK S
                applyRankBonus('S');
                break;
            case >= 430:  //RANK A
                applyRankBonus('A');
                break;
            case >= 310:  //RANK B
                applyRankBonus('B');
                break;
            case >= 220:  //RANK C
                applyRankBonus('C');
                break;
            case >= 90:   //RANK D
                applyRankBonus('D');
                break;
            default:      //RANK E
                applyRankBonus('E');
                break;

        }
    }

    private void applyRankBonus(char rank)
    {
        switch (rank)
        {
            case 'S':
                damage_multiplier = 2;
                break;
            case 'A':
                damage_multiplier = 1.8;
                break;
            case 'B':
                damage_multiplier = 1.6;
                break;
            case 'C':
                damage_multiplier = 1.4;
                break;
            case 'D':
                damage_multiplier = 1.2;
                break;
            default:
                damage_multiplier = 1;
                break;
        }

        player_katana_dmg = damage_multiplier * player_katana_base_dmg;  
        player_pistol_dmg = damage_multiplier * player_pistol_base_dmg; 
    }

    private void playerTakesDamage(double damage)
    {
        player_HP -= damage;
        addPlayerPoints(-60); // Player perde 60 pontos ao tomar dano
    }

    public int getPlayerPoints()
    {
        return player_points;
    }
}
