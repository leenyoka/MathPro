using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MathBase;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MathPro
{
    public enum ViewPropeties
    {
        NoDivisors, NoRoots, NoDenominators, IsDivisor, IsDenominator, IsFirst
    }
    public enum ViewPlayerItemType
    {
        ViewFactorDistributeSolution, ViewSolveForXSolution,
        ViewSimplificationSolution, ViewInequalitiesSolution,
        ViewExponentialSolution, ViewLogarithmicSolution, None
    }
    public abstract class ViewPlayerItem : ListBox
    {
        public virtual ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.None;
        }
        public virtual bool ShowNext()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                Step step = (Step)this.Items[i];
                if (step.ShowNext()) return true;
            }
            return false;
        }
        public virtual void HideAll()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                Step step = (Step)this.Items[i];
                step.HideAll();
            }
        }
    }
    public class ViewFactorDistributeSolution :ViewPlayerItem
    {
        #region Properties

        FactorDistributeSolution _sol;

        #endregion Properties

        #region Constructor

        public ViewFactorDistributeSolution(FactorDistributeSolution sol)
        {
            this._sol = sol;

            for (int i = 0; i < sol.Solution.Count; i++)
            {
                this.Items.Add(Step(sol.Solution[i]));
            }
        }

        #endregion Constructor

        #region Methods

        private WrapPanel Step(MathBase.Expression exp)
        {
            return new Step(exp);
        }

        #endregion Methods

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewFactorDistributeSolution;
        }
    }
    public class ViewSolveForXSolution : ViewPlayerItem
    {
        #region Properties

        #endregion Properties

        #region Constructor

        public ViewSolveForXSolution(SolveForX sol)
        {
            for (int i = 0; i < sol.Solution.Count; i++)
            {
                if (sol.Solution[i].GetStepType() == SolveForXStepType.Equation)
                    this.Items.Add(new Step((Equation)sol.Solution[i]));
                else ShowFactored(sol.Solution[i]);
            }
        }

        #endregion Constructor

        #region Methods
        public bool EquationsAreTheSame(Equation one, Equation two)
        {

            if (one.Split != two.Split)
                return false;

            List<iEquationPiece> oneP = new List<iEquationPiece>();
            List<iEquationPiece> twoP = new List<iEquationPiece>();

            oneP.AddRange(one.Left);
            oneP.AddRange(one.Right);
            twoP.AddRange(two.Left);
            twoP.AddRange(two.Right);

            if (oneP.Count != twoP.Count)
                return false;

            for (int i = 0; i < oneP.Count; i++)
            {
                if(oneP[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    if (twoP[i].GetEquationPieceType() != EquationPieceType.Expression)
                        return false;
                    else if (!CompareExpressions((MathBase.Expression)oneP[i], (MathBase.Expression)twoP[i]))
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
        private bool CompareExpressions(MathBase.Expression expOne, MathBase.Expression expTwo)
        {
            if (expOne.Numerator.Count != expTwo.Numerator.Count)
                return false;

            for (int i = 0; i < expOne.Numerator.Count; i++)
            {
                if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Term)
                        return false;
                    else if (!((Term)expOne.Numerator[i]).AreEqual((Term)expTwo.Numerator[i]))
                        return false;
                }
                else if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Brace)
                        return false;
                    else if (((Brace)expOne.Numerator[i]).Key != ((Brace)expTwo.Numerator[i]).Key)
                        return false;
                }
                else throw new NotImplementedException();
            }
            return true;
        }
        private bool CompareSigns(Sign signOne, Sign signTwo)
        {
            if (signOne.SignType == signTwo.SignType)
                return true;
            else return false;
        }
        private void ShowFactored(ISolveForXStep pieces)
        {
            FactorEquations eq = (FactorEquations)pieces;

            int value = 0;

            while (value < eq.FactoringSolutions[0].Solution.Count
                || value < eq.FactoringSolutions[1].Solution.Count)
            {

                if (value < eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                else if (value < eq.FactoringSolutions[0].Solution.Count
                    && value >= eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[eq.FactoringSolutions[1].Solution.Count - 1];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                else if (value >= eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[eq.FactoringSolutions[0].Solution.Count - 1];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                value++;
            }
        }


        //public override bool ShowNext()
        //{
            
        //}
        #endregion Methods

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewSolveForXSolution;
        }
    }
    public class ViewSimpificationSolution : ViewPlayerItem
    {
        #region Constructor

        public ViewSimpificationSolution(SimpificationEquation eq)
        {
            for (int i = 0; i < eq.Solution.Count; i++)
                this.Items.Add(new Step(eq.Solution[i]));
        }

        #endregion Constructor

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewSimplificationSolution;
        }
    }
    public class ViewInequalitiesSolution :  ViewPlayerItem
    {
        #region Constructor

        public ViewInequalitiesSolution(Inequalities inequalitySolution)
        {
            for (int i = 0; i < inequalitySolution.Solution.Count; i++)
            {
                if (inequalitySolution.Solution[i].GetStepType() == SolveForXStepType.FactorEquations)
                {
                    //if(i == (inequalitySolution.Solution.Count -1))
                    ShowFactored(inequalitySolution.Solution[i]);
                }
                else
                    this.Items.Add(new Step((Equation)inequalitySolution.Solution[i]));
            }
        }

        #endregion Constructor

        #region Methods

        public bool EquationsAreTheSame(Equation one, Equation two)
        {

            if (one.Split != two.Split)
                return false;

            List<iEquationPiece> oneP = new List<iEquationPiece>();
            List<iEquationPiece> twoP = new List<iEquationPiece>();

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
                    else if (!CompareExpressions((MathBase.Expression)oneP[i], (MathBase.Expression)twoP[i]))
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
        private bool CompareExpressions(MathBase.Expression expOne, MathBase.Expression expTwo)
        {
            if (expOne.Numerator.Count != expTwo.Numerator.Count)
                return false;

            for (int i = 0; i < expOne.Numerator.Count; i++)
            {
                if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Term)
                        return false;
                    else if (!((Term)expOne.Numerator[i]).AreEqual((Term)expTwo.Numerator[i]))
                        return false;
                }
                else if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Brace)
                        return false;
                    else if (((Brace)expOne.Numerator[i]).Key != ((Brace)expTwo.Numerator[i]).Key)
                        return false;
                }
                else throw new NotImplementedException();
            }
            return true;
        }
        private bool CompareSigns(Sign signOne, Sign signTwo)
        {
            if (signOne.SignType == signTwo.SignType)
                return true;
            else return false;
        }
        private List<iEquationPiece> FixFactoredForShow(Equation one, Equation two)
        {
            List<iEquationPiece> pieces = new List<iEquationPiece>();
            pieces.AddRange(one.Right);
            pieces.Add(Flip(new Sign(one.Split)));
            pieces.AddRange(two.Left);
            pieces.Add(new Sign(two.Split));
            pieces.AddRange(two.Right);
            return pieces;

        }
        public Sign Flip(Sign old)
        {
            if (old.SignType == SignType.greater)
                return new Sign(SignType.smaller);
            else if (old.SignType == SignType.greaterEqual)
                return new Sign(SignType.smallerEqual);
            else if (old.SignType == SignType.smaller)
                return new Sign(SignType.greater);
            else if (old.SignType == SignType.smallerEqual)
                return new Sign(SignType.greaterEqual);
            else return old;
        }
        private void ShowFactored(ISolveForXStep pieces)
        {
            FactorEquations eq = (FactorEquations)pieces;

            int value = 0;

            while (value < eq.FactoringSolutions[0].Solution.Count
                || value < eq.FactoringSolutions[1].Solution.Count)
            {

                if (value < eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                    {
                        //if(value == eq.FactoringSolutions[0].Solution.Count -1)
                        this.Items.Add(new Step(one, two));
                    }
                    else this.Items.Add(new Step(one));
                }
                else if (value < eq.FactoringSolutions[0].Solution.Count
                    && value >= eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[eq.FactoringSolutions[1].Solution.Count - 1];

                    if (!EquationsAreTheSame(one, two))
                    {
                        //if (value == eq.FactoringSolutions[0].Solution.Count - 1)
                            this.Items.Add(new Step(one, two));
                    }
                    else this.Items.Add(new Step(one));
                }
                else if (value >= eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[eq.FactoringSolutions[0].Solution.Count - 1];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                    {
                        //if (value == eq.FactoringSolutions[0].Solution.Count - 1)
                            this.Items.Add(new Step(one, two));
                    }
                    else this.Items.Add(new Step(one));
                }
                value++;
            }
        }

        #endregion Methods

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewInequalitiesSolution;
        }
    }
    public class ViewExponentialSolution : ViewPlayerItem
    {
        #region Properties

        #endregion Properties

        #region Constructor

        public ViewExponentialSolution(SolveForExponentialEquations sol)
        {
            for (int i = 0; i < sol.Solution.Count; i++)
            {
                if (sol.Solution[i].GetStepType() == SolveForXStepType.ExpoEquation)
                {
                    this.Items.Add(new Step((ExponentialEquation)sol.Solution[i]));
                }
                else if (sol.Solution[i].GetStepType() == SolveForXStepType.Equation)
                {
                    
                    this.Items.Add(new Step((Equation)sol.Solution[i], true));
                }
                else if (sol.Solution[i].GetStepType() == SolveForXStepType.FactorEquations)
                {
                    ShowFactored(sol.Solution[i]);
                }
                else throw new NotImplementedException();
            }
        }

        #endregion Constructor

        #region Methods
        public bool EquationsAreTheSame(Equation one, Equation two)
        {

            if (one.Split != two.Split)
                return false;

            List<iEquationPiece> oneP = new List<iEquationPiece>();
            List<iEquationPiece> twoP = new List<iEquationPiece>();

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
                    else if (!CompareExpressions((MathBase.Expression)oneP[i], (MathBase.Expression)twoP[i]))
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
        private bool CompareExpressions(MathBase.Expression expOne, MathBase.Expression expTwo)
        {
            if (expOne.Numerator.Count != expTwo.Numerator.Count)
                return false;

            for (int i = 0; i < expOne.Numerator.Count; i++)
            {
                if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Term)
                        return false;
                    else if (!((Term)expOne.Numerator[i]).AreEqual((Term)expTwo.Numerator[i]))
                        return false;
                }
                else if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Brace)
                        return false;
                    else if (((Brace)expOne.Numerator[i]).Key != ((Brace)expTwo.Numerator[i]).Key)
                        return false;
                }
                else throw new NotImplementedException();
            }
            return true;
        }
        private bool CompareSigns(Sign signOne, Sign signTwo)
        {
            if (signOne.SignType == signTwo.SignType)
                return true;
            else return false;
        }
        private void ShowFactored(ISolveForXStep pieces)
        {
            FactorEquations eq = (FactorEquations)pieces;

            int value = 0;

            while (value < eq.FactoringSolutions[0].Solution.Count
                || value < eq.FactoringSolutions[1].Solution.Count)
            {

                if (value < eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                else if (value < eq.FactoringSolutions[0].Solution.Count
                    && value >= eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[eq.FactoringSolutions[1].Solution.Count - 1];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                else if (value >= eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[eq.FactoringSolutions[0].Solution.Count - 1];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                value++;
            }
        }
        #endregion Methods

        public override bool ShowNext()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                UIElement element = (UIElement)this.Items[0];

                if (element.Visibility == System.Windows.Visibility.Collapsed)
                {
                    element.Visibility = System.Windows.Visibility.Visible;
                    return true;
                }

                //Step step = (Step)this.Items[i];
                //if (step.ShowNext()) return true;
            }
            return false;
        }
        public override void HideAll()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                UIElement element = (UIElement)this.Items[0];
                element.Visibility = System.Windows.Visibility.Collapsed;
                //step.HideAll();
            }
        }

        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewExponentialSolution;
        }
    }
    public class ViewLogarithmicSolution :  ViewPlayerItem
    {
        #region Properties

        #endregion Properties

        #region Constructor

        public ViewLogarithmicSolution(SolveForLogarithmicEquations sol)
        {
           
            for (int i = 0; i < sol.Solution.Count; i++)
            {
                if (sol.Solution[i].GetStepType() == SolveForXStepType.Equation)
                {
                    this.Items.Add(new Step((Equation)sol.Solution[i]));
                }
                else if (sol.Solution[i].GetStepType() == SolveForXStepType.LogEquation)
                {
                    this.Items.Add(new Step((LogarithmicEquation)sol.Solution[i]));
                }
                else if (sol.Solution[i].GetStepType() == SolveForXStepType.FactorEquations)
                {
                    ShowFactored(sol.Solution[i]);
                }
                else throw new NotImplementedException();
            }

        }

        #endregion Constructor

        #region Methods
        public bool EquationsAreTheSame(Equation one, Equation two)
        {

            if (one.Split != two.Split)
                return false;

            List<iEquationPiece> oneP = new List<iEquationPiece>();
            List<iEquationPiece> twoP = new List<iEquationPiece>();

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
                    else if (!CompareExpressions((MathBase.Expression)oneP[i], (MathBase.Expression)twoP[i]))
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
        private bool CompareExpressions(MathBase.Expression expOne, MathBase.Expression expTwo)
        {
            if (expOne.Numerator.Count != expTwo.Numerator.Count)
                return false;

            for (int i = 0; i < expOne.Numerator.Count; i++)
            {
                if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Term)
                        return false;
                    else if (!((Term)expOne.Numerator[i]).AreEqual((Term)expTwo.Numerator[i]))
                        return false;
                }
                else if (expOne.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (expTwo.Numerator[i].GetTypePiece() != ExpressionPieceType.Brace)
                        return false;
                    else if (((Brace)expOne.Numerator[i]).Key != ((Brace)expTwo.Numerator[i]).Key)
                        return false;
                }
                else throw new NotImplementedException();
            }
            return true;
        }
        private bool CompareSigns(Sign signOne, Sign signTwo)
        {
            if (signOne.SignType == signTwo.SignType)
                return true;
            else return false;
        }
        private void ShowFactored(ISolveForXStep pieces)
        {
            FactorEquations eq = (FactorEquations)pieces;

            int value = 0;

            while (value < eq.FactoringSolutions[0].Solution.Count
                || value < eq.FactoringSolutions[1].Solution.Count)
            {

                if (value < eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                else if (value < eq.FactoringSolutions[0].Solution.Count
                    && value >= eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[value];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[eq.FactoringSolutions[1].Solution.Count - 1];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                else if (value >= eq.FactoringSolutions[0].Solution.Count
                    && value < eq.FactoringSolutions[1].Solution.Count)
                {
                    Equation one = (Equation)((SolveForX)eq.FactoringSolutions[0]).Solution[eq.FactoringSolutions[0].Solution.Count - 1];

                    Equation two = (Equation)((SolveForX)eq.FactoringSolutions[1]).Solution[value];

                    if (!EquationsAreTheSame(one, two))
                        this.Items.Add(new Step(one, two));
                    else this.Items.Add(new Step(one));
                }
                value++;
            }
        }
        #endregion Methods

        public override bool ShowNext()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                UIElement element = (UIElement)this.Items[0];

                if (element.Visibility == System.Windows.Visibility.Collapsed)
                {
                    element.Visibility = System.Windows.Visibility.Visible;
                    return true;
                }

                //Step step = (Step)this.Items[i];
                //if (step.ShowNext()) return true;
            }
            return false;
        }
        public override void HideAll()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                UIElement element = (UIElement)this.Items[0];
                element.Visibility = System.Windows.Visibility.Collapsed;
                //step.HideAll();
            }
        }
        public override ViewPlayerItemType GetViewPlayerItemType()
        {
            return ViewPlayerItemType.ViewLogarithmicSolution;
        }
    }
    public class Step : WrapPanel
    {
        #region Properties



        #endregion Properties

        #region Constructor

        public Step(MathBase.Expression exp)
        {

            if (NoDenominators(new List<iEquationPiece>(new iEquationPiece[] { exp }))
                && NoDivisors(new List<iEquationPiece>(new iEquationPiece[] { exp })))
            {
                this.Width = 760;
                this.Height = 60;

                this.Children.Add(new ViewExpression(exp, 
                    new ViewPropeties[] { ViewPropeties.NoDivisors, ViewPropeties.NoDenominators }));
            }
            else throw new NotImplementedException();
        }
        public Step(MathBase.Equation eq)
        {
            DoEquation(eq, false);
        }
        public Step(List<iEquationPiece> pieces)
        {
            DoEquation(pieces);
        }
        public Step(MathBase.LogarithmicEquation eq)
        {
            this.Width = 760;
            this.Height = 60;

            for (int i = 0; i < eq.Left.Count; i++)
            {
                if (eq.Left[i].GetType1() == MathBase.Type.Log)
                    this.Children.Add(new ViewLogTerm((LogTerm)eq.Left[i]));
                else if (eq.Left[i].GetType1() == MathBase.Type.Sign)
                    this.Children.Add(LogSign((Sign)eq.Left[i]));
                else if (eq.Left[i].GetType1() == MathBase.Type.Term)
                    this.Children.Add(GeTermForLogEq((Term)eq.Left[i]));
                else throw new NotImplementedException();
            }

            if (eq.IsComplete)
            {
                this.Children.Add(LogSign(new Sign("=")));

                for (int i = 0; i < eq.Right.Count; i++)
                {
                    if (eq.Right[i].GetType1() == MathBase.Type.Log)
                        this.Children.Add(new ViewLogTerm((LogTerm)eq.Right[i]));
                    else if (eq.Right[i].GetType1() == MathBase.Type.Sign)
                        this.Children.Add(LogSign((Sign)eq.Right[i]));
                    else if (eq.Right[i].GetType1() == MathBase.Type.Term)
                        this.Children.Add(GeTermForLogEq((Term)eq.Right[i]));
                    else throw new NotImplementedException();
                }
            }
        }
        public Step(MathBase.LogarithmicEquation eq, ref DispatcherTimer timer)
        {
            this.Width = 760;
            this.Height = 60;

            for (int i = 0; i < eq.Left.Count; i++)
            {
                if (eq.Left[i].GetType1() == MathBase.Type.Log)
                    this.Children.Add(new ViewLogTerm((LogTerm)eq.Left[i], ref timer));
                else if (eq.Left[i].GetType1() == MathBase.Type.Sign)
                    this.Children.Add(LogSign((Sign)eq.Left[i]));
                else if (eq.Left[i].GetType1() == MathBase.Type.Term)
                    this.Children.Add(GeTermForLogEq((Term)eq.Left[i]));
                else throw new NotImplementedException();
            }

            if (eq.IsComplete)
            {
                this.Children.Add(LogSign(new Sign("=")));

                for (int i = 0; i < eq.Right.Count; i++)
                {
                    if (eq.Right[i].GetType1() == MathBase.Type.Log)
                        this.Children.Add(new ViewLogTerm((LogTerm)eq.Right[i], ref timer));
                    else if (eq.Right[i].GetType1() == MathBase.Type.Sign)
                        this.Children.Add(LogSign((Sign)eq.Right[i]));
                    else if (eq.Right[i].GetType1() == MathBase.Type.Term)
                        this.Children.Add(GeTermForLogEq((Term)eq.Right[i]));
                    else throw new NotImplementedException();
                }
            }
        }
        public Step(MathBase.Equation eq, bool expEquation)
        {
            DoEquation(eq, expEquation);
        }
        public Step(MathBase.Equation eq, ref DispatcherTimer timer)
        {
            DoEquation(eq, ref timer);
        }
        public Step(MathBase.Equation oneEq, MathBase.Equation twoEq)
        {
            DoEquation(oneEq, false);
            this.Children.Add(new ViewSign(new Sign(SignType.OR), new List<ViewPropeties>()));
            DoEquation(twoEq, false );
        }
        public Step(List<ISimplificationPiece> eq)
        {
            List<ViewPropeties> prop = new List<ViewPropeties>();
            List<ISimplificationPiece> all = new List<ISimplificationPiece>(eq);
            

            if (NoDenominators(eq) && NoDivisors(eq))
            {
                this.Width = 760;
                this.Height = 60;
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!NoDenominators(eq) && NoDivisors(eq))
            {
                this.Width = 760;
                this.Height = 120;
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].GetSimplificationType() ==  SimplificationPieceType.Expression)
                    this.Children.Add(new ViewExpression(((MathBase.Expression)all[i]), prop.ToArray()));
                else if (all[i].GetSimplificationType() ==  SimplificationPieceType.Sign
                    && ((Sign)all[i]).GetTypePiece() == ExpressionPieceType.Sign)
                    this.Children.Add(new ViewSign((Sign)all[i], new List<ViewPropeties>(prop)));
                else
                    this.Children.Add(new ViewSign((Brace)all[i], new List<ViewPropeties>(prop)));
            }

        }
        public Step(List<ISimplificationPiece> eq, ref DispatcherTimer timer)
        {
            List<ViewPropeties> prop = new List<ViewPropeties>();
            List<ISimplificationPiece> all = new List<ISimplificationPiece>(eq);


            if (NoDenominators(eq) && NoDivisors(eq))
            {
                this.Width = 760;
                this.Height = 60;
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!NoDenominators(eq) && NoDivisors(eq))
            {
                this.Width = 760;
                this.Height = 120;
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].GetSimplificationType() == SimplificationPieceType.Expression)
                    this.Children.Add(new ViewExpression(((MathBase.Expression)all[i]), prop.ToArray(), ref timer));
                else if (all[i].GetSimplificationType() == SimplificationPieceType.Sign
                    && ((Sign)all[i]).GetTypePiece() == ExpressionPieceType.Sign)
                    this.Children.Add(new ViewSign((Sign)all[i], new List<ViewPropeties>(prop), ref timer));
                else
                    this.Children.Add(new ViewSign((Brace)all[i], new List<ViewPropeties>(prop)));
            }

        }
        public Step(ExponentialEquation eq)
        {
            this.Height = 80;
            this.Width = 750;

            for (int i = 0; i < eq.Left.Count; i++)
            {
                this.Children.Add(new ViewExpoTerm(eq.Left[i]));
            }
            //add equal sign

            if (eq.IsComplete)
            {
                this.Children.Add(GetEqualSignForExpo());

                for (int i = 0; i < eq.Right.Count; i++)
                {
                    this.Children.Add(new ViewExpoTerm(eq.Right[i]));
                }
            }
        }
        public Step(ExponentialEquation eq, ref DispatcherTimer timer)
        {
            this.Height = 80;
            this.Width = 750;

            for (int i = 0; i < eq.Left.Count; i++)
            {
                this.Children.Add(new ViewExpoTerm(eq.Left[i], ref timer));
            }
            //add equal sign

            if (eq.IsComplete)
            {
                this.Children.Add(GetEqualSignForExpo());

                for (int i = 0; i < eq.Right.Count; i++)
                {
                    this.Children.Add(new ViewExpoTerm(eq.Right[i], ref timer));
                }
            }
        }
        //public Step(ExponentialEquation eq, ref DispatcherTimer timer)
        //{
        //    this.Height = 80;
        //    this.Width = 750;

        //    for (int i = 0; i < eq.Left.Count; i++)
        //    {
        //        this.Children.Add(new ViewExpoTerm(eq.Left[i]));
        //    }
        //    //add equal sign

        //    if (eq.IsComplete)
        //    {
        //        this.Children.Add(GetEqualSignForExpo());

        //        for (int i = 0; i < eq.Right.Count; i++)
        //        {
        //            this.Children.Add(new ViewExpoTerm(eq.Right[i]));
        //        }
        //    }
        //}
        #endregion Constructor

        #region Methods

        private WrapPanel GeTermForLogEq(Term term)
        {
            WrapPanel panel = new WrapPanel();
            panel.Height = 60;

            if (term.Constant)
            {
                foreach (char myChar in term.CoEfficient.ToString())
                {

                    Grid grid = new Grid();
                    grid.Height = 60;
                    Grid inner = new Grid();
                    inner.Width = 30;
                    inner.Height = 30;
                    inner.Margin = new Thickness(0, -10, 0, 0);
                    inner.Background = GetBackGround(myChar.ToString());
                    grid.Children.Add(inner);
                    panel.Children.Add(grid);
                }
            }
            else throw new NotImplementedException();
            return panel;
        }
        private Grid LogSign(Sign sign)
        {
            Grid grid = new Grid();
            grid.Width = 70;
            grid.Height = 60;
            Grid inner = new Grid();
            inner.Width = 20;
            inner.Height = 15;
            inner.Margin = new Thickness(0, 0, 0, 0);
            inner.Background = GetBackGround(FixSign(sign));
            grid.Children.Add(inner);
            return grid;

        }
        private string FixSign(Sign signType)
        {
            if (signType.SignType == SignType.Add)
                return "plus";
            else if (signType.SignType == SignType.Subtract)
                return "minus";
            else if (signType.SignType == SignType.Divide)
                return "divide";
            else if (signType.SignType == SignType.equal)
                return "equal";
            else if (signType.SignType == SignType.greater)
                return "greaterthan";
            else if (signType.SignType == SignType.greaterEqual)
                return "greaterOrEqual";
            else if (signType.SignType == SignType.Multiply)
                return "multiply";
            else if (signType.SignType == SignType.OR)
                return "or";
            else if (signType.SignType == SignType.smaller)
                return "smallerthan";
            else if (signType.SignType == SignType.smallerEqual)
                return "smallerOrEqual";
            throw new NotImplementedException();
        }
        private Grid GetEqualSignForExpo()
        {
            Grid grid = new Grid();
            grid.Width = 30;
            grid.Height = 80;
            Grid inner = new Grid();
            inner.Height = 16;
            inner.Width = 20;
            inner.Margin = new Thickness(0, 15, 0, 0);
            inner.Background = GetBackGround("equal");
            grid.Children.Add(inner);
            return grid;
        }
        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        /// <summary>
        /// Changes term divisors to denominators 
        /// where there are no denominators
        /// Reason: Display looks cooler...
        /// </summary>
        /// <returns></returns>
        private Equation FixDivisorsInEqToDenoninators(Equation eq)
        {
            for (int i = 0; i < eq.Left.Count; i++)
            {
                #region Middle
                if (eq.Left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    MathBase.Expression expCur = (MathBase.Expression)eq.Left[i];

                    if(expCur.Numerator.Count == 1 && (expCur.Denominator == null || expCur.Denominator.Count ==0))
                    {
                        if (expCur.Numerator[0].GetTypePiece() == ExpressionPieceType.Term &&
                            (((Term)expCur.Numerator[0]).Devisor != null))
                        {
                            Term current = ((Term)expCur.Numerator[0]); 
                            expCur.Denominator = new List<IExpressionPiece>();
                            expCur.Denominator.Add(new Term(((Term)expCur.Numerator[0]).Devisor));

                            current.Devisor = null;

                            expCur.Numerator[0] = new Term(current);
                            
                        }
                    }
                    eq.Left[i] = new MathBase.Expression(expCur);
                }

                #endregion Middle
            }

            for (int i = 0; i < eq.Right.Count; i++)
            {
                #region Middle
                if (eq.Right[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    MathBase.Expression expCur = (MathBase.Expression)eq.Right[i];

                    if (expCur.Numerator.Count == 1 && (expCur.Denominator == null || expCur.Denominator.Count == 0))
                    {
                        if (expCur.Numerator[0].GetTypePiece() == ExpressionPieceType.Term &&
                            (((Term)expCur.Numerator[0]).Devisor != null))
                        {
                            Term current = ((Term)expCur.Numerator[0]);
                            expCur.Denominator = new List<IExpressionPiece>();
                            expCur.Denominator.Add(new Term(((Term)expCur.Numerator[0]).Devisor));

                            current.Devisor = null;

                            expCur.Numerator[0] = new Term(current);

                        }
                    }
                    eq.Right[i] = new MathBase.Expression(expCur);
                }

                #endregion Middle
            }

            return eq;
        }
        private void DoEquation(Equation eq, bool expoEquation)
        {
            eq = FixDivisorsInEqToDenoninators(eq);
            List<ViewPropeties> prop = new List<ViewPropeties>();
            List<iEquationPiece> all = new List<iEquationPiece>(eq.Left);

            if (eq.IsComplete || (eq.Right != null && eq.Right.Count > 0))
            {
                all.Add(new Sign(eq.Split));
                if (eq.Right != null)
                    all.AddRange(eq.Right);
            }

            bool noDenominators = NoDenominators(eq);

            if ((noDenominators && NoDivisors(eq)) || (expoEquation && noDenominators))
            {
                this.Width = 760;
                this.Height = 60;
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!noDenominators && NoDivisors(eq))
            {
                this.Width = 760;
                this.Height = 120;
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].GetEquationPieceType() == EquationPieceType.Expression)
                    this.Children.Add(new ViewExpression(((MathBase.Expression)all[i]), prop.ToArray()));
                else if (all[i].GetEquationPieceType() == EquationPieceType.Sign
                    && ((Sign)all[i]).GetTypePiece() == ExpressionPieceType.Sign)
                    this.Children.Add(new ViewSign((Sign)all[i], new List<ViewPropeties>(prop)));
                else
                    this.Children.Add(new ViewSign((Brace)all[i], new List<ViewPropeties>(prop)));
            }
        }
        private void DoEquation(List<iEquationPiece> pieces)
        {
            
            List<iEquationPiece> all = new List<iEquationPiece>(pieces);
            List<ViewPropeties> prop = new List<ViewPropeties>();

            bool noDenominators = true;

            if ((noDenominators && NoDivisors(all)))
            {
                this.Width = 760;
                this.Height = 60;
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!noDenominators && NoDivisors(all))
            {
                this.Width = 760;
                this.Height = 120;
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].GetEquationPieceType() == EquationPieceType.Expression)
                    this.Children.Add(new ViewExpression(((MathBase.Expression)all[i]), prop.ToArray()));
                else if (all[i].GetEquationPieceType() == EquationPieceType.Sign
                    && ((Sign)all[i]).GetTypePiece() == ExpressionPieceType.Sign)
                {
                    this.Children.Add(new ViewSign((Sign)all[i], new List<ViewPropeties>(prop)));
                }
                else
                    this.Children.Add(new ViewSign((Brace)all[i], new List<ViewPropeties>(prop)));
            }
        }
        private void DoEquation(Equation eq, ref DispatcherTimer timer)
        {
            eq = FixDivisorsInEqToDenoninators(eq);
            List<ViewPropeties> prop = new List<ViewPropeties>();
            List<iEquationPiece> all = new List<iEquationPiece>(eq.Left);

            if (eq.IsComplete || (eq.Right != null && eq.Right.Count > 0))
            {
                all.Add(new Sign(eq.Split));
                if (eq.Right != null)
                    all.AddRange(eq.Right);
            }

            bool noDenominators = NoDenominators(eq);

            if ((noDenominators && NoDivisors(eq)))
            {
                this.Width = 760;
                this.Height = 60;
                prop.Add(ViewPropeties.NoDenominators);
                prop.Add(ViewPropeties.NoDivisors);
            }
            else if (!noDenominators && NoDivisors(eq))
            {
                this.Width = 760;
                this.Height = 120;
                prop.Add(ViewPropeties.NoDivisors);
            }
            else throw new NotImplementedException();

            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].GetEquationPieceType() == EquationPieceType.Expression)
                    this.Children.Add(new ViewExpression(((MathBase.Expression)all[i]), prop.ToArray(),ref timer));
                else if (all[i].GetEquationPieceType() == EquationPieceType.Sign
                    && ((Sign)all[i]).GetTypePiece() == ExpressionPieceType.Sign)
                {
                    if(eq.SplitSelected)
                    this.Children.Add(new ViewSign((Sign)all[i], new List<ViewPropeties>(prop), ref timer));
                    else this.Children.Add(new ViewSign((Sign)all[i], new List<ViewPropeties>(prop)));
                }
                else
                    this.Children.Add(new ViewSign((Brace)all[i], new List<ViewPropeties>(prop)));
            }
        }
        public bool ShowNext()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                try
                {
                    ViewSign current = (ViewSign)this.Children[i];

                    if (current.ShowNext()) return true;
                }
                catch
                {
                    ViewExpression expCur = (ViewExpression)this.Children[i];

                    if (expCur.ShowNext()) return true;
                }
            }
            return false;
        }
        public void HideAll()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                try
                {
                    ViewSign current = (ViewSign)this.Children[i];

                    current.HideAll();
                }
                catch
                {
                    ViewExpression expCur = (ViewExpression)this.Children[i];

                   expCur.HidAll();
                }
            }
        }
        private bool NoDenominators(Equation eq)
        {
            List<iEquationPiece> all = new List<iEquationPiece>(eq.Left);
            all.Add(new Sign(eq.Split));
            if(eq.Right != null)
            all.AddRange(eq.Right);

            return NoDenominators(all);
        
        }
        private bool NoDivisors(Equation eq)
        {
            List<iEquationPiece> all = new List<iEquationPiece>(eq.Left);
            all.Add(new Sign(eq.Split));
            if (eq.Right != null)
                all.AddRange(eq.Right);

            return NoDivisors(all);
        }

        private bool NoDenominators(List<iEquationPiece> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    if (!((MathBase.Expression)pieces[i]).NoDenominator())
                        return false;
                }
            }
            return true;
        }
        private bool NoDenominators(List<ISimplificationPiece> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    if (!((MathBase.Expression)pieces[i]).NoDenominator())
                        return false;
                }
            }
            return true;
        }
        private bool NoDivisors(List<iEquationPiece> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    if (!((MathBase.Expression)pieces[i]).NoDivisors())
                        return false;
                }
            }
            return true;
        }
        private bool NoDivisors(List<ISimplificationPiece> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    if (!((MathBase.Expression)pieces[i]).NoDivisors())
                        return false;
                }
            }
            return true;
        }

        #endregion Methods
    }
    public class ViewSign:Grid
    {
        #region Properties

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

        public ViewSign(Sign sign, List<ViewPropeties> prop)
        {
            _viewProperties = new List<ViewPropeties>(prop);
            Grid inner = new Grid();

            if(sign.SignType == SignType.OR)
            {
                this.Width = 30;
                this.Height = 20;
                inner.Height = 18;
                inner.Width = 18;
                inner.Margin = new Thickness(0, -12, 0, 0);
            }
            else if ((NoDenominator() && NoDivisors(true)))
            {
                this.Width = 20;
                this.Height = 20;
                inner.Height = 9;
                inner.Width = 9;
                inner.Margin = new Thickness(0, -12, 0, 0);
                
            }
            else if (!NoDenominator() && NoDivisors(false))
            {
                this.Width = 20;
                this.Height = 120;
                inner.Height = 9;
                inner.Width = 9;
                inner.Margin = new Thickness(0, -18, 0, 0);
            }
            inner.Background = GetBackGround(FixSign(sign));
            this.Children.Add(inner);
        }
        public ViewSign(Sign sign, List<ViewPropeties> prop, ref DispatcherTimer timer)
        {
            _viewProperties = new List<ViewPropeties>(prop);
            Grid inner = new Grid();

            if (sign.SignType == SignType.OR)
            {
                this.Width = 30;
                this.Height = 20;
                inner.Height = 18;
                inner.Width = 18;
                inner.Margin = new Thickness(0, -12, 0, 0);

            }
            else if ((NoDenominator() && NoDivisors(true)))
            {
                this.Width = 20;
                this.Height = 20;
                inner.Height = 9;
                inner.Width = 9;
                inner.Margin = new Thickness(0, -12, 0, 0);

               // _mySelect.Add(new SelectionCursor(-2,20,9,20,9, new Thickness(0, -12, 0, 0)));
                //this.Children.Add(_mySelect[_mySelect.Count - 1]);

            }
            else if (!NoDenominator() && NoDivisors(false))
            {
                this.Width = 20;
                this.Height = 120;
                inner.Height = 9;
                inner.Width = 9;
                inner.Margin = new Thickness(0, -18, 0, 0);
                _mySelect.Add(new SelectionCursor(-2,20,9,120,9, new Thickness(0, -18, 0, 0)));
                //this.Children.Add(_mySelect[_mySelect.Count - 1]);
            
            }
                //Select(SelectedPieceType.OutSide, -2, ref timer);

            inner.Background = GetBackGround(FixSign(sign));
            this.Children.Add(inner);
        }
        public bool ShowNext()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i].Visibility == System.Windows.Visibility.Collapsed
                    && !DontTouch(this.Children[i]))
                {
                    this.Children[i].Visibility = System.Windows.Visibility.Visible;
                    return true;
                }
               
            }
            return false;
        }
        public void HideAll()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private bool DontTouch(UIElement element)
        {
            try
            {
                SelectionCursor cor = (SelectionCursor)element;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public ViewSign(Brace brace, List<ViewPropeties> prop)
        {
            _viewProperties = new List<ViewPropeties>(prop);
            Grid inner = new Grid();
            this.Width = 15;
            this.Height = 60;
            inner.Width = 9;
            inner.Height = 20;
            inner.Margin = new Thickness(0, -45, 0, 0);
            if (brace.Key == ')')
                inner.Background = GetBackGround("closeBrace");
            else inner.Background = GetBackGround("openBrace");
            this.Children.Add(inner);
        }

        #endregion Constructor

        #region Selection Functions

        public bool Select(SelectedPieceType type1, int index, ref DispatcherTimer timer)
        {
            bool answer = false;
            DeSelect();

            for (int i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].MySelectedPieceType == type1 && _mySelect[i].Index == index)
                {
                    _mySelect[i].Active = true;
                    _mySelect[i].Visibility = System.Windows.Visibility.Visible;
                    timer.Tick += new EventHandler(Toggle);
                    answer = true;
                    break;
                }

            }
            return answer;
        }
        private void Toggle(object sender, EventArgs e)
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].Active)
                {
                    if (_mySelect[i].Children[0].Visibility == System.Windows.Visibility.Visible)
                        _mySelect[i].Children[0].Visibility = System.Windows.Visibility.Collapsed;
                    else
                        _mySelect[i].Children[0].Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public void DeSelect()
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                _mySelect[i].Active = false;
                _mySelect[i].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion Selection Functions

        #region Method

        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        private string FixSign(Sign signType)
        {
            if (signType.SignType == SignType.Add)
                return "plus";
            else if (signType.SignType == SignType.Subtract)
                return "minus";
            else if (signType.SignType == SignType.Divide)
                return "divide";
            else if (signType.SignType == SignType.equal)
                return "equal";
            else if (signType.SignType == SignType.greater)
                return "greaterthan";
            else if (signType.SignType == SignType.greaterEqual)
                return "greaterOrEqual";
            else if (signType.SignType == SignType.Multiply)
                return "multiply";
            else if (signType.SignType == SignType.OR)
                return "or";
            else if (signType.SignType == SignType.smaller)
                return "smallerthan";
            else if (signType.SignType == SignType.smallerEqual)
                return "smallerOrEqual";
            throw new NotImplementedException();
        }
        private bool NoDenominator()
        {
            return ViewPropertyTrue(ViewPropeties.NoDenominators);
        }
        private bool ViewPropertyTrue(ViewPropeties property)
        {
            for (int i = 0; i < this._viewProperties.Count; i++)
            {
                if (this._viewProperties[i] == property)
                    return true;
            }
            return false;
        }
        private bool NoDivisors(bool noDenominator)
        {
            return ViewPropertyTrue(ViewPropeties.NoDivisors);
            
        }
        #endregion Methods

    }
    public class ViewExpression:Grid
    {
        #region Properties

        List<SelectionCursor> _mySelect = new List<SelectionCursor>();

        List<ViewPropeties> _viewProperties = new List<ViewPropeties>();

        public List<ViewPropeties> ViewProperties
        {
            get { return _viewProperties; }
            set { _viewProperties = value; }
        }

        MathBase.Expression exp;

        #endregion Properties

        #region Constructor

        public ViewExpression(MathBase.Expression exp, ViewPropeties[] properties)
        {
            this.exp = exp;
            if(properties != null && properties.Length > 0)
            this._viewProperties.AddRange(properties);
            Prepare();
        }
        public ViewExpression(MathBase.Expression exp, ViewPropeties[] properties, ref DispatcherTimer timer)
        {
            this.exp = exp;
            if (properties != null && properties.Length > 0)
                this._viewProperties.AddRange(properties);
            timer.Tick += new EventHandler(Toggle);
            Prepare(ref timer);
        }

        #endregion Constructor

        #region Methods

        private void Prepare( ref DispatcherTimer timer)
        {
            bool noDims = NoDenominator();

            if (noDims && NoDivisors(true))
            {
                this.Height = 60;
                this.Children.Add(GetNumerator(true, true, ref timer));
                // no dims no divs
            }
            else if (!noDims && NoDivisors(false))
            {
                this.Height = 120;
                bool noDim = (exp.Denominator == null || exp.Denominator.Count == 0) ? true : false;
                this.Children.Add(GetNumerator(noDim, true,ref timer));
                if (exp.Denominator != null && exp.Denominator.Count > 0)
                {
                    this.Children.Add(GetDeviderLine());
                    this.Children.Add(GetDenominator(true,ref timer));
                }
            }
            else throw new NotImplementedException();

        }

        private void Prepare()
        {
            bool noDims = NoDenominator();

            if (noDims && NoDivisors(true))
            {
                this.Height = 60;
                this.Children.Add(GetNumerator(true, true));
                // no dims no divs
            }
            else if (!noDims && NoDivisors(false))
            {
                this.Height = 120;
                bool noDim = (exp.Denominator == null || exp.Denominator.Count == 0)? true:false;
                this.Children.Add(GetNumerator(noDim, true));
                if (exp.Denominator != null && exp.Denominator.Count > 0)
                {
                    this.Children.Add(GetDeviderLine());
                    this.Children.Add(GetDenominator(true));
                }
            }
            else throw new NotImplementedException();

        }

        public bool ShowNext()
        {
            WrapPanel panel = (WrapPanel)this.Children[0];
            for (int i = 0; i < panel.Children.Count; i++)
            {
                try
                {
                    ViewTerm term = (ViewTerm)panel.Children[i];

                    if (term.ShowNext()) return true;
                }
                catch
                {
                    Grid grid = (Grid)panel.Children[i];
                    if (grid.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        grid.Visibility = System.Windows.Visibility.Visible;
                        return true;
                    }
                }
            }
            
            if (this.Children.Count == 3)
            {
                WrapPanel panel2 = (WrapPanel)this.Children[2];
                for (int i = 0; i < panel2.Children.Count; i++)
                {
                    try
                    {
                        ViewTerm term = (ViewTerm)panel2.Children[i];

                        if (term.ShowNext()) return true;
                    }
                    catch
                    {
                        Grid grid = (Grid)panel2.Children[i];
                        if (grid.Visibility == System.Windows.Visibility.Collapsed)
                        {
                            grid.Visibility = System.Windows.Visibility.Visible;
                            return true;
                        }
                    }
                }
            }
            
            return false;

        }
        public bool HidAll()
        {
            WrapPanel panel = (WrapPanel)this.Children[0];
            for (int i = 0; i < panel.Children.Count; i++)
            {
                try
                {
                    ViewTerm term = (ViewTerm)panel.Children[i];
                    term.HideAll();
                }
                catch
                {
                    Grid grid = (Grid)panel.Children[i];
                    grid.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            if (this.Children.Count == 3)
            {
                WrapPanel panel2 = (WrapPanel)this.Children[2];
                for (int i = 0; i < panel2.Children.Count; i++)
                {
                    try
                    {
                        ViewTerm term = (ViewTerm)panel2.Children[i];
                        term.HideAll();
                    }
                    catch
                    {
                        Grid grid = (Grid)panel2.Children[i];
                        grid.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }

            return false;

        }
        // 
        private WrapPanel GetNumerator(bool noDims, bool noDivs, ref DispatcherTimer timer)
        {
            WrapPanel panel = new WrapPanel();


            List<ViewPropeties> prop = new List<ViewPropeties>();
            panel.Height = 60;

            if (noDims && noDivs)
            {
                prop = new List<ViewPropeties>(
                new ViewPropeties[] { ViewPropeties.NoDenominators, ViewPropeties.NoDivisors });
                panel.Margin = new Thickness(0, 15, 0, 0);
            }
            else if (!noDims && noDivs)
            {
                prop = new List<ViewPropeties>(
                new ViewPropeties[] { ViewPropeties.NoDivisors });
                panel.Margin = new Thickness(0, -14, 0, 0);
            }

            for (int i = 0; i < exp.Numerator.Count; i++)
            {
                if (exp.Numerator[i] != null && exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    List<ViewPropeties> propCurr = new List<ViewPropeties>(prop);

                    if (IsFirst(i, true))
                        propCurr.Add(ViewPropeties.IsFirst);
                    _myTerms.Add(new ViewTerm((Term)exp.Numerator[i], propCurr.ToArray(), ref timer));
                    panel.Children.Add(_myTerms[_myTerms.Count - 1]);
                }
                else if (exp.Numerator[i]   != null && exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    Grid grid = new Grid();
                    Grid inner = new Grid();
                    if (noDims && noDivs)
                    {
                        grid.Width = 15;
                        grid.Height = 60;
                        inner.Width = 9;
                        inner.Height = 20;
                        inner.Margin = new Thickness(0, -45, 0, 0);
                    }
                    else if (!noDims && noDivs)
                    {
                        grid.Width = 30;
                        grid.Height = 60;
                        inner.Width = 9;
                        inner.Height = 20;
                        inner.Margin = new Thickness(0, -45, 0, 0);
                    }
                    if (((Brace)exp.Numerator[i]).Key == ')')
                        inner.Background = GetBackGround("closeBrace");
                    else inner.Background = GetBackGround("openBrace");
                    grid.Children.Add(inner);
                    panel.Children.Add(grid);

                    if (((Brace)exp.Numerator[i]).Selected)
                    {
                        _mySelect.Add(new SelectionCursor(SelectedPieceType.brace, 0));
                        _mySelect[_mySelect.Count - 1].Active = true;
                        panel.Children.Add(_mySelect[_mySelect.Count - 1]);
                    }
                }
                //else throw new NotImplementedException();
            }


            return panel;
        }
        private WrapPanel GetNumerator(bool noDims, bool noDivs)
        {
            WrapPanel panel = new WrapPanel();


            List<ViewPropeties> prop = new List<ViewPropeties>();
                panel.Height = 60;

                if (noDims && noDivs)
                {
                    prop = new List<ViewPropeties>(
                    new ViewPropeties[] { ViewPropeties.NoDenominators, ViewPropeties.NoDivisors });
                    panel.Margin = new Thickness(0, 15, 0, 0);
                }
                else if (!noDims && noDivs)
                {
                    prop = new List<ViewPropeties>(
                    new ViewPropeties[] { ViewPropeties.NoDivisors });
                    panel.Margin = new Thickness(0, -14, 0, 0);
                }

                for (int i = 0; i < exp.Numerator.Count; i++)
                {
                    if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    {
                        List<ViewPropeties> propCurr = new List<ViewPropeties>(prop);

                        if (IsFirst(i, true))
                            propCurr.Add(ViewPropeties.IsFirst);
                        _myTerms.Add(new ViewTerm(
                            (Term)exp.Numerator[i], propCurr.ToArray()));
                        panel.Children.Add(_myTerms[_myTerms.Count - 1]);
                    }
                    else if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                    {
                        Grid grid = new Grid();
                        Grid inner = new Grid();
                        if (noDims && noDivs)
                        {
                            grid.Width = 15;
                            grid.Height = 60;
                            inner.Width = 9;
                            inner.Height = 20;
                            inner.Margin = new Thickness(0, -45, 0, 0);
                        }
                        else if (!noDims && noDivs)
                        {
                            grid.Width = 30;
                            grid.Height = 60;
                            inner.Width = 9;
                            inner.Height = 20;
                            inner.Margin = new Thickness(0, -45, 0, 0);
                        }
                        if (((Brace)exp.Numerator[i]).Key == ')')
                            inner.Background = GetBackGround("closeBrace");
                        else inner.Background = GetBackGround("openBrace");
                        grid.Children.Add(inner);
                        panel.Children.Add(grid);

                        if (((Brace)exp.Numerator[i]).Selected)
                        {
                            _mySelect.Add(new SelectionCursor(SelectedPieceType.brace, i));
                            panel.Children.Add(_mySelect[_mySelect.Count - 1]);
                        }
                    }
                    else throw new NotImplementedException();
                }


            return panel;
        }
        //index - yi term yesingaphi le okanye yi brace yesingaphi le....
        //innerIndex - the silected index on the actual piece(coe, termBase, power
        public bool Select(int index, int innerIndex, SelectedPieceType type1, ref DispatcherTimer timer, bool numerator)
        {
            bool answer = false;
            timer.Tick += new EventHandler(Toggle);
            DeSelect();

            if (type1 == SelectedPieceType.brace)
            {
                for (int i = 0; i < _mySelect.Count; i++)
                {

                    if (_mySelect[i].MySelectedPieceType == type1 && _mySelect[i].Index == index)
                    {
                        _mySelect[i].Active = true;
                        //_mySelect[i].Visibility = System.Windows.Visibility.Visible;
                        //timer.Tick += new EventHandler(Toggle);
                        answer = true;
                        break;
                    }
                }

            }
            else
            {
                for (int i = 0; i < _myTerms.Count; i++)
                {
                    if (i == index)
                    {
                        _myTerms[i].Select(type1, innerIndex, ref timer);
                        break;
                    }
                }
            }
            return answer;
        }
        List<ViewTerm> _myTerms = new List<ViewTerm>();
        private void Toggle(object sender, EventArgs e)
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].Active)
                {
                    if (_mySelect[i].Visibility == System.Windows.Visibility.Visible)
                        _mySelect[i].Visibility = System.Windows.Visibility.Collapsed;
                    else
                        _mySelect[i].Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        private void DeSelect()
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                _mySelect[i].Active = false;
                _mySelect[i].Visibility = System.Windows.Visibility.Collapsed;
            }
            for (int i = 0; i < this._myTerms.Count; i++)
            {
                _myTerms[i].DeSelect();
            }
        }
        private Grid GetDeviderLine()
        {
            Grid grid = new Grid();
            grid.Height = 4;
            grid.Margin = new Thickness(0, -17, 0, 0);
            grid.Background = GetBackGround("Line");
            return grid;
        }
        private WrapPanel GetDenominator( bool noDivs,ref DispatcherTimer timer)
        {
            WrapPanel panel = new WrapPanel();


            List<ViewPropeties> prop = new List<ViewPropeties>();
            panel.Height = 60;

            if ( noDivs)
            {
                prop = new List<ViewPropeties>(
                new ViewPropeties[] { ViewPropeties.NoDivisors });
                panel.Margin = new Thickness(0, 51, 0, 0);
            }

            for (int i = 0; i < exp.Denominator.Count; i++)
            {
                if (exp.Denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    List<ViewPropeties> propCurr = new List<ViewPropeties>(prop);
                    if (IsFirst(i, false))
                        propCurr.Add(ViewPropeties.IsFirst);
                    panel.Children.Add(new ViewTerm((Term)exp.Denominator[i], propCurr.ToArray(), ref timer));
                }
                else if (exp.Denominator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    Grid grid = new Grid();
                    Grid inner = new Grid();
                     if ( noDivs)
                    {
                        grid.Width = 30;
                        grid.Height = 60;
                        inner.Width = 9;
                        inner.Height = 20;
                        inner.Margin = new Thickness(0, -45, 0, 0);
                    }
                    if (((Brace)exp.Denominator[i]).Key == ')')
                        inner.Background = GetBackGround("closeBrace");
                    else inner.Background = GetBackGround("openBrace");
                    grid.Children.Add(inner);
                    panel.Children.Add(grid);
                }
                else throw new NotImplementedException();
            }


            return panel;
        }
        private WrapPanel GetDenominator(bool noDivs)
        {
            WrapPanel panel = new WrapPanel();


            List<ViewPropeties> prop = new List<ViewPropeties>();
            panel.Height = 60;

            if (noDivs)
            {
                prop = new List<ViewPropeties>(
                new ViewPropeties[] { ViewPropeties.NoDivisors });
                panel.Margin = new Thickness(0, 51, 0, 0);
            }

            for (int i = 0; i < exp.Denominator.Count; i++)
            {
                if (exp.Denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    List<ViewPropeties> propCurr = new List<ViewPropeties>(prop);
                    if (IsFirst(i, false))
                        propCurr.Add(ViewPropeties.IsFirst);
                    panel.Children.Add(new ViewTerm((Term)exp.Denominator[i], propCurr.ToArray()));
                }
                else if (exp.Denominator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    Grid grid = new Grid();
                    Grid inner = new Grid();
                    if (noDivs)
                    {
                        grid.Width = 30;
                        grid.Height = 60;
                        inner.Width = 9;
                        inner.Height = 20;
                        inner.Margin = new Thickness(0, -45, 0, 0);
                    }
                    if (((Brace)exp.Denominator[i]).Key == ')')
                        inner.Background = GetBackGround("closeBrace");
                    else inner.Background = GetBackGround("openBrace");
                    grid.Children.Add(inner);
                    panel.Children.Add(grid);
                }
                else throw new NotImplementedException();
            }


            return panel;
        }
        private bool IsFirst(int index, bool top)
        {

            if (index == 0)
                return true;
            else if (top)
            {
                if (exp.Numerator[index].GetTypePiece() == ExpressionPieceType.Term
                && (exp.Numerator[index - 1].GetTypePiece() == ExpressionPieceType.Sign
                || (exp.Numerator[index - 1].GetTypePiece() == ExpressionPieceType.Brace
                && ((Brace)exp.Numerator[index - 1]).Key == '(')))
                    return true;
            }
            else if (!top)
            {
                if (exp.Denominator[index].GetTypePiece() == ExpressionPieceType.Term
                && exp.Denominator[index - 1].GetTypePiece() == ExpressionPieceType.Sign
                || (exp.Denominator[index - 1].GetTypePiece() == ExpressionPieceType.Brace
                && ((Brace)exp.Denominator[index - 1]).Key == '('))
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
            for (int i = 0; i < this._viewProperties.Count; i++)
            {
                if (this._viewProperties[i] == property)
                    return true;
            }
            return false;
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

        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }

        #endregion Methods
    }
    public class ViewTerm : WrapPanel
    {
        #region Properties

        List<SelectionCursor> _mySelect = new List<SelectionCursor>();

        public List<SelectionCursor> MySelect
        {
            get { return _mySelect; }
            set { _mySelect = value; }
        }

        Term _myTerm;

        public Term MyTerm
        {
            get { return _myTerm; }
            set { _myTerm = value; }
        }

        List<ViewPropeties> _myProperties;

        public List<ViewPropeties> MyProperties
        {
            get { return _myProperties; }
            set { _myProperties = value; }
        }

        #endregion Properties


        #region Constructor

        public ViewTerm(Term source, ViewPropeties[] properties)
        {
            this._myTerm = source;

            if (properties != null)
                this._myProperties = new List<ViewPropeties>(properties);
            else this._myProperties = new List<ViewPropeties>();
            Prepare();
        }
        public ViewTerm(Term source, ViewPropeties[] properties, ref DispatcherTimer timer)
        {
            this._myTerm = source;

            if (properties != null)
                this._myProperties = new List<ViewPropeties>(properties);
            else this._myProperties = new List<ViewPropeties>();
            Prepare();
            if(source.Selected)
                Select(source.MySelectedPieceType, source.MySelectedindex, ref timer);
        }

        #endregion Constructor

        #region Methods

        #region Player Methods

        public void HideAll()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        public bool ShowNext()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i].Visibility == System.Windows.Visibility.Collapsed
                    && !DontTouch(this.Children[i]))
                {
                    this.Children[i].Visibility = System.Windows.Visibility.Visible;
                    return true;
                }
            }
            return false;
        }
        private bool DontTouch(UIElement grid)
        {
            try
            {
                SelectionCursor cursor = (SelectionCursor)grid;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Player Methods

        private void Prepare()
        {
            if (_myTerm.Root > 1)
            {
                //space waster
                Grid grid = new Grid();
                grid.Width = 20;

                if (!NoDenominators())
                    grid.Height = 29;
                else
                {
                    grid.Height = 16;
                    grid.Margin = new Thickness(0, -10, 0, 0);
                }
                this.Children.Add(grid);
                this.Background = GetBackGround("root");
            }
            _mySelect.Add(new SelectionCursor(SelectedPieceType.Sign, -1));
            this.Children.Add(_mySelect[_mySelect.Count - 1]);

            if ((!IsFirst() || _myTerm.Sign == "-" || _myTerm.TwoSigns || _myTerm.Joke) && !_myTerm.ExpressJoke)
            {
              this.Children.Add(GetSign());
              _mySelect.Add(new SelectionCursor(SelectedPieceType.Sign, 0));
              this.Children.Add(_mySelect[_mySelect.Count - 1]);
            }
            _mySelect.Add(new SelectionCursor(SelectedPieceType.Coefficient, -1));
            this.Children.Add(_mySelect[_mySelect.Count - 1]);

            if(MyTerm.Joke || _myTerm.ExpressJoke)
            _mySelect[_mySelect.Count - 1].Active = true;

            if (!MyTerm.Joke && !_myTerm.ExpressJoke)
            {
                #region Not joke
                if (_myTerm.Constant ||
                    _myTerm.CoEfficient != 1)
                {
                    int count = 0;
                    foreach (Grid myGrid in GetCoefficient())
                    {
                        this.Children.Add(myGrid);
                        _mySelect.Add(new SelectionCursor(SelectedPieceType.Coefficient, count));
                        this.Children.Add(_mySelect[_mySelect.Count - 1]);
                        count++;
                        //break;
                    }
                }
                _mySelect.Add(new SelectionCursor(SelectedPieceType.Base, -1));
                this.Children.Add(_mySelect[_mySelect.Count - 1]);

                if (!_myTerm.Constant)
                {
                    this.Children.Add(GetTermBase());
                    _mySelect.Add(new SelectionCursor(SelectedPieceType.Base, 0));
                    this.Children.Add(_mySelect[_mySelect.Count - 1]);
                }
                _mySelect.Add(new SelectionCursor(SelectedPieceType.Power, -1));
                this.Children.Add(_mySelect[_mySelect.Count - 1]);

                if (_myTerm.Power > 1 || _myTerm.PowerSign == "-")
                {
                    List<Grid> items = GetPower(_myTerm.PowerSign, _myTerm.Power);

                    for (int i = 0; i < items.Count; i++)
                    {
                        this.Children.Add(items[i]);
                        _mySelect.Add(new SelectionCursor(SelectedPieceType.Power, i));
                        this.Children.Add(_mySelect[_mySelect.Count - 1]);
                    }
                }

                if (_myTerm.MultipledBy != null && _myTerm.MultipledBy.Count > 0)
                {

                    List<Grid> items = GetMultiples();

                    for (int i = 0; i < items.Count; i++)
                    {
                        this.Children.Add(items[i]);
                    }
                }

                _mySelect.Add(new SelectionCursor(SelectedPieceType.OutSide, -2));
                this.Children.Add(_mySelect[_mySelect.Count - 1]);

                #endregion Not Joke
            }

            //
        }
        private void GetDenominator()
        {
            throw new NotImplementedException();
        }
        private List<Grid> GetMultiples()
        {
            List<Grid> answer = new List<Grid>();

            for (int i = 0; i < _myTerm.MultipledBy.Count; i++)
            {
                answer.Add(GetTermMultipleSign());
                _mySelect.Add(new SelectionCursor(SelectedPieceType.Sign, 0));
                this.Children.Add(_mySelect[_mySelect.Count - 1]);
                answer.Add(GetTermBase(_myTerm.MultipledBy[i].TermBase.ToString()));
                _mySelect.Add(new SelectionCursor(SelectedPieceType.Base, i));
                this.Children.Add(_mySelect[_mySelect.Count - 1]);

                if (_myTerm.MultipledBy[i].Power > 1 || _myTerm.MultipledBy[i].PowerSign == "-")
                {
                    answer.AddRange(GetPower(_myTerm.MultipledBy[i].PowerSign,
                        _myTerm.MultipledBy[i].Power));
                    _mySelect.Add(new SelectionCursor(SelectedPieceType.Power, i));
                    this.Children.Add(_mySelect[_mySelect.Count - 1]);
                }
            }

            return answer;
        }
        private List<Grid> GetPower(string powerSign, int powerV)
        {
            List<Grid> answer = new List<Grid>();

            if (powerSign == "-")
            {
                Grid pSign = new Grid();
                Grid pSignInner = new Grid();

                //if (NoDenominators() && NoDevisors())
                //{
                    pSign.Width = 7;
                    pSign.Height = 16;
                    pSignInner.Height = 2;
                    pSignInner.Width = 9;
                    pSignInner.Margin = new Thickness(0, -12, 0, 0);
                    pSignInner.Background = GetBackGround("minus");
                //}
                //else throw new NotImplementedException();

                pSign.Children.Add(pSignInner);
                answer.Add(pSign);
            }
            foreach (char myChar in powerV.ToString())
            {
                Grid power = new Grid();
                Grid powerInner = new Grid();
                power.Width = 15;
                power.Height = 16;
                powerInner.Width = 9;
                powerInner.Height = 10;
                powerInner.Margin = new Thickness(0, -12, 0, 0);

                powerInner.Background = GetBackGround(myChar.ToString());

                power.Children.Add(powerInner);
                answer.Add(power);
            }

            return answer;
        }
        private Grid GetTermBase()
        {
            return GetTermBase(_myTerm.TermBase.ToString());
        }
        private Grid GetTermBase(string baseValue)
        {
            Grid grid = new Grid();

            //if ((NoDevisors() && NoDenominators() ||
            //    (NoDevisors() && !NoDenominators())))
            //{
                grid.Height = 20;
                grid.Width = 20;
           // }
            //else throw new NotImplementedException();

            grid.Background = GetBackGround(baseValue);
            return grid;
        }
        private Grid GetTermMultipleSign()
        {
            Grid grid = new Grid();
            Grid inner = new Grid();
            if (NoDevisors() && NoDenominators())
            {
                grid.Width = 20;
                grid.Height = 20;
                inner.Width = 7;
                inner.Height = 7;
                inner.Margin = new Thickness(0, 0, 0, 0);
            }
            else throw new NotImplementedException();

            inner.Background = GetBackGround("termMultiply");
            grid.Children.Add(inner);
            return grid;
        }
        private List<Grid> GetCoefficient()
        {
            List<Grid> answer = new List<Grid>();

            foreach (char myChar in _myTerm.CoEfficient.ToString())
            {
                answer.Add(GetTermBase(myChar.ToString()));
            }
            return answer;
        }
        private Grid GetSign()
        {
            Grid grid = new Grid();
            Grid inner = new Grid();

            //if (NoDevisors() && NoDenominators())
            //{
                grid.Width = 20;
                grid.Height = 20;

                inner.Height = 9;
                inner.Width = 9;
                if (_myTerm.Sign == "-")
                    inner.Height = 3;

                if (!MyTerm.TwoSigns)
                {
                    inner.Background = GetBackGround(FixSign(new Sign(_myTerm.Sign)));
                }
                else inner.Background = GetBackGround("plusminus");
            //}
            //else throw new NotImplementedException();
            grid.Children.Add(inner);
            return grid;
        }
        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        private string FixSign(Sign signType)
        {
            if (signType.SignType == SignType.Add)
                return "plus";
            else if (signType.SignType == SignType.Subtract)
                return "minus";
            else if (signType.SignType == SignType.Divide)
                return "divide";
            else if (signType.SignType == SignType.equal)
                return "equal";
            else if (signType.SignType == SignType.greater)
                return "greaterthan";
            else if (signType.SignType == SignType.greaterEqual)
                return "greaterOrEqual";
            else if (signType.SignType == SignType.Multiply)
                return "multiply";
            else if (signType.SignType == SignType.OR)
                return "or";
            else if (signType.SignType == SignType.smaller)
                return "smallerthan";
            else if (signType.SignType == SignType.smallerEqual)
                return "smallerOrEqual";
            throw new NotImplementedException();
        }
        private bool IsFirst()
        {
            return PropertyTrue(ViewPropeties.IsFirst);
        }
        private bool NoDevisors()
        {
            return PropertyTrue(ViewPropeties.NoDivisors);
        }
        private bool NoDenominators()
        {
            return PropertyTrue(ViewPropeties.NoDenominators);
        }
        private bool PropertyTrue(ViewPropeties property)
        {
            for (int i = 0; i < _myProperties.Count; i++)
            {
                if (_myProperties[i] == property)
                    return true;
            }
            return false;
        }
        public bool Select(SelectedPieceType type1, int index, ref DispatcherTimer timer)
        {
            bool answer = false;
            DeSelect();

            for (int i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].MySelectedPieceType == type1 && _mySelect[i].Index == index)
                {
                    _mySelect[i].Active = true;
                    _mySelect[i].Visibility = System.Windows.Visibility.Visible;
                    timer.Tick += new EventHandler(Toggle);
                    answer = true;
                    break;
                }
         
            }
            return answer;
        }
        private void Toggle(object sender, EventArgs e)
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].Active)
                {
                    if (_mySelect[i].Children[0].Visibility == System.Windows.Visibility.Visible)
                        _mySelect[i].Children[0].Visibility = System.Windows.Visibility.Collapsed;
                    else
                        _mySelect[i].Children[0].Visibility = System.Windows.Visibility.Visible; 
                }
            }
        }
        public void DeSelect()
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                _mySelect[i].Active = false;
                _mySelect[i].Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        #endregion Methods
    }
    public class ExpoStep : WrapPanel
    {
        #region Properties

        #endregion Properties

        #region Constructor

        public ExpoStep(ExponentialEquation eq)
        {

        }

        #endregion Constructor

        #region Methods

        #endregion Methods
    }
    public class ViewExpoTerm : WrapPanel
    {
        #region Properties

        List<SelectionCursor> _mySelect = new List<SelectionCursor>();

        public List<SelectionCursor> MySelect
        {
            get { return _mySelect; }
            set { _mySelect = value; }
        }

        #endregion Properties

        #region Constructor

        public ViewExpoTerm(ExponentialTerm term)
        {
            ShapeMe();
            if (term.Status == ExpontentialStatus._double)
                Attach(DoubleStatus(term));
            else if (term.Status == ExpontentialStatus._exponential)
                Attach(ExpoStatus(term));
            else if (term.Status == ExpontentialStatus._expression)
                Attach(ExpressionStatus(term));
            else if (term.Status == ExpontentialStatus._integer)
                Attach(IntegerStatus(term));
            else if (term.Status == ExpontentialStatus._logarithmic)
                Attach(LogStatus(term));
            else if (term.Status == ExpontentialStatus._logarithmicWithDivisor)
                Attach(LogWithDivStatus(term));
            else if (term.Status == ExpontentialStatus.expressionWithoutLog)
                Attach(ExpressionWithoutLogStatus(term));
            else if (term.Status == ExpontentialStatus.variable)
                Attach(VariableStatus(term));
            
        }
        public ViewExpoTerm(ExponentialTerm term, ref DispatcherTimer timer)
        {
            ShapeMe();
            if (term.Status == ExpontentialStatus._double)
                Attach(DoubleStatus(term));
            else if (term.Status == ExpontentialStatus._exponential)
                Attach(ExpoStatus(term, ref timer));
            else if (term.Status == ExpontentialStatus._expression)
                Attach(ExpressionStatus(term));
            else if (term.Status == ExpontentialStatus._integer)
                Attach(IntegerStatus(term, ref timer));
            else if (term.Status == ExpontentialStatus._logarithmic)
                Attach(LogStatus(term));
            else if (term.Status == ExpontentialStatus._logarithmicWithDivisor)
                Attach(LogWithDivStatus(term));
            else if (term.Status == ExpontentialStatus.expressionWithoutLog)
                Attach(ExpressionWithoutLogStatus(term));
            else if (term.Status == ExpontentialStatus.variable)
                Attach(VariableStatus(term));

            if (term.Selected)
                Select(term.MySelectedPieceType, term.MySelectedindex, ref timer);

        }

        #endregion Constructor

        #region Methods
        private void ShapeMe()
        {
            this.Height = 80;
        }
        private void Attach(List<UIElement> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
                this.Children.Add(pieces[i]);
        }
        private List<UIElement> ExpoStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();

            foreach (char value in term.TermBase.CoEfficient.ToString())
            {
                answer.Add(GetTermBase(int.Parse(value.ToString())));
 
            }
            answer.Add(GetPower(term.Power));

            return answer;
        }
        private List<UIElement> ExpoStatus(ExponentialTerm term, ref DispatcherTimer timer)
        {
            List<UIElement> answer = new List<UIElement>();

            foreach (char value in term.TermBase.CoEfficient.ToString())
            {
                answer.Add(GetTermBase(int.Parse(value.ToString())));

            }
           

            answer.Add(GetPower(term.Power));

            return answer;
        }
        private List<UIElement> LogStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();

            answer.Add(GetLog());

            foreach (char value in term.TermBase.CoEfficient.ToString())
            {
                answer.Add(GetTermBase(int.Parse(value.ToString())));
            }
            if(term.Power != null && term.Power.Numerator != null)
            answer.Add(GetPower(term.Power));

            return answer;
        }
        private List<UIElement> LogWithDivStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();

            
            answer.Add(GetLog());
            foreach (char value in term.TermBase.CoEfficient.ToString())
            {
                answer.Add(GetTermBase(int.Parse(value.ToString())));
            }
            answer.Add(GetBaseSign(new Sign(SignType.Divide)));

            answer.Add(GetLog());

            foreach (char value in term.Divisor.TermBase.CoEfficient.ToString())
            {
                answer.Add(GetTermBase(int.Parse(value.ToString())));
            }

            return answer;
        }
        private List<UIElement> DoubleStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();
            string value = Math.Round(term._Double,5).ToString();

            for (int i = 0; i < value.Length; i++)
            {
                int cuValue;

                if (int.TryParse(value[i].ToString(), out cuValue))
                {
                    string pieces = cuValue.ToString();
                    answer.Add(GetTermBase(int.Parse(value[i].ToString())));
                }
                else
                {
                    answer.Add(GetDot());
                }
            }
            return answer;
        }
        private List<UIElement> ExpressionStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();

            answer.Add(GetBrace(true));
            answer.AddRange(GetExpression(term.Power));
            answer.Add(GetBrace(false));
            answer.Add(GetBrace(true));
            answer.Add(GetLog());
            foreach (char value in term.TermBase.CoEfficient.ToString())
            {
                answer.Add(GetTermBase(int.Parse(value.ToString())));
            }
            answer.Add(GetBrace(false));
            return answer;
        }
        private List<UIElement> ExpressionWithoutLogStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();

            answer.Add(GetBrace(true));
            answer.AddRange(GetExpression(term.Power));
            answer.Add(GetBrace(false));
            answer.Add(GetBrace(true));
            answer.Add(GetTermBase(1));
            answer.Add(GetBrace(false));
            return answer;
        }
        private List<UIElement> VariableStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();

            //answer.Add(GetBrace(true));
            answer.AddRange(GetExpression(term.Power));
            //answer.Add(GetBrace(false));
            //answer.Add(GetBrace(true));
            //answer.Add(GetTermBase(1));
            //answer.Add(GetBrace(false));
            return answer;
        }
        private List<UIElement> IntegerStatus(ExponentialTerm term)
        {
            List<UIElement> answer = new List<UIElement>();

            string pieces = term.TermBase.CoEfficient.ToString();

            foreach (char value in pieces)
            {
                answer.Add(GetTermBase(int.Parse(value.ToString())));
            }

            if (term.TermBase.Power != 1)
            {
                MathBase.Expression exp = new MathBase.Expression();
                exp.AddToExpression(true, new Term(term.TermBase.Power));
                answer.Add(GetPower(exp));
            }

            return answer;
        }
        private List<UIElement> IntegerStatus(ExponentialTerm term, ref DispatcherTimer timer)
        {
            List<UIElement> answer = new List<UIElement>();

            string pieces = term.TermBase.CoEfficient.ToString();

            if (!term.Joke && !term.ExpressJoke)
            {
                foreach (char value in pieces)
                {
                    answer.Add(GetTermBase(int.Parse(value.ToString())));

                }
                _mySelect.Add(new SelectionCursor(SelectedPieceType.BaseExpo, -1));

                if (term.Selected)
                {
                    //_mySelect[_mySelect.Count - 1].Active = true;
                    //answer.Add(_mySelect[_mySelect.Count - 1]);
                }
            }
            else
            {
                _mySelect.Add(new SelectionCursor(SelectedPieceType.BaseExpo, -1));
                //_mySelect[_mySelect.Count - 1].Active = true;
                //answer.Add(_mySelect[_mySelect.Count - 1]);
            }
            return answer;
        }
        private Grid GetDot()
        {
            Grid grid = new Grid();
            grid.Height = 80;
            grid.Width = 18;

            Grid inner = new Grid();
            inner.Width = 12;
            inner.Height = 12;
            inner.Margin = new Thickness(0, 29, 0, 0);
            inner.Background = GetBackGround("termmultiply");
            grid.Children.Add(inner);
            return grid;
        }
        private Grid GetLog()
        {
            Grid grid = new Grid();
            grid.Width = 60;
            grid.Height = 80;
            Grid inner = new Grid();
            inner.Height = 40;
            inner.Width = 60;
            inner.Background = GetBackGround("Log");
            grid.Children.Add(inner);
            return grid;
        }
        private Grid GetTermBase(int value)
        {
            Grid grid = new Grid();
            grid.Height = 80;
            grid.Width = 40;
            Grid inner = new Grid();
            inner.Height = 43;
            inner.Width = 40;
            inner.Background = GetBackGround(value.ToString());
            grid.Children.Add(inner);
            return grid;
        }
        private Grid GetTermBase(string value)
        {
            Grid grid = new Grid();
            grid.Height = 80;
            grid.Width = 40;
            Grid inner = new Grid();
            inner.Height = 43;
            inner.Width = 40;
            inner.Background = GetBackGround(value);
            grid.Children.Add(inner);
            return grid;
        }
        private WrapPanel GetPower(MathBase.Expression exp)
        {
            WrapPanel panel = new WrapPanel();
            panel.Height = 30;
            panel.Margin = new Thickness(0, -34, 0, 0);

            for (int i = 0; i < exp.Numerator.Count; i++)
            {
                if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (i != 0 && exp.Numerator[i -1].GetTypePiece() !=  ExpressionPieceType.Brace)
                    {
                        panel.Children.Add(GetPowersSign(new Sign (((Term)exp.Numerator[i]).Sign)));
                    }

                    foreach (UIElement el in GetPowerTerm((Term)exp.Numerator[i]))
                    {
                        panel.Children.Add(el);
                    }
                }
                else if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Sign)
                {
                    panel.Children.Add(GetPowersSign((Sign)exp.Numerator[i]));
                }
                else if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (((Brace)exp.Numerator[i]).Key == '(')
                        panel.Children.Add(GetBracePower(true));
                    else panel.Children.Add(GetBracePower(false));
                }
                else throw new NotImplementedException();
            }
            return panel;
        }
        private Grid GetBracePower(bool opening)
        {
            
            Grid inner = new Grid();
            inner.Height = 20;
            inner.Width = 10;
            if (opening)
                inner.Background = GetBackGround("openBrace");
            else inner.Background = GetBackGround("closeBrace");
            return inner;  
        }
        private List<UIElement> GetExpression(MathBase.Expression exp)
        {
            List<UIElement> answer = new List<UIElement>();

            for (int i = 0; i < exp.Numerator.Count; i++)
            {
                if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (i != 0 && exp.Numerator[i - 1].GetTypePiece() != ExpressionPieceType.Brace)
                    {
                        answer.Add(GetBaseSign(new Sign(((Term)exp.Numerator[i]).Sign)));
                    }
                    foreach (UIElement el in GetTerm((Term)exp.Numerator[i]))
                    {
                        answer.Add(el);
                    }
                }
                else if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Sign)
                {
                    answer.Add(GetBaseSign((Sign)exp.Numerator[i]));
                }
                else if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (((Brace)exp.Numerator[i]).Key == ')')
                        answer.Add(GetBrace(false));
                    else answer.Add(GetBrace(true));
                }
                else throw new NotImplementedException();
            }
            return answer;
        }
        private List<UIElement> GetTerm(Term term)
        {
            List<UIElement> answer = new List<UIElement>();

            if (term.Constant || term.CoEfficient > 1)
            {
                foreach(char cur in term.CoEfficient.ToString())
                {
                answer.Add(GetTermBase(int.Parse(cur.ToString())));
                }
            }
            if (!term.Constant)
            {
                answer.Add(GetTermBase(term.TermBase.ToString()));
            }

            if (term.Power > 1)
            {
                MathBase.Expression exp = new MathBase.Expression();
                foreach (char cur in term.Power.ToString())
                {
                    exp.Numerator.Add(new Term(int.Parse(cur.ToString())));

                }
                answer.Add(GetPower(exp));
            }

            return answer;
        }
        private List<UIElement> GetPowerTerm(Term term)
        {
            List<UIElement> answer = new List<UIElement>();

            if (term.Constant || term.CoEfficient > 1)
            {
                foreach(char cur in term.CoEfficient.ToString())
                {
                answer.Add(GetPowerBase(cur.ToString()));
                }
            }
            if (!term.Constant)
            {
                answer.Add(GetPowerBase(term.TermBase.ToString()));
            }

            if(term.Power > 1)
            {
                foreach(char cur in term.Power.ToString())
                {
                    answer.Add(GetPowersPower(int.Parse(cur.ToString())));
                }
            }
            return answer;
        }
        private Grid GetBaseSign(Sign signType)
        {
            Grid grid = new Grid();
            grid.Width = 30;
            grid.Height = 80;
            Grid inner = new Grid();
            inner.Height = 11;
            inner.Width = 20;
            inner.Background = GetBackGround(FixSign(signType));
            grid.Children.Add(inner);
            return grid;
        }
        private Grid GetPowerBase(string sig)
        {
            Grid grid = new Grid();
            grid.Width = 20;
            grid.Height = 20;
            //Grid inner = new Grid();
            //inner.Height = 10;
            //inner.Width = 10;
            //inner.Margin = new Thickness(0, -15, 0, 0);
            grid.Background = GetBackGround(sig);
            //grid.Children.Add(inner);
            return grid;
        }
        private Grid GetPowersSign(Sign sign)
        {
            Grid grid = new Grid();
            grid.Height = 20;
            grid.Width = 20;
            Grid inner = new Grid();
            inner.Height = 5;
            inner.Width = 10;
            inner.Background = GetBackGround(FixSign(sign));
            grid.Children.Add(inner);
            return grid;
        }
        private Grid GetPowersPower(int value)
        {
            Grid grid = new Grid();
            grid.Width = 20;
            grid.Height = 20;
            Grid inner = new Grid();
            inner.Height = 10;
            inner.Width = 10;
            inner.Margin = new Thickness(0, -15, 0, 0);
            inner.Background = GetBackGround(value.ToString());
            grid.Children.Add(inner);
            return grid;
        }
        private Grid GetBrace(bool opening)
        {
            Grid grid = new Grid();
            grid.Width = 40;
            grid.Height = 80;
            Grid inner = new Grid();
            inner.Height = 43;
            inner.Width = 20;
            if (opening)
                inner.Background = GetBackGround("openBrace");
            else inner.Background = GetBackGround("closeBrace");
            grid.Children.Add(inner);
            return grid;       
        }
        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        private string FixSign(Sign signType)
        {
            if (signType.SignType == SignType.Add)
                return "plus";
            else if (signType.SignType == SignType.Subtract)
                return "minus";
            else if (signType.SignType == SignType.Divide)
                return "divide";
            else if (signType.SignType == SignType.equal)
                return "equal";
            else if (signType.SignType == SignType.greater)
                return "greaterthan";
            else if (signType.SignType == SignType.greaterEqual)
                return "greaterOrEqual";
            else if (signType.SignType == SignType.Multiply)
                return "multiply";
            else if (signType.SignType == SignType.OR)
                return "or";
            else if (signType.SignType == SignType.smaller)
                return "smallerthan";
            else if (signType.SignType == SignType.smallerEqual)
                return "smallerOrEqual";
            throw new NotImplementedException();
        }
        public bool Select(SelectedPieceType type1, int index, ref DispatcherTimer timer)
        {
            bool answer = false;
            DeSelect();

            for (int i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].MySelectedPieceType == type1 && _mySelect[i].Index == index)
                {
                    _mySelect[i].Active = true;
                    _mySelect[i].Visibility = System.Windows.Visibility.Visible;
                    timer.Tick += new EventHandler(Toggle);
                    answer = true;
                    break;
                }

            }
            return answer;
        }
        private void Toggle(object sender, EventArgs e)
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].Active)
                {
                    if (_mySelect[i].Children[0].Visibility == System.Windows.Visibility.Visible)
                        _mySelect[i].Children[0].Visibility = System.Windows.Visibility.Collapsed;
                    else
                        _mySelect[i].Children[0].Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public void DeSelect()
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                _mySelect[i].Active = false;
                _mySelect[i].Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        #endregion Methods
    }
    public class ViewLogTerm : WrapPanel
    {
        #region Properties

        LogTerm _myTerm;

        #endregion Properties

        #region Constructor

        public ViewLogTerm(LogTerm term)
        {
            _myTerm = term;
            Prepare();
        }
        public ViewLogTerm(LogTerm term, ref DispatcherTimer timer)
        {
            _myTerm = term;
            Prepare(ref timer);
        }

        #endregion Constructor

        #region Methods
        private void Prepare()
        {
            if(_myTerm.Coefficient != 1)
            this.Children.Add(GetCoEfficient());

            if(this._myTerm.IsComplete || this._myTerm.ShowLog)
            this.Children.Add(GetLog());
            if(this._myTerm.IsComplete || this._myTerm.ShowBase)
            this.Children.Add(GetLogBase());
            if(this._myTerm.IsComplete)
            if(_myTerm.Power != null && _myTerm.Power.Numerator != null
                && _myTerm.Power.Numerator.Count > 0)
            this.Children.Add(GetPower());
        }

        private void Prepare(ref DispatcherTimer timer)
        {
            if (_myTerm.Coefficient != 1)
                this.Children.Add(GetCoEfficient());

            if (this._myTerm.IsComplete || this._myTerm.ShowLog)
                this.Children.Add(GetLog());
            if (this._myTerm.IsComplete || this._myTerm.ShowBase)
                this.Children.Add(GetLogBase());
            if (this._myTerm.IsComplete)
                if (_myTerm.Power != null && _myTerm.Power.Numerator != null
                    && _myTerm.Power.Numerator.Count > 0)
                    this.Children.Add(GetPower(ref timer));
        }

        private WrapPanel GetCoEfficient()
        {
            WrapPanel panel = new WrapPanel();
            panel.Height = 60;

            foreach (char myChar in _myTerm.Coefficient.ToString())
            {
                Grid grid = new Grid();
                grid.Height = 60;
                Grid inner = new Grid();
                inner.Height = 30;
                inner.Width = 30;
                inner.Margin = new Thickness(0, -10, 0, 0);
                inner.Background = GetBackGround(myChar.ToString());
                grid.Children.Add(inner);
                panel.Children.Add(grid);
            }

            return panel;
        }
        private Grid GetLog()
        {
            Grid grid = new Grid();
            grid.Width = 50;
            grid.Height = 60;
            Grid inner = new Grid();
            inner.Height = 30;
            inner.Margin = new Thickness(0, 0, 0, 0);
            inner.Background = GetBackGround("log");
            grid.Children.Add(inner);
            return grid;
        }
        private Grid GetLogBase()
        {
            Grid grid = new Grid();
            grid.Width = 30;
            grid.Height = 60;
            Grid inner = new Grid();
            inner.Height = 25;
            inner.Width = 24;
            inner.Margin = new Thickness(0, 25, 0, 0);
            inner.Background = GetBackGround(_myTerm.LogBase.ToString());
            grid.Children.Add(inner);
            return grid;
        }
        private WrapPanel GetPower()
        {
            WrapPanel panel = new WrapPanel();
            panel.Height = 60;

            if (_myTerm.Power != null)
            {
                if (_myTerm.Power.Numerator.Count > 1)
                    panel.Children.Add(GetBrace(true));

                for (int i = 0; i < _myTerm.Power.Numerator.Count; i++)
                {
                    if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    {
                        if (i != 0 && _myTerm.Power.Numerator[i - 1].GetTypePiece() != ExpressionPieceType.Brace)
                        {
                            panel.Children.Add(GetInnerSign(new Sign(((Term)_myTerm.Power.Numerator[i]).Sign)));
                        }

                        List<UIElement> pieces = GetPowerElement((Term)_myTerm.Power.Numerator[i]);

                        for (int k = 0; k < pieces.Count; k++)
                            panel.Children.Add(pieces[k]);

                    }
                    else if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                    {
                        if (((Brace)_myTerm.Power.Numerator[i]).Key == ')')
                            panel.Children.Add(GetBrace(false));
                        else panel.Children.Add(GetBrace(true));
                    }
                    else throw new NotImplementedException();
                }

                if (_myTerm.Power.Numerator.Count > 1)
                    panel.Children.Add(GetBrace(false));
            }
            return panel;
        }
        private WrapPanel GetPower(ref DispatcherTimer timer)
        {
            WrapPanel panel = new WrapPanel();
            panel.Height = 60;

            if (_myTerm.Power != null)
            {
                //if (_myTerm.Power.Numerator.Count > 1)
                    //panel.Children.Add(GetBrace(true));

                for (int i = 0; i < _myTerm.Power.Numerator.Count; i++)
                {
                    if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    {
                        if ((i != 0 && _myTerm.Power.Numerator[i - 1].GetTypePiece() != ExpressionPieceType.Brace)
                            || ((Term)_myTerm.Power.Numerator[i]).Joke)
                        {
                            panel.Children.Add(GetInnerSign(new Sign(((Term)_myTerm.Power.Numerator[i]).Sign)));
                        }


                        List<UIElement> pieces = GetPowerElement((Term)_myTerm.Power.Numerator[i]);

                        if(((Term)_myTerm.Power.Numerator[i]).Joke == false)
                            for (int k = 0; k < pieces.Count; k++)
                                panel.Children.Add(pieces[k]);

                    }
                    else if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                    {
                        if (((Brace)_myTerm.Power.Numerator[i]).Key == ')')
                            panel.Children.Add(GetBrace(false));
                        else panel.Children.Add(GetBrace(true));
                    }
                    else throw new NotImplementedException();
                }

                //if (_myTerm.Power.Numerator.Count > 1)
                    //panel.Children.Add(GetBrace(false));
            }
            return panel;
        }
        private List<UIElement> GetPowerElement(Term term)
        {
            List<UIElement> answer = new List<UIElement>();

            if (term.CoEfficient > 1 || term.Constant)
            {
                answer.AddRange(PowerElementBase(term.CoEfficient.ToString()));
            }

            if (term.Constant == false)
            {
                answer.AddRange(PowerElementBase(term.TermBase.ToString()));
            }

            if (term.Power > 1)
            {
                answer.AddRange(PowerElementBasePower(term.Power.ToString()));
            }
            return answer;
        }
        private List<UIElement> PowerElementBasePower(string value)
        {
            List<UIElement> answer = new List<UIElement>();

            foreach (char myChar in value)
            {
                Grid grid = new Grid();
                grid.Height = 60;
                Grid inner = new Grid();
                inner.Height = 15;
                inner.Width = 14;
                inner.Margin = new Thickness(0, -22, 0, 0);
                inner.Background = GetBackGround(myChar.ToString());
                grid.Children.Add(inner);
                answer.Add(grid);
            }
            return answer;
        }
        private List<UIElement> PowerElementBase(string value)
        {
            List<UIElement> answer = new List<UIElement>();

            foreach (char myChar in value)
            {
                Grid grid = new Grid();
                grid.Height = 60;
                Grid inner = new Grid();
                inner.Height = 30;
                inner.Width = 30;
                inner.Margin = new Thickness(0, -1, 0, 0);
                inner.Background = GetBackGround(myChar.ToString());
                grid.Children.Add(inner);
                answer.Add(grid);
            }
            return answer;
        }
        private Grid GetInnerSign(Sign sign)
        {
            Grid grid = new Grid();
            grid.Height = 60;
            Grid inner = new Grid();
            inner.Height = 5;
            inner.Width = 15;
            inner.Margin = new Thickness(0, -1, 0, 0);
            inner.Background = GetBackGround(FixSign(sign));
            grid.Children.Add(inner);
            return grid;
        }
        private string FixSign(Sign signType)
        {
            if (signType.SignType == SignType.Add)
                return "plus";
            else if (signType.SignType == SignType.Subtract)
                return "minus";
            else if (signType.SignType == SignType.Divide)
                return "divide";
            else if (signType.SignType == SignType.equal)
                return "equal";
            else if (signType.SignType == SignType.greater)
                return "greaterthan";
            else if (signType.SignType == SignType.greaterEqual)
                return "greaterOrEqual";
            else if (signType.SignType == SignType.Multiply)
                return "multiply";
            else if (signType.SignType == SignType.OR)
                return "or";
            else if (signType.SignType == SignType.smaller)
                return "smallerthan";
            else if (signType.SignType == SignType.smallerEqual)
                return "smallerOrEqual";
            throw new NotImplementedException();
        }
        private Grid GetBrace(bool Opening)
        {
            Grid grid = new Grid();
            grid.Height = 60;
            Grid inner = new Grid();
            inner.Width = 15;
            inner.Height = 30;
            if (Opening)
                inner.Background = GetBackGround("openbrace");
            else inner.Background = GetBackGround("closebrace");
            inner.Margin = new Thickness(0,-1,0,0);
            grid.Children.Add(inner);
            return grid;

        }
        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }

        #endregion Methods
    }
    public class SelectionCursor : Grid
    {
        #region Properties

        SelectedPieceType _mySelectedPieceType;

        public SelectedPieceType MySelectedPieceType
        {
            get { return _mySelectedPieceType; }
            set { _mySelectedPieceType = value; }
        }

        int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        bool active = false;

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        #endregion Properties

        #region Constructor

        public SelectionCursor(SelectedPieceType type1, int index)
        {
            this.index = index;
            this._mySelectedPieceType = type1;
            this.Visibility = System.Windows.Visibility.Collapsed;
            this.active = false;

            if (_mySelectedPieceType == SelectedPieceType.Power)
            {
                this.Width = 5;
                this.Height = 10;
                Grid grid = new Grid();
                grid.Height = 9;
                grid.Width = 4;
                grid.Margin = new Thickness(0, -12, 0, 0);
                grid.Background = GetBackGround("Cursor");
                this.Children.Add(grid);
            }
            else if (_mySelectedPieceType == SelectedPieceType.brace)
            {
                Grid grid = new Grid();
                this.Width = 15;
                this.Height = 60;
                grid.Width = 7;
                grid.Height = 20;
                grid.Background = GetBackGround("Cursor");
                grid.Margin = new Thickness(0, -45, 0, 0);
                this.Children.Add(grid);
            }
            else if (_mySelectedPieceType == SelectedPieceType.BaseExpo)
            {
                Grid grid = new Grid();
                this.Height = 80;
                this.Width = 10;
                Grid inner = new Grid();
                inner.Height = 43;
                inner.Width = 10;
                inner.Background = GetBackGround("Cursor");
                this.Children.Add(inner);
            }
            else
            {
                Grid grid = new Grid();
                this.Height = 20;
                this.Width = 5;
                grid.Height = 20;
                grid.Width = 5;
                grid.Background = GetBackGround("Cursor");
                this.Children.Add(grid);
            }
        }
        public SelectionCursor(int index, int widthOuter,
            int widthInner, int heightOuter, int heightInner, Thickness inner)
        {
            this.index = index;
            this._mySelectedPieceType = SelectedPieceType.OutSide;// type1;
            this.Visibility = System.Windows.Visibility.Collapsed;
            this.active = false;

            this.Width = widthOuter;
            this.Height = heightOuter;
            Grid grid = new Grid();
            grid.Height = heightInner;
            grid.Width = widthInner;
            grid.Margin = inner;// new Thickness(0, -12, 0, 0);
            grid.Background = GetBackGround("Cursor");
            this.Children.Add(grid);
        }
        #endregion Constructor

        #region Methods
        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        #endregion Methods
    }
    //public enum SelectedPieceType
    //{
    //    Base, Coefficient, Power, Sign, brace,OutSide, Empty /*Next to the term*/
    //}
}
