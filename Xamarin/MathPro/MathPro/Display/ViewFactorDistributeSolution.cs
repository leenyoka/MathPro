using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public sealed class ViewFactorDistributeSolution : ViewPlayerItem
    {
        #region Properties
		private readonly Helper _helper = new Helper();
        FactorDistributeSolution _sol;
        private readonly Context _context;

        #endregion Properties

        #region Constructor

        public ViewFactorDistributeSolution(FactorDistributeSolution sol,Context contex)
            :base(contex)
        {
            _context = contex;
            _sol = sol;

            foreach (var t in sol.Solution)
            {
                AddView(Step(t));
				AddView(SpaceWaster());
            }
        }

        #endregion Constructor

        #region Methods

        private LinearLayout Step(Expression exp)
        {
            return new Step(exp,_context);
        }

        #endregion Methods

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewFactorDistributeSolution;
        }
		private LinearLayout SpaceWaster()
		{
			var panel = new LinearLayout(_context) { LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(20)) };

			return panel;
		}
    }
}