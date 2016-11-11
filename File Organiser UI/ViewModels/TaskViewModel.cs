﻿namespace File.Organiser.UI.ViewModels
{
    using System;
    using EyssyApps.Core.Library.Events;
    using EyssyApps.Organiser.Library;
    using EyssyApps.Organiser.Library.Tasks;
    using EyssyApps.UI.Library.Controls;

    public class TaskViewModel : ViewModelBase
    {
        protected readonly ITask Task;

        public TaskViewModel(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "No Task provided");
            }

            this.Task = task;
            this.Task.StateChanged += Task_StateChanged;
        }

        private void Task_StateChanged(object sender, EventArgs<TaskState> e)
        {
            this.State = e.First;
            this.OnPropertyChanged(nameof(this.State));
        }

        public string ID { get; set; }

        public TaskType TaskType { get; set; }

        public TaskState State { get; set; }

        public string Description { get; set; }
    }
}