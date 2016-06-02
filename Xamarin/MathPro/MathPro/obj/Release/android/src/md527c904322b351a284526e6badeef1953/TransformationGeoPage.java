package md527c904322b351a284526e6badeef1953;


public class TransformationGeoPage
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_getRequestedOrientation:()I:GetGetRequestedOrientationHandler\n" +
			"n_setRequestedOrientation:(I)V:GetSetRequestedOrientation_IHandler\n" +
			"";
		mono.android.Runtime.register ("MathPro.Geometry.TransformationGeoPage, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", TransformationGeoPage.class, __md_methods);
	}


	public TransformationGeoPage () throws java.lang.Throwable
	{
		super ();
		if (getClass () == TransformationGeoPage.class)
			mono.android.TypeManager.Activate ("MathPro.Geometry.TransformationGeoPage, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public int getRequestedOrientation ()
	{
		return n_getRequestedOrientation ();
	}

	private native int n_getRequestedOrientation ();


	public void setRequestedOrientation (int p0)
	{
		n_setRequestedOrientation (p0);
	}

	private native void n_setRequestedOrientation (int p0);

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
