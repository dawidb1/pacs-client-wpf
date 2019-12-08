using System.Collections.Generic;

namespace pacs_client.Model
{
    // based on https://platforma.polsl.pl/rib/mod/resource/view.php?id=12805
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
            gdcm.ERootType typ = gdcm.ERootType.ePatientRootType;
            gdcm.EQueryLevel poziom = gdcm.EQueryLevel.ePatient;
            gdcm.KeyValuePairArrayType klucze = new gdcm.KeyValuePairArrayType();

            gdcm.Tag tag = new gdcm.Tag(0x0010, 0x0010);
            gdcm.KeyValuePairType klucz1 = new gdcm.KeyValuePairType(tag, "*");
            klucze.Add(klucz1);
            klucze.Add(new gdcm.KeyValuePairType(new gdcm.Tag(0x0010, 0x0020), ""));
            gdcm.BaseRootQuery zapytanie = gdcm.CompositeNetworkFunctions.ConstructQuery(typ, poziom, klucze);

            // sprawdź, czy zapytanie spełnia kryteria
            if (!zapytanie.ValidateQuery())
            {
                return null;
            }

            // kontener na wyniki
            gdcm.DataSetArrayType wynik = new gdcm.DataSetArrayType();

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

                gdcm.Value val = de.GetValue();
                string str = val.toString();
                patients.Add(str);
            }
            return patients;
        }
    }
}
