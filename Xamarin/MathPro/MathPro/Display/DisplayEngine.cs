using System;
using System.Threading;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public class DisplayEngine
    {

    }
    public enum ViewPropeties
    {
        NoDivisors, NoRoots, NoDenominators, IsDivisor, IsDenominator, IsFirst
    }
    public sealed class SelectionCursor : RelativeLayout
    {
        #region Properties

        SelectedPieceType _mySelectedPieceType;

        public SelectedPieceType MySelectedPieceType
        {
            get { return _mySelectedPieceType; }
            set { _mySelectedPieceType = value; }
        }

        public int Index { get; set; }

        public bool Active { get; set; }

        #endregion Properties

        #region Constructor

        public SelectionCursor(SelectedPieceType type1, int index, Context context)
            :base(context)
        {
            Index = index;
            _mySelectedPieceType = type1;
            Visibility = ViewStates.Gone;
            Active = false;

            if (_mySelectedPieceType == SelectedPieceType.Power)
            {

                var relativeLayout = new RelativeLayout(context);
                var params1 = new LayoutParams(5, 10);
                LayoutParameters = params1;
                var params2 = new LayoutParams(4, 9) {TopMargin = -12};
	            relativeLayout.LayoutParameters = params2;
                relativeLayout.SetBackgroundResource(Resource.Drawable.Cursor);
                AddView(relativeLayout);
            }
            else if (_mySelectedPieceType == SelectedPieceType.brace)
            {
                var relativeLayout = new RelativeLayout(context);
                var params1 = new LayoutParams(15, 60);
                LayoutParameters = params1;
                var params2 = new LayoutParams(7, 20) {TopMargin = -54};
	            relativeLayout.LayoutParameters = params2;
                relativeLayout.SetBackgroundResource(Resource.Drawable.Cursor);
                AddView(relativeLayout);
            }
            else if (_mySelectedPieceType == SelectedPieceType.BaseExpo)
            {
                
                var relativeLayout = new RelativeLayout(context);
                var params1 = new LayoutParams(10, 80);
                LayoutParameters = params1;
                var inner = new RelativeLayout(context);
                var params2 = new LayoutParams(10, 43);
                inner.LayoutParameters = params2;
                inner.SetBackgroundResource(Resource.Drawable.Cursor);
                relativeLayout.AddView(inner);
                AddView(relativeLayout);
            }
            else
            {
                var params1 = new LayoutParams(5, 20);
                LayoutParameters = params1;
                var inner = new RelativeLayout(context);
                var params2 = new LayoutParams(5, 20);
                inner.LayoutParameters = params2;
                inner.SetBackgroundResource(Resource.Drawable.Cursor);
               AddView(inner);
            }
        }
        public SelectionCursor(int index, int widthOuter,
            int widthInner, int heightOuter, int heightInner, Thickness inner, Context context)
            :base(context)
        {
            Index = index;
            _mySelectedPieceType = SelectedPieceType.OutSide;// type1;
            Visibility = ViewStates.Gone;
            Active = false;

            var params1 = new LayoutParams(widthOuter, heightOuter);
            LayoutParameters = params1;
            var relativeLayout = new RelativeLayout(context);
            var params2 = new LayoutParams(widthInner, heightInner)
            {
	            TopMargin = inner.Top,
	            LeftMargin = inner.Left,
	            RightMargin = inner.Right,
	            BottomMargin = inner.Bottom
            };
	        relativeLayout.LayoutParameters = params2;
            relativeLayout.SetBackgroundResource(Resource.Drawable.Cursor);
            AddView(relativeLayout);
        }
        #endregion Constructor

        #region Methods

        #endregion Methods
    }

    public class Thickness
    {
        public int Left{ get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public Thickness()
        {
            
        }

        public Thickness(int left, int top, int right, int bottom)
        {
            
        }
    }



	public class DispatcherTimer
	{
		private readonly Timer _timer;
		public EventHandler Tick { get; set; }

		public DispatcherTimer()
		{
			_timer = new Timer(Callback, null, 500, Timeout.Infinite);
			
			//Tick = handle;
		}

		public void Callback(Object state)
		{
			if(Tick != null)
				Tick.Invoke(this,null);
			_timer.Change(500, Timeout.Infinite);
		}

	}
}