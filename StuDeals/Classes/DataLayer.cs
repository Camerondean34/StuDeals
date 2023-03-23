using System.Data.SQLite;

namespace StuDeals.Classes
{
    public class DataLayer
    {
        private static DataLayer? _Instance;
        private const string _Source = @"Data\\Employees.db";
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

        DataLayer()
        {
            connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = _Source;
        }

        public static Venue[] GetVenues()
        {
            List<Venue> result = new List<Venue>();
            using (SQLiteConnection connection = new SQLiteConnection())
            {
                connection.Open();
                SQLiteCommand getVenuesCommand = connection.CreateCommand();
                getVenuesCommand.CommandText = "SELECT * FROM 'Venues';";
                using (var reader = getVenuesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Venue(int.Parse((string)reader[0]), (string)reader[1], (string)reader[2], (string)reader[3], (string)reader[4]));
                    }
                }
            }
            return result.ToArray();
        }
    }
}
