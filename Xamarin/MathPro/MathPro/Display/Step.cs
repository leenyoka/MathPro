using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public sealed class Step : LinearLayout
    {
        #region Properties

        private readonly Context _context;
	    private readonly Helper _helper = new Helper();

        #endregion Properties

        #region Constructor

        public Step(Expression exp, Context context)
            :base(context)
        {
	        
            _context = context;
            if (NoDenominators(new List<iEquationPiece>(new iEquationPiece[] { exp }))
                && NoDivisors(new List<iEquationPiece>(new iEquationPiece[] { exp })))
            {
				LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

                AddView(new ViewExpression(exp,new[] { ViewPropeties.NoDivisors, ViewPropeties.NoDenominators },_context));
            }
            else throw new NotImplementedException();
        }
        public Step(Equation eq, Context context)
            : base(context)
        {
			
            _context = context;
            DoEquation(eq, false);
        }
        public Step(IEnumerable<iEquationPiece> pieces, Context context)
            : base(context)
        {
			
            _context = context;
            DoEquation(pieces);
        }
        public Step(LogarithmicEquation eq, Context context)
            : base(context)
        {
		
            _context = context;
            LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(411), _helper.Factor(50));

            foreach (var t in eq.Left)
            {
                switch (t.GetType1())
                {
                    case MathBase.Type.Log:
                        AddView(new ViewLogTerm((LogTerm)t, _context));
                        break;
                    case MathBase.Type.Sign:
                        AddView(LogSign((Sign)t));
                        break;
                    case MathBase.Type.Term:
                        AddView(GeTermForLogEq((Term)t));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (eq.IsComplete)
            {
                AddView(LogSign(new Sign("=")));

                foreach (var t in eq.Right)
                {
                    switch (t.GetType1())
                    {
                        case MathBase.Type.Log:
                            AddView(new ViewLogTerm((LogTerm)t,_context));
                            break;
                        case MathBase.Type.Sign:
                            AddView(LogSign((Sign)t));
                            break;
                        case MathBase.Type.Term:
                            AddView(GeTermForLogEq((Term)t));
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
        public Step(LogarithmicEquation eq, ref DispatcherTimer timer, Context context)
            : base(context)
        {
		
            _context = context;
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            foreach (var t in eq.Left)
            {
                switch (t.GetType1())
                {
                    case MathBase.Type.Log:
                        AddView(new ViewLogTerm((LogTerm)t, ref timer,_context));
                        break;
                    case MathBase.Type.Sign:
                        AddView(LogSign((Sign)t));
                        break;
                    case MathBase.Type.Term:
                        AddView(GeTermForLogEq((Term)t));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (!eq.IsComplete) return;
            AddView(LogSign(new Sign("=")));

            foreach (var t in eq.Right)
            {
                switch (t.GetType1())
                {
                    case MathBase.Type.Log:
                        AddView(new ViewLogTerm((LogTerm)t, ref timer,_context));
                        break;
                    case MathBase.Type.Sign:
                        AddView(LogSign((Sign)t));
                        break;
                    case MathBase.Type.Term:
                        AddView(GeTermForLogEq((Term)t));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public Step(Equation eq, bool expEquation, Context context)
            : base(context)
        {
			
            _context = context;
            DoEquation(eq, expEquation);
        }
        public Step(Equation eq, ref DispatcherTimer timer, Context context)
            : base(context)
        {
			
            _context = context;
            DoEquation(eq, ref timer);
        }
        public Step(Equation oneEq, Equation twoEq, Context context)
            : base(context)
        {
			
            _context = context;
            DoEquation(oneEq, false);
            AddView(new ViewSign(new Sign(SignType.Or), new List<ViewPropeties>(),_context));
            DoEquation(twoEq, false);
        }
        public Step(List<ISimplificationPiece> eq, Context context)
            : base(context)
        {
		
            _context = context;
            var prop = new List<ViewPropeties>();
            var all = new List<ISimplificationPiece>(eq);
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            if (NoDenominators(eq) && NoDivisors(eq))
            {
				LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!NoDenominators(eq) && NoDivisors(eq))
            {
				LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            foreach (var t in all)
            {
                if (t.GetSimplificationType() == SimplificationPieceType.Expression)
                    AddView(new ViewExpression(((Expression)t), prop.ToArray(),_context));
                else if (t.GetSimplificationType() == SimplificationPieceType.Sign
                         && ((Sign)t).GetTypePiece() == ExpressionPieceType.Sign)
                    AddView(new ViewSign((Sign)t, new List<ViewPropeties>(prop),_context));
                else
                    AddView(new ViewSign(brace: (Brace)t, prop: new List<ViewPropeties>(prop), context: _context));
            }
        }
        public Step(List<ISimplificationPiece> eq, ref DispatcherTimer timer, Context context)
            : base(context)
        {
			
            _context = context;
            var prop = new List<ViewPropeties>();
            var all = new List<ISimplificationPiece>(eq);
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            if (NoDenominators(eq) && NoDivisors(eq))
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!NoDenominators(eq) && NoDivisors(eq))
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            foreach (var t in all)
            {
                if (t.GetSimplificationType() == SimplificationPieceType.Expression)
                    AddView(new ViewExpression(((Expression)t), prop.ToArray(), ref timer,_context));
                else if (t.GetSimplificationType() == SimplificationPieceType.Sign
                         && ((Sign)t).GetTypePiece() == ExpressionPieceType.Sign)
                    AddView(new ViewSign((Sign)t, new List<ViewPropeties>(prop), ref timer,_context));
                else
                    AddView(new ViewSign(brace: (Brace)t, prop: new List<ViewPropeties>(prop), context: _context));
            }
        }
        public Step(ExponentialEquation eq, Context context)
            : base(context)
        {
		
            _context = context;
			LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(350), _helper.Factor(50));

            foreach (var t in eq.Left)
            {
                AddView(new ViewExpoTerm(t, _context));
            }
            //add equal sign

            if (!eq.IsComplete) return;
            AddView(GetEqualSignForExpo());

            foreach (var t in eq.Right)
            {
                AddView(new ViewExpoTerm(t,_context));
            }
        }
        public Step(ExponentialEquation eq, ref DispatcherTimer timer, Context context)
            : base(context)
        {
	
            _context = context;
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            foreach (var t in eq.Left)
            {
                AddView(new ViewExpoTerm(t, ref timer,_context));
            }
            //add equal sign

            if (!eq.IsComplete) return;
            AddView(GetEqualSignForExpo());

            foreach (var t in eq.Right)
            {
                AddView(new ViewExpoTerm(t, ref timer,_context));
            }
        }
        //public Step(ExponentialEquation eq, ref DispatcherTimer timer)
        //{
        //    this.Height = 80;
        //    this.Width = 750;

        //    for (int i = 0; i < eq.Left.Count; i++)
        //    {
        //        AddView(new ViewExpoTerm(eq.Left[i]));
        //    }
        //    //add equal sign

        //    if (eq.IsComplete)
        //    {
        //        AddView(GetEqualSignForExpo());

        //        for (int i = 0; i < eq.Right.Count; i++)
        //        {
        //            AddView(new ViewExpoTerm(eq.Right[i]));
        //        }
        //    }
        //}
        #endregion Constructor

        #region Methods

        private LinearLayout GeTermForLogEq(Term term)
        {
            var panel = new LinearLayout(_context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(60))
            };

            if (term.Constant)
            {
                foreach (var myChar in term.CoEfficient.ToString(CultureInfo.InvariantCulture))
                {

                    var grid = new RelativeLayout(_context)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(60))
                    };
                    var inner = new RelativeLayout(_context)
                    {
                        LayoutParameters = new LayoutParams(30, 30) {TopMargin = -10}
                    };

                    inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(myChar));
                    grid.AddView(inner);
                    panel.AddView(grid);
                }
            }
            else throw new NotImplementedException();
            return panel;
        }
        private RelativeLayout LogSign(Sign sign)
        {
            var grid = new RelativeLayout(_context) {LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(30), _helper.Factor(30))};
	        var myParams = new RelativeLayout.LayoutParams(_helper.Factor(15), _helper.Factor(10));
			myParams.AddRule(LayoutRules.CenterInParent);
            var inner = new RelativeLayout(_context) {LayoutParameters = myParams};
            inner.SetBackgroundResource(FixSign(sign));
            grid.AddView(inner);
            return grid;

        }
        private int FixSign(Sign signType)
        {
           return _utility.FixSign(signType);
        }

        readonly TermUtility _utility = new TermUtility();
        private RelativeLayout GetEqualSignForExpo()
        {
	        var myParmas = new RelativeLayout.LayoutParams(_helper.Factor(30)/2, _helper.Factor(80)/2){TopMargin = 20};
			var grid = new RelativeLayout(_context) { LayoutParameters = myParmas };
            var inner = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(_helper.Factor(16)/2, _helper.Factor(20)/2) };

            inner.SetBackgroundResource(Resource.Drawable.equal);//.Background = GetBackGround("equal");
            grid.AddView(inner);
            return grid;
        }
 
        /// <summary>
        /// Changes term divisors to denominators 
        /// where there are no denominators
        /// Reason: Display looks cooler...
        /// </summary>
        /// <returns></returns>
        private Equation FixDivisorsInEqToDenoninators(Equation eq)
        {
            for (var i = 0; i < eq.Left.Count; i++)
            {
                #region Middle

                if (eq.Left[i].GetEquationPieceType() != EquationPieceType.Expression) continue;
                var expCur = (Expression)eq.Left[i];

                if (expCur.Numerator.Count == 1 && (expCur.Denominator == null || expCur.Denominator.Count == 0))
                {
                    if (expCur.Numerator[0].GetTypePiece() == ExpressionPieceType.Term &&
                        (((Term)expCur.Numerator[0]).Devisor != null))
                    {
                        var current = ((Term)expCur.Numerator[0]);
                        expCur.Denominator = new List<IExpressionPiece>
                        {
                            new Term(((Term) expCur.Numerator[0]).Devisor)
                        };

                        current.Devisor = null;

                        expCur.Numerator[0] = new Term(current);

                    }
                }
                eq.Left[i] = new Expression(expCur);

                #endregion Middle
            }

            for (var i = 0; i < eq.Right.Count; i++)
            {
                #region Middle
                if (eq.Right[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    var expCur = (Expression)eq.Right[i];

                    if (expCur.Numerator.Count == 1 && (expCur.Denominator == null || expCur.Denominator.Count == 0))
                    {
                        if (expCur.Numerator[0].GetTypePiece() == ExpressionPieceType.Term &&
                            (((Term)expCur.Numerator[0]).Devisor != null))
                        {
                            var current = ((Term)expCur.Numerator[0]);
                            expCur.Denominator = new List<IExpressionPiece>
                            {
                                new Term(((Term) expCur.Numerator[0]).Devisor)
                            };

                            current.Devisor = null;

                            expCur.Numerator[0] = new Term(current);

                        }
                    }
                    eq.Right[i] = new Expression(expCur);
                }

                #endregion Middle
            }

            return eq;
        }
        private void DoEquation(Equation eq, bool expoEquation)
        {
            eq = FixDivisorsInEqToDenoninators(eq);
            var prop = new List<ViewPropeties>();
            var all = new List<iEquationPiece>(eq.Left);

            if (eq.IsComplete || (eq.Right != null && eq.Right.Count > 0))
            {
                all.Add(new Sign(eq.Split));
                if (eq.Right != null)
                    all.AddRange(eq.Right);
            }

            var noDenominators = NoDenominators(eq);

            if ((noDenominators && NoDivisors(eq)) || (expoEquation && noDenominators))
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!noDenominators && NoDivisors(eq))
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            foreach (var t in all)
            {
                if (t.GetEquationPieceType() == EquationPieceType.Expression)
                    AddView(new ViewExpression(((Expression)t), prop.ToArray(),_context));
                else if (t.GetEquationPieceType() == EquationPieceType.Sign
                         && ((Sign)t).GetTypePiece() == ExpressionPieceType.Sign)
                    AddView(new ViewSign((Sign)t, new List<ViewPropeties>(prop),_context));
                else
                    AddView(new ViewSign(brace: (Brace)t, prop: new List<ViewPropeties>(prop), context: _context));
            }
        }
        private void DoEquation(IEnumerable<iEquationPiece> pieces)
        {

            var all = new List<iEquationPiece>(pieces);
            var prop = new List<ViewPropeties>();

            if ((NoDivisors(all)))
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            
            else throw new NotImplementedException();

            foreach (var t in all)
            {
                if (t.GetEquationPieceType() == EquationPieceType.Expression)
                    AddView(new ViewExpression(((Expression)t), prop.ToArray(),_context));
                else if (t.GetEquationPieceType() == EquationPieceType.Sign
                         && ((Sign)t).GetTypePiece() == ExpressionPieceType.Sign)
                {
                    AddView(new ViewSign((Sign)t, new List<ViewPropeties>(prop),_context));
                }
                else
                    AddView(new ViewSign(brace: (Brace)t, prop: new List<ViewPropeties>(prop), context: _context));
            }
        }
        private void DoEquation(Equation eq, ref DispatcherTimer timer)
        {
            eq = FixDivisorsInEqToDenoninators(eq);
            var prop = new List<ViewPropeties>();
            var all = new List<iEquationPiece>(eq.Left);

            if (eq.IsComplete || (eq.Right != null && eq.Right.Count > 0))
            {
                all.Add(new Sign(eq.Split));
                if (eq.Right != null)
                    all.AddRange(eq.Right);
            }

            var noDenominators = NoDenominators(eq);

            if ((noDenominators && NoDivisors(eq)))
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!noDenominators && NoDivisors(eq))
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            foreach (var t in all)
            {
                if (t.GetEquationPieceType() == EquationPieceType.Expression)
                    AddView(new ViewExpression(((Expression)t), prop.ToArray(), ref timer,_context));
                else if (t.GetEquationPieceType() == EquationPieceType.Sign
                         && ((Sign)t).GetTypePiece() == ExpressionPieceType.Sign)
                {
                    AddView(eq.SplitSelected
                        ? new ViewSign((Sign) t, new List<ViewPropeties>(prop), ref timer, _context)
                        : new ViewSign((Sign) t, new List<ViewPropeties>(prop), _context));
                }
                else
                    AddView(new ViewSign(brace: (Brace)t, prop: new List<ViewPropeties>(prop), context: _context));
            }
        }
        public bool ShowNext()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                try
                {
                    var current = (ViewSign)GetChildAt(i);

                    if (current.ShowNext()) return true;
                }
                catch
                {
                    var expCur = (ViewExpression)GetChildAt(i);

                    if (expCur.ShowNext()) return true;
                }
            }
            return false;
        }
        public void HideAll()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                try
                {
                    var current = (ViewSign)GetChildAt(i);

                    current.HideAll();
                }
                catch
                {
                    var expCur = (ViewExpression)GetChildAt(i);

                    expCur.HidAll();
                }
            }
        }
        private bool NoDenominators(Equation eq)
        {
            var all = new List<iEquationPiece>(eq.Left) {new Sign(eq.Split)};
	        if (eq.Right != null)
                all.AddRange(eq.Right);

            return NoDenominators(all);

        }
        private bool NoDivisors(Equation eq)
        {
            var all = new List<iEquationPiece>(eq.Left) {new Sign(eq.Split)};
	        if (eq.Right != null)
                all.AddRange(eq.Right);

            return NoDivisors(all);
        }

        private bool NoDenominators(IEnumerable<iEquationPiece> pieces)
        {
	        return pieces.Where(t => t.GetEquationPieceType() 
				== EquationPieceType.Expression).All(t => ((Expression) t).NoDenominator());
        }

	    private bool NoDenominators(IEnumerable<ISimplificationPiece> pieces)
        {
	        return pieces.Where(t => t.GetSimplificationType() == 
				SimplificationPieceType.Expression).All(t => ((Expression) t).NoDenominator());
        }

	    private bool NoDivisors(IEnumerable<iEquationPiece> pieces)
        {
	        return pieces.Where(t => t.GetEquationPieceType() 
				== EquationPieceType.Expression).All(t => ((Expression) t).NoDivisors());
        }

	    private bool NoDivisors(IEnumerable<ISimplificationPiece> pieces)
	    {
		    return pieces.Where(t => t.GetSimplificationType() 
				== SimplificationPieceType.Expression).All(t => ((Expression) t).NoDivisors());
	    }

	    #endregion Methods
    }
}