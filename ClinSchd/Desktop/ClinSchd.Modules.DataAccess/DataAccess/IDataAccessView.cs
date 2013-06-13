using System;

namespace ClinSchd.Modules.DataAccess.DataAccess
{
    public interface IDataAccessView
    {
        DataAccessPresentationModel Model { get; set; }
    }
}
