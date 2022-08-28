using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Common
{
    public sealed class SingletonLock
    {
        private static volatile SingletonLock instance;
        private static object syncRoot = new Object();

        private SingletonLock() { }

        public static SingletonLock Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SingletonLock();
                    }
                }

                return instance;
            }
        }
    }

}
