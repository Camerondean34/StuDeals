namespace StuDeals.Classes
{
    public class Venue
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }
        public string ImageLink { get; private set; }
        public float Rating { get; private set; }
        public bool Suggestion { get; private set; }
        public List<string> Tags { get; private set; }

        public Venue(string pName, string pDescription, string pLocation, string pImage, float pRating, bool pSuggestion) : this(0, pName, pDescription, pLocation, pImage, pRating, pSuggestion) { }

        public Venue(int pID, string pName, string pDescription, string pLocation, string pImage, float pRating, bool pSuggestion)
        {
            ID = pID;
            Name = pName;
            Description = pDescription;
            Location = pLocation;
            ImageLink = pImage;
            Rating = pRating;
            Suggestion = pSuggestion;
            Tags = new List<string>();
        }

        public void AddTag(string pTag)
        {
            Tags.Add(pTag);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
