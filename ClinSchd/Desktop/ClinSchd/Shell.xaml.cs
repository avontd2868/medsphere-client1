using System.Windows;
using Telerik.Windows.Controls;
using System.Windows.Input;

namespace ClinSchd
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
	public partial class Shell : Window, IShellView
    {
        public Shell()
        {
			Telerik.Windows.Controls.StyleManager.ApplicationTheme = new Telerik.Windows.Controls.Office_BlueTheme();
			InitializeComponent();
        }

		public ShellPresenter Presenter { get; set; }

        public void ShowView()
        {
            this.Show();
        }

		private void Grid_KeyDown (object sender, System.Windows.Input.KeyEventArgs e)
		{
			Presenter.KeyPressed (e);
		}
    }
}
