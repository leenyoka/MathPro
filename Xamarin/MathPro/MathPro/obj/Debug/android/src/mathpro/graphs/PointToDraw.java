package mathpro.graphs;


public class PointToDraw
	extends android.graphics.Point
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MathPro.Graphs.PointToDraw, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PointToDraw.class, __md_methods);
	}


	public PointToDraw () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PointToDraw.class)
			mono.android.TypeManager.Activate ("MathPro.Graphs.PointToDraw, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public PointToDraw (android.graphics.Point p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == PointToDraw.class)
			mono.android.TypeManager.Activate ("MathPro.Graphs.PointToDraw, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Graphics.Point, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public PointToDraw (int p0, int p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == PointToDraw.class)
			mono.android.TypeManager.Activate ("MathPro.Graphs.PointToDraw, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}

	public PointToDraw (android.graphics.Point p0, mathpro.graphs.SweetPoint p1) throws java.lang.Throwable
	{
		super ();
		if (getClass () == PointToDraw.class)
			mono.android.TypeManager.Activate ("MathPro.Graphs.PointToDraw, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Graphics.Point, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:MathPro.Graphs.SweetPoint, MathPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0, p1 });
	}

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
