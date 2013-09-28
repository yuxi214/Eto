using System;
using System.ComponentModel;

namespace Eto.Forms
{
	public partial interface IApplication : IInstanceWidget
	{
		void Run (string[] args);
		void Quit();
		
		void GetSystemActions(GenerateActionArgs args, bool addStandardItems);
		
		Key CommonModifier { get; }
		Key AlternateModifier { get; }
		
		void Open(string url);

		void Invoke (Action action);
		void AsyncInvoke (Action action);

		string BadgeLabel { get; set; }
	}

	public partial class Application : InstanceWidget
	{
		public static Application Instance { get; private set; }
		
		public event EventHandler<EventArgs> Initialized;
		public virtual void OnInitialized(EventArgs e)
		{
			if (Initialized != null) Initialized(this, e);
		}

		public const string TerminatingEvent = "Application.Terminating";

		event EventHandler<CancelEventArgs> terminating;

		public event EventHandler<CancelEventArgs> Terminating {
			add {
				HandleEvent (TerminatingEvent);
				terminating += value;
			}
			remove { terminating -= value; }
		}

		public virtual void OnTerminating (CancelEventArgs e)
		{
			if (terminating != null)
				terminating (this, e);
		}
		
		new IApplication Handler { get { return (IApplication)base.Handler; } }

		public Form MainForm { get; set; }
		
		public string Name { get; set; }

		public Application() : this(Generator.Detect)
		{
		}
		
		public Application(Generator g) : this(g, typeof(IApplication))
		{
		}
			
		protected Application(Generator g, Type type, bool initialize = true)
				: base(g, type, initialize)
		{
			Application.Instance = this;
			Generator.Initialize(g); // make everything use this by default
		}

		public virtual void Run(params string[] args)
		{
			Handler.Run(args);
		}

		[Obsolete("Use Invoke instead")]
		public virtual void InvokeOnMainThread (Action action)
		{
			Invoke (action);
		}

		public virtual void Invoke (Action action)
		{
			Handler.Invoke (action);
		}

		public virtual void AsyncInvoke (Action action)
		{
			Handler.AsyncInvoke (action);
		}
		
		public void Quit()
		{
			Handler.Quit();
		}
		
		public void Open(string url)
		{
			Handler.Open(url);
		}
		
		public Key CommonModifier
		{
			get { return Handler.CommonModifier; }
		}
		
		public Key AlternateModifier
		{
			get { return Handler.AlternateModifier; }
		}
		
		public virtual void GetSystemActions(GenerateActionArgs args, bool addStandardItems = false)
		{
			Handler.GetSystemActions(args, addStandardItems);
		}

		public string BadgeLabel
		{
			get { return Handler.BadgeLabel; }
			set { Handler.BadgeLabel = value; }
		}
	}
}
