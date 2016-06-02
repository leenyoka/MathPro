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
using System.Collections.Generic;
using System.Collections;

namespace MathBase
{
    
        #region Trig

        public enum TrigFunction
        {
            Sin, Cos, Tan, Cot, Sec, Csc
        }
        public enum TrigTermType
        {
            Term, Constant, TrigTerm
        }
        public enum TrigExpressionPieceType
        {
            Term, TrigTerm
        }
        public interface ITrigExpressionPiece
        {
            TrigExpressionPieceType GetPieceType();
        }
        public interface ITrigTerm
        {
            TrigTermType TrigTermType();
            bool AreEqual(ITrigTerm other);
        }
        public class TrigTerm : ITrigTerm, ITrigExpressionPiece
        {
            #region Properties

            TrigFunction _trigFun;
            public TrigFunction TrigFun
            {
                get { return _trigFun; }
                set { _trigFun = value; }
            }
            Term _term; //variable and power for the trigFunction
            public Term Term
            {
                get { return _term; }
                set { _term = value; }
            }
            List<ITrigTerm> multipliedBy;
            public List<ITrigTerm> MultipliedBy
            {
                get { return multipliedBy; }
                set { multipliedBy = value; }
            }
            int _coEfficient;
            public int CoEfficient
            {
                get { return _coEfficient; }
                set { _coEfficient = value; }
            }
            string _sign = "+";
            public string Sign
            {
                get { return _sign; }
                set { _sign = value; }
            }
            #endregion Properties

            #region Constructor

            public TrigTerm(char variable, TrigFunction fun)
            {
                this.Term = new Term(variable);
                this._trigFun = fun;
                this.CoEfficient = 1;
            }
            public TrigTerm(Term term, TrigFunction fun)
            {
                this.Term = new Term(term);
                this._trigFun = fun;
                this.CoEfficient = 1;
            }
            public TrigTerm(char variable, TrigFunction fun, int coef)
            {
                this.Term = new Term(variable);
                this._trigFun = fun;
                this.CoEfficient = coef;
            }
            public TrigTerm(Term term, TrigFunction fun, int coef)
            {
                this.Term = new Term(term);
                this._trigFun = fun;
                this.CoEfficient = coef;
            }

            public TrigTerm(TrigTerm parent)
            {
                this.TrigFun = parent.TrigFun;
                this.Term = new Term(parent.Term);
                this.multipliedBy = new List<ITrigTerm>();

                if(parent.MultipliedBy != null && parent.MultipliedBy.Count > 0)
                for (int i = 0; i < parent.multipliedBy.Count; i++)
                {
                    if (parent.multipliedBy[i].TrigTermType() ==  MathBase.TrigTermType.TrigTerm)
                        this.multipliedBy.Add(new TrigTerm((TrigTerm)parent.multipliedBy[i]));
                    else parent.multipliedBy.Add(new Term((Term)parent.multipliedBy[i]));
                }

                this.CoEfficient = parent.CoEfficient;
                this.Sign = parent.Sign;
            }

            #endregion Constructor

            #region Methods

            public TrigTermType TrigTermType()
            {
                return MathBase.TrigTermType.TrigTerm; 
            }

            public TrigExpressionPieceType GetPieceType()
            {
                return TrigExpressionPieceType.TrigTerm;
            }

            public bool AreEqual(ITrigTerm other)
            {
                if (this._coEfficient == ((TrigTerm)other)._coEfficient
                    && this.Sign == ((TrigTerm)other).Sign
                    && this._trigFun == ((TrigTerm)other).TrigFun
                    && this.Term.AreEqual(((TrigTerm)other).Term))
                {
                    if (this.multipliedBy != null && ((TrigTerm)other).multipliedBy != null)
                    {
                        if (this.multipliedBy != null && this.multipliedBy.Count > 0)
                        {
                            if (((TrigTerm)other).multipliedBy == null || ((TrigTerm)other).multipliedBy.Count == 0)
                                return false;
                        }
                        if (((TrigTerm)other).multipliedBy != null && ((TrigTerm)other).multipliedBy.Count > 0)
                        {
                            if (this.multipliedBy == null || this.multipliedBy.Count == 0)
                                return false;
                        }
                    }

                    return true;
                }
                else return false;
            }
        
            #endregion Methods
        }
        public class TrigExpression
        {
            #region Properties

            List<ITrigExpressionPiece> numerator;

            public List<ITrigExpressionPiece> Numerator
            {
                get { return numerator; }
                set { numerator = value; }
            }
            List<ITrigExpressionPiece> denominator;

            public List<ITrigExpressionPiece> Denominator
            {
                get { return denominator; }
                set { denominator = value; }
            }


            #endregion Properties

            #region Constructor

            public TrigExpression(ITrigExpressionPiece[] num, ITrigExpressionPiece[] dim)
            {
                this.numerator = new List<ITrigExpressionPiece>(num);

                if(dim != null && dim.Length != 0)
                    this.denominator = new List<ITrigExpressionPiece>(dim);
            }
            public TrigExpression()
            {
            }

            #endregion Constructor

            #region Methods

            public bool AreEqual(TrigExpression other)
            {
                throw new NotImplementedException();
            }

            #endregion Methods


        }
        public class TrigEquation 
        {
            #region Properties

            SignType _split;
            public SignType Split
            {
                get { return _split; }
                set { _split = value; }
            }
            List<TrigExpression> _left;
            public List<TrigExpression> Left
            {
                get { return _left; }
                set { _left = value; }
            }
            List<TrigExpression> _right;
            public List<TrigExpression> Right
            {
                get { return _right; }
                set { _right = value; }
            }

            #endregion Properties

            #region Constructor

            public TrigEquation(TrigExpression[] left, TrigExpression[] right)
            {
                this._left = new List<TrigExpression>(left);
                this._right = new List<TrigExpression>(right);
                this.Split = SignType.equal;
            }
            public TrigEquation(TrigExpression[] left, SignType split, TrigExpression[] right)
            {
                this._left = new List<TrigExpression>(left);
                this._right = new List<TrigExpression>(right);
                this.Split = split;
            }
            public TrigEquation()
            {
            }

            #endregion Constructor

            #region Methods

            #endregion Methods
        }
        public class TrigIdentity
        {
            #region Properties

            int _number;
            public int Number
            {
                get { return _number; }
                set { _number = value; }
            }

            TrigEquation _equation = new TrigEquation();

            public TrigEquation Equation
            {
                get { return _equation; }
                set { _equation = value; }
            }

            #endregion  Properties

            #region Constructor

            public TrigIdentity(List<TrigExpression> left, List<TrigExpression> right)
            {
                _equation.Left = new List<TrigExpression>(left);
                _equation.Right  = new List<TrigExpression>(right);
            }

            #endregion Constructor

            #region Methods 

            #endregion Methods
        
        }
        #endregion Trig


    public enum EquationType
    {
        Algebraic, Exponential, Logarithmic, Trigonomatric, Inequality, General
    }
    public enum Action
    {
        Simplify, Factor, Distribute, Solve, Prove, Expand, Inequalities, Plot
    }
    public enum GraphType
    {
        straight, Parabola, Expo, None
    }
    public class SimpificationEquation
    {
        #region Properties

        List<List<ISimplificationPiece>> _solution = new List<List<ISimplificationPiece>>();

        public List<List<ISimplificationPiece>> Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        #endregion Properties

        #region Constructor

        public SimpificationEquation(params ISimplificationPiece[] pieces)
        {
            this._solution.Add(new List<ISimplificationPiece>(pieces));
            Solve();
        }

        #endregion Constructor

        #region Methods

        private void Solve()
        {
            //BODMUS
            SimplifyExpressions();
            RemoveBraces();
            if (CanRemovePower())
            {
                RemovePower();
                RemoveBraces();
                //remove power
            }
            Simplify();

            int index = -1;

            while (CanDivide(ref index))
            {
                DoDivide(index);
                Simplify();
                RemoveBraces();
                AddLikeTerms();
            }

            while (CanMultiply(ref index))
            {
                DoMultiply(index);
                Simplify();
                RemoveBraces();
                AddLikeTerms();
            }

            while (CanAdd(ref index))
            {
                DoAddition(index);
                Simplify();
                RemoveBraces();
                AddLikeTerms();
            }

            while (CanSubtract(ref index))
            {
                DoSubtraction(index);
                Simplify();
                RemoveBraces();
                AddLikeTerms();
            }
            FinishUp();
            AddLikeTerms();

        }
        private void FinishUp()
        {
            bool result = false;
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    SimplificationExpression expression = new SimplificationExpression((Expression)piece[i]);
                    if (expression.Numerator.Count == 0)
                    {
                        expression.Numerator.Add(new Term(1));
                        //expression.Denominator.Sort();
                        piece[i] = expression;
                        result = true;

                    }
                    //Expression exp = (Expression)expression;
                    //exp.KillFraction();
                    //answer.Add( new SimplificationExpression(exp));
                }

                else answer.Add(piece[i]);
            }

