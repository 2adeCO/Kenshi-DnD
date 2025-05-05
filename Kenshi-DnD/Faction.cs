using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Kenshi_DnD
{
    class Faction
    {
        int factionId;
        string factionName;
        string factionDescription;
        int factionColor;
        BitmapImage factionImage;

        public Faction(int factionId, string factionName, string factionDescription, int factionColor) 
        { 
            this.factionId = factionId;
            this.factionName = factionName;
            this.factionDescription = factionDescription;
            this.factionColor = factionColor;
            this.factionImage = new BitmapImage();
        }
        public Faction(int factionId, string factionName, string factionDescription, string factionImageSource)
        {
            this.factionId = factionId;
            this.factionName = factionName;
            this.factionDescription = factionDescription;
            this.factionImage = new BitmapImage();
            factionImage.UriSource = new Uri(factionImageSource, UriKind.RelativeOrAbsolute);
        }

        public void SetFactionId(int factionId)
        {
            this.factionId = factionId;
        }
        public int GetFactionId()
        {
            return factionId;
        }
        public void SetFactionName(string factionName)
        {
            this.factionName = factionName;
        }
        public string GetFactionName()
        {
            return "@" + factionColor + "@" + factionName +"@";
        }
        public void SetFactionDescription(string factionDescription)
        {
            this.factionDescription = factionDescription;
        }
        public string GetFactionDescription()
        {
            return factionDescription;
        }
        public void SetFactionImage(string factionImageSource)
        {
            this.factionImage = new BitmapImage();
            factionImage.UriSource = new Uri(factionImageSource, UriKind.RelativeOrAbsolute);
        }
        public void SetFactionColor(int factionColor)
        {
            this.factionColor = factionColor;
        }
        public int GetFactionColor()
        {
            return this.factionColor;
        }
        public BitmapImage GetFactionImage()
        {
            return factionImage;
        }
    }
}
