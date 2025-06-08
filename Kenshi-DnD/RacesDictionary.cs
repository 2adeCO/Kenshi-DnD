namespace Kenshi_DnD
{
    // Has each race's subraces in a string, used to apply correct subraces to random heroes. - Santiago Cabrero
    class RacesDictionary
    {
        public static Dictionary<string, List<string>> RacesAndSubraces = new Dictionary<string, List<string>>
        {
            {"Humano",new List<string>{"Greenlander", "Scorchlander" } },
            {"Enjambre" , new List<string>{"Príncipe","Dron Soldado", "Dron Trabajador" } },
            {"Shek" , new List<string>{"Puro"} },
            {"Esqueleto" , new List<string>{"Puro" } }
        };
    }
}
