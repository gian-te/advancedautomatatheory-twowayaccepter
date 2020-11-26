using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoWayAccepter.Entities
{
    public class Diagnostics: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }

        }

        private string _currentStateName = "Current State:";

        public string CurrentStateName
        {
            get { return _currentStateName; }
            set { _currentStateName = value; NotifyPropertyChanged("CurrentStateName"); }
        }

        private string _currentSymbol = "Current Symbol:";

        public string CurrentSymbol
        {
            get { return _currentSymbol; }
            set { _currentSymbol = value; NotifyPropertyChanged("CurrentSymbol"); }
        }

        private string _possibleNextStates = "Possible Next States:";

        public string PossibleNextStates
        {
            get { return _possibleNextStates; }
            set { _possibleNextStates = value; NotifyPropertyChanged("PossibleNextStates"); }
        }

        private State _currentState;

        public State CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; NotifyPropertyChanged("CurrentState"); }
        }

    }
}
