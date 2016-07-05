using UnityEngine;

public class ThreadedJob
{
	private bool m_IsDone = false;
	private object m_Handle = new object();
	private System.Threading.Thread m_Thread = null;
	public bool IsDone
	{
		get
		{
			bool tmp;
			lock (m_Handle)
			{
				tmp = m_IsDone;
			}
			return tmp;
		}
		set
		{
			lock (m_Handle)
			{
				m_IsDone = value;
			}
		}
	}
	
	public void Start()
	{
		m_Thread = new System.Threading.Thread(Run);
		m_Thread.Start();
	}
	
	public void Abort()
	{
		m_Thread.Abort();
	}
	
	protected virtual void OnRun() { }
	
	private void Run()
	{
		OnRun();
		IsDone = true;
	}
}
