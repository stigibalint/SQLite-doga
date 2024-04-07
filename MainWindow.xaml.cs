using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SQLite
{
    public partial class MainWindow : Window
    {
        SqliteConnection connection;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAdatbazis_Click(object sender, RoutedEventArgs e)
        {
            string databaseName = "adatbazis.db";
            if (!File.Exists(databaseName))
            {
                try
                {
                    using (var connection = new SqliteConnection($"FileName={databaseName}"))
                    {
                        connection.Open();

                        var createTableCmd = connection.CreateCommand();
                        createTableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Felvi (
                                      Nev TEXT,
                                      Nem TEXT,
                                      Pontszam INTEGER,
                                      Szak TEXT
                                  )";

                        createTableCmd.ExecuteNonQuery();

                        string[] lines = File.ReadAllLines("felvi.csv");
                        bool elsoSorkihagyas = true;

                        foreach (string line in lines)
                        {
                            if (elsoSorkihagyas)
                            {
                                elsoSorkihagyas = false;
                                continue;
                            }

                            string[] values = line.Split(';');


                            var insertCmd = connection.CreateCommand();
                            insertCmd.CommandText = "INSERT INTO Felvi (nev, nem, pontszam, szak) VALUES (@nev, @nem, @pontszam, @szak)";
                            insertCmd.Parameters.AddWithValue("@nev", values[0]);
                            insertCmd.Parameters.AddWithValue("@nem", values[1]);
                            insertCmd.Parameters.AddWithValue("@pontszam", int.Parse(values[2]));
                            insertCmd.Parameters.AddWithValue("@szak", values[3]);
                            insertCmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Adatbázis létrehozva és feltöltve a felvi.csv tartalmával.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt az adatbázis létrehozása: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Az adatbázis már létezik.");
            }
        }

        private void btnLegmagasabbPontszam_Click(object sender, RoutedEventArgs e)
        {
            string szak = txtSzakReszlet.Text;

            try
            {
                using (var connection = new SqliteConnection($"FileName=adatbazis.db"))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"SELECT Nev
                                FROM Felvi 
                                WHERE szak Like '%@szak%'
                                ORDER BY pontszam DESC
                                LIMIT 1";

                    cmd.Parameters.AddWithValue("@szak", szak);
                    var reader = cmd.ExecuteReader();
                    reader.Read();
                    string legmagasabbPontszamNev = reader.GetString(0);
                    MessageBox.Show($"A legmagasabb pontszámot elért tanuló a(z) {szak} szakon: {legmagasabbPontszamNev}");

                    {
                        MessageBox.Show("Nincs ilyen szak.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}");
            }
        }

        private void btnSzamolas_Click(object sender, RoutedEventArgs e)
        {

            int lanyokSzama = 0;
            int fiukSzama = 0;
            foreach (var item in gridRacs.Items)
            {
                Felvetelizo felvetelizo = item as Felvetelizo;
                if (felvetelizo.Szak.Contains("informatikus"))
                {
                    if (felvetelizo.Nem)
                        fiukSzama++;

                    else
                        lanyokSzama++;

                }
            }


            lblEredmeny.Content = $"Lányok száma: {lanyokSzama}, Fiúk száma: {fiukSzama}";
        }

        private void btnTorles_Click(object sender, RoutedEventArgs e)
        {
            if (gridRacs.SelectedItem != null)
            {

                var v = gridRacs.SelectedItem as Felvetelizo;
                var items = (gridRacs.ItemsSource as List<Felvetelizo>).Where(x => x.Nev == v.Nev && x.Nem == v.Nem && x.Pontszam == v.Pontszam && x.Szak == v.Szak);
            }
            else
            {
                MessageBox.Show("Nincs kijelölt sor a törléshez!");
            }
        }

        private void btnBetoltes_Click(object sender, RoutedEventArgs e)  //most vettem észre, hogy kell szűrni is :(
        {
            string szak = txtSzakReszlet.Text;

            List<Felvetelizo> felvetelizok = new List<Felvetelizo>();

            try
            {
                using (var connection = new SqliteConnection($"FileName=adatbazis.db"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    if (string.IsNullOrWhiteSpace(szak)) 
                    {
                        command.CommandText = "SELECT * FROM Felvi";
                    }
                    else 
                    {
                        command.CommandText = "SELECT * FROM Felvi WHERE Szak = @szak";
                        command.Parameters.AddWithValue("@szak", szak);
                    }

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string nev = reader.GetString(0);
                        bool nem = reader.GetString(1) == "f";
                        int pontszam = reader.GetInt32(2);
                        string szakRead = reader.GetString(3);
                        Felvetelizo felvetelizo = new Felvetelizo(nev, nem, pontszam, szakRead);
                        felvetelizok.Add(felvetelizo);
                    }
                    reader.Close();
                }

                gridRacs.ItemsSource = felvetelizok;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}");
            }
        }


    }
}