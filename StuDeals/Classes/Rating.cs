using static StuDeals.Classes.Account;

namespace StuDeals.Classes
{
    public class Rating
    {
        public int ID { get; private set; }
        public Account Account { get; private set; }
        public string Text { get; private set; }
        public int Stars { get; private set; }

        public Rating(int pID, Account pUser, string pText, int pStars)
        {
            ID = pID;
            Account = pUser;
            Text = pText;
            Stars = pStars;
        }

    }
}
