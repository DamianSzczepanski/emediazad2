using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Emedia_Program2
{

    class RSA
    {
        public string sciezka;
        public byte[] TablicaBytowObrazka, TablicaBytowZakodowana;
        public int[] TablicaIntObrazka, TablicaIntZakodowana;
        public int dlugosc;

        public int n, e, d;


        ///######### ALGORYTM RSA ##############################
        int NWD(int a, int b)
        {
            while (a != b)
            {
                if (a < b)
                    b = b - a;
                else
                    a = a - b;
            }
            return a;
        }

        int OdwrotnoscModulo(int a, int n)
        {
            int a0, n0, p0, p1, q, r, t;

            p0 = 0; p1 = 1; a0 = a; n0 = n;
            q = n0 / a0;
            r = n0 % a0;
            while (r > 0)
            {
                t = p0 - q * p1;
                if (t >= 0)
                    t = t % n;
                else
                    t = n - ((-t) % n);
                p0 = p1; p1 = t;
                n0 = a0; a0 = r;
                q = n0 / a0;
                r = n0 % a0;
            }
            return p1;
        }
    
        public void GenerujKluczRSA()
        {
            int[] tp = { 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59 };
            int p, q, fi;
            System.Random losowa = new Random();

            do //Losujemy 2 liczby pierwsze
            {
                p = tp[losowa.Next(0, 13)];
                q = tp[losowa.Next(0, 13)];
            } while (p == q);


            fi = (p - 1) * (q - 1);  
            n = p * q;          
            for (e = 3; NWD(e, fi) != 1; e += 2) ; //wybieramy wzglednie pierwszą liczbe e z fi
            d = OdwrotnoscModulo(e, fi);
        } //Generujemy liczby e i d (klucz prywanty i publiczny)

        public int ModuloPotegi(int a, int w, int n)
        {
            int potega, wynik;
            int q;

            potega = a; wynik = 1;
            for (q = w; q > 0; q /= 2) //Algorytm Hornera
            {
                if ( (q % 2) != 0)
                    wynik = (wynik * potega) % n;
                potega = (potega * potega) % n; 
            }
            return wynik;
        }
        
        public int KodujRSA(int wartosc)
        {
            return ModuloPotegi(wartosc, e, n);
        }

        public int DekodujRSA(int zakodowana)
        {
            return ModuloPotegi(zakodowana, d, n);
        }

        //######## OPERACJE NA OBRAZKU #######
        public void WczytajObrazek(string nazwa)
        {
            sciezka = @"..\..\obrazy\"+nazwa;
            TablicaBytowObrazka = File.ReadAllBytes(sciezka);
            dlugosc = TablicaBytowObrazka.Length;
        }

        public int[] ByteToInt(byte[] TablicaB)
        {
            int[] tablicaInt = new int[dlugosc];
            tablicaInt = Array.ConvertAll(TablicaB, c => (int)c);
            return tablicaInt;
        }

        public static byte[] IntToByte(int[] TablicaInt)
        {
            List<byte> bytes = new List<byte>(TablicaInt.GetUpperBound(0) * sizeof(byte));
            foreach (int integer in TablicaInt)
            {
                bytes.Add(BitConverter.GetBytes(integer)[0]);
            }
            return bytes.ToArray();
        }

        public void KodujObrazek(int[] tablicaInt)
        {
            for (int i = 0; i < dlugosc; i++)
            {
                if (tablicaInt[i] == 0xff && tablicaInt[i + 1] == 0xda)
                {
                    for (int j = i + 11; ; ++j)
                    {      
                        if (tablicaInt[j] == 0xff && tablicaInt[j + 1] == 0xd9)   
                            break;

                       tablicaInt[j] = KodujRSA(tablicaInt[j]);
                    }
                    break;
                }
            }
        }

        public void DekodujObrazek(int[] tablicaInt)
        {
            for (int i = 0; i < (dlugosc); i++)
            {
                if (tablicaInt[i] == 0xff && tablicaInt[i + 1] == 0xda)
                {
                    for (int j = i + 11; ; ++j)
                    {      
                        if (tablicaInt[j] == 0xff && tablicaInt[j + 1] == 0xd9)   //if EOI found - break
                            break;

                        tablicaInt[j] = DekodujRSA(tablicaInt[j]);
                    }
                    break;
                }
            }
        }

        public void ZapiszZakodowanyPlik(byte[] TablicaB)
        {
            var fs = new BinaryWriter(new FileStream(@"..\..\obrazy\zakodowane.jpg", FileMode.Append, FileAccess.Write));
            fs.Write(TablicaB);
            fs.Close();
        }

        public void ZapiszOdkodowanyPlik(byte[] TablicaB)
        {
            var fs = new BinaryWriter(new FileStream(@"..\..\obrazy\odkodowane.jpg", FileMode.Append, FileAccess.Write));
            fs.Write(TablicaB);
            fs.Close();
        }

        //////////

        public void start()
        {
            GenerujKluczRSA();
            WczytajObrazek("lena.jpg");
            TablicaIntObrazka = ByteToInt(TablicaBytowObrazka);
            TablicaIntZakodowana = TablicaIntObrazka;
            KodujObrazek(TablicaIntZakodowana);
            TablicaBytowZakodowana = IntToByte(TablicaIntZakodowana);
            ZapiszZakodowanyPlik(TablicaBytowZakodowana);

            DekodujObrazek(TablicaIntZakodowana);
            TablicaBytowObrazka = IntToByte(TablicaIntZakodowana);
            ZapiszOdkodowanyPlik(TablicaBytowObrazka); // zapis pliku


        }

        public void startdecode(string nazwa)
        {
            GenerujKluczRSA();
            WczytajObrazek(nazwa);
            TablicaIntObrazka = ByteToInt(TablicaBytowObrazka);
            TablicaIntZakodowana = TablicaIntObrazka;
            KodujObrazek(TablicaIntZakodowana);
            TablicaBytowZakodowana = IntToByte(TablicaIntZakodowana);
            ZapiszZakodowanyPlik(TablicaBytowZakodowana);
        }

        public void startundecode ()
        {
            WczytajObrazek("zakodowane.jpg");
            TablicaIntObrazka = ByteToInt(TablicaBytowObrazka);
            DekodujObrazek(TablicaIntObrazka);
            TablicaBytowObrazka = IntToByte(TablicaIntZakodowana);
            ZapiszOdkodowanyPlik(TablicaBytowObrazka);

        }


    }

}
