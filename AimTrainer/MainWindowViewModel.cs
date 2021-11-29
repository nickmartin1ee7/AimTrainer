using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;

namespace AimTrainer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Random _rng = new();
        private readonly Timer _timer = new(10);
        private string _timerCount;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand Start { get; set; }
        public ICommand RandomizeGrid { get; set; }
        public ICommand CircleClick { get; set; }

        public ObservableCollection<Brush> Cells { get; set; } = new ObservableCollection<Brush>();

        public string TimerCount
        {
            get { return _timerCount; }
            set
            {
                _timerCount = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimerCount)));
            }
        }

        public MainWindowViewModel()
        {
            Start = new RelayCommand(() =>
            {
                var start = DateTime.Now;

                _timer.Elapsed += (s, e) =>
                    TimerCount = e.SignalTime.Subtract(start).ToString();

                SetNewRed();

                _timer.Start();
            });

            CircleClick = new RelayCommand(() =>
            {
                if (!_timer.Enabled) return;
                _timer.Stop();
                ResetCellBrushes();
            });

            RandomizeGrid = new RelayCommand(() =>
            {
                var size = _rng.Next(4, 16);

                if (size % 2 != 0) size++;

                Cells.Clear();

                var l = size * size;
                for (int i = 0; i < l; i++)
                {
                    Cells.Add(Brushes.Transparent);
                }
            });
        }

        private void ResetCellBrushes()
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i] = Brushes.Transparent;
            }
        }

        private void SetNewRed()
        {
            var rngIndex = _rng.Next(Cells.Count);
            Cells.RemoveAt(rngIndex);
            Cells.Insert(rngIndex, Brushes.Red);
        }
    }
}
