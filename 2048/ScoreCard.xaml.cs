using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _2048
{
    public sealed partial class ScoreCard : INotifyPropertyChanged
    {
        private int _score;

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged("Score");
                }
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public ScoreCard()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
