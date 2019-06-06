using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core.Common.Items
{
    public class ProgressBarModel : INotifyPropertyChanged
    {
        public ProgressBarModel()
        {
            Progress = 0.00;
        }
        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double _progress;
        public double Progress
        {
            get { return _progress;}
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
    }
}
