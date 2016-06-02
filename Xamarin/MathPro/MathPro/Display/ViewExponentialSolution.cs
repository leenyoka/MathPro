using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public sealed class ViewExponentialSolution : ViewPlayerItem
    {
        #region Properties
		private readonly Helper _helper = new Helper();
        private readonly Context _context;

        #endregion Properties

        #region Constructor

        public ViewExponentialSolution(SolveForExponentialEquations sol, Context context)
            :base(context)
        {
            _context = context;
            foreach (var t in sol.Solution)
            {
                switch (t.GetStepType())
                {
                    case SolveForXStepType.ExpoEquation:
                        AddView(new Step((ExponentialEquation)t,context));//AddView(SpaceWaster());
                        break;
                    case SolveForXStepType.Equation:
                        AddView(new Step((Equation)t, true,context));//AddView(SpaceWaster());
                        break;
                    case SolveForXStepType.FactorEquations:
                        ShowFactored(t);
                        break;
                    default:
                        throw new NotImplementedException();
                }
				
            }
        }

        #endregion Constructor

        #region Methods
        public bool EquationsAreTheSame(Equation one, Equation two)
        {

            if (one.Split != two.Split)
                return false;

            var oneP = new List<iEquationPiece>();
            var twoP = new List<iEquationPiece>();

            oneP.AddRange(one.Left);
            oneP.AddRange(one.Right);
            twoP.AddRange(two.Left);
            twoP.AddRange(two.Right);

            if (oneP.Count != twoP.Count)
                return false;

            for (int i = 0; i < oneP.Count; i++)
            {
                if (oneP[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    if (twoP[i].GetEquationPieceType() != EquationPieceType.Expression)
                        return false;
                    if (!CompareExpressions((Expression)oneP[i], (Expression)twoP[i]))
                        return false;
                }
                else
                {
                    if (twoP[i].GetEquationPieceType() != EquationPieceType.Sign)
                        return false;
                    if (!CompareSigns((Sign)oneP[i], (Sign)twoP[i]))
                        return false;
                }
            }

            return true;
        }
        private bool CompareExpressions(Expression expOne, Expression expTwo)
        {
            if (expOne.Numerator.Count != expTwo.Numerator.Count)
                return false;

            for (var i = 0; i < expOne.Numerator.Count; i++)
            {
                switch (expOne.Numerator[i].GetTypePiece())
                {
                    case ExpressionPieceType.Term:
                        if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Term)
                            return false;
                        if (!((Term)expOne.Numerator[i]).AreEqual((Term)expTwo.Numerator[i]))
                            return false;
                        break;
                    case ExpressionPieceType.Brace:
                        if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Brace)
                            return false;
                        if (((Brace)expOne.Numerator[i]).Key != ((Brace)expTwo.Numerator[i]).Key)
                            return false;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return true;
        }
        private bool CompareSigns(Sign signOne, Sign signTwo)
        {
            return signOne.SignType == signTwo.SignType;
        }

        private void ShowFactored(ISolveForXStep pieces)
        {
            var eq = (FactorEquations)pieces;

            var value = 0;

            while (value < eq.FactoringSolutions[0].Solution.Count
                || value < eq.FactoringSolutions[1].Solution.Count)
            {

                if (value < eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    var one = (Equation)eq.FactoringSolutions[0].Solution[value];

                    var two = (Equation)eq.FactoringSolutions[1].Solution[value];

                    AddView(!EquationsAreTheSame(one, two) ? new Step(one, two, _context) : new Step(one, _context));
					//AddView(SpaceWaster());
                }
                else if (value < eq.FactoringSolutions[0].Solution.Count
                    && value >= eq.FactoringSolutions[1].Solution.Count)
                {
                    var one = (Equation)eq.FactoringSolutions[0].Solution[value];

                    var two = (Equation)eq.FactoringSolutions[1].Solution[eq.FactoringSolutions[1].Solution.Count - 1];

                    AddView(!EquationsAreTheSame(one, two) ? new Step(one, two, _context) : new Step(one, _context));
					//AddView(SpaceWaster());
                }
                else if (value >= eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    var one = (Equation)eq.FactoringSolutions[0].Solution[eq.FactoringSolutions[0].Solution.Count - 1];

                    var two = (Equation)eq.FactoringSolutions[1].Solution[value];

                    AddView(!EquationsAreTheSame(one, two) ? new Step(one, two, _context) : new Step(one, _context));
					//AddView(SpaceWaster());
                }
                value++;
            }
        }
        #endregion Methods
		private LinearLayout SpaceWaster()
		{
			var panel = new LinearLayout(_context) { LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(20), _helper.Factor(20)) };

			return panel;
		}
        public override bool ShowNext()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                var element = GetChildAt(0);

                if (element.Visibility != ViewStates.Gone) continue;
                element.Visibility = ViewStates.Visible;
                return true;

                //Step step = (Step)this.Items[i];
                //if (step.ShowNext()) return true;
            }
            return false;
        }
        public override void HideAll()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                var element = GetChildAt(0);
                element.Visibility = ViewStates.Gone;
                //step.HideAll();
            }
        }

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewExponentialSolution;
        }
    }
}