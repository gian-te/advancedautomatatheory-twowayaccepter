﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwoWayAccepter.Entities;

namespace TwoWayAccepter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModel _viewModel;
        public int i { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Rebind();
            Init();

        }

        private void Init()
        {
            _viewModel.States.Add(new State() { StateName = "1", TransitionSymbol = "a", DestinationState = "2", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "1", TransitionSymbol = "b", DestinationState = "3", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "2", TransitionSymbol = "a", DestinationState = "1", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "2", TransitionSymbol = "b", DestinationState = "4", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "2", TransitionSymbol = "#", DestinationState = "ACCEPT", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "3", TransitionSymbol = "b", DestinationState = "1", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "3", TransitionSymbol = "a", DestinationState = "4", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "4", TransitionSymbol = "a", DestinationState = "3", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "4", TransitionSymbol = "b", DestinationState = "2", ScanDirection = "Right" });
            _viewModel.States.Add(new State() { StateName = "ACCEPT", TransitionSymbol = "", DestinationState = "", ScanDirection = "" });

            _viewModel.Omega = "#aaabbbb#";
            _viewModel.InitialState = "1";
            _viewModel.Diagnostics.CurrentSymbol = _viewModel.Omega[0].ToString();
        }

        private void Rebind()
        {
            i = 1;
            _viewModel = new ViewModel();
            this.DataContext = _viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.States.Add(new State() { StateName = "b", TransitionSymbol = "1", DestinationState = "a" });
        }

        /// <summary>
        /// Contains the meat of the automatic transition logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _viewModel.Playing = true;
            SetInitialState();

            Task.Run(() =>
            {
                for (; i < _viewModel.Omega.Length;)
                {
                    if (_viewModel.Stopped)
                    {
                        break;
                    }
                    HighlightCurrentStateInDatagrid();

                    var letter = _viewModel.Omega[i];
                    var symbol = letter.ToString();

                    UpdateCurrentSymbolLabel(symbol);
                    UpdateCurrentStateLabel();


                    if (IsAccepted() || IsRejected())
                    {
                        break;
                    }


                    EvaluateNextState(symbol);
                    UpdateProcessedSymbolLabel(_viewModel.Omega.Substring(1, i));
                    HighlightCurrentStateInDatagrid();
                    UpdateCurrentStateLabel();
                    UpdateNextPossibleStates();
                    IncrementOrDecrementOmegaIndex();
                    DisplayAcceptOrRejectMessage();
                    Thread.Sleep(200);
                }

                //DisplayAcceptOrRejectMessage();

                _viewModel.Diagnostics.CurrentStateName = _viewModel.Diagnostics.CurrentState.StateName;

                _viewModel.Playing = false;
            });
        }

        /// <summary>
        /// Manual Step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SetInitialState();
            if (i >= _viewModel.Omega.Length)
            {
                return;
            }

            var letter = _viewModel.Omega[i];
            var symbol = letter.ToString();

            UpdateCurrentSymbolLabel(symbol);

            UpdateCurrentStateLabel();

            if (IsAccepted() || IsRejected())
            {
                return;
            }

            EvaluateNextState(symbol);
            UpdateProcessedSymbolLabel(_viewModel.Omega.Substring(1, i));
            HighlightCurrentStateInDatagrid();
            UpdateCurrentStateLabel();
            UpdateNextPossibleStates();
            IncrementOrDecrementOmegaIndex();
            DisplayAcceptOrRejectMessage();
        }

        private void UpdateNextPossibleStates()
        {
            _viewModel.Diagnostics.PossibleNextStates = "Possible Next States: ";

            var nextPossibleStates = new List<State>();
            nextPossibleStates = _viewModel.States.Where(s => s.StateName == _viewModel.Diagnostics.CurrentState.StateName).ToList();
            
            for (int j = 0; j < nextPossibleStates.Count; j++)
            {
                _viewModel.Diagnostics.PossibleNextStates += nextPossibleStates[j].DestinationState;
                
                if (j >= 0 && j < (nextPossibleStates.Count - 1) && !string.IsNullOrEmpty(nextPossibleStates[j].DestinationState))
                {
                    _viewModel.Diagnostics.PossibleNextStates += ", ";
                }
            }
        }

        private void HighlightCurrentStateInDatagrid()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    var stateObj = _viewModel.States.Where(name => name.StateName == _viewModel.Diagnostics.CurrentState.StateName).FirstOrDefault();
                    var index = _viewModel.States.IndexOf(stateObj);
                    dgStates.SelectedIndex = index;
                    DataGridRow row = (DataGridRow)dgStates.ItemContainerGenerator.ContainerFromIndex(index);
                    row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                });
            }
            catch
            {
                // swallow UI exception, if any
            }
        }

        private bool IsAccepted()
        {
            return _viewModel.Diagnostics.CurrentState.StateName == "ACCEPT";
        }

        private void UpdateCurrentStateLabel()
        {
            _viewModel.Diagnostics.CurrentStateName = _viewModel.Diagnostics.CurrentState.StateName;
        }

        private void DisplayAcceptOrRejectMessage()
        {
            if (IsAccepted())
            {
                MessageBox.Show("Accepted.");
            }
            else if (IsRejected())
            {
                MessageBox.Show("Rejected.");
            }
        }

        private void SetInitialState()
        {
            if (_viewModel.Diagnostics.CurrentState == null)
            {
                _viewModel.Diagnostics.CurrentState = _viewModel.States.Where(name => name.StateName == _viewModel.InitialState).FirstOrDefault();
            }
        }


        private bool IsRejected()
        {
            return _viewModel.Diagnostics.CurrentState.StateName == "REJECT";
        }

        private void UpdateCurrentSymbolLabel(string symbol)
        {
            _viewModel.Diagnostics.CurrentSymbol = symbol;
        }

        private void UpdateProcessedSymbolLabel(string subStr)
        {
            _viewModel.Diagnostics.ProcessedSymbols = subStr;
        }

        private void IncrementOrDecrementOmegaIndex()
        {
            try
            {
                if (_viewModel.Diagnostics.CurrentState.ScanDirection.ToLower() == "right")
                {
                    i++;
                }
                else if (_viewModel.Diagnostics.CurrentState.ScanDirection.ToLower() == "left")
                {
                    i--;
                }
            }
            catch
            {
                // swallow
            }
        }

        private void EvaluateNextState(string symbol)
        {
            var nextPossibleStates = new List<State>();

            // todo: handle how to transition to next state if current state has many transitions.
            nextPossibleStates = _viewModel.States.Where(s => s.StateName == _viewModel.Diagnostics.CurrentState.StateName).ToList();
            var targetStateName = nextPossibleStates.Where(state => state.TransitionSymbol == symbol).Select(s => s.DestinationState).FirstOrDefault();
            State nextState = _viewModel.States.Where(s => s.StateName == targetStateName).FirstOrDefault();

            _viewModel.Diagnostics.CurrentState = nextState;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _viewModel.Playing = false;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Rebind();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            i = 1;
            _viewModel.Diagnostics.CurrentSymbol = _viewModel.Omega[0].ToString();
            _viewModel.Diagnostics.CurrentState = null;
            _viewModel.Diagnostics.ProcessedSymbols = "";
            SetInitialState();
            UpdateCurrentStateLabel();
            UpdateProcessedSymbolLabel("");

        }
    }
}
