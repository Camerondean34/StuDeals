namespace StuDeals.Classes
{
    public class Account
    {
        public static Account? CurrentAccount { get; private set; }

        public static bool LogIn(string pUsername, string pPassword)
        {
            if (CurrentAccount != null) return false;

            CurrentAccount = DataLayer.Instance.GetAccount(pUsername, pPassword);
            if (CurrentAccount == null) return false;
            return true;
        }

        public static void LogOut()
        {
            CurrentAccount = null;
        }

        public enum AccountType { User, Mod }
        public int ID { get; private set; }
        public string Username { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public AccountType Type { get; private set; }

        public Account(string pUsername, string pName, string pEmail, AccountType pType) : this(0, pUsername, pName, pEmail, pType) { }

        public Account(int pID, string pUsername, string pName, string pEmail, AccountType pType)
        {
            ID = pID;
            Username = pUsername;
            Name = pName;
            Email = pEmail;
            Type = pType;
        }

    }
}
