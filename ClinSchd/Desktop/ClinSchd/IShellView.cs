namespace ClinSchd
{
    public interface IShellView
    {
        void ShowView();
		ShellPresenter Presenter { get; set; }
    }
}