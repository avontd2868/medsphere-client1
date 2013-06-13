using System;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.UserLogin.Services;
using ClinSchd.Modules.UserLogin.UserLogin;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.UserLogin.Controllers
{
	public class UserLoginController : IUserLoginController
    {
		private readonly IUnityContainer container;
		private readonly IUserLoginService UserLoginService;
		private readonly IEventAggregator eventAggregator;

		public UserLoginController (IUnityContainer container,
			IUserLoginService UserLoginService,
			IEventAggregator eventAggregator)
		{
			this.container = container;
			this.UserLoginService = UserLoginService;
			this.eventAggregator = eventAggregator;
		}

		public void Run ()
		{
			this.eventAggregator.GetEvent<LaunchUserLoginDialogEvent> ().Subscribe (LaunchUserLoginDialog, ThreadOption.UIThread, true);


			this.eventAggregator.GetEvent<LaunchUserLoginDialogEvent>().Publish("User Login");
		}

		public void LaunchUserLoginDialog (string Title)
		{
			IUserLoginPresentationModel Model = container.Resolve<IUserLoginPresentationModel> ();
			
			if (Model.ValidationMessage.IsValid) {
				this.UserLoginService.ShowDialog (
				Model.View,
				Model, () => Model.OnClose ());
			} else {
				Model.View.AlertUser (Model.ValidationMessage.Message, Model.ValidationMessage.Title);
				Model.ValidationMessage.IsValid = true;
				Model.ValidationMessage.Title = string.Empty;
				Model.ValidationMessage.Message = string.Empty;
			}
		}
    }
}
