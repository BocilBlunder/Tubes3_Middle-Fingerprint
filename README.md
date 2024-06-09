<h1 align="center">Tugas Besar 3 IF2211 Strategi Algoritma</h1>
<h1 align="center"> Semester II tahun 2023/2024 </h1>
<h1 align="center"> Pemanfaatan Pattern Matching dalam Membangun Sistem Deteksi Individu Berbasis Biometrik Melalui Citra Sidik Jari </h1>

<h1 align=""> Kelompok 50 - Middle Fingerprint </h1>

| NIM      | Nama                    | Kelas |
| -------- | ----------------------- | ----- |
| 13522109 | Azmi Mahmud Bazeid      | K-01  |
| 13522113 | William Glory Henderson | K-01  |
| 13522115 | Derwin Rustanly         | K-01  |

## Table of Contents

- [General Info](#General-Info)
- [How To Run](#How-to-Run)
- [How To Use](#How-To-Use)
- [Programming Language](#Programming-Language)
- [Link](#link)

## General-Info

This program is designed for biometric identification through fingerprint analysis, utilizing the Knuth-Morris-Pratt (KMP) and Boyer-Moore (BM) algorithms for efficient pattern matching. It integrates a robust system for handling name searches, using Regular Expressions (Regex) to find or fix from variations name such as 'bahasa alay'. In cases where exact matches (fingerprint or name) are not found, the program further enhances its search accuracy using the Levenshtein Distance to find the closest possible matches, ensuring reliable identification.

## How-To-Run

1.  Clone this repository :

    ```bash
    git clone https://github.com/BocilBlunder/Tubes3_Middle-Fingerprint.git
    ```

2. Create the local database using sql file in folder Database

3. Change the server, name, user, and password for dbController in API.cs

4.  Navigate to the directory:

    ```bash
    cd Tubes3_Middle-Fingerprint/src/fe/MiddleFingerprintUI
    ```

5. Run the program:
    ```bash
    dotnet run
    ```

## How-To-Use

1. Type "dotnet run" in terminal to launch the GUI screen.
2. Press upload image button to upload a file in format jpg, jpeg, png, bmp.
3. Select the algorithm you want to use and press search button.
4. Press info button to see the owner biodata of the fingerprint.

## Programming-Language

1. C# (C Sharp)
2. SQL (MYSQL)

## Link

- Repository : https://github.com/BocilBlunder/Tubes3_Middle-Fingerprint
- Video : https://youtu.be/csvWkI7znQ0?si=3VnyoNE92doZEcZ1