using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public interface IDataManager<T>
    {
        void Add();

        void Update();

        void Delete();

        IEnumerable<T> GetData();

        void HasData();
    }
}
