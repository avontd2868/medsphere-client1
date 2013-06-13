using System;
using ClinSchd.Modules.DataAccess.DataAccess;

namespace ClinSchd.Modules.DataAccess.Tests.Mocks
{
    public class MockDataAccessView : IDataAccessView
    {
        public event EventHandler<EventArgs> ShowDataAccess = delegate { };

        public DataAccessPresentationModel Model { get; set; }
        
        public void RaiseShowDataAccessEvent()
        {
            ShowDataAccess(this, EventArgs.Empty);
        }
    }
}
