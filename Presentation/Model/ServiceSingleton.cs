using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace IntroSE.Kanban.Presentation.Model
{
    ///<summary>Represents a Service Singleton</summary>
    public class ServiceSingleton
    {
        private static Service instance = null;

        ///<summary>Return a exist Service or construct a new service if does not exsist and return it</summary>
        ///<returns>return Service.</returns>
        public static Service GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new Service();
                return instance;
            }
        }

    }
}
