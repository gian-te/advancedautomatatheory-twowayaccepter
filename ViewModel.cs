using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoWayAccepter.Entities;

namespace TwoWayAccepter
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _play;

        public bool Playing
        {
            get { return _play; }
            set { _play = value; NotifyPropertyChanged("Playing"); NotifyPropertyChanged("Stopped"); }
        }

        public bool Stopped { get { return !Playing; } }

        public ViewModel()
        {
            States = new ObservableCollection<State>();
            Diagnostics = new Diagnostics();
        }

        private ObservableCollection<State> _states;
        public ObservableCollection<State> States 
        {
            get { return _states; }
            set
            {
                _states = value;
                NotifyPropertyChanged("States");
            }
        }

        private Diagnostics diagnostics;

        public Diagnostics Diagnostics
        {
            get { return diagnostics; }
            set { diagnostics = value; NotifyPropertyChanged("Diagnostics"); }
        }


        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        private string _omega;

        public string Omega
        {
            get { return _omega; }
            set { _omega = value; NotifyPropertyChanged("Omega"); }
        }

        private string _initialState;

        public string InitialState
        {
            get { return _initialState; }
            set { _initialState = value; Diagnostics.CurrentStateName = _initialState; NotifyPropertyChanged("InitialState"); }
        }

 

    }
}
