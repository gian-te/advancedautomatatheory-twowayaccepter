using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoWayAccepter.Entities
{
    public class State: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /*
         <DataGridTextColumn Header="State Name" Binding="{Binding StateName}"/>
                <DataGridTextColumn Header="Transition Symbol" Binding="{Binding TransitionSymbol}" />
                <DataGridTextColumn Header="Destination State" Binding="{Binding DestinationState}" />
         */

        private string _stateName;

        public string StateName
        {
            get { return _stateName; }
            set { _stateName = value; NotifyPropertyChanged("StateName"); }
        }

        private string _transitionSymbol;

        public string TransitionSymbol
        {
            get { return _transitionSymbol; }
            set { _transitionSymbol = value; NotifyPropertyChanged("TransitionSymbol"); }
        }
        private string _destinationState;

        public string DestinationState
        {
            get { return _destinationState; }
            set { _destinationState = value; NotifyPropertyChanged("DestinationState"); }
        }


        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        private string _direction;

        public string ScanDirection
        {
            get { return _direction; }
            set { _direction = value; NotifyPropertyChanged("Direction"); }
        }

    }
}
