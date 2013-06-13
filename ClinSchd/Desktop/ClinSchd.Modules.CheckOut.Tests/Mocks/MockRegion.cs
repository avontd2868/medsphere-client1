using System.ComponentModel;
using Microsoft.Practices.Composite.Regions;

namespace ClinSchd.Modules.CheckOut.Tests.Mocks
{
    internal class MockRegion : IRegion
    {
        public string RequestedViewName;
        public bool ActivateCalled;
        public object ActivateArg;

        public string Name { get; set; }

        public object GetView(string viewName)
        {
            RequestedViewName = viewName;
            return null;
        }

        public void Activate(object view)
        {
            ActivateCalled = true;
            ActivateArg = view;
        }

        public IRegionManager Add(object view, string viewName)
        {
            return null;
        }

        #region Not Implemented members

        public event PropertyChangedEventHandler PropertyChanged;

        public IViewsCollection Views
        {
            get { throw new System.NotImplementedException(); }
        }

        public IViewsCollection ActiveViews
        {
            get { throw new System.NotImplementedException(); }
        }

        public object Context
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public IRegionManager Add(object view)
        {
            return null;
        }

        public IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(object view)
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate(object view)
        {
            throw new System.NotImplementedException();
        }

        public IRegionManager RegionManager
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public IRegionBehaviorCollection Behaviors
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}