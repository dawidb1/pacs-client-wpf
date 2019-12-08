using System.Collections.Generic;

namespace pacs_client.Model
{
    public class Patients
    {
        IPacsConfiguration pacsConfiguration = null;
        public Patients(IPacsConfiguration pacsConfiguration)
        {
            this.pacsConfiguration = pacsConfiguration;
        }

        string status = string.Empty;

        public List<string> GetPatients()
        {
            var patients = new List<string>();
            // typ wyszukiwania (rozpoczynamy od pacjenta)
            gdcm.ERootType typ = gdcm.ERootType.ePatientRootType;

            // do jakiego poziomu wyszukujemy 
            gdcm.EQueryLevel poziom = gdcm.EQueryLevel.ePatient; // zobacz tez inne 

            // klucze (filtrowanie lub określenie, które dane są potrzebne)
            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();

            gdcm.Tag tag = new gdcm.Tag(0x0010, 0x0010); // 10,10 == PATIENT_NAME
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(tag, "*"); // * == dowolne imię
            klucze.Add(klucz1);
            klucze.Add(new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0020), ""));
            // zwrotnie oczekujemy wypełnionego 10,20 czyli PATIENT_ID

            // skonstruuj zapytanie
            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(typ, poziom, klucze);

            // sprawdź, czy zapytanie spełnia kryteria
            if (!zapytanie.ValidateQuery())
            {
                return null;
            }

            // kontener na wyniki
            gdcm.DataSetArrayType wynik = new gdcm.DataSetArrayType();

            // wykonaj zapytanie
            bool stan = gdcm.CompositeNetworkFunctions.CFind(
                this.pacsConfiguration.ipPACS,
                this.pacsConfiguration.portPACS,
                zapytanie,
                wynik,
                this.pacsConfiguration.myAET,
                this.pacsConfiguration.callAET);

            // sprawdź stan
            if (!stan)
            {
                return null;
            }

            // pokaż wyniki
            foreach (gdcm.DataSet x in wynik)
            {
                // jeden element pary klucz-wartość
                gdcm.DataElement de = x.GetDataElement(new gdcm.Tag(0x0010, 0x0020)); // konkretnie 10,20 = PATIENT_ID

                // dostęp jako string
                gdcm.Value val = de.GetValue(); // pobierz wartość dla wskazanego klucza...
                string str = val.toString();    // ...jako napis
                patients.Add(str);
            }
            return patients;
        }
    }
}
