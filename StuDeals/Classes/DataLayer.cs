using System.Data.SQLite;

namespace StuDeals.Classes
{
    public class DataLayer
    {
        private static DataLayer? _Instance;
        private const string _Source = "Data\\SQLiteDB.db";
        private string _ConnectionString;

        public static DataLayer Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DataLayer();
                }
                return _Instance;
            }
        }

        private DataLayer()
        {
            SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = _Source;
            _ConnectionString = connectionStringBuilder.ConnectionString;
        }

        private string[][] SelectAll(string pTableName, string pCondition = "")
        {
            List<string[]> result = new List<string[]>();
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand getAllCommand = connection.CreateCommand();
                getAllCommand.CommandText = $"SELECT * FROM '{pTableName}' {pCondition};";
                using (var reader = getAllCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] current = new string[reader.VisibleFieldCount];
                        for (int index = 0; index < reader.FieldCount; ++index)
                        {
                            string? data = reader.GetValue(index).ToString();
                            if (data == null) data = string.Empty;
                            current[index] = data;
                        }
                        result.Add(current);
                    }
                }
            }
            return result.ToArray();
        }

        public Venue[] GetVenues()
        {
            string[][] venueFields = SelectAll("Venues");
            Venue[] result = new Venue[venueFields.Length];
            for (int index = 0; index < venueFields.Length; ++index)
            {
                int ID = int.Parse(venueFields[index][0]);
                result[index] = new Venue(ID, venueFields[index][1], venueFields[index][2], venueFields[index][3], venueFields[index][4]);
                string[][] tags = SelectAll("Tags", $"WHERE 'ID' = '{ID}'");
                foreach (string[] tag in tags) result[index].AddTag(tag[0]);
            }
            return result;
        }

        public Deal[] GetDeals()
        {
            string[][] dealFields = SelectAll("Deals");
            Deal[] result = new Deal[dealFields.Length];
            for (int index = 0; index < dealFields.Length; ++index)
            {
                int venueID = int.Parse(SelectAll("Venues_has_Deals", $"WHERE Deals_ID = '{dealFields[index][0]}'")[0][0]);
                result[index] = new Deal(int.Parse(dealFields[index][0]), dealFields[index][1], dealFields[index][2], dealFields[index][3], venueID);
            }
            return result;
        }

        private void Insert(string pTableCols, string pValues)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand insertCommand = connection.CreateCommand();
                insertCommand.CommandText = $"INSERT INTO {pTableCols} VALUES {pValues};";
                insertCommand.ExecuteNonQuery();
            }
        }

        private int GetAmount(string pTableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand getAllCommand = connection.CreateCommand();
                getAllCommand.CommandText = $"SELECT COUNT('ID') FROM '{pTableName}';";
                string? value = getAllCommand.ExecuteScalar().ToString();
                if (string.IsNullOrEmpty(value)) return 0;
                return int.Parse(value);
            }
        }

        private bool CheckExists(string pTableName, int pID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand CheckCommand = connection.CreateCommand();
                CheckCommand.CommandText = $"SELECT * FROM '{pTableName}' WHERE ID = '{pID}';";
                if(CheckCommand.ExecuteScalar() == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void InsertVenue(Venue pVenue)
        {
            int currentID = GetAmount("Venues") + 1;
            Insert("'Venues' ( ID, Name, Description, Location, Image)", $"('{currentID}','{pVenue.Name}','{pVenue.Description}','{pVenue.Location}','{pVenue.Image}')");
        }

        public bool InsertDeal(Deal pDeal)
        {
            if(!CheckExists("Venues", pDeal.VenueID))
            {
                return false;
            }

            int currentDealID = GetAmount("Deals") + 1;
            Insert("'Deals' ( ID, Name, Description, Image)", $"('{currentDealID}','{pDeal.Name}','{pDeal.Description}','{pDeal.Image}')");
            Insert("'Venues_has_Deals' (Venues_ID, Deals_ID)", $"('{pDeal.VenueID}','{currentDealID}')");
            return true;
        }
    }
}
