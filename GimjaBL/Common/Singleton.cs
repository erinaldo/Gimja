using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public sealed class Singleton
    {
        public static Singleton Instance { get; private set; }

        private Singleton() { }

        public string UserID;

        static Singleton() { Instance = new Singleton(); }
    }
}