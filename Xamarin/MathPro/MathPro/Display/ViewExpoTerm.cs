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
    public class ViewExpoTerm : LinearLayout
    {
        #region Properties

        private Context _context;
		private readonly Helper _helper = new Helper();
        List<SelectionCursor> _mySelect = new List<SelectionCursor>();

        public List<SelectionCursor> MySelect
        {
            get { return _mySelect; }
            set { _mySelect = value; }
        }

        #endregion Properties

        #region Constructor

        public ViewExpoTerm(ExponentialTerm term, Context context)
            :base(context)
        {
            _context = context;
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
        public ViewExpoTerm(ExponentialTerm term, ref DispatcherTimer timer, Context context)
            :base(context)
        {
            _context = context;
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
            LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent,_helper.Factor(40));
        }
        private void Attach(IReadOnlyList<View> pieces)
        {
            for (var i = 0; i < pieces.Count; i++)
                AddView(pieces[i]);
        }
        private List<View> ExpoStatus(ExponentialTerm term)
        {
            var answer = term.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(value => 
                GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))).Cast<View>().ToList();

            answer.Add(GetPower(term.Power));

            return answer;
        }
        private List<View> ExpoStatus(ExponentialTerm term, ref DispatcherTimer timer)
        {
            var answer = term.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(value => 
                GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))).Cast<View>().ToList();


            answer.Add(GetPower(term.Power));

            return answer;
        }
        private List<View> LogStatus(ExponentialTerm term)
        {
            var answer = new List<View> {GetLog()};

            answer.AddRange(term.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(value =>
                GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))));
            if (term.Power != null && term.Power.Numerator != null)
                answer.Add(GetPower(term.Power));

            return answer;
        }
        private List<View> LogWithDivStatus(ExponentialTerm term)
        {
            var answer = new List<View> {GetLog()};
            answer.AddRange(term.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(value => 
                GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))));


            answer.Add(GetBaseSign(new Sign(SignType.Divide)));

            answer.Add(GetLog());

            answer.AddRange(term.Divisor.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(value => 
                GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))));

            return answer;
        }
        private List<View> DoubleStatus(ExponentialTerm term)
        {
            var answer = new List<View>();
            var value = Math.Round(term._Double, 5).ToString(CultureInfo.InvariantCulture);

            for (var i = 0; i < value.Length; i++)
            {
                int cuValue;

                if (int.TryParse(value[i].ToString(CultureInfo.InvariantCulture), out cuValue))
                {
                    var pieces = cuValue.ToString(CultureInfo.InvariantCulture);
                    answer.Add(GetTermBase(int.Parse(value[i].ToString(CultureInfo.InvariantCulture))));
                }
                else
                {
                    answer.Add(GetDot());
                }
            }
            return answer;
        }
        private List<View> ExpressionStatus(ExponentialTerm term)
        {
            var answer = new List<View>();

            answer.Add(GetBrace(true));
            answer.AddRange(GetExpression(term.Power));
            answer.Add(GetBrace(false));
            answer.Add(GetBrace(true));
            answer.Add(GetLog());
            answer.AddRange(term.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(value => 
                GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))));
            answer.Add(GetBrace(false));
            return answer;
        }
        private List<View> ExpressionWithoutLogStatus(ExponentialTerm term)
        {
            var answer = new List<View> {GetBrace(true)};

            answer.AddRange(GetExpression(term.Power));
            answer.Add(GetBrace(false));
            answer.Add(GetBrace(true));
            answer.Add(GetTermBase(1));
            answer.Add(GetBrace(false));
            return answer;
        }
        private List<View> VariableStatus(ExponentialTerm term)
        {
            var answer = new List<View>();

            //answer.Add(GetBrace(true));
            answer.AddRange(GetExpression(term.Power));
            //answer.Add(GetBrace(false));
            //answer.Add(GetBrace(true));
            //answer.Add(GetTermBase(1));
            //answer.Add(GetBrace(false));
            return answer;
        }
        private List<View> IntegerStatus(ExponentialTerm term)
        {
            var pieces = term.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture);

            var answer = pieces.Select(value => GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))).Cast<View>().ToList();

            if (term.TermBase.Power == 1) return answer;
            var exp = new Expression();
            exp.AddToExpression(true, new Term(term.TermBase.Power));
            answer.Add(GetPower(exp));

            return answer;
        }
        private List<View> IntegerStatus(ExponentialTerm term, ref DispatcherTimer timer)
        {
            var answer = new List<View>();

            var pieces = term.TermBase.CoEfficient.ToString(CultureInfo.InvariantCulture);

            if (!term.Joke && !term.ExpressJoke)
            {
                answer.AddRange(pieces.Select(value => GetTermBase(int.Parse(value.ToString(CultureInfo.InvariantCulture)))));
                _mySelect.Add(new SelectionCursor(SelectedPieceType.BaseExpo, -1,_context));

                if (term.Selected)
                {
                    //_mySelect[_mySelect.Count - 1].Active = true;
                    //answer.Add(_mySelect[_mySelect.Count - 1]);
                }
            }
            else
            {
                _mySelect.Add(new SelectionCursor(SelectedPieceType.BaseExpo, -1,_context));
                //_mySelect[_mySelect.Count - 1].Active = true;
                //answer.Add(_mySelect[_mySelect.Count - 1]);
            }
            return answer;
        }
        private RelativeLayout GetDot()
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(18) / 2, _helper.Factor(81) / 2) };

            var inner = new  RelativeLayout(_context) ;
			var parems = new LayoutParams(_helper.Factor(12) / 2, _helper.Factor(12) / 2) { TopMargin = 29 };
            inner.LayoutParameters = parems;
            inner.SetBackgroundResource(Resource.Drawable.termMultiply);//.Background = GetBackGround("termmultiply");
            grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetLog()
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(61) / 2, _helper.Factor(81) / 2) };
			var inner = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(61) / 2, _helper.Factor(40) / 2) };
            inner.SetBackgroundResource(Resource.Drawable.Log);//.Background = GetBackGround("Log");
            grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetTermBase(int value)
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(40) / 2, _helper.Factor(80) / 2) };
			var inner = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(40) / 2, _helper.Factor(43) / 2) };
            inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(value.ToString(CultureInfo.InvariantCulture)[0]));
            grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetTermBase(string value)
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(40) / 2, _helper.Factor(80) / 2) };
			var inner = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(40) / 2, _helper.Factor(43) / 2) };
            inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(value[0]));
            grid.AddView(inner);
            return grid;
        }
        private LinearLayout GetPower(Expression exp)
        {
            var panel = new  LinearLayout(_context);
			var parems = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(30) / 2) { TopMargin = 0 };
            panel.LayoutParameters = parems;

            for (var i = 0; i < exp.Numerator.Count; i++)
            {
                if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                {
                    if (i != 0 && exp.Numerator[i - 1].GetTypePiece() != ExpressionPieceType.Brace)
                    {
                        panel.AddView(GetPowersSign(new Sign(((Term)exp.Numerator[i]).Sign)));
                    }

                    foreach (View el in GetPowerTerm((Term)exp.Numerator[i]))
                    {
                        panel.AddView(el);
                    }
                }
                else if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Sign)
                {
                    panel.AddView(GetPowersSign((Sign)exp.Numerator[i]));
                }
                else if (exp.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                {
                    if (((Brace)exp.Numerator[i]).Key == '(')
                        panel.AddView(GetBracePower(true));
                    else panel.AddView(GetBracePower(false));
                }
                else throw new NotImplementedException();
            }
            return panel;
        }
        private RelativeLayout GetBracePower(bool opening)
        {

			var inner = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(10) / 2, _helper.Factor(20) / 2) };
            inner.SetBackgroundResource(opening ? Resource.Drawable.openBrace : Resource.Drawable.closeBrace);
            return inner;
        }
        private IEnumerable<View> GetExpression(Expression exp)
        {
            var answer = new List<View>();

            for (var i = 0; i < exp.Numerator.Count; i++)
            {
                switch (exp.Numerator[i].GetTypePiece())
                {
                    case ExpressionPieceType.Term:
                        if (i != 0 && exp.Numerator[i - 1].GetTypePiece() != ExpressionPieceType.Brace)
                        {
                            answer.Add(GetBaseSign(new Sign(((Term)exp.Numerator[i]).Sign)));
                        }
                        answer.AddRange(GetTerm((Term) exp.Numerator[i]));
                        break;
                    case ExpressionPieceType.Sign:
                        answer.Add(GetBaseSign((Sign)exp.Numerator[i]));
                        break;
                    case ExpressionPieceType.Brace:
                        answer.Add(((Brace) exp.Numerator[i]).Key == ')' ? GetBrace(false) : GetBrace(true));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return answer;
        }
        private IEnumerable<View> GetTerm(Term term)
        {
            var answer = new List<View>();

            if (term.Constant || term.CoEfficient > 1)
            {
                answer.AddRange(term.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(cur =>
                    GetTermBase(int.Parse(cur.ToString(CultureInfo.InvariantCulture)))).Cast<View>());
            }
            if (!term.Constant)
            {
                answer.Add(GetTermBase(term.TermBase.ToString(CultureInfo.InvariantCulture)));
            }

            if (term.Power <= 1) return answer;
            var exp = new Expression();
            foreach (var cur in term.Power.ToString(CultureInfo.InvariantCulture))
            {
                exp.Numerator.Add(new Term(int.Parse(cur.ToString(CultureInfo.InvariantCulture))));

            }
            answer.Add(GetPower(exp));

            return answer;
        }
        private IEnumerable<View> GetPowerTerm(Term term)
        {
            var answer = new List<View>();

            if (term.Constant || term.CoEfficient > 1)
            {
                answer.AddRange(term.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(cur => 
                    GetPowerBase(cur.ToString(CultureInfo.InvariantCulture))));
            }
            if (!term.Constant)
            {
                answer.Add(GetPowerBase(term.TermBase.ToString(CultureInfo.InvariantCulture)));
            }

            if (term.Power > 1)
            {
                answer.AddRange(term.Power.ToString(CultureInfo.InvariantCulture).Select(cur => 
                    GetPowersPower(int.Parse(cur.ToString(CultureInfo.InvariantCulture)))));
            }
            return answer;
        }
        private RelativeLayout GetBaseSign(Sign signType)
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(30) / 2, _helper.Factor(80) / 2) };
			var inner = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(20) / 2, _helper.Factor(11) / 2) };
	        inner.SetBackgroundResource(_utility.FixSign(signType));
			//grid.SetPadding(0,0,0,0);
            grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetPowerBase(string sig)
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(20) / 2, _helper.Factor(20) / 2) };
            //Grid inner = new  RelativeLayout(_context) ;
            //inner.Height = 10;
            //inner.Width = 10;
            //inner.Margin = new Thickness(0, -15, 0, 0);
            grid.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(sig[0]));
            //grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetPowersSign(Sign sign)
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(20) / 2, _helper.Factor(20) / 2) };
			grid.SetPadding(0,5,0,0);
			var inner = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(10) / 2, _helper.Factor(7) / 2) };
            inner.SetBackgroundResource(_utility.FixSign((sign)));
            grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetPowersPower(int value)
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(20) / 2, _helper.Factor(20) / 2) };
            var inner = new  RelativeLayout(_context) ;
			var parems = new LayoutParams(_helper.Factor(10), _helper.Factor(10) / 2) { TopMargin = -5 };
            inner.LayoutParameters = parems;
            inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(value.ToString(CultureInfo.InvariantCulture)[0]));
            grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetBrace(bool opening)
        {
			var grid = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(40) / 2, _helper.Factor(80) / 2) };
			var inner = new RelativeLayout(_context) { LayoutParameters = new LayoutParams(_helper.Factor(20) / 2, _helper.Factor(43) / 2) };
            inner.SetBackgroundResource(opening ? Resource.Drawable.openBrace : Resource.Drawable.closeBrace);
            grid.AddView(inner);
            return grid;
        }
        
        private int FixSign(Sign signType)
        {
            return _utility.FixSign(signType);
        }

        readonly TermUtility _utility = new TermUtility();
        public bool Select(SelectedPieceType type1, int index, ref DispatcherTimer timer)
        {
            var answer = false;
            DeSelect();

            for (var i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].MySelectedPieceType != type1 || _mySelect[i].Index != index) continue;
                _mySelect[i].Active = true;
                _mySelect[i].Visibility = ViewStates.Visible;
                //timer.Tick += new EventHandler(Toggle);
                answer = true;
                break;
            }
            return answer;
        }
        private void Toggle(object sender, EventArgs e)
        {
            for (var i = 0; i < _mySelect.Count; i++)
            {
                if (_mySelect[i].Active)
                {
                    _mySelect[i].GetChildAt(0).Visibility = _mySelect[i].GetChildAt(0).Visibility == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
                }
            }
        }
        public void DeSelect()
        {
            for (int i = 0; i < _mySelect.Count; i++)
            {
                _mySelect[i].Active = false;
                _mySelect[i].Visibility = ViewStates.Gone;
            }
        }
        #endregion Methods
    }
}