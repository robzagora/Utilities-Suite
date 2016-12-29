﻿namespace Zagorapps.Utilities.Suite.UI.ViewModels
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using Controls;
    using Core.Library.Extensions;
    using Utilities.Library;

    public class BluetoothInteractionViewModel : ViewModelBase
    {
        private readonly ConcurrentDictionary<string, ConnectedClientViewModel> connectedClients;

        private Visibility progressBarVisibility = Visibility.Hidden,
            startServiceButtonVisibility = Visibility.Visible,
            contentVisibility = Visibility.Hidden;

        private LinkedList<string> serviceClientLogger = new LinkedList<string>();

        // TOOD: apply linked list logic to this too
        private StringBuilder serviceServerLogBuilder = new StringBuilder();

        private string serviceStartText;
        private string pin;

        private int clientLogLinesAdded = 0,
            clientLogMaxLines = 10;

        private ICommand serviceStartCommand;
        private bool serviceEnabled, serviceStartButtonEnabled, contentEnabled;

        private ConnectionType conenctionType;

        public BluetoothInteractionViewModel()
        {
            this.connectedClients = new ConcurrentDictionary<string, ConnectedClientViewModel>();

            this.ServiceButtonText = "Start Service";
            this.ContentEnabled = false;
            this.ServiceEnabled = false;
            this.ServiceButtonEnabled = true;
        }

        public void UpdateConnectionClientHeartbeat(string name, DateTime time)
        {
            this.connectedClients[name].NextHeartbeatTimestamp = time;
        }

        public TResult InvokeConnectedClientNotifyableAction<TResult>(Func<ConcurrentDictionary<string, ConnectedClientViewModel>, TResult> action)
        {
            return this.NotifyableAction(this.connectedClients, action, nameof(this.ConnectedClients));
        }

        public bool TryRemoveClient(string clientName)
        {
            ConnectedClientViewModel model;
            if (this.connectedClients.TryRemove(clientName, out model))
            {
                this.OnPropertyChanged(nameof(this.ConnectedClients));

                return true;
            }

            return false;
        }

        public void InvokeConnectedClientNotifyableAction(Action<ConcurrentDictionary<string, ConnectedClientViewModel>> action)
        {
            this.NotifyableAction(this.connectedClients, action, nameof(this.ConnectedClients));
        }

        public ConcurrentDictionary<string, ConnectedClientViewModel> ClientModels
        {
            get { return this.connectedClients; }
        }

        public IEnumerable<ConnectedClientViewModel> ConnectedClients
        {
            get { return this.ClientModels.Select(h => h.Value).ToArray(); }
        }

        public string ServiceClientLogConsole
        {
            get { return this.serviceClientLogger.Any() ? this.serviceClientLogger.Aggregate((a, b) => a + "\n" + b) : string.Empty; }
            set
            {
                if (this.clientLogLinesAdded == this.clientLogMaxLines)
                {
                    this.serviceClientLogger.RemoveFirst();
                }
                else
                {
                    this.clientLogLinesAdded++;
                }

                this.serviceClientLogger.AddLast(value);

                this.OnPropertyChanged(nameof(ServiceClientLogConsole));
            }
        }

        public string ServiceServerLogConsole
        {
            get { return this.serviceServerLogBuilder.ToString(); }
            set
            {
                this.serviceServerLogBuilder.AppendLine(value);

                this.OnPropertyChanged(nameof(ServiceServerLogConsole));
            }
        }

        public string Pin
        {
            get { return this.pin; }
            set { this.SetField(ref pin, value, nameof(this.Pin)); }
        }

        public bool ServiceEnabled
        {
            get { return this.serviceEnabled; }
            set { this.SetField(ref serviceEnabled, value, nameof(this.ServiceEnabled)); }
        }

        public bool ContentEnabled
        {
            get { return this.contentEnabled; }
            set { this.SetField(ref contentEnabled, value, nameof(this.ContentEnabled)); }
        }

        public bool ServiceButtonEnabled
        {
            get { return this.serviceStartButtonEnabled; }
            set { this.SetField(ref serviceStartButtonEnabled, value, nameof(this.ServiceButtonEnabled)); }
        }

        public string ServiceButtonText
        {
            get { return this.serviceStartText; }
            set { this.SetField(ref serviceStartText, value, nameof(this.ServiceButtonText)); }
        }

        public Visibility ContentVisibility
        {
            get { return this.contentVisibility; }
            set { this.SetField(ref contentVisibility, value, nameof(this.ContentVisibility)); }
        }

        public Visibility ProgressBarVisibility
        {
            get { return this.progressBarVisibility; }
            set { this.SetField(ref progressBarVisibility, value, nameof(this.ProgressBarVisibility)); }
        }

        public Visibility StartServiceButtonVisibility
        {
            get { return this.startServiceButtonVisibility; }
            set { this.SetFieldIfChanged(ref startServiceButtonVisibility, value, nameof(this.StartServiceButtonVisibility)); }
        }

        public ICommand ServiceStartCommand
        {
            get { return this.serviceStartCommand; }
            set { this.SetFieldIfChanged(ref serviceStartCommand, value, nameof(this.ServiceStartCommand)); }
        }

        public ConnectionType ConnectionType
        {
            get { return this.conenctionType; }
            set { this.SetFieldIfChanged(ref conenctionType, value, nameof(this.ConnectionType)); }
        }

        public IEnumerable<ConnectionType> ConnectionTypes
        {
            get { return this.GetValues<ConnectionType>(); }
        }
    }
}