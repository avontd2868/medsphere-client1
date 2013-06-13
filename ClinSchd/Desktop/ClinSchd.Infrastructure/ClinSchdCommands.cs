using Microsoft.Practices.Composite.Presentation.Commands;

namespace ClinSchd.Infrastructure
{

    public static class ClinSchdCommands
    {
		private static CompositeCommand selectPatientSearchCommand = new CompositeCommand();

		public static CompositeCommand SelectPatientSearchCommand
        {
			get { return selectPatientSearchCommand; }
			set { selectPatientSearchCommand = value; }
        }
    }
}
