using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class RacesDictionary
    {
        public static Dictionary<string, List<string>> RacesAndSubraces = new Dictionary<string, List<string>>
        {
            {"Humano",new List<string>{"Greenlander", "Scorchlander" } },
            {"Hive" , new List<string>{"Prince","Soldier", "Worker" } }
        };
    }
}
