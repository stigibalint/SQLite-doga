using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    public class Felvetelizo
    {
        string nev, szak;
        bool nem;
        int pontszam;

        public Felvetelizo(string nev, bool nem, int pontszam,string szak)
        {
            this.Nev = nev;
            this.Szak = szak;
            this.Nem = nem;
            this.Pontszam = pontszam;
        }

        public string Nev { get => nev; set => nev = value; }
        public string Szak { get => szak; set => szak = value; }
        public bool Nem { get => nem; set => nem = value; }
        public int Pontszam { get => pontszam; set => pontszam = value; }
    }

}
