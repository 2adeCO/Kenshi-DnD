namespace Kenshi_DnD
{
    // Custom exception which will cause the program to close due to not having data to play it. - Santiago Cabrero
    class XMLNotFoundException : Exception
    {
        public XMLNotFoundException() : base("XML no encontrado, faltan datos para jugar al juego.")
        {
        }
    }
}
