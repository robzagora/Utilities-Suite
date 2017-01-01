﻿namespace Zagorapps.Utilities.Suite.UI.Views.Organiser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Commands;
    using Controls;
    using Events;
    using Library.Attributes;
    using Services;
    using ViewModels;
    using Zagorapps.Core.Library.Events;
    using Zagorapps.Utilities.Library.Factories;
    using Zagorapps.Utilities.Library.Managers;
    using Zagorapps.Utilities.Library.Tasks;

    [DefaultNavigatable(Home.ViewName)]
    public partial class Home : ViewControlBase
    {
        private const string ViewName = nameof(Home);

        protected readonly ITaskManager Manager;
        protected readonly ISnackbarNotificationService Notifier;

        public Home(IOrganiserFactory factory, ICommandProvider commandProvider) 
            : base(Home.ViewName, factory, commandProvider)
        {
            this.InitializeComponent();
            
            this.Manager = this.Factory.Create<ITaskManager>();
            this.Notifier = this.Factory.Create<ISnackbarNotificationService>();

            this.DataContext = this;
        }

        public IEnumerable<TaskViewModel> Tasks
        {
            get
            {
                return this.Manager
                    .GetAll()
                    .Select(task => new TaskViewModel(task))
                    .ToArray();
            }
        }

        public override void InitialiseView(object arg)
        {
            EventArgs<ITask, bool> eventArgs = arg as EventArgs<ITask, bool>;

            if (eventArgs != null)
            {
                ITask task = eventArgs.First;
                bool immediateStart = eventArgs.Second;

                this.Manager.Add(task);

                if (immediateStart)
                {
                    this.Manager.RunTaskById(task.Identity);
                    
                    this.Notifier.Notify(string.Format(UiResources.Message_TaskAddedAndStarted, task.Identity));
                }
                else
                {
                    this.Notifier.Notify(string.Format(UiResources.Message_TaskAdded, task.Identity));
                }

                this.OnPropertyChanged(nameof(this.Tasks));
            }
        }

        public override void FinaliseView()
        {
            Console.WriteLine("Home - finalised");
        }

        protected void RunTask(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button runTask = sender as Button;

                string id = (runTask.DataContext as TaskViewModel).Identity;

                this.Notifier.Notify(string.Format(UiResources.Message_TaskInvoked, id));

                this.Manager.RunTaskById(Guid.Parse(id));
            }
        }

        protected void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Notifier.Notify(((ButtonBase)sender).Content.ToString());
        }

        protected void Button_AddTask_Click(object sender, RoutedEventArgs e)
        {
            this.OnViewChange(ViewBag.GetViewName<AddTask>());
        }

        protected void ViewTask_Click(object sender, RoutedEventArgs e)
        {
            TaskViewModel task = ((sender as Button).DataContext as TaskViewModel);

            this.OnViewChange(ViewBag.GetViewName<IndividualTask>(), task);
        }
        
        private void ConfirmDialog_OnConfirm(object sender, ConfirmDialogEventArgs e)
        {
            this.Manager.DeleteById(Guid.Parse(e.First));

            this.Notifier.Notify(string.Format(UiResources.Message_TaskDeleted, e.First));

            this.OnPropertyChanged(nameof(this.Tasks));
        }
    }
}