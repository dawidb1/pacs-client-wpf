using System;
using System.Collections.Generic;
using System.Drawing;

namespace pacs_client.Model
{
    // based on https://platforma.polsl.pl/rib/mod/resource/view.php?id=12805
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
            gdcm.ERootType typ = gdcm.ERootType.ePatientRootType;

            gdcm.EQueryLevel poziom = gdcm.EQueryLevel.ePatient;

            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0020), patientId);
            klucze.Add(klucz1);

            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(typ, poziom, klucze, true);

            string odebrane = System.IO.Path.Combine(".", "odebrane");
            if (!System.IO.Directory.Exists(odebrane))
                System.IO.Directory.CreateDirectory(odebrane);
            string dane = System.IO.Path.Combine(odebrane, System.IO.Path.GetRandomFileName());
            System.IO.Directory.CreateDirectory(dane);

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

                gdcm.Bitmap bmjpeg2000 = BitmapCoder.pxmap2jpeg2000(reader.GetPixmap());
                Bitmap[] X = BitmapCoder.gdcmBitmap2Bitmap(bmjpeg2000);
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
