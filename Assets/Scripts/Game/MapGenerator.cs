using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int[,] map;
    public int[,] redBarrelXY;

    void Start()
    {
        // Seules coordonnées où peuvent apparaître désormais les Red Barrel.
        redBarrelXY = new int[,]
        { { 6, 6 }, { 3, 6 }, { 0, 6 }, { 6, 0 }, { 6, 12 }, { 3, 0 }, { 3, 12 }, { 0, 0 }, { 0, 12 } };
    }

    public void InitialiseMap()
    {
        map = new int[,]
        {
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
        };
    }

    void PlaceEnemyBlocs012(int who, int lvl, int[] voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber, forIValue = 0, forJLimite = 0, jOffset = 0;

        switch(who)
        {
            case 0:
                forIValue = 7;
                forJLimite = 3;
                break;
            case 1:
                forIValue = 7;
                forJLimite = 3;
                jOffset = 3;
                break;
            case 2:
                forIValue = 4;
                forJLimite = 5;
                break;
        }

        for (int i = forIValue; i < forIValue + lvl; i++)
        {
            do
            {
                atLeastOneEnemy = i != forIValue;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < forJLimite; j++)
                {
                    draw = i == (forIValue + 1) ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                    switch (draw)
                    {
                        case 0:
                            map[i, j + jOffset] = map[i, 12 - j - jOffset] = 0;
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j + jOffset] = map[i, 12 - j - jOffset] = 1;
                            atLeastOneEnemy = true;
                            atLeastOne = true;
                            break;
                        default:
                            map[i, j + jOffset] = map[i, 12 - j - jOffset] = 9;
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal[i - forIValue])));
        }
    }

    void PlaceEnemyBlocs23Middle(int who, int lvl, int[] voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber, forIValue = 0, forJLimite = 0, maxLevel = 0, jOffsetLeft = 0, jOffsetRight, lastIvalue;

        if (who == 2)       {forIValue = 5; forJLimite = 2; maxLevel = 2; jOffsetLeft = 5;}
        else if (who == 3)  {forIValue = 2; forJLimite = 3; maxLevel = 3; jOffsetLeft = 4;}
        lastIvalue = forIValue - (maxLevel - 1);
        jOffsetRight = jOffsetLeft + (forJLimite-1) * 2;

        for (int i = forIValue; i > forIValue - lvl; i--)
        {
            do
            {
                atLeastOneEnemy = i != forIValue;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < forJLimite; j++)
                {
                    draw = i == lastIvalue ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                    switch (draw)
                    {
                        case 0:
                            map[i, j + jOffsetLeft] = i == lastIvalue
                                ? j < forJLimite - 1
                                    ? (map[i + maxLevel, j + jOffsetLeft] = map[i, jOffsetRight - j] = map[i + maxLevel, jOffsetRight - j] = 0)
                                    : (map[i + maxLevel, j + jOffsetLeft] = 0)
                                : j < forJLimite - 1 ? (map[i, jOffsetRight - j] = 0) : 0;
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j + jOffsetLeft] = j < forJLimite - 1 ? (map[i, jOffsetRight - j] = 1) : 1;
                            atLeastOne = true;
                            atLeastOneEnemy = true;
                            break;
                        default:
                            map[i, j + jOffsetLeft] = i == lastIvalue
                                ? j < forJLimite - 1
                                    ? (map[i + maxLevel, j + jOffsetLeft] = map[i, jOffsetRight - j] = map[i + maxLevel, jOffsetRight - j] = 9)
                                    : (map[i + maxLevel, j + jOffsetLeft] = 9)
                                : j < forJLimite - 1 ? (map[i, jOffsetRight - j] = 9) : 9;
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal[-i + forIValue])));
        }
    }

    void PlaceEnemyBlocs3LeftRight(int lvl, int[] voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber;

        for (int i = 2; i > 2 - lvl; i--)
        {
            do
            {
                atLeastOneEnemy = i != 2;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < 4; j++)
                {
                    draw = i == 0 ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                    switch (draw)
                    {
                        case 0:
                            map[i, j] = i == 0 ? (map[i, 12 - j] = map[i + 3, j] = map[i + 3, 12 - j] = 0) : (map[i, 12 - j] = 0);
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j] = map[i, 12 - j] = 1;
                            atLeastOne = true;
                            atLeastOneEnemy = true;
                            break;
                        default:
                            map[i, j] = i == 0 ? (map[i, 12 - j] = map[i + 3, j] = map[i + 3, 12 - j] = 9) : (map[i, 12 - j] = 9);
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal[-i + 2])));
        }
    }

    void PlacePlayerCover(int coverLevelEarn) // coverLevelEarn ne doit pas dépasser 3.
    {
        bool atLeastOne;

        for (int i = 12; i > 12 - coverLevelEarn; i--)
        {
            do
            {
                atLeastOne = false;

                for (int j = 0; j < 3; j++)
                {
                    map[i, j + 1] = Random.Range(0, 2) == 0 ? (map[i, 11 - j] = 0) : (map[i, 11 - j] = 9);
                    atLeastOne |= map[i, j + 1] == 0;
                }
            }
            while (!(atLeastOne));

            do
            {
                atLeastOne = false;

                map[i, 5] = Random.Range(0, 2) == 0 ? (map[i, 7] = 0) : (map[i, 7] = 9);
                map[i, 6] = Random.Range(0, 2) == 0 ? 0 : 9;
                atLeastOne |= map[i, 5] == 0 | map[i, 6] == 0;

            }
            while (!(atLeastOne));
        }
    }

    public void PlaceRedBarrel(int barrelEarn) // barrelEarn ne doit pas dépasser 10.
    {
        bool barrelAlready;
        int index;

        for (int i = 0; i < barrelEarn; i++)
        {
            do
            {
                index = IndexOfRedBarrelPositionDraw();
                if (map[redBarrelXY[index, 0], redBarrelXY[index, 1]] != 5)
                {
                    map[redBarrelXY[index, 0], redBarrelXY[index, 1]] = 5;
                    barrelAlready = false;
                }
                else
                {
                    barrelAlready = true;
                }
            }
            while (barrelAlready) ;
    }
    }

    public int IndexOfRedBarrelPositionDraw()
    {
        int start, end, result, draw;
        int[] chance = { 1, 3, 18, 28, 50 }; // Le total doit faire 100.

        end = 0;
        draw = Random.Range(0, 100);

        for (int i = 0; i < 5; i++)
        {
            start = i == 0 ? -1 : end - 1;
            end = 0;
            for (int j = 0; j < i + 1; j++)
            {
                end += chance[j];
            }
            if (draw > start && draw < end)
            {
                switch (i)
                {
                    case 0:
                        return result = i;
                    case 1:
                        return result = i;
                    case 2:
                        return result = Random.Range(2, 5);
                    case 3:
                        return result = Random.Range(5, 7);
                    case 4:
                        return result = Random.Range(7, 9);
                }
            }
        }
        return 0;
    }

    public int[] VoidNumberGoalNormalized(int draw, int[] voidNumberGoal)
    {
        for (int j = 0; j < 3; j++)
        {
            if (draw == 0 | draw == 1 | draw == 5)
            {
                if (voidNumberGoal[j] == 4 | voidNumberGoal[j] == 3)
                {
                    voidNumberGoal[j] = 2;
                }
                else if (voidNumberGoal[j] == 2 | voidNumberGoal[j] == 1)
                {
                    voidNumberGoal[j] = 1;
                }
            }
            else if (draw == 3)
            {
                if (voidNumberGoal[j] != 0)
                {
                    voidNumberGoal[j] = 1;
                }
            }
            else if (draw == 4)
            {
                if (voidNumberGoal[j] == 4 | voidNumberGoal[j] == 3)
                {
                    voidNumberGoal[j] = 3;
                }
            }
        }
        return voidNumberGoal;
    }

    public int levelNormalized(int level)
    {
        return level switch
        {
            2 => 1,
            3 => 2,
            _ => level
        };
    }

        public int[,] GenerateMap(int barrelEarn, int coverEarn, int[,] blocsLevel)
    {
        bool[] unavailable = new bool[6];
        int draw;
        int[] voidNumberGoal;

        InitialiseMap();

        for (int i = 0; i < 6; i++)
        {
            voidNumberGoal = new int[] { blocsLevel[i,1], blocsLevel[i,2], blocsLevel[i,3] };
            if (blocsLevel[i, 0] != 0)
            {
                do
                {
                    draw = Random.Range(0, 6);
                }
                while (unavailable[draw]);

                switch (draw)
                {
                    case 0:
                        PlaceEnemyBlocs012(0, blocsLevel[i, 0], VoidNumberGoalNormalized(draw, voidNumberGoal));
                        unavailable[draw] = true;
                        break;
                    case 1:
                        PlaceEnemyBlocs012(1, blocsLevel[i, 0], VoidNumberGoalNormalized(draw, voidNumberGoal));
                        unavailable[draw] = true;
                        break;
                    case 2:
                        PlaceEnemyBlocs012(2, blocsLevel[i, 0], voidNumberGoal);
                        unavailable[draw] = true;
                        break;
                    case 3:
                        PlaceEnemyBlocs23Middle(2, levelNormalized(blocsLevel[i, 0]), VoidNumberGoalNormalized(draw, voidNumberGoal));
                        unavailable[draw] = true;
                        break;
                    case 4:
                        PlaceEnemyBlocs3LeftRight(blocsLevel[i, 0], VoidNumberGoalNormalized(draw, voidNumberGoal));
                        unavailable[draw] = true;
                        break;
                    case 5:
                        PlaceEnemyBlocs23Middle(3, blocsLevel[i, 0], VoidNumberGoalNormalized(draw, voidNumberGoal));
                        unavailable[draw] = true;
                        break;
                }
            }
        }
        PlacePlayerCover(coverEarn);
        PlaceRedBarrel(barrelEarn);

        return map;
    }
}
