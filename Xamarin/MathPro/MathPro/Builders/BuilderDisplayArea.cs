using Android.Content;
using Android.Views;
using Android.Widget;

namespace MathPro.Builders
{
	public sealed class BuilderDisplayArea:HorizontalScrollView
	{
		public BuilderDisplayArea(Context context, View child)
			:base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			AddView(child);
		}
	}
}