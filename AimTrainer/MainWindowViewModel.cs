using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Input;
using System.Windows.Media;

namespace AimTrainer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Random _rng = new();
        private readonly Stopwatch _sw = new();
        private readonly SoundPlayer _clickRedSP;
        private readonly SoundPlayer _clickMenuSP;
        private int? _lastRedIndex;
        private string? _timerCount;
        private int _totalClicks;
        private double _accuracy;
        private int _correctClicks;
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

        public int TotalClicks
        {
            get { return _totalClicks; }
            set
            {
                _totalClicks = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TotalClicks)));
            }
        }

        public double Accuracy
        {
            get { return _accuracy; }
            set
            {
                _accuracy = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Accuracy)));
            }
        }

        public MainWindowViewModel()
        {
            Start = new RelayCommand(StartGame);
            Stop = new RelayCommand(StopGame);
            RandomizeGrid = new RelayCommand(RandomizeGameGrid);
            CircleClick = new RelayCommand<object>(fill => OnCircleClick(fill));


            _clickMenuSP = new(GetResourceStreamByName("clickMenu.wav"));
            _clickRedSP = new(GetResourceStreamByName("clickRed.wav"));
        }

        private Stream GetResourceStreamByName(string name)
        {
            var asm = GetType().Assembly;
            var resourceNames = asm.GetManifestResourceNames();
            var gnomeResName = resourceNames.First(r => r.Contains(name));
            return asm.GetManifestResourceStream(gnomeResName);
        }

        private void OnCircleClick(object fillBrush)
        {
            if (!_sw.IsRunning)
                return;

            TotalClicks++;

            if (fillBrush != Brushes.Red)
            {
                Accuracy = _correctClicks / (double)TotalClicks;
                return;
            }
            else
            {
                _clickRedSP.Play();
                _correctClicks++;
                Accuracy = _correctClicks / (double)TotalClicks;
            }

            _sw.Stop();
            _reactionTimes.Add(_sw.Elapsed);
            TimerCount = $"Average reaction time: {_reactionTimes.Average(x => x.TotalMilliseconds):N0} ms";
            StartNewRound();
        }

        private void RandomizeGameGrid()
        {
            int GetNewCellSize()
            {
                int size = _rng.Next(4, 16);
                if (size % 2 != 0) size++;
                return size;
            }

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
        }

        private void StartGame()
        {
            _clickMenuSP.Play();
            
            if (_sw.IsRunning)
                StopGame();

            TotalClicks = 0;
            _correctClicks = 0;
            Accuracy = 0;
            TimerCount = string.Empty;

            StartNewRound();
        }

        private void StartNewRound()
        {
            ResetCellBrushes();
            _sw.Restart();
            SetNewRed();
        }

        private void StopGame()
        {
            _clickMenuSP.Play();

            if (!_sw.IsRunning)
                return;

            _sw.Stop();
            _totalClicks = 0;
            _correctClicks = 0;
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
