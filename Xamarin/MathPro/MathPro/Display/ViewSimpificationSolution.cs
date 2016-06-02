using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public sealed class ViewSimpificationSolution : ViewPlayerItem
    {
        #region Properties
		private readonly Helper _helper = new Helper();
	    private readonly Context _context;

        #endregion Properties
        #region Constructor

        public ViewSimpificationSolution(SimpificationEquation eq,Context context)
            :base(context)
        {
	        _context = context;
	        foreach (var t in eq.Solution)
	        {
		        AddView(new Step(t, context));
		        AddView(SpaceWaster());
	        }
        }

        #endregion Constructor

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewSimplificationSolution;
        }
		private LinearLayout SpaceWaster()
		{
			var panel = new LinearLayout(_context) { LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(20)) };

			return panel;
		}
    }
}