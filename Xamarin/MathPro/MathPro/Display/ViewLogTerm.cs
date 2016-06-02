using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public class ViewLogTerm : LinearLayout 
    {
        #region Properties

        readonly LogTerm _myTerm;
		private readonly Helper _helper = new Helper();
        private readonly Context _context;

        #endregion Properties

        #region Constructor

        public ViewLogTerm(LogTerm term, Context context)
            :base(context)
        {
            _myTerm = term;
            _context = context;
            Prepare();
        }
        public ViewLogTerm(LogTerm term, ref DispatcherTimer timer, Context context)
            :base(context)
        {
            _myTerm = term;
            _context = context;
            Prepare(ref timer);
        }

        #endregion Constructor

        #region Methods
        private void Prepare()
        {
            //LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, 80);
            if (_myTerm.Coefficient != 1)
                AddView(GetCoEfficient());

            if (_myTerm.IsComplete || _myTerm.ShowLog)
                AddView(GetLog());
            if (_myTerm.IsComplete || _myTerm.ShowBase)
                AddView(GetLogBase());
            if (_myTerm.IsComplete)
                if (_myTerm.Power != null && _myTerm.Power.Numerator != null
                    && _myTerm.Power.Numerator.Count > 0)
                    AddView(GetPower());
        }

        private void Prepare(ref DispatcherTimer timer)
        {
            if (_myTerm.Coefficient != 1)
                AddView(GetCoEfficient());

            if (_myTerm.IsComplete || _myTerm.ShowLog)
                AddView(GetLog());
            if (_myTerm.IsComplete || _myTerm.ShowBase)
                AddView(GetLogBase());
            if (_myTerm.IsComplete)
                if (_myTerm.Power != null && _myTerm.Power.Numerator != null
                    && _myTerm.Power.Numerator.Count > 0)
                    AddView(GetPower(ref timer));
        }

        private LinearLayout GetCoEfficient()
        {
            var panel = new LinearLayout(_context) {LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(30))};

            foreach (var myChar in _myTerm.Coefficient.ToString(CultureInfo.InvariantCulture))
            {
                var grid = new RelativeLayout(_context)
                {
                    LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(30))
                };
                var inner = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(_helper.Factor(15), _helper.Factor(20)) {TopMargin = -10}};
                inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(myChar));//.Background = GetBackGround(myChar.ToString());
                grid.AddView(inner);
                panel.AddView(grid);
            }

            return panel;
        }
        private RelativeLayout GetLog()
        {
            var grid = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(_helper.Factor(25), _helper.Factor(30))};
            var inner = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(40))};
            inner.SetBackgroundResource(Resource.Drawable.Log);//.Background = GetBackGround("log");
            grid.AddView(inner);
            return grid;
        }
        private RelativeLayout GetLogBase()
        {

            var grid = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(_helper.Factor(20), _helper.Factor(30)) };
	        var myParams = new RelativeLayout.LayoutParams(_helper.Factor(12), _helper.Factor(11));// {TopMargin = 45};
			myParams.AddRule(LayoutRules.AlignParentBottom);
            var inner = new RelativeLayout(_context)
            {
                LayoutParameters = myParams
            };

            inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(_myTerm.LogBase));//.Background = GetBackGround(_myTerm.LogBase.ToString());
            grid.AddView(inner);
            return grid;
        }
        private LinearLayout GetPower()
        {
            var panel = new LinearLayout(_context)
            {
                LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(30))
            };

            if (_myTerm.Power != null)
            {
                if (_myTerm.Power.Numerator.Count > 1)
                    panel.AddView(GetBrace(true));

                for (var i = 0; i < _myTerm.Power.Numerator.Count; i++)
                {
                    if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    {
                        if (i != 0 && _myTerm.Power.Numerator[i - 1].GetTypePiece() != ExpressionPieceType.Brace)
                        {
                            panel.AddView(GetInnerSign(new Sign(((Term)_myTerm.Power.Numerator[i]).Sign)));
                        }

                        var pieces = GetPowerElement((Term)_myTerm.Power.Numerator[i]);

                        for (int k = 0; k < pieces.Count; k++)
                            panel.AddView(pieces[k]);

                    }
                    else if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                    {
                        if (((Brace)_myTerm.Power.Numerator[i]).Key == ')')
                            panel.AddView(GetBrace(false));
                        else panel.AddView(GetBrace(true));
                    }
                    else throw new NotImplementedException();
                }

                if (_myTerm.Power.Numerator.Count > 1)
                    panel.AddView(GetBrace(false));
            }
            return panel;
        }
        private LinearLayout GetPower(ref DispatcherTimer timer)
        {
            var panel = new LinearLayout(_context) {LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(35))};

            if (_myTerm.Power != null)
            {
                //if (_myTerm.Power.Numerator.Count > 1)
                //panel.AddView(GetBrace(true));

                for (int i = 0; i < _myTerm.Power.Numerator.Count; i++)
                {
                    if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Term)
                    {
                        if ((i != 0 && _myTerm.Power.Numerator[i - 1].GetTypePiece() != ExpressionPieceType.Brace)
                            || ((Term)_myTerm.Power.Numerator[i]).Joke)
                        {
                            panel.AddView(GetInnerSign(new Sign(((Term)_myTerm.Power.Numerator[i]).Sign)));
                        }


                        List<View> pieces = GetPowerElement((Term)_myTerm.Power.Numerator[i]);

                        if (((Term)_myTerm.Power.Numerator[i]).Joke == false)
                            for (int k = 0; k < pieces.Count; k++)
                                panel.AddView(pieces[k]);

                    }
                    else if (_myTerm.Power.Numerator[i].GetTypePiece() == ExpressionPieceType.Brace)
                    {
                        if (((Brace)_myTerm.Power.Numerator[i]).Key == ')')
                            panel.AddView(GetBrace(false));
                        else panel.AddView(GetBrace(true));
                    }
                    else throw new NotImplementedException();
                }

                //if (_myTerm.Power.Numerator.Count > 1)
                //panel.AddView(GetBrace(false));
            }
            return panel;
        }
        private List<View> GetPowerElement(Term term)
        {
            var answer = new List<View>();

            if (term.CoEfficient > 1 || term.Constant)
            {
                answer.AddRange(PowerElementBase(term.CoEfficient.ToString(CultureInfo.InvariantCulture)));
            }

            if (term.Constant == false)
            {
                answer.AddRange(PowerElementBase(term.TermBase.ToString(CultureInfo.InvariantCulture)));
            }

            if (term.Power > 1)
            {
                answer.AddRange(PowerElementBasePower(term.Power.ToString(CultureInfo.InvariantCulture)));
            }
            return answer;
        }
        private IEnumerable<View> PowerElementBasePower(string value)
        {
            var answer = new List<View>();

            foreach (var myChar in value)
            {
                var grid = new RelativeLayout(_context)
                {
                    LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(20)) {TopMargin = -3}
                };
                var inner = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(_helper.Factor(10), _helper.Factor(11))};
                inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(myChar));//.Background = GetBackGround(myChar.ToString());
                grid.AddView(inner);
                answer.Add(grid);
            }
            return answer;
        }
        private IEnumerable<View> PowerElementBase(string value)
        {
            var answer = new List<View>();

            foreach (var myChar in value)
            {
                var grid = new RelativeLayout(_context)
                {
                    LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(35))
                };
                var inner = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(_helper.Factor(15), _helper.Factor(20)) {TopMargin = -1}};
                inner.SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar(myChar));//.Background = GetBackGround(myChar.ToString());
                grid.AddView(inner);
                answer.Add(grid);
            }
            return answer;
        }
        private RelativeLayout GetInnerSign(Sign sign)
        {
            var grid = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(20))};
	        var myParams = new RelativeLayout.LayoutParams(15, 5);
			myParams.AddRule(LayoutRules.CenterInParent);
			var inner = new RelativeLayout(_context) { LayoutParameters = myParams };
            inner.SetBackgroundResource(_utility.FixSign(sign));//.Background = GetBackGround(FixSign(sign));
            grid.AddView(inner);
            return grid;
        }

        readonly TermUtility _utility = new TermUtility();

        private RelativeLayout GetBrace(bool opening)
        {
            var grid = new RelativeLayout(_context)
            {
                LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, _helper.Factor(20))
            };
            var inner = new RelativeLayout(_context) {LayoutParameters = new LayoutParams(_helper.Factor(10), _helper.Factor(15)) {TopMargin = -1}};
            inner.SetBackgroundResource(opening ? Resource.Drawable.openBrace : Resource.Drawable.closeBrace);

            grid.AddView(inner);
            return grid;

        }


        #endregion Methods
    }
}