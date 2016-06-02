package md53313e26110785687c6c932abb95bf65e;


public class GraphBuilder
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("MathPro.Graphs.GraphBuilder, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", GraphBuilder.class, __md_methods);
	}


	public GraphBuilder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == GraphBuilder.class)
			mono.android.TypeManager.Activate ("MathPro.Graphs.GraphBuilder, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
