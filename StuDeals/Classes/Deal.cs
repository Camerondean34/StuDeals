namespace StuDeals.Classes
{
    public class Deal
    {
        private string _ID;
        private string _Name;
        private string _Description;
        private string _Image;

        public Deal(string pID, string pName, string pDescription, string pImage)
        {
            _ID = pID;
            _Name = pName;
            _Description = pDescription;
            _Image = pImage;
        }


        public override string ToString()
        {
            return _Name;
        }
    }
}
