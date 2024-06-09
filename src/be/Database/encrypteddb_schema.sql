CREATE TABLE `encryptedbiodata` (
  `NIK` varbinary(2000) NOT NULL,
  `nama` varbinary(2000) DEFAULT NULL,
  `tempat_lahir` varbinary(2000) DEFAULT NULL,
  `tanggal_lahir` date DEFAULT NULL,
  `jenis_kelamin` enum('Laki-Laki','Perempuan') DEFAULT NULL,
  `golongan_darah` varbinary(2000) DEFAULT NULL,
  `alamat` varbinary(2000) DEFAULT NULL,
  `agama` varbinary(2000) DEFAULT NULL,
  `status_perkawinan` enum('Belum Menikah','Menikah','Cerai') DEFAULT NULL,
  `pekerjaan` varbinary(2000) DEFAULT NULL,
  `kewarganegaraan` varbinary(2000) DEFAULT NULL
);