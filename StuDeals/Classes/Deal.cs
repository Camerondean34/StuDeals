namespace StuDeals.Classes
{
    public class Deal
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Image { get; private set; }
        public int VenueID { get; private set; }


        public Deal(string pName, string pDescription, string pImage, int pVenueID) : this(0, pName, pDescription, pImage, pVenueID) { }

        public Deal(int pID, string pName, string pDescription, string pImage, int pVenueID)
        {
            ID = pID;
            Name = pName;
            Description = pDescription;
            Image = pImage;
            VenueID = pVenueID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}