using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Text;
using Tubes3_Middle_Fingerprint.Database;
using System.Configuration;
using ZstdSharp.Unsafe;

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
        List<string> alayNames = new List<string>();
        string query = "SELECT nama FROM encryptedbiodata";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(query, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Assuming the 'nama' column is stored in encrypted byte array form
                    byte[] encryptedName = (byte[])reader["nama"];
                    string decryptedName = decrypt(encryptedName);
                    alayNames.Add(decryptedName);
                }
            }
            conn.Close();
        }
        return alayNames;
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
        string query = "SELECT * FROM encryptedbiodata WHERE nama = @Name";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open(); 

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", encrypt(alayname));

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i).Equals("NIK")||
                            reader.GetName(i).Equals("nama") ||
                            reader.GetName(i).Equals("tempat_lahir") ||
                            reader.GetName(i).Equals("golongan_darah") ||
                            reader.GetName(i).Equals("alamat") ||
                            reader.GetName(i).Equals("agama") ||
                            reader.GetName(i).Equals("pekerjaan") ||
                            reader.GetName(i).Equals("kewarganegaraan"))
                        {
                            biodata.Add(reader.GetName(i) + ": " + decrypt((byte[])reader[i]));
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
    private byte[] encrypt(string value)
    {
        byte[] unecryptedByte = Encoding.UTF8.GetBytes(value);
        Salsa20.s20_crypt(key, nonce, 0, unecryptedByte);
        return unecryptedByte;
    }

    private string decrypt(byte[] value)
    {
        Salsa20.s20_crypt(key, nonce, 0, value);
        return Encoding.UTF8.GetString(value);
    }

    public void encryptDB(string query)
    {

        List<byte[][]> listOfStringArrays = new List<byte[][]>();


        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        byte[][] stringArray = new byte[][] {
                            encrypt(reader[0].ToString()),
                            encrypt(reader[1].ToString()),
                            encrypt(reader[2].ToString()),
                            Encoding.UTF8.GetBytes(reader[3].ToString()),
                            Encoding.UTF8.GetBytes(reader[4].ToString()),
                            encrypt(reader[5].ToString()),
                            encrypt(reader[6].ToString()),
                            encrypt(reader[7].ToString()),
                            Encoding.UTF8.GetBytes(reader[8].ToString()),
                            encrypt(reader[9].ToString()),
                            encrypt(reader[10].ToString())
                        };
                        listOfStringArrays.Add(stringArray);
                    }
                    
                    
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error connecting to MySQL database:", ex.Message);
            }
        }
        using (MySqlConnection conn2 = new MySqlConnection(connectionString))
        {
            conn2.Open();
            foreach (byte[][] arr in listOfStringArrays)
            {
                string insertQuery = "insert into encryptedbiodata values (@NIK, @NAMA, @TEMPAT_LAHIR, STR_TO_DATE(@TANGGAL_LAHIR, '%m/%d/%Y %h:%i:%s %p'), @JENIS_KELAMIN, @GOLONGAN_DARAH, @ALAMAT, @AGAMA, @STATUSKWN, @JOB, @KWN)";
                MySqlCommand cmd2 = new MySqlCommand(insertQuery, conn2);

                cmd2.Parameters.AddWithValue("@NIK", arr[0]);
                cmd2.Parameters.AddWithValue("@NAMA", arr[1]);
                cmd2.Parameters.AddWithValue("@TEMPAT_LAHIR", arr[2]);
                cmd2.Parameters.AddWithValue("@TANGGAL_LAHIR", arr[3]);
                cmd2.Parameters.AddWithValue("@JENIS_KELAMIN", arr[4]);
                cmd2.Parameters.AddWithValue("@GOLONGAN_DARAH", arr[5]);
                cmd2.Parameters.AddWithValue("@ALAMAT", arr[6]);
                cmd2.Parameters.AddWithValue("@AGAMA", arr[7]);
                cmd2.Parameters.AddWithValue("@STATUSKWN", arr[8]);
                cmd2.Parameters.AddWithValue("@JOB", arr[9]);
                cmd2.Parameters.AddWithValue("@KWN", arr[10]);

                cmd2.ExecuteNonQuery();
            }
        }
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
