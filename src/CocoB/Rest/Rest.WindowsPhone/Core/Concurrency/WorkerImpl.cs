/*
 * WorkerImpl.cs
 * 
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/3/2011 10:45:30 PM
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace CocoB.Rest.WindowsPhone.Core.Concurrency
{
    public class WorkerImpl : Worker
    {
        private readonly string _name;

        #region Member Variables

        private readonly Queue<Action> _jobs = new Queue<Action>();
        private readonly object _syncLock = new object();
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Constructors

        public WorkerImpl(string name)
        {
            _name = name;
            _worker.DoWork += ProcessJobs;
            _worker.RunWorkerAsync();
        }

        #endregion

        #region Overrides of Worker

        public override void QueueJob(Action job)
        {
            lock (_syncLock)
            {
                _jobs.Enqueue(job);
                Monitor.Pulse(_syncLock);
            }
        }

        private void ProcessJobs(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = _name;
            while (true)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Queue<Action> localJobs;
                lock (_syncLock)
                {
                    while (_jobs.Count == 0)
                    {
                        Monitor.Wait(_syncLock);
                    }

                    localJobs = TransferJobs(_jobs);
                }

                ProcessLocalJobs(localJobs);
            }
        }

        private static Queue<Action> TransferJobs(Queue<Action> source)
        {
            var destination = new Queue<Action>();
            foreach (Action networkJob in source)
            {
                destination.Enqueue(networkJob);
            }
            source.Clear();

            return destination;
        }

        private static void ProcessLocalJobs(IEnumerable<Action> jobs)
        {
            foreach (Action networkJob in jobs)
            {
                networkJob();
            }
        }

        #endregion
    }
}