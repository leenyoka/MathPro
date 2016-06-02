using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public class ViewExpression : LinearLayout
    {
        #region Properties
		private readonly Helper _helper = new Helper();
        private readonly Context _context;

        readonly List<SelectionCursor> _mySelect = new List<SelectionCursor>();

        List<ViewPropeties> _viewProperties = new List<ViewPropeties>();

        public List<ViewPropeties> ViewProperties
        {
            get { return _viewProperties; }
            set { _viewProperties = value; }
        }

        readonly Expression _exp;

        #endregion Properties

        #region Constructor

        public ViewExpression(Expression exp, ViewPropeties[] properties, Context context)
            :base(context)
        {
            _exp = exp;
            _context = context;
            if (properties != null && properties.Length > 0)
                _viewProperties.AddRange(properties);
            Prepare();
        }
        public ViewExpression(Expression exp, ViewPropeties[] properties, ref DispatcherTimer timer, Context context)
            :base(context)
        {
            _exp = exp;
            _context = context;
            if (properties != null && properties.Length > 0)
                _viewProperties.AddRange(properties);
            //timer.Tick += new EventHandler(Toggle);
            Prepare(ref timer);
        }

        #endregion Constructor

        #region Methods

        private void Prepare(ref DispatcherTimer timer)
        {
            bool noDims = NoDenominator();
            LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            Orientation = Orientation.Vertical;

            if (noDims && NoDivisors(true))
            {
                AddView(GetNumerator(true, true, ref timer));
                // no dims no divs
            }
            else if (!noDims && NoDivisors(false))
            {
                bool noDim = (_exp.Denominator == null || _exp.Denominator.Count == 0);
				AddView(GetNumerator(true, true, ref timer));
                if (_exp.Denominator != null && _exp.Denominator.Count > 0)
                {
					AddView(SpaceWaster());
                    AddView(GetDeviderLine());
					AddView(SpaceWaster());
                    AddView(GetDenominator(true, ref timer));
                }
            }
            else throw new NotImplementedException();

        }

	    private LinearLayout SpaceWaster()
	    {
			var panel = new LinearLayout(_context) {LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(1))};

		    return panel;
	    }

        private void Prepare()
        {
            var noDims = NoDenominator();
            LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            Orientation = Orientation.Vertical;

            if (noDims && NoDivisors(true))
            {
                
                AddView(GetNumerator(true, true));
                // no dims no divs
            }
            else if (!noDims && NoDivisors(false))
            {
				//LayoutParameters = new LayoutParams(_exp.Numerator.Count + 10, ViewGroup.LayoutParams.WrapContent);
                bool noDim = (_exp.Denominator == null || _exp.Denominator.Count == 0);
                AddView(GetNumerator(noDim, true));
                if (_exp.Denominator != null && _exp.Denominator.Count > 0)
                {
					
					
                    AddView(GetDeviderLine());
                    AddView(GetDenominator(true));
                }
            }
            else throw new NotImplementedException();

        }

        public bool ShowNext()
        {
           var panel = (LinearLayout)GetChildAt(0);
            for (var i = 0; i < panel.ChildCount; i++)
            {
                try
                {
                    var term = (ViewTerm)panel.GetChildAt(i);

					if (term.Inner.ShowNext()) return true;
                }
                catch
                {
                   var grid = (RelativeLayout)panel.GetChildAt(i);
                    if (grid.Visibility == ViewStates.Gone)
                    {
                        grid.Visibility = ViewStates.Visible;
                        return true;
                    }
                }
            }

            if (ChildCount == 3)
            {
               var panel2 = (LinearLayout)GetChildAt(2);
                for (var i = 0; i < panel2.ChildCount; i++)
                {
                    try
                    {
                        var term = (ViewTerm)panel2.GetChildAt(i);

						if (term.Inner.ShowNext()) return true;
                    }
                    catch
                    {
                       var grid = (RelativeLayout)panel2.GetChildAt(i);
                        if (grid.Visibility == ViewStates.Gone)
                        {
                            grid.Visibility = ViewStates.Visible;
                            return true;
                        }
                    }
                }
            }

            return false;

        }
        public bool HidAll()
        {
           var panel = (LinearLayout)GetChildAt(0);
            for (var i = 0; i < panel.ChildCount; i++)
            {
                try
                {
                    var term = (ViewTerm)panel.GetChildAt(i);
                    term.Inner.HideAll();
                }
                catch
                {
                   var grid = (RelativeLayout)panel.GetChildAt(i);
                    grid.Visibility = ViewStates.Gone;
                }
            }

            if (ChildCount == 3)
            {
               var panel2 = (LinearLayout)GetChildAt(2);
                for (var i = 0; i < panel2.ChildCount; i++)
                {
                    try
                    {
                        var term = (ViewTerm)panel2.GetChildAt(i);
						term.Inner.HideAll();
                    }
                    catch
                    {
                       var grid = (RelativeLayout)panel2.GetChildAt(i);
                        grid.Visibility = ViewStates.Gone;
                    }
                }
            }

            return false;

        }
        // 
        private LinearLayout  GetNumerator(bool noDims, bool noDivs, ref DispatcherTimer timer)
        {
           var panel = new LinearLayout(_context)  ;


            var prop = new List<ViewPropeties>();
            panel.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            if (noDims && noDivs)
            {
                prop = new List<ViewPropeties>(
                new[] { ViewPropeties.NoDenominators, ViewPropeties.NoDivisors });
            }
            else if (!noDims && noDivs)
            {
                prop = new List<ViewPropeties>(
                new[] { ViewPropeties.NoDivisors });
            }

            for (var i = 0; i < _exp.Numerator.Count; i++)
            {
                if (_exp.Numerator[i] != null && _exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    var propCurr = new List<ViewPropeties>(prop);

                    if (IsFirst(i, true))
                        propCurr.Add(ViewPropeties.IsFirst);
                    _myTerms.Add(new ViewTerm((Term)_exp.Numerator[i], propCurr.ToArray(), ref timer,_context));
                    panel.AddView(_myTerms[_myTerms.Count - 1]);
                }
                else if (_exp.Numerator[i] != null && _exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                   var grid = new RelativeLayout(_context)  ;
                   var inner = new RelativeLayout(_context)  ;
                    if (noDims && noDivs)
                    {
                        grid.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(15), _helper.Factor(30));
                        inner.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(9), _helper.Factor(25));
                        //inner.Margin = new Thickness(0, -45, 0, 0);
                    }
                    else if (!noDims && noDivs)
                    {
                        grid.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(30), _helper.Factor(30));
                        inner.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(9), _helper.Factor(25));
                        //inner.Margin = new Thickness(0, -45, 0, 0);
                    }
                    inner.SetBackgroundResource(((Brace) _exp.Numerator[i]).Key == ')'
                        ? Resource.Drawable.closeBrace
                        : Resource.Drawable.openBrace);
                    grid.AddView(inner);
                    panel.AddView(grid);

                    if (((Brace)_exp.Numerator[i]).Selected)
                    {
                        _mySelect.Add(new SelectionCursor(SelectedPieceType.brace, 0,_context));
                        _mySelect[_mySelect.Count - 1].Active = true;
                        panel.AddView(_mySelect[_mySelect.Count - 1]);
                    }
                }
                //else throw new NotImplementedException();
            }


            return panel;
        }
        private LinearLayout  GetNumerator(bool noDims, bool noDivs)
        {
           var panel = new LinearLayout(_context)  ;


            var prop = new List<ViewPropeties>();
            panel.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            panel.Orientation = Orientation.Horizontal;

            if (noDims && noDivs)
            {
                prop = new List<ViewPropeties>(
                new[] { ViewPropeties.NoDenominators, ViewPropeties.NoDivisors });

            }
            else if (!noDims && noDivs)
            {
                prop = new List<ViewPropeties>(
                new[] { ViewPropeties.NoDivisors });

            }

            for (var i = 0; i < _exp.Numerator.Count; i++)
            {
                switch (_exp.Numerator[i].GetTypePiece())
                {
                    case ExpressionPieceType.Term:
                    {
                        var propCurr = new List<ViewPropeties>(prop);

                        if (IsFirst(i, true))
                            propCurr.Add(ViewPropeties.IsFirst);
                        _myTerms.Add(new ViewTerm(
                            (Term)_exp.Numerator[i], propCurr.ToArray(),_context));
                        panel.AddView(_myTerms[_myTerms.Count - 1]);
                    }
                        break;
                    case ExpressionPieceType.Brace:
                    {
                        var grid = new RelativeLayout(_context)  ;
                        var inner = new RelativeLayout(_context)  ;
                        if (noDims && noDivs)
                        {
                            grid.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(15),_helper.Factor( 30));
                            inner.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(9), _helper.Factor(20));
                        }
                        else if (!noDims && noDivs)
                        {
                            grid.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(30), _helper.Factor(30));
                            inner.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(9), _helper.Factor(20));
                        }
                        inner.SetBackgroundResource(((Brace) _exp.Numerator[i]).Key == ')'
                            ? Resource.Drawable.closeBrace
                            : Resource.Drawable.openBrace);
                        grid.AddView(inner);
                        panel.AddView(grid);

                        if (!((Brace) _exp.Numerator[i]).Selected) continue;
                        _mySelect.Add(new SelectionCursor(SelectedPieceType.brace, i,_context));
                        panel.AddView(_mySelect[_mySelect.Count - 1]);
                    }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }


            return panel;
        }
        //index - yi term yesingaphi le okanye yi brace yesingaphi le....
        //innerIndex - the silected index on the actual piece(coe, termBase, power
        public bool Select(int index, int innerIndex, SelectedPieceType type1, ref DispatcherTimer timer, bool numerator)
        {
            var answer = false;
            //timer.Tick += new EventHandler(Toggle);
            DeSelect();

            if (type1 == SelectedPieceType.brace)
            {
                foreach (SelectionCursor t in _mySelect)
                {
                    if (t.MySelectedPieceType != type1 || t.Index != index) continue;
                    t.Active = true;
                    //_mySelect[i].Visibility = ViewStates.Visible;
                    //timer.Tick += new EventHandler(Toggle);
                    answer = true;
                    break;
                }

            }
            else
            {
                for (var i = 0; i < _myTerms.Count; i++)
                {
                    if (i != index) continue;
					_myTerms[i].Inner.Select(type1, innerIndex, ref timer);
                    break;
                }
            }
            return answer;
        }

        readonly List<ViewTerm> _myTerms = new List<ViewTerm>();
        private void Toggle(object sender, EventArgs e)
        {
            foreach (var t in _mySelect.Where(t => t.Active))
            {
                t.Visibility = t.Visibility == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        private void DeSelect()
        {
            foreach (var t in _mySelect)
            {
                t.Active = false;
                t.Visibility = ViewStates.Gone;
            }
            foreach (var t in _myTerms)
            {
				t.Inner.DeSelect();
            }
        }

        private RelativeLayout GetDeviderLine()
        {
           var grid = new RelativeLayout(_context)
           {
               LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, _helper.Factor(2))
           };

            grid.SetBackgroundResource(Resource.Drawable.Line);//.Background = GetBackGround("Line");
            return grid;
        }
        private LinearLayout  GetDenominator(bool noDivs, ref DispatcherTimer timer)
        {
           var panel = new LinearLayout(_context)  ;


            var prop = new List<ViewPropeties>();
			panel.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor((_exp.Denominator.Count * 40)), ViewGroup.LayoutParams.WrapContent);
			//panel.Orientation = Orientation.Vertical;

            if (noDivs)
            {
                prop = new List<ViewPropeties>(
                new[] { ViewPropeties.NoDivisors });
            }

            for (var i = 0; i < _exp.Denominator.Count; i++)
            {
                switch (_exp.Denominator[i].GetTypePiece())
                {
                    case ExpressionPieceType.Term:
                    {
                        var propCurr = new List<ViewPropeties>(prop);
                        if (IsFirst(i, false))
                            propCurr.Add(ViewPropeties.IsFirst);
                        panel.AddView(new ViewTerm((Term)_exp.Denominator[i], propCurr.ToArray(), ref timer,_context));
                    }
                        break;
                    case ExpressionPieceType.Brace:
                    {
                        var grid = new RelativeLayout(_context)  ;
                        var inner = new RelativeLayout(_context)  ;
                        if (noDivs)
                        {
                            grid.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(30), _helper.Factor(30));
                            inner.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(9), _helper.Factor(20));
                        }
                        inner.SetBackgroundResource(((Brace) _exp.Denominator[i]).Key == ')'
                            ? Resource.Drawable.closeBrace
                            : Resource.Drawable.openBrace);
                        grid.AddView(inner);
                        panel.AddView(grid);
                    }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }


            return panel;
        }
        private LinearLayout  GetDenominator(bool noDivs)
        {
           var panel = new LinearLayout(_context);


            var prop = new List<ViewPropeties>();
            panel.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            if (noDivs)
            {
                prop = new List<ViewPropeties>(
                new[] { ViewPropeties.NoDivisors });
                //panel.Margin = new Thickness(0, 51, 0, 0);
            }

            for (var i = 0; i < _exp.Denominator.Count; i++)
            {
                switch (_exp.Denominator[i].GetTypePiece())
                {
                    case ExpressionPieceType.Term:
                    {
                        var propCurr = new List<ViewPropeties>(prop);
                        if (IsFirst(i, false))
                            propCurr.Add(ViewPropeties.IsFirst);
                        panel.AddView(new ViewTerm((Term)_exp.Denominator[i], propCurr.ToArray(),_context));
                    }
                        break;
                    case ExpressionPieceType.Brace:
                    {
                        var grid = new RelativeLayout(_context)  ;
                        var inner = new RelativeLayout(_context)  ;
                        if (noDivs)
                        {
                            grid.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(30), _helper.Factor(30));
                            inner.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(9), _helper.Factor(20));
                        }
                        inner.SetBackgroundResource(((Brace) _exp.Denominator[i]).Key == ')'
                            ? Resource.Drawable.closeBrace
                            : Resource.Drawable.openBrace);
                        grid.AddView(inner);
                        panel.AddView(grid);
                    }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }


            return panel;
        }
        private bool IsFirst(int index, bool top)
        {

            if (index == 0)
                return true;
            if (top)
            {
                if (_exp.Numerator[index].GetTypePiece() == ExpressionPieceType.Term
                    && (_exp.Numerator[index - 1].GetTypePiece() == ExpressionPieceType.Sign
                        || (_exp.Numerator[index - 1].GetTypePiece() == ExpressionPieceType.Brace
                            && ((Brace)_exp.Numerator[index - 1]).Key == '(')))
                    return true;
            }
            else
            {
                if (_exp.Denominator[index].GetTypePiece() == ExpressionPieceType.Term
                    && _exp.Denominator[index - 1].GetTypePiece() == ExpressionPieceType.Sign
                    || (_exp.Denominator[index - 1].GetTypePiece() == ExpressionPieceType.Brace
                        && ((Brace)_exp.Denominator[index - 1]).Key == '('))
                    return true;
            }
            return false;
        }

        private bool NoDenominator()
        {
            //return exp.NoDenominator();
            return ViewPropertyTrue(ViewPropeties.NoDenominators);
        }
        private bool ViewPropertyTrue(ViewPropeties property)
        {
            return _viewProperties.Any(t => t == property);
        }

        private bool NoDivisors(bool noDenominator)
        {
            return ViewPropertyTrue(ViewPropeties.NoDivisors);
            //List<IExpressionPiece> pieces = new List<IExpressionPiece>
            //    (new List<IExpressionPiece>(exp.Numerator));

            //if (!noDenominator)
            //    pieces.AddRange(new List<IExpressionPiece>(exp.Denominator));

            //for (int i = 0; i < pieces.Count; i++)
            //{
            //    if (pieces[i].GetTypePiece() == ExpressionPieceType.Term
            //        && ((Term)pieces[i]).Devisor != null)
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }

 

        #endregion Methods
    }
}