using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class GameController_Davi : MonoBehaviour
{

    public int player_points = 0;

    private double player_katana_base_dmg = 50;
    private double player_pistol_base_dmg = 35;  
    private double damage_multiplier = 1; 

    private double player_katana_dmg;  
    private double player_pistol_dmg;


    public PlayerMovement_Davi player;
    public GameObject S;
    public GameObject A;
    public RankLetterFill_Davi A_claro;
    public GameObject B;
    public RankLetterFill_Davi B_claro;
    public GameObject C;
    public RankLetterFill_Davi C_claro;
    public GameObject D;
    public RankLetterFill_Davi D_claro;
    public GameObject E;
    public RankLetterFill_Davi E_claro;

    void Start()
    {
        player_katana_dmg = damage_multiplier * player_katana_base_dmg;
        player_pistol_dmg = damage_multiplier * player_pistol_base_dmg;

        E.SetActive(true);

        InvokeRepeating("losePointsWithTime", 5f, 2f); // Player perde pontos a cada 5 segundos, quando fora de combate (TODO!)
    }

    public void losePointsWithTime()
    {
        addPlayerPoints(-5);
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
        // Desativa todas as UI dos ranks, para ativar uma única dentro do switch
        S.SetActive(false);
        A.SetActive(false);
        B.SetActive(false);
        C.SetActive(false);
        D.SetActive(false);
        E.SetActive(false);

        // Calcula a porcentagem de completude
        
        switch (player_points)
        {
            case >= 560:  //RANK S
                applyRankBonus('S');
                player.setPlayerSpeedMultiplier(1.4f);
                S.SetActive(true);
                break;
            case >= 430:  //RANK A
                applyRankBonus('A');
                player.setPlayerSpeedMultiplier(1.2f);
                A.SetActive(true);
                A_claro.setFillAmount(560f, 430f, player_points);
                break;
            case >= 310:  //RANK B
                applyRankBonus('B');
                player.setPlayerSpeedMultiplier(1f);
                B.SetActive(true);
                B_claro.setFillAmount(430f, 310f, player_points);
                break;
            case >= 220:  //RANK C
                applyRankBonus('C');
                player.setPlayerSpeedMultiplier(1f);
                C.SetActive(true);
                C_claro.setFillAmount(310f, 220f, player_points);
                break;
            case >= 90:   //RANK D
                applyRankBonus('D');
                player.setPlayerSpeedMultiplier(1f);
                D.SetActive(true);
                D_claro.setFillAmount(220f, 90f, player_points);
                break;
            default:      //RANK E
                applyRankBonus('E');
                player.setPlayerSpeedMultiplier(1f);
                E.SetActive(true);
                E_claro.setFillAmount(90f, 0f, player_points);
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

    public int getPlayerPoints()
    {
        return player_points;
    }
}
