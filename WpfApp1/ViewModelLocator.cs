using System;
using GalaSoft.MvvmLight;

namespace WpfApp1
{
    public class ViewModelLocator
    {
        private static readonly Lazy<ViewModelLocator> instance = new Lazy<ViewModelLocator>(() => new ViewModelLocator());

        public static ViewModelLocator Instance => instance.Value;

        private ViewModelLocator()
        {
            RegisterViewModels();
        }

        public MainViewModel MainViewModel { get; private set; }

        private void RegisterViewModels()
        {
            MainViewModel = new MainViewModel();
        }
    }
}
