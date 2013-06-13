using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.UserLogin.UserLogin;
using ClinSchd.Modules.UserLogin.Controllers;
using ClinSchd.Modules.UserLogin.Services;

namespace ClinSchd.Modules.UserLogin
{
    public class UserLoginModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IUserLoginController controller;

		public UserLoginModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
			this.RegisterViewsAndServices ();
			controller = this.container.Resolve<IUserLoginController> ();
			controller.Run ();
        }

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IUserLoginController, UserLoginController> ();
			this.container.RegisterType<IUserLoginView, UserLoginView> ();
			this.container.RegisterType<IUserLoginPresentationModel, UserLoginPresentationModel> ();
			this.container.RegisterType<IUserLoginService, UserLoginService> ();
        }
    }
}
