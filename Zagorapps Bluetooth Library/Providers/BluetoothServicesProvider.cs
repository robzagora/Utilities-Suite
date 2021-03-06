﻿namespace Zagorapps.Bluetooth.Library.Providers
{
    using System;
    using Client;
    using Client.Models;
    using Core.Library.Timing;
    using Handlers;
    using InTheHand.Net;
    using InTheHand.Net.Bluetooth;
    using InTheHand.Net.Sockets;
    using Messaging;
    using Networking;

    public class BluetoothServicesProvider : IBluetoothServicesProvider
    {
        public bool IsBluetoothAvailable
        {
            get { return BluetoothRadio.PrimaryRadio != null; }
        }

        public BluetoothAddress LocalAddress
        {
            get { return BluetoothRadio.PrimaryRadio.LocalAddress; }
        }

        public string Name
        {
            get { return BluetoothRadio.PrimaryRadio.Name; }
        }

        public ISimpleBluetoothClientReceiver CreateReceiver(ConnectionSettings settings, IBluetoothServicesProvider provider)
        {
            return new SimpleBluetoothClientReceiver(settings, provider);
        }

        public IBluetoothClient CreateClient(BluetoothClient client)
        {
            return new BluetoothClientWrapper(client);
        }

        public IBluetoothListener CreateListener(Guid serviceID)
        {
            return new BluetoothListenerWrapper(serviceID);
        }

        public IBluetoothClient CreateServerClient()
        {
            return this.CreateClient(new BluetoothClient());
        }

        public IBluetoothConnectionHandler CreateConnectionHandler(
            IBluetoothClient client,
            IStreamProvider streamProvider,
            IMessageHandler<IMessage> messageHandler,
            ICommandOperationsProvider operationProvider,
            IMessageProvider messageProvider,
            ITimer timer)
        {
            return new BluetoothConnectionHandler(client, streamProvider, messageHandler, operationProvider, messageProvider, timer);
        }
    }
}