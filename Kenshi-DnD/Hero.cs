using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Hero
    {
        //Name and title, example: "Carlos, the database scholar"
        string name;
        string title;

        //Offensive stats
        //Brute Force is associated with Sheks, skeletons and Hive Soldiers
        //Skill is associated with Humans(All types), Hive Princes and Hive Workers 
        int bruteForce;
        int skill;

        //Defensive stats
        //Health points
        int hp;
        //Resistance is defense against Brute Force
        //Perception is defense against Skill
        int resistance;
        int perception;
        //Race is a key component in the world building of this game. The races are: Human, Shek, Skeleton and Hive

        //Subrace specifies the exact type of race that the character is. The subraces are:
        //Human: Greenlander, scorchlander
        //Hive: Hive Prince, Hive Worker, Hive Soldier
        //The other races don't have subrace

        //Inventory

        //Background story is a semi randomly asigned tale to give the hero a bit of depth. Depending on their circunstances, these will vary.
        //For example: If a Shek is a rebel, their background could tell how they rejected Esata the Stone Golem(Shek Kingdom's Queen) for her untraditionalist ways.
        //If a human is a merchant noble, their background could tell how they inherited their father's slave farm.
        string backgroundStory;

        //Recruitment date is the date when the hero was recruited. This is important for the game, as it will determine how long the hero has been in the party.
        DateTime recruitment;
        //Level determines roughly how strong the hero is. However, this is not a direct correlation, as the hero's stats are built upon leveling up.
        int level;
        //Experience is the amount of experience the hero has. It determines how much the hero has leveled up.
        int experience;

    }
}
