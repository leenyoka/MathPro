package md5b6766f5b397001a7818161c3ca2f913a;


public class SolutionPlayer
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("MathPro.SolutionPlayer, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SolutionPlayer.class, __md_methods);
	}


	public SolutionPlayer () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SolutionPlayer.class)
			mono.android.TypeManager.Activate ("MathPro.SolutionPlayer, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
