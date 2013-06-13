using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.DataAccess.Controllers
{
    public interface IDataAccessController
    {
        //It may be reasonable to have a Run method instead of relying on the constructor to start it up
        void Run();
    }
}