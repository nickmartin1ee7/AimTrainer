using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AimTrainer
{
    public class MainWindowViewModel
    {
        private readonly MainWindow _window;

        public ICommand Start { get; set; }
        public ICommand RandomizeGrid { get; set; }

        public ObservableCollection<Brush> Cells { get; set; } = new ObservableCollection<Brush>();

        public MainWindowViewModel(MainWindow window)
        {
            _window = window;
            Start = new StartCommand(this, window);
            RandomizeGrid = new RandomizeGridCommand(this, window);
        }

        private class StartCommand : ICommand
        {
            private readonly Random _rng = new Random();
            private readonly MainWindowViewModel _vm;
            private readonly MainWindow _window;

            public StartCommand(MainWindowViewModel mainWindowViewModel, MainWindow window)
            {
                _vm = mainWindowViewModel;
                _window = window;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class RandomizeGridCommand : ICommand
        {
            private readonly Random _rng = new Random();
            private readonly MainWindowViewModel _vm;
            private readonly MainWindow _window;

            public RandomizeGridCommand(MainWindowViewModel mainWindowViewModel, MainWindow window)
            {
                _vm = mainWindowViewModel;
                _window = window;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                var size = _rng.Next(4, 16);

                if (size % 2 != 0) size++;

                _vm.Cells.Clear();

                var l = size * size;
                for (int i = 0; i < l; i++)
                {
                    _vm.Cells.Add(Brushes.Transparent);
                }
            }
        }
    }
}
