using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace StuDeals.Classes
{
    public class DataLayer
    {
        private static DataLayer? _Instance;
        private const string _Source = "Data\\SQLiteDB.db";
        private readonly string _ConnectionString;

        public static DataLayer Instance
        {
            get
            {
                _Instance ??= new DataLayer();
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
                getAllCommand.CommandText = $"SELECT * FROM {pTableName} {pCondition};";
                using (var reader = getAllCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] current = new string[reader.VisibleFieldCount];
                        for (int index = 0; index < reader.FieldCount; ++index)
                        {
                            string? data = reader.GetValue(index).ToString();
                            data ??= string.Empty;
                            current[index] = data;
                        }
                        result.Add(current);
                    }
                }
            }
            return result.ToArray();
        }

        private Venue? GenerateVenue(string[] pVenueFields)
        {
            if (pVenueFields.Length != 7) return null;
            int ID;
            float rating;
            bool suggestion;
            try
            {
                ID = int.Parse(pVenueFields[0]);
                rating = float.Parse(pVenueFields[5]);
                suggestion = bool.Parse(pVenueFields[6]);
            }
            catch
            {
                return null;
            }
            Venue result = new Venue(ID, pVenueFields[1], pVenueFields[2], pVenueFields[3], pVenueFields[4], rating, suggestion);
            string[][] tags = SelectAll("Tags, Venues_has_Tags", $"WHERE Tags.ID = Venues_has_Tags.Tags_ID AND Venues_has_Tags.Venues_ID = '{ID}'");
            foreach (string[] tag in tags) result.AddTag(tag[1]);
            return result;
        }

        public Venue[] GetVenues(bool pSuggestions = false)
        {
            string[][] venueFields = SelectAll("Venues", $"WHERE Suggestion = '{pSuggestions}'");
            List<Venue> result = new List<Venue>();
            for (int index = 0; index < venueFields.Length; ++index)
            {
                Venue? currentVenue = GenerateVenue(venueFields[index]);
                if (currentVenue != null) result.Add(currentVenue);
            }
            return result.ToArray();
        }

        public Venue[] GetVenuesWithTags(string pTag)
        {
            List<Venue> filteredList = new List<Venue>();
            Venue[] unfiliteredList = GetVenues();
            foreach (Venue venue in unfiliteredList)
            {
                foreach (string tag in venue.Tags)
                {
                    if (tag.ToLower() == pTag.ToLower())
                    {
                        filteredList.Add(venue);
                        break;
                    }
                }
            }
            return filteredList.ToArray();
        }

        public Venue[] GetTopFiveVenues()
        {
            List<Venue> unfilteredList = GetVenues().OrderBy(i => i.Rating).ToList();
            List<Venue> filteredList = new List<Venue>();
            for (int index = unfilteredList.Count - 1; index >= 0 && index > unfilteredList.Count - 6; --index)
            {
                filteredList.Add(unfilteredList[index]);
            }
            return filteredList.ToArray();
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

        private bool CheckExists(string pTableName, string pData, out int pID, string pField = "ID")
        {
            pID = -1;
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand CheckCommand = connection.CreateCommand();
                CheckCommand.CommandText = $"SELECT ID FROM '{pTableName}' WHERE {pField} = '{pData}';";
                object? objID = CheckCommand.ExecuteScalar();
                if (objID != null)
                {
                    string? strID = objID.ToString();
                    if (strID != null)
                    {
                        pID = int.Parse(strID);
                        return true;
                    }
                }
                return false;
            }
        }

        public void InsertVenue(Venue pVenue)
        {
            int currentID = GetAmount("Venues") + 1;
            Insert("'Venues' ( ID, Name, Description, Location, Image, Rating, Suggestion)", $"('{currentID}','{pVenue.Name}','{pVenue.Description}','{pVenue.Location}','{pVenue.ImageLink}', {pVenue.Rating}, '{pVenue.Suggestion}')");
        }

        public bool InsertDeal(Deal pDeal)
        {
            if (!CheckExists("Venues", pDeal.VenueID.ToString(), out _))
            {
                return false;
            }

            int currentDealID = GetAmount("Deals") + 1;
            Insert("'Deals' ( ID, Name, Description, Image)", $"('{currentDealID}','{pDeal.Name}','{pDeal.Description}','{pDeal.Image}')");
            Insert("'Venues_has_Deals' (Venues_ID, Deals_ID)", $"('{pDeal.VenueID}','{currentDealID}')");
            return true;
        }

        public Venue? SelectVenue(int pID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand getCommand = connection.CreateCommand();
                getCommand.CommandText = $"SELECT * FROM 'Venues' WHERE ID = '{pID}';";
                using (var reader = getCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] venueFields = new string[reader.VisibleFieldCount];
                        for (int index = 0; index < reader.FieldCount; ++index)
                        {
                            string? data = reader.GetValue(index).ToString();
                            data ??= string.Empty;
                            venueFields[index] = data;
                        }
                        return GenerateVenue(venueFields);
                    }
                }
            }
            return null;
        }

        public Account? GetAccount(string pUsername, string pPassword)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand getCommand = connection.CreateCommand();
                getCommand.CommandText = $"SELECT ID FROM User WHERE Username = '{pUsername}' AND Password = '{pPassword}';";
                int ID;
                try
                {
                    string? strID = getCommand.ExecuteScalar().ToString();
                    if (strID == null) return null;
                    ID = int.Parse(strID);
                }
                catch
                {
                    return null;
                }
                return GetAccount(ID);
            }
        }

        public Account? GetAccount(int pID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand getCommand = connection.CreateCommand();
                getCommand.CommandText = $"SELECT * FROM User WHERE ID = '{pID}';";
                using (var reader = getCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        try
                        {
                            string username = (string)reader.GetValue(1);
                            string name = (string)reader.GetValue(2);
                            string email = (string)reader.GetValue(3);
                            Account.AccountType type = Enum.Parse<Account.AccountType>((string)reader.GetValue(5), true);
                            return new Account(pID, username, name, email, type);
                        }
                        catch { }
                    }
                }
            }
            return null;
        }

        public void InserTag(string pTag, int pVenueID)
        {
            int tagID;
            if (!CheckExists("Tags", pTag, out tagID, "Name"))
            {
                tagID = GetAmount("Tags") + 1;
                Insert("'Tags' ( ID, Name)", $"('{tagID}','{pTag}')");
            }

            Insert("Venues_has_Tags ( Venues_ID, Tags_ID)", $"('{pVenueID}', '{tagID}')");
        }

        public void InsertUser(string pUsername, string pName, string pEmail, string pPassword, Account.AccountType pType)
        {
            int currentID = GetAmount("User") + 1;
            string type;
            if (pType == Account.AccountType.User) type = "USER";
            else type = "MOD";
            Insert("'User' ( ID, Username, Name, Email, Password, Occupation)", $"('{currentID}','{pUsername}','{pName}','{pEmail}','{pPassword}', '{type}')");
        }

        private bool Delete(string pTableName, int pID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand delCommand = connection.CreateCommand();
                delCommand.CommandText = $"DELETE FROM '{pTableName}' WHERE ID = '{pID}';";
                if (delCommand.ExecuteNonQuery() > 0) return true;
                return false;
            }
        }

        public bool DeleteVenue(int pID)
        {
            return Delete("Venues", pID);
        }

        public bool DeleteRating(int pID)
        {
            return Delete("Rating", pID);
        }

        public void AcceptSuggestion(int pID, string pDescription, string pImageLink)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = $"UPDATE 'Venues' SET 'Description' = '{pDescription}', 'Image' = '{pImageLink}', 'Suggestion' = '{false}' WHERE ID = '{pID}';";
                updateCommand.ExecuteNonQuery();
            }
        }

        public List<Rating> GetRatings(int pVenueID)
        {
            List<Rating> list = new List<Rating>();
            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand getCommand = connection.CreateCommand();
                getCommand.CommandText = $"SELECT * FROM 'Rating' WHERE Venues_ID = '{pVenueID}';";
                using (var reader = getCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            int ID = (int)reader["ID"];
                            string text = (string)reader["Text"];
                            int stars = int.Parse((string)reader["Stars"]);
                            int userID = int.Parse((string)reader["User_ID"]);
                            Account? user = GetAccount(userID);
                            if (user == null) continue;
                            list.Add(new Rating(ID, user, text, stars));
                        }
                        catch { }
                    }
                }
            }
            return list;
        }

        public void AddRating(int pVenueID, Account pAccount, string pText, int pStars)
        {
            int currentID = GetAmount("Rating") + 1;
            Insert("'Rating' ( ID, Venues_ID, User_ID, Text, Stars)", $"('{currentID}', '{pVenueID}', '{pAccount.ID}', '{pText}', '{pStars}')");
            List<Rating> list = GetRatings(pVenueID);
            float averageRating = 0;
            foreach (Rating rating in list) averageRating += rating.Stars;
            if (averageRating > 0 && list.Count > 0) averageRating /= list.Count;

            using (SQLiteConnection connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                SQLiteCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = $"UPDATE 'Venues' SET 'Rating' = '{(float)Math.Round(averageRating, 1)}' WHERE ID = '{pVenueID}';";
                updateCommand.ExecuteNonQuery();
            }
        }

    }
}
