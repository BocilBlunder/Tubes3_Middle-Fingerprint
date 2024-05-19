#pragma warning disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        // Load input fingerprint image
        Bitmap inputFingerprint = new Bitmap("..\\..\\img\\2__F_Right_index_finger.BMP");

        // Segment the input image to ASCII
        string inputAscii = ImageProcessing.SegmentToAscii(inputFingerprint, 0, 0, inputFingerprint.Width, inputFingerprint.Height);

        // Load fingerprint images from the database
        List<Bitmap> databaseFingerprints = LoadFingerprintsFromDatabase();

        // Convert database images to ASCII
        List<string> databaseAscii = new List<string>();
        foreach (var bmp in databaseFingerprints)
        {
            string ascii = ImageProcessing.SegmentToAscii(bmp, 0, 0, bmp.Width, bmp.Height);
            databaseAscii.Add(ascii);
        }

        // Search for a match using KMP and Boyer-Moore algorithms
        bool matchFound = false;
        for (int i = 0; i < databaseAscii.Count; i++)
        {
            if (KMP.Search(databaseAscii[i], inputAscii) != -1)
            {
                Console.WriteLine($"KMP: Match found in fingerprint {i + 1}");
                matchFound = true;
            }

            if (BM.Search(databaseAscii[i], inputAscii) != -1)
            {
                Console.WriteLine($"BM: Match found in fingerprint {i + 1}");
                matchFound = true;
            }
        }

        if (!matchFound)
        {
            Console.WriteLine("No match found.");
        }
    }

    static List<Bitmap> LoadFingerprintsFromDatabase()
    {
        string connectionString = "server=localhost;user=root;password=;database=tubes3";
        string query = "SELECT berkas_citra FROM sidik_jari";

        List<Bitmap> fingerprints = new List<Bitmap>();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string imageData = "..\\..\\"+(string)reader["berkas_citra"];
                        Bitmap bmp = new Bitmap(imageData);
                        fingerprints.Add(bmp);
                    }
                }
            }
        }

        return fingerprints;
    }
}

#pragma warning restore