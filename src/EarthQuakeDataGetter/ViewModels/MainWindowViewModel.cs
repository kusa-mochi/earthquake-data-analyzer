using Prism.Mvvm;

namespace EarthQuakeDataGetter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "国内地震情報収集ツール";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