            if (result)
            {
                _solution.Add(new List<ISimplificationPiece>(answer));
            }
        }
        private bool CanMultiply(ref int index)
        {
            return CanDoMathFunction(ref index, SignType.Multiply);
        }
        private bool CanDivide(ref int index)
        {
            return CanDoMathFunction(ref index, SignType.Divide);
        }
        private bool CanAdd(ref int index)
        {
            return CanDoMathFunction(ref index, SignType.Add);
        }
        private bool CanSubtract(ref int index)
        {
            return CanDoMathFunction(ref index, SignType.Subtract);
        }
        private void DoDivide(int index)
        {
            DoMathFunction(index, MathFunction.Divide);
        }
        private bool CanDoMathFunction(ref int index, SignType type)
        {
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);

            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Sign)
                {
                    if (((Sign)piece[i]).SignType == type)
                    {
                        index = i;
                        return true;
                    }
                }
            }
            return false;
        }
        private void DoMultiply(int index)
        {
            DoMathFunction(index, MathFunction.Mutliply);
        }
        private void DoAddition(int index)
        {
            DoMathFunction(index, MathFunction.Add);
        }
        private void DoSubtraction(int index)
        {
            DoMathFunction(index, MathFunction.Subtract);
        }
        private void DoMathFunction(int index, MathFunction function)
        {
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();

            for (int i = 0; i < piece.Count; i++)
            {
                if (InActionRange(index, i))
                {
                    answer.Add(new SimplificationExpression(((Expression)
                        calc.Calculate((Expression)piece[index - 1], (Expression)piece[index + 1], function))));
                    i += 2;
                }
                else
                    answer.Add(piece[i]);
            }

            _solution.Add(answer);
        }
        Calculator calc = new Calculator();
        private bool InActionRange(int index, int current)
        {
            if (current == index ||
                current - 1 == index ||
                current + 1 == index)
                return true;
            return false;
        }
        private void Simplify()
        {
            while (SimplifyDo())
            {
                try
                {
                    DoCancellation();
                }
                catch
                {
                    break;
                }
            }
        }
        private bool DoCancellation()  //
        {
            bool result = false;
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    SimplificationExpression expression = new SimplificationExpression((SimplificationExpression)piece[i]);
                    if (expression.DoCancellationPlossible())
                    {
                        result = true;
                        expression.DoCancellation();

                        if (expression.Denominator.Count == 0 && expression.Numerator.Count == 0)
                            piece[i] = null;
                        else
                            piece[i] = expression;
                    }

                }
                answer.Add(piece[i]);
            }

            if (result)
            {
                _solution.Add(answer);
                return true;
            }
            return false;
        }
        private bool SimplifyDo()
        {
            bool result = false;
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    SimplificationExpression expression = new SimplificationExpression((Expression)piece[i]);
                    if (expression.ShowCancellation())
                    {
                        result = true;
                        piece[i] = expression;
                    }

                }
                answer.Add(piece[i]);
            }

            if (result)
            {
                _solution.Add(answer);
                return true;
            }
            return false;
        }
        private bool AddLikeTerms()
        {
            bool result = false;
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    SimplificationExpression expression = new SimplificationExpression((Expression)piece[i]);
                    if (expression.AddLikeTerms())
                    {
                        expression.Numerator.Sort();
                        expression.Denominator.Sort();
                        result = true;
                        piece[i] = expression;
                    }

                }
                answer.Add(piece[i]);
            }

            if (result)
            {
                _solution.Add(answer);
                return true;
            }
            return false;
        }
        private bool RemoveBraces()
        {
            int count = 0;
            while (RemoveBracesDo())
            {
                AddLikeTerms();
                count++;
            }

            if (count == 0) return false;
            return true;
        }
        private bool SimplifyExpressions()
        {
            bool result = false;
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    SimplificationExpression expression = new SimplificationExpression((SimplificationExpression)piece[i]);
                    if (expression.Simplify(true))
                    {
                        result = true;
                        piece[i] = expression;
                    }

                }
                answer.Add(piece[i]);
            }

            if (result)
            {
                _solution.Add(answer);
                return true;
            }
            return false;
        }
        private bool RemoveBracesDo()
        {
            bool result = false;
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    SimplificationExpression expression = new SimplificationExpression((SimplificationExpression)piece[i]);
                    if (expression.CanRemoveBraces())
                    {
                        result = true;
                        expression.RemoveBracesBabySteps();
                        piece[i] = expression;
                    }

                }
                answer.Add(piece[i]);
            }

            if (result)
            {
                _solution.Add(answer);
                return true;
            }
            return false;
        }
        private bool RemovePower()
        {
            bool result = false;
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            List<ISimplificationPiece> answer = new List<ISimplificationPiece>();
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    SimplificationExpression expression = new SimplificationExpression((SimplificationExpression)piece[i]);

                    #region  Numerator

                    if (expression.BracePowerNumerator > 1)
                    {
                        expression.Numerator = new List<IExpressionPiece>(expression.BraketThis(expression.Numerator));
                        List<IExpressionPiece> numCopy = new List<IExpressionPiece>(expression.Numerator);

                        while (expression.BracePowerNumerator > 1)
                        {
                            expression.Numerator.AddRange(new List<IExpressionPiece>(numCopy));
                            //( expression.Numerator), new List<IExpressionPiece>( expression.Numerator));
                            expression.BracePowerNumerator -= 1;
                        }

                        result = true;
                        piece[i] = expression;
                    }

                    #endregion Numerator

                    #region  Denominator

                    if (expression.BracePowerDenominator > 1)
                    {
                        expression.Denominator = new List<IExpressionPiece>(expression.BraketThis(expression.Denominator));
                        List<IExpressionPiece> dimCopy = new List<IExpressionPiece>(expression.Denominator);

                        while (expression.BracePowerDenominator > 1)
                        {
                            expression.Denominator.AddRange(new List<IExpressionPiece>(dimCopy));
                            //( expression.Numerator), new List<IExpressionPiece>( expression.Numerator));
                            expression.BracePowerDenominator -= 1;
                        }

                        result = true;
                        piece[i] = expression;
                    }

                    #endregion Denominator

                }
                answer.Add(piece[i]);
            }

            if (result)
            {
                _solution.Add(answer);
                return true;
            }
            return false;
        }
        private bool CanRemovePower()
        {
            List<ISimplificationPiece> piece = new List<ISimplificationPiece>(_solution[_solution.Count - 1]);
            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i] != null && piece[i].GetSimplificationType() == SimplificationPieceType.Expression)
                {
                    if (((SimplificationExpression)piece[i]).BracePowerNumerator > 1 ||
                        ((SimplificationExpression)piece[i]).BracePowerDenominator > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion Methods
    }
    public class SimplificationExpression : Expression, ISimplificationPiece
    {
        #region Properties

        List<Range> numeratorRanges = new List<Range>();
        public List<Range> NumeratorRanges
        {
            get { return numeratorRanges; }
            set { numeratorRanges = value; }
        }
        List<Range> denominatorRanges = new List<Range>();
        public List<Range> DenominatorRanges
        {
            get { return denominatorRanges; }
            set { denominatorRanges = value; }
        }
        bool _cancelledOut = false;
        public bool CancelledOut
        {
            get { return _cancelledOut; }
            set { _cancelledOut = value; }
        }

        #region Cancelling Out

        List<Range> cancellingNum = new List<Range>();

        public List<Range> CancellingNum
        {
            get { return cancellingNum; }
            set { cancellingNum = value; }
        }
        List<Range> cancellingDim = new List<Range>();

        public List<Range> CancellingDim
        {
            get { return cancellingDim; }
            set { cancellingDim = value; }
        }

        #endregion Cancelling Out

        #endregion Properties

        #region Constructor

        public SimplificationExpression()
            : base()
        {

        }
        public SimplificationExpression(Expression exp)
            : base(exp)
        {
        }
        public SimplificationExpression(IExpressionPiece[] numerator, IExpressionPiece[] denominator)
            : base(numerator, denominator)
        {
        }
        public SimplificationExpression(SimplificationExpression exp)
            : base(new Expression((Expression)exp))
        {
            this.numeratorRanges = exp.numeratorRanges;
            this.denominatorRanges = exp.denominatorRanges;
            this.CancelledOut = exp.CancelledOut;
            this.cancellingDim = exp.cancellingDim;
            this.cancellingNum = exp.cancellingNum;
        }

        #endregion Constructor

        #region Methods

        public SimplificationPieceType GetSimplificationType()
        {
            return SimplificationPieceType.Expression;
        }
        public override bool Factorize()
        {
            bool answer = base.Factorize();
            SetRanges();
            return answer;
        }
        public void SetRanges()
        {
            this.numeratorRanges = GetRanges(this.Numerator);
            this.denominatorRanges = GetRanges(this.Denominator);
        }
        private List<Range> GetRanges(List<IExpressionPiece> pieces)
        {
            List<Range> answer = new List<Range>();
            bool inRange = false;
            int currentStart = -1;

            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Term && !inRange)
                {
                    inRange = true;
                    currentStart = i;
                }
                else if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace && inRange)
                {
                    inRange = false;
                    answer.Add(new Range(currentStart, i - 1));
                }
            }

            if (answer.Count == 0 && pieces.Count > 0)
                answer.Add(new Range(0, pieces.Count - 1));
            return answer;
        }
        public bool CancelOut()
        {
            List<Term> all = new List<Term>();
            all.AddRange(GetTerms(base.Numerator));

            List<Term> denominator = new List<Term>(GetTerms(this.Denominator));

            for (int i = 0; i < denominator.Count; i++)
                all.Add((Term)calc.Calculate(denominator[i], new Term(1, true), MathFunction.Mutliply));

            all = calc.AddLikeTerms(all.ToArray());

            if (all.Count == 0 || calc.allNull(all))
                return true;
            else return false;
        }
        public bool CancelOutShow()
        {
            List<Term> all = new List<Term>();
            all.AddRange(GetTerms(base.Numerator));

            List<Term> denominator = new List<Term>(GetTerms(this.Denominator));

            for (int i = 0; i < denominator.Count; i++)
                all.Add((Term)calc.Calculate(denominator[i], new Term(1, true), MathFunction.Mutliply));

            if (all.Count == 0)
                return false;

            all = calc.AddLikeTerms(all.ToArray());

            if (all.Count == 0 || calc.allNull(all))
            {
                this.cancellingNum.Add(new Range(0, base.Numerator.Count - 1));
                this.cancellingDim.Add(new Range(0, base.Denominator.Count - 1));
                return true;
            }
            else return false;
        }
        private bool CanCancelOut()//Dont use for optimazation purporse
        {
            base.RemoveBraces();

            if (CancelOut()) return true;

            if (FactorCancelOut()) return true;

            if (CommonFactorCancelOut()) return true;

            return false;
        }
        public bool ShowCancellation()
        {
            base.RemoveBraces();

            if (base.Numerator.Count > 0 && base.Denominator.Count > 0)
            {

                if (CancelOutShow()) return true;

                if (base.Numerator.Count > 0 && base.Denominator.Count > 0
                    && (base.Numerator.Count != 1 || ((Term)base.Numerator[0]).CoEfficient > 1
                    || !((Term)base.Numerator[0]).Constant)
                    && (base.Denominator.Count != 1 || ((Term)base.Denominator[0]).CoEfficient > 1
                    || !((Term)base.Denominator[0]).Constant))
                {
                    if (FactorCancelOutShow()) return true;

                    if (CommonFactorCancelOutShow()) return true;
                }
            }

            return false;
        }
        public void DoCancellation()
        {
            FixRanges();

            List<IExpressionPiece> newNum = new List<IExpressionPiece>();

            for (int i = 0; i < base.Numerator.Count; i++)
                if (!IsInRange(i, true))
                    newNum.Add(base.Numerator[i]);

            if (newNum.Count != 0)
                base.Numerator = new List<IExpressionPiece>(newNum);
            else base.Numerator = new List<IExpressionPiece>(new IExpressionPiece[] { new Term(1) });

            List<IExpressionPiece> newDim = new List<IExpressionPiece>();

            for (int i = 0; i < base.Denominator.Count; i++)
                if (!IsInRange(i, false))
                    newDim.Add(base.Denominator[i]);

            base.Denominator = new List<IExpressionPiece>(newDim);

            cancellingNum = new List<Range>();
            CancellingDim = new List<Range>();

        }
        public bool DoCancellationPlossible()
        {
            if (cancellingNum.Count > 0 || CancellingDim.Count > 0)
                return true;
            return false;
        }
        private bool IsInRange(int index, bool numerator)
        {
            if (numerator)
                return isInRange(index, cancellingNum);
            return isInRange(index, CancellingDim);
        }
        private bool isInRange(int index, List<Range> ranges)
        {
            for (int i = 0; i < ranges.Count; i++)
            {
                if (index >= ranges[i].Start && index <= ranges[i].End)
                    return true;
            }
            return false;
        }
        private void FixRanges()
        {
            for (int i = 0; i < cancellingNum.Count; i++)
            {
                if (cancellingNum[i].End != base.Numerator.Count - 1
                    && (cancellingNum[i].Start - 1 >= 0) && base.Numerator[cancellingNum[i].Start - 1].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)base.Numerator[cancellingNum[i].Start - 1]).Key == '(')
                {
                    cancellingNum[i].Start -= 1;
                }

                if (cancellingNum[i].End != base.Numerator.Count - 1
                    && base.Numerator[cancellingNum[i].End + 1].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)base.Numerator[cancellingNum[i].End + 1]).Key == ')')
                {
                    cancellingNum[i].End += 1;
                }
            }

            for (int i = 0; i < cancellingDim.Count; i++)
            {
                if (cancellingDim[i].End != base.Denominator.Count - 1
                    && (cancellingDim[i].Start - 1 >= 0) && base.Denominator[cancellingDim[i].Start - 1].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)base.Denominator[cancellingDim[i].Start - 1]).Key == '(')
                {
                    cancellingDim[i].Start -= 1;
                }

                if (cancellingDim[i].End != base.Denominator.Count - 1
                    && base.Denominator[cancellingDim[i].End + 1].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)base.Denominator[cancellingDim[i].End + 1]).Key == ')')
                {
                    cancellingDim[i].End += 1;
                }
            }
        }
        private bool CommonFactorCancelOutShow()
        {
            base.RemoveBraces();
            List<Term> copyNum = GetTerms(new List<IExpressionPiece>(base.Numerator));
            List<Term> copyDim = GetTerms(new List<IExpressionPiece>(base.Denominator));

            List<IExpressionPiece> commonFactorNum = base.CommonFactor(GetTerms(new List<IExpressionPiece>(base.Numerator)));
            List<IExpressionPiece> commonFactorDim = base.CommonFactor(GetTerms(new List<IExpressionPiece>(base.Denominator)));
            List<IExpressionPiece> factoredNum = base.Factorize(new List<IExpressionPiece>(base.Numerator));
            List<IExpressionPiece> factoredDim = base.Factorize(new List<IExpressionPiece>(base.Denominator));

            List<Range> top = GetRanges(commonFactorNum);
            List<Range> bottom = GetRanges(commonFactorDim);
            List<Range> factoredTop = GetRanges(factoredNum);
            List<Range> factoredBottom = GetRanges(factoredDim);

            #region Four

            for (int i = 0; i < factoredTop.Count; i++)
            {
                List<Term> currentTop = GetRange(factoredTop[i], factoredNum);

                for (int j = 0; j < factoredBottom.Count; j++)
                {
                    List<Term> currentBot = GetRange(factoredBottom[j], factoredDim);
                    if (CancelOut(currentTop, currentBot))
                    {
                        base.Numerator = new List<IExpressionPiece>(factoredNum);
                        base.Denominator = new List<IExpressionPiece>(factoredDim);
                        cancellingNum.Add(factoredTop[i]);
                        cancellingDim.Add(factoredBottom[j]);
                        return true;
                    }
                }
            }

            #endregion Four

            #region One
            for (int b = 0; b < top.Count; b++)
            {
                List<Term> currentTop = GetRange(top[b], commonFactorNum);
                if (CancelOut(currentTop, copyDim))
                {
                    base.Numerator = new List<IExpressionPiece>(commonFactorNum);
                    cancellingNum.Add(top[b]);
                    CancellingDim.Add(new Range(0, copyDim.Count - 1));
                    return true;
                }

                for (int bot = 0; bot < factoredBottom.Count; bot++)
                {
                    List<Term> currentBot = GetRange(factoredBottom[bot], factoredDim);
                    if (CancelOut(currentTop, currentBot))
                    {
                        base.Numerator = new List<IExpressionPiece>(commonFactorNum);
                        cancellingNum.Add(top[b]);
                        base.Denominator = new List<IExpressionPiece>(factoredDim);
                        cancellingDim.Add(factoredBottom[bot]);
                        return true;
                    }
                }
            }

            #endregion One

            #region Two

            for (int c = 0; c < bottom.Count; c++)
            {
                List<Term> currentBottom = GetRange(bottom[c], commonFactorDim);
                if (CancelOut(currentBottom, copyNum))
                {
                    base.Numerator = new List<IExpressionPiece>(TermsToExpressionPieces(copyNum));
                    base.Denominator = new List<IExpressionPiece>(commonFactorDim);
                    cancellingNum.Add(new Range(0, base.Numerator.Count - 1));
                    cancellingDim.Add(bottom[c]);
                    return true;
                }

                for (int t = 0; t < factoredTop.Count; t++)
                {
                    List<Term> currentTop = GetRange(factoredTop[t], factoredNum);
                    if (CancelOut(currentBottom, currentTop))
                    {
                        base.Numerator = new List<IExpressionPiece>(factoredNum);
                        base.Denominator = new List<IExpressionPiece>(commonFactorDim);
                        cancellingNum.Add(factoredTop[t]);
                        cancellingDim.Add(bottom[c]);
                        return true;
                    }
                }
            }

            #endregion Two

            #region Three

            for (int i = 0; i < top.Count; i++)
            {
                List<Term> currentTop = GetRange(top[i], commonFactorNum);
                for (int k = 0; k < bottom.Count; k++)
                {
                    List<Term> currentBottom = GetRange(bottom[k], commonFactorDim);
                    if (CancelOut(currentTop, currentBottom))
                    {
                        base.Numerator = new List<IExpressionPiece>(commonFactorNum);
                        base.Denominator = new List<IExpressionPiece>(commonFactorDim);
                        cancellingNum.Add(top[i]);
                        CancellingDim.Add(bottom[k]);
                        return true;
                    }
                }
            }

            #endregion Three



            return false;
        }
        private List<IExpressionPiece> TermsToExpressionPieces(List<Term> terms)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            for (int i = 0; i < terms.Count; i++)
                answer.Add(terms[i]);
            return answer;
        }
        private bool CommonFactorCancelOut()
        {
            base.RemoveBraces();
            List<Term> copyNum = GetTerms(new List<IExpressionPiece>(base.Numerator));
            List<Term> copyDim = GetTerms(new List<IExpressionPiece>(base.Denominator));

            List<IExpressionPiece> commonFactorNum = base.CommonFactor(GetTerms(new List<IExpressionPiece>(base.Numerator)));
            List<IExpressionPiece> commonFactorDim = base.CommonFactor(GetTerms(new List<IExpressionPiece>(base.Denominator)));
            List<IExpressionPiece> factoredNum = base.Factorize(new List<IExpressionPiece>(base.Numerator));
            List<IExpressionPiece> factoredDim = base.Factorize(new List<IExpressionPiece>(base.Denominator));

            List<Range> top = GetRanges(commonFactorNum);
            List<Range> bottom = GetRanges(commonFactorDim);
            List<Range> factoredTop = GetRanges(factoredNum);
            List<Range> factoredBottom = GetRanges(factoredDim);


            for (int b = 0; b < top.Count; b++)
            {
                List<Term> currentTop = GetRange(top[b], commonFactorNum);
                if (CancelOut(currentTop, copyDim))
                    return true;

                for (int bot = 0; bot < factoredBottom.Count; bot++)
                {
                    List<Term> currentBot = GetRange(factoredBottom[bot], factoredDim);
                    if (CancelOut(currentTop, currentBot))
                        return true;
                }
            }

            for (int c = 0; c < bottom.Count; c++)
            {
                List<Term> currentBottom = GetRange(bottom[c], commonFactorDim);
                if (CancelOut(currentBottom, copyNum))
                    return true;

                for (int t = 0; t < factoredTop.Count; t++)
                {
                    List<Term> currentTop = GetRange(factoredTop[t], factoredNum);
                    if (CancelOut(currentBottom, currentTop))
                        return true;
                }
            }

            for (int i = 0; i < top.Count; i++)
            {
                List<Term> currentTop = GetRange(top[i], commonFactorNum);
                for (int k = 0; k < bottom.Count; k++)
                {
                    List<Term> currentBottom = GetRange(bottom[k], commonFactorDim);
                    if (CancelOut(currentTop, currentBottom))
                        return true;
                }
            }

            for (int i = 0; i < factoredTop.Count; i++)
            {
                List<Term> currentTop = GetRange(factoredTop[i], factoredNum);

                for (int j = 0; j < factoredBottom.Count; j++)
                {
                    List<Term> currentBot = GetRange(factoredBottom[j], factoredDim);
                    if (CancelOut(currentTop, currentBot))
                        return true;
                }
            }

            return false;
        }
        private bool CancelOut(List<IExpressionPiece> one, List<IExpressionPiece> two)
        {
            List<Term> all = new List<Term>();
            all.AddRange(GetTerms(one));

            List<Term> denominator = new List<Term>(GetTerms(two));

            for (int i = 0; i < denominator.Count; i++)
                all.Add((Term)calc.Calculate(denominator[i], new Term(1, true), MathFunction.Mutliply));

            all = calc.AddLikeTerms(all.ToArray());

            if (all.Count == 0 || calc.allNull(all))
                return true;
            else return false;
        }
        private bool CancelOut(List<Term> one, List<Term> two)
        {
            List<Term> all = new List<Term>();
            all.AddRange(one);

            List<Term> denominator = new List<Term>(two);

            for (int i = 0; i < denominator.Count; i++)
                all.Add((Term)calc.Calculate(denominator[i], new Term(1, true), MathFunction.Mutliply));

            all = calc.AddLikeTerms(all.ToArray());

            if (all.Count == 0 || calc.allNull(all))
                return true;
            else return false;
        }
        private List<Term> GetRange(Range range, List<IExpressionPiece> source)
        {
            List<Term> answer = new List<Term>();

            for (int i = range.Start; i <= range.End; i++)
            {
                if (i > -1 && i < source.Count)
                {
                    answer.Add((Term)source[i]);
                }
            }
            return answer;
        }
        private bool FactorCancelOut()
        {
            List<Term> top = GetTerms(new List<IExpressionPiece>(this.Numerator));
            List<Term> botom = GetTerms(new List<IExpressionPiece>(this.Denominator));

            LongDivisionSolution NumeratorDivDenominator = new LongDivisionSolution(botom, top);

            if (NumeratorDivDenominator.Failed)
            {
                LongDivisionSolution DenominatorDivNumerator = new LongDivisionSolution(top, botom);

                if (DenominatorDivNumerator.Failed)
                    return false;
                return true;
            }
            else return true;


        }
        private bool FactorCancelOutShow()
        {
            List<Term> top = GetTerms(new List<IExpressionPiece>(this.Numerator));
            List<Term> botom = GetTerms(new List<IExpressionPiece>(this.Denominator));

            LongDivisionSolution NumeratorDivDenominator = new LongDivisionSolution(botom, top);

            if (NumeratorDivDenominator.Failed || NumeratorDivDenominator.Remainder)
            //|| (NumeratorDivDenominator.B.Count == 1 && NumeratorDivDenominator.B[0].Constant))
            {
                LongDivisionSolution DenominatorDivNumerator = new LongDivisionSolution(top, botom);

                if (DenominatorDivNumerator.Failed || DenominatorDivNumerator.Remainder)
                    //|| (DenominatorDivNumerator.B.Count == 1 && DenominatorDivNumerator.B[0].Constant))
                    return false;
                else
                {
                    base.Denominator = BraketThis(DenominatorDivNumerator.A, DenominatorDivNumerator.B);
                    cancellingDim.Add(GetRanges(base.Denominator)[0]);
                    cancellingNum.Add(new Range(0, base.Numerator.Count - 1));
                    return true;
                }
            }
            else
            {
                base.Numerator = BraketThis(NumeratorDivDenominator.A, NumeratorDivDenominator.B);
                cancellingNum.Add(GetRanges(base.Numerator)[0]);
                cancellingDim.Add(new Range(0, base.Denominator.Count - 1));
                return true;
            }


        }
        private List<IExpressionPiece> BraketThis(List<Term> one, List<Term> two)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();
            answer.AddRange(BraketThis(one));
            answer.AddRange(BraketThis(two));
            return answer;
        }
        public List<IExpressionPiece> BraketThis(List<IExpressionPiece> one, List<IExpressionPiece> two)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();
            answer.AddRange(BraketThis(one));
            answer.AddRange(BraketThis(two));
            return answer;
        }
        private List<IExpressionPiece> BraketThis(List<Term> pieces)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();
            answer.Add(new Brace('('));
            answer.AddRange(TermsToExpressionPieces(pieces));
            answer.Add(new Brace(')'));
            return answer;
        }
        public List<IExpressionPiece> BraketThis(List<IExpressionPiece> pieces)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();
            answer.Add(new Brace('('));
            answer.AddRange(pieces);
            answer.Add(new Brace(')'));
            return answer;
        }
        private List<Term> GetTerms(List<IExpressionPiece> pieces)
        {
            List<Term> answer = new List<Term>();
            Expression expression = new Expression();

            foreach (IExpressionPiece piece in pieces)
                expression.AddToExpression(piece, true);



            if (!expression.NoBraces(expression.Numerator))
            {
                //remove braces
                throw new NotImplementedException();
            }

            foreach (IExpressionPiece piece in expression.Numerator)
                if (piece.GetTypePiece() == ExpressionPieceType.Term)
                    answer.Add((Term)piece);
            return answer;
        }
        Calculator calc = new Calculator();

        #endregion Methods
    }
    public class Range
    {
        #region Properties

        int _start = -1;

        public int Start
        {
            get { return _start; }
            set { _start = value; }
        }
        int _end = -1;

        public int End
        {
            get { return _end; }
            set { _end = value; }
        }

        #endregion Properties

        #region Constructor

        public Range(int start, int end)
        {
            this._start = start;
            this._end = end;
        }

        #endregion Constructor

        #region Methods

        #endregion  Methods
    }
    public interface ISimplificationPiece
    {
        SimplificationPieceType GetSimplificationType();
    }
    public enum SimplificationPieceType
    {
        Sign, Expression
    }
    public class FactorDistributeSolution
    {
        #region Properties

        List<Expression> _solution = new List<Expression>();

        public List<Expression> Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        #endregion Properties

        #region Constructor

        public FactorDistributeSolution(Expression expression, bool factor)
        {
            this._solution.Add(new Expression(expression));

            Expression copy = new Expression();
            copy.Numerator = new List<IExpressionPiece>(expression.Numerator);
            if (expression.Denominator != null)
                copy.Denominator = new List<IExpressionPiece>(expression.Denominator);

            if (copy.Simplify(false))
            {
                this.Solution.Add(new Expression(copy));
            }

            if (factor)
            {
                if (expression.Factorize())
                    this._solution.Add(new Expression(expression));
            }
            else
            {
                Expression withouBraces = new Expression(expression);
                withouBraces.RemoveBraces(false);
                this._solution.Add(new Expression(withouBraces));

                if (withouBraces.AddLikeTerms())
                    this._solution.Add(new Expression(withouBraces));
            }
        }

        #endregion Constructor
    }
    public class Calculator
    {
        #region Methods
        //pass sorted terms

        #region Calculator public functionality

        public List<Term> GetAll(Term term, bool div)
        {
            List<Term> terms = new List<Term>();

            if (!div)
            {
                if (term.MultipledBy != null)
                {
                    for (int i = 0; i < term.MultipledBy.Count; i++)
                        terms.Add(term.MultipledBy[i]);
                }
                term = new Term(term);
                term.Devisor = null;
                term.MultipledBy = null;
                if (!term.Constant)
                    terms.Add(new Term(term));
                return terms;

            }
            else
            {
                if (term.Devisor != null)
                    return GetAll(term.Devisor, false);
                else
                    return new List<Term>();
            }
        }
        public List<TrigTerm> GetAll(TrigTerm term)
        {
            List<TrigTerm> terms = new List<TrigTerm>();
            if (term.MultipliedBy != null)
            {
                for (int i = 0; i < term.MultipliedBy.Count; i++)
                    terms.Add((TrigTerm)term.MultipliedBy[i]);
            }
            term = new TrigTerm(term);
            term.MultipliedBy = null;
            terms.Add(new TrigTerm(term));
            return terms;
        }
        private TrigTerm ChangeSign(TrigTerm term)
        {
            if (term.Sign == "+")
                term.Sign = "-";
            else term.Sign = "+";
            return term;
        }
        public bool AreLikeTerms(ITrigTerm one, ITrigTerm two, MathFunction function)
        {
            if (one.TrigTermType() == two.TrigTermType())
            {
                if (one.TrigTermType() == TrigTermType.TrigTerm)
                {
                    TrigTerm tOne = (TrigTerm)one;
                    TrigTerm tTwo = (TrigTerm)two;

                    if (tOne.TrigFun == tTwo.TrigFun)
                    {
                        return AreLikeTerms(tOne.Term, tTwo.Term, function);
                    }
                    else return false;
                }
                else
                    return AreLikeTerms((Term)one, (Term)two, function);
            }
            else return false;
        }
        public bool AreLikeTerms(Term one, Term two, MathFunction function)
        {
            one = Sort(one);
            two = Sort(two);

            //if (function == MathFunction.Mutliply ||
            //    function == MathFunction.Divide )
            //    return true;

            if ((one != null || two != null) && (one == null || two == null))
                return false;

            if (one == null && two == null)
                return false;

            if (one.Constant && two.Constant)
                return true;

            if ((!one.Constant || !two.Constant) && (one.Constant || two.Constant))
                return false;

            if (one.TermBase != two.TermBase)
                return false;

            if (((one.MultipledBy != null && one.MultipledBy.Count != 0) ||
                (two.MultipledBy != null && two.MultipledBy.Count != 0))
                && ((one.MultipledBy == null || one.MultipledBy.Count == 0) ||
                (two.MultipledBy == null || two.MultipledBy.Count == 0)))
                return false;

            if ((one.Devisor != null || two.Devisor != null)
                && (one.Devisor == null || two.Devisor == null))
                return false;

            if (one.Devisor != null && !AreLikeTerms(one.Devisor, two.Devisor, function))
                return false;

            if (one.MultipledBy != null &&
                two.MultipledBy != null)
            {
                if (one.MultipledBy.Count != two.MultipledBy.Count)
                    return false;

                for (int i = 0; i < one.MultipledBy.Count; i++)
                {
                    if (!AreLikeTerms(one.MultipledBy[i], two.MultipledBy[i], function))
                        return false;
                }
            }

            if (one.Power != two.Power && function != MathFunction.Divide && function != MathFunction.Mutliply)
                return false;

            if (one.Root != two.Root)
                return false;


            //Cant fight it
            return true;
        }

        public Term Sort(Term term)
        {
            if (term == null || term.Sorted || term.Constant ||
                (term.Devisor == null && term.MultipledBy == null))
                return term;

            Term answer = new Term();
            if (term.Devisor != null)
                term.Devisor = Sort(term.Devisor);


            Term divisor = term.Devisor;
            List<Term> mult = (term.MultipledBy != null) ? term.MultipledBy : new List<Term>();
            term.Devisor = null;
            term.MultipledBy = null;
            mult.Add(term);

            if (mult != null)
                mult = Sort(mult);

            answer = new Term(mult[0].TermBase,
                mult[0].Power, divisor);
            mult.Remove(mult[0]);
            answer.MultipledBy = mult;
            answer.CoEfficient = term.CoEfficient;
            answer.Constant = term.Constant;
            answer.PowerSign = term.PowerSign;
            answer.Root = term.Root;
            answer.Sign = term.Sign;
            answer.TwoSigns = term.TwoSigns;
            answer.Sorted = true;
            return answer;
        }

        public List<Term> Sort(List<Term> terms)
        {
            Term temp = new Term(' ');

            for (int write = 0; write < terms.Count; write++)
            {
                for (int sort = 0; sort < terms.Count - 1; sort++)
                {
                    if ((terms[sort] == null && terms[sort + 1] != null))
                    {
                        temp = terms[sort + 1];
                        terms[sort + 1] = terms[sort];
                        terms[sort] = temp;
                    }
                    else if ((terms[sort] != null && terms[sort + 1] == null)
                        || (terms[sort] == null && terms[sort + 1] == null))
                    {
                        //do nothing
                    }
                    else if (terms[sort].CompareTo(terms[sort + 1]) == 1
                        || (terms[sort].CompareTo(terms[sort + 1]) == 0
                             && terms[sort].Power < terms[sort + 1].Power))
                    {
                        temp = terms[sort + 1];
                        terms[sort + 1] = terms[sort];
                        terms[sort] = temp;
                    }

                }
            }
            return terms;
        }
        private Term PrepareTerm(Term term)
        {
            Term one = Sort(term);
            one.Simplify();
            return new Term(one);
        }
        public IAlgebraPiece Calculate(Term oneT, Term twoT)
        {
            IAlgebraPiece answer = null;
            Term one = PrepareTerm(oneT);
            Term two = PrepareTerm(twoT);
            bool like = true;// AreLikeTerms(one, two, function);

            //if (function == MathFunction.Mutliply)
            //    answer = Multiply(new Term(one), new Term(two), like);
            //else if (function == MathFunction.Divide)
            //    answer = Divide(new Term(one), new Term(two), like);
            //else if (function == MathFunction.Add)
            answer = Add(new Term(one), new Term(two), like);
            //else if (function == MathFunction.Subtract)
            //    answer = Subtract(new Term(one), new Term(two), like);

            //if (answer != null && answer.GetType1() == Type.Term)
            //{
            //    Term ans = ((Term)answer);
            //    ans.Simplify();
            //    answer = new Term(ans);
            //}

            return answer;
        }
        public IAlgebraPiece Calculate(Term oneT, Term twoT, MathFunction function)
        {
            IAlgebraPiece answer = null;
            Term one = PrepareTerm(oneT);
            Term two = PrepareTerm(twoT);
            bool like = AreLikeTerms(one, two, function);

            if (function == MathFunction.Mutliply)
                answer = Multiply(new Term(one), new Term(two), like);
            else if (function == MathFunction.Divide)
                answer = Divide(new Term(one), new Term(two), like);
            else if (function == MathFunction.Add)
                answer = Add(new Term(one), new Term(two), like);
            else if (function == MathFunction.Subtract)
                answer = Subtract(new Term(one), new Term(two), like);

            if (answer != null && answer.GetType1() == Type.Term)
            {
                Term ans = ((Term)answer);
                ans.Simplify();
                answer = new Term(ans);
            }

            return answer;
        }
        public TrigExpression Calculate(ITrigTerm oneTT, ITrigTerm twoTT, MathFunction function)
        {
            if (AreLikeTerms(oneTT, twoTT, function))
            {
                if (oneTT.TrigTermType() == TrigTermType.TrigTerm)
                {
                    #region TrigTerm
                    Term oneT = new Term(TrigTermForCalc((TrigTerm)oneTT));
                    Term twoT = new Term(TrigTermForCalc((TrigTerm)twoTT));
                    IAlgebraPiece answer = null;
                    Term one = PrepareTerm(oneT);
                    Term two = PrepareTerm(twoT);
                    bool like = AreLikeTerms(one, two, function);

                    if (function == MathFunction.Mutliply)
                        answer = Multiply(new Term(one), new Term(two), like);
                    else if (function == MathFunction.Divide)
                        answer = Divide(new Term(one), new Term(two), like);
                    else if (function == MathFunction.Add)
                        answer = Add(new Term(one), new Term(two), like);
                    else if (function == MathFunction.Subtract)
                        answer = Subtract(new Term(one), new Term(two), like);

                    if (answer != null && answer.GetType1() == Type.Term)
                    {
                        Term ans = ((Term)answer);
                        ans.Simplify();
                        answer = new Term(ans);
                        if (ans.CoEfficient != 0)
                        {
                            int coE = ans.CoEfficient;
                            ans.CoEfficient = 1;
                            string sign = ans.Sign;
                            ans.Sign = "+";
                            TrigTerm result = new TrigTerm(ans, ((TrigTerm)oneTT).TrigFun, coE);
                            result.Sign = sign;
                            return new TrigExpression(new ITrigExpressionPiece[] { result }, null);
                        }
                        else return new TrigExpression();//cancelled out...
                    }
                    throw new NotImplementedException();
                    #endregion TrigTerm
                }
                else
                {
                    IAlgebraPiece piece = Calculate((Term)oneTT, (Term)twoTT, function);

                    if (piece.GetType1() == Type.Term)
                        return new TrigExpression(new ITrigExpressionPiece[] { (Term)piece }, null);
                    else return new TrigExpression();
                }
            }
            else if (function == MathFunction.Divide || function == MathFunction.Mutliply)
            {
                #region Unlike

                if (oneTT.TrigTermType() == TrigTermType.TrigTerm && twoTT.TrigTermType() == TrigTermType.Constant)
                {
                    TrigTerm term = (TrigTerm)oneTT;
                    if (function == MathFunction.Mutliply)
                        term.CoEfficient *= ((Term)twoTT).CoEfficient;
                    else term.CoEfficient /= ((Term)twoTT).CoEfficient;
                    throw new NotImplementedException(); // cater for the fucken signs....
                    return new TrigExpression(new ITrigExpressionPiece[] { term }, null);
                }
                else if (twoTT.TrigTermType() == TrigTermType.TrigTerm && oneTT.TrigTermType() == TrigTermType.Constant)
                {
                    TrigTerm term = (TrigTerm)twoTT;
                    if (function == MathFunction.Mutliply)
                        term.CoEfficient *= ((Term)oneTT).CoEfficient;
                    else term.CoEfficient /= ((Term)oneTT).CoEfficient;
                    throw new NotImplementedException(); //cater for the fucken signs....
                    return new TrigExpression(new ITrigExpressionPiece[] { term }, null);
                }
                else if (oneTT.TrigTermType() == TrigTermType.TrigTerm && twoTT.TrigTermType() == TrigTermType.TrigTerm)
                {
                    return ActForUnlikeTrigTerms((TrigTerm)oneTT, (TrigTerm)twoTT, function);
                }
                else throw new NotImplementedException();

                #endregion Unlike
            }
            return null;
        }
        private TrigExpression ActForUnlikeTrigTerms(TrigTerm one, TrigTerm two, MathFunction function)
        {
            List<TrigTerm> oneL = GetAll(one);
            List<TrigTerm> twoL = GetAll(two);

            DoFunctionForLike(ref oneL, ref twoL, function);

            oneL = RemoveNulls(oneL);
            twoL = RemoveNulls(twoL);

            if (function == MathFunction.Mutliply)
            {
                #region Multiplication

                if (oneL.Count > 0)
                {
                    TrigTerm ansOne = Merge(oneL);
                    TrigTerm ansTwo = null;

                    if (twoL.Count > 0)
                        ansTwo = Merge(twoL);

                    if (ansTwo != null)
                    {
                        if (ansTwo.CoEfficient > 0)
                        {
                            ansOne.CoEfficient *= ansTwo.CoEfficient;
                            ansTwo.CoEfficient = 1;
                        }

                        foreach (TrigTerm termC in GetAll(ansTwo))
                        {
                            ansOne.MultipliedBy.Add(termC);
                        }
                    }

                    return new TrigExpression(new ITrigExpressionPiece[] { ansOne }, null);
                }
                else if (twoL.Count > 0) throw new NotImplementedException();



                #endregion Multiplication
            }
            else
            {
                TrigTerm ansOne = null, ansTwo = null;

                if (oneL.Count > 0)
                    ansOne = Merge(oneL);
                if (twoL.Count > 0)
                    ansTwo = Merge(twoL);

                return new TrigExpression(new ITrigExpressionPiece[] { (ansOne != null) ? (ITrigExpressionPiece)ansOne : ((ITrigExpressionPiece)new Term(1)) },
                    new ITrigExpressionPiece[] { (ansTwo != null) ? (ITrigExpressionPiece)ansTwo : ((ITrigExpressionPiece)new Term(1)) });
            }


            throw new NotImplementedException();
        }
        private TrigTerm Merge(List<TrigTerm> terms)
        {

            TrigTerm term = new TrigTerm(terms[0]);

            for (int i = 1; i < terms.Count; i++)
            {
                term.MultipliedBy.Add(terms[i]);
            }

            for (int i = 0; i < term.MultipliedBy.Count; i++)
            {
                if (term.CoEfficient < ((TrigTerm)term.MultipliedBy[i]).CoEfficient)
                {
                    term.CoEfficient = ((TrigTerm)term.MultipliedBy[i]).CoEfficient;
                    TrigTerm termT = ((TrigTerm)term.MultipliedBy[i]);
                    termT.CoEfficient = 1;
                    term.MultipliedBy[i] = termT;
                }
            }
            return term;
        }
        private bool allNull(List<TrigTerm> terms)
        {
            for (int i = 0; i < terms.Count; )
            {
                if (terms[i] != null)
                    return false;
            }
            return true;
        }
        private List<TrigTerm> RemoveNulls(List<TrigTerm> terms)
        {
            List<TrigTerm> answer = new List<TrigTerm>();

            for (int i = 0; i < terms.Count; i++)
            {
                if (terms[i] != null)
                    answer.Add(terms[i]);
            }

            return answer;
        }
        public Term TrigTermForCalc(TrigTerm term1)
        {
            Term term = new Term(term1.Term);
            term.CoEfficient = term1.CoEfficient;
            term.Sign = term1.Sign;
            return term;
        }
        public IAlgebraPiece Calculate(Expression one, Expression two, MathFunction function)
        {
            if (function == MathFunction.Mutliply)
                return MultiplyExpressions(one, two);
            else if (function == MathFunction.Divide)
                return DivideExpressions(one, two);
            else if (function == MathFunction.Add)
                return AddOrSubtractExpressions(one, two, function);
            else if (function == MathFunction.Subtract)
                return AddOrSubtractExpressions(one, two, function);
            return null;
        }
        //needs to be tested
        public bool AllOne(List<IExpressionPiece> piece)
        {
            if (piece.Count > 1)
                return false;

            for (int i = 0; i < piece.Count; i++)
            {
                if (piece[i].GetTypePiece() != ExpressionPieceType.Term)
                    return false;
                else if (!((Term)piece[i]).Constant)
                    return false;
                else if (((Term)piece[i]).CoEfficient != 1)
                    return false;
            }
            return true;
        }
        public IAlgebraPiece AddOrSubtractExpressions(Expression one, Expression two, MathFunction function)
        {
            if ((one.Denominator.Count > 0 && !AllOne(one.Denominator)) || (two.Denominator.Count > 0 && !AllOne(two.Denominator)))
            {
                //throw new NotImplementedException();

                one.RemoveBraces();
                two.RemoveBraces();

                #region Devisor

                Expression expression = new Expression();
                expression.AddToExpression(new Brace('('), true);
                //add opening brace

                foreach (IExpressionPiece top in two.Denominator)
                    expression.AddToExpression(top, true);

                if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), true);

                expression.AddToExpression(new Brace(')'), true);
                expression.AddToExpression(new Brace('('), true);
                foreach (IExpressionPiece top in one.Numerator)
                    expression.AddToExpression(top, true);

                if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), true);

                expression.AddToExpression(new Brace(')'), true);

                expression.AddToExpression((MathFunction.Add == function) ? new Term(1) : new Term(1, true), true);

                expression.AddToExpression(new Brace('('), true);
                expression.AddToExpression(new Brace('('), true);
                foreach (IExpressionPiece top in one.Denominator)
                    expression.AddToExpression(top, true);

                if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), true);

                expression.AddToExpression(new Brace(')'), true);
                expression.AddToExpression(new Brace('('), true);
                foreach (IExpressionPiece top in two.Numerator)
                    expression.AddToExpression(top, true);

                if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), true);

                expression.AddToExpression(new Brace(')'), true);
                expression.AddToExpression(new Brace(')'), true);

                #region Deno

                expression.AddToExpression(new Brace('('), false);
                foreach (IExpressionPiece top in one.Denominator)
                    expression.AddToExpression(top, false);

                if (expression.Denominator[expression.Denominator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), false);

                expression.AddToExpression(new Brace(')'), false);
                expression.AddToExpression(new Brace('('), false);
                foreach (IExpressionPiece top in two.Denominator)
                    expression.AddToExpression(top, false);

                if (expression.Denominator[expression.Denominator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), false);

                expression.AddToExpression(new Brace(')'), false);

                #endregion Deno

                return expression;

                #endregion Devisor
            }
            else
            {
                #region noDevisor

                Expression answer = new Expression();
                answer.AddToExpression(true, GetTerms(one.Numerator));

                List<IExpressionPiece> piecesTwo = GetTerms(two.Numerator);

                foreach (IExpressionPiece piece in piecesTwo)
                    if (function == MathFunction.Subtract)
                    {
                        Term term = (Term)Calculate((Term)piece, new Term(1, true), MathFunction.Mutliply);
                        answer.AddToExpression(new Term(term), true);
                    }
                    else
                    {
                        answer.AddToExpression(piece, true);
                    }

                return answer;

                #endregion noDevisor
            }

        }
        private List<IExpressionPiece> GetTerms(List<IExpressionPiece> pieces)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();
            Expression expression = new Expression();

            foreach (IExpressionPiece piece in pieces)
                expression.AddToExpression(piece, true);



            if (!expression.NoBraces(expression.Numerator))
            {
                //remove braces
                throw new NotImplementedException();
            }

            foreach (IExpressionPiece piece in expression.Numerator)
                if (piece.GetTypePiece() == ExpressionPieceType.Term)
                    answer.Add(piece);
            return answer;
        }
        public List<Term> GetTerms1(List<IExpressionPiece> pieces)
        {
            List<Term> answer = new List<Term>();
            Expression expression = new Expression();

            foreach (IExpressionPiece piece in pieces)
                expression.AddToExpression(piece, true);



            if (!expression.NoBraces(expression.Numerator))
            {
                //remove braces
                throw new NotImplementedException();
            }

            foreach (IExpressionPiece piece in expression.Numerator)
                if (piece.GetTypePiece() == ExpressionPieceType.Term)
                    answer.Add((Term)piece);
            return answer;
        }
        public IAlgebraPiece DivideExpressions(Expression one, Expression two)
        {
            //one.RemoveBraces();
            //two.RemoveBraces();

            if ((one.Denominator.Count > 0 && !AllOne(one.Denominator)) || (two.Denominator.Count > 0 && !AllOne(two.Denominator)))
            {

                #region One has a devisor
                Expression expression = new Expression();
                expression.AddToExpression(new Brace('('), true);
                //add opening brace

                foreach (IExpressionPiece top in one.Numerator)
                    expression.AddToExpression(top, true);

                if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), true);

                expression.AddToExpression(new Brace(')'), true);

                expression.AddToExpression(new Brace('('), true);
                //add opening brace

                foreach (IExpressionPiece top in two.Denominator)
                    expression.AddToExpression(top, true);

                if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), true);

                expression.AddToExpression(new Brace(')'), true);



                expression.AddToExpression(new Brace('('), false);
                //add opening brace

                foreach (IExpressionPiece top in one.Denominator)
                    expression.AddToExpression(top, false);

                if (expression.Denominator[expression.Denominator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), false);

                expression.AddToExpression(new Brace(')'), false);

                expression.AddToExpression(new Brace('('), false);
                //add opening brace

                foreach (IExpressionPiece top in two.Numerator)
                    expression.AddToExpression(top, false);

                if (expression.Denominator[expression.Denominator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                    expression.AddToExpression(new Term(1), false);

                expression.AddToExpression(new Brace(')'), false);

                //expression.RemoveBraces();
                return expression;

                #endregion One has a devisor
            }
            else
            {
                Expression expression = new Expression();
                expression.AddToExpression(true, one.Numerator);
                expression.AddToExpression(false, two.Numerator);
                return expression;
            }
        }
        public IAlgebraPiece MultiplyExpressions(Expression one, Expression two)
        {
            #region Devisors
            Expression expression = new Expression();
            expression.AddToExpression(new Brace('('), true);
            //add opening brace

            foreach (IExpressionPiece top in one.Numerator)
                expression.AddToExpression(top, true);

            if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                expression.AddToExpression(new Term(1), true);

            expression.AddToExpression(new Brace(')'), true);

            expression.AddToExpression(new Brace('('), true);
            //add opening brace

            foreach (IExpressionPiece top in two.Numerator)
                expression.AddToExpression(top, true);

            if (expression.Numerator[expression.Numerator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                expression.AddToExpression(new Term(1), true);

            expression.AddToExpression(new Brace(')'), true);



            expression.AddToExpression(new Brace('('), false);
            //add opening brace

            foreach (IExpressionPiece top in one.Denominator)
                expression.AddToExpression(top, false);

            if (expression.Denominator[expression.Denominator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                expression.AddToExpression(new Term(1), false);

            expression.AddToExpression(new Brace(')'), false);

            expression.AddToExpression(new Brace('('), false);
            //add opening brace

            foreach (IExpressionPiece top in two.Denominator)
                expression.AddToExpression(top, false);

            if (expression.Denominator[expression.Denominator.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
                expression.AddToExpression(new Term(1), false);

            expression.AddToExpression(new Brace(')'), false);

            //expression.RemoveBraces();
            return expression;

            #endregion Devisors
        }

        public List<Term> GroupLikeTerms(List<Term> terms)
        {
            List<Term> constants = new List<Term>();
            List<Term> other = new List<Term>();

            foreach (Term term in terms)
                if (term != null)
                {
                    if (term.Constant)
                        constants.Add(term);
                    else
                        other.Add(term);
                }

            List<Term> answer = Sort(other);
            answer.AddRange(constants);
            return answer;

        }

        public List<Term> GroupLikeTerms(params Term[] terms)
        {
            List<Term> answer = new List<Term>();
            foreach (Term term in terms)
                answer.Add(term);
            return GroupLikeTerms(answer);
        }

        public List<Term> AddLikeTerms(List<Term> terms)
        {
            if (terms.Count == 0)
                return terms;

            for (int i = 0; i < terms.Count; i++)
            {
                for (int j = 0; j < terms.Count; j++)
                    if (j != i)
                    {
                        if (terms[i] != null && terms[j] != null)
                        {
                            if (AreLikeTerms(new Term(terms[i]), new Term(terms[j]), MathFunction.Add))
                            {
                                if (new Term(terms[i]).CancelOut(new Term(terms[j])))
                                {
                                    terms[j] = null;
                                    terms[i] = null;
                                }
                                else
                                {
                                    terms[i] = (Term)Calculate(new Term(terms[i]), new Term(terms[j]), MathFunction.Add);
                                    terms[j] = null;
                                }
                            }
                        }
                    }
            }


            return terms;
        }
        public List<TrigTerm> AddLikeTerms(List<TrigTerm> terms)
        {
            return DoFunctionForLike(terms, MathFunction.Add);
        }
        public List<TrigTerm> DoFunctionForLike(List<TrigTerm> terms, MathFunction function)
        {
            if (terms.Count == 0)
                return terms;

            for (int i = 0; i < terms.Count; i++)
            {
                for (int j = 0; j < terms.Count; j++)
                    if (j != i)
                    {
                        if (terms[i] != null && terms[j] != null)
                        {
                            if (AreLikeTerms(new TrigTerm(terms[i]), new TrigTerm(terms[j]), function))
                            {
                                TrigExpression answer = (Calculate(new TrigTerm(terms[i]), new TrigTerm(terms[j]), function));

                                if (answer != null && answer.Numerator.Count == 0)
                                {
                                    terms[j] = null;
                                    terms[i] = null;
                                }
                                else
                                {
                                    terms[i] = (TrigTerm)answer.Numerator[0];
                                    terms[j] = null;
                                }
                            }
                        }
                    }
            }


            return terms;
        }

        public void DoFunctionForLike(ref List<TrigTerm> one, ref List<TrigTerm> two, MathFunction function)
        {
            for (int i = 0; i < one.Count; i++)
            {
                for (int j = 0; j < two.Count; j++)

                    if (one[i] != null && two[j] != null)
                    {
                        //if (AreLikeTerms(new TrigTerm(one[i]), new TrigTerm(two[j]), function))
                        //{
                        if (one[i].TrigFun == two[i].TrigFun)
                        {
                            throw new NotImplementedException(); //Add coEfficient to terms before passing to math Term functions
                            Term answer = (function == MathFunction.Divide) ? (Term)DivideMegaDo((Term)one[i].Term, (Term)two[j].Term)
                                : (Term)MultiplyUnlike(((Term)one[i].Term), (Term)two[j].Term);

                            if (answer.Power == 0)
                            {
                                two[j] = null;
                                one[i] = null;
                            }
                            else
                            {
                                one[i] = new TrigTerm(answer, one[i].TrigFun);
                                two[j] = null;
                            }

                        }
                        //}
                    }
            }
        }

        public List<Term> AddLikeTerms(params Term[] terms)
        {
            List<Term> answer = new List<Term>();

            foreach (Term term in terms)
                answer.Add(term);
            return AddLikeTerms(answer);
        }
        public List<IExpressionPiece> AddLikeTerms(List<IExpressionPiece> terms)
        {
            foreach (IExpressionPiece piece in terms)
                if (piece.GetTypePiece() != ExpressionPieceType.Term)
                    throw new Exception(); /// can't add where there are signs and braces

            List<Term> termsConverted = new List<Term>();

            foreach (IExpressionPiece piece in terms)
                termsConverted.Add((Term)piece);


            termsConverted = AddLikeTerms(termsConverted);


            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            foreach (Term term in termsConverted)
            {
                if (term == null)
                    continue;
                if (term.CancelledOut)
                    continue;
                if (term.Constant && term.CoEfficient == 0)
                    continue;

                answer.Add(term);
            }

            return answer;
        }

        public List<IExpressionPiece> AddLikeTerms(params IExpressionPiece[] terms)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            foreach (IExpressionPiece term in terms)
                answer.Add(term);
            return AddLikeTerms(answer);
        }

        #endregion Calculator public functionality

        #region Math Functions For Terms

        private Term LikeDenominators(Term one, Term two, MathFunction fun)
        {
            Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
            one.Devisor = null;
            two.Devisor = null;
            Term answer = (fun == MathFunction.Add) ? (Term)Add(one, two, true) : (Term)Subtract(one, two, true);
            answer.Devisor = new Term(devisorOne);
            return answer;
        }

        private IAlgebraPiece Add(Term one, Term two, bool like)
        {
            if (!like)
            {
                return null;
                #region food
                if (one.Devisor != null || two.Devisor != null)
                {
                    //Term devisorOne = (one.Devisor == null) ? new Term(1) : new Term(one.Devisor);
                    //Term devisorTwo = (two.Devisor == null) ? new Term(1) : new Term(two.Devisor);

                    //one.Devisor = null;
                    //two.Devisor = null;


                    //Term denominator = (Term)Multiply(devisorOne, devisorTwo, AreLikeTerms(devisorOne, devisorTwo, MathFunction.Add));
                    //Expression expression = new Expression();
                    //expression.AddToExpression((Term)Multiply(one, devisorTwo, AreLikeTerms(one, devisorTwo, MathFunction.Add)), true);
                    //expression.AddToExpression(new Sign("+"), true);
                    //expression.AddToExpression((Term)Multiply(two, devisorOne, AreLikeTerms(two, devisorOne, MathFunction.Add)), true);
                    //expression.AddToExpression(denominator, false);

                    //return expression;
                    return null;

                }
                else
                {
                    Expression expression = new Expression();
                    expression.AddToExpression(one, true);
                    expression.AddToExpression(new Sign("+"), true);
                    expression.AddToExpression(two, true);
                    return expression;
                }
                #endregion Food
            }
            else
            {
                if (one.Constant)
                    return AddConstants(one, two);
                else
                    return AddLike(one, two);
            }
        }
        private IAlgebraPiece AddConstants(Term one, Term two)
        {
            if (one.Root > 1 || two.Root > 1)
                return null;

            Term answer = new Term((int.Parse(one.Sign + one.CoEfficient.ToString())
            + int.Parse(two.Sign + two.CoEfficient.ToString())), one.Power);

            int power = int.Parse((one.PowerSign + one.Power.ToString())) +
                int.Parse(two.PowerSign + two.Power.ToString());

            if (one.Devisor != null || two.Devisor != null)
            {
                Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term devisorTwo = (two.Devisor == null) ? new Term(1) : two.Devisor;

                Term denominator = new Term(devisorOne.CoEfficient * devisorTwo.CoEfficient);
                denominator.Sign = ((int.Parse(devisorOne.Sign + "1") * int.Parse(devisorTwo.Sign + "1")) > 0) ? "+" : "-";

                Term nemarator = new Term((one.CoEfficient * devisorTwo.CoEfficient) + (two.CoEfficient * devisorOne.CoEfficient));

                nemarator.Devisor = denominator;
                nemarator.Sign = ((int.Parse(one.Sign + "1") * int.Parse(two.Sign + "1")) > 0) ? "+" : "-";

                return nemarator;
            }

            //if (power == 0)
            //   return new Term(true);//canceled out

            return answer;
        }
        private IAlgebraPiece AddLike(Term one, Term two)
        {
            Term answer = new Term(one.TermBase, one.Power);
            answer.CoEfficient = Math.Abs(int.Parse(one.Sign + one.CoEfficient.ToString())
                + int.Parse(two.Sign + two.CoEfficient.ToString()));

            if (one.MultipledBy != null && two.MultipledBy != null)
                answer.MultipledBy = one.MultipledBy;
            if (one.Devisor != null && two.Devisor != null)
            {
                IAlgebraPiece piece = Add(one.Devisor, two.Devisor, AreLikeTerms(one.Devisor, two.Devisor, MathFunction.Add));
                if (piece.GetType1() == Type.Term)
                    answer.Devisor = (Term)piece;
            }
            answer.Sign = (one.Sign == two.Sign) ? one.Sign : ((one.CoEfficient > two.CoEfficient) ? one.Sign : two.Sign);

            if ((one.Sign.Trim().ToLower() != two.Sign.Trim().ToLower())
                && (one.CoEfficient == two.CoEfficient))
            {
                //cancel out
                answer.CancelledOut = true;
                return answer;
                //return new Term(true);
            }

            if (one.Devisor != null || two.Devisor != null)
            {
                Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term devisorTwo = (two.Devisor == null) ? new Term(1) : two.Devisor;

                if (devisorOne.AreEqual(devisorTwo) && AreLikeTerms(one, two, MathFunction.Add))
                {
                    return LikeDenominators(new Term(one), new Term(two), MathFunction.Add);
                }
                else
                {

                    Term denominator = (Term)Multiply(devisorOne, devisorTwo, AreLikeTerms(devisorOne, devisorTwo, MathFunction.Add));
                    Expression expression = new Expression();
                    expression.AddToExpression((Term)Multiply(one, devisorTwo, AreLikeTerms(one, devisorTwo, MathFunction.Add)), true);
                    expression.AddToExpression(new Sign("+"), true);
                    expression.AddToExpression((Term)Multiply(two, devisorOne, AreLikeTerms(two, devisorOne, MathFunction.Add)), true);
                    expression.AddToExpression(denominator, false);

                    return expression;
                }

            }

            return answer;
        }
        private IAlgebraPiece Subtract(Term one, Term two, bool like)
        {
            if (!like)
            {
                return null;
                #region Food

                //if (one.Devisor != null || two.Devisor != null)
                //{
                //    Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
                //    Term devisorTwo = (two.Devisor == null) ? new Term(1) : two.Devisor;

                //    Term denominator = (Term)Multiply(devisorOne, devisorTwo, AreLikeTerms(devisorOne, devisorTwo, MathFunction.Add));
                //    Expression expression = new Expression();
                //    expression.AddToExpression((Term)Multiply(one, devisorTwo, AreLikeTerms(one, devisorTwo, MathFunction.Add)), true);
                //    expression.AddToExpression(new Sign("-"), true);
                //    expression.AddToExpression((Term)Multiply(two, devisorOne, AreLikeTerms(two, devisorOne, MathFunction.Add)), true);
                //    expression.AddToExpression(denominator, false);

                //    return expression;

                //}
                //else
                //{
                //    Expression expression = new Expression();
                //    expression.AddToExpression(one, true);
                //    expression.AddToExpression(new Sign("-"), true);
                //    expression.AddToExpression(two, true);
                //    return expression;
                //}

                #endregion Food
            }
            else
            {
                if (one.Constant)
                    return SubtractConstants(one, two);
                else
                    return SubtractLike(one, two);
            }
        }
        private IAlgebraPiece SubtractConstants(Term one, Term two)
        {
            if (one.Root > 1 || two.Root > 1)
                return null;

            Term answer = new Term((int.Parse(one.Sign + one.CoEfficient.ToString())
            - int.Parse(two.Sign + two.CoEfficient.ToString())), one.Power);

            int power = int.Parse((one.PowerSign + one.Power.ToString())) -
                int.Parse(two.PowerSign + two.Power.ToString());



            if (one.Devisor != null || two.Devisor != null)
            {
                Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term devisorTwo = (two.Devisor == null) ? new Term(1) : two.Devisor;

                Term denominator = new Term(devisorOne.CoEfficient * devisorTwo.CoEfficient);
                denominator.Sign = ((int.Parse(devisorOne.Sign + "1") * int.Parse(devisorTwo.Sign + "1")) > 0) ? "+" : "-";

                Term nemarator = new Term((one.CoEfficient * devisorTwo.CoEfficient) - (two.CoEfficient * devisorOne.CoEfficient));

                nemarator.Devisor = denominator;
                nemarator.Sign = ((int.Parse(one.Sign + "1") * int.Parse(two.Sign + "1")) > 0) ? "+" : "-";

                return nemarator;

                //fractions
                //Lowest common denominator
            }

            //if (power == 0)
            //  return new Term(true);//canceled out

            return answer;
        }
        private IAlgebraPiece SubtractLike(Term one, Term two)
        {
            Term answer = new Term(one.TermBase, one.Power);
            answer.CoEfficient = Math.Abs(int.Parse(one.Sign + one.CoEfficient.ToString())
                - int.Parse(two.Sign + two.CoEfficient.ToString()));

            if (one.MultipledBy != null && two.MultipledBy != null)
                answer.MultipledBy = one.MultipledBy;
            if (one.Devisor != null && two.Devisor != null)
            {
                IAlgebraPiece piece = Subtract(one.Devisor, two.Devisor, true);
                if (piece.GetType1() == Type.Term)
                    answer.Devisor = (Term)piece;
            }
            answer.Sign = (one.Sign == two.Sign) ? one.Sign : ((one.CoEfficient > two.CoEfficient) ? one.Sign : two.Sign);

            if ((one.Sign.Trim().ToLower() != two.Sign.Trim().ToLower())
                && (one.CoEfficient == two.CoEfficient))
            {
                //cancel out
                return new Term(true);
            }
            if (one.Devisor != null || two.Devisor != null)
            {
                Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term devisorTwo = (two.Devisor == null) ? new Term(1) : two.Devisor;
                if (devisorOne.AreEqual(devisorTwo) && AreLikeTerms(one, two, MathFunction.Add))
                {
                    return LikeDenominators(new Term(one), new Term(two), MathFunction.Subtract);
                }
                else
                {
                    Term denominator = (Term)Multiply(devisorOne, devisorTwo, AreLikeTerms(devisorOne, devisorTwo, MathFunction.Subtract));
                    Expression expression = new Expression();
                    expression.AddToExpression((Term)Multiply(one, devisorTwo, AreLikeTerms(one, devisorTwo, MathFunction.Subtract)), true);
                    expression.AddToExpression(new Sign("-"), true);
                    expression.AddToExpression((Term)Multiply(two, devisorOne, AreLikeTerms(two, devisorOne, MathFunction.Subtract)), true);
                    expression.AddToExpression(denominator, false);

                    return expression;
                }

            }
            return answer;
        }
        private IAlgebraPiece Multiply(Term one, Term two, bool like)
        {
            if (one.Constant && two.Constant)
                return MultiplyConstants(one, two);
            else
                if (like)
                    return MultiplyLike(one, two);
                else
                    return MultiplyUnlike(one, two);
        }
        private IAlgebraPiece MultiplyConstants(Term one, Term two)
        {
            if (one.Root > 1 || two.Root > 1)
                return null;


            Term answer = new Term((int.Parse(one.Sign + one.CoEfficient.ToString())
            * int.Parse(two.Sign + two.CoEfficient.ToString())), one.Power);

            answer.Sign = ((int.Parse((one.Sign + "1")) * int.Parse((two.Sign + "1"))) > 0) ? "+" : "-";


            if (one.Devisor != null || two.Devisor != null)
            {
                Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term devisorTwo = (two.Devisor == null) ? new Term(1) : two.Devisor;

                Term denominator = (Term)Multiply(devisorOne, devisorTwo, true);
                denominator.Sign = ((int.Parse((devisorOne.Sign + "1")) * int.Parse((devisorTwo.Sign + "1"))) > 0) ? "+" : "-";
                answer.Devisor = denominator;
            }

            return answer;///not catering for power
        }
        private IAlgebraPiece MultiplyLike(Term one, Term two)
        {
            int coeff = one.CoEfficient * two.CoEfficient;
            string sign = ((int.Parse((one.Sign + "1")) * int.Parse((two.Sign + "1"))) > 0) ? "+" : "-";
            int power = one.Power + two.Power;
            string powerSign = ((int.Parse((one.PowerSign + "1")) >= int.Parse((two.PowerSign + "1")))) ? "+" : "-";

            Term sol = new Term(one.TermBase, power);
            sol.Sign = sign;
            sol.PowerSign = powerSign;
            sol.CoEfficient = coeff;

            if (one.MultipledBy != null)
                foreach (Term term in one.MultipledBy)
                {
                    term.Power += term.Power;
                    sol.MultipledBy.Add(term);
                }

            if (one.Devisor != null || two.Devisor != null)
            {
                Term devisorOne = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term devisorTwo = (two.Devisor == null) ? new Term(1) : two.Devisor;
                sol.Devisor = (Term)Multiply(devisorOne, devisorTwo, AreLikeTerms(devisorOne, devisorTwo, MathFunction.Add));
            }

            return sol;
        }
        private bool HasARoot(Term one)
        {
            List<Term> oneTop = GetAll(new Term(one), false);

            oneTop.AddRange(GetAll(new Term(one), true));

            foreach (Term term in oneTop)
                if (term != null && term.Root > 1)
                    return true;

            return false;
        }


        private IAlgebraPiece MultiplyUnlike(Term one, Term two)
        {
            if (HasARoot(one) || HasARoot(two))
                return null;

            int coff = one.CoEfficient * two.CoEfficient;
            one.CoEfficient = 1;
            two.CoEfficient = 1;
            Term devisor = null;

            if (one.Devisor != null || two.Devisor != null)
            {
                Term devisorOne = (one.Devisor == null) ? new Term(1) : new Term(one.Devisor);
                Term devisorTwo = (two.Devisor == null) ? new Term(1) : new Term(two.Devisor);
                devisor = (Term)Multiply(new Term(devisorOne), new Term(devisorTwo), AreLikeTerms(new Term(devisorOne), new Term(devisorTwo), MathFunction.Add));
            }

            List<Term> termsOne = (one.MultipledBy != null) ? one.MultipledBy : new List<Term>();
            one.MultipledBy = null;
            if (!one.Constant)
                termsOne.Add(one);

            List<Term> termTwo = (two.MultipledBy != null) ? two.MultipledBy : new List<Term>();
            two.MultipledBy = null;
            if (!two.Constant)
                termTwo.Add(two);

            for (int i = 0; i < termsOne.Count; i++)
            {
                for (int j = 0; j < termTwo.Count; j++)
                    if (termTwo[j] != null)
                        if (termsOne[i].TermBase == termTwo[j].TermBase)
                        {
                            termsOne[i] = (Term)MultiplyLike(new Term(termsOne[i]), new Term(termTwo[j]));
                            termTwo[j] = null;
                        }
            }

            foreach (Term term in termTwo)
                if (term != null)
                    termsOne.Add(term);

            termsOne = Sort(termsOne);


            try
            {
                Term answer = new Term(termsOne[0].TermBase,
                    termsOne[0].Power);
                termsOne.Remove(termsOne[0]);
                answer.Devisor = devisor;
                answer.MultipledBy = termsOne;
                answer.CoEfficient = coff;
                answer.Constant = false;
                answer.PowerSign = ((int.Parse((one.PowerSign + "1")) >= int.Parse((two.PowerSign + "1")))) ? "+" : "-";
                answer.Sign = ((int.Parse((one.Sign + "1")) * int.Parse((two.Sign + "1"))) > 0) ? "+" : "-";

                return answer;
            }
            catch
            {
                return new Term(coff);
            }

        }
        private IAlgebraPiece Divide(Term one, Term two, bool like)
        {
            return DivideMegaDo(one, two);

            //if (one.Constant && two.Constant)
            //    return DivideConstants(one, two);
            //else
            //    //if (like)
            //    //    return DivideLike(one, two);
            //    //else
            //        return DivideUnlike(one, two);
        }
        private IAlgebraPiece DivideConstants(Term one, Term two)
        {
            if (one.Devisor != null || two.Devisor != null)//fractions
            {
                #region fractions

                //fractions
                Term devisor1 = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term deversor2 = (one.Devisor == null) ? new Term(1) : one.Devisor;
                one.Devisor = null;
                two.Devisor = null;

                Term numerator = (Term)Multiply(one, deversor2, AreLikeTerms(one, deversor2, MathFunction.Add));
                numerator.Devisor = (Term)Multiply(two, devisor1, AreLikeTerms(two, devisor1, MathFunction.Add));

                return numerator;

                #endregion fractions
            }
            else
            {
                int coff = one.CoEfficient / two.CoEfficient;
                int power = one.Power - two.Power;
                string sign = (int.Parse(one.Sign + "1") / int.Parse(two.Sign + "1") >= 0) ? "+" : "-";
                string powerSign = (one.Power >= two.Power) ? "+" : "-";

                if ((one.CoEfficient % two.CoEfficient != 0))
                {
                    Expression expression = new Expression();
                    expression.AddToExpression(one, true);
                    expression.AddToExpression(two, false);
                    return expression;
                    //need denominator
                }
                else
                {
                    Term answer = new Term(coff, (power == 0) ? 1 : power, ((sign == "+") ? false : true));


                    if (!((powerSign == "+") ? false : true))
                    {
                        answer.PowerSign = powerSign;
                        return answer;
                    }
                    else
                    {
                        Term term = new Term(1);
                        term.Devisor = answer;
                        return term;
                    }
                }

            }

        }
        private IAlgebraPiece DivideLikeSimplify(Term one, Term two)
        {
            if (one.CoEfficient == two.CoEfficient && two.Power == one.Power
                && two.TermBase == one.TermBase && one.Devisor == null && two.Devisor == null)
            {
                if ((one.MultipledBy != null && !allNull(one.MultipledBy))
                    || two.MultipledBy != null && !allNull(two.MultipledBy))
                    throw new NotImplementedException();
                one = new Term();
                one.CancelledOut = true;
                return one;
            }

            if (one.TermBase == two.TermBase
                && one.Devisor == null && two.Devisor == null)
            {
                if ((one.MultipledBy != null && !allNull(one.MultipledBy))
                    || two.MultipledBy != null && !allNull(two.MultipledBy))
                    throw new NotImplementedException();

                if (one.Power == two.Power)
                {
                    one.Constant = true;
                    one.TermBase = '\0';
                    one.Power = 1;
                }
                else
                {
                    one.Power -= two.Power;
                }
                one.PowerSign = (one.Power - two.Power < 0) ? "-" : "+";
                one.Sign = (int.Parse(one.Sign + "1") / int.Parse(two.Sign + "1") < 0) ? "-" : "1";

                if (one.CoEfficient % two.CoEfficient == 0)
                    one.CoEfficient /= two.CoEfficient;
                else
                {
                    int[] sol = SimplifyNumberFraction(one.CoEfficient, two.CoEfficient);
                    one.CoEfficient = sol[0];
                    one.Devisor = new Term(sol[1]);
                    //two.CoEfficient = sol[1];
                }


                return new Term(one);
            }
            else
            {
                //simplify
                throw new NotImplementedException();
            }
            throw new NotImplementedException(); ;
        }
        public int[] SimplifyNumberFraction(int top, int bottom)
        {
            int max = (top > bottom ? bottom : top);

            if (top == 3 || top == 2 || top == 1)
                return new int[] { top, bottom };

            for (int i = 2; i < max; i++)
            {
                if (top % i == 0 && bottom % i == 0) //lcm
                    return SimplifyNumberFraction((top / i), (bottom / i));
            }
            return null;
        }
        private Term Simplify(Term term)
        {
            if (term.Devisor != null && term.Devisor.Devisor == null
                && (term.MultipledBy == null || allNull(term.MultipledBy))
                && (term.Devisor.MultipledBy == null || allNull(term.Devisor.MultipledBy)))
            {
                Term div = new Term(term.Devisor);
                Term top = new Term(term);
                top.Devisor = null;

                return (Term)DivideLikeSimplify(top, div);
            }

            return term;
            //throw new NotImplementedException();
        }
        private IAlgebraPiece DivideUnlike(Term one, Term two)
        {
            if (one.Devisor != null || two.Devisor != null)
            {
                #region fractions

                //fractions
                Term devisor1 = (one.Devisor == null) ? new Term(1) : one.Devisor;
                Term deversor2 = (two.Devisor == null) ? new Term(1) : two.Devisor;
                one.Devisor = null;
                two.Devisor = null;

                Term numerator = (Term)Multiply(new Term(one), new Term(deversor2), AreLikeTerms(new Term(one), new Term(deversor2), MathFunction.Add));
                numerator.Devisor = (Term)Multiply(new Term(two), new Term(devisor1), AreLikeTerms(new Term(two), new Term(devisor1), MathFunction.Add));

                return FixAfterDivision(Simplify(numerator));

                #endregion fractions
            }
            else
            {
                int coff = one.CoEfficient / two.CoEfficient;
                //int numeratorPower = (int)(one.Power / two.Power);
                //one.CoEfficient = 1;
                //two.CoEfficient = 1;

                List<Term> termsOne = (one.MultipledBy != null) ? one.MultipledBy : new List<Term>();
                one.MultipledBy = null;
                if (!one.Constant)
                    termsOne.Add(one);

                List<Term> termsTwo = (two.MultipledBy != null) ? two.MultipledBy : new List<Term>();
                two.MultipledBy = null;
                if (!two.Constant)
                    termsTwo.Add(two);

                Term numerator = (one.CoEfficient % two.CoEfficient == 0) ? new Term(one.CoEfficient) : null;
                //numerator.Power = numeratorPower;
                Term denominator = (one.CoEfficient % two.CoEfficient == 0) ? new Term(two.CoEfficient) : null;

                for (int i = 0; i < termsTwo.Count; i++)
                {
                    for (int j = 0; j < termsOne.Count; j++)
                    {
                        if (termsOne[j] != null && termsTwo[i] != null)
                            if (termsTwo[i].TermBase == termsOne[j].TermBase)
                            {
                                termsOne[j] = (Term)SolveDivide(termsOne[j], termsTwo[i]);
                                termsTwo[i] = null;
                            }
                    }
                }

                if (!allNull(termsTwo))
                {
                    bool added = false;

                    foreach (Term term in termsTwo)
                        if (term != null)
                        {
                            if (!added && (denominator == null ||
                                (denominator.CoEfficient == 1 && denominator.Constant)))
                            {
                                denominator = term;
                                added = true;
                            }
                            else
                                denominator.MultipledBy.Add(term);
                        }
                }

                try
                {
                    Term answer = new Term(termsOne[0].TermBase,
                        termsOne[0].Power);
                    termsOne.Remove(termsOne[0]);
                    answer.Devisor = (denominator.Constant && denominator.CoEfficient == 1) ? null : denominator;
                    answer.MultipledBy = termsOne;
                    answer.CoEfficient = coff;
                    answer.Constant = false;
                    //answer.PowerSign = ((int.Parse((one.PowerSign + "1")) >= int.Parse((two.PowerSign + "1")))) ? "+" : "-";
                    answer.PowerSign = ((int.Parse(one.PowerSign + one.Power) >= int.Parse(two.PowerSign + two.Power))) ? "+" : "-";
                    answer.Sign = ((int.Parse((one.Sign + "1")) / int.Parse((two.Sign + "1"))) > 0) ? "+" : "-";

                    if (one.TermBase == two.TermBase
                        && one.Power == two.Power)
                    {
                        //no divisors for sure
                        answer.Constant = true;
                        answer.TermBase = '\0';
                    }

                    return FixAfterDivision(answer);
                }
                catch
                {
                    return new Term(coff);
                }



            }

        }

        public IAlgebraPiece DivideMegaDo(Term one, Term two)
        {
            List<TermOnSteriods> pieces = new List<TermOnSteriods>();
            List<Term> answerPieces = new List<Term>();
            List<Term> answerPiecesDiv = new List<Term>();
            int co1 = one.CoEfficient;
            int co2 = two.CoEfficient;
            one.CoEfficient = 1;
            two.CoEfficient = 1;

            #region Prepare
            if (one.MultipledBy != null)
                foreach (Term term in one.MultipledBy)
                    if (term != null)
                        pieces.Add(new TermOnSteriods(term, Side.Top));
            one.MultipledBy = null;
            if (!one.Constant)
                pieces.Add(new TermOnSteriods(one, Side.Top));


            if (two.MultipledBy != null)
                foreach (Term term in two.MultipledBy)
                    if (term != null)
                        pieces.Add(new TermOnSteriods(term, Side.Bottom));
            two.MultipledBy = null;
            if (!two.Constant)
                pieces.Add(new TermOnSteriods(two, Side.Bottom));
            #endregion Prepare

            #region Process

            while (!DivideMegaDoDone(pieces) && !allNull(pieces))
            {
                DoDivideMegaDo(ref pieces, ref answerPieces);
            }

            foreach (TermOnSteriods term in pieces)//
                if (term != null && term.Side == Side.Top)
                {
                    answerPieces.Add(term.Term);
                }
                else if (term != null && term.Side == Side.Bottom)
                {
                    answerPiecesDiv.Add(term.Term);
                }

            if (answerPieces.Count > 1)
                answerPieces = Sort(answerPieces);

            if (answerPiecesDiv.Count > 1)
                answerPiecesDiv = Sort(answerPiecesDiv);

            #endregion Process

            #region Rap up
            Term div = null;
            Term top = null;

            if (co1 % co2 == 0)
            {
                co1 = co1 / co2;
                co2 = 1;
            }

            #region top

            if (answerPieces.Count > 0)
            {
                top = new Term(answerPieces[0]);
                top.MultipledBy = new List<Term>();
                for (int i = 1; i < answerPieces.Count; i++)
                    top.MultipledBy.Add(answerPieces[i]);
                top.CoEfficient = co1;
            }
            else
            {
                top = new Term(co1);

            }

            #endregion top

            #region Div

            if (answerPiecesDiv.Count > 0)
            {
                div = new Term(answerPiecesDiv[0]);
                div.MultipledBy = new List<Term>();
                for (int i = 1; i < answerPiecesDiv.Count; i++)
                    div.MultipledBy.Add(answerPiecesDiv[i]);
                div.CoEfficient = co2;
            }
            else if (co2 != 1)
            {
                div = new Term(co2);
            }
            else
            {
                div = null;
            }

            #endregion Div



            #endregion Rap up

            top.Devisor = div;

            top.Sign = ((one.Sign == "-" && two.Sign == "+") || (one.Sign == "+" && two.Sign == "-")) ? "-" : "+";

            return top;

            //co1 % co2 == 0 one coE, esle need fractional answer
        }
        private void DoDivideMegaDo(ref List<TermOnSteriods> pieces, ref List<Term> answerPieces)
        {
            Term top1 = null;
            Term bottom1 = null;

            #region find pair

            bool skipping = false;

            for (int i = 0; i < pieces.Count; i++)
            {
                for (int j = 0; j < pieces.Count; j++)
                {
                    if (j != i)
                    {
                        if ((pieces[i] != null && pieces[j] != null) && pieces[j].Term.TermBase == pieces[i].Term.TermBase
                            && pieces[j].Side != pieces[i].Side)
                        {
                            if (pieces[j].Side == Side.Top)
                            {
                                top1 = new Term(pieces[j].Term);
                                bottom1 = new Term(pieces[i].Term);
                            }
                            else
                            {
                                top1 = new Term(pieces[i].Term);
                                bottom1 = new Term(pieces[j].Term);
                            }
                            pieces[i] = null;
                            pieces[j] = null;
                            skipping = true;
                            break;
                        }
                    }
                }
                if (skipping)
                    break;
            }
            #endregion find pair

            if (top1 != null && bottom1 != null)
            {
                Term piece = DoDivideMegaDoFinalStep(top1, bottom1);

                if (!piece.CancelledOut)
                    answerPieces.Add(piece);
            }
        }
        private Term DoDivideMegaDoFinalStep(Term one, Term two)
        {
            return (Term)DivideUnlike(one, two);
        }

        private bool DivideMegaDoDone(List<TermOnSteriods> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                for (int j = 0; j < pieces.Count; j++)
                {
                    if (j != i && pieces[j] != null && pieces[i] != null)
                    {
                        if (pieces[j].Term.TermBase == pieces[i].Term.TermBase
                            && pieces[j].Side != pieces[i].Side)
                            return false;
                    }
                }
            }
            return true;
        }

        public Term FixAfterDivision(Term term)
        {
            #region CancelOut
            if (term.Power == 0 && (term.CoEfficient == 1 || term.CoEfficient == 0))
            {
                if (term.Devisor == null || (term.Devisor != null && FixAfterDivision(term.Devisor).CancelledOut))
                {
                    bool areAllNul = allNull(new List<Term>(term.MultipledBy));
                    if (term.MultipledBy == null || (term.MultipledBy != null && areAllNul))
                    {
                        term.CancelledOut = true;
                        return term;
                    }
                    else
                    {
                        bool allOut = false;
                        foreach (Term term1 in term.MultipledBy)
                            if (!FixAfterDivision(term1).CancelledOut)
                            {
                                allOut = false;
                                break;
                            }

                        if (allOut)
                        {
                            term.CancelledOut = true;
                            return term;
                        }
                    }
                }
            }
            #endregion CancelOut

            #region Fix Remaining part

            if (term.Power == 0)
            {
                term.Power = 1;
                term.TermBase = ' ';
                term.Constant = true;
            }

            if (term.TermBase.ToString() == "\0")
            {
                term.Constant = true;
            }

            if (term.Devisor != null)
                term.Devisor = FixAfterDivision(term.Devisor);

            if (term.MultipledBy != null && !allNull(term.MultipledBy))
                for (int i = 0; i < term.MultipledBy.Count; i++)
                    if (term.MultipledBy[i] != null)
                        term.MultipledBy[i] = FixAfterDivision(term.MultipledBy[i]);
            return term;

            #endregion Fix Remaining part
        }
        private Term SolveDivide(Term one, Term two)
        {
            int coff = one.CoEfficient / two.CoEfficient;
            int power = one.Power - two.Power;
            string PowerSign = ((int.Parse((one.PowerSign + "1")) >= int.Parse((two.PowerSign + "1")))) ? "+" : "-";
            string Sign = ((int.Parse((one.Sign + "1")) / int.Parse((two.Sign + "1"))) > 0) ? "+" : "-";

            Term Answer = new Term(one.TermBase);
            Answer.CoEfficient = (one.CoEfficient % two.CoEfficient == 0) ? coff : one.CoEfficient;
            Answer.Power = power;
            Answer.Sign = Sign;
            Answer.PowerSign = PowerSign;

            if (one.CoEfficient % two.CoEfficient != 0)
            {
                Term devisor = new Term(two.CoEfficient);
                Answer.Devisor = devisor;
            }
            return Answer;
        }
        public bool allNull(List<Term> terms)
        {
            foreach (Term term in terms)
                if (term != null)
                    return false;
            return true;
        }
        public bool allNull(List<TermOnSteriods> terms)
        {
            foreach (TermOnSteriods term in terms)
                if (term != null)
                    return false;
            return true;
        }

        #endregion Math Functions For Terms

        #endregion
    }
    public class TermOnSteriods
    {
        #region Properties

        string _sign = null;

        public string Sign
        {
            get { return _sign; }
            set { _sign = value; }
        }

        int _coEfficient2;

        public int CoEfficient2
        {
            get { return _coEfficient2; }
            set { _coEfficient2 = value; }
        }
        int _coEfficient1;

        public int CoEfficient1
        {
            get { return _coEfficient1; }
            set { _coEfficient1 = value; }
        }

        int _HostTermNumber = -1;

        public int HostTermNumber
        {
            get { return _HostTermNumber; }
            set { _HostTermNumber = value; }
        }

        Term _term = null;
        Side _side = Side.unAssigned;

        public Side Side
        {
            get { return _side; }
            set { _side = value; }
        }

        public Term Term
        {
            get { return _term; }
            set { _term = value; }
        }

        #endregion Properties

        #region Constructor

        public TermOnSteriods(Term term, Side side)
        {
            _term = new Term(term);
            _side = side;
        }
        public TermOnSteriods(int HostTerm, Term term, int coEff, Side side)
        {
            _HostTermNumber = HostTerm;
            _term = new Term(term);
            _coEfficient1 = coEff;
            _side = side;
        }
        public TermOnSteriods(int HostTerm, Term term, int coEff1, string sign, Side side)
        {
            _HostTermNumber = HostTerm;
            _term = new Term(term);
            _coEfficient1 = coEff1;
            _sign = sign;
            _side = side;
        }

        #endregion Constructor

        #region Methods

        #endregion Methods
    }
    public enum Side
    {
        Top, Bottom, unAssigned
    }
    public class LongDivisionSolution
    {
        #region Explanation

        #endregion Explanation

        #region Properties
        bool _failed = false;

        public bool Failed
        {
            get { return _failed; }
            set { _failed = value; }
        }
        bool solved = false;
        List<Term> _a = null;

        public List<Term> A
        {
            get { return _a; }
            set { _a = value; }
        }

        List<Term> _b = new List<Term>();
        public List<Term> B
        {
            get { return _b; }
            set { _b = value; }
        }
        List<List<Term>> C = new List<List<Term>>();
        List<List<Term>> D = new List<List<Term>>();
        bool remainder = false;

        public bool Remainder
        {
            get { return remainder; }
            set { remainder = value; }
        }
        Equation answer = null;

        #endregion Properties

        #region constructor

        public LongDivisionSolution(List<Term> a_div, List<Term> c_numerator) //x - 3 , x^3 - 2x^2  + 4x +3
        {
            this._a = a_div;
            this.C.Add(c_numerator);

            if (a_div.Count != 0 && c_numerator.Count != 0)
            {
                Solve();
            }
            else
                Failed = true;
        }

        #endregion Constructor

        #region Methods

        private void Solve()
        {
            if (!solved)
            {
                solved = true;
                int count = 0;

                do
                {
                    #region Do work
                    Term top = new Term(C[C.Count - 1][0]);
                    Term div = new Term(_a[0]);

                    _b.Add((Term)calc.Calculate(new Term(top), new Term(div), MathFunction.Divide));
                    //divide returns wrong sign
                    //D
                    List<Term> newD = new List<Term>();

                    for (int i = 0; i < _a.Count; i++)
                    {
                        Term top1 = new Term(_a[i]);
                        Term div2 = new Term(_b[_b.Count - 1]);

                        newD.Add((Term)calc.Calculate(new Term(top1), new Term(div2), MathFunction.Mutliply));
                    }
                    D.Add(new List<Term>(newD));
                    //D

                    //new C

                    List<Term> newC = new List<Term>(new List<Term>(C[C.Count - 1]));

                    for (int i = 0; i < D[D.Count - 1].Count; i++)
                    {
                        Term div3 = new Term(D[D.Count - 1][i]);
                        div3.Sign = (div3.Sign == "+") ? "-" : "+";

                        newC.Add(new Term(div3));
                    }

                    newC = RemoveCancelledOut(calc.AddLikeTerms(new List<Term>(newC)));
                    newC = calc.Sort(newC);

                    C.Add(new List<Term>(newC));
                    #endregion Do work

                    #region Safety keeper

                    count++;

                    #endregion Safety Keeper
                }
                while (NotDone(new List<Term>(C[C.Count - 1]), count));

                remainder = HasARemainder(new List<Term>(C[C.Count - 1]));
                //new C

                //final answer
                Expression expression = new Expression();
                for (int i = 0; i < _b.Count; i++)
                {
                    expression.AddToExpression(new Term(_b[i]), true);
                }
                answer = new Equation(new iEquationPiece[] { expression });

                if (remainder)
                {
                    answer.Left.Add(new Sign("+"));
                    Expression expressionTwo = new Expression();
                    expressionTwo.AddToExpression(GetRemainder(), true);
                    for (int i = 0; i < _a.Count; i++)
                        expressionTwo.AddToExpression(_a[i], false);
                    answer.Left.Add(expressionTwo);
                }

                if (B.Count == 1 && B[0].Constant) //...&& B[0].CoEfficient == 1)
                    Failed = true;

                //final answer
            }

        }

        private bool NotDone(List<Term> terms, int count)
        {
            if (count >= 11)
            {
                _failed = true;
                return false;
            }

            for (int i = 0; i < terms.Count; i++)
                if (terms[i] != null && !terms[i].Constant && !terms[i].CancelledOut)
                    return true;
            return false;
        }
        private Term GetRemainder()
        {
            for (int i = 0; i < C[C.Count - 1].Count; i++)
                if (C[C.Count - 1][i] != null && !C[C.Count - 1][i].Constant && !C[C.Count - 1][i].CancelledOut)
                    return new Term(C[C.Count - 1][i]);
            return null;
        }
        private bool HasARemainder(List<Term> terms)
        {
            for (int i = 0; i < terms.Count; i++)
                if (terms[i] != null && terms[i].Constant && terms[i].CoEfficient != 0)
                    return true;
            return false;
        }

        private List<Term> RemoveCancelledOut(List<Term> terms)
        {
            List<Term> answer = new List<Term>();

            for (int i = 0; i < terms.Count; i++)
                if (terms[i] != null && !terms[i].CancelledOut)
                    answer.Add(terms[i]);
            return answer;
        }
        #endregion Methods
        Calculator calc = new Calculator();
    }
    public class Differencial
    {
        #region Properties
        bool done = false;
        List<Term> _answer = new List<Term>();

        public List<Term> Answer
        {
            get { return _answer; }
            set { _answer = value; }
        }
        #endregion Properties

        #region Constuctor

        public Differencial(params Term[] terms)
        {
            foreach (Term myTerm in terms)
            {
                if (!myTerm.Constant)//ignore constants
                {
                    Term current = new Term(myTerm);
                    current.CoEfficient *= current.Power;
                    current.Power -= 1;

                    if (current.Power == 0)
                    {
                        current = new Term(1, (current.Sign == "+") ? false : true);
                    }
                    Answer.Add(new Term(current));
                }
            }
        }

        #endregion Constructor

        #region Methods

        public List<Term> GetAnswer()
        {
            if (!done)
            {
                Answer = new Calculator().AddLikeTerms(new List<Term>(Answer));
                Answer = new Calculator().GroupLikeTerms(new List<Term>(Answer));
                done = true;
            }
            return _answer;
        }

        #endregion Methods
    }
    public class LogarithmicEquation : ISolveForXStep
    {
        #region Properties

        bool isComplete = true;

        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }

        bool isHalfEquation = true;

        public bool IsHalfEquation
        {
            get { return isHalfEquation; }
            set { isHalfEquation = value; }
        }

        List<IAlgebraPiece> _left = new List<IAlgebraPiece>();

        public List<IAlgebraPiece> Left
        {
            get { return _left; }
            set { _left = value; }
        }

        List<IAlgebraPiece> _right = new List<IAlgebraPiece>();

        public List<IAlgebraPiece> Right
        {
            get { return _right; }
            set { _right = value; }
        }

        SignType _split = SignType.None;

        public SignType Split
        {
            get { return _split; }
            set { _split = value; }
        }

        #endregion Properties

        #region Constructor

        public LogarithmicEquation(LogarithmicEquation eq)
        {
            this.isHalfEquation = eq.isHalfEquation;
            //this.Left = new List<IAlgebraPiece>(eq.Left);

            for (int i = 0; i < eq.Left.Count; i++)
            {
                if (eq.Left[i].GetType1() == Type.Log)
                    this.Left.Add(new LogTerm((LogTerm)eq.Left[i]));
                else if (eq.Left[i].GetType1() == Type.Term)
                    this.Left.Add(new Term((Term)eq.Left[i]));
                else
                    this.Left.Add(new Sign(((Sign)eq.Left[i]).SignType));
            }


            for (int i = 0; i < eq.Right.Count; i++)
            {
                if (eq.Right[i].GetType1() == Type.Log)
                    this.Right.Add(new LogTerm((LogTerm)eq.Right[i]));
                else if (eq.Right[i].GetType1() == Type.Term)
                    this.Right.Add(new Term((Term)eq.Right[i]));
                else
                    this.Right.Add(new Sign(((Sign)eq.Right[i]).SignType));
            }
            //this.Right = new List<IAlgebraPiece>( eq.Right);
            this.Split = eq.Split;
        }

        public LogarithmicEquation(IAlgebraPiece[] left)
        {
            this._left = new List<IAlgebraPiece>(left);
            isHalfEquation = true;
        }

        public LogarithmicEquation(IAlgebraPiece[] left, SignType split, IAlgebraPiece[] right)
        {
            this._left = new List<IAlgebraPiece>(left);
            isHalfEquation = false;
            this._split = split;
            this._right = new List<IAlgebraPiece>(right);
        }
        public LogarithmicEquation(SignType split)
        {
            this._split = split;
        }

        #endregion Constructor

        #region Methods


        public SolveForXStepType GetStepType()
        {
            return SolveForXStepType.LogEquation;
        }

        #endregion Methods
    }
    public class SolveForLogarithmicEquations
    {
        #region Properties
        char _variable = 'x';

        List<ISolveForXStep> _solution = new List<ISolveForXStep>();

        public List<ISolveForXStep> Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        #endregion Properties

        #region Constructor

        public SolveForLogarithmicEquations(LogarithmicEquation eq)
        {
            this.Solution.Add(eq);
            Solve();
        }

        #endregion Constructor

        #region Methods

        private void Solve()
        {
            //throw new NotImplementedException(); //kill referencing....
            //if there is stuff that can be added on a side, do add
            AddSub();

            if (SameBaseApplicable())
            {
                //done
                //check which answer is valid
            }
            else
            {
                if (RelationshipCanBeApplied())
                {
                    ApplyRelationship();
                }
                else
                {
                    //if(!AsolateVariable())
                    throw new NotImplementedException();
                }


            }

        }
        private bool AsolateVariable()
        {
            if (!VariableCanBeAssolated())
                return false;
            else
            {
                LogTerm containingOnlyVariable = GetVariableForAssolation();
                LogTerm termOnOppositeSide = GetNonVariableForAssolation();
                return true;
            }
        }
        private void ApplyRelationship()
        {
            Term term = GetConstantForRelationship();
            LogTerm logTerm = GetLogForRelationship();

            Term newTerm = new Term(IsNumber(logTerm.LogBase.ToString()) ? int.Parse(logTerm.LogBase.ToString()) : logTerm.LogBase);
            newTerm.Power = term.CoEfficient;




            Expression left = new Expression(new IExpressionPiece[] { newTerm }, null);
            Expression right1 = new Expression(logTerm.Power);

            Equation eqNew = new Equation(new iEquationPiece[] { left },
                ((LogarithmicEquation)_solution[Solution.Count - 1]).Split, new iEquationPiece[] { right1 });

            _solution.Add(new Equation(eqNew));


            //if (newTerm.Constant)
            //{
            //    newTerm = new Term((int)Math.Pow(newTerm.CoEfficient, newTerm.Power));
            //}


            SolveForX();
        }
        private Term GetConstantForRelationship()
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));

            if ((ContainsOnlyConstant(logEq.Left) && !ContainsOnlyConstant(logEq.Right)))
            {
                IAlgebraPiece piece = logEq.Left[0];

                if (piece.GetType1() == Type.Log)
                    return ((LogTerm)piece).Kill();
                return (Term)piece;
            }
            else if ((!ContainsOnlyConstant(logEq.Left) && ContainsOnlyConstant(logEq.Right)))
            {
                IAlgebraPiece piece = logEq.Right[0];

                if (piece.GetType1() == Type.Log)
                    return ((LogTerm)piece).Kill();
                return (Term)piece;
            }
            return null;
        }
        private LogTerm GetVariableForAssolation()
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));

            if ((ContainsOnlyVariable(logEq.Left) && !ContainsOnlyVariable(logEq.Right)))
            {
                return (LogTerm)logEq.Left[0];
            }
            else if ((!ContainsOnlyVariable(logEq.Left) && ContainsOnlyVariable(logEq.Right)))
            {
                return (LogTerm)logEq.Right[0];
            }
            return null;
        }
        private LogTerm GetNonVariableForAssolation()
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));

            if ((ContainsOnlyVariable(logEq.Left) && !ContainsOnlyVariable(logEq.Right)))
            {
                return new LogTerm((LogTerm)logEq.Right[0]);
            }
            else if ((!ContainsOnlyVariable(logEq.Left) && ContainsOnlyVariable(logEq.Right)))
            {
                return new LogTerm((LogTerm)logEq.Left[0]);
            }
            return null;
        }
        private LogTerm GetLogForRelationship()
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));

            if ((ContainsOnlyConstant(logEq.Left) && !ContainsOnlyConstant(logEq.Right)))
            {
                return new LogTerm((LogTerm)logEq.Right[0]);
            }
            else if ((!ContainsOnlyConstant(logEq.Left) && ContainsOnlyConstant(logEq.Right)))
            {
                return new LogTerm((LogTerm)logEq.Left[0]);
            }
            return null;
        }
        private bool VariableCanBeAssolated()
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));

            if ((ContainsOnlyVariable(logEq.Left) && !ContainsOnlyVariable(logEq.Right))
                || (!ContainsOnlyVariable(logEq.Left) && ContainsOnlyVariable(logEq.Right)))
                return true;
            return false;
        }
        private bool RelationshipCanBeApplied()
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));

            if ((ContainsOnlyConstant(logEq.Left) && !ContainsOnlyConstant(logEq.Right))
                || (!ContainsOnlyConstant(logEq.Left) && ContainsOnlyConstant(logEq.Right)))
                return true;
            return false;
        }
        private bool ContainsOnlyConstant(List<IAlgebraPiece> pieces)
        {
            if (pieces.Count == 1 && (pieces[0].GetType1() == Type.Term ||
                (pieces[0].GetType1() == Type.Log && (((LogTerm)pieces[0]).Kill()) != null)))
                return true;

            //for (int i = 0; i < pieces.Count; i++)
            //{
            //    if (pieces[i].GetType1() != Type.Term && pieces[i].GetType1() != Type.Sign)
            //        return false;
            //}
            return false;
        }
        private bool ContainsOnlyVariable(List<IAlgebraPiece> pieces)
        {
            if (pieces.Count == 1 && pieces[0].GetType1() == Type.Log &&
                 ((LogTerm)pieces[0]).Power.Numerator.Count == 1 &&
                ((LogTerm)pieces[0]).Power.Numerator[0].GetTypePiece() == ExpressionPieceType.Term
                && ((Term)((LogTerm)pieces[0]).Power.Numerator[0]).TermBase == _variable)
                return true;
            return false;
        }
        private bool SameBaseApplicable()
        {
            LogTerm value = null;
            if (BasesCanBeTheSame(ref value))
            {
                int tester = 0;
                //make bases the same
                if (int.TryParse(value.LogBase.ToString(), out tester))
                    MakeBasesTheSame(value);
                FixForDrop();
                DropLogs();//tested!

                SolveForX();
                return true;
            }
            return false;
        }
        private bool AddSub()
        {

            LogarithmicEquation logEq = new LogarithmicEquation((LogarithmicEquation)Solution[Solution.Count - 1]);
            while (!(logEq.Left.Count == 1 && logEq.Right.Count == 1))
            {
                LogTerm answerLeft = new LogTerm(new List<IAlgebraPiece>(logEq.Left).ToArray());
                LogTerm answerRight = new LogTerm(new List<IAlgebraPiece>(logEq.Right).ToArray());
                logEq.Left = new List<IAlgebraPiece>();
                logEq.Right = new List<IAlgebraPiece>();
                logEq.Left.Add(answerLeft);
                logEq.Right.Add(answerRight);
                Solution.Add(new LogarithmicEquation(logEq));
                logEq = new LogarithmicEquation((LogarithmicEquation)Solution[Solution.Count - 1]);
            }
            return false;
        }
        private void SolveForX()
        {
            FixExpressions();
            SolveForX x = new SolveForX((Equation)Solution[Solution.Count - 1]);
            for (int i = 1; i < x.Solution.Count; i++)
            {
                Solution.Add(x.Solution[i]);
            }
        }
        private void FixExpressions()
        {
            bool workedLeft = false, workedRight = false;
            Equation logEq = new Equation((Equation)Solution[Solution.Count - 1]);
            logEq.Left = FixExpression(new List<iEquationPiece>(logEq.Left), ref workedLeft);
            logEq.Right = FixExpression(new List<iEquationPiece>(logEq.Right), ref workedRight);

            if (workedLeft || workedRight)
                Solution[Solution.Count - 1] = (new Equation(logEq));
        }
        private List<iEquationPiece> FixExpression(List<iEquationPiece> pieces, ref bool worked)
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression current = new Expression((Expression)pieces[i]);
                    Expression expression = new Expression();
                    expression.BracePowerNumerator = current.BracePowerNumerator;
                    expression.BracePowerDenominator = current.BracePowerDenominator;
                    expression.RootPowerDenominator = current.RootPowerDenominator;
                    expression.RootPowerNumerator = current.RootPowerNumerator;

                    for (int k = 0; k < current.Numerator.Count; k++)
                    {
                        if (current.Numerator[k].GetTypePiece() != ExpressionPieceType.Sign)
                            expression.Numerator.Add(current.Numerator[k]);
                        else
                        {

                            if (((Sign)current.Numerator[k]).SignType != SignType.Add
                                && ((Sign)current.Numerator[k]).SignType != SignType.Subtract)
                                throw new NotImplementedException();
                            else
                            {
                                bool negative = (((Sign)current.Numerator[k]).SignType == SignType.Add) ? false : true;
                                Term term = new Term(1, negative);
                                expression.Numerator.Add((Term)calc.Calculate(term,
                                    new Term((Term)current.Numerator[k + 1]), MathFunction.Mutliply));
                                worked = true;
                            }
                            k++;
                        }
                    }
                    pieces[i] = expression;
                }
            }
            return pieces;
        }
        Calculator calc = new Calculator();
        private void DropLogs()
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));
            Equation eqCurrent = new Equation(logEq.Split);
            eqCurrent.Left = DropLogs(new List<IAlgebraPiece>(logEq.Left));
            eqCurrent.Right = DropLogs(new List<IAlgebraPiece>(logEq.Right));
            Solution.Add(new Equation(eqCurrent));
        }
        private void FixForDrop()
        {
            bool workedLeft = false, workedRight = false;
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]).Split);

            logEq.Left = new List<IAlgebraPiece>(FixForDrop(new List<IAlgebraPiece>(((LogarithmicEquation)Solution[Solution.Count - 1]).Left), ref workedLeft));
            logEq.Right = new List<IAlgebraPiece>(FixForDrop(new List<IAlgebraPiece>(((LogarithmicEquation)Solution[Solution.Count - 1]).Right), ref workedRight));

            if (workedRight || workedLeft)
                Solution.Add(new LogarithmicEquation(logEq));
        }
        private List<IAlgebraPiece> FixForDrop(List<IAlgebraPiece> pieces, ref bool worked)
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>();

            for (int i = 0; i < pieces.Count; i++)
            {

                if (pieces[i].GetType1() == Type.Log)
                {
                    LogTerm current = new LogTerm((LogTerm)pieces[i]);
                    current = current.Fix();
                    answer.Add(new LogTerm(current));

                    if (((LogTerm)pieces[i]).Coefficient != current.Coefficient)
                        worked = true;
                }
                else
                {
                    if (pieces[i].GetType1() == Type.Sign)
                        answer.Add(new Sign(((Sign)pieces[i]).SignType));
                    else throw new NotImplementedException();
                }
            }
            return answer;
        }
        private List<iEquationPiece> DropLogs(List<IAlgebraPiece> pieces)
        {
            List<iEquationPiece> answer = new List<iEquationPiece>();

            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetType1() == Type.Log)
                {
                    LogTerm current = new LogTerm((LogTerm)pieces[i]);
                    current = current.Fix();
                    answer.Add(current.Power);

                }
                else
                {
                    if (pieces[i].GetType1() == Type.Sign)
                        answer.Add((Sign)pieces[i]);
                    else throw new NotImplementedException();
                }
            }
            return answer;
        }
        private void MakeBasesTheSame(LogTerm asTerm)
        {
            LogarithmicEquation logEq = new LogarithmicEquation(((LogarithmicEquation)Solution[Solution.Count - 1]));

            logEq.Left = MakeBasesTheSame(asTerm, logEq.Left);
            logEq.Right = MakeBasesTheSame(asTerm, logEq.Right);
            Solution.Add(new LogarithmicEquation(logEq));
        }
        private List<IAlgebraPiece> MakeBasesTheSame(LogTerm asTerm, List<IAlgebraPiece> pieces)
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>();
            bool mainIsNum = IsNumber(asTerm.LogBase.ToString());

            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetType1() == Type.Log)
                {
                    LogTerm current = new LogTerm((LogTerm)pieces[i]);
                    if (!mainIsNum)
                    {
                        if (current.LogBase == asTerm.LogBase)
                            answer.Add(pieces[i]);
                        else throw new NotImplementedException();
                    }
                    else if (IsNumber(current.LogBase.ToString()))
                    {
                        #region Number

                        if (asTerm.LogBase != current.LogBase)
                        {
                            int newBase = int.Parse(asTerm.LogBase.ToString());
                            int oldBase = int.Parse(current.LogBase.ToString());

                            double check = Math.Log(oldBase, newBase);

                            if (isGood(check))
                            {
                                Expression expression = new Expression(((LogTerm)pieces[i]).Power);

                                if (expression.Numerator.Count == 1)
                                {
                                    Term currentTerm = (Term)expression.Numerator[0];
                                    if (currentTerm.Power == 1)
                                        currentTerm.Power = (int)check;
                                    else
                                        currentTerm.Power += (int)check;
                                    //currentTerm.CoEfficient = 1;// int.Parse(asTerm.LogBase.ToString());
                                    expression.Numerator[0] = currentTerm;
                                    answer.Add(new LogTerm(((LogTerm)pieces[i]).Coefficient, asTerm.LogBase, expression));
                                }
                                else
                                {
                                    if (expression.BracePowerNumerator == 1)
                                        expression.BracePowerNumerator = (int)check;
                                    else
                                        expression.BracePowerNumerator += (int)check;
                                    answer.Add(new LogTerm(((LogTerm)pieces[i]).Coefficient, asTerm.LogBase, expression));
                                }
                                //return answer;
                            }
                            else throw new NotImplementedException();
                        }
                        else answer.Add(new LogTerm(current));
                        #endregion Number
                    }
                    else throw new NotImplementedException();
                }
                else
                {
                    if (pieces[i].GetType1() == Type.Sign)
                        answer.Add((Sign)pieces[i]);
                    else throw new NotImplementedException();
                }
            }
            return answer;
        }
        private bool BasesCanBeTheSame(ref LogTerm index)
        {
            List<IAlgebraPiece> allPiece = new List<IAlgebraPiece>();
            allPiece.AddRange(new List<IAlgebraPiece>(((LogarithmicEquation)
                this._solution[this.Solution.Count - 1]).Left));
            allPiece.AddRange(new List<IAlgebraPiece>(((LogarithmicEquation)
                this._solution[this.Solution.Count - 1]).Right));

            for (int i = 0; i < allPiece.Count; i++)
            {
                if (allPiece[i].GetType1() == Type.Log)
                    if (CanBeSameAsAll(i, allPiece))
                    {
                        index = new LogTerm((LogTerm)allPiece[i]);
                        return true;
                    }
            }
            return false;
        }
        private bool CanBeSameAsAll(int index, List<IAlgebraPiece> allpieces)
        {
            LogTerm main = new LogTerm(allpieces[index]);
            bool mainIsNum = IsNumber(main.LogBase.ToString());

            for (int i = 0; i < allpieces.Count; i++)
            {
                if (i == index) continue;
                if (allpieces[i].GetType1() == Type.Log)
                {
                    LogTerm current = new LogTerm(allpieces[i]);

                    if (!mainIsNum)
                    {
                        if (current.LogBase != main.LogBase)
                            return false;
                    }
                    else if (IsNumber(current.LogBase.ToString()))
                    {
                        if (main.LogBase != current.LogBase)
                        {
                            int newBase = int.Parse(main.LogBase.ToString());
                            int oldBase = int.Parse(current.LogBase.ToString());

                            double check = Math.Log(oldBase, newBase);

                            if (!isGood(check)) { return false; }
                        }
                    }
                }
                else if (allpieces[i].GetType1() == Type.Term)
                    return false;
            }
            return true;
        }
        private bool IsNumber(string value)
        {
            int answer;

            return int.TryParse(value, out answer);

        }
        private bool isGood(double value)
        {
            string answerAssist = value.ToString();

            if (answerAssist.Length == 1)
                return true;
            return false;
        }

        #endregion Methods

    }
    public class SolveForExponentialEquations
    {
        #region Properties

        List<ISolveForXStep> _solution = new List<ISolveForXStep>();

        public List<ISolveForXStep> Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        #endregion Properties

        #region Constructor
        public SolveForExponentialEquations(ExponentialEquation eq)
        {
            this.Solution.Add(eq);
            Solve();
        }
        #endregion Constructor

        #region Methods

        private void Solve()
        {
            Simplify();
            RemoveBraces();

            //what if there are divisors
            if (!OnePerSide())
                throw new NotImplementedException(); //add on each side

            if (AllBasesAreTheSame())
            {
                //drop bases
                DropBases();
            }
            else
            {
                if (OnlyOneContainingVariable())
                {
                    AddNaturalLog();
                    RewriteToExpression();
                    if (!LogClearsOut())
                        DivideByLogBothSides();
                    else ClearOutLog();

                    WorkOutVariableValue();
                    //simpify

                }
                else
                {
                    throw new NotImplementedException();
                    //in this case the it should be possible
                    //to make the bases the same. otherwise 
                    //there is no solution.
                }
            }
        }
        private void WorkOutVariableValue()
        {
            SimpfyFraction();
            SubAdd();
            Divide();

        }
        private bool OnePerSide()
        {
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);

            if (eq.Left.Count != 1 || eq.Right.Count != 1)
                return false;
            return true;
        }
        private bool SubAdd()
        {
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);
            ExponentialTerm host = GetContainingVariable();
            Term term = host.GetTermToSubAdd();

            if (term != null)
            {
                eq.Left = SubAdd(new List<ExponentialTerm>(eq.Left), term);
                eq.Right = SubAdd(new List<ExponentialTerm>(eq.Right), term);
                this._solution.Add(eq);
                return true;
            }
            return false;
        }
        private bool Divide()
        {
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);
            Term term = GetContainingVariable().GetTermToDivideBy();

            if (term != null)
            {
                eq.Left = Divide(new List<ExponentialTerm>(eq.Left), term);
                eq.Right = Divide(new List<ExponentialTerm>(eq.Right), term);
                this._solution.Add(eq);
                return true;
            }
            return false;
        }
        private List<ExponentialTerm> SubAdd(List<ExponentialTerm> pieces, Term subAddTerm)
        {
            List<ExponentialTerm> answer = new List<ExponentialTerm>();

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);
                if (term.ContainsVariable())
                {
                    term.AddSubract(true, subAddTerm);
                }
                else
                {
                    term.AddSubract(false, subAddTerm);
                }
                answer.Add(term);
            }
            return answer;
        }
        private List<ExponentialTerm> Divide(List<ExponentialTerm> pieces, Term subAddTerm)
        {
            List<ExponentialTerm> answer = new List<ExponentialTerm>();

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);
                if (term.ContainsVariable())
                    term.Divide(true, subAddTerm);
                else
                    term.Divide(false, subAddTerm);
                answer.Add(term);
            }
            return answer;
        }
        private bool SimpfyFraction()
        {
            bool didWork = false;
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);
            eq.Left = SimpfyFraction(new List<ExponentialTerm>(eq.Left), ref didWork);
            eq.Right = SimpfyFraction(new List<ExponentialTerm>(eq.Right), ref didWork);

            if (didWork)
            {
                this._solution.Add(eq);
                return true;
            }
            return false;
        }
        private List<ExponentialTerm> SimpfyFraction(List<ExponentialTerm> pieces, ref bool didWork)
        {
            List<ExponentialTerm> answer = new List<ExponentialTerm>();

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);

                if (term.Status == ExpontentialStatus._logarithmicWithDivisor)
                {
                    term.ChangeStatus(ExpontentialStatus._double);
                    didWork = true;
                }
                else if (term.Status == ExpontentialStatus.expressionWithoutLog)
                {
                    term.Status = ExpontentialStatus.variable;
                }
                answer.Add(term);
            }
            return answer;
        }
        private void DivideByLogBothSides()
        {
            ExponentialTerm term = GetContainingVariable();
            Term div = new Term(term.TermBase);//term.GetTermToDivideBy();

            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]).Split);
            eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);
            eq.Left = DivideByLogBothSides(new List<ExponentialTerm>(eq.Left), div);
            eq.Right = DivideByLogBothSides(new List<ExponentialTerm>(eq.Right), div);

            this._solution.Add(eq);
        }
        private List<ExponentialTerm> DivideByLogBothSides(List<ExponentialTerm> pieces, Term div)
        {
            List<ExponentialTerm> answer = new List<ExponentialTerm>();
            ExponentialTerm divTerm = new ExponentialTerm(div);
            divTerm.Status = ExpontentialStatus._logarithmic;

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);
                if (term.ContainsVariable())
                {
                    term.Divide(true);
                    term.Status = ExpontentialStatus.expressionWithoutLog;
                }
                else
                {
                    term.Divide(divTerm);
                }

                answer.Add(term);

            }
            return answer;
        }
        private bool LogClearsOut()
        {
            ExponentialTerm term = GetContainingVariable();

            if (term != null && !term.TermBase.Constant)
            {
                if (term.TermBase.TermBase == 'e')
                    return true;
            }
            return false;
        }
        private void ClearOutLog()
        {
            ExponentialEquation eq = new ExponentialEquation((ExponentialEquation)_solution[Solution.Count - 1]);
            eq.Left = ClearOutLog(new List<ExponentialTerm>(eq.Left));
            eq.Right = ClearOutLog(new List<ExponentialTerm>(eq.Right));

            this._solution.Add(eq);
        }
        private List<ExponentialTerm> ClearOutLog(List<ExponentialTerm> pieces)
        {
            List<ExponentialTerm> answer = new List<ExponentialTerm>();

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);
                if (term.ContainsVariable() && pieces[i].Status == ExpontentialStatus._expression)
                {
                    term.TermBase = new Term(1);
                    term.Status = ExpontentialStatus.expressionWithoutLog;
                    answer.Add(term);
                }
                else answer.Add(term);
            }
            return answer;
        }
        private void RewriteToExpression() // move exponent in front
        {
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);
            eq.Left = RewriteToExpression(new List<ExponentialTerm>(eq.Left));
            eq.Right = RewriteToExpression(new List<ExponentialTerm>(eq.Right));

            this._solution.Add(eq);
        }
        private List<ExponentialTerm> RewriteToExpression(List<ExponentialTerm> pieces)
        {
            List<ExponentialTerm> answer = new List<ExponentialTerm>();

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);
                term.ChangeStatus(ExpontentialStatus._expression);
                answer.Add(term);
            }
            return answer;
        }
        private void AddNaturalLog()
        {
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);
            eq.Left = AddNaturalLog(new List<ExponentialTerm>(eq.Left));
            eq.Right = AddNaturalLog(new List<ExponentialTerm>(eq.Right));

            this._solution.Add(eq);
        }
        private List<ExponentialTerm> AddNaturalLog(List<ExponentialTerm> pieces)
        {
            List<ExponentialTerm> answer = new List<ExponentialTerm>();

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);
                term.ChangeStatus(ExpontentialStatus._logarithmic);
                answer.Add(term);
            }
            return answer;
        }
        private bool OnlyOneContainingVariable()
        {
            ExponentialEquation eq = new ExponentialEquation((ExponentialEquation)_solution[Solution.Count - 1]);

            List<ExponentialTerm> allTerms = new List<ExponentialTerm>();
            allTerms.AddRange(eq.Left);
            allTerms.AddRange(eq.Right);
            int count = 0;

            for (int i = 0; i < allTerms.Count; i++)
            {
                if (allTerms[i].ContainsVariable())
                    count++;
                if (count > 1)
                    return false;
            }
            return true;
        }
        private ExponentialTerm GetContainingVariable()
        {
            ExponentialEquation eq = new ExponentialEquation((ExponentialEquation)_solution[Solution.Count - 1]);

            List<ExponentialTerm> allTerms = new List<ExponentialTerm>();
            allTerms.AddRange(eq.Left);
            allTerms.AddRange(eq.Right);

            for (int i = 0; i < allTerms.Count; i++)
            {
                if (allTerms[i].ContainsVariable())
                    return allTerms[i];
            }
            return null;
        }
        private void DropBases()
        {
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);
            Equation eqNew = new Equation(eq.Split);
            eqNew.Left = DropBases(new List<ExponentialTerm>(eq.Left));
            eqNew.Right = DropBases(new List<ExponentialTerm>(eq.Right));

            SolveForX x = new SolveForX(eqNew);
            this._solution.AddRange(x.Solution);
        }
        private List<iEquationPiece> DropBases(List<ExponentialTerm> pieces)
        {
            List<iEquationPiece> answer = new List<iEquationPiece>();

            for (int i = 0; i < pieces.Count; i++)
            {
                ExponentialTerm term = new ExponentialTerm(pieces[i]);

                if (term.Power != null)
                {
                    if (i != 0)
                    {
                        answer.Add(new Sign(term.TermBase.Sign));
                        if (term.TermBase.Sign == "-")
                            term.TermBase.Sign = "+";
                    }
                    answer.Add(term.Power);
                }
                else if (term.Status == ExpontentialStatus._integer)
                {
                    Expression exp = new Expression();
                    exp.AddToExpression(true, new Term(term.TermBase.Power));
                    answer.Add(exp);
                }
            }
            return answer;
        }
        private bool Simplify()
        {
            ExponentialEquation eq = new ExponentialEquation((ExponentialEquation)_solution[Solution.Count - 1]);
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);

            List<ExponentialTerm> left = new List<ExponentialTerm>(eq.Left);
            List<ExponentialTerm> right = new List<ExponentialTerm>(eq.Right);

            bool workOnLeft = Simplify(ref left);
            bool workOnRight = Simplify(ref right);

            if (workOnLeft || workOnRight)
            {
                eq.Left = new List<ExponentialTerm>(left);
                eq.Right = new List<ExponentialTerm>(right);
                this.Solution.Add(new ExponentialEquation(eq));
                return true;
            }
            return false;
        }
        private bool Simplify(ref List<ExponentialTerm> pieces)
        {
            bool answer = false;

            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].SimplifyBase())
                    answer = true;
            }
            return answer;
        }
        private bool RemoveBraces()
        {
            ExponentialEquation eq = new ExponentialEquation(((ExponentialEquation)_solution[Solution.Count - 1]));
            //eq.Left = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Left);
            //eq.Right = new List<ExponentialTerm>(((ExponentialEquation)_solution[Solution.Count - 1]).Right);

            List<ExponentialTerm> left = new List<ExponentialTerm>(eq.Left);
            List<ExponentialTerm> right = new List<ExponentialTerm>(eq.Right);

            bool workOnLeft = RemoveBraces(ref left);
            bool workOnRight = RemoveBraces(ref right);

            if (workOnLeft || workOnRight)
            {
                eq.Left = new List<ExponentialTerm>(left);
                eq.Right = new List<ExponentialTerm>(right);
                this.Solution.Add(new ExponentialEquation(eq));
                return true;
            }
            return false;
        }
        private bool RemoveBraces(ref List<ExponentialTerm> pieces)
        {
            bool answer = false;

            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].Power != null && pieces[i].Power.CanRemoveBraces())
                {
                    pieces[i].Power.RemoveBraces();
                    answer = true;
                }
            }
            return answer;
        }
        private bool AllBasesAreTheSame()
        {
            ExponentialEquation eq = new ExponentialEquation((ExponentialEquation)_solution[Solution.Count - 1]);

            List<ExponentialTerm> allTerms = new List<ExponentialTerm>();
            allTerms.AddRange(new List<ExponentialTerm>(eq.Left));
            allTerms.AddRange(new List<ExponentialTerm>(eq.Right));

            Term _baseCheck = new Term(allTerms[0].TermBase);

            for (int i = 0; i < allTerms.Count; i++)
            {
                if (_baseCheck.Constant && _baseCheck.CoEfficient != allTerms[i].TermBase.CoEfficient)
                    return false;
                else if (_baseCheck.Constant == false && _baseCheck.TermBase != allTerms[i].TermBase.TermBase)
                    return false;
            }
            return true;
        }

        #endregion Methods
    }
    public class ExponentialEquation : ISolveForXStep
    {

        #region Properties

        bool isComplete = true;

        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }

        List<ExponentialTerm> _left = new List<ExponentialTerm>();

        public List<ExponentialTerm> Left
        {
            get { return _left; }
            set { _left = value; }
        }
        List<ExponentialTerm> _right = new List<ExponentialTerm>();

        public List<ExponentialTerm> Right
        {
            get { return _right; }
            set { _right = value; }
        }
        SignType _split;

        public SignType Split
        {
            get { return _split; }
            set { _split = value; }
        }

        #endregion Properties

        #region Constructor

        public ExponentialEquation(SignType split)
        {
            this.Split = split;
        }

        //public ExponentialEquation(ExponentialEquation eq)
        //{
        //    this.Left = new List<ExponentialTerm>(eq.Left);
        //    this.Right = new List<ExponentialTerm>(eq.Right);
        //    this.Split = eq.Split;
        //}
        public ExponentialEquation(ExponentialEquation eq)
        {
            this._left = new List<ExponentialTerm>();
            for (int i = 0; i < eq._left.Count; i++)
            {
                this._left.Add(new ExponentialTerm(eq._left[i]));
            }
            this.Right = new List<ExponentialTerm>();

            for (int i = 0; i < eq.Right.Count; i++)
            {
                this.Right.Add(new ExponentialTerm(eq.Right[i]));
            }
            this.Split = eq.Split;
        }
        public ExponentialEquation(ExponentialTerm[] left, ExponentialTerm[] right)
        {
            this.Split = SignType.equal;
            if (left != null)
                this.Left.AddRange(left);
            if (right != null)
                this.Right.AddRange(right);
        }
        public ExponentialEquation(ExponentialTerm[] left, SignType split, ExponentialTerm[] right)
        {
            this.Split = split;
            if (left != null)
                this.Left.AddRange(left);
            if (right != null)
                this.Right.AddRange(right);
        }


        #endregion Constructor

        #region Methods

        public SolveForXStepType GetStepType()
        {
            return SolveForXStepType.ExpoEquation;
        }

        #endregion Methods
    }
    public class ExponentialExpression
    {
        #region Properties

        List<ExponentialTerm> numerator;

        public List<ExponentialTerm> Numerator
        {
            get { return numerator; }
            set { numerator = value; }
        }
        List<ExponentialTerm> denominator;

        public List<ExponentialTerm> Denominator
        {
            get { return denominator; }
            set { denominator = value; }
        }

        #endregion Properties

        #region Constructor

        public ExponentialExpression(ExponentialTerm[] numerator, ExponentialTerm[] denominator)
        {
            if (numerator != null)
                this.numerator = new List<ExponentialTerm>(numerator);
            if (denominator != null)
                this.denominator = new List<ExponentialTerm>(denominator);
        }

        #endregion Constructor

        #region Methods

        #endregion Methods
    }
    public class ExponentialTerm
    {
        #region Properties

        #region BuilderAssist

        bool joke = false;

        public bool Joke
        {
            get { return joke; }
            set { joke = value; }
        }

        bool expressJoke = false;

        public bool ExpressJoke
        {
            get { return expressJoke; }
            set { expressJoke = value; }
        }

        int mySelectedindex = 0;

        public int MySelectedindex
        {
            get { return mySelectedindex; }
            set { mySelectedindex = value; }
        }
        SelectedPieceType _mySelectedPieceType;

        public SelectedPieceType MySelectedPieceType
        {
            get { return _mySelectedPieceType; }
            set { _mySelectedPieceType = value; }
        }

        bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        #endregion BuilderAssist

        ExpontentialStatus _status = ExpontentialStatus._exponential;
        public ExpontentialStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        Term _termBase;

        public Term TermBase
        {
            get { return _termBase; }
            set { _termBase = value; }
        }
        Expression _power;

        public Expression Power
        {
            get { return _power; }
            set { _power = value; }
        }
        double _double;

        public double _Double
        {
            get { return _double; }
            set { _double = value; }
        }
        ExponentialTerm divisor = null;

        public ExponentialTerm Divisor
        {
            get { return divisor; }
            set { divisor = value; }
        }

        #endregion Properties

        #region Constructor

        public ExponentialTerm(ExponentialTerm parent)
        {
            this.Status = parent.Status;
            this._termBase = new Term(parent._termBase);
            if (parent.Power != null)
                this._power = new Expression(parent._power);
            this._double = parent._double;
            if (parent.divisor != null)
                this.divisor = new ExponentialTerm(parent.divisor);
        }
        public ExponentialTerm(int _base)
        {
            this._termBase = new Term(_base);
            this._status = ExpontentialStatus._integer;
        }
        public ExponentialTerm(Term _base)
        {
            this._termBase = _base;

            if (_termBase.Constant)
                this._status = ExpontentialStatus._integer;
            else
                throw new NotImplementedException();
        }
        public ExponentialTerm(bool e, Expression power)
        {
            this._termBase = new Term('e');
            this._power = power;
            this.Status = ExpontentialStatus._exponential;
        }
        public ExponentialTerm(bool e, int _base, Expression power)
        {
            this._termBase = new Term('e', _base, 1);
            this._power = power;
            this.Status = ExpontentialStatus._exponential;
        }
        public ExponentialTerm(int _base, Expression power)
        {
            this._termBase = new Term(_base);
            this._power = power;
        }
        public ExponentialTerm(double _doubleV)
        {
            this._double = _doubleV;
            this._status = ExpontentialStatus._double;
        }

        #endregion Constructor

        #region Methods
        public Term GetTermToDivideBy()
        {
            if (this.Status == ExpontentialStatus._expression || this.Status == ExpontentialStatus.expressionWithoutLog
                 || this.Status == ExpontentialStatus.variable)
            {
                Term current = (Term)this._power.Numerator[0];

                if (current.Sign == "-" || current.CoEfficient > 1)
                {
                    Term result = new Term(current.CoEfficient);
                    result.Sign = current.Sign;
                    return result;
                }
            }
            return null;
        }
        public Term GetTermToSubAdd()
        {
            if (this.Status == ExpontentialStatus._expression || this.Status == ExpontentialStatus.expressionWithoutLog
                || this.Status == ExpontentialStatus.variable)
            {
                if (this._power.Numerator.Count != 1)
                {
                    for (int i = 0; i < this.Power.Numerator.Count; i++)
                    {
                        if (((Term)this._power.Numerator[i]).Constant)
                        {
                            Term answer = new Term((Term)this._power.Numerator[this._power.Numerator.Count - 1]);
                            answer.Sign = (answer.Sign == "+") ? "-" : "+";
                            return answer;
                        }
                    }
                }
            }
            return null;
        }
        public bool ContainsVariable()
        {
            if (this.Power == null)
                return false;
            for (int i = 0; i < this._power.Numerator.Count; i++)
            {
                if (!((Term)this._power.Numerator[i]).Constant)
                    return true;
            }
            return false;
        }
        public bool Divide(ExponentialTerm divisor)
        {
            if (this._status == ExpontentialStatus._logarithmic)
            {
                this.divisor = divisor;
                this._status = ExpontentialStatus._logarithmicWithDivisor;
                return true;
            }
            return false;
        }
        public bool Divide(bool byMyOwnBase)
        {
            if (byMyOwnBase && this.Status == ExpontentialStatus._expression)
            {
                this._termBase = new Term(1);
                return true;
            }
            return false;
        }
        public bool Divide(bool byMyOwn, Term term)
        {
            if (byMyOwn && (this.Status == ExpontentialStatus._expression ||
                this.Status == ExpontentialStatus.expressionWithoutLog || this.Status == ExpontentialStatus.variable))
            {
                Term current = (Term)this._power.Numerator[0];
                this._power.Numerator[0] = (Term)calc.Calculate(
                    current, term, MathFunction.Divide);
                return true;
            }
            else if (!byMyOwn && this.Status == ExpontentialStatus._double)
            {
                this._double /= (term.Devisor == null) ? term.CoEfficient : (term.CoEfficient / term.Devisor.CoEfficient);
                return true;
            }
            return false;
        }
        public bool AddSubract(bool byMyOwn, Term term)
        {
            if (byMyOwn && (this.Status == ExpontentialStatus._expression ||
                this.Status == ExpontentialStatus.expressionWithoutLog || this.Status == ExpontentialStatus.variable))
            {
                this._power.Numerator.Add(term);
                this._power.AddLikeTerms();
                return true;
            }
            else if (!byMyOwn && this.Status == ExpontentialStatus._double)
            {
                this._double += int.Parse(term.Sign + ((term.Devisor == null) ? term.CoEfficient : (term.CoEfficient / term.Devisor.CoEfficient)).ToString());
                return true;
            }
            return false;
        }
        public bool ChangeStatus(ExpontentialStatus newStatus)
        {
            if (GoodChangeRequest(newStatus))
                return CarryOutProposedStatusChange(newStatus);
            else return false;
        }
        private bool CarryOutProposedStatusChange(ExpontentialStatus newStatus)
        {
            if (newStatus == ExpontentialStatus._logarithmic)
            {
                this.Status = ExpontentialStatus._logarithmic;
            }
            else if (newStatus == ExpontentialStatus._double)
            {
                this.Status = ExpontentialStatus._double;

                this._double = (Math.Log(_termBase.CoEfficient) /
                    Math.Log(this.divisor._termBase.CoEfficient));
                //calculate double value...
            }
            else if (newStatus == ExpontentialStatus._expression)
            {
                this.Status = ExpontentialStatus._expression;
            }

            return true;
        }
        private bool GoodChangeRequest(ExpontentialStatus proposedStatus)
        {
            if (proposedStatus == ExpontentialStatus._logarithmic)
            {
                if (this.Status == ExpontentialStatus._exponential ||
                    this.Status == ExpontentialStatus._integer)
                    return true;
            }
            else if (proposedStatus == ExpontentialStatus._double)
            {
                if (this.Status == ExpontentialStatus._logarithmicWithDivisor)
                    return true;
            }
            else if (proposedStatus == ExpontentialStatus._expression)
            {
                if (this.Status == ExpontentialStatus._exponential)
                    return true;
            }
            return false;
        }
        public bool SimplifyBase()
        {
            if (this._termBase.Constant && (this.Status == ExpontentialStatus._exponential
                || this.Status == ExpontentialStatus._integer))
            {
                int[] values = GetSmallestBase(this.TermBase);

                if (values[0] != this._termBase.CoEfficient)
                {

                    this._termBase.CoEfficient = values[0];

                    if (this.Status == ExpontentialStatus._exponential)
                        this._power = (Expression)calc.Calculate(new Expression(new IExpressionPiece[] { new Term(values[1]) }, null), this._power, MathFunction.Mutliply);
                    else this._termBase.Power = values[1];
                    return true;
                }
                else if (this._termBase.CoEfficient == 1 && this._termBase.Devisor != null)
                {
                    int[] moreValues = GetSmallestBase(this.TermBase.Devisor);

                    if (this._termBase.Devisor.CoEfficient != moreValues[0])
                    {
                        //negative power
                        this.TermBase.Devisor = null;
                        this._termBase.CoEfficient = moreValues[0];
                        if (this.Status == ExpontentialStatus._exponential)
                            this._power = (Expression)calc.Calculate(new Expression(new IExpressionPiece[] { new Term(moreValues[1], true) }, null), this._power, MathFunction.Mutliply);
                        else this._termBase.Power = moreValues[1];
                        return true;
                    }
                }

            }
            return false;
        }
        private int[] GetSmallestBase(Term worker)
        {
            int myBase = worker.CoEfficient;

            for (int i = 2; i < this._termBase.CoEfficient; i++)
            {
                if (myBase % i == 0)
                {
                    myBase = i;
                    break;
                }
            }

            if (myBase != this._termBase.CoEfficient)
            {
                int originalBaseValue = myBase;
                int count = 1;
                while (myBase != this._termBase.CoEfficient)
                {
                    myBase *= originalBaseValue;
                    count++;

                    if (myBase > this._termBase.CoEfficient)
                        return new int[] { this.TermBase.CoEfficient, 1 };
                }
                return new int[] { originalBaseValue, count };
            }
            return new int[] { myBase, 1 };
        }
        #endregion Methods
        Calculator calc = new Calculator();
    }
    public enum ExpontentialStatus
    {
        _exponential, _integer, _logarithmic, _logarithmicWithDivisor, _double, _expression, expressionWithoutLog, variable
    }
    public class Equation : ISolveForXStep
    {
        #region Properties

        bool splitSelected = false;

        public bool SplitSelected
        {
            get { return splitSelected; }
            set { splitSelected = value; }
        }

        bool isComplete = true;

        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }

        bool isHalfEquation = true;

        public bool IsHalfEquation
        {
            get { return isHalfEquation; }
            set { isHalfEquation = value; }
        }

        List<iEquationPiece> _left = new List<iEquationPiece>();

        public List<iEquationPiece> Left
        {
            get { return _left; }
            set { _left = value; }
        }

        List<iEquationPiece> _right = new List<iEquationPiece>();

        public List<iEquationPiece> Right
        {
            get { return _right; }
            set { _right = value; }
        }

        SignType _split = SignType.None;

        public SignType Split
        {
            get { return _split; }
            set { _split = value; }
        }

        #endregion Properties

        #region Constructor

        public Equation(Equation eq)
        {
            this.isHalfEquation = eq.isHalfEquation;
            //this.Left = new List<iEquationPiece>(new List<iEquationPiece>(eq.Left));

            for (int i = 0; i < eq.Left.Count; i++)
            {
                #region Left

                if (eq.Left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = new Expression((Expression)eq.Left[i]);
                    Expression newExp = new Expression();

                    for (int j = 0; j < exp.Numerator.Count; j++)
                    {
                        if (exp.Numerator[j].GetTypePiece() == ExpressionPieceType.Term)
                            newExp.Numerator.Add(new Term((Term)exp.Numerator[j]));
                        else if (exp.Numerator[j].GetTypePiece() == ExpressionPieceType.Brace)
                            newExp.Numerator.Add(new Brace(((Brace)exp.Numerator[j]).Key));
                        else if (exp.Numerator[j].GetTypePiece() == ExpressionPieceType.SequenceTerm)
                            newExp.Numerator.Add(new SequenceTerm((SequenceTerm)exp.Numerator[j]));
                        else throw new NotImplementedException();
                    }


                    for (int j = 0; j < exp.Denominator.Count; j++)
                    {
                        if (exp.Denominator[j].GetTypePiece() == ExpressionPieceType.Term)
                            newExp.Denominator.Add(new Term((Term)exp.Denominator[j]));
                        else if (exp.Denominator[j].GetTypePiece() == ExpressionPieceType.Brace)
                            newExp.Denominator.Add(new Brace(((Brace)exp.Denominator[j]).Key));
                        else if (exp.Numerator[j].GetTypePiece() == ExpressionPieceType.SequenceTerm)
                            newExp.Numerator.Add(new SequenceTerm((SequenceTerm)exp.Numerator[j]));
                        else throw new NotImplementedException();
                    }
                    this.Left.Add(newExp);
                }
                else
                    this.Left.Add(new Sign(((Sign)eq.Left[i]).SignType));
                #endregion Left
            }
            for (int i = 0; i < eq.Right.Count; i++)
            {
                #region Right

                if (eq.Right[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = new Expression((Expression)eq.Right[i]);
                    Expression newExp = new Expression();

                    for (int j = 0; j < exp.Numerator.Count; j++)
                    {
                        if (exp.Numerator[j].GetTypePiece() == ExpressionPieceType.Term)
                            newExp.Numerator.Add(new Term((Term)exp.Numerator[j]));
                        else if (exp.Numerator[j].GetTypePiece() == ExpressionPieceType.Brace)
                            newExp.Numerator.Add(new Brace(((Brace)exp.Numerator[j]).Key));
                        else throw new NotImplementedException();
                    }


                    for (int j = 0; j < exp.Denominator.Count; j++)
                    {
                        if (exp.Denominator[j].GetTypePiece() == ExpressionPieceType.Term)
                            newExp.Denominator.Add(new Term((Term)exp.Denominator[j]));
                        else if (exp.Denominator[j].GetTypePiece() == ExpressionPieceType.Brace)
                            newExp.Denominator.Add(new Brace(((Brace)exp.Denominator[j]).Key));
                        else throw new NotImplementedException();
                    }
                    this.Right.Add(newExp);
                }
                else
                    this.Right.Add(new Sign(((Sign)eq.Right[i]).SignType));
                #endregion Right
            }

            //this.Right =new List<iEquationPiece>(new List<iEquationPiece>(eq.Right));
            this.Split = eq.Split;
        }

        public Equation(iEquationPiece[] left)
        {
            this._left = new List<iEquationPiece>(left);
            isHalfEquation = true;
        }

        public Equation(iEquationPiece[] left, SignType split, iEquationPiece[] right)
        {
            this._left = new List<iEquationPiece>(left);
            isHalfEquation = false;
            this._split = split;
            this._right = new List<iEquationPiece>(right);
        }
        public Equation(SignType split)
        {
            this._split = split;
        }

        #endregion Constructor

        #region Methods

        #region Simplification

        public bool Simplify()
        {
            bool simplified = false;
            if (!isHalfEquation)
            {
                if (AddingSquareRootToEverythingSimplifies())
                {
                    AddSquareRootToEverything();
                    simplified = true;
                }
                else if (RaisingEverythingToPowerTwoSimplifies())
                {
                    RaiseEverythingToPowerTwo();
                    simplified = true;
                }
            }

            if (SimplifyAll())
                simplified = true;

            return simplified;

        }

        private void AddSquareRootToEverything()
        {

            for (int i = 0; i < _left.Count; i++)
            {
                if (_left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = (Expression)_left[i];
                    exp.RootPowerNumerator = 2;
                    _left[i] = exp;
                }
            }

            if (_right != null)
            {
                for (int i = 0; i < _right.Count; i++)
                {
                    if (_right[i].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        Expression exp = (Expression)_right[i];
                        exp.RootPowerNumerator = 2;
                        _right[i] = exp;
                    }
                }
            }
        }
        private bool AddingSquareRootToEverythingSimplifies()
        {
            for (int i = 0; i < _left.Count; i++)
            {
                if (_left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = (Expression)_left[i];
                    if (exp.AddingSqaureRootSimplifies())
                        return true;
                }
            }

            if (_right != null)
            {
                for (int i = 0; i < _right.Count; i++)
                {
                    if (_right[i].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        Expression exp = (Expression)_right[i];
                        if (exp.AddingSqaureRootSimplifies())
                            return true;
                    }
                }
            }
            return false;
        }
        private bool RaisingEverythingToPowerTwoSimplifies()
        {
            for (int i = 0; i < _left.Count; i++)
            {
                if (_left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = (Expression)_left[i];
                    if (exp.RaisingEverythingToPowerTwoSimplifies())
                        return true;
                }
            }

            if (_right != null)
            {
                for (int i = 0; i < _right.Count; i++)
                {
                    if (_right[i].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        Expression exp = (Expression)_right[i];
                        if (exp.RaisingEverythingToPowerTwoSimplifies())
                            return true;
                    }
                }
            }
            return false;
        }
        private void RaiseEverythingToPowerTwo()
        {
            for (int i = 0; i < _left.Count; i++)
            {
                if (_left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = (Expression)_left[i];
                    exp.BracePowerNumerator *= 2;
                    _left[i] = exp;
                }
            }

            if (_right != null)
            {
                for (int i = 0; i < _right.Count; i++)
                {
                    if (_right[i].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        Expression exp = (Expression)_right[i];
                        exp.BracePowerNumerator *= 2;
                        _right[i] = exp;
                    }
                }
            }
        }
        private bool SimplifyAll()
        {
            bool simplified = false;

            for (int i = 0; i < _left.Count; i++)
            {
                if (_left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = (Expression)_left[i];
                    bool answer = exp.Simplify((isHalfEquation) ? false : true);

                    if (answer)
                        simplified = true;
                    _left[i] = exp;
                }
            }

            if (_right != null)
            {
                for (int i = 0; i < _right.Count; i++)
                {
                    if (_right[i].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        Expression exp = (Expression)_right[i];
                        bool answer = exp.Simplify((isHalfEquation) ? false : true);

                        if (answer)
                            simplified = true;
                        _right[i] = exp;
                    }
                }
            }

            return simplified;
        }
        public bool RemoveBraces()
        {
            bool answer = false;
            for (int i = 0; i < _left.Count; i++)
            {
                if (_left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    if (!((Expression)_left[i]).NoBraces(((Expression)_left[i]).Numerator)
                        || !((Expression)_left[i]).NoBraces(((Expression)_left[i]).Denominator))
                    {
                        Expression current = new Expression((Expression)_left[i]);
                        current.RemoveBraces();
                        _left[i] = current;
                        answer = true;
                    }
                }
            }

            for (int i = 0; i < _right.Count; i++)
            {
                if (_right[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    if (!((Expression)_right[i]).NoBraces(((Expression)_right[i]).Numerator)
                        || !((Expression)_right[i]).NoBraces(((Expression)_right[i]).Denominator))
                    {
                        Expression current = new Expression((Expression)_right[i]);
                        current.RemoveBraces();
                        _right[i] = current;
                        answer = true;
                    }
                }
            }

            return answer;
        }

        #endregion Simplification

        public bool AddLikeTerms()
        {
            bool answer = false;

            for (int i = 0; i < _left.Count; i++)
            {
                if (_left[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = new Expression((Expression)_left[i]);
                    if (exp.AddLikeTerms())
                    {
                        _left[i] = exp;
                        answer = true;
                    }
                }
            }



            for (int i = 0; i < _right.Count; i++)
            {
                if (_right[i].GetEquationPieceType() == EquationPieceType.Expression)
                {
                    Expression exp = new Expression((Expression)_right[i]);
                    if (exp.AddLikeTerms())
                    {
                        _right[i] = exp;
                        answer = true;
                    }
                }
            }


            return answer;
        }

        #endregion Methods

        public SolveForXStepType GetStepType()
        {
            return SolveForXStepType.Equation;
        }
    }
    public interface ISolveForXStep
    {
        SolveForXStepType GetStepType();
    }

	
    public enum SolveForXStepType
    {
        Equation, ExpoEquation, LogEquation, FactorEquations
    }
    public class FactorEquations : ISolveForXStep
    {
        #region Properties

        List<SolveForX> _factoringSolutions = new List<SolveForX>();

        public List<SolveForX> FactoringSolutions
        {
            get { return _factoringSolutions; }
            set { _factoringSolutions = value; }
        }

        #endregion Properties

        public FactorEquations(ISolveForXStep eq)
        {
            List<Expression> pieces = SplitItUp((Expression)((Equation)eq).Left[0]);

            for (int i = 0; i < pieces.Count; i++)
            {
                Expression exp = new Expression();
                exp.AddToExpression(true, new Term(0));

                _factoringSolutions.Add(new SolveForX(new Equation(new iEquationPiece[] { pieces[i] }, ((Equation)eq).Split, new iEquationPiece[] { exp })));
            }
        }

        #region Constructor

        #endregion Constructor

        #region Methods

        private List<Expression> SplitItUp(Expression factored)
        {
            List<Expression> pieces = new List<Expression>();

            Expression piece = new Expression();
            for (int i = 0; i < factored.Numerator.Count; i++)
            {
                if (factored.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    piece.AddToExpression(true, (Term)factored.Numerator[i]);
                else if ((factored.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace))
                {
                    if (((Brace)factored.Numerator[i]).Key == ')')
                    {
                        pieces.Add(new Expression(piece));
                        piece = new Expression();
                    }
                }
            }

            return pieces;
        }

        public SolveForXStepType GetStepType()
        {
            return SolveForXStepType.FactorEquations;
        }

        #endregion Methods
    }
    public class LogTerm : IAlgebraPiece
    {
        #region Properties

        bool isComplete = true;

        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }

        bool showBase;

        public bool ShowBase
        {
            get { return showBase; }
            set { showBase = value; }
        }
        bool showLog;

        public bool ShowLog
        {
            get { return showLog; }
            set { showLog = value; }
        }

        int _coefficient = 1;

        public int Coefficient
        {
            get { return _coefficient; }
            set { _coefficient = value; }
        }
        char _logBase;

        public char LogBase
        {
            get { return _logBase; }
            set { _logBase = value; }
        }
        Expression _power = null;

        public Expression Power
        {
            get { return _power; }
            set { _power = value; }
        }


        #endregion Properties

        #region Constructor
        public LogTerm(LogTerm parent)
        {
            this.Coefficient = parent.Coefficient;
            this.LogBase = parent.LogBase;
            //this.Power = new Expression(parent.Power);


            this.Power = new Expression();

            for (int i = 0; i < parent.Power.Numerator.Count; i++)
            {
                if (parent.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    this.Power.Numerator.Add(new Term((Term)parent.Power.Numerator[i]));
                else if (parent.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                    this.Power.Numerator.Add(new Brace(((Brace)parent.Power.Numerator[i]).Key));
                else throw new NotImplementedException();
            }


            for (int i = 0; i < parent.Power.Denominator.Count; i++)
            {
                if (parent.Power.Denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                    this.Power.Denominator.Add(new Term((Term)parent.Power.Denominator[i]));
                else if (parent.Power.Denominator[i].GetTypePiece() == ExpressionPieceType.Brace)
                    this.Power.Denominator.Add(new Brace(((Brace)parent.Power.Denominator[i]).Key));
                else throw new NotImplementedException();
            }
        }
        public LogTerm(char logBase, Expression power)
        {
            this._logBase = logBase;
            this._power = power;
        }
        public LogTerm(int coefficient, char logBase, Expression power)
        {
            this._coefficient = coefficient;
            this._logBase = logBase;
            this._power = power;
        }
        public LogTerm(params IAlgebraPiece[] pieces)
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>(pieces);
            int count = answer.Count;

            //if(answer.Count == 1 && answer[0].GetType1() == Type.Term)

            while (1 == 1)
            {

                answer = FromList(answer);

                if (answer.Count == 1)
                {
                    LogTerm term = (LogTerm)answer[0];
                    this._coefficient = term._coefficient;
                    this._power = term._power;
                    this._logBase = term._logBase;
                    break;
                }
                else if (count == answer.Count)
                    throw new NotImplementedException();
            }
        }

        #endregion Constructor

        #region Methods

        private List<IAlgebraPiece> FromList(List<IAlgebraPiece> starterPieces)
        {
            if (starterPieces.Count >= 3)
            {
                List<IAlgebraPiece> answer = new List<IAlgebraPiece>();

                for (int i = 0; i <= ((starterPieces.Count - 1) - 3); i++)
                {
                    answer.Add(starterPieces[i]);

                }
                if (((LogTerm)starterPieces[starterPieces.Count - 3])._logBase
                    == ((LogTerm)starterPieces[starterPieces.Count - 1])._logBase
                    && (((Sign)starterPieces[starterPieces.Count - 2]).SignType == SignType.Add
                    || ((Sign)starterPieces[starterPieces.Count - 2]).SignType == SignType.Subtract))
                {
                    answer.Add(Merge((LogTerm)starterPieces[starterPieces.Count - 3],
                        ((Sign)starterPieces[starterPieces.Count - 2]).SignType,
                        (LogTerm)starterPieces[starterPieces.Count - 1]));
                    return answer;
                }

            }
            return starterPieces;
        }
        private LogTerm Merge(LogTerm one, SignType sign, LogTerm two)
        {
            one = FixForMerge(one);
            two = FixForMerge(two);

            if (one._coefficient > 1 || two._coefficient > 1)
                return null;
            else
            {
                if (sign == SignType.Add)
                    return new LogTerm(one._logBase, (Expression)calc.Calculate(one._power, two._power, MathFunction.Mutliply));
                else if (sign == SignType.Subtract)
                    return new LogTerm(one._logBase, (Expression)calc.Calculate(one._power, two._power, MathFunction.Divide));
                else throw new NotImplementedException();
            }
        }
        Calculator calc = new Calculator();
        private LogTerm FixForMerge(LogTerm term)
        {
            if (term._coefficient > 1 && this.Power.Numerator.Count == 1)
            {
                Term currentTerm = (Term)this.Power.Numerator[0];
                if (currentTerm.Power != 1)
                    currentTerm.Power += term._coefficient;
                else
                    currentTerm.Power = term._coefficient;
                this.Power.Numerator[0] = currentTerm;
                //term._power.BracePowerNumerator = term._coefficient;
                term._coefficient = 1;
            }
            return term;
        }
        public LogTerm Fix()
        {
            return FixForMerge(this);
        }
        public Type GetType1()
        {
            return Type.Log;
        }
        public List<List<IAlgebraPiece>> Expand()
        {
            List<List<IAlgebraPiece>> mainAnswer = new List<List<IAlgebraPiece>>();
            mainAnswer.Add(new List<IAlgebraPiece>((new IAlgebraPiece[] { this })));

            while (CanExpand(mainAnswer[mainAnswer.Count - 1]))
            {
                mainAnswer.Add(DoExpansion(new List<IAlgebraPiece>(mainAnswer[mainAnswer.Count - 1])));
            }
            return mainAnswer;
        }
        private List<IAlgebraPiece> DoExpansion(List<IAlgebraPiece> starterPieces)
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>();

            for (int i = 0; i < starterPieces.Count; i++)
                if ((starterPieces[i].GetType1() == Type.Log))
                {
                    if (((Expression)((LogTerm)starterPieces[i]).Power).CanExpand((LogTerm)starterPieces[i]))
                    {
                        List<IAlgebraPiece> subSet = ((Expression)((LogTerm)starterPieces[i]).Power).Expand((LogTerm)starterPieces[i]);
                        answer.AddRange(subSet);
                    }
                    else answer.Add(starterPieces[i]);
                }
                else
                {
                    answer.Add(starterPieces[i]);
                }
            return answer;
        }
        private bool CanExpand(List<IAlgebraPiece> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
                if ((pieces[i].GetType1() == Type.Log))
                {
                    // if (((Expression)pieces[i]).CanExpand())
                    if (((Expression)((LogTerm)pieces[i]).Power).CanExpand((LogTerm)pieces[i]))
                    {
                        return true;
                    }
                }
            return false;
        }

        public Term Kill()
        {
            List<List<IAlgebraPiece>> mainAnswer = new List<List<IAlgebraPiece>>();
            mainAnswer.Add(new List<IAlgebraPiece>((new IAlgebraPiece[] { this })));

            if (!(CanExpand(mainAnswer[mainAnswer.Count - 1])))
            {
                return null;
            }
            else
            {
                List<List<IAlgebraPiece>> answer = Expand();

                if (answer[answer.Count - 1].Count != 1)
                    return null;
                else
                {
                    if (answer[answer.Count - 1][answer[answer.Count - 1].Count - 1].GetType1() == Type.Term)
                        return (Term)answer[answer.Count - 1][answer[answer.Count - 1].Count - 1];
                    //else if((answer[answer.Count - 1][answer[answer.Count - 1].Count - 1].GetType1() == Type.Log))
                    //{
                    //    throw new NotImplementedException();
                    //}
                    else return null;
                }
            }

        }

        #endregion Methods
    }
    public class Inequalities
    {
        #region Properties

        bool Failed = false;
        List<iEquationPiece> _pieces = new List<iEquationPiece>();
        Equation eq = null;
        SolveForX _one = null;
        SolveForX _two = null;

        List<ISolveForXStep> _solution = new List<ISolveForXStep>();

        public List<ISolveForXStep> Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        #endregion Properties

        #region Constructor

        public Inequalities(params iEquationPiece[] pieces)
        {
            this._pieces = new List<iEquationPiece>(pieces);
            Prepare();
            Merge();
            //done
        }
        public Inequalities(Equation eq)
        {
            this._pieces = new List<iEquationPiece>(eq.Left);
            if (eq.Split != null)
                this._pieces.Add(new Sign(eq.Split));
            if (eq.Right != null)
                this._pieces.AddRange(eq.Right);
            if (!TwoSplits())
                Solution.AddRange((new SolveForX(eq)).Solution);
            else
            {
                Prepare();
                Merge();
            }
            //done
        }

        #endregion Constructor

        #region Methods
        private void Merge()
        {
            //AddFirstStep();

            for (int i = 0; i < Math.Max(_one.Solution.Count, _two.Solution.Count); i++)
            {
                Equation eqCur = new Equation(((Equation)_two.Solution[i]).Split);
                eqCur.Left = new List<iEquationPiece>(((Equation)_one.Solution[i]).Right);
                eqCur.Left.Add(new Sign(((Equation)_one.Solution[i]).Split));
                eqCur.Left.AddRange(new List<iEquationPiece>(((Equation)_two.Solution[i]).Left));
                eqCur.Right = new List<iEquationPiece>(((Equation)_two.Solution[i]).Right);

                _solution.Add(new Equation(eqCur));
            }
            //throw new NotImplementedException();
        }
        private void AddFirstStep()
        {
            Equation eqCurrent = new Equation(((Equation)_one.Solution[0]).Split);
            bool addingLeft = true;
            for (int i = 0; i < _pieces.Count; i++)
            {
                if (addingLeft && _pieces[i].GetEquationPieceType() == EquationPieceType.Sign
                    && IsAssinmentSymbol(((Sign)_pieces[i]).SignType))
                    addingLeft = false;
                else
                {
                    if (addingLeft)
                        eqCurrent.Left.Add(_pieces[i]);
                    else
                        eqCurrent.Right.Add(_pieces[i]);
                }

            }
            _solution.Add(new Equation(eqCurrent));
        }
        private SignType IsSplit(SignType current)
        {
            if (current == SignType.greater)
                return SignType.smaller;
            else if (current == SignType.smaller)
                return SignType.greater;
            else if (current == SignType.greaterEqual)
                return SignType.smallerEqual;
            else if (current == SignType.smallerEqual)
                return SignType.greaterEqual;
            else
                return current;
        }
        private SignType FlipSplit(SignType current)
        {
            if (current == SignType.greater)
                return SignType.smaller;
            else if (current == SignType.smaller)
                return SignType.greater;
            else if (current == SignType.greaterEqual)
                return SignType.smallerEqual;
            else if (current == SignType.smallerEqual)
                return SignType.greaterEqual;
            else
                return current;
        }
        private void Prepare()
        {
            if (TwoSplits())
            {
                int[] indices = FindIndicesForAssignment();

                List<iEquationPiece> one1 = GetOnePieces(indices);
                List<iEquationPiece> two2 = GetTwoPieces(indices);
                List<iEquationPiece> mid = GetMiddlePieces(indices);

                Equation eq1 = new Equation(((Sign)_pieces[indices[0]]).SignType);
                eq1.Left = new List<iEquationPiece>(mid);
                eq1.Right = new List<iEquationPiece>(one1);
                _one = new SolveForX(eq1);
                Equation eq2 = new Equation(((Sign)_pieces[indices[0]]).SignType);
                eq2.Left = new List<iEquationPiece>(mid);
                eq2.Right = new List<iEquationPiece>(two2);
                _two = new SolveForX(eq2);
            }
            //else throw new NotImplementedException();
        }
        private List<iEquationPiece> GetOnePieces(int[] indices)
        {
            List<iEquationPiece> terms = new List<iEquationPiece>();

            for (int i = 0; i < indices[0]; i++)
            {
                terms.Add(_pieces[i]);
            }

            return terms;
        }
        Calculator calc = new Calculator();
        private List<iEquationPiece> GetTwoPieces(int[] indices)
        {

            List<iEquationPiece> terms = new List<iEquationPiece>();

            for (int i = indices[1] + 1; i < _pieces.Count; i++)
            {
                terms.Add(_pieces[i]);
            }

            return terms;
        }
        private List<iEquationPiece> GetMiddlePieces(int[] indices)
        {
            List<iEquationPiece> terms = new List<iEquationPiece>();

            #region form the middle

            for (int i = indices[0] + 1; i < indices[1]; i++)
            {
                terms.Add(_pieces[i]);
            }

            #endregion From the middle

            return terms;
        }
        private bool TwoSplits()
        {
            int count = 0;
            for (int i = 0; i < _pieces.Count; i++)
            {
                if (_pieces[i].GetEquationPieceType() == EquationPieceType.Sign
                    && IsAssinmentSymbol(((Sign)(_pieces[i])).SignType))
                {
                    count++;
                    if (count > 1)
                        return true;
                }
            }
            return false;
        }
        private int[] FindIndicesForAssignment()
        {
            List<int> answer = new List<int>();

            for (int i = 0; i < _pieces.Count; i++)
            {
                if (_pieces[i].GetEquationPieceType() == EquationPieceType.Sign
                    && IsAssinmentSymbol(((Sign)(_pieces[i])).SignType))
                {
                    answer.Add(i);
                }
            }

            return answer.ToArray();
        }
        private bool IsAssinmentSymbol(SignType type)
        {
            if (type == SignType.greater || type == SignType.greaterEqual
                || type == SignType.smaller || type == SignType.smallerEqual)
                return true;
            return false;
        }

        #endregion Methods
    }
    public class SolveForX
    {
        #region Properties

        List<ISolveForXStep> _solution = new List<ISolveForXStep>();

        public List<ISolveForXStep> Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        #endregion Properties

        #region Constructor

        public SolveForX(Equation equation)
        {
            this._solution = new List<ISolveForXStep>();
            this._solution.Add(equation);
            Solve();
        }

        #endregion Constructor

        #region Methods

        private void Solve()
        {
            AddSimplerStep();
            //if =0 try and move constant to the other side

            if (ThereAreDevisors() && CanGetExpressionToMultiplyEverythingBy())
            {
                MultiplyToCancelOut();
                //
            }

            if (ThereAreSidesWithMoreThanOneExpression())
            {
                do
                    AddExpressions(true);
                while (ThereAreSidesWithMoreThanOneExpression());
            }

            if (CanCrossMultiply() && !IsAssinmentSymbol(((Equation)this.Solution[Solution.Count - 1]).Split))
            {
                CrossMultiply();
            }

            //add like terms
            AddLikeTerms();

            if (NumberOfUnlikeTerms() > 2)
            {
                MoveToOneSide();
                AddLikeTerms();

                if (Factor())
                {
                    _solution.Add(new FactorEquations(new Equation((Equation)_solution[_solution.Count - 1])));
                }
                //if there are more two unlike variable terms

            }
            else
            {
                MoveToAppropriateSide();
                AddLikeTerms();
                CleanCoEfficient();
                CleanPower();
                //solution reached!
            }
        }
        private bool IsAssinmentSymbol(SignType type)
        {
            if (type == SignType.greater || type == SignType.greaterEqual
                || type == SignType.smaller || type == SignType.smallerEqual)
                return true;
            return false;
        }
        Calculator calc = new Calculator();

        private bool Factor()
        {
            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

            Expression expCurrent = new Expression((Expression)eqCurrent.Left[0]);

            if (expCurrent.Factorize())
            {
                eqCurrent.Left[0] = expCurrent;
                _solution.Add(new Equation(eqCurrent));
                return true;
            }
            return false;
        }
        private void CrossMultiply()
        {
            Expression left = null, right = null;
            CrossMultiplyAssist(ref left, ref right);

            Expression leftResult = new Expression();
            leftResult.AddToExpression(new Brace('('), true);
            leftResult.AddToExpression(true, left.Numerator);
            leftResult.AddToExpression(new Brace(')'), true);
            leftResult.AddToExpression(new Brace('('), true);
            leftResult.AddToExpression(true, right.Denominator);
            leftResult.AddToExpression(new Brace(')'), true);

            Expression RightResult = new Expression();
            RightResult.AddToExpression(new Brace('('), true);
            RightResult.AddToExpression(true, right.Numerator);
            RightResult.AddToExpression(new Brace(')'), true);
            RightResult.AddToExpression(new Brace('('), true);
            RightResult.AddToExpression(true, left.Denominator);
            RightResult.AddToExpression(new Brace(')'), true);

            Equation eq = new Equation(new iEquationPiece[] { new Expression(leftResult) },
                ((Equation)this.Solution[0]).Split, new iEquationPiece[] { new Expression(RightResult) });
            _solution.Add(eq);

            leftResult.RemoveBraces();
            RightResult.RemoveBraces();

            Equation eqAfter = new Equation(new iEquationPiece[] { new Expression(leftResult) },
                ((Equation)this.Solution[0]).Split, new iEquationPiece[] { new Expression(RightResult) });
            _solution.Add(eqAfter);
            //left.Numerator
        }
        private void CrossMultiplyAssist(ref Expression left, ref Expression right)
        {
            for (int i = 0; i < ((Equation)_solution[_solution.Count - 1]).Left.Count; i++)
                if (((Equation)_solution[_solution.Count - 1]).Left[i].GetEquationPieceType()
                    == EquationPieceType.Expression)
                {
                    left = (Expression)((Equation)_solution[_solution.Count - 1]).Left[i];
                    break;
                }

            for (int j = 0; j < ((Equation)_solution[_solution.Count - 1]).Right.Count; j++)
                if (((Equation)_solution[_solution.Count - 1]).Right[j].GetEquationPieceType()
                    == EquationPieceType.Expression)
                {
                    right = (Expression)((Equation)_solution[_solution.Count - 1]).Right[j];
                    break;
                }

            if (left.Denominator == null)
                left.Denominator = new List<IExpressionPiece>();

            if (left.Denominator.Count == 0)
                left.Denominator.Add(new Term(1));

            if (right.Denominator == null)
                right.Denominator = new List<IExpressionPiece>();

            if (right.Denominator.Count == 0)
                right.Denominator.Add(new Term(1));
        }
        private bool CanCrossMultiply()
        {
            int leftCount = 0, rightCount = 0;

            for (int i = 0; i < ((Equation)_solution[_solution.Count - 1]).Left.Count; i++)
                if (((Equation)_solution[_solution.Count - 1]).Left[i].GetEquationPieceType()
                    == EquationPieceType.Expression)
                    leftCount++;

            for (int j = 0; j < ((Equation)_solution[_solution.Count - 1]).Right.Count; j++)
                if (((Equation)_solution[_solution.Count - 1]).Right[j].GetEquationPieceType()
                    == EquationPieceType.Expression)
                    rightCount++;

            if (leftCount > 1 || rightCount > 1)
                return false;

            if (!AllDinominatorsAreOne())
                return true;
            else
                return false;
        }
        private bool AllDinominatorsAreOne()//can only be called by method CanCrossMultiply
        {
			if (_solution != null && ((Equation)_solution[_solution.Count - 1]).Left != null &&
				((Equation)_solution[_solution.Count - 1]).Left.Count > 0 &&
				((Expression)((Equation)_solution[_solution.Count - 1]).Left[0]).Denominator != null)
				for (int i = 0; i < ((Expression)((Equation)_solution[_solution.Count - 1]).Left[0]).Denominator.Count; i++)
				{
					if (!((Term)((Expression)((Equation)_solution[_solution.Count - 1]).Left[0]).Denominator[i]).Constant)
						return false;
					else if (((Term)((Expression)((Equation)_solution[_solution.Count - 1]).Left[0]).Denominator[i]).CoEfficient != 1)
						return false;
				}

			if (_solution != null && ((Equation)_solution[_solution.Count - 1]).Right  != null &&

				((Equation)_solution[_solution.Count - 1]).Right.Count > 0 &&
				((Expression)((Equation)_solution[_solution.Count - 1]).Right[0]).Denominator != null)
				for (int i = 0; i < ((Expression)((Equation)_solution[_solution.Count - 1]).Right[0]).Denominator.Count; i++)
				{
					if (!((Term)((Expression)((Equation)_solution[_solution.Count - 1]).Right[0]).Denominator[i]).Constant)
						return false;
					else if (((Term)((Expression)((Equation)_solution[_solution.Count - 1]).Right[0]).Denominator[i]).CoEfficient != 1)
						return false;
				}
            return true;
        }
        private bool ThereAreDevisors()
        {
            for (int i = 0; i < ((Equation)_solution[_solution.Count - 1]).Left.Count; i++)
                if (((Equation)_solution[_solution.Count - 1]).Left[i].GetEquationPieceType()
                    == EquationPieceType.Expression)
                    if (((Expression)((Equation)_solution[_solution.Count - 1]).Left[i]).Denominator != null
                        && ((Expression)((Equation)_solution[_solution.Count - 1]).Left[i]).Denominator.Count != 0)
                    {
                        Expression exp = new Expression((((Expression)((Equation)_solution[_solution.Count - 1]).Left[i])));
                        exp.RemoveBraces();
                        if (!(exp.Denominator.Count == 1 && (exp).Denominator[0].GetTypePiece() == ExpressionPieceType.Term
                            && ((Term)(exp).Denominator[0]).Constant && (((Term)(exp).Denominator[0]).CoEfficient == 1)))
                            return true;
                    }

            for (int j = 0; j < ((Equation)_solution[_solution.Count - 1]).Right.Count; j++)
                if (((Equation)_solution[_solution.Count - 1]).Right[j].GetEquationPieceType()
                    == EquationPieceType.Expression)
                    if (((Expression)((Equation)_solution[_solution.Count - 1]).Right[j]).Denominator != null
                        && ((Expression)((Equation)_solution[_solution.Count - 1]).Right[j]).Denominator.Count != 0)
                    {
                        Expression exp = new Expression((((Expression)((Equation)_solution[_solution.Count - 1]).Right[j])));
                        exp.RemoveBraces();
                        if (!(exp.Denominator.Count == 1 && (exp).Denominator[0].GetTypePiece() == ExpressionPieceType.Term
                            && ((Term)(exp).Denominator[0]).Constant && (((Term)(exp).Denominator[0]).CoEfficient == 1)))
                            return true;
                    }
            return false;
        }
        private List<List<Term>> GetAllExpressionDivisors()
        {
            RemoveBraces();
            List<List<Term>> terms = new List<List<Term>>();

            for (int i = 0; i < ((Equation)_solution[_solution.Count - 1]).Left.Count; i++)
                if (((Equation)_solution[_solution.Count - 1]).Left[i].GetEquationPieceType()
                    == EquationPieceType.Expression)
                    if (((Expression)((Equation)_solution[_solution.Count - 1]).Left[i]).Denominator != null)
                    {
                        List<Term> current = new List<Term>();
                        for (int k = 0; k < ((Expression)((Equation)_solution[_solution.Count - 1]).Left[i]).Denominator.Count; k++)
                            if (((Expression)((Equation)_solution[_solution.Count - 1]).Left[i]).Denominator[k].GetTypePiece() == ExpressionPieceType.Term)
                                current.Add((Term)((Expression)((Equation)_solution[_solution.Count - 1]).Left[i]).Denominator[k]);
                        if (current.Count > 0)
                            terms.Add(current);
                    }


            for (int j = 0; j < ((Equation)_solution[_solution.Count - 1]).Right.Count; j++)
                if (((Equation)_solution[_solution.Count - 1]).Right[j].GetEquationPieceType()
                    == EquationPieceType.Expression)
                    if (((Expression)((Equation)_solution[_solution.Count - 1]).Right[j]).Denominator != null)
                    {
                        List<Term> current = new List<Term>();
                        for (int k = 0; k < ((Expression)((Equation)_solution[_solution.Count - 1]).Right[j]).Denominator.Count; k++)
                            if (((Expression)((Equation)_solution[_solution.Count - 1]).Right[j]).Denominator[k].GetTypePiece() == ExpressionPieceType.Term)
                                current.Add((Term)((Expression)((Equation)_solution[_solution.Count - 1]).Right[j]).Denominator[k]);
                        if (current.Count > 0)
                            terms.Add(current);
                    }

            return terms;
        }
        private void MoveToOneSide()
        {
            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);

            if (!ExpressionOnlyContainsZero((Expression)((Equation)_solution[_solution.Count - 1]).Right[0]))
            {
                eqCurrent.Left.Add(new Sign("+"));
                eqCurrent.Left.Add(ChangeSigns((Expression)((Equation)_solution[_solution.Count - 1]).Right[0]));

                Expression expression = new Expression();
                expression.AddToExpression(true, new Term(0));
                eqCurrent.Right.Add(expression);

                //else eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

                eqCurrent = AddExpressions(eqCurrent);
                _solution.Add(new Equation(eqCurrent));
            }
            //eqCurrent.AddLikeTerms();}

            //return ((Expression)eqCurrent.Left[0]).Numerator.Count;

        }
        private bool ExpressionOnlyContainsZero(Expression expression)
        {
            if (expression.Numerator.Count > 1 || expression.Denominator.Count > 1)
                return false;
            if (expression.Numerator[0].GetTypePiece() != ExpressionPieceType.Term)
                return false;
            if (!((Term)expression.Numerator[0]).Constant)
                return false;
            if (((Term)expression.Numerator[0]).CoEfficient != 0)
                return false;
            return true;
        }
        private int NumberOfUnlikeTerms()
        {
            Equation eqNew = new Equation(((Equation)_solution[_solution.Count - 1]).Split);


            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
	        if (((Equation) _solution[_solution.Count - 1]).Right.Count > 0)
	        {
		        eqCurrent.Left.Add(new Sign("+"));
		        eqCurrent.Left.Add(ChangeSigns((Expression) ((Equation) _solution[_solution.Count - 1]).Right[0])); 
				eqCurrent = AddExpressions(eqCurrent);
	        }

	       
            eqCurrent.AddLikeTerms();

            return ((Expression)eqCurrent.Left[0]).Numerator.Count;

        }
        private bool AddLikeTerms()
        {
            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

            if (eqCurrent.AddLikeTerms())
            {
                _solution.Add(eqCurrent);
                return true;
            }
            return false;
        }
        private void MoveToAppropriateSide()//can only be called when current solution is in format expression = expression
        {
            bool moved = false;
            Equation eqNew = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            Expression leftResult = new Expression();
            Expression rightResult = new Expression();

            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

            eqCurrent.RemoveBraces();

            #region Work

            for (int i = 0; i < ((Expression)eqCurrent.Left[0]).Numerator.Count; i++)
            {
                if (((Term)((Expression)eqCurrent.Left[0]).Numerator[i]).Constant)
                {
                    rightResult.AddToExpression(true, ((Term)calc.Calculate(new Term(1, true),
                        ((Term)((Expression)eqCurrent.Left[0]).Numerator[i]), MathFunction.Mutliply)));
                    moved = true;
                }
                else
                {
                    leftResult.AddToExpression(true, ((Term)((Expression)eqCurrent.Left[0]).Numerator[i]));
                }
            }
			if (eqCurrent.Right != null && eqCurrent.Right.Count > 0)
				for (int i = 0; i < ((Expression)eqCurrent.Right[0]).Numerator.Count; i++)
				{
					if (((Term)((Expression)eqCurrent.Right[0]).Numerator[i]).Constant != true)
					{
						leftResult.AddToExpression(true, ((Term)calc.Calculate(new Term(1, true),
							((Term)((Expression)eqCurrent.Right[0]).Numerator[i]), MathFunction.Mutliply)));
						moved = true;
					}
					else
					{
						if (((Term)((Expression)eqCurrent.Right[0]).Numerator[i]).CoEfficient != 0)
						{
							rightResult.AddToExpression(true, ((Term)((Expression)eqCurrent.Right[0]).Numerator[i]));
						}
						else if (rightResult.Numerator.Count == 0)
							rightResult.AddToExpression(true, ((Term)((Expression)eqCurrent.Right[0]).Numerator[i]));
					}
				}
            #endregion Work

            eqNew.Left.Add(leftResult);
            eqNew.Right.Add(rightResult);

            if (moved)
                _solution.Add(new Equation(eqNew));
        }
        private void CleanCoEfficient()//can only be called when current solution is in format expression(one term) = expression(one term)
        {
            bool needToDivide = false;
            Term divisor = null;
            Equation eqNew = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            Expression leftResult = new Expression();
            Expression rightResult = new Expression();

            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

            eqCurrent.RemoveBraces();

            #region Work


            if (((Term)((Expression)eqCurrent.Left[0]).Numerator[0]).CoEfficient != 1
                    || ((Term)((Expression)eqCurrent.Left[0]).Numerator[0]).Sign == "-")
            {
                divisor = new Term(((Term)((Expression)eqCurrent.Left[0]).Numerator[0]).CoEfficient);
                divisor.Sign = ((Term)((Expression)eqCurrent.Left[0]).Numerator[0]).Sign;

                leftResult.AddToExpression(true, (Term)calc.Calculate(((Term)((Expression)eqCurrent.Left[0]).Numerator[0]),
                    divisor, MathFunction.Divide));
                needToDivide = true;
            }
            else
            {
                leftResult.AddToExpression(true, ((Term)((Expression)eqCurrent.Left[0]).Numerator[0]));
            }



            if (needToDivide)
            {
                rightResult.AddToExpression(true, (Term)calc.Calculate(((Term)((Expression)eqCurrent.Right[0]).Numerator[0]),
                    divisor, MathFunction.Divide));
            }
            else
            {
			 	if(eqCurrent.Right != null && eqCurrent.Right.Count > 0)
					rightResult.AddToExpression(true, ((Term)((Expression)eqCurrent.Right[0]).Numerator[0]));
            }

            #endregion Work

            #region show step

            if (needToDivide)
            {
                Equation eqNewShow = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
                Expression leftResultShow = new Expression();
                Expression rightResultShow = new Expression();

                eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
                eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

                leftResultShow.AddToExpression(true, (Term)((Expression)eqCurrent.Left[0]).Numerator[0]);
                leftResultShow.AddToExpression(false, divisor);

                rightResultShow.AddToExpression(true, (Term)((Expression)eqCurrent.Right[0]).Numerator[0]);
                rightResultShow.AddToExpression(false, divisor);

                eqNewShow.Left.Add(leftResultShow);
                eqNewShow.Right.Add(rightResultShow);

                _solution.Add(eqNewShow);
            }

            #endregion show step

            eqNew.Left.Add(leftResult);
            eqNew.Right.Add(rightResult);

            if (needToDivide)
            {
                if (divisor.Sign == "-")
                {
                    eqNew.Split = FlipSplit(eqNew.Split);
                }
                _solution.Add(new Equation(eqNew));
            }
        }
        private SignType FlipSplit(SignType current)
        {
            if (current == SignType.greater)
                return SignType.smaller;
            else if (current == SignType.smaller)
                return SignType.greater;
            else if (current == SignType.greaterEqual)
                return SignType.smallerEqual;
            else if (current == SignType.smallerEqual)
                return SignType.greaterEqual;
            else
                return current;
        }
        private void CleanPower()//can only be called when current solution is in format expression(one term) = expression(one term)
        {
            bool needToRoot = false;
            Equation eqNew = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            Expression leftResult = new Expression();
            Expression rightResult = new Expression();

            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);
            eqCurrent.RemoveBraces();

            #region Work


            if (((Term)((Expression)eqCurrent.Left[0]).Numerator[0]).Power == 2)
            {
                Term current = new Term((Term)((Expression)eqCurrent.Left[0]).Numerator[0]);
                if (current.Sign != "-")
                    current.Power = 1;
                leftResult.AddToExpression(true, current);
                needToRoot = true;
            }
            else
            {
                leftResult.AddToExpression(true, new Term(((Term)((Expression)eqCurrent.Left[0]).Numerator[0])));
            }



            if (needToRoot)
            {
				if(eqCurrent.Right == null || eqCurrent.Right.Count == 0)return;

                Term current = new Term((Term)((Expression)eqCurrent.Right[0]).Numerator[0]);

                if (current.Power == 2 && current.Sign != "-")
                    current.Power = 1;
                else if (IsPerfectSquare(current.CoEfficient) && current.Sign != "-")
                {
                    current.CoEfficient = (int)Math.Sqrt(current.CoEfficient);
                    current.TwoSigns = true;
                }
                else
                    current.Root = 2;

                rightResult.AddToExpression(true, current);

            }
            else
            {
                rightResult.AddToExpression(true, ((Term)((Expression)eqCurrent.Right[0]).Numerator[0]));
            }

            #endregion Work

            #region show step

            if (needToRoot)
            {
                Equation eqNewShow = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
                Expression leftResultShow = new Expression();
                Expression rightResultShow = new Expression();

                eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
                eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

                Term left = new Term((Term)((Expression)eqCurrent.Left[0]).Numerator[0]);
                left.Root = 2;
                leftResultShow.AddToExpression(true, left);

                Term right = new Term((Term)((Expression)eqCurrent.Right[0]).Numerator[0]);
                right.Root = 2;
                rightResultShow.AddToExpression(true, right);


                eqNewShow.Left.Add(leftResultShow);
                eqNewShow.Right.Add(rightResultShow);

                _solution.Add(eqNewShow);
            }

            #endregion show step

            eqNew.Left.Add(leftResult);
            eqNew.Right.Add(rightResult);

            if (needToRoot)
                _solution.Add(new Equation(eqNew));
        }
        private bool IsPerfectSquare(int num)
        {
            int root = (int)Math.Sqrt(num);
            return (int)Math.Pow(root, 2) == num;
        }
        private Expression ChangeSigns(Expression expOld)//to be used by NumberOfUnlikeTerms only
        {
            expOld.RemoveBraces();
            Expression expNew = new Expression();

            for (int i = 0; i < expOld.Numerator.Count; i++)
            {
                expNew.AddToExpression(true, (Term)calc.Calculate((Term)expOld.Numerator[i], new Term(1, true), MathFunction.Mutliply));
            }
            for (int i = 0; i < expOld.Denominator.Count; i++)
            {
                expNew.AddToExpression(false, (Term)calc.Calculate((Term)expOld.Denominator[i], new Term(1, true), MathFunction.Mutliply));
            }
            return expNew;
        }
        private void AddExpressions(bool addToSolution)
        {
            bool AddedSomething = false;
            Equation eqNew = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

            for (int i = 0; i < eqCurrent.Left.Count; i++)
            {
                if (i + 1 >= eqCurrent.Left.Count || eqCurrent.Left[i + 1].GetEquationPieceType() != EquationPieceType.Sign || AddedSomething)
                    eqNew.Left.Add(eqCurrent.Left[i]);
                else
                {

                    eqNew.Left.Add((Expression)calc.Calculate((Expression)eqCurrent.Left[i],
                        (Expression)eqCurrent.Left[i + 2], function((Sign)eqCurrent.Left[i + 1])));
                    i += 2;
                    AddedSomething = true;
                }
            }

            for (int i = 0; i < eqCurrent.Right.Count; i++)
            {
                if (i + 1 >= eqCurrent.Right.Count || eqCurrent.Right[i + 1].GetEquationPieceType() != EquationPieceType.Sign || AddedSomething)
                    eqNew.Right.Add(eqCurrent.Right[i]);
                else
                {

                    eqNew.Right.Add((Expression)calc.Calculate((Expression)eqCurrent.Right[i],
                        (Expression)eqCurrent.Right[i + 2], function((Sign)eqCurrent.Right[i + 1])));
                    i += 2;
                    AddedSomething = true;
                }
            }

            if (addToSolution)
            {
                _solution.Add(eqNew);
                RemoveBraces();
            }
        }
        private Equation AddExpressions(Equation eq)
        {
            Equation eqNew = new Equation(eq.Split);
            Equation eqCurrent = new Equation(eq.Split);
            eqCurrent.Left = new List<iEquationPiece>(eq.Left);
            eqCurrent.Right = new List<iEquationPiece>(eq.Right);

            for (int i = 0; i < eqCurrent.Left.Count; i++)
            {
                if (i + 1 >= eqCurrent.Left.Count || eqCurrent.Left[i + 1].GetEquationPieceType() != EquationPieceType.Sign)
                    eqNew.Left.Add(eqCurrent.Left[i]);
                else
                {

                    eqNew.Left.Add((Expression)calc.Calculate((Expression)eqCurrent.Left[i],
                        (Expression)eqCurrent.Left[i + 2], function((Sign)eqCurrent.Left[i + 1])));
                    i += 2;
                }
            }

            for (int i = 0; i < eqCurrent.Right.Count; i++)
            {
                if (i + 1 >= eqCurrent.Right.Count || eqCurrent.Right[i + 1].GetEquationPieceType() != EquationPieceType.Sign)
                    eqNew.Right.Add(eqCurrent.Right[i]);
                else
                {

                    eqNew.Right.Add((Expression)calc.Calculate((Expression)eqCurrent.Right[i],
                        (Expression)eqCurrent.Right[i + 2], function((Sign)eqCurrent.Right[i + 1])));
                    i += 2;
                }
            }

            return eqNew;
        }
        private MathFunction function(Sign sign)
        {
            if (sign.SignType == SignType.Add)
                return MathFunction.Add;
            else if ((sign.SignType == SignType.Subtract))
                return MathFunction.Subtract;
            else if ((sign.SignType == SignType.Divide))
                return MathFunction.Divide;
            else if ((sign.SignType == SignType.Multiply))
                return MathFunction.Mutliply;
            return MathFunction.None;
        }
        private bool ThereAreSidesWithMoreThanOneExpression()
        {
            RemoveBraces();
            List<List<Term>> terms = new List<List<Term>>();

            int currentCountLeft = 0;
            for (int i = 0; i < ((Equation)_solution[_solution.Count - 1]).Left.Count; i++)
                if (((Equation)_solution[_solution.Count - 1]).Left[i].GetEquationPieceType()
                    == EquationPieceType.Expression)
                {
                    currentCountLeft++;
                    if (currentCountLeft > 1)
                        return true;
                }


            int currentCountRight = 0;
            for (int i = 0; i < ((Equation)_solution[_solution.Count - 1]).Right.Count; i++)
                if (((Equation)_solution[_solution.Count - 1]).Right[i].GetEquationPieceType()
                    == EquationPieceType.Expression)
                {
                    currentCountRight++;
                    if (currentCountRight > 1)
                        return true;
                }

            return false;
        }
        private void MultiplyToCancelOut()
        {
            RemoveBraces();
            Equation old = new Equation((Equation)this._solution[this._solution.Count - 1]);
            Equation _new = new Equation(old.Split);
            List<Term> multiplier = GetExpressionToMultiplyEverythingBy();
            Expression multiplierExp = CreateExpression(multiplier, new List<Term>(new Term[] { new Term(1) }));

            #region Multiplied by same thing
            if (multiplier != null)
            {
                for (int i = 0; i < old.Left.Count; i++)
                {
                    _new.Left.Add(old.Left[i]);
                    if (old.Left[i].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        _new.Left.Add(new Sign("*"));
                        _new.Left.Add(new Expression(multiplierExp));
                    }
                }

                for (int k = 0; k < old.Right.Count; k++)
                {
                    _new.Right.Add(old.Right[k]);
                    if (old.Right[k].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        _new.Right.Add(new Sign("*"));
                        _new.Right.Add(new Expression(multiplierExp));
                    }
                }
            }
            this._solution.Add(new Equation(_new));
            #endregion Multiplied by same thing

            #region Expand to Demo Cancellation

            _new = new Equation(old.Split);
            LongDivisionSolution lDivSol = null;
            if (multiplier != null)
            {
                for (int i = 0; i < old.Left.Count; i++)
                {
                    _new.Left.Add(old.Left[i]);
                    if (old.Left[i].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        Expression currentMultiplier = new Expression(multiplierExp);

                        lDivSol = new LongDivisionSolution(GetTermsFromExpressList(((Expression)old.Left[i]).Denominator),
                            GetTermsFromExpressList(multiplierExp.Numerator));

                        _new.Left.Add(new Sign("*"));
                        if (!lDivSol.Failed && !lDivSol.Remainder && !ContainsOnlyOne(lDivSol.B))
                        {
                            currentMultiplier = new Expression();
                            currentMultiplier.AddToExpression(true, new Brace('('));
                            currentMultiplier.AddToExpression(true, lDivSol.A.ToArray());
                            currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(true, new Brace('('));
                            currentMultiplier.AddToExpression(true, lDivSol.B.ToArray());
                            currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(false, new Term(1));//over one
                        }
                        _new.Left.Add(new Expression(currentMultiplier));
                    }
                }

                for (int k = 0; k < old.Right.Count; k++)
                {
                    _new.Right.Add(old.Right[k]);
                    if (old.Right[k].GetEquationPieceType() == EquationPieceType.Expression)
                    {
                        Expression currentMultiplier = new Expression(multiplierExp);

                        lDivSol = new LongDivisionSolution(GetTermsFromExpressList(((Expression)old.Right[k]).Denominator),
                            GetTermsFromExpressList(multiplierExp.Numerator));

                        _new.Right.Add(new Sign("*"));
                        if (!lDivSol.Failed && !lDivSol.Remainder && !ContainsOnlyOne(lDivSol.B))
                        {
                            currentMultiplier = new Expression();
                            currentMultiplier.AddToExpression(true, new Brace('('));
                            currentMultiplier.AddToExpression(true, lDivSol.A.ToArray());
                            currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(true, new Brace('('));
                            currentMultiplier.AddToExpression(true, lDivSol.B.ToArray());
                            currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(false, new Term(1));//over one
                        }
                        _new.Right.Add(new Expression(currentMultiplier));
                    }
                }
            }
            this._solution.Add(new Equation(_new));

            #endregion Expand to Demo Cancellation

            #region Expand to Demo Cancellation 2

            _new = new Equation(old.Split);

            if (multiplier != null)
            {
                for (int i = 0; i < old.Left.Count; i++)
                {
                    #region Left
                    if (old.Left[i].GetEquationPieceType() != EquationPieceType.Expression)
                        _new.Left.Add(old.Left[i]);
                    else
                    {
                        Expression existing = new Expression((Expression)old.Left[i]);
                        existing.Denominator = new List<IExpressionPiece>(new IExpressionPiece[] { new Term(1) });
                        _new.Left.Add(existing);

                        Expression currentMultiplier = new Expression(multiplierExp);

                        lDivSol = new LongDivisionSolution(GetTermsFromExpressList(((Expression)old.Left[i]).Denominator),
                            GetTermsFromExpressList(multiplierExp.Numerator));

                        _new.Left.Add(new Sign("*"));
                        if (!lDivSol.Failed && !lDivSol.Remainder && !ContainsOnlyOne(lDivSol.B))
                        {
                            currentMultiplier = new Expression();
                            //currentMultiplier.AddToExpression(true, new Brace('('));
                            //currentMultiplier.AddToExpression(true, lDivSol.A.ToArray());
                            //currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(true, new Brace('('));
                            currentMultiplier.AddToExpression(true, lDivSol.B.ToArray());
                            currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(false, new Term(1));//over one
                        }
                        _new.Left.Add(new Expression(currentMultiplier));
                    }
                    #endregion Left
                }

                for (int k = 0; k < old.Right.Count; k++)
                {
                    #region Right

                    if (old.Right[k].GetEquationPieceType() != EquationPieceType.Expression)
                        _new.Left.Add(old.Right[k]);
                    else
                    {
                        Expression existing = new Expression((Expression)old.Right[k]);
                        existing.Denominator = new List<IExpressionPiece>(new IExpressionPiece[] { new Term(1) });
                        _new.Left.Add(existing);

                        Expression currentMultiplier = new Expression(multiplierExp);

                        lDivSol = new LongDivisionSolution(GetTermsFromExpressList(((Expression)old.Right[k]).Denominator),
                            GetTermsFromExpressList(multiplierExp.Numerator));

                        _new.Left.Add(new Sign("*"));
                        if (!lDivSol.Failed && !lDivSol.Remainder && !ContainsOnlyOne(lDivSol.B))
                        {
                            currentMultiplier = new Expression();
                            //currentMultiplier.AddToExpression(true, new Brace('('));
                            //currentMultiplier.AddToExpression(true, lDivSol.A.ToArray());
                            //currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(true, new Brace('('));
                            currentMultiplier.AddToExpression(true, lDivSol.B.ToArray());
                            currentMultiplier.AddToExpression(true, new Brace(')'));
                            currentMultiplier.AddToExpression(false, new Term(1));//over one
                        }
                        _new.Right.Add(new Expression(currentMultiplier));
                    }

                    #endregion Right
                }
            }
            //this._solution.Add(new Equation(_new));

            #endregion Expand to Demo Cancellation 2

            #region Carry out Cancellation

            _new = new Equation(old.Split);

            if (multiplier != null)
            {
                for (int i = 0; i < old.Left.Count; i++)
                {
                    #region Left
                    if (old.Left[i].GetEquationPieceType() != EquationPieceType.Expression)
                        _new.Left.Add(old.Left[i]);
                    else
                    {
                        Expression existing = new Expression((Expression)old.Left[i]);
                        existing.Denominator = new List<IExpressionPiece>(new IExpressionPiece[] { new Term(1) });

                        lDivSol = new LongDivisionSolution(GetTermsFromExpressList(((Expression)old.Left[i]).Denominator),
                            GetTermsFromExpressList(multiplierExp.Numerator));

                        if (!lDivSol.Failed && !lDivSol.Remainder && !ContainsOnlyOne(lDivSol.B))
                        {
                            List<IExpressionPiece> top = new List<IExpressionPiece>(existing.Numerator);

                            existing.Numerator = new List<IExpressionPiece>();
                            existing.AddToExpression(true, new Brace('('));
                            existing.AddToExpression(true, top.ToArray());
                            existing.AddToExpression(true, new Brace(')'));

                            existing.AddToExpression(true, new Brace('('));
                            existing.AddToExpression(true, lDivSol.B.ToArray());
                            existing.AddToExpression(true, new Brace(')'));
                            //existing.AddToExpression(false, new Term(1));//over one
                        }
                        //existing.RemoveBraces();
                        _new.Left.Add(existing);
                    }
                    #endregion Left
                }

                for (int k = 0; k < old.Right.Count; k++)
                {
                    #region Right

                    if (old.Right[k].GetEquationPieceType() != EquationPieceType.Expression)
                        _new.Left.Add(old.Right[k]);
                    else
                    {
                        Expression existing = new Expression((Expression)old.Right[k]);
                        existing.Denominator = new List<IExpressionPiece>(new IExpressionPiece[] { new Term(1) });

                        lDivSol = new LongDivisionSolution(GetTermsFromExpressList(((Expression)old.Right[k]).Denominator),
                            GetTermsFromExpressList(multiplierExp.Numerator));

                        if (!lDivSol.Failed && !lDivSol.Remainder && !ContainsOnlyOne(lDivSol.B))
                        {
                            List<IExpressionPiece> top = new List<IExpressionPiece>(existing.Numerator);

                            existing.Numerator = new List<IExpressionPiece>();
                            existing.AddToExpression(true, new Brace('('));
                            existing.AddToExpression(true, top.ToArray());
                            existing.AddToExpression(true, new Brace(')'));

                            existing.AddToExpression(true, new Brace('('));
                            existing.AddToExpression(true, lDivSol.B.ToArray());
                            existing.AddToExpression(true, new Brace(')'));
                            //existing.AddToExpression(false, new Term(1));//over one
                        }
                        //existing.RemoveBraces();
                        _new.Right.Add(existing);
                    }

                    #endregion Right
                }
            }
            this._solution.Add(new Equation(_new));

            #endregion Carry out Cancellation

            #region Clean

            RemoveBraces();

            #endregion Clean
        }
        private bool RemoveBraces()//add to list of things done or sum
        {
            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]).Split);
            eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);

            if (eqCurrent.RemoveBraces())
            {
                _solution.Add(new Equation(eqCurrent));
                return true;
            }
            else return false;
        }
        private bool ContainsOnlyOne(List<Term> terms)//Checks if the list contains anything that is not one.
        {
            for (int i = 0; i < terms.Count; i++)
            {
                if (!terms[i].Constant)
                    return false;
                else
                {
                    if (terms[i].CoEfficient != 1)
                        return false;
                }
            }
            return true;
        }
        private List<Term> GetTermsFromExpressList(List<IExpressionPiece> whole)
        {
            List<Term> answer = new List<Term>();
            for (int i = 0; i < whole.Count; i++)
            {
                if (whole[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    answer.Add((Term)whole[i]);
                }
                else throw new Exception("Has Expression has braces/signs");
            }
            return answer;
        }
        private Expression CreateExpression(List<Term> top, List<Term> div)
        {
            Expression result = new Expression();
            result.AddToExpression(true, top.ToArray());

            if (div.Count != 0)
                result.AddToExpression(false, div.ToArray());

            return result;
        }
        private List<Term> GetExpressionToMultiplyEverythingBy()
        {
            //remove braces on all expression deno
            RemoveBraces();
            List<List<Term>> deno = GetAllExpressionDivisors();

            for (int i = 0; i < deno.Count; i++)
            {
                if (CanBeDivedBy(i, deno))
                    return deno[i];
            }

            Expression exp = new Expression();
            for (int i = 0; i < deno.Count; i++)
            {
                exp.AddToExpression(true, new Brace('('));
                exp.AddToExpression(true, deno[i].ToArray());
                exp.AddToExpression(true, new Brace('('));
            }

            exp.RemoveBraces();

            return GetTermsFromExpressList(exp.Numerator);

        }
        private bool CanGetExpressionToMultiplyEverythingBy()
        {
            List<List<Term>> deno = GetAllExpressionDivisors();

            for (int i = 0; i < deno.Count; i++)
            {
                if (CanBeDivedBy(i, deno))
                    return true;
            }
            return false;
        }
        private bool CanBeDivedBy(int value, List<List<Term>> deno)
        {
            for (int i = 0; i < deno.Count; i++)
                if (i != value)
                {
                    LongDivisionSolution sol = new LongDivisionSolution(deno[i], deno[value]);
                    if (sol.Failed || sol.Remainder)
                        return false;
                }
            return true;
        }
        private void AddSimplerStep()
        {
            Equation eqCurrent = new Equation(((Equation)_solution[_solution.Count - 1]));
            //eqCurrent.Left = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Left);
            //eqCurrent.Right = new List<iEquationPiece>(((Equation)_solution[_solution.Count - 1]).Right);
            if (eqCurrent.Simplify())
                this._solution.Add(new Equation(eqCurrent));
        }

        #endregion Methods
    }
    public interface iEquationPiece
    {
        EquationPieceType GetEquationPieceType();
    }
    public enum SelectedPieceType
    {
        Base, Coefficient, Power, Sign, brace, OutSide,
        Empty /*Next to the term*/
            , BaseExpo, PowerExpo
    }
    public class SequenceTerm : Term, IExpressionPiece
    {
        #region Properties

        string _number;

        public string Number
        {
            get { return _number; }
            set { _number = value; }
        }

        #endregion Properties

        #region Constructor

        public SequenceTerm(Term term)
            : base(term)
        {
            this._number = "n";
        }
        public SequenceTerm(Term term, int num)
            : base(term)
        {
            this._number = num.ToString();
        }
        public SequenceTerm(SequenceTerm term)
            : base((Term)term)
        {
            this._number = term.Number;
        }

        #endregion Constructor

        #region Methods

        ExpressionPieceType IExpressionPiece.GetTypePiece()
        {
            return ExpressionPieceType.SequenceTerm;
        }

        #endregion Methods


    }
    public class Term : IComparable, IAlgebraPiece, IExpressionPiece, ITrigTerm, ITrigExpressionPiece
    {
        #region Properties

        bool joke = false;

        public bool Joke
        {
            get { return joke; }
            set { joke = value; }
        }

        bool expressJoke = false;

        public bool ExpressJoke
        {
            get { return expressJoke; }
            set { expressJoke = value; }
        }

        int mySelectedindex = 0;

        public int MySelectedindex
        {
            get { return mySelectedindex; }
            set { mySelectedindex = value; }
        }
        SelectedPieceType _mySelectedPieceType;

        public SelectedPieceType MySelectedPieceType
        {
            get { return _mySelectedPieceType; }
            set { _mySelectedPieceType = value; }
        }

        bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        bool sorted = false;

        public bool Sorted
        {
            get { return sorted; }
            set { sorted = value; }
        }

        int _coEfficient = 1;

        public int CoEfficient
        {
            get { return _coEfficient; }
            set { _coEfficient = value; }
        }
        char _termBase;

        public char TermBase
        {
            get { return _termBase; }
            set { _termBase = value; }
        }
        int _power = 1;

        public int Power
        {
            get { return _power; }
            set { _power = value; }
        }
        List<Term> _multipledBy = new List<Term>();

        public List<Term> MultipledBy
        {
            get { return _multipledBy; }
            set { _multipledBy = value; }
        }
        Term _devisor;

        public Term Devisor
        {
            get { return _devisor; }
            set { _devisor = value; }
        }

        string _sign;

        public string Sign
        {
            get { return _sign; }
            set { _sign = value; }
        }

        string _powerSign = "+";

        public string PowerSign
        {
            get { return _powerSign; }
            set { _powerSign = value; }
        }

        int _root = 1;

        public int Root
        {
            get { return _root; }
            set { _root = value; }
        }

        bool _twoSigns = false;

        public bool TwoSigns
        {
            get { return _twoSigns; }
            set { _twoSigns = value; }
        }

        bool _constant = false;

        public bool Constant
        {
            get { return _constant; }
            set { _constant = value; }
        }

        bool cancelledOut = false;

        public bool CancelledOut
        {
            get { return cancelledOut; }
            set { cancelledOut = value; }
        }

        #endregion Propeties

        #region Constructor
        public Term()
        {
        }
        public Term(bool cancelledOut)
        {
            this.cancelledOut = cancelledOut;
        }
        #region Constant


        public Term(int number)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Sign = (number < 0) ? "-" : "+";
        }
        public Term(int number, bool neg)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Sign = (neg) ? "-" : "+";
        }
        public Term(int number, int power)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (number < 0) ? "-" : "+";
        }
        public Term(int number, int power, bool neg)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(int number, int power, bool neg, bool negPower)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(int number, bool neg, int root)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
        }
        public Term(int number, int power, int root)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (number < 0) ? "-" : "+";
            this.Root = Math.Abs(root);
        }
        public Term(int number, int power, bool neg, int root)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
        }

        public Term(int number, int power, bool neg, bool negPower, int root)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
        }
        public Term(int number, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Sign = (number < 0) ? "-" : "+";
            this.MultipledBy = multiplier;
        }
        public Term(int number, bool neg, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Sign = (neg) ? "-" : "+";
            this.MultipledBy = multiplier;
        }
        public Term(int number, int power, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (number < 0) ? "-" : "+";
            this.MultipledBy = multiplier;
        }
        public Term(int number, int power, bool neg, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.MultipledBy = multiplier;
        }

        public Term(int number, int power, bool neg, bool negPower, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.MultipledBy = multiplier;
        }

        public Term(int number, bool neg, int root, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }
        public Term(int number, int power, int root, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (number < 0) ? "-" : "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }
        public Term(int number, int power, bool neg, int root, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }

        public Term(int number, int power, bool neg, bool negPower, int root, List<Term> multiplier)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }
        public Term(int number, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Devisor = divisor;
            this.Sign = (number < 0) ? "-" : "+";
        }
        public Term(int number, bool neg, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Devisor = divisor;
            this.Sign = (neg) ? "-" : "+";
        }
        public Term(int number, int power, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Devisor = divisor;
            this.Sign = (number < 0) ? "-" : "+";
        }
        public Term(int number, int power, bool neg, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Devisor = divisor;
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(int number, int power, bool neg, bool negPower, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Devisor = divisor;
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(int number, bool neg, int root, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Sign = (neg) ? "-" : "+";
            this.Devisor = divisor;
            this.Root = Math.Abs(root);
        }
        public Term(int number, int power, int root, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (number < 0) ? "-" : "+";
            this.Devisor = divisor;
            this.Root = Math.Abs(root);
        }
        public Term(int number, int power, bool neg, int root, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.Devisor = divisor;
            this.Root = Math.Abs(root);
        }

        public Term(int number, int power, bool neg, bool negPower, int root, Term divisor)
        {
            this.Constant = true;
            this.CoEfficient = Math.Abs(number);
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.Devisor = divisor;
            this.Root = Math.Abs(root);
        }



        #endregion Constant

        #region Term

        public Term(Term term)
        {
            if (term != null)
            {
                this._termBase = term.TermBase;
                this.Constant = term.Constant;
                this.cancelledOut = term.cancelledOut;
                this.CoEfficient = term.CoEfficient;
                this.Sign = term.Sign;
                this.Devisor = term.Devisor;
                this.MultipledBy = term.MultipledBy;
                this.Power = term.Power;
                this.PowerSign = term.PowerSign;
                this.Root = term.Root;
                this.Sorted = term.Sorted;
                this.TwoSigns = term.TwoSigns;
            }

        }

        public Term(char termBase)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Sign = "+";
        }
        public Term(char termBase, bool neg)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Sign = (neg) ? "-" : "+";
        }
        public Term(char termBase, int power)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = "+";
        }
        public Term(char termBase, int coEff, int power, params Term[] Multipliers)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = "+";

            MultipledBy.AddRange(Multipliers);
        }
        public Term(char termBase, int coEff, int power)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.CoEfficient = coEff;
            this.Power = Math.Abs(power);
            this.Sign = "+";
        }
        public Term(char termBase, int coEff, int power, bool neg)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.CoEfficient = coEff;
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
        }
        public Term(char termBase, int power, bool neg)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(char termBase, int power, bool neg, bool negPower)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(char termBase, bool neg, int root)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
        }
        public Term(char termBase, int power, bool hasRoot, int root)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = "+";
            this.Root = Math.Abs(root);
        }
        public Term(char termBase, int power, bool neg, bool hasRoot, int root)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
        }

        public Term(char termBase, int power, bool neg, bool negPower, bool hasRoot, int root)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
        }
        public Term(char termBase, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Sign = "+";
            this.MultipledBy = multiplier;
        }
        public Term(char termBase, bool neg, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Sign = (neg) ? "-" : "+";
            this.MultipledBy = multiplier;
        }
        public Term(char termBase, int power, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = "+";
            this.MultipledBy = multiplier;
        }
        public Term(char termBase, int power, bool neg, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.MultipledBy = multiplier;
        }

        public Term(char termBase, int power, bool neg, bool negPower, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.MultipledBy = multiplier;
        }

        public Term(char termBase, bool neg, int root, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }
        public Term(char termBase, int power, int root, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }
        public Term(char termBase, int power, bool neg, int root, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }

        public Term(char termBase, int power, bool neg, bool negPower, int root, List<Term> multiplier)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.Root = Math.Abs(root);
            this.MultipledBy = multiplier;
        }
        public Term(char termBase, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Devisor = divisor;
            this.Sign = "+";
        }
        public Term(char termBase, bool neg, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Devisor = divisor;
            this.Sign = (neg) ? "-" : "+";
        }
        public Term(char termBase, int power, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Devisor = divisor;
            this.Power = Math.Abs(power);
            this.Sign = "+";
        }
        public Term(char termBase, int power, bool neg, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Devisor = divisor;
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(char termBase, int power, bool neg, bool negPower, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Devisor = divisor;
            this.Sign = (neg) ? "-" : "+";
        }

        public Term(char termBase, bool neg, int root, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Sign = (neg) ? "-" : "+";
            this.Devisor = divisor;
            this.Root = Math.Abs(root);
        }

        public Term(char termBase, int power, bool neg, int root, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.Sign = (neg) ? "-" : "+";
            this.Devisor = divisor;
            this.Root = Math.Abs(root);
        }

        public Term(char termBase, int power, bool neg, bool negPower, int root, Term divisor)
        {
            this.Constant = false;
            this.TermBase = termBase;
            this.Power = Math.Abs(power);
            this.PowerSign = (negPower) ? "-" : "+";
            this.Sign = (neg) ? "-" : "+";
            this.Devisor = divisor;
            this.Root = Math.Abs(root);
        }

        #endregion Term

        #endregion Constructor

        #region Methods

        public int CompareTo(object obj)
        {

            Term two = (Term)obj;

            if (!this.Constant && !two.Constant)
            {
                if (this.TermBase > two.TermBase)
                    return 1;
                else if (this.TermBase == two.TermBase)
                {
                    if (this.Power > two.Power)
                        return -1;
                    else
                        return 1;
                }
                return -1;
            }
            else if (this.Constant && !two.Constant)
                return 1;
            else if (!this.Constant && two.Constant)
                return -1;
            return 0;

        }
        public bool AreEqual(Term term)
        {
            if (this._termBase != term._termBase ||
            this.Constant != term.Constant ||
            this.cancelledOut != term.cancelledOut ||
            this.CoEfficient != term.CoEfficient ||
            this.Sign != term.Sign ||
            this.Power != term.Power ||
            this.PowerSign != term.PowerSign ||
            this.Root != term.Root ||
            this.TwoSigns != term.TwoSigns)
                return false;

            if (!(MutipliedByCancelOut(this.MultipledBy, term.MultipledBy)))
                return false;
            if (((this.MultipledBy == null) ? 0 : this.MultipledBy.Count) != ((term.MultipledBy == null) ? 0 : term.MultipledBy.Count))
                return false;

            if (this.MultipledBy != null)
            {
                for (int i = 0; i < this.MultipledBy.Count; i++)
                {
                    if (i >= term.MultipledBy.Count)
                        return false;
                    else if (!this.MultipledBy[i].AreEqual(term.MultipledBy[i]))
                        return false;
                }
            }

            if ((this.Devisor == null && term.Devisor != null) ||
                (term.Devisor == null && this.Devisor != null))
                return false;

            if (this.Devisor != null && !this.Devisor.AreEqual(term.Devisor))
                return false;

            return true;
        }
        public bool MutipliedByCancelOut(List<Term> one, List<Term> two)
        {
            if (one == null && (two != null && two.Count > 0))
                return false;
            if (two == null && (one != null && one.Count > 0))
                return false;
            return true;
        }
        public bool CancelOut(Term term)
        {
            if (this._termBase != term._termBase ||
            this.Constant != term.Constant ||
            this.cancelledOut != term.cancelledOut ||
            this.CoEfficient != term.CoEfficient ||
            this.Sign == term.Sign || //meed to be unlike to cancelout
            this.Power != term.Power ||
            this.PowerSign != term.PowerSign ||
            this.Root != term.Root ||
            this.TwoSigns != term.TwoSigns)
                return false;

            if (!(MutipliedByCancelOut(this.MultipledBy, term.MultipledBy)))
                return false;
            if (((this.MultipledBy == null) ? 0 : this.MultipledBy.Count) != ((term.MultipledBy == null) ? 0 : term.MultipledBy.Count))
                return false;

            if (this.MultipledBy != null)
            {
                for (int i = 0; i < this.MultipledBy.Count; i++)
                {
                    if (i >= term.MultipledBy.Count)
                        return false;
                    else if (!this.MultipledBy[i].AreEqual(term.MultipledBy[i]))
                        return false;
                }
            }

            if ((this.Devisor == null && term.Devisor != null) ||
                (term.Devisor == null && this.Devisor != null))
                return false;

            if (this.Devisor != null && !this.Devisor.AreEqual(term.Devisor))
                return false;

            return true;
        }

        #region Simplify

        public bool Simplify()
        {
            bool simplified = false;

            if (this.Root == 1)
            {
                if (!this.Constant)
                {
                    SignsSimplification(ref simplified);
                    PowerSignSimplification(ref simplified);
                    CoEfficientSimplification(ref simplified);

                    //RootSimplification(); 
                }
                else
                {
                    ConstantSimplification(ref simplified);
                }
            }
            else if (this._constant && this.Sign == "+")
            {
                if (this._coEfficient == 1)
                {
                    this.Root = 1;
                    simplified = true;
                }
                if (IsPerfectSquare(this.CoEfficient))
                {
                    this.CoEfficient = (int)Math.Sqrt(this.CoEfficient);
                    simplified = true;
                }
            }
            else if (this.Power == this._root && this.Sign == "+")
            {
                this.Power = 1;
                this.Root = 1;
                this.Sign = "+";
                simplified = true;
            }

            //if (this.Power > 1 && this.Constant)
            //{
            //    this.CoEfficient = (int)Math.Pow(this.CoEfficient, this.Power);
            //    simplified = true;
            //}
            //else
            //throw new NotImplementedException();
            return simplified;
        }
        private bool IsPerfectSquare(int num)
        {
            int root = (int)Math.Sqrt(num);
            return (int)Math.Pow(root, 2) == num;
        }
        public void ConstantSimplification(ref bool simplified)
        {
            if (this.Power > 1)
            {
                this.CoEfficient = (int)Math.Pow(this.CoEfficient, this.Power);
                this.Power = 1;
                simplified = true;
            }

            if (this.Devisor != null)
            {
                #region Divisor

                if (this._powerSign == "-" || this.Devisor._powerSign == "-")
                {
                    simplified = true;
                    int[] one = new int[2];
                    int[] two = new int[2];

                    #region Prepare

                    if (this.PowerSign == "-")
                    {
                        one[0] = 1;
                        one[1] = (int)Math.Pow(this.CoEfficient, this.Power);
                        this.PowerSign = "+";
                        this.Power = 1;
                    }
                    else
                    {
                        one[0] = (int)Math.Pow(this.CoEfficient, this.Power);
                        this.Power = 1;
                        one[1] = 1;
                    }

                    if (this.Devisor.PowerSign == "-")
                    {
                        two[0] = 1;
                        this.Devisor.PowerSign = "+";
                        two[1] = (int)Math.Pow(this.Devisor.CoEfficient, this.Devisor.Power);
                    }
                    else
                    {
                        two[0] = (int)Math.Pow(this.Devisor.CoEfficient, this.Devisor.Power);
                        two[1] = 1;
                    }

                    #endregion Prepare

                    int[] answer = new int[2];
                    answer[0] = one[0] * two[1];
                    answer[1] = one[1] * two[0];

                    this.CoEfficient = answer[0];
                    if (answer[1] != 1)
                    {
                        this.Devisor.CoEfficient = answer[1];
                    }
                    else
                        this.Devisor = null;
                }

                if (this.Devisor != null)
                {
                    if (this.Devisor.Constant)
                    {
                        this.Devisor.CoEfficient = (int)Math.Pow(this.Devisor.CoEfficient, this.Devisor.Power);
                        this.Devisor.Power = 1;
                        simplified = true;
                    }
                    if (this.Devisor.Sign == "-")
                    {
                        this.Sign = (this.Sign == "-") ? "+" : "-";
                        this.Devisor.Sign = "+";
                        simplified = true;
                    }


                    if (this.CoEfficient % this.Devisor.CoEfficient == 0)
                    {
                        this.CoEfficient /= this.Devisor.CoEfficient;
                        this.Devisor = null;
                        simplified = true;
                    }
                    else
                    {
                        int[] answer = new Calculator().SimplifyNumberFraction(
                        this.CoEfficient, this.Devisor.CoEfficient);

                        if (answer != null && answer.Length > 0)
                        {
                            if (this.CoEfficient != answer[0])
                                simplified = true;

                            this.CoEfficient = answer[0];
                            this.Devisor.CoEfficient = answer[1];
                        }

                    }
                }



                //what if ther are neg powers

                #endregion Divisor
            }
            else
            {
                #region No Divisor
                if (this.PowerSign == "-" && this.CoEfficient != 1)
                {
                    this.Devisor = new Term(this.CoEfficient);
                    this.CoEfficient = 1;
                    this.PowerSign = "+";
                    simplified = true;
                }


                #endregion No Divisor
            }
        }
        private void CoEfficientSimplification(ref bool simplified)
        {
            if (this.Devisor != null)
            {
                if (this._coEfficient % this.Devisor._coEfficient == 0)
                {
                    this.CoEfficient /= this.Devisor.CoEfficient;
                    this.Devisor.CoEfficient = 1;
                    simplified = true;
                }
                else
                {
                    int[] answer = new Calculator().SimplifyNumberFraction(
                        this.CoEfficient, this.Devisor.CoEfficient);

                    if (answer[0] != this.CoEfficient)
                        simplified = true;
                    this.CoEfficient = answer[0];
                    this.Devisor.CoEfficient = answer[1];
                }
            }
        }
        private void SignsSimplification(ref bool simplified)
        {
            if (this.Devisor != null)
            {
                if (this.Devisor.Devisor != null)
                    this.Devisor.Devisor.Simplify();

                if (this.Devisor.Sign == "-")
                {
                    if (this.Devisor.Sign == "-")
                        simplified = true;

                    this.Devisor.Sign = "+";
                    this.Sign = (this.Sign == "-") ? "+" : "-";
                }
            }
        }
        private List<Term> SimplifyRoot(List<Term> terms, ref bool simplified)
        {
            for (int i = 0; i < terms.Count; i++)
            {
                if (terms[i] != null && terms[i].Root == terms[i].Power)
                {
                    if (terms[i].Power != 1)
                        simplified = true;
                    terms[i].Root = 1;
                    terms[i].Power = 1;

                }
            }
            return terms;

        }
        private void PowerSignSimplification(ref bool simplified)
        {
            Calculator calc = new Calculator();
            List<Term> all = calc.GetAll(this, false);

            if (this.Devisor != null)
                foreach (Term termMe in calc.GetAll(this.Devisor, false))
                {
                    Term O = new Term(termMe);
                    O._powerSign = (O._powerSign == "+") ? "-" : "+";
                    all.Add(O);
                }



            all = SimplifyRoot(new List<Term>(all), ref simplified);

            int count = all.Count;
            all = MultiplyLike(all);

            if (count != all.Count)
                simplified = true;

            List<Term> top = new List<Term>();
            List<Term> bottom = new List<Term>();
            TopAtTopDivOnBottom(all, ref top, ref bottom);

            RapUpPowerSign(top, false);
            RapUpPowerSign(bottom, true);

        }
        private void RapUpPowerSign(List<Term> terms, bool div)
        {
            terms = new Calculator().Sort(terms);

            if (!div)
            {
                if (terms.Count != 0)
                {
                    this.TermBase = terms[0].TermBase;
                    this.Power = terms[0].Power;
                    this.PowerSign = "+";
                    List<Term> mul = new List<Term>();

                    for (int i = 0; i < terms.Count; i++)
                    {
                        if (terms[i]._coEfficient > this._coEfficient)
                        {
                            this.CoEfficient = terms[i].CoEfficient;
                            terms[i].CoEfficient = 1;
                        }
                    }

                    for (int i = 1; i < terms.Count; i++)
                    {
                        terms[i].PowerSign = "+";
                        mul.Add(terms[i]);
                    }

                    if (mul.Count != 0)
                        this.MultipledBy = mul;
                    else
                        this.MultipledBy = null;
                }
                else
                {
                    this.Constant = true;
                    this.TermBase = '\0';
                    this.Power = 1;
                    this.PowerSign = "+";
                }
            }
            else
            {
                if (terms.Count != 0)
                {
                    Term term = new Term(terms[0].TermBase);
                    term.Power = terms[0].Power;
                    term.PowerSign = "+";
                    List<Term> mul = new List<Term>();

                    for (int i = 0; i < terms.Count; i++)
                    {
                        if (terms[i]._coEfficient > term._coEfficient)
                        {
                            term.CoEfficient = terms[i].CoEfficient;
                            terms[i].CoEfficient = 1;
                        }
                    }

                    for (int i = 1; i < terms.Count; i++)
                    {
                        terms[i].PowerSign = "+";
                        mul.Add(terms[i]);
                    }

                    if (mul.Count != 0)
                        term.MultipledBy = mul;

                    if (this.Devisor != null && this._devisor.Constant)
                        term.CoEfficient *= this._devisor.CoEfficient;
                    this.Devisor = term;
                }
                else
                    this.Devisor = null;
            }
        }
        private void TopAtTopDivOnBottom(List<Term> all, ref List<Term> top, ref List<Term> bottom)
        {
            for (int i = 0; i < all.Count; i++)
                if (all[i]._powerSign == "+")
                    top.Add(all[i]);
                else
                    bottom.Add(all[i]);
        }
        private List<Term> MultiplyLike(List<Term> terms)
        {
            List<Term> answer = new List<Term>();
            while (ThereAreLikeTerms(terms))
            {
                int[] like = GetTheseLikeTerms(terms);
                terms[like[0]] = MultiplyLike(new Term(terms[like[0]]), new Term(terms[like[1]]));
                terms[like[1]] = null;
            }

            for (int i = 0; i < terms.Count; i++)
                if (terms[i] != null)
                    answer.Add(terms[i]);
            return answer;

        }
        private Term MultiplyLike(Term one, Term two)//doesnt do coEfficients
        {
            string sign = (one.Power > two.Power) ? one._powerSign : two._powerSign;
            one.Power = int.Parse((one.PowerSign + one.Power).ToString()) + int.Parse((two._powerSign + two.Power.ToString()));
            one._powerSign = sign;

            if (one.Power == 0)
            {
                return new Term(1);
            }
            return one;
        }
        private bool ThereAreLikeTerms(List<Term> terms)
        {
            for (int i = 0; i < terms.Count; i++)
                for (int k = 0; k < terms.Count; k++)
                    if (i != k && (terms[i] != null && terms[k] != null)
                        && AreLikeInContext(terms[i], terms[k]))
                        return true;
            return false;
        }
        private int[] GetTheseLikeTerms(List<Term> terms)
        {
            for (int i = 0; i < terms.Count; i++)
                for (int k = 0; k < terms.Count; k++)
                    if (i != k && (terms[i] != null && terms[k] != null)
                        && AreLikeInContext(terms[i], terms[k]))
                        return new int[2] { i, k };
            return null;
        }
        private bool AreLikeInContext(Term one, Term two)
        {
            if (one.TermBase == two.TermBase)
                return true;
            else return false;
        }


        #endregion Simplify

        public Type GetType1()
        {
            return Type.Term;
        }

        public ExpressionPieceType GetTypePiece()
        {
            return ExpressionPieceType.Term;
        }



        public TrigTermType TrigTermType()
        {
            if (this.Constant == true)
                return MathBase.TrigTermType.Constant;
            else return MathBase.TrigTermType.Term;
        }
        public bool AreEqual(ITrigTerm other)
        {
            return this.AreEqual((Term)other);
        }
        public TrigExpressionPieceType GetPieceType()
        {
            return TrigExpressionPieceType.Term;
        }
        #endregion Methods
    }
    public class Expression : iEquationPiece, IAlgebraPiece
    {
        #region Properties

        int bracePowerNumerator = 1;

        public int BracePowerNumerator
        {
            get { return bracePowerNumerator; }
            set { bracePowerNumerator = value; }
        }

        int bracePowerDenominator = 1;

        public int BracePowerDenominator
        {
            get { return bracePowerDenominator; }
            set { bracePowerDenominator = value; }
        }

        int rootPowerNumerator = 1;

        public int RootPowerNumerator
        {
            get { return rootPowerNumerator; }
            set { rootPowerNumerator = value; }
        }

        int rootPowerDenominator = 1;

        public int RootPowerDenominator
        {
            get { return rootPowerDenominator; }
            set { rootPowerDenominator = value; }
        }

        List<IExpressionPiece> numerator = new List<IExpressionPiece>();

        public List<IExpressionPiece> Numerator
        {
            get { return numerator; }
            set { numerator = value; }
        }
        List<IExpressionPiece> denominator = new List<IExpressionPiece>();

        public List<IExpressionPiece> Denominator
        {
            get { return denominator; }
            set { denominator = value; }
        }

        #endregion Properties

        #region constructor

        public Expression()
        {
        }

        public Expression(Expression exp)
        {
            this.bracePowerNumerator = exp.bracePowerNumerator;
            this.rootPowerNumerator = exp.rootPowerNumerator;
            this.bracePowerDenominator = exp.bracePowerDenominator;
            this.rootPowerDenominator = exp.rootPowerDenominator;
            this.numerator = new List<IExpressionPiece>(new List<IExpressionPiece>(exp.numerator));
            this.denominator = new List<IExpressionPiece>(new List<IExpressionPiece>(exp.denominator));
        }

        public Expression(IExpressionPiece[] numerator, IExpressionPiece[] denominator)
        {
            if (numerator != null)
                foreach (IExpressionPiece piece in numerator)
                    this.numerator.Add(piece);
            if (denominator != null)
                foreach (IExpressionPiece piece in denominator)
                    this.denominator.Add(piece);
        }

        #endregion constructor

        #region methods

        public bool NoDenominator()
        {
            if (Denominator == null || Denominator.Count == 0)
                return true;

            for (int i = 0; i < Denominator.Count; i++)
            {
                if (Denominator[i].GetTypePiece() == ExpressionPieceType.Term
                    && (!((Term)Denominator[i]).Constant ||
                    (((Term)Denominator[i]).CoEfficient == 1 &&
                    ((Term)Denominator[i]).Sign == "-") ||
                    ((((Term)Denominator[i]).Constant &&
                    ((Term)Denominator[i]).CoEfficient != 1))))
                {
                    return false;
                }
            }
            return true;
        }
        public bool NoDivisors()
        {
            List<IExpressionPiece> pieces = new List<IExpressionPiece>(this.numerator);
            if (this.denominator != null)
                pieces.AddRange(this.denominator);

            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i] != null && pieces[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (((Term)pieces[i]).Devisor != null)
                        return false;
                }
            }
            return true;
        }
        public void AddToExpression(IExpressionPiece piece, bool addToNumerator)
        {
            if (addToNumerator)
                numerator.Add(piece);
            else
                denominator.Add(piece);
        }
        public void AddToExpression(bool addToNumerator, params IExpressionPiece[] pieces)
        {
            foreach (IExpressionPiece piece in pieces)
            {
                if (addToNumerator)
                    numerator.Add(piece);
                else
                    denominator.Add(piece);
            }
        }
        public void AddToExpression(bool addToNumerator, List<IExpressionPiece> pieces)
        {
            //remove braces
            if (addToNumerator)
                numerator.AddRange(pieces);
            else
                Denominator.AddRange(pieces);
        }
        public bool AddLikeTerms()
        {
            bool answer = false;
            if (NoBraces(this.numerator) && NoBraces(this.denominator))
            {
                Calculator calc = new Calculator();

                if (this.numerator.Count > 1)
                {
                    List<IExpressionPiece> newNumerator = calc.AddLikeTerms(this.numerator.ToArray());

                    if (numerator.Count != newNumerator.Count)
                        answer = true;
                    if (newNumerator.Count > 0)
                        this.numerator = new List<IExpressionPiece>(newNumerator);
                    else this.numerator = new List<IExpressionPiece>(new IExpressionPiece[] { new Term(0) });
                }

                if (this.denominator.Count > 1)
                {
                    List<IExpressionPiece> newDenominator = calc.AddLikeTerms(this.denominator.ToArray());

                    if (newDenominator.Count != this.denominator.Count)
                        answer = true;

                    this.denominator = new List<IExpressionPiece>(newDenominator);
                }
                return answer;
            }
            else return false;
        }
        public new Type GetType1()
        {
            return Type.Expression;
        }
        #region RemoveBraces
        public void RemoveBraces()
        {
            if (numerator != null && !NoBraces(numerator))
                RemoveBraces(ref numerator, true);
            if (denominator != null && !NoBraces(denominator))
                RemoveBraces(ref denominator, true);
        }
        public void RemoveBraces(bool Clean) //does not add like terms after
        {
            if (numerator != null && !NoBraces(numerator))
                RemoveBraces(ref numerator, false);
            if (denominator != null && !NoBraces(denominator))
                RemoveBraces(ref denominator, false);
        }
        public void RemoveBracesBabySteps() //does one step and returns...
        {
            if (numerator != null && !NoBraces(numerator))
                RemoveBracesBabySteps(ref numerator);
            if (denominator != null && !NoBraces(denominator))
                RemoveBracesBabySteps(ref denominator);
        }
        private List<IExpressionPiece> GetRange(int start, int end, List<IExpressionPiece> pieces)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            for (int i = start + 1; i < end; i++)
                answer.Add(pieces[i]);
            return answer;
        }
        private void InnerBraces(ref List<IExpressionPiece> pieces, int nextStart, int nextEnd)
        {
            List<IExpressionPiece> subPieces = GetRange(nextStart, nextEnd, pieces);
            RemoveBraces(ref subPieces, true);

            List<IExpressionPiece> old = new List<IExpressionPiece>(pieces);
            pieces.Clear();

            for (int i = 0; i <= nextStart; i++)
                pieces.Add(old[i]);

            pieces.AddRange(subPieces);
            for (int i = nextEnd; i < old.Count; i++)
                pieces.Add(old[i]);
        }
        private List<Term> RemoveBraces(List<IExpressionPiece> pieces)
        {
            RemoveBraces(ref pieces, true);
            List<Term> terms = new List<Term>();

            for (int i = 0; i < pieces.Count; i++)
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Term)
                    terms.Add((Term)pieces[i]);
            return terms;
        }
        private List<IExpressionPiece> RemoveBraceAssist(List<IExpressionPiece> pieces)
        {
            List<List<int>> assistPairs = GetAssistPairs(pieces);
            List<IExpressionPiece> answer = new List<IExpressionPiece>();
            bool started = false;
            List<IExpressionPiece> one = new List<IExpressionPiece>();

            #region Process
            for (int i = 0; i < pieces.Count; i++)
            {
                if (InRange(assistPairs, i))
                {
                    if (started)
                    {
                        one.Add(pieces[i]);
                        if (i == pieces.Count - 1)
                        {
                            answer.Add(new Brace('('));
                            List<IExpressionPiece> subSol = caseA(0, one.Count - 1, new List<IExpressionPiece>(one));
                            subSol = caseC(0, subSol.Count - 1, new List<IExpressionPiece>(subSol));
                            subSol = calculator.AddLikeTerms(subSol);

                            for (int k = 0; k < subSol.Count; k++)
                                if (subSol[k] != null)
                                    answer.Add((subSol[k]));

                            answer.Add(new Brace(')'));
                        }
                    }
                    else
                    {
                        started = true;
                        one = new List<IExpressionPiece>();
                        one.Add(pieces[i]);
                    }
                }
                else if (started)
                {
                    started = false;
                    if (one.Count > 0)
                    {
                        answer.Add(new Brace('('));
                        List<IExpressionPiece> subSol = caseA(0, one.Count - 1, new List<IExpressionPiece>(one));
                        subSol = caseC(0, subSol.Count - 1, new List<IExpressionPiece>(subSol));
                        subSol = calculator.AddLikeTerms(subSol);

                        for (int k = 0; k < subSol.Count; k++)
                            if (subSol[k] != null)
                                answer.Add((subSol[k]));

                        answer.Add(new Brace(')'));
                    }
                    one = new List<IExpressionPiece>();
                }
                else
                {
                    answer.Add(pieces[i]);
                }
            }

            #endregion Process

            return answer;

        }
        private bool InRange(List<List<int>> range, int index)
        {
            for (int i = 0; i < range.Count; i++)
            {
                int lower = range[i][1];
                int upper = range[i][0];

                if (index >= lower && index <= upper)
                    return true;
            }
            return false;
        }
        private List<List<int>> GetAssistPairs(List<IExpressionPiece> pieces)
        {
            int next = 0;
            List<List<int>> answer = new List<List<int>>();

            while (next != -1)
            {
                next = GetClosingForNextAssist((next == 0) ? next : (next + 1), pieces);

                if (next != -1)
                {
                    int opening = GetOpeningIndex(next, pieces);
                    List<int> piece = new List<int>();
                    piece.Add(next);
                    piece.Add(opening);
                    answer.Add(new List<int>(piece));
                }
            }
            return answer;
        }
        private int GetClosingForNextAssist(int start, List<IExpressionPiece> pieces)
        {
            for (int i = start; i < pieces.Count; i++)
            {
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace
                   && ((Brace)pieces[i]).Power > 1)
                    return i;
            }
            return -1;
        }
        private int GetOpeningIndex(int closing, List<IExpressionPiece> pieces)
        {
            int count = 0;
            for (int i = closing - 1; i > 0; i--)
            {
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)pieces[i]).Key == '(')
                {
                    if (count != 0)
                        count--;
                    else
                        return i;
                }
                else if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)pieces[i]).Key == ')')
                    count++;
            }
            return -1;
        }
        private List<IExpressionPiece> CleanExterior(List<IExpressionPiece> pieces)
        {
            while (pieces[0].GetTypePiece() == ExpressionPieceType.Brace &&
                pieces[pieces.Count - 1].GetTypePiece() == ExpressionPieceType.Brace)
            {
                if (((Brace)pieces[0]).Key == '(' && ((Brace)pieces[pieces.Count - 1]).Key == ')')
                {
                    List<IExpressionPiece> newPieces = new List<IExpressionPiece>();
                    for (int i = 1; i < pieces.Count - 1; i++)
                        newPieces.Add(pieces[i]);

                    pieces = new List<IExpressionPiece>(newPieces);
                }
                else throw new NotImplementedException();
            }
            return pieces;
        }
        private void RemoveBraces(ref List<IExpressionPiece> pieces, bool clean)
        {
            pieces = RemoveBraceAssist(pieces);
            while (!NoBraces(pieces))
            {

                int nextStart = GetIndexOfNextOpeningBrace(pieces);
                int nextEnd = GetClosingIndex(nextStart, pieces);

                if (HasInnerBraces(nextStart, nextEnd, pieces))
                    InnerBraces(ref pieces, nextStart, nextEnd);
                else
                {
                    //pieces = pieces;
                    //Do magic here nigga nigga
                    switch (DetermineCase(ref nextStart, ref nextEnd, pieces))
                    {
                        case "CaseA":
                            pieces = caseA(nextStart, nextEnd, pieces);
                            break;
                        case "CaseB":
                            pieces = caseB(nextStart, nextEnd, pieces);
                            break;
                        case "CaseC":
                            pieces = caseA(nextStart, nextEnd, pieces);
                            break;
                        case "CaseD":
                            pieces = caseD(nextStart, nextEnd, pieces);
                            break;
                        default:
                            pieces = CleanExterior(pieces);
                            break;
                    }
                }
            }

            if (clean)
                pieces = calculator.AddLikeTerms(pieces);

        }

        private void RemoveBracesBabySteps(ref List<IExpressionPiece> pieces)
        {
            pieces = RemoveBraceAssist(pieces);
            if (!NoBraces(pieces))
            {

                int nextStart = GetIndexOfNextOpeningBrace(pieces);
                int nextEnd = GetClosingIndex(nextStart, pieces);

                if (HasInnerBraces(nextStart, nextEnd, pieces))
                    InnerBraces(ref pieces, nextStart, nextEnd);
                else
                {
                    //pieces = pieces;
                    //Do magic here nigga nigga
                    switch (DetermineCase(ref nextStart, ref nextEnd, pieces))
                    {
                        case "CaseA":
                            pieces = caseA(nextStart, nextEnd, pieces);
                            break;
                        case "CaseB":
                            pieces = caseB(nextStart, nextEnd, pieces);
                            break;
                        case "CaseC":
                            pieces = caseA(nextStart, nextEnd, pieces);
                            break;
                        case "CaseD":
                            pieces = caseD(nextStart, nextEnd, pieces);
                            break;
                        default:
                            pieces = CleanExterior(pieces);
                            break;
                    }
                }
            }

            //if (clean)
            //pieces = calculator.AddLikeTerms(pieces);

        }
        public string DetermineCase(ref int Start, ref int End, List<IExpressionPiece> pieces)
        {


            if (((Brace)pieces[End]).Power > 0) // (x + 1)2
            {
                return "CaseA";
            }
            else if ((End + 1) != pieces.Count &&
                        pieces[End + 1].GetTypePiece() == ExpressionPieceType.Brace)//caseB,C
            {
                int value = GetLastIndex(End, pieces);
                End = (value != -1) ? value : End;
                return "CaseB";
            }
            else if ((Start - 1 > -1 && pieces[Start - 1].GetTypePiece() != ExpressionPieceType.Brace)
                        && (End + 1 == pieces.Count || pieces[End + 1].GetTypePiece() != ExpressionPieceType.Brace)
                        && pieces[Start - 1].GetTypePiece() == ExpressionPieceType.Term)
            {
                Start = Start - 1;
                return "CaseD";
            }
            return null;//
            //cange star and end to corespond

        }
        public int GetLastIndex(int start, List<IExpressionPiece> pieces)
        {
            bool inBraces = true;

            for (int i = start; i < pieces.Count; i++)
                if (pieces[i].GetTypePiece() != ExpressionPieceType.Brace && !inBraces)
                    return i - 1;
                else if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (((Brace)pieces[i]).Key == ')')
                        inBraces = false;
                    else if (((Brace)pieces[i]).Key == '(')
                        inBraces = true;
                }
            return pieces.Count - 1;
        }
        private int GetIndexOfNextOpeningBrace(List<IExpressionPiece> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)pieces[i]).Key == '(')
                    return i;
            return -1;
        }
        private int GetClosingIndex(int index, List<IExpressionPiece> pieces)
        {
            int capper = 0;

            for (int i = index + 1; i < pieces.Count; i++)
            {
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)pieces[i]).Key == ')')
                    if (capper == 0)
                        return i;
                    else
                        capper--;
                else if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)pieces[i]).Key == '(')
                    capper++;
            }
            return -1;
        }
        private bool HasInnerBraces(int start, int end, List<IExpressionPiece> pieces)
        {
            for (int i = start + 1; i < end; i++)
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Brace)
                    return true;
            return false;
        }
        public bool NoBraces(List<IExpressionPiece> pieces)
        {
            foreach (IExpressionPiece piece in pieces)
                if (piece.GetTypePiece() == ExpressionPieceType.Brace)
                    return false;
            return true;
        }
        Calculator calculator = new Calculator();
        private List<IExpressionPiece> caseD(int start, int end, List<IExpressionPiece> pieces) //2 (x + 1)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            for (int i = 0; i < start; i++)
                answer.Add(pieces[i]);

            //do work here 
            #region Work
            List<IExpressionPiece> problem = new List<IExpressionPiece>();
            for (int i = start; i <= end; i++)
                problem.Add(pieces[i]);

            List<IExpressionPiece> solution = new List<IExpressionPiece>();


            Term outerTerm = (Term)GetOuterTerm(problem);
            int constValue = (outerTerm.Constant) ? outerTerm.CoEfficient : -1;
            bool started = false;
            foreach (IExpressionPiece piece in problem)
                if (piece.GetTypePiece() == ExpressionPieceType.Brace &&
                    ((Brace)piece).Key == '(')
                    started = true;
                else if (piece.GetTypePiece() == ExpressionPieceType.Brace &&
                        ((Brace)piece).Key == ')')
                    started = false;
                else if (started)
                    if (piece.GetTypePiece() == ExpressionPieceType.Term)
                    {
                        if (outerTerm.Constant)
                        {
                            Term term = new Term(constValue);
                            term.Sign = outerTerm.Sign;
                            term.Devisor = outerTerm.Devisor;
                            term.MultipledBy = outerTerm.MultipledBy;
                            term.Power = outerTerm.Power;
                            term.PowerSign = outerTerm.PowerSign;
                            term.Root = outerTerm.Root;
                            term.Sorted = outerTerm.Sorted;
                            term.TwoSigns = outerTerm.TwoSigns;

                            solution.Add((Term)calculator.Calculate(term, (Term)piece, MathFunction.Mutliply));
                        }
                        else
                            solution.Add((Term)calculator.Calculate(outerTerm, (Term)piece, MathFunction.Mutliply));
                    }
                    else
                        solution.Add(piece);

            answer.AddRange(solution);
            #endregion Work

            for (int i = end + 1; i < pieces.Count; i++)
                answer.Add(pieces[i]);

            return answer;
        }
        private List<IExpressionPiece> caseC(int start, int end, List<IExpressionPiece> pieces) // (x + 1) (x + 1)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            for (int i = 0; i < start; i++)
                answer.Add(pieces[i]);

            //do work here 
            #region Work
            List<IExpressionPiece> problem = new List<IExpressionPiece>();
            for (int i = start; i <= end; i++)
                problem.Add(pieces[i]);

            List<IExpressionPiece> solution = new List<IExpressionPiece>();

            //
            List<IExpressionPiece> braceOne = new List<IExpressionPiece>();
            List<IExpressionPiece> braceTwo = new List<IExpressionPiece>();

            SetPieces(ref braceOne, ref braceTwo, problem);

            for (int i = 0; i < braceOne.Count; i++)
            {
                for (int j = 0; j < braceTwo.Count; j++)
                {

                    Term pieceOne = null, pieceTwo = null;

                    pieceOne = (((Term)braceOne[i]).Constant) ? new Term(((Term)braceOne[i]).CoEfficient) : new Term((Term)braceOne[i]);
                    pieceTwo = (((Term)braceTwo[j]).Constant) ? new Term(((Term)braceTwo[j]).CoEfficient) : new Term((Term)braceTwo[j]);

                    if (((Term)braceOne[i]).Constant)
                    {
                        pieceOne.Sign = ((Term)braceOne[i]).Sign;
                        pieceOne.Devisor = ((Term)braceOne[i]).Devisor;
                        pieceOne.MultipledBy = ((Term)braceOne[i]).MultipledBy;
                        pieceOne.Power = ((Term)braceOne[i]).Power;
                        pieceOne.PowerSign = ((Term)braceOne[i]).PowerSign;
                        pieceOne.Root = ((Term)braceOne[i]).Root;
                        pieceOne.Sorted = ((Term)braceOne[i]).Sorted;
                        pieceOne.TwoSigns = ((Term)braceOne[i]).TwoSigns;
                    }
                    if (((Term)braceTwo[j]).Constant)
                    {
                        pieceTwo.Sign = ((Term)braceTwo[j]).Sign;
                        pieceTwo.Devisor = ((Term)braceTwo[j]).Devisor;
                        pieceTwo.MultipledBy = ((Term)braceTwo[j]).MultipledBy;
                        pieceTwo.Power = ((Term)braceTwo[j]).Power;
                        pieceTwo.PowerSign = ((Term)braceTwo[j]).PowerSign;
                        pieceTwo.Root = ((Term)braceTwo[j]).Root;
                        pieceTwo.Sorted = ((Term)braceTwo[j]).Sorted;
                        pieceTwo.TwoSigns = ((Term)braceTwo[j]).TwoSigns;
                    }

                    //pieceOne.MultipledBy = null;
                    //pieceTwo.MultipledBy = null;
                    //solution.Add((Term)calculator.Calculate((Term)braceOne[i], (Term)braceTwo[j], MathFunction.Mutliply));
                    solution.Add(new Term((Term)calculator.Calculate(new Term(pieceOne), new Term(pieceTwo), MathFunction.Mutliply)));
                }
            }

            answer.AddRange(solution);
            #endregion Work

            for (int i = end + 1; i < pieces.Count; i++)
                answer.Add(pieces[i]);

            return answer;
        }
        private List<IExpressionPiece> caseA(int start, int end, List<IExpressionPiece> pieces) // (x + 1)2
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            for (int i = 0; i < start; i++)
                answer.Add(pieces[i]);

            //do work here 
            #region Work

            List<IExpressionPiece> problem = new List<IExpressionPiece>();

            for (int i = start; i <= end; i++)
                if (pieces[i].GetTypePiece() != ExpressionPieceType.Sign)
                    problem.Add(pieces[i]);

            List<IExpressionPiece> solution = new List<IExpressionPiece>();

            //
            Brace closing = (Brace)problem[problem.Count - 1];
            int times = closing.Power;
            closing.Power = 0;
            problem[problem.Count - 1] = closing;

            for (int i = 1; i <= times; i++)
                solution.AddRange(problem);

            answer.AddRange(solution);
            #endregion Work

            for (int i = end + 1; i < pieces.Count; i++)
                answer.Add(pieces[i]);

            return answer;
        }
        private List<IExpressionPiece> caseB(int start, int end, List<IExpressionPiece> pieces) // (x + 1)(x + 1)(x + 1)
        {
            List<IExpressionPiece> answer = new List<IExpressionPiece>();

            for (int i = 0; i < start; i++)
                answer.Add(pieces[i]);

            //do work here 
            #region Work

            List<IExpressionPiece> problem = new List<IExpressionPiece>();

            for (int i = start; i <= end; i++)
                if (pieces[i].GetTypePiece() != ExpressionPieceType.Sign)
                    problem.Add(pieces[i]);


            while (MoreThanTwo(problem))
            {
                List<IExpressionPiece> firstTwo = new List<IExpressionPiece>();
                GetFirstTwo(ref problem, ref firstTwo);
                problem.Add(new Brace('('));
                problem.AddRange(caseC(0, firstTwo.Count - 1, firstTwo));
                problem.Add(new Brace(')'));
            }


            List<IExpressionPiece> solution = caseC(0, problem.Count - 1, problem);

            //

            answer.AddRange(solution);
            #endregion Work

            for (int i = end + 1; i < pieces.Count; i++)
                answer.Add(pieces[i]);

            return answer;
        }
        private void GetFirstTwo(ref List<IExpressionPiece> whole, ref List<IExpressionPiece> firstTwo)
        {
            int count = 0;
            bool adding = true;
            List<IExpressionPiece> remainder = new List<IExpressionPiece>();

            foreach (IExpressionPiece piece in whole)
            {
                if (adding)
                {
                    firstTwo.Add(piece);

                    if (piece.GetTypePiece() == ExpressionPieceType.Brace
                        && ((Brace)piece).Key == ')')
                    {
                        count++;
                        if (count >= 2)
                            adding = false;
                    }
                }
                else
                    remainder.Add(piece);
            }
            whole = new List<IExpressionPiece>(remainder);


        }
        private bool MoreThanTwo(List<IExpressionPiece> items)
        {
            int count = 0;
            foreach (IExpressionPiece piece in items)
                if (piece.GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)piece).Key == ')')
                {
                    count++;
                    if (count > 2)
                        return true;
                }
            return false;
        }
        private void SetPieces(ref List<IExpressionPiece> braceOne, ref List<IExpressionPiece> braceTwo, List<IExpressionPiece> whole)// (x + 1) (x + 1)
        {
            bool second = false;

            for (int i = 1; i < whole.Count; i++)
            {
                if (whole[i].GetTypePiece() == ExpressionPieceType.Brace
                    && ((Brace)whole[i]).Key == '(')
                    second = true;

                if (whole[i].GetTypePiece() == ExpressionPieceType.Term)
                    if (!second)
                        braceOne.Add(whole[i]);
                    else
                        braceTwo.Add(whole[i]);

            }
        }
        private IExpressionPiece GetOuterTerm(List<IExpressionPiece> problemExp)
        {
            bool isIn = false;

            foreach (IExpressionPiece piece in problemExp)
                if (piece.GetTypePiece() == ExpressionPieceType.Term && !isIn)
                    return (Term)piece;
                else if (piece.GetTypePiece() == ExpressionPieceType.Brace)
                    if (((Brace)piece).Key == ')')
                        isIn = false;
                    else
                        isIn = true;

            throw new Exception();

        }

        #endregion RemoveBraces

        #region Factor
        public virtual bool Factorize()
        {
            if (this.numerator != null && this.numerator.Count > 1)
            {
                if (!NoBraces(this.numerator))
                    RemoveBraces(ref this.numerator, true);
                //factor numerator
                List<IExpressionPiece> answer = Factorize(this.numerator);

                if (answer.Count == 0 && numerator.Count > 1)
                    return false;
                else
                {
                    this.numerator = new List<IExpressionPiece>(answer);
                }
            }


            if (this.denominator != null && this.denominator.Count > 1)
            {

                if (!NoBraces(this.denominator))
                    RemoveBraces(ref this.denominator, true);
                //factor numerator
                //Factorize(ref this.denominator);

                List<IExpressionPiece> answer = Factorize(this.denominator);

                if (answer.Count == 0 && this.denominator.Count > 1)
                    return false;
                else
                {
                    this.denominator = new List<IExpressionPiece>(answer);
                }
            }

            return true;
        }
        public List<IExpressionPiece> Factorize(List<IExpressionPiece> Pieces)
        {
            List<Term> terms = GetTerms(Pieces);

            if (terms.Count == 2 && terms[0].Power % 2 == 0 &&
                IsPerfectSquare(terms[0].CoEfficient) && IsPerfectSquare(terms[1].CoEfficient))//binomial, perfect squares
                Pieces = DifferenceOfTwoSquires(terms);
            else if (terms.Count == 3 && terms[0].Power % 2 == 0 && (terms[2].Constant || terms[2].Power % 2 == 0))//trinimial
            {
                Pieces = QuadraticEquation(terms);

                //if (Pieces == null)
                //{
                //    //List<List<IExpressionPiece>> solution = QuadraticFormula(terms);
                //    //use quadratic formula
                //}
            }
            else if (terms.Count == 4)
                Pieces = Polynomial(terms);
            //else if (CanGetCommonFactor(terms))
            //Pieces = CommonFactor(terms);

            if (NoBraces(Pieces))
                return new List<IExpressionPiece>();
            return Pieces;

        }
        public List<List<IExpressionPiece>> QuadraticFormula(List<Term> pieces)
        {
            int a = pieces[0].CoEfficient * ((pieces[0].Sign == "+" ? 1 : -1)),
                b = pieces[1].CoEfficient * ((pieces[1].Sign == "+" ? 1 : -1)),
                c = pieces[2].CoEfficient * ((pieces[2].Sign == "+" ? 1 : -1));

            double one = (-1 * b + (Math.Sqrt(Math.Pow(b, 2) - (4 * a * c)))) / (a * 2);

            double two = (-1 * b - (Math.Sqrt(Math.Pow(b, 2) - (4 * a * c)))) / (a * 2);



            if (one % 1 == 0 && two % 1 == 0)
            {
                //factored
            }

            throw new NotImplementedException();
        }
        public List<IExpressionPiece> CommonFactor(List<Term> pieces)
        {
            Term term = null;
            if (CanGetCommonFactor(pieces, ref term))
            {
                Term commonTerm = new Term(term);

                List<IExpressionPiece> answer = new List<IExpressionPiece>();
                answer.Add(commonTerm);
                answer.Add(new Brace('('));

                for (int i = 0; i < pieces.Count; i++)
                {
                    answer.Add((Term)calculator.Calculate(new Term((Term)pieces[i]), new Term(commonTerm), MathFunction.Divide));
                }
                answer.Add(new Brace(')'));

                return answer;
            }
            return new List<IExpressionPiece>();
        }
        private bool CanGetCommonFactor(List<Term> pieces)
        {
            Term term = null;
            return CanGetCommonFactor(pieces, ref term);
        }
        private bool CanGetCommonFactor(List<Term> pieces, ref Term cu)
        {
            List<TermOnSteriods> terms = new List<TermOnSteriods>();

            if (pieces.Count >= 2)
            {

                #region Prepare

                for (int i = 0; i < pieces.Count; i++)
                {
                    Term Host = new Term(pieces[i]);
                    int co1 = Host.CoEfficient;
                    int co2 = 1;

                    if (Host.Devisor != null && Host.Devisor.CoEfficient > 1)
                        co2 = Host.Devisor.CoEfficient;

                    List<Term> mult = (Host.MultipledBy == null) ? new List<Term>() : new List<Term>(Host.MultipledBy);
                    List<Term> multDiv = new List<Term>();

                    Term div = null;
                    if (Host.Devisor != null)
                        div = new Term(Host.Devisor);

                    if (div != null && div.MultipledBy != null)
                        multDiv.AddRange(div.MultipledBy);
                    Host.Devisor = null;
                    Host.MultipledBy = null;
                    Host.CoEfficient = 1;

                    //mult.Add(Host);
                    terms.Add(new TermOnSteriods(i, Host, co1, Host.Sign, Side.Top));

                    for (int j = 0; j < mult.Count; j++)
                        terms.Add(new TermOnSteriods(i, mult[j], co1, Side.Top));

                    if (div != null)
                    {
                        div.MultipledBy = null;
                        div.CoEfficient = 1;

                        //.Add(div);
                        terms.Add(new TermOnSteriods(i, div, co2, div.Sign, Side.Bottom));

                        for (int j = 0; j < multDiv.Count; j++)
                            terms.Add(new TermOnSteriods(i, multDiv[j], co2, Side.Bottom));
                    }

                }
                #endregion Prepare

                #region Process

                cu = new Term(1);


                #region Check Sign

                bool negSign = true;
                for (int i = 1; i < terms.Count; i++)
                {
                    if (terms[i].Sign != null && terms[i].Sign != "-")
                    {
                        negSign = false;
                        break;
                    }
                }

                if (negSign)
                    cu.Sign = "-";
                else
                    cu.Sign = "+";

                #endregion Check Sign

                #region check CoEfficient

                cu.CoEfficient = GetLCM(terms, Side.Top);

                int dim = GetLCM(terms, Side.Bottom);
                if (dim != 1)
                {
                    if (cu.Devisor == null)
                        cu = new Term(dim);
                    else
                        cu.CoEfficient = dim;
                }


                #endregion check CoEfficient

                Term commonTop = GetCommonTerms(terms);
                //List<Term> commonBot = GetCommonTerms(terms, Side.Bottom);

                #endregion Process

                #region Rap up

                if (commonTop != null)
                {
                    commonTop.Sign = cu.Sign;
                    commonTop.CoEfficient = cu.CoEfficient;
                    if (cu.Devisor != null)
                        commonTop.Devisor.CoEfficient = cu.Devisor.CoEfficient;

                    cu = new Term(commonTop);

                    if (cu.CoEfficient == 1 && (cu.Devisor == null || cu.CoEfficient == 1) && cu.Sign != "-")
                    {
                        cu = null;
                        return false;
                    }
                    return true;
                    //add cu to common 
                }
                else
                {
                    if (cu.CoEfficient == 1 && (cu.Devisor == null || cu.CoEfficient == 1) && cu.Sign != "-")
                    {
                        cu = null;
                        return false;
                    }
                    return true;
                }
                #endregion Rap up
            }
            return false;

        }
        private Term GetCommonTerms(List<TermOnSteriods> terms)
        {
            Term top = GetCommonTerms(terms, Side.Top);
            Term Devisor = GetCommonTerms(terms, Side.Bottom);

            if (top != null && Devisor != null)
            {
                top.Devisor = Devisor;
            }
            else if (top == null && Devisor != null)
            {
                top = new Term(1);
                top.Devisor = Devisor;
            }

            return top;
        }
        private Term GetCommonTerms(List<TermOnSteriods> terms, Side side)
        {
            if (terms.Count > 0)
            {
                List<List<TermOnSteriods>> groupings = GroupStuff(terms, side);//perhaps add like terms on these before working...

                List<TermOnSteriods> common = new List<TermOnSteriods>(groupings[0]);

                #region Base
                for (int j = 0; j < common.Count; j++)///common
                {
                    for (int i = 1; i < groupings.Count; i++)
                    {
                        bool found = false;
                        for (int k = 0; k < groupings[i].Count; k++)
                        {
                            if (common[j] != null && common[j].Term.TermBase == groupings[i][k].Term.TermBase)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            common[j] = null;
                        }
                        //Term int Current
                    }
                }

                #endregion Base

                #region Power

                for (int j = 0; j < common.Count; j++)///common
                {
                    if (common[j] != null)
                    {
                        List<int> allpowers = new List<int>();
                        for (int i = 0; i < groupings.Count; i++)
                        {
                            for (int k = 0; k < groupings[i].Count; k++)
                            {
                                if (common[j].Term.TermBase == groupings[i][k].Term.TermBase)
                                    allpowers.Add(groupings[i][k].Term.Power);
                            }
                        }
                        common[j].Term.Power = Min(allpowers);
                    }
                }

                #endregion Power

                #region Rap up

                List<Term> decode = RemoveFromSteriods(common);

                if (common.Count == 0 || calculator.allNull(common))
                    return null;

                decode = calculator.Sort(decode);

                Term answer = null;

                if (decode[0].Constant)
                    return decode[0];
                else
                {

                    answer = new Term(decode[0]);
                    answer.MultipledBy = new List<Term>();
                    for (int i = 1; i < decode.Count; i++)
                        answer.MultipledBy.Add(decode[i]);
                    return answer;
                }

                #endregion Rap up
            }
            else return null;

        }
        private int Min(List<int> values)
        {
            int min = values[0];

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] > min)
                    min = values[i];
            }
            return min;
        }
        private List<Term> RemoveFromSteriods(List<TermOnSteriods> terms)
        {
            List<Term> answer = new List<Term>();
            foreach (TermOnSteriods term in terms)
                if (term != null)
                    answer.Add(term.Term);
            return answer;
        }
        private List<List<TermOnSteriods>> GroupStuff(List<TermOnSteriods> terms, Side side)
        {
            List<List<TermOnSteriods>> answer = new List<List<TermOnSteriods>>();

            int current = 0;
            List<TermOnSteriods> innerAnswer = new List<TermOnSteriods>();

            for (int i = 0; i < terms.Count; i++)
            {
                if (terms[i].HostTermNumber == current)
                {
                    if (terms[i].Side == side)
                    {
                        innerAnswer.Add(terms[i]);
                    }
                    if (i == terms.Count - 1)
                    {
                        answer.Add(new List<TermOnSteriods>(innerAnswer));
                    }

                }
                else
                {
                    answer.Add(new List<TermOnSteriods>(innerAnswer));
                    innerAnswer = new List<TermOnSteriods>();
                    if (terms[i].Side == side)
                    {
                        innerAnswer.Add(terms[i]);
                    }
                    current = terms[i].HostTermNumber;
                    if (i == terms.Count - 1)
                    {
                        answer.Add(new List<TermOnSteriods>(innerAnswer));
                    }
                }

            }

            return answer;
        }
        private int GetLCM(List<TermOnSteriods> pieces, Side side)
        {
            List<int> values = new List<int>();

            for (int i = 0; i < pieces.Count; i++)
                if (pieces[i].Sign != null && pieces[i].Side == side)
                {
                    values.Add(pieces[i].CoEfficient1);
                }

            int min = (values.Count == 0) ? 1 : Min(values);

            if (DividesIntoAll(values, min))
                return min;

            for (int k = 2; k <= min; k++)
            {
                if (DividesIntoAll(values, k))
                {
                    return k;
                }
            }
            return 1;
        }
        private bool DividesIntoAll(List<int> values, int divisor)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i] % divisor != 0)
                    return false;
            return true;
        }
        private bool TermsEqual(Term one, Term two)
        {
            if (!one.Sorted)
                one = calculator.Sort(one);

            if (!two.Sorted)
                two = calculator.Sort(two);

            if (one.Constant == two.Constant)
            {
                if (one.TermBase != two.TermBase)
                    return false;

                if (((one.MultipledBy == null || one.MultipledBy.Count == 0) && (two.MultipledBy != null && two.MultipledBy.Count != 0))
                    || ((two.MultipledBy == null || two.MultipledBy.Count == 0) && (one.MultipledBy != null && one.MultipledBy.Count != 0)))
                    return false;

                if (one.MultipledBy != null && two.MultipledBy != null)
                    if (one.MultipledBy.Count != two.MultipledBy.Count)
                        return false;

                if (one.MultipledBy != null)
                    for (int i = 0; i < one.MultipledBy.Count; i++)
                        if (!TermsEqual(one.MultipledBy[i], two.MultipledBy[i]))
                            return false;

                if ((one.Devisor != null && two.Devisor == null)
                    || one.Devisor == null && two.Devisor != null)
                    return false;

                if ((one.Devisor != null && two.Devisor != null))
                    if (!TermsEqual(one.Devisor, two.Devisor))
                        return false;

                if (one.Constant && one.CoEfficient != two.CoEfficient)
                    return false;

                if (one.Root != two.Root || one.Sign != two.Sign
                    || one.PowerSign != two.PowerSign || one.Power != two.Power)
                    return false;

                return true;
            }
            else
                return false;
        }
        private List<IExpressionPiece> Polynomial(List<Term> terms)
        {


            Term a1 = null, a2 = null, b1 = null, b2 = null;

            if (terms[3].Constant)
            {
                a2 = (Term)calculator.Calculate(new Term(terms[0].TermBase, terms[0].Power, ((terms[0].Sign == "+") ? false : true)),
                    new Term(terms[1].TermBase, terms[1].Power, ((terms[1].Sign == "+") ? false : true)), MathFunction.Divide);
                a2.MultipledBy = null;
                b1 = new Term(terms[2].CoEfficient, ((terms[2].Sign == "+") ? false : true));
                b2 = new Term(terms[1].CoEfficient, ((terms[1].Sign == "+") ? false : true));
            }
            else
            {
                Term current = new Term(terms[1]);
                current.MultipledBy = null;
                a2 = (Term)calculator.Calculate(new Term(terms[0].TermBase, terms[0].Power, ((terms[0].Sign == "+") ? false : true)),
                    new Term(current.TermBase, current.Power, ((current.Sign == "+") ? false : true)), MathFunction.Divide);

                b1 = (Term)calculator.Calculate(new Term(terms[3]), new Term(terms[1].MultipledBy[0]), MathFunction.Divide);
                b2 = (Term)calculator.Calculate(new Term(terms[3]), new Term(terms[2].MultipledBy[0]), MathFunction.Divide);

            }

            a1 = (Term)calculator.Calculate(new Term(terms[0]), new Term(a2), MathFunction.Divide);

            List<IExpressionPiece> factors = new List<IExpressionPiece>();
            factors.Add(new Brace('('));
            factors.Add(new Term(a1));
            factors.Add(new Term(b1));
            factors.Add(new Brace(')'));
            factors.Add(new Brace('('));
            factors.Add(new Term(a2));
            factors.Add(new Term(b2));
            factors.Add(new Brace(')'));

            List<Term> termAsnwer = RemoveBraces(factors);

            if (AreTheSame(termAsnwer, terms))
                return factors;
            else
                return new List<IExpressionPiece>();



            throw new NotImplementedException();
        }
        private List<IExpressionPiece> QuadraticEquation(List<Term> terms) //    x^2 - x - 2
        {
            List<int[]> a = GetFactors(terms[0].CoEfficient); //possible a combinations
            List<int[]> b = GetFactors(terms[2].CoEfficient);//possible b combinations

            for (int i = 0; i < a.Count; i++)
            {
                for (int j = 0; j < b.Count; j++)
                {
                    if (QuadraticEquationIsGood(a[i][0], a[i][1], b[j][0], b[j][1], terms))
                    {                           //int a1, int a2, int b1, int b2

                        #region Work
                        Term a1 = new Term(terms[0].TermBase, Math.Abs(a[i][0]), terms[0].Power / 2, (a[i][0] < 0) ? true : false);
                        Term a2 = new Term(terms[0].TermBase, Math.Abs(a[i][1]), terms[0].Power / 2, (a[i][1] < 0) ? true : false);

                        Term b1 = null, b2 = null;

                        if (!terms[2].Constant)
                        {
                            b1 = new Term(terms[2].TermBase, Math.Abs(b[j][0]), terms[2].Constant ? 1 : (terms[2].Power / 2), (b[j][0] < 0) ? true : false);
                            b2 = new Term(terms[2].TermBase, Math.Abs(b[j][1]), terms[2].Constant ? 1 : (terms[2].Power / 2), (b[j][1] < 0) ? true : false);
                            //check that the middle term can be generated from these new terms

                            Term answer = (Term)calculator.Calculate((Term)calculator.Calculate(new Term(a1), new Term(b2), MathFunction.Mutliply)
                                , (Term)calculator.Calculate(new Term(a2), new Term(b1), MathFunction.Mutliply), MathFunction.Add);

                            if (!TermsEqual(terms[1], answer))
                                return new List<IExpressionPiece>();

                        }
                        else
                        {
                            b1 = new Term(Math.Abs(b[j][0]), (b[j][0] < 0) ? true : false);
                            b2 = new Term(Math.Abs(b[j][1]), (b[j][1] < 0) ? true : false);


                        }
                        #endregion Work
                        List<IExpressionPiece> factors = new List<IExpressionPiece>();
                        factors.Add(new Brace('('));
                        factors.Add(new Term(a1));
                        factors.Add(new Term(b1));
                        factors.Add(new Brace(')'));
                        factors.Add(new Brace('('));
                        factors.Add(new Term(a2));
                        factors.Add(new Term(b2));
                        factors.Add(new Brace(')'));


                        List<Term> termAsnwer = calculator.AddLikeTerms(RemoveBraces(factors));

                        if (AreTheSame(termAsnwer, terms))
                            return factors;
                        //build solution
                    }
                }
            }
            return new List<IExpressionPiece>();
        }
        private bool QuadraticEquationIsGood(int a1, int a2, int b1, int b2, List<Term> terms)
        {
            int E1 = (terms[0].Sign == "+") ? terms[0].CoEfficient : (terms[0].CoEfficient * -1);
            int E2 = (terms[1].Sign == "+") ? terms[1].CoEfficient : (terms[1].CoEfficient * -1);
            int E3 = (terms[2].Sign == "+") ? terms[2].CoEfficient : (terms[2].CoEfficient * -1);

            if (!(((a1 * b2) + (b1 * a2)) == E2))
                return false;
            else if (!(b1 * b2 == E3))
                return false;
            else if (!(a1 * a2 == E1))
                return false;
            else
                return true;
        }
        private List<int[]> GetFactors(int number)
        {
            List<int[]> factors = GetFactorsCore(number);

            List<int[]> answer = new List<int[]>();

            foreach (int[] factorPair in factors)
            {
                answer.Add(factorPair);// both positive
                answer.Add(new int[] { factorPair[0] * -1, factorPair[1] * -1 }); //both negative
                answer.Add(new int[] { factorPair[0] * -1, factorPair[1] }); //first negative
                answer.Add(new int[] { factorPair[0], factorPair[1] * -1 }); //second negative
            }
            return answer;
        }
        private List<int[]> GetFactorsCore(int number)
        {
            number = Math.Abs(number);
            List<int[]> factors = new List<int[]>();

            if (number == 1)
            {
                factors.Add(new int[] { 1, 1 });
                return factors;
            }
            else if (number == 0)
                return null;

            int upperBound = (int)(number / 2);

            for (int i = 1; i <= upperBound; i++)
                if (number % i == 0 && !FactorsAlreadyFound(i, (number / i), factors))
                    factors.Add(new int[] { i, (number / i) });


            return factors;
        }
        public bool AreTheSame(List<Term> first, List<Term> second)
        {
            List<Term> a = new List<Term>(first);
            List<Term> b = new List<Term>(second);


            for (int i = 0; i < a.Count; i++)
            {
                for (int j = 0; j < b.Count; j++)
                {
                    if (a[i] != null && b[j] != null)
                    {
                        if (a[i].AreEqual(b[j]))
                        {
                            a[i] = null;
                            b[j] = null;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < a.Count; i++)
                if (a[i] != null)
                    return false;
            for (int j = 0; j < b.Count; j++)
                if (b[j] != null)
                    return false;

            return true;
        }
        private bool FactorsAlreadyFound(int factor1, int factor2, List<int[]> factors)
        {
            foreach (int[] variables in factors)
                if ((variables[0] == factor1 && variables[1] == factor2)
                    || (variables[0] == factor2 && variables[1] == factor1))
                    return true;
            return false;
        }
        private List<IExpressionPiece> DifferenceOfTwoSquires(List<Term> terms) //  x^2 + 64
        {
            #region Terms

            if (terms[1].Power > 1)
            {
                terms[1].CoEfficient = (int)Math.Pow(terms[1].CoEfficient, terms[1].Power);
                terms[1].Power = 1;
            }

            int one = (int)Math.Sqrt(terms[0].CoEfficient);
            int two = (int)Math.Sqrt(terms[1].CoEfficient);

            Term termOne = new Term(terms[0]);
            termOne.Power = termOne.Power / 2;
            termOne.CoEfficient = one;

            Term termTwo = new Term(terms[1]);
            termTwo.CoEfficient = two;

            Term termThree = new Term(termTwo);
            termThree.Sign = (termTwo.Sign == "+") ? "-" : "+";

            #endregion Terms


            List<IExpressionPiece> answer = new List<IExpressionPiece>();
            answer.Add(new Brace('('));
            answer.Add(termOne);
            answer.Add(termTwo);
            answer.Add(new Brace(')'));
            answer.Add(new Brace('('));
            answer.Add(termOne);
            answer.Add(termThree);
            answer.Add(new Brace(')'));



            return answer;
        }
        private bool IsPerfectSquare(int num)
        {
            int root = (int)Math.Sqrt(num);
            return (int)Math.Pow(root, 2) == num;
        }
        private List<Term> GetTerms(List<IExpressionPiece> pieces)
        {
            List<Term> temrs = new List<Term>();

            for (int i = 0; i < pieces.Count; i++)
                if (pieces[i].GetTypePiece() == ExpressionPieceType.Term)
                    temrs.Add(new Term((Term)pieces[i]));
            temrs = new Calculator().AddLikeTerms(temrs);
            temrs = new Calculator().GroupLikeTerms(temrs);
            //temrs.Sort();
            //temrs.Reverse();
            return temrs;
        }
        #endregion Factor

        #region Simplify
        public bool AddingSqaureRootSimplifies()
        {
            if (BracePowerNumerator == 2 && BracePowerDenominator == 2)
                return true;

            for (int i = 0; i < this.numerator.Count; i++)
            {
                if (numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)numerator[i];

                    if (current.Power % 2 != 0 && !current.Constant)
                        return false;
                    else if (current.Constant && current.CoEfficient != 1)
                    {
                        if (current.CoEfficient == 0 || (current.Power % 2 != 0 && !IsPerfectSquare(current.CoEfficient)))
                            return false;
                    }
                    if (!current.Constant && current.Power % 2 != 0)
                        return false;
                    //current.Simplify();
                    //numerator[i] = new Term(current);
                }
            }

            for (int i = 0; i < this.denominator.Count; i++)
            {
                if (denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)denominator[i];
                    if (current.Power % 2 != 0 && !current.Constant)
                        return false;
                    else if (current.Constant && current.CoEfficient != 1)
                    {
                        if (current.CoEfficient == 0 || (current.Power % 2 != 0 && !IsPerfectSquare(current.CoEfficient)))
                            return false;
                    }
                    if (!current.Constant && current.Power % 2 != 0)
                        return false;
                }
            }
            return true;
        }
        public bool RaisingEverythingToPowerTwoSimplifies()
        {
            if (rootPowerNumerator == 2 && rootPowerDenominator == 2)
                return true;

            for (int i = 0; i < this.numerator.Count; i++)
            {
                if (numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)numerator[i];
                    if (current.Root == 2)
                        return true;
                }
            }

            for (int i = 0; i < this.denominator.Count; i++)
            {
                if (denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)denominator[i];
                    if (current.Root == 2)
                        return true;
                }
            }
            return false;
        }
        public bool Simplify(bool isInAnEquation)
        {
            bool simplified = false;
            SimplifyAllTerms(ref simplified);
            PowerRootSimplification(ref simplified);
            RootSimplification(ref simplified);
            SimplyPowerToInner(ref simplified);
            //RemoveBraceDenominator();

            return simplified;
        }
        Calculator calc = new Calculator();
        private List<Term> MultiplyLike(List<Term> terms)
        {
            List<Term> answer = new List<Term>();
            while (ThereAreLikeTerms(terms))
            {
                int[] like = GetTheseLikeTerms(terms);
                terms[like[0]] = MultiplyLike(new Term(terms[like[0]]), new Term(terms[like[1]]));
                terms[like[1]] = null;
            }

            for (int i = 0; i < terms.Count; i++)
                if (terms[i] != null)
                    answer.Add(terms[i]);
            return answer;

        }
        private Term MultiplyLike(Term one, Term two)//doesnt do coEfficients
        {
            string sign = (one.Power > two.Power) ? one.PowerSign : two.PowerSign;
            one.Power = int.Parse((one.PowerSign + one.Power).ToString()) + int.Parse((two.PowerSign + two.Power.ToString()));
            one.PowerSign = sign;

            if (one.Power == 0)
            {
                return new Term(1);
            }
            return one;
        }
        private bool ThereAreLikeTerms(List<Term> terms)
        {
            for (int i = 0; i < terms.Count; i++)
                for (int k = 0; k < terms.Count; k++)
                    if (i != k && (terms[i] != null && terms[k] != null)
                        && AreLikeInContext(terms[i], terms[k]))
                        return true;
            return false;
        }
        private int[] GetTheseLikeTerms(List<Term> terms)
        {
            for (int i = 0; i < terms.Count; i++)
                for (int k = 0; k < terms.Count; k++)
                    if (i != k && (terms[i] != null && terms[k] != null)
                        && AreLikeInContext(terms[i], terms[k]))
                        return new int[2] { i, k };
            return null;
        }
        private bool AreLikeInContext(Term one, Term two)
        {
            if (one.TermBase == two.TermBase)
                return true;
            else return false;
        }

        public void KillFraction()
        {
            List<Term> _topStart = calc.GetTerms1(this.numerator);
            List<Term> _botStart = calc.GetTerms1(this.denominator);
            List<Term> _allMyTerms = new List<Term>();
            _allMyTerms.AddRange(_topStart);

            for (int i = 0; i < _botStart.Count; i++)
                _botStart[i].PowerSign = (_botStart[i].PowerSign == "-") ? "+" : "-";

            _allMyTerms.AddRange(_botStart);

            _allMyTerms = MultiplyLike(_allMyTerms);

            List<Term> finishWithTop = new List<Term>();
            List<Term> finishWithBot = new List<Term>();

            for (int i = 0; i < _allMyTerms.Count; i++)
            {
                if (_allMyTerms[i].PowerSign == "+")
                    finishWithTop.Add(_allMyTerms[i]);
                else
                {
                    _allMyTerms[i].PowerSign = "+";
                    _allMyTerms[i].Power = (_allMyTerms[i].Power < 0) ? _allMyTerms[i].Power * -1 : _allMyTerms[i].Power;
                    finishWithBot.Add(_allMyTerms[i]);
                }
            }

            this.numerator = new List<IExpressionPiece>();
            for (int i = 0; i < finishWithTop.Count; i++)
                numerator.Add(finishWithTop[i]);
            this.denominator = new List<IExpressionPiece>();
            for (int i = 0; i < finishWithBot.Count; i++)
                denominator.Add(finishWithBot[i]);

            if (this.numerator.Count == 0)
                this.numerator.Add(new Term(1));
        }
        public void RemoveBraceDenominator()
        {
            if (!NoBraces(denominator))
                RemoveBraces(ref denominator, true);
        }
        public bool CanRemoveBraces()
        {
            if (!NoBraces(numerator) || !NoBraces(denominator))
                return true;
            return false;
        }
        private void SimplyPowerToInner(ref bool simplified)
        {
            if (bracePowerNumerator > 1)
            {

                for (int i = 0; i < this.numerator.Count; i++)
                {
                    if (numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    {
                        Term current = (Term)numerator[i];
                        if (current.Power != 1)
                            current.Power = current.Power + bracePowerNumerator;
                        else current.Power = bracePowerNumerator;
                        numerator[i] = new Term(current);
                    }
                }
                bracePowerNumerator = 1;
                simplified = true;
            }

            if (bracePowerDenominator > 1)
            {

                for (int i = 0; i < this.denominator.Count; i++)
                {
                    if (denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                    {
                        Term current = (Term)denominator[i];
                        if (current.Power != 1)
                            current.Power = current.Power + bracePowerDenominator;
                        else current.Power = BracePowerDenominator;
                        denominator[i] = new Term(current);
                    }
                }
                bracePowerDenominator = 1;
                simplified = true;
            }
        }
        private void SimplifyAllTerms(ref bool simplified)
        {
            for (int i = 0; i < this.numerator.Count; i++)
            {
                if (numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)numerator[i];
                    if (current.Simplify())
                    {
                        if (!simplified)
                            simplified = true;
                        numerator[i] = new Term(current);
                    }
                }
            }

            for (int i = 0; i < this.denominator.Count; i++)
            {
                if (denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)denominator[i];
                    if (current.Simplify())
                        simplified = true;
                    denominator[i] = new Term(current);
                }
            }
        }
        private void PowerRootSimplification(ref bool simplified)
        {
            if (rootPowerNumerator == bracePowerNumerator && bracePowerNumerator != 1)
            {
                //cancel out power for the whole expression becomes 2/2
                this.bracePowerNumerator = 1;
                this.rootPowerNumerator = 1;
                simplified = true;
            }

            if (rootPowerDenominator == bracePowerDenominator && bracePowerDenominator != 1)
            {
                //cancel out power for the whole expression becomes 2/2
                this.bracePowerDenominator = 1;
                this.rootPowerDenominator = 1;
                simplified = true;
            }

        }
        private void RootSimplification(ref bool simplified)
        {
            if ((rootPowerNumerator != 1 && bracePowerNumerator == 1) && (rootPowerDenominator != 1 && bracePowerDenominator == 1))
            {
                if (CanSimplifyRoot(rootPowerNumerator, rootPowerDenominator))
                {
                    SimplifyRoot(rootPowerNumerator, rootPowerDenominator);
                    simplified = true;
                }
            }
        }
        private Term SimplifyRoot(Term current, int value)
        {
            if (!current.Constant)
            {
                current.Power = current.Power / 2;
                return current;
            }
            else
            {
                if (current.Constant && value == 2)
                {
                    current.CoEfficient = (int)Math.Sqrt(current.CoEfficient);
                    return current;
                }
                else if (current.Constant && value != 2)
                    throw new NotImplementedException();
            }
            return null;
        }
        private bool CanSimplifyRoot(int value, int value2)
        {
            for (int i = 0; i < this.numerator.Count; i++)
            {
                if (numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)numerator[i];

                    if (current.Root != 1)
                        return false;
                    else if (current.Power != value && !current.Constant)
                        return false;

                    else if (current.Constant && value == 2)
                    {
                        if (!IsPerfectSquare(current.CoEfficient))
                            return false;
                    }
                    else if (current.Constant && value != 2)
                        throw new NotImplementedException();
                }
            }

            for (int i = 0; i < this.denominator.Count; i++)
            {
                if (denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)denominator[i];
                    if (current.Root != 1)
                        return false;
                    else if (current.Power != value2 && !current.Constant)
                        return false;

                    else if (current.Constant && value2 == 2)
                    {
                        if (!IsPerfectSquare(current.CoEfficient))
                            return false;
                    }
                    else if (current.Constant && value2 != 2)
                        throw new NotImplementedException();
                }
            }

            return true;
        }
        private void SimplifyRoot(int value, int value2)
        {
            for (int i = 0; i < this.numerator.Count; i++)
            {
                if (numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)numerator[i];
                    numerator[i] = SimplifyRoot(new Term(current), value);
                }
            }

            for (int i = 0; i < this.denominator.Count; i++)
            {
                if (denominator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    Term current = (Term)denominator[i];
                    denominator[i] = SimplifyRoot(new Term(current), value2);
                }
            }
        }
        private bool IsPerferctSquare(uint number)
        {
            return (Math.Sqrt(number) % 1 == 0);
        }
        #endregion Simplify

        #region Log Support
        public List<IAlgebraPiece> Expand(LogTerm hostCopy)
        {
            if (HasDivisor())
                return Divisor(hostCopy);
            else if (CanSplitTerms())
                return SplitTerms(hostCopy);
            else if (CanExpandTerm(hostCopy))
                return ExpandTerm(hostCopy);
            return new List<IAlgebraPiece>(new IAlgebraPiece[] { hostCopy });
        }
        public bool CanExpand(LogTerm hostCopy)
        {
            if (HasDivisor() || CanSplitTerms() || CanExpandTerm(hostCopy))//|| CanExpandThis()) what do do with that?
                return true;
            return false;
        }
        private bool CanExpandThis()
        {
            if (this.bracePowerNumerator > 1)
                return true;
            return false;
        }
        private bool CanExpandTerm(LogTerm hostCopy)
        {
            if (this.numerator.Count > 1)
                return false;
            if (((Term)this.numerator[0]).Power > 1)
                return true;
            if (!((Term)this.numerator[0]).Constant && ((Term)this.numerator[0]).CoEfficient > 1)
                return true;

            if (this.numerator[0].GetTypePiece() == ExpressionPieceType.Term &&
                ((Term)this.numerator[0]).Constant
                && IsNumber(hostCopy.LogBase.ToString()))
            {
                int newBase = int.Parse(hostCopy.LogBase.ToString());
                int oldBase = ((Term)this.numerator[0]).CoEfficient;

                double check = Math.Log(oldBase, newBase);

                if (isGood(check))
                    return true;

                //if value check has decimals (2.1) then ignore esle (2.0) return true

            }
            else if (this.numerator[0].GetTypePiece() == ExpressionPieceType.Term
                && hostCopy.LogBase == ((Term)this.numerator[0]).TermBase)
            {
                return true;
            }
            return false;
        }
        private bool isGood(double value)
        {
            string answerAssist = value.ToString();

            if (answerAssist.Length == 1)
                return true;
            return false;
        }
        private bool IsNumber(string value)
        {
            int answer;
            return int.TryParse(value, out answer);

        }
        private List<IAlgebraPiece> ExpandTerm(LogTerm hostCopy)
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>();
            Term current = new Term((Term)this.numerator[0]);
            Expression expression = new Expression();
            expression.numerator.Add(new Term((Term)this.numerator[0]));
            Expression expression2 = new Expression();
            expression2.numerator.Add(new Term((Term)this.numerator[0]));

            if (!current.Constant && current.CoEfficient > 1)
            {

                expression.numerator[0] = new Term(((Term)expression.numerator[0]).CoEfficient);
                answer.Add(new LogTerm(hostCopy.Coefficient, hostCopy.LogBase, new Expression(expression)));

                Term term = new Term((Term)expression2.numerator[0]);
                term.CoEfficient = 1;
                expression2.numerator[0] = new Term(term);
                answer.Add(new Sign(SignType.Add));
                answer.Add(new LogTerm(hostCopy.Coefficient, hostCopy.LogBase, new Expression(expression2)));
                return answer;
            }

            if (current.Power > 1)
            {
                int power = current.Power;
                current.Power = 1;
                expression.numerator[0] = current;
                answer.Add(new LogTerm(power, hostCopy.LogBase, expression));
                return answer;
            }
            if (((Term)this.numerator[0]).Constant && int.Parse(hostCopy.LogBase.ToString()) == ((Term)this.numerator[0]).CoEfficient
                && ((Term)this.numerator[0]).Power == 1)
            {
                int coEfficient = hostCopy.Coefficient *
                    ((Term)this.numerator[0]).Power;

                answer.Add(new Term(coEfficient));
                return answer;
            }

            if (this.numerator[0].GetTypePiece() == ExpressionPieceType.Term &&
                ((Term)this.numerator[0]).Constant
                && IsNumber(hostCopy.LogBase.ToString())
                && int.Parse(hostCopy.LogBase.ToString()) != ((Term)this.numerator[0]).CoEfficient)
            {
                int newBase = int.Parse(hostCopy.LogBase.ToString());
                int oldBase = ((Term)this.numerator[0]).CoEfficient;

                double check = Math.Log(oldBase, newBase);

                if (isGood(check))
                {
                    current.Power = (int)check;
                    current.CoEfficient = int.Parse(hostCopy.LogBase.ToString());
                    expression.numerator[0] = current;
                    answer.Add(new LogTerm(hostCopy.LogBase, hostCopy.LogBase, expression));
                    return answer;
                }
                else
                    return new List<IAlgebraPiece>(new IAlgebraPiece[] { hostCopy });

                //if value check has decimals (2.1) then ignore esle (2.0) return true

            }
            else if (this.numerator[0].GetTypePiece() == ExpressionPieceType.Term
                && hostCopy.LogBase == ((Term)this.numerator[0]).TermBase
                && ((Term)this.numerator[0]).Power == 1)
            {
                //return true;

                answer.Add(new Term(hostCopy.Coefficient));
                return answer;
            }

            return answer;
        }
        private bool CanSplitTerms()//add for multiples inside the term
        {
            if (!NoBraces(this.numerator))
                return false;
            else
            {
                for (int i = 0; i < this.numerator.Count; i++)
                {
                    if (this.numerator[i].GetTypePiece() == ExpressionPieceType.Sign
                        && ((Sign)this.numerator[i]).SignType == SignType.Multiply)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private List<IAlgebraPiece> SplitTerms(LogTerm hostCopy)//add for multiples inside the term
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>();

            Expression before = new Expression();
            Expression after = new Expression();
            bool found = false;

            for (int i = 0; i < this.numerator.Count; i++)
            {

                if (!found && this.numerator[i].GetTypePiece() == ExpressionPieceType.Sign
                    && ((Sign)this.numerator[i]).SignType == SignType.Multiply)
                {
                    found = true;
                }

                if (!found)
                    before.numerator.Add(this.numerator[i]);
                else after.numerator.Add(this.numerator[i]);
            }
            answer.Add(new LogTerm(hostCopy.Coefficient, hostCopy.LogBase, before));
            answer.Add(new Sign(SignType.Add));
            answer.Add(new LogTerm(hostCopy.Coefficient, hostCopy.LogBase, after));

            return answer;
        }
        public bool HasDivisor()
        {
            List<IExpressionPiece> piece = new List<IExpressionPiece>(this.denominator);
            List<Term> terms = RemoveBraces(piece);

            for (int i = 0; i < terms.Count; i++)
                if (!terms[i].Constant || terms[i].CoEfficient != 1)
                    return true;
            return false;
        }
        private List<IAlgebraPiece> Divisor(LogTerm hostCopy)
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>();

            Expression top = new Expression();
            top.numerator = new List<IExpressionPiece>(hostCopy.Power.numerator);

            Expression bottom = new Expression();
            bottom.numerator = new List<IExpressionPiece>(hostCopy.Power.denominator);

            answer.Add(new LogTerm(hostCopy.Coefficient, hostCopy.LogBase, top));
            answer.Add(new Sign(SignType.Subtract));
            answer.Add(new LogTerm(hostCopy.Coefficient, hostCopy.LogBase, bottom));

            return answer;
        }
        #endregion Log Support

        #endregion methods

        public EquationPieceType GetEquationPieceType()
        {
            return EquationPieceType.Expression;
        }
    }
    public class Sign : ISimplificationPiece, iEquationPiece, IExpressionPiece, IAlgebraPiece
    {
        #region Properties
        bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        private SignType signType;

        public SignType SignType
        {
            get { return signType; }
            set { signType = value; }
        }


        #endregion Properties

        #region Constructor

        public Sign(string sign)
        {
            if (sign == null)
                this.signType = MathBase.SignType.Add;

            else
            {

                switch (sign.Trim().ToLower())
                {
                    case "+":
                        this.signType = SignType.Add;
                        break;
                    case "-":
                        this.signType = SignType.Subtract;
                        break;
                    case "/":
                        this.signType = SignType.Divide;
                        break;
                    case "*":
                        this.signType = SignType.Multiply;
                        break;
                    case "=":
                        this.signType = SignType.equal;
                        break;
                    case ">":
                        this.signType = SignType.greater;
                        break;
                    case "<":
                        this.signType = SignType.smaller;
                        break;
                    case ">=":
                        this.signType = SignType.greaterEqual;
                        break;
                    case "<=":
                        this.signType = SignType.smallerEqual;
                        break;
                    case "OR":
                        this.signType = SignType.Or;
                        break;
                }
            }
        }

        public Sign(SignType type)
        {
            this.signType = type;
        }


        #endregion Constructor

        #region Methods

        public string Key()
        {
            if (this.signType == MathBase.SignType.Add)
                return "+";
            else if (this.signType == MathBase.SignType.Subtract)
                return "-";
            else if (this.signType == MathBase.SignType.Divide)
                return "/";
            else if (this.signType == MathBase.SignType.equal)
                return "=";
            else if (this.signType == MathBase.SignType.greater)
                return ">";
            else if (this.signType == MathBase.SignType.greaterEqual)
                return ">=";
            else if (this.signType == MathBase.SignType.Multiply)
                return "*";
            else if (this.signType == MathBase.SignType.smaller)
                return "<";
            else if (this.signType == MathBase.SignType.smallerEqual)
                return "<=";
            throw new NotImplementedException();
        }

        public SignType GetTypeSign()
        {
            return this.signType;
        }

        public EquationPieceType GetEquationPieceType()
        {
            return EquationPieceType.Sign;
        }


        public ExpressionPieceType GetTypePiece()
        {
            return ExpressionPieceType.Sign;
        }

        public Type GetType1()
        {
            return Type.Sign;
        }


        public SimplificationPieceType GetSimplificationType()
        {
            return SimplificationPieceType.Sign;
        }

        #endregion Methods

    }
    public class Brace : IExpressionPiece, IAlgebraPiece
    {
        #region Properties

        bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        char _key;

        public char Key
        {
            get { return _key; }
            set { _key = value; }
        }
        int _power = 0;

        public int Power
        {
            get { return _power; }
            set { _power = value; }
        }

        #endregion Properties

        #region Constructor

        public Brace(char key, bool Selected)
        {
            _key = key;
        }
        public Brace(char key)
        {
            _key = key;
        }
        public Brace(int power)
        {
            _key = ')';
            _power = power;
        }

        #endregion Constructor

        #region Methods


        public ExpressionPieceType GetTypePiece()
        {
            return ExpressionPieceType.Brace;
        }

        public Type GetType1()
        {
            return Type.Brace;
        }
        #endregion Methods

    }
    public interface IAlgebraPiece
    {
        Type GetType1();
    }
    public interface IExpressionPiece
    {
        ExpressionPieceType GetTypePiece();
    }
    public enum MathFunction
    {
        Add, Subtract, Mutliply, Divide, None
    }
    public enum Type
    {
        Term, Expression, Equation, Sign, Brace, Log
    }
    public enum ExpressionPieceType
    {
        Term, SequenceTerm, Sign, Brace, Log
    }
    public enum EquationPieceType
    {
        Sign, Expression
    }
    public enum SignType
    {
        None, Add, Subtract, Divide, Multiply, equal,
        greater, smaller, greaterEqual, smallerEqual, notEqual, Or
    }

   
}
