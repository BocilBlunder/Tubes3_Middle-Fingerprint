import mysql.connector

# Connect to the MariaDB database
conn = mysql.connector.connect(
    host='localhost',  # Change if needed
    user='root',  # Replace with your username
    password='',  # Replace with your password
    database='tubes3',  # Your database name
    port=3306  # Change if needed
)

cursor = conn.cursor()

# List of new 'kata alay' names
kata_alay_names = [
    "lwrNc c5", "jMs tAt", "jshU nLsn", "j5n BErn4rd", "mnd cSy", "rnd4Ll all3n", "cHrL5 Mcn3l",
    "sctt 6rnr", "kV1N brNTT", "cHr15tN4 P0p", "frnK r0drg2", "pl rngL", "H4Th3R fI5hr", "tRrnC3 cmpbLL",
    "mY jM5", "DN3 brry", "jM5 w1S5", "whtny wllM5", "jaSn 4yRs", "KMBRly jrv5", "mchll R45mssn", "htHr haLl",
    "Rbecc bKer", "54mUl MlL3r", "jN p4rk", "pl mcL3N", "jeNnfr k3nn3Dy", "dR. thR5 Cl4rk", "55n jnnN65",
    "tdD wlf3", "b4ley 6rdn3R", "br3nd blck", "ptRc nwmn", "dgL5 nVrR", "M1ch4l hrt", "bryN McmLln", "5MmeR h1ll",
    "brn mThw5", "4my 5Mp5n", "Mry rch4rd5n", "d4m 4Ndr5on", "Nth4Nl 3tn", "5hnNn R0b3rt5n", "cry5t4l bRwn",
    "hctR prs0n5", "chR5TpH3R smTh", "bRnD cNLy", "JFfry Hn5n", "4ndR4 n3L5On", "pTrc mrr15"
]

# Fetch all NIKs from the biodata table
cursor.execute("SELECT NIK FROM biodata")
niks = cursor.fetchall()

# Ensure there are enough NIKs to match with the provided names
if len(niks) < len(kata_alay_names):
    print("Error: Not enough rows in biodata table to update with provided names.")
else:
    # Update the 'biodata' table with new 'kata alay' names
    for i, (NIK,) in enumerate(niks[:len(kata_alay_names)]):
        update_command = "UPDATE biodata SET nama = %s WHERE NIK = %s"
        cursor.execute(update_command, (kata_alay_names[i], NIK))

    # Commit the changes
    conn.commit()
    print(f"Updated {len(kata_alay_names)} rows in the biodata table.")

# Close the connection
cursor.close()
conn.close()
