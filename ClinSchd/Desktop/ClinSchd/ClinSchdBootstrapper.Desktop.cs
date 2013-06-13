using Microsoft.Practices.Composite.Logging;

namespace ClinSchd
{
    public partial class ClinSchdBootstrapper
    {
        private readonly EnterpriseLibraryLoggerAdapter _logger = new EnterpriseLibraryLoggerAdapter();

        protected override ILoggerFacade LoggerFacade
        {
            get { return _logger; }
        }
    }
}
