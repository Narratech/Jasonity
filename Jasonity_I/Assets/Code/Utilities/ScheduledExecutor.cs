using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Utilities
{
    public class ScheduledExecutor : IExecutor
    {
        private Queue<IRunnable> tasks = new Queue<IRunnable>();
        private bool paused = false;

        public ScheduledExecutor()
        {

        }

        public ScheduledExecutor(Queue<IRunnable> tasks)
        {
            this.tasks = tasks;
        }

        public void AddTask(IRunnable task)
        {
            tasks.Enqueue(task);
        }

        public void Pause()
        {
            paused = true;
        }

        public void Restart()
        {
            paused = false;
        }

        public void Stop()
        {
            paused = true;
            tasks.Clear();
        }

        public void Start()
        {
            paused = true;

            while(tasks.Count > 0 && !paused)
            {
                IRunnable r = tasks.Peek();
                Execute(r);
                tasks.Dequeue();
            }
        }

        public void Execute(IRunnable r)
        {
            r.Run();
        }
    }
}
