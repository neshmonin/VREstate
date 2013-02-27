using System;
using System.Collections.Generic;
using Vre.Server.Task;
using System.Threading;

namespace Vre.Server.Command
{
    internal class TaskRunner : ICommand
    {
        private Dictionary<string, ITask> _tasks;

        public TaskRunner()
        {
            _tasks = new Dictionary<string, ITask>();
            addTask(new UpdateViewOrderTargets());
            addTask(new NotifyExpiringViewOrders());
            addTask(new RemoveStaleReverseRequests());
        }

        private void addTask(ITask task) { _tasks.Add(task.Name, task); }

        public string Name { get { return "runtask"; } }

        public void Execute(Parameters param)
        {
            string taskName = param.GetOption("taskname");
            if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentException("Must specify a task name.");

            ITask task;
            if (!_tasks.TryGetValue(taskName, out task)) throw new ArgumentException("Unknown task name.");

            param.RemoveKey("taskname");

            try
            {
                Thread.CurrentThread.Name = task.Name + "#" + Thread.CurrentThread.ManagedThreadId.ToString();
                ServiceInstances.Logger.Info("Running task...");
                task.Execute(param);
                ServiceInstances.Logger.Info("Task finished.");
            }
            catch (Exception e)
            {
                ServiceInstances.Logger.Error("Task failed: {0}", e);
                sendAdministrativeAlert("Task " + task.Name + ": run failed", Utilities.ExplodeException(e));
                throw new ApplicationException("Task " + task.Name + " run failed.", e);
            }
        }

        private void sendAdministrativeAlert(string subject, string text)
        {
            try
            {
                string recipient = ServiceInstances.Configuration.GetValue("AdminMessageRecipients", "admin@3dcondox.com");

                ServiceInstances.EmailSender.Send(recipient, subject, text);
            }
            catch (Exception e)
            {
                ServiceInstances.Logger.Error("Administrative alert was not send.\r\nTask: {0}\r\nSubject: {1}\r\nText: {2}\r\nError: {3}",
                    Name, subject, text, e);
            }
        }
    }
}