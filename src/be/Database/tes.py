import mysql.connector
from faker import Faker
import random
import os

# Initialize Faker
faker = Faker()

# Connect to the MariaDB database
conn = mysql.connector.connect(
    host='localhost',  # Change if needed
    user='root',  # Replace with your username
    password='',  # Replace with your password
    database='tubes3',  # Your database name
    port=3306  # Change if needed
)

def print_files_in_folder(folder_path):
    files = []
    # List all files and folders in the directory
    for item in os.listdir(folder_path):
        # Create full path
        full_path = os.path.join(folder_path, item)
        # Check if it is a file
        if os.path.isfile(full_path):
            files.append(full_path)
    return files

cursor = conn.cursor()

# Generate and insert biodata records
num_biodata_records = 50
for _ in range(num_biodata_records):
    nik = faker.unique.random_number(digits=16)
    nama = faker.name()
    tempat_lahir = faker.city()
    tanggal_lahir = faker.date_of_birth()
    jenis_kelamin = random.choice(['Laki-Laki', 'Perempuan'])
    golongan_darah = random.choice(['A', 'B', 'AB', 'O'])
    alamat = faker.address()
    agama = random.choice(['Islam', 'Kristen', 'Katolik', 'Hindu', 'Buddha', 'Konghucu'])
    status_perkawinan = random.choice(['Belum Menikah', 'Menikah', 'Cerai'])
    pekerjaan = faker.job()
    kewarganegaraan = 'Indonesia'

    cursor.execute(
        "INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
        (nik, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
    )

# Fetch all names
cursor.execute("SELECT nama FROM biodata")
names = [row[0] for row in cursor.fetchall()]

# Insert sidik_jari records
image_files = print_files_in_folder("img")
for image_path in image_files:
    name = random.choice(names)  # Randomly select a name from biodata
    cursor.execute(
        "INSERT INTO sidik_jari (berkas_citra, nama) VALUES (%s, %s)",
        (image_path, name)
    )

# Commit the changes
conn.commit()

# Close the connection
cursor.close()
conn.close()
