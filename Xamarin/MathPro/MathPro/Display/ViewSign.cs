using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public sealed class ViewSign : RelativeLayout
    {
        #region Properties
		private readonly Helper _helper = new Helper();
        private readonly Context _context;
        List<SelectionCursor> _mySelect = new List<SelectionCursor>();

        public List<SelectionCursor> MySelect
        {
            get { return _mySelect; }
            set { _mySelect = value; }
        }

        List<ViewPropeties> _viewProperties = new List<ViewPropeties>();

        public List<ViewPropeties> ViewProperties
        {
            get { return _viewProperties; }
            set { _viewProperties = value; }
        }

        #endregion Properties

        #region Constructor

        public ViewSign(Sign sign, IEnumerable<ViewPropeties> prop, Context context)
            :base(context)
        {
            _context = context;
            _viewProperties = new List<ViewPropeties>(prop);
            var inner = new RelativeLayout(_context);

            if (sign.SignType == SignType.Or)
            {
                LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(30), _helper.Factor(20));
				inner.LayoutParameters = new LayoutParams(_helper.Factor(18), _helper.Factor(18)) 
				{ TopMargin = sign.GetTypeSign() == SignType.Subtract ? _helper.Factor(10) : _helper.Factor(5) };
            }
            else if ((NoDenominator() && NoDivisors(true)))
            {
                LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(20));
				inner.LayoutParameters = new LayoutParams(_helper.Factor(9), _helper.Factor(9)) 
				{ TopMargin = sign.GetTypeSign() == SignType.Subtract ? _helper.Factor(9) : _helper.Factor(5) };

            }
            else if (!NoDenominator() && NoDivisors(false))
            {
				LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(25));
				inner.LayoutParameters = new LayoutParams(_helper.Factor(15), _helper.Factor(17)) 
				{LeftMargin = 3,TopMargin = sign.GetTypeSign() == SignType.Subtract ? _helper.Factor(19) : _helper.Factor(18) };
                //LayoutParameters = new ViewGroup.LayoutParams(20, 120);
                //inner.LayoutParameters = new LayoutParams(9, 9) { TopMargin = 8};
            }
            inner.SetBackgroundResource(FixSign(sign));
            AddView(inner);
        }
        public ViewSign(Sign sign, IEnumerable<ViewPropeties> prop, ref DispatcherTimer timer, Context context)
            : base(context)
        {
            _context = context;
            _viewProperties = new List<ViewPropeties>(prop);
            var inner = new RelativeLayout(_context);

            if (sign.SignType == SignType.Or)
            {
                LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(30), _helper.Factor(20));
				var myParams = new LayoutParams(_helper.Factor(18), _helper.Factor(18))
				{LeftMargin = 3,TopMargin = sign.GetTypeSign() == SignType.Subtract ? _helper.Factor(9) : _helper.Factor(5) };

                inner.LayoutParameters = myParams;

            }
            else if ((NoDenominator() && NoDivisors(true)))
            {
                LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(20));
				inner.LayoutParameters = new LayoutParams(_helper.Factor(9), _helper.Factor(9)) 
				{ TopMargin = sign.GetTypeSign() == SignType.Subtract ? _helper.Factor(9) : _helper.Factor(5) };


                // _mySelect.Add(new SelectionCursor(-2,20,9,20,9, new Thickness(0, -12, 0, 0)));
                //AddView(_mySelect[_mySelect.Count - 1]);

            }
            else if (!NoDenominator() && NoDivisors(false))
            {
                LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(120));
                inner.LayoutParameters = new LayoutParams(_helper.Factor(9),_helper.Factor(9)){TopMargin = 18};
                //_mySelect.Add(new SelectionCursor(-2, 20, 9, 120, 9, new Thickness(0, -18, 0, 0),_context));
                //AddView(_mySelect[_mySelect.Count - 1]);

            }
            //Select(SelectedPieceType.OutSide, -2, ref timer);

            inner.SetBackgroundResource(FixSign(sign));
            AddView(inner);
        }
        public bool ShowNext()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                if (GetChildAt(i).Visibility != ViewStates.Gone || DontTouch(GetChildAt(i))) continue;
                GetChildAt(i).Visibility = ViewStates.Visible;
                return true;
            }
            return false;
        }
        public void HideAll()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                GetChildAt(i).Visibility = ViewStates.Gone;
            }
        }
        private bool DontTouch(View element)
        {
            try
            {
                var cor = (SelectionCursor)element;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public ViewSign(Brace brace, IEnumerable<ViewPropeties> prop, Context context)
            : base(context)
        {
            _viewProperties = new List<ViewPropeties>(prop);
            var inner = new RelativeLayout(_context);
            LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(10), _helper.Factor(50));
            inner.LayoutParameters = new LayoutParams(_helper.Factor(9), _helper.Factor(15)){TopMargin = -45};
            inner.SetBackgroundResource(brace.Key == ')' ? Resource.Drawable.closeBrace : Resource.Drawable.openBrace);
            AddView(inner);
        }

        #endregion Constructor

        #region Selection Functions

        public bool Select(SelectedPieceType type1, int index, ref DispatcherTimer timer)
        {
            var answer = false;
            DeSelect();

            foreach (var t in _mySelect.Where(t => t.MySelectedPieceType == type1 && t.Index == index))
            {
                t.Active = true;
                t.Visibility = ViewStates.Visible;
                //timer.Tick += new EventHandler(Toggle);
                answer = true;
                break;
            }
            return answer;
        }
        private void Toggle(object sender, EventArgs e)
        {
            foreach (var t in _mySelect.Where(t => t.Active))
            {
                t.GetChildAt(0).Visibility = t.GetChildAt(0).Visibility == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        public void DeSelect()
        {
            foreach (var t in _mySelect)
            {
                t.Active = false;
                t.Visibility = ViewStates.Gone;
            }
        }

        #endregion Selection Functions

        #region Method


        private int FixSign(Sign signType)
        {
            return _utility.FixSign(signType);
        }

        readonly TermUtility _utility = new TermUtility();
        private bool NoDenominator()
        {
            return ViewPropertyTrue(ViewPropeties.NoDenominators);
        }
        private bool ViewPropertyTrue(ViewPropeties property)
        {
            return _viewProperties.Where((t, i) => _viewProperties[i] == property).Any();
        }

        private bool NoDivisors(bool noDenominator)
        {
            return ViewPropertyTrue(ViewPropeties.NoDivisors);

        }
        #endregion Methods

    }
}