namespace StuDeals.Classes
{
    public class Deal
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Image { get; private set; }

        public Deal(int pID, string pName, string pDescription, string pImage)
        {
            ID = pID;
            Name = pName;
            Description = pDescription;
            Image = pImage;
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
