using Android.Content;
using Android.Widget;

namespace MathPro.Display
{
    public abstract class ViewPlayerItem : LinearLayout
    {
        protected ViewPlayerItem(Context context)
            : base(context)
        {
            Orientation = Orientation.Vertical;
        }
        public virtual ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.None;
        }
        public virtual bool ShowNext()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                var step = (Step)GetChildAt(i);
                if (step.ShowNext()) return true;
            }
            return false;
        }
        public virtual void HideAll()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                var step = (Step)GetChildAt(i);
                step.HideAll();
            }
        }
    }
    public enum ViewPlayerItemType
    {
        ViewFactorDistributeSolution, ViewSolveForXSolution,
        ViewSimplificationSolution, ViewInequalitiesSolution,
        ViewExponentialSolution, ViewLogarithmicSolution, None
    }
}