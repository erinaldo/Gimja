using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class ConvertBytesToImageBL
    {
        /// <summary>
        /// This method accepts byte and returns an image
        /// </summary>
        /// <param name="_byte"></param>
        /// <returns></returns>
        public static MemoryStream ByteToImage(byte[] _byte)
        {
            if (_byte == null)
                return null;

            MemoryStream image = new MemoryStream(_byte);
            return image;
        }

    }
}
