using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Code.Utilities
{
    public static class Executor
    {
        private static Queue<IRunnable> taskQueue;

        //Create an instance of the executor
        static Executor()
        {
            if (taskQueue == null)
            {
                taskQueue = new Queue<IRunnable>();
            }
            else
            {
                taskQueue.Clear();
            }
        }

        //Add a task
        public static void AddTask(IRunnable task)
        {
            taskQueue.Enqueue(task);
        }

        //Dequeue a task and returns it
        public static IRunnable Dequeue()
        {
            return taskQueue.Dequeue();
        }

        //Return the size of the queue
        public static int Size()
        {
            return taskQueue.Count;
        }

        //Execute all the tasks in the queue
        public static void Run()
        {
            while (taskQueue.Count > 0)
            {
                IRunnable t = Dequeue();
                t.Run();
            }
        }
    }
}