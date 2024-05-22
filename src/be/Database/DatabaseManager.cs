using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;

public class DatabaseManager
{
    private string connectionString;

    public DatabaseManager(string server, string database, string userId, string password)
    {
        connectionString = $"server={server};user={userId};password={password};database={database}";
    }

    public List<string> getAllAlayNames()
    {
        List<string> alayname = new List<string>();
        string query = "SELECT nama FROM biodata";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    alayname.Add(reader["nama"].ToString());
                }
            }
            conn.Close();
        }
        return alayname;
    }

    public List<string> getOrisinilNames(string imagePath)
    {
        List<string> orisinilnames = new List<string>();
        string query = "SELECT nama FROM sidik_jari WHERE berkas_citra = @Image";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Image", imagePath); 

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    orisinilnames.Add(reader["nama"].ToString());
                }
            }
            conn.Close();
        }
        return orisinilnames;
    }


public List<FingerprintOwner> getImageFromDB()
{
    List<FingerprintOwner> FingerprintOwners = new List<FingerprintOwner>();
    string query = "SELECT nama, berkas_citra FROM sidik_jari";

    using (MySqlConnection conn = new MySqlConnection(connectionString))
    {
        conn.Open();
        MySqlCommand cmd = new MySqlCommand(query, conn);

        // Find the directory that includes the '/src' subpath starting from the current directory
        string rootDirectoryContainingSrc = DirectoryUtils.FindDirectoryContainingSubpath(Directory.GetCurrentDirectory(), "src");

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                // Construct the image path relative to the found directory
                string imageData = Path.Combine(rootDirectoryContainingSrc, (string)reader["berkas_citra"]);
                Bitmap bmp = new Bitmap(imageData);
                FingerprintOwner Fingerprint = new FingerprintOwner(bmp, imageData, (string)reader["nama"]);
                FingerprintOwners.Add(Fingerprint);
            }
        }
        conn.Close();
    }
    return FingerprintOwners;
}


    public List<string> getBiodata(string alayname)
    {
        List<string> biodata = new List<string>();
        string query = "SELECT * FROM biodata WHERE nama = @Name";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open(); 

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", alayname);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        biodata.Add(reader.GetName(i) + ": " + reader[i].ToString());
                    }
                }
            }
            conn.Close();
        }
        return biodata;
    }

    // static void Main(string[] args)
    // {
    //     DatabaseManager dbOps = new DatabaseManager("localhost", "yourDatabase", "root", "yourPassword");
    //     List<string> names = dbOps.getAllAlayNames();

    //     foreach (var name in names)
    //     {
    //         Console.WriteLine(name);
    //     }
    // }
}


public class DirectoryUtils
{
    public static string FindDirectoryContainingSubpath(string startingPath, string subpathToFind)
    {
        DirectoryInfo currentDirectory = new DirectoryInfo(startingPath);

        // Traverse up the directory tree and check each directory
        while (currentDirectory != null)
        {
            // Check if the current directory contains the subpath
            if (Directory.Exists(Path.Combine(currentDirectory.FullName, subpathToFind)))
            {
                return currentDirectory.FullName;
            }

            // Move to the parent directory
            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("No directory containing the specified subpath was found in the directory tree.");
    }
}
