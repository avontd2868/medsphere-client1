using System.Windows.Input;
using Microsoft.Practices.Composite.Events;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
namespace ClinSchd
{
    public class ShellPresenter
    {
		IEventAggregator eventAggregator;
        public ShellPresenter(IShellView view, IEventAggregator _eventAggregator, IDataAccessService _dataAccess)
        {
            View = view;
			View.Presenter = this;
			eventAggregator = _eventAggregator;
		}

        public IShellView View { get; private set; }

		internal void KeyPressed (System.Windows.Input.KeyEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control) {
				switch (e.Key) {
				case Key.F:
					eventAggregator.GetEvent<FocusPatientSearchEvent> ().Publish (null);
					break;
				case Key.S:
					eventAggregator.GetEvent<FocusSchedulerEvent> ().Publish (null);
					break;
				case Key.C:
					eventAggregator.GetEvent<CopyKeysPressedEvent> ().Publish (null);
					break;
				case Key.V:
					eventAggregator.GetEvent<PasteKeysPressedEvent> ().Publish (null);
					break;
				default:
					break;
				}
			}
		}
	}
}