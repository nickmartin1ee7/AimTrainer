using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AimTrainer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Random _rng = new();
        private readonly Stopwatch _sw = new();
        private int? _lastRedIndex;
        private string _timerCount;
        private List<TimeSpan> _reactionTimes = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }
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
                TimerCount = string.Empty;
                StartGame();
            });

            Stop = new RelayCommand(StopGame);

            CircleClick = new RelayCommand<object>(_ =>
            {
                _sw.Stop();
                _reactionTimes.Add(_sw.Elapsed);
                TimerCount = $"Average reaction time: {_reactionTimes.Average(x => x.TotalMilliseconds):N0} ms";
                StartGame();
            }, canExecute: btn => ((btn as Button)?.Content as Ellipse)?.Fill == Brushes.Red);

            RandomizeGrid = new RelayCommand(() =>
            {
                StopGame();

                int size = Cells.Count;
                int newSize;
                do
                {
                    newSize = GetNewCellSize();
                    newSize *= newSize;
                } while (size == newSize);

                Cells.Clear();

                for (int i = 0; i < newSize; i++)
                {
                    Cells.Add(Brushes.Transparent);
                }
            });

            int GetNewCellSize()
            {
                int size = _rng.Next(4, 16);
                if (size % 2 != 0) size++;
                return size;
            }
        }

        private void StartGame()
        {
            ResetCellBrushes();
            SetNewRed();
            _sw.Restart();
        }

        private void StopGame()
        {
            if (!_sw.IsRunning) return;
            _sw.Stop();
            _reactionTimes.Clear();
            _timerCount = string.Empty;
            ResetCellBrushes();
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
            while (true)
            {
                var rngIndex = _rng.Next(Cells.Count);

                if (_lastRedIndex == rngIndex)
                {
                    continue;
                }
                else
                {
                    Cells[rngIndex] = Brushes.Red;
                    _lastRedIndex = rngIndex;
                    break;
                }
            }
        }
    }
}
