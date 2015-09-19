using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ServerViewModel.Annotations;

namespace ServerViewModel.Utils
{

        public class ClientActivity : INotifyPropertyChanged
        {
            private bool _active;
            public int Number { get; set; }

            public bool Active
            {
                get { return _active; }
                set
                {
                    _active = value;
                    OnPropertyChanged("Active");
                }
            }

            public ClientManager ClientManager { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
}
