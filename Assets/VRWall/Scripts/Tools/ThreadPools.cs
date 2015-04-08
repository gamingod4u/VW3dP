using UnityEngine;
using System.Collections;
using System.Threading;

public class ThreadPools : MonoBehaviour 
{
	private static ThreadPools instance;

	public static ThreadPools Instance{get{return instance;}}
	// Use this for initialization
	void Start () 
	{
		if (instance == null) 
		{
			Init(128,0);
		}
	}
	public static bool Init(int queueSize, int threadNum)
	{
		if (instance != null) 
		{
			Debug.Log ("Thread already running");
			return false;
		}
		instance = new ThreadPools (queueSize, threadNum);
		return true;

	}

	public static void QueueUserWorkItem(WaitCallback callBack, object state)
	{
		instance.EnqueueTask (callBack, state);
	}

	private Thread[] threadPool;
	struct TaskInfo
	{
		public WaitCallback callBack;
		public object args;
	}
	private TaskInfo[] taskQueue;
	private int putPointer;
	private int getPointer;
	private int numTasks;
	private AutoResetEvent putNotify;
	private AutoResetEvent getNotify;
	private Semaphore semaphore;

	private ThreadPools(int queueSize, int threadNum)
	{
		threadNum = 1;

		if (threadNum == 0) {
			threadNum = SystemInfo.processorCount;
		}
		threadPool = new Thread[threadNum];
		taskQueue = new TaskInfo[queueSize];
		putPointer = 0;
		getPointer = 0;
		numTasks = 0;
		putNotify = new AutoResetEvent (false);
		getNotify = new AutoResetEvent (false);

		if (threadNum > 1) {
			semaphore = new Semaphore (0, queueSize);
			for (int i = 0; i < threadNum; i++) 
			{
				threadPool [i] = new Thread (ThreadFunc);
				threadPool [i].Start ();
			}
		}
		else 
		{
			threadPool[0] = new Thread(SingleThreadFunc);
			threadPool[0].Start();
		}
	}

	private void EnqueueTask(WaitCallback callBack, object state)
	{
		while(numTasks == taskQueue.Length)
		{
			getNotify.WaitOne();
		}
		taskQueue[putPointer].callBack = callBack;
		taskQueue[putPointer].args = state;
		putPointer++;

		if(putPointer == taskQueue.Length)
			putPointer = 0;

		if(threadPool.Length == 1)
		{
			if(Interlocked.Increment(ref numTasks) == 1)
				putNotify.Set();
		}
		else
		{
			Interlocked.Increment(ref numTasks);
			semaphore.Release();
		}
	}

	private void ThreadFunc()
	{
		for(;;)
		{
			int currPoint, nextPoint;
			do
			{
				currPoint = getPointer;
				nextPoint = currPoint+1;
				if(nextPoint == taskQueue.Length)
					nextPoint = 0;
			}
			while(Interlocked.CompareExchange(ref getPointer,nextPoint,currPoint)!= currPoint);
			
			TaskInfo task = taskQueue[currPoint];

			if(Interlocked.Decrement(ref numTasks) == taskQueue.Length-1)
				getNotify.Set();

			task.callBack(task.args);
		}
	}

	private void SingleThreadFunc()
	{
		for(;;)
		{
			while(numTasks == 0)
			{
				putNotify.WaitOne();
			}

			TaskInfo task = taskQueue[getPointer++];
			if(getPointer == taskQueue.Length)
			{
				getPointer = 0;
			}

			if(Interlocked.Decrement(ref numTasks) == taskQueue.Length - 1)
				getNotify.Set ();

			task.callBack(task.args);
		}
	}
}