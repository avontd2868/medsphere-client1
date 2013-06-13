using ClinSchd.Modules.DataAccess.Controllers;

namespace ClinSchd.Modules.DataAccess.DataAccess
{
    public interface IDataAccessPresentationModel
    {
        IDataAccessView View { get; }
        IDataAccessController Controller { get; set; }
    }
}
