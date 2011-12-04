/*
 * Worker.cs
 *
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/3/2011 10:43:54 PM
 *
 */

using System;

namespace CocoB.Rest.WindowsPhone.Core.Concurrency
{
    public abstract class Worker
    {
        public abstract void QueueJob(Action job);
    }
}
