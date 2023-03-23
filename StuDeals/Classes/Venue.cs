namespace StuDeals.Classes
{
    public class Venue
    {
        private string _ID;
        private string _Name;
        private string _Description;
        private string _Location;
        private string _Image;
        private List<string> _Tags;

        public Venue(string pID, string pName, string pDescription, string pLocation, string pImage)
        {
            _ID = pID;
            _Name = pName;
            _Description = pDescription;
            _Location = pLocation;
            _Image = pImage;
            _Tags = new List<string>();
        }

        public void AddTag(string pTag)
        {
            _Tags.Add(pTag);
        }

        public override string ToString()
        {
            return _Name;
        }
    }
}
