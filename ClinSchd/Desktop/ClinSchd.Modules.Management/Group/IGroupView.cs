using System;

namespace ClinSchd.Modules.Management.Group
{
	public interface IGroupView
    {
		event EventHandler<EventArgs> ShowManagement;

        GroupPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog ();
		object DataContext { get; set; }
		void Close ();

    }
}
