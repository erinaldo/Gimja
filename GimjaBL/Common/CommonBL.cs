using System.Data;
using System.Reflection;

namespace GimjaBL
{
    public class CommonBL
    {
        public CommonBL()
        {
        }

        public static DataTable GetCountriesList()
        {
            Assembly _assembly = Assembly.GetExecutingAssembly();

            DataSet _dsCountries = new DataSet();

            _dsCountries.ReadXml(_assembly.GetManifestResourceStream("GimjaBL.Countries.xml"));

            return _dsCountries.Tables[0];
        }
    }
}
