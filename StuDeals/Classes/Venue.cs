namespace StuDeals.Classes
{
    public class Venue
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }
        public string Image { get; private set; }
        public List<string> Tags { get; private set; }

        public Venue(int pID, string pName, string pDescription, string pLocation, string pImage)
        {
            ID = pID;
            Name = pName;
            Description = pDescription;
            Location = pLocation;
            Image = pImage;
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
