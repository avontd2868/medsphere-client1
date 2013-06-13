using Microsoft.Practices.Composite.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ClinSchd
{
    public class EnterpriseLibraryLoggerAdapter : ILoggerFacade
    {
        #region ILoggerFacade Members

        public void Log(string message, Category category, Priority priority)
        {
            Logger.Write(message, category.ToString(), (int)priority);
        }

        #endregion
    }
}
