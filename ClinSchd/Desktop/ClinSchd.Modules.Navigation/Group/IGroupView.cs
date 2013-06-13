using System;
using System.Windows.Threading;

namespace ClinSchd.Modules.Navigation.Group
{
	public interface IGroupView
    {
		GroupPresentationModel Model { get; set; }
    }
}
