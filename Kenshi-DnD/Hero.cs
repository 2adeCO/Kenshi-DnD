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
        //Skill is associated with Humans, Hive Princes and Hive Workers 
        int bruteForce;
        int skill;
        //Defensive stats
        //Health points
        //Resistance is defense against Brute Force
        //Perception is defense against Skill
        int hp;
        int resistance;
        int perception;

    }
}
