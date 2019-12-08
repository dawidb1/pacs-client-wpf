using System;
using System.Collections.Generic;
using System.Drawing;

namespace pacs_client.Model
{
    public class Images
    {
        private IPacsConfiguration pacsConfiguration;

        public Images(IPacsConfiguration pacsConfiguration)
        {
            this.pacsConfiguration = pacsConfiguration;
        }

        public List<string> GetImages(string patientId)
        {
            var images = new List<string>();
            string resultString = string.Empty;
            // typ wyszukiwania (rozpoczynamy od pacjenta)
            gdcm.ERootType typ = gdcm.ERootType.ePatientRootType;

            // do jakiego poziomu wyszukujemy 
            gdcm.EQueryLevel poziom = gdcm.EQueryLevel.ePatient; // zobacz inne 

            // klucze (filtrowanie lub określenie, które dane są potrzebne)
            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0020), patientId); // NIE WOLNO TU STOSOWAC *; tutaj PatientID="01"
            klucze.Add(klucz1);

            // skonstruuj zapytanie
            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(typ, poziom, klucze, true);


            // przygotuj katalog na wyniki
            string odebrane = System.IO.Path.Combine(".", "odebrane"); // podkatalog odebrane w bieżącym katalogu
            if (!System.IO.Directory.Exists(odebrane)) // jeśli nie istnieje
                System.IO.Directory.CreateDirectory(odebrane); // utwórz go
            string dane = System.IO.Path.Combine(odebrane, System.IO.Path.GetRandomFileName()); // wygeneruj losową nazwę podkatalogu
            System.IO.Directory.CreateDirectory(dane); // i go utwórz

            // wykonaj zapytanie - pobierz do katalogu jak w zmiennej 'dane'
            bool stan = gdcm.CompositeNetworkFunctions.CMove(this.pacsConfiguration.ipPACS,
                this.pacsConfiguration.portPACS,
                zapytanie, this.pacsConfiguration.portMove,
                this.pacsConfiguration.myAET,
                this.pacsConfiguration.callAET,
                dane);


            List<string> pliki = new List<string>(System.IO.Directory.EnumerateFiles(dane));
            foreach (string plik in pliki)
            {
                gdcm.PixmapReader reader = new gdcm.PixmapReader();
                reader.SetFileName(plik);
                if (!reader.Read())
                {
                    // do not remove!!!! 
                    continue;
                }

                // przekonwertuj na "znany format"
                gdcm.Bitmap bmjpeg2000 = BitmapCoder.pxmap2jpeg2000(reader.GetPixmap());
                // przekonwertuj na .NET bitmapę
                Bitmap[] X = BitmapCoder.gdcmBitmap2Bitmap(bmjpeg2000);
                // zapisz
                for (int i = 0; i < X.Length; i++)
                {
                    string name = String.Format("{0}_warstwa{1}.jpg", plik, i);
                    X[i].Save(name);
                    images.Add(name);
                }
            }
            return images;
        }
    }
}
