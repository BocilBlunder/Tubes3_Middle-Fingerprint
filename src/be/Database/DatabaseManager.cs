using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Tubes3_Middle_Fingerprint.Database;

public class DatabaseManager
{

    byte[] key = new byte[32] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
    byte[] nonce = new byte[8] { 0, 1, 2, 3, 4, 5, 6, 7 };


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
                        if (reader.GetName(i).Equals("NIK"))
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(reader[i].ToString());
                            Salsa20.s20_crypt(key, nonce, 0, buffer);

                            biodata.Add(reader.GetName(i) + ": " + Encoding.UTF8.GetString(buffer));
                        }
                        else
                        {
                            biodata.Add(reader.GetName(i) + ": " + reader[i].ToString());
                        }
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
