using MagiRogue.DataCreatorWpf.Research;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.DataCreatorWpf
{
    public class MainWindowViewModel : ObservableObject
    {
        private IPageView? _currentPageView;
        private List<IPageView> pages;

        public IPageView? CurrentPageView
        {
            get
            {
                return _currentPageView;
            }

            set
            {
                if (_currentPageView != value)
                {
                    _currentPageView = value;
                    OnPropertyChanged(nameof(CurrentPageView));
                }
            }
        }

        public List<IPageView> Pages
        {
            get
            {
                pages ??= new List<IPageView>();

                return pages;
            }
        }

        public MainWindowViewModel()
        {
            Pages.Add(new ResearchViewModel());

            CurrentPageView = Pages[0];
        }
    }
}
