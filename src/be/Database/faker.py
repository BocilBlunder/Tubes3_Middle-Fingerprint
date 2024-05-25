import mysql.connector
from be.Database.faker import Faker
import random
import os

# MySQL database configuration
db_config = {
    'user': 'root',
    'password': '',
    'host': '127.0.0.1',
    'port':"3306",
    'database': 'tubes3',
}

fake = Faker('id_ID')  # Using Indonesian locale

jenis_kelamin_list = ['Laki-Laki', 'Perempuan']
status_perkawinan_list = ['Belum Menikah', 'Menikah', 'Cerai']

kode_wilayah_list = [
    '317401',  # Jakarta Selatan
    '320401',  # Bandung
    '327401',  # Bogor
    '330401',  # Semarang
    '340401'   # Yogyakarta
]

replacement_dict = {
    'a': ['4', 'a', 'A'],
    'e': ['3', 'e', 'E'],
    'i': ['1', 'i', 'I'],
    'o': ['0', 'o', 'O'],
    'b': ['8', 'b', 'B'],
    'g': ['9', '6', 'g', 'G'],
    's': ['5', 's', 'S'],
    't': ['7', 't', 'T']
}

def replace_with_numbers(word):
    result = []
    for char in word:
        if char.lower() in replacement_dict:
            replacements = replacement_dict[char.lower()]
            result.append(random.choice(replacements))
        else:
            result.append(char)
    return ''.join(result)

def random_case(word):
    return ''.join(char.lower() if random.choice([True, False]) else char.upper() for char in word)

def shorten_word(word):
    vowels = 'aeiouAEIOU'
    return ''.join(char for char in word if char not in vowels)

def alay_name_generator(original_name):
    words = original_name.split()
    alay_words = []

    for word in words:
        transformations = [shorten_word, replace_with_numbers, random_case]
        random.shuffle(transformations)
        
        for transformation in transformations:
            word = transformation(word)
        
        alay_words.append(word)
    
    return ' '.join(alay_words)

class Person:
    def __init__(self, nik, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, gol_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan):
        self.nik = nik
        self.nama = nama
        self.tempat_lahir = tempat_lahir
        self.tanggal_lahir = tanggal_lahir
        self.jenis_kelamin = jenis_kelamin
        self.gol_darah = gol_darah
        self.alamat = alamat
        self.agama = agama
        self.status_perkawinan = status_perkawinan
        self.pekerjaan = pekerjaan
        self.kewarganegaraan = kewarganegaraan

def generate_names():
    word_count = random.choice([2, 3, 4])
    name_parts = [fake.first_name() for _ in range(word_count - 1)]
    name_parts.append(fake.last_name())
    return ' '.join(name_parts)

def generate_nik(jenis_kelamin, tanggal_lahir):
    kode_wilayah = random.choice(kode_wilayah_list)
    
    dd = tanggal_lahir.day
    mm = tanggal_lahir.month
    yy = tanggal_lahir.year % 100

    if jenis_kelamin == 'Perempuan':
        dd += 40
    
    tanggal_lahir_str = '{:02d}{:02d}{:02d}'.format(dd, mm, yy)
    
    nomor_urut = '{:04d}'.format(random.randint(1, 9999))
    
    nik = kode_wilayah + tanggal_lahir_str + nomor_urut
    return nik

def generate_person():
    nama = generate_names()
    tempat_lahir = fake.city()
    tanggal_lahir = fake.date_of_birth(minimum_age=18, maximum_age=80)
    jenis_kelamin = random.choice(jenis_kelamin_list)
    nik = generate_nik(jenis_kelamin, tanggal_lahir)
    tanggal_lahir_str = tanggal_lahir.strftime("%Y-%m-%d")
    gol_darah = random.choice(['A', 'B', 'AB', 'O'])
    alamat = fake.address().replace("\n", ", ")
    agama = random.choice(['Islam', 'Kristen', 'Katholik', 'Hindu', 'Buddha', 'Konghucu'])
    status_perkawinan = random.choice(status_perkawinan_list)
    pekerjaan = fake.job()
    kewarganegaraan = 'Indonesia'

    return Person(nik, nama, tempat_lahir, tanggal_lahir_str, jenis_kelamin, gol_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)

class FingerPrintData:
    def __init__(self, image_path, nama_alay):
        self.image_path = image_path
        self.nama_alay = nama_alay

PERSON_COUNT = 600
person_list = [generate_person() for _ in range(PERSON_COUNT)]

directory_path = './test/'
finger_print_data = []

for i, person in enumerate(person_list, start=1):
    nama = person.nama
    nama_alay = alay_name_generator(nama)
    person.nama = nama_alay
    
    for filename in os.listdir('./test/'):
        if filename.startswith(f'{i}_'):
            image_path = os.path.join(directory_path, filename)
            finger_print_data.append(FingerPrintData(image_path, nama))

cursor = None
try:
    conn = mysql.connector.connect(**db_config)
    cursor = conn.cursor()

    for person in person_list:
        biodata_query = """
        INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
        VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        """
        cursor.execute(biodata_query, (person.nik, person.nama, person.tempat_lahir, person.tanggal_lahir, person.jenis_kelamin, person.gol_darah, person.alamat, person.agama, person.status_perkawinan, person.pekerjaan, person.kewarganegaraan))
    
    for data in finger_print_data:
        sidik_jari_query = """
        INSERT INTO sidik_jari (berkas_citra, nama)
        VALUES (%s, %s)
        """
        cursor.execute(sidik_jari_query, (data.image_path, data.nama_alay))

    conn.commit()
    print("Data inserted successfully.")
except Exception as err:
    print(err)
finally:
    # cursor.close()
    conn.close()