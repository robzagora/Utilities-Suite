﻿namespace Zagorapps.Utilities.Suite.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using Commands;
    using Navigation;
    using Services;
    using Suites;
    using Views;
    using Zagorapps.Core.Library.Events;
    using Zagorapps.Organiser.Library.Factories;

    public abstract class MainWindowBase : Window, IMainWindow
    {
        protected readonly IOrganiserFactory Factory;
        protected readonly ISuiteManager SuiteManager;
        protected readonly ISnackbarNotificationService Notifier;

        protected MainWindowBase(IOrganiserFactory factory)
        {
            this.Factory = factory;

            this.Notifier = this.Factory.Create<ISnackbarNotificationService>();

            ICommandProvider provider = this.Factory.Create<ICommandProvider>();

            //this.Navigator = this.Factory.Create<IViewNavigator>();

            //// TODO: these should be bindings - figure out how to register a collection in simple injector the way i need it to
            IEnumerable<IViewControl> controls = new List<IViewControl>
            {
                new Home(this.Factory, provider),
                new AddTask(this.Factory, provider),
                new IndividualTask(this.Factory, provider)
            };

            IEnumerable<IViewControl> tempControls = new List<IViewControl>
            {
                new TempControl(this.Factory, provider),
                new TempControl2(this.Factory, provider)
            };

            this.SuiteManager = new SuiteManager(new List<ISuite>
            {
                new FileOrganiserSuite(controls),
                new TempSuite(tempControls)
            });

            this.SuiteManager.OnSuiteChanged += SuiteManager_OnSuiteChanged;
            this.SuiteManager.OnSuiteViewChanged += SuiteManager_OnSuiteViewChanged;
        }

        private void SuiteManager_OnSuiteViewChanged(object sender, EventArgs<IViewControl, object> e)
        {
            this.OnPropertyChanged(nameof(this.ActiveView));
        }

        private void SuiteManager_OnSuiteChanged(object sender, EventArgs<ISuite, object> e)
        {
            this.OnPropertyChanged(nameof(this.ActiveView));

            this.Notifier.Notify("'" + e.First.Identifier + "' suite loaded.");
        }

        public IViewControl ActiveView
        {
            get { return this.SuiteManager.ActiveSuiteView; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object TryRetrieveResource(string name)
        {
            return this.TryFindResource(name);
        }

        public virtual void ShowWindow()
        {
            this.WindowState = WindowState.Normal;

            this.Show();
            this.BringIntoView();
        }

        public virtual void CloseWindow()
        {
            this.Close();
        }

        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void TerminateApplication(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}