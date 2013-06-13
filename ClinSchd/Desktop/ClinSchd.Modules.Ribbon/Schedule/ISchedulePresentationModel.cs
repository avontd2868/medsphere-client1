using ClinSchd.Modules.Ribbon.Controllers;

namespace ClinSchd.Modules.Ribbon.Schedule
{
	public interface ISchedulePresentationModel
    {
		IScheduleView View { get; }
		bool IsManagerUser { get; set; }
	}
}
