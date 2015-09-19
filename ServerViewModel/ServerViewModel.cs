using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Command;
using ServerModel;
using ServerViewModel.Annotations;
using ServerViewModel.Utils;
using System.Windows;

namespace ServerViewModel
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        #region fields

        private ObservableCollection<ClientManager> _clientManagers;
        private RelayCommand _addNewClientCommand;
        private RelayCommand _checkConnectionCommand;
        private RelayCommand _deleteClientCommand;
        private RelayCommand _executeOnClientsCommand;
        private RelayCommand _executeOnServerCommand;
        private string _newClientIp;
        private int _newClientPort;
        private int _currPort;
        private int _currId;
        private XmlSerializer _serverXmlSerializer;
        public object Lock; 
        #endregion


        #region properties

        public ObservableCollection<ClientManager> ClientManagers
        {
            get
            {
                return _clientManagers;
            }
            set
            {
                _clientManagers = value;
            }
        }
        public ObservableCollection<ClientActivity> ClientActivities { get; set; } 

        public string NewClientIp
        {
            get { return _newClientIp; }
            set { _newClientIp = value; }
        }

        public int NewClientPort
        {
            get { return _newClientPort; }
            set { _newClientPort = value; }
        }

        public RelayCommand AddNewClientCommand
        {
            get { return _addNewClientCommand ?? (_addNewClientCommand = new RelayCommand(AddNewClient)); }
        }

        public ClientManager SelectedClient { get; set; }

        public RelayCommand DeleteClientCommand
        {
            get { return _deleteClientCommand ?? (_deleteClientCommand = new RelayCommand(DeleteClient)); }
        }

        public RelayCommand CheckConnectionCommand
        {
            get { return _checkConnectionCommand ?? (_checkConnectionCommand = new RelayCommand(CheckConnection)); }
        }


        #endregion


        #region methods

        public void AddNewClient()
        {
            var manager = new ClientManager(_currPort++, _currPort++, NewClientIp, NewClientPort);
            ClientManagers.Add(manager);
            ClientActivities.Add(new ClientActivity(){Active = false, Number = _currId++, ClientManager = manager});
            manager.ConnectToClient();
            System.Threading.Thread recievingThread = new Thread(new ThreadStart(manager.Recieve));
            manager.MessageRecievedEvent += GetDataFromManager;
            recievingThread.Start();
        }

        public void GetDataFromManager(object sender, EventArgs e)
        {
            Message message;
            var data = ((ClientManager) sender).RecievedData;
            using (TextReader reader = new StringReader(data))
            {
                message = (Message)(_serverXmlSerializer.Deserialize(reader));
            }

            MessageSolution(message, sender);
        }

        public void MessageSolution(Message message, object sender)
        {
            switch (message.MessageType)
            {
                    case MessageType.ConnectionCheckFromClient:
                    
                        var activity = ClientActivities.FirstOrDefault(t => t.ClientManager == (ClientManager) sender);
                        if (activity != null)
                            Application.Current.Dispatcher.BeginInvoke((Action)(() => { activity.Active = true; }));
      
                    
                    break;
            }
        }

        public void DeleteClient()
        {
            ClientActivities.Remove(ClientActivities.First(t => t.ClientManager == SelectedClient));
            ClientManagers.Remove(SelectedClient);
        }

        public ServerViewModel()
        {
            ClientManagers=new ObservableCollection<ClientManager>();
            ClientActivities = new ObservableCollection<ClientActivity>();
            
            _currId = 0;
            _currPort = 12300;
            _serverXmlSerializer = new XmlSerializer(typeof(Message),"nstu.ru");
            
            
            
        }

        public string FormMessage(MessageType type, string data)
        {
            var message = new Message(){MessageType = type};
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            _serverXmlSerializer.Serialize(writer, message);
            return builder.ToString();
        }

        public void Listen(ClientManager manager)
        {

            
        }

        public void CheckConnection()
        {
            var message = FormMessage(MessageType.ConnectionCheckFromServer, "");
            foreach (var activity in ClientActivities)
            {
                activity.Active = false;
            }
            foreach (var clientManager in ClientManagers)
            {
                clientManager.SendData(message);
            }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
