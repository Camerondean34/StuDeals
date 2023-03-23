using System.Data.SQLite;

namespace StuDeals.Classes
{
    public class DataLayer
    {
        private static DataLayer? _Instance;
        private const string _Source = "Data\\Employees.db";
        private SQLiteConnectionStringBuilder connectionStringBuilder;

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
            connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = _Source;
        }

        private string[][] SelectAll(string pTableName, string pCondition = "")
        {
            List<string[]> result = new List<string[]>();
            using (SQLiteConnection connection = new SQLiteConnection())
            {
                connection.Open();
                SQLiteCommand getVenuesCommand = connection.CreateCommand();
                getVenuesCommand.CommandText = $"SELECT * FROM '{pTableName}' {pCondition};";
                using (var reader = getVenuesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] current = new string[reader.VisibleFieldCount];
                        for (int index = 0; index < reader.FieldCount; ++index)
                        {
                            current[index] = (string)reader.GetValue(index);
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
                result[index] = new Deal(int.Parse(dealFields[index][0]), dealFields[index][1], dealFields[index][2], dealFields[index][3]);
            }
            return result;
        }

        private void Insert(string pTableCols, string pValues)
        {
            using (SQLiteConnection connection = new SQLiteConnection())
            {
                connection.Open();
                SQLiteCommand getVenuesCommand = connection.CreateCommand();
                getVenuesCommand.CommandText = $"INSERT INTO {pTableCols} VALUES {pValues};";
                getVenuesCommand.ExecuteNonQuery();
            }
        }

        public void InsertVenue(Venue venue)
        {
            Insert("'Venues'", $"({""},{""},{""},{""},{""})");
            //TODO create properties in Venue class
        }
    }
}
