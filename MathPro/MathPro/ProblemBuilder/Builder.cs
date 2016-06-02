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
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using MathBase;
using System.Windows.Media.Imaging;

namespace MathPro.ProblemBuilder
{
    public interface IProblemBuilder
    {
        EquationType GetEquationType();
    }

    public class History
    {
        #region Properties
        string[] _topBottoms;
        public string[] TopBottoms
        {
            get { return _topBottoms; }
            set { _topBottoms = value; }
        }
        ExpressionLevel _expLevel;
        public ExpressionLevel ExpLevel
        {
            get { return _expLevel; }
            set { _expLevel = value; }
        }
        TermLevel _termLevel;
        public TermLevel TermLevel
        {
            get { return _termLevel; }
            set { _termLevel = value; }
        }
        TermLevel _termLevel2;
        public TermLevel TermLevel2
        {
            get { return _termLevel2; }
            set { _termLevel2 = value; }
        }
        #endregion Properties

        #region Constructor

        public History(string top, string bot, ExpressionLevel expLevel, TermLevel termLevel)
        {
            this._topBottoms =new string[] { top, bot };
            this._termLevel = termLevel;
            this._expLevel = expLevel;
        }
        public History(string top, string bot, TermLevel expLevel, TermLevel termLevel)
        {
            this._topBottoms = new string[] { top, bot };
            this._termLevel = termLevel;
            this._termLevel2 = expLevel;
        }

        #endregion Constructor

        #region Methods

        public bool Changed(History piece)
        {
            if (piece._expLevel != this._expLevel ||
                piece._termLevel != this._termLevel ||
                piece._topBottoms[0] != this._topBottoms[0] ||
                piece._topBottoms[1] != this._topBottoms[1])
                return true;
            else return false;
        }
        public bool Changed2(History piece)
        {
            if (piece._termLevel2 != this._termLevel2 ||
                piece._termLevel != this._termLevel ||
                piece._topBottoms[0] != this._topBottoms[0] ||
                piece._topBottoms[1] != this._topBottoms[1])
                return true;
            else return false;
        }

        #endregion Methods
    }

    public class GeneralBuilder : Grid, IProblemBuilder
    {
        #region Properties

        TermLevel _myTermLevel;
        ExpressionLevel _myExpressionLevel;
        string _top = "";

        public string Top
        {
            get { return _top; }
            set { _top = value; }
        }
        string _bot = "";

        public string Bot
        {
            get { return _bot; }
            set { _bot = value; }
        }
        List<History> myHistory = new List<History>();
        WrapPanel _myDisplayArea;
        EquationType _eqType;

        public EquationType EqType
        {
            get { return _eqType; }
            set { _eqType = value; }
        }
        MathBase.Action _action;

        public MathBase.Action Action
        {
            get { return _action; }
            set { _action = value; }
        }

        #endregion Properties
        #region Save Properties

        string _savedAs = "";

        public string SavedAs
        {
            get { return _savedAs; }
            set { _savedAs = value; }
        }

        #endregion Save Properties

        #region Constructor
        DispatcherTimer timer;

        public DispatcherTimer Timer
        {
            get { return timer; }
            set { timer = value; }
        }
        public GeneralBuilder(ref DispatcherTimer timer, EquationType eqType, MathBase.Action action)
        {
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 251);
            timer.Tick += new EventHandler(ToggleSelected);
            this.timer = timer;
            this.Children.Add(_myDisplayArea);
            this.Children.Add(GetKeyboard());
            this._action = action;
            this._eqType = eqType;
        }
        public GeneralBuilder( EquationType eqType, MathBase.Action action)
        {
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 251);
            //timer.Tick += new EventHandler(ToggleSelected);
            this.Children.Add(_myDisplayArea);
            this.Children.Add(GetKeyboard());
            this._action = action;
            this._eqType = eqType;
        }
        public GeneralBuilder(string savedAs, string topSaved, string botSaved, EventHandler handler, EquationType eqType, MathBase.Action action)
        {
            _savedAs = savedAs;
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 5);
            Top = topSaved.Replace("equals", "=");
            Bot = botSaved.Replace("equals", "=");
            this.Children.Add(_myDisplayArea);
            this.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
            this._action = action;
            this._eqType = eqType;
            Draw();
            AddDeleteKey();
        }
        #endregion Constructor

        #region Methods

        private void AddDeleteKey()
        {
            Grid grid = new Grid();
            grid.Width = 50;
            grid.Height = 50;
            grid.Background = GetBackGround();
            grid.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(DeleteFromStorage);
            grid.Margin = new Thickness(700, 0, 0, 0);
            this.Children.Add(grid);
        }
        private ImageBrush GetBackGround()
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/deleteIcon.png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        private void DeleteFromStorage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SettingsAssistant assistant = new SettingsAssistant();
            assistant.changeSetting(new valuePair(SavedAs + "top", "deleted"));
            Top = "deleted";
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void ToggleSelected(object sender, EventArgs e)
        {
        }
        public void InputHandler(object sender, EventArgs e)
        {
            try
            {
                #region Do this ish
                bool ignore = false;
                History startedWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                IKeyboardItemID caller = (IKeyboardItemID)sender;

                if (_myExpressionLevel == ExpressionLevel.Numerator)
                    _top = _top.Replace("@", "");
                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                    _bot = _bot.Replace("@", "");

                if (caller.GetID() == KeyBoardItemIDType.Other)
                {
                    if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
                    {
                        Undo();
                        ignore = true;
                    }
                    else Change((Other)caller);
                }
                else if (caller.GetID() == KeyBoardItemIDType.NumberVar)
                {
                    #region Number Or Variable
                    //Add.
                    if (_myExpressionLevel == ExpressionLevel.Numerator)
                    {
                        _top = _top.Replace("@", "");
                        if (_top.IndexOf("%") == -1)
                            _top += ((NumberVar)caller).MyVariableNumberValue + "%";
                        else
                            _top = _top.Replace("%", (((NumberVar)caller).MyVariableNumberValue + "%"));
                        Draw(ref timer);
                    }
                    else if (_myExpressionLevel == ExpressionLevel.Denominator)
                    {
                        _bot = _bot.Replace("@", "");
                        _bot = _bot.Trim('%');
                        _bot += ((NumberVar)caller).MyVariableNumberValue + "%";
                        Draw(ref timer);
                    }
                    #endregion Number Or Variable
                }
                else
                {
                    SignCounterAct((KeyboardSign)caller);
                }

                History EndedUpWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                if (!ignore && (startedWith.Changed(EndedUpWith)))
                {
                    myHistory.Add(startedWith);
                }

                #endregion Do this ish
            }
            catch
            {
                Undo();
                errorHandler(this, e);
                //Draw(ref timer);
            }
        }
        private bool Undo()
        {
            if (myHistory.Count > 0)
            {
                for (int i = myHistory.Count - 1; i >= 0; i--)
                {
                    if (myHistory[i] != null)
                    {
                        History previous = myHistory[i];
                        _top = previous.TopBottoms[0];
                        _bot = previous.TopBottoms[1];
                        _myExpressionLevel = previous.ExpLevel;
                        _myTermLevel = previous.TermLevel;
                        myHistory[i] = null;
                        Draw(ref timer);
                        return true;
                    }
                }
            }
            return false;
        }
        private bool SignCounterAct(KeyboardSign sign)
        {
            bool _draw = false;

            #region plus minus Mult

            if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "+%");
                else _top += "+";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "+%");
                else _bot += "+";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "+")
            {
                _top += "'+'";
                _bot += "'+'";
                _draw = true;
            }

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "-%");
                else _top += "-";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "-%");
                else _bot += "-";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "-")
            {
                _top += "'-'";
                _bot += "'-'";
                _draw = true;
            }

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "*")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "*%");
                else _top += "*";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "*")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "*%");
                else _bot += "*";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "*" && _action == MathBase.Action.Simplify)
            {
                _top += "'*'";
                _bot += "'*'";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "/" && _action == MathBase.Action.Simplify)
            {
                _top += "'/'";
                _bot += "'/'";
                _draw = true;
            }
            #endregion plus minus

            #region braces

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "(%");
                else _top += "(%";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "(%");
                else _bot += "(%";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", ")%");
                else _top += ")%";
                _draw = true;
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", ")%");
                else _bot += ")%";
                _draw = true;
            }

            #endregion braces

            #region Split

            else if (_myTermLevel == TermLevel.Base && sign.MyKey == "=" && _top.IndexOf('=') == -1)
            {
                _myExpressionLevel = ExpressionLevel.Numerator;
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "'='%");
                else _top += "'='%";

                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "'='");
                else _bot += "'='";
                _draw = true;
            }

            #endregion Split

            else throw new NotImplementedException();
            if (_draw)
            {
                Draw(ref timer);
                return true;
            }

            return false;
        }
        public bool Change(Other other)
        {
            bool _draw = false;

            #region Small
            if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    if (_top.IndexOf("%") == -1)
                        _top += "^";
                    else _top = _top.Replace("%", "^%");
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    if (_bot.IndexOf("%") == -1)
                        _bot += "^";
                    else _bot = _bot.Replace("%", "^%");
                }

                _myTermLevel = TermLevel.Power;
                _draw = true;
            }
            else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    if (_top.IndexOf("%") == -1)
                        _top += "^";
                    else _top = _top.Replace("%", "^%");
                    _myTermLevel = TermLevel.Base;
                }


                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    if (_bot.IndexOf("%") == -1)
                        _bot += "^";
                    else _bot = _bot.Replace("%", "^%");
                    _myTermLevel = TermLevel.Base;
                }

                _draw = true;
            }
            #endregion Small
            #region Big
            else if (other.MyKeyboardItem == KeyboardItem.Up && _myExpressionLevel != ExpressionLevel.Numerator && _action != MathBase.Action.Plot)
            {
                if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    _myExpressionLevel = ExpressionLevel.MiddleSigns;
                    _bot = _bot.Replace("%", "");
                    _bot = _bot.Replace("@", "");
                    _draw = true;
                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
                {
                    _myExpressionLevel = ExpressionLevel.Numerator;
                    //throw new NotImplementedException(); // remove selection from the Middle sign
                    _top += "%";

                    _draw = true;
                }

                // else if(_myExpressionLevel == ExpressionLevel.MiddleSigns
            }
            else if (other.MyKeyboardItem == KeyboardItem.Down && _myExpressionLevel != ExpressionLevel.Denominator && _action != MathBase.Action.Plot)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    _myExpressionLevel = ExpressionLevel.MiddleSigns;
                    _top = _top.Replace("%", "");
                    _top = _top.Replace("@", "");

                    _draw = true;

                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
                {
                    _myExpressionLevel = ExpressionLevel.Denominator;
                    if (_bot.Trim() != "")
                        _bot += "%";
                    else _bot = "@";
                    _draw = true;
                }
            }
            #endregion Big

            if (_action == MathBase.Action.Plot && other.MyKeyboardItem == KeyboardItem.f)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    if (_top.IndexOf("%") != -1)
                        _top.Replace("%", "f%");
                    else _top += "f";
                    _draw = true;
                }
                else
                {
                    if (_bot.IndexOf("%") != -1)
                        _bot.Replace("%", "f%");
                    else _bot += "f";
                    _draw = true;
                }
            }

            if (_draw)
            {
                Draw(ref timer);
                return true;
            }
            return false;
        }
        EventHandler handler;
        public Keyboard GetKeyboard()
        {
            handler = new EventHandler(InputHandler);
            return new Keyboard(ref handler);
        }
        public void Draw(ref DispatcherTimer timer)
        {
                if (_top.Length > 0 || _bot.Length > 0)
                {
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
                }
                else
                {
                    _top = "@";
                    timer.Tick += new EventHandler(ToggleSelected);
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
                }
        }
        private void Draw()
        {
            if (_top.Length > 0 || _bot.Length > 0)
            {
                _myDisplayArea.Children.Clear();
                _myDisplayArea.Children.Add(new Step(GetEquation()));
            }
        }
        EventHandler errorHandler;
        public EventHandler ErrorHandler
        {
            get { return errorHandler; }
            set { errorHandler = value; }
        }
        public Equation GetEquation()
        {
            if (_top.Trim() == "" && _bot.Trim() != "")
                _top = "+1";
            string rightTop = "";
            string rightBot = "";
            string _top1 = "";
            string _bot1 = "";
            Equation eq = new Equation(SignType.equal);
            if (_top.IndexOf("'='") == -1)
            {
                eq.IsComplete = false;
                _top1 = _top;
                _bot1 = _bot;
            }
            else
            {
                rightTop = _top.Substring(_top.IndexOf("'='") + 3);
                if (rightTop[0] == '%')
                {
                    eq.SplitSelected = true;
                }
                _top1 = _top.Substring(0, _top.IndexOf("'='"));

                rightBot = _bot.Substring(_bot.IndexOf("'='") + 3);
                _bot1 = _bot.Substring(0, _bot.IndexOf("'='"));
                eq.IsComplete = true;
            }

            if ((_top1.IndexOf("'+'") != -1 || _top1.IndexOf("'-'") != -1 || _top1.IndexOf("'*'") != -1 || _top1.IndexOf("'/'") != -1))
            {
                #region More than one expression

                while (_top1.IndexOf("'+'") != -1 || _top1.IndexOf("'-'") != -1 || _top1.IndexOf("'*'") != -1 || _top1.IndexOf("'/'") != -1)
                {
                    string expTop = "";
                    string expBot = "";

                    int indexTop = GetClosestIndex(_top1, GetClosestIndex(_top1, GetClosestIndex(_top1, "'+'", "'-'"), "'*'"), "'/'");
                    int indexBot = GetClosestIndex(_bot1, GetClosestIndex(_bot1, GetClosestIndex(_bot1, "'+'", "'-'"), "'*'"), "'/'");

                    expTop = _top1.Substring(0, indexTop);
                    expBot = _bot.Substring(0, indexBot);

                    eq.Left.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1)));

                    eq.Left.Add(new Sign(_top1.Substring(indexTop, 3).Trim('\'')));

                    _top1 = _top1.Substring(indexTop + 3);
                    _bot1 = _bot1.Substring(indexBot + 3);


                    if (_top1.IndexOf("'+'") == -1 && _top1.IndexOf("'-'") == -1 && _top1.IndexOf("'*'") == -1 && _top1.IndexOf("'/'") == -1)
                    {
                        if ((_top1.Trim() == "" && _bot1.Trim() == ""))
                        {
                            _top1 = "@";
                            _myExpressionLevel = ExpressionLevel.Numerator;
                        }
                        eq.Left.Add(GetExpression(_top1, (_bot1 == "%") ? "@" : _bot1, new Range(0, _top1.Length - 1), new Range(0, _bot1.Length - 1)));
                    }
                }

                #endregion More than one expression
            }
            else
            {
                eq.Left.Add(GetExpression(_top1, _bot1, new Range(0, _top1.Length - 1), new Range(0, _bot1.Length - 1)));
            }

            if (eq.IsComplete)
            {
                if ((rightTop.IndexOf("'+'") != -1 || rightTop.IndexOf("'-'") != -1 || _top1.IndexOf("'*'") != -1 || _top1.IndexOf("'/'") != -1))
                {
                    #region More than one expression

                    while (rightTop.IndexOf("'+'") != -1 || rightTop.IndexOf("'-'") != -1 || _top1.IndexOf("'/'") != -1)
                    {
                        string expTop = "";
                        string expBot = "";

                        int indexTop = GetClosestIndex(rightTop, GetClosestIndex(rightTop, GetClosestIndex(rightTop, "'+'", "'-'"), "'*'"), "'/'");
                        int indexBot = GetClosestIndex(rightBot, GetClosestIndex(rightBot, GetClosestIndex(rightBot, "'+'", "'-'"), "'*'"), "'/'");

                        expTop = rightTop.Substring(0, indexTop);
                        expBot = rightBot.Substring(0, indexBot);

                        eq.Right.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1)));

                        eq.Right.Add(new Sign(rightTop.Substring(indexTop, 3).Trim('\'')));

                        rightTop = rightTop.Substring(indexTop + 3);
                        rightBot = rightBot.Substring(indexBot + 3);


                        if (rightTop.IndexOf("'+'") == -1 && rightTop.IndexOf("'-'") == -1 && rightTop.IndexOf("'*'") == -1 && _top1.IndexOf("'/'") == -1)
                        {
                            if ((rightTop.Trim() == "" && rightBot.Trim() == ""))
                            {
                                rightTop = "@";
                                _myExpressionLevel = ExpressionLevel.Numerator;
                            }
                            eq.Right.Add(GetExpression(rightTop, (rightBot == "%") ? "@" : rightBot, new Range(0, rightTop.Length - 1), new Range(0, rightBot.Length - 1)));
                        }
                    }

                    #endregion More than one expression
                }
                else
                    eq.Right.Add(GetExpression(rightTop, rightBot, new Range(0, rightTop.Length - 1), new Range(0, rightBot.Length - 1)));
            }

            return eq;
        }
        private int GetClosestIndex(string source, string searchOne, string searchTwo)
        {
            if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) != -1)
            {
                return (source.IndexOf(searchTwo) < source.IndexOf(searchOne)) ?
                    source.IndexOf(searchTwo) : source.IndexOf(searchOne);
            }
            else if (source.IndexOf(searchOne) == -1 && source.IndexOf(searchTwo) != -1)
            {
                return source.IndexOf(searchTwo);
            }
            else if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) == -1)
            {
                return source.IndexOf(searchOne);
            }
            return -1;
        }
        private int GetClosestIndex(string source, int prev, string search)
        {
            if (source.IndexOf(search) == -1)
                return prev;
            else if (source.IndexOf(search) < prev)
                return source.IndexOf(search);
            else if (prev == -1)
                return source.IndexOf(search);
            else return prev;
        }
        private Term GetMyTerm(string value)
        {
            value = value.Replace("(%", "");
            value = value.Replace(")%", "");
            if (value.Trim() == "@" || value.Trim('%') == "")
            {
                Term term = new Term(2);
                term.ExpressJoke = true;
                term.Sign = "+";
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            if (value.Length == 1 && (value[0] == '-' || value[0] == '+'))
            {
                Term term = new Term(1);
                term.Sign = value;
                term.Joke = true;
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            else if (value == "-%" || value == "+%")
            {
                Term term = new Term(1);
                term.Sign = value[0].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                term.Joke = true;
                return term;
            }
            else
            {
                #region Term TT
                string[] pieces = value.Split('.');
                Term term = null;
                string sign = "+";
                if (pieces.Length > 1)
                    throw new NotImplementedException();// do something about them multiples...

                if (pieces[0].StartsWith("-"))
                {
                    sign = "-";
                }
                pieces[0] = pieces[0].Trim('-', '+');

                string coeF = "";
                char _base = ' ';
                string power = "";
                int indexPow = -3;
                int indexBase = -3;
                int indexCoe = -3;
                LookingFor lookingFor = LookingFor.myC;
                LookingFor foundFor = LookingFor.None;
                bool outsideAfter = false;
                bool outsideBefore = false;

                for (int i = 0; i < pieces[0].Length; i++)
                {
                    if (lookingFor != LookingFor.None)
                    {
                        #region Looking
                        int valueInt = -1;

                        if (int.TryParse(pieces[0][i].ToString(), out valueInt))
                        {
                            if (lookingFor == LookingFor.myC)
                                coeF += pieces[0][i].ToString();
                            else if (lookingFor == LookingFor.myP)
                                power += pieces[0][i].ToString();
                        }
                        else if (pieces[0][i] == '%')
                        {
                            if (i == 0)
                            {
                                foundFor = lookingFor;
                                outsideBefore = true;
                            }
                            else if (i == pieces[0].Length - 1 && lookingFor != LookingFor.myP)
                            {
                                outsideAfter = true;
                                foundFor = lookingFor;
                            }
                            else
                            {
                                if (lookingFor == LookingFor.myC)
                                    indexCoe = coeF.Length - 1;
                                else if (lookingFor == LookingFor.myP)
                                    indexPow = power.Length - 1;
                            }
                        }
                        else if (pieces[0][i] == '%')
                        {
                            //if (lookingFor == LookingFor.myB)
                            //{
                            //    lookingFor = LookingFor.myP;
                            //    _base += pieces[0][i];
                            //}
                            if (lookingFor == LookingFor.myP)
                            {
                                indexPow = power.Length - 1;
                            }
                            else throw new NotImplementedException();

                        }
                        else if (pieces[0][i] == '^')
                        {
                            if (lookingFor != LookingFor.myP)
                                lookingFor = LookingFor.myP;
                            else
                                lookingFor = LookingFor.None;
                        }
                        else if ("abcdefghijklmnopqrstuvwxyz".IndexOf(pieces[0][i]) != -1)
                        {
                            _base = pieces[0][i];
                            if (i + 1 < pieces[0].Length)
                                if (pieces[0][i + 1] == '%')
                                {
                                    indexBase = 0; ;
                                    i++;
                                }
                            //lookingFor = LookingFor.myP;
                        }
                        #endregion Looking
                    }
                    else
                    {
                        if (pieces[0][i] == '%')
                            outsideAfter = true;
                    }
                }

                SelectionCursor cursor = null;

                if (indexPow != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Power, indexPow);
                else if (indexCoe != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Coefficient, indexCoe);
                if (indexBase != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Base, indexBase);
                else if (outsideBefore)
                    cursor = new SelectionCursor(GetFoundFor(foundFor), -1);
                else if (outsideAfter)
                    cursor = new SelectionCursor(SelectedPieceType.OutSide, -2);

                if (power.Length == 0)
                    power = "1";


                if (coeF.Length != 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length != 0 && _base.ToString().Trim() == "" && power.Length != 0)
                {
                    term = new Term(int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length == 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(power));
                }

                if (cursor != null && term != null)
                {
                    term.Selected = true;
                    term.MySelectedindex = cursor.Index;
                    term.MySelectedPieceType = cursor.MySelectedPieceType;
                }
                term.Sign = sign;

                return term;

                #endregion Term TT
            }
            //throw new NotImplementedException();
        }
        public SelectedPieceType GetFoundFor(LookingFor fo)
        {
            if (fo == LookingFor.myB)
                return SelectedPieceType.Base;
            else if (fo == LookingFor.myC)
                return SelectedPieceType.Coefficient;
            else if (fo == LookingFor.myP)
                return SelectedPieceType.Power;
            else return SelectedPieceType.Base;
        }
        private MathBase.Expression GetExpression(string sourceTop, string sourceBot, Range rangeTop, Range rangeBot)
        {
            MathBase.Expression exp = new MathBase.Expression();

            #region Numerator

            string piece = sourceTop.Substring(rangeTop.Start, 1 + (rangeTop.End - rangeTop.Start));

            if (piece.Length == 2 || piece.Length == 1)
            {
                if (piece[0] == '(' || piece[0] == ')')
                {
                    Brace brace = new Brace(piece[0]);
                    if (piece.IndexOf("%") != -1)
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                }
                else if (piece == "@")
                {
                    Term term = new Term(1);
                    term.ExpressJoke = true;
                    term.Sign = "+";
                    term.Selected = true;
                    term.MySelectedPieceType = SelectedPieceType.Coefficient;
                    term.MySelectedindex = -1;
                    exp.Numerator.Add(term);
                }
                else
                {
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece));
                }
            }
            else
            {
                int startIndex = 0;
                for (int i = 0; i < piece.Length; i++)
                {
                    Act(ref exp, ref piece, i, ref startIndex);
                }
            }

            #endregion Numerator

            #region Denominator

            if (sourceBot.Trim() != "")
            {
                MathBase.Expression dimExp = new MathBase.Expression();
                string pieceBot = sourceBot.Substring(rangeBot.Start, 1 + (rangeBot.End - rangeBot.Start));

                if (pieceBot.Length == 2 || pieceBot.Length == 1)
                {
                    if (pieceBot[0] == '(' || pieceBot[0] == ')')
                    {
                        dimExp.Numerator.Add(new Brace(pieceBot[0]));
                    }
                    else
                    {
                        if (pieceBot.Trim() != "%")
                            dimExp.Numerator.Add(GetMyTerm(pieceBot));
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (int i = 0; i < pieceBot.Length; i++)
                    {
                        Act(ref dimExp, ref pieceBot, i, ref startIndex);
                    }
                }

                exp.Denominator = new List<IExpressionPiece>(dimExp.Numerator);
            }

            #endregion Denominator

            return exp;
        }
        private void Decoy()
        {

        }
        private void Act(ref MathBase.Expression exp, ref string piece, int i, ref int startIndex)
        {
            if (piece[i] == '(' || piece[i] == ')')
            {
                if (startIndex == i)
                {
                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                }
                else if (i != 0 && piece[i - 1] != '(' && piece[i - 1] != ')')
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));

                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                    //there is your fucken term bro!!
                }
            }
            else if ((i != piece.Length - 1 && i != startIndex && (piece[i] == '-' || piece[i] == '+')))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex)));
                startIndex = i;
            }
            else if ((i == piece.Length - 1 && (piece[i] == '-' || piece[i] == '+')))
            {
                if (i - 1 > 0 && piece[i - 1] == ')')
                {
                    startIndex = i + 1;
                }
                else
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    //there is your fucken term bro!!!
                    char part = piece[(i - startIndex) + startIndex];
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                    startIndex = i;
                }
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i] == '+' || piece[i] == '-')))
            {
                throw new NotImplementedException();
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i - 1] == '+' || piece[i - 1] == '-')
                && piece[i] == '%'))
            {
                Term term = new Term(2);
                term.Joke = true;
                term.Sign = piece[i - 1].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                exp.Numerator.Add(term);
                startIndex = i + 1;
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i != startIndex))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                startIndex = i + 1;
            }
        }
        public EquationType GetEquationType()
        {
            return EquationType.General;
        }
        #endregion Methods
    }
    public class InequalitiesBuilder : Grid, IProblemBuilder
    {
        #region Properties
        string _savedAs = "";

        public string SavedAs
        {
            get { return _savedAs; }
            set { _savedAs = value; }
        }
        TermLevel _myTermLevel;
        ExpressionLevel _myExpressionLevel;
        string _top = "";

        public string Top
        {
            get { return _top; }
            set { _top = value; }
        }
        string _bot = "";

        public string Bot
        {
            get { return _bot; }
            set { _bot = value; }
        }
        List<History> myHistory = new List<History>();
        WrapPanel _myDisplayArea;

        #endregion Properties

        #region Constructor
        DispatcherTimer timer;

        public DispatcherTimer Timer
        {
            get { return timer; }
            set { timer = value; }
        }
        public InequalitiesBuilder(ref DispatcherTimer timer)
        {
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 251);
            timer.Tick += new EventHandler(ToggleSelected);
            this.timer = timer;
            this.Children.Add(_myDisplayArea);
            this.Children.Add(GetKeyboard());
        }
        public InequalitiesBuilder(string savedAs, string topSaved, string botSaved, EventHandler handler)
        {
            _savedAs = savedAs;
            Top = topSaved.Replace("equals", "=").Replace("greater", ">").Replace("smaller", "<");
            Bot = botSaved.Replace("equals", "=").Replace("greater", ">").Replace("smaller", "<");
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 15);     
            this.Children.Add(_myDisplayArea);
            //this.Children.Add(GetKeyboard());
            this.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
            Draw();
            AddDeleteKey();
        }
        #endregion Constructor
        private void AddDeleteKey()
        {
            Grid grid = new Grid();
            grid.Width = 50;
            grid.Height = 50;
            grid.Background = GetBackGround();
            grid.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(DeleteFromStorage);
            grid.Margin = new Thickness(700, 0, 0, 0);
            this.Children.Add(grid);
        }
        private ImageBrush GetBackGround()
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/deleteIcon.png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        private void DeleteFromStorage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SettingsAssistant assistant = new SettingsAssistant();
            assistant.changeSetting(new valuePair(SavedAs + "top", "deleted"));
            Top = "deleted";
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
        #region Methods
        EventHandler errorHandler;
        public EventHandler ErrorHandler
        {
            get { return errorHandler; }
            set { errorHandler = value; }
        }
        public void ToggleSelected(object sender, EventArgs e)
        {
        }
        public void InputHandler(object sender, EventArgs e)
        {
            try
            {
                #region food
                bool ignore = false;
                History startedWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                IKeyboardItemID caller = (IKeyboardItemID)sender;

                if (_myExpressionLevel == ExpressionLevel.Numerator)
                    _top = _top.Replace("@", "");
                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                    _bot = _bot.Replace("@", "");

                if (caller.GetID() == KeyBoardItemIDType.Other)
                {
                    if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
                    {
                        Undo();
                        ignore = true;
                    }
                    else Change((Other)caller);
                }
                else if (caller.GetID() == KeyBoardItemIDType.NumberVar)
                {
                    #region Number Or Variable
                    //Add.
                    if (_myExpressionLevel == ExpressionLevel.Numerator)
                    {
                        _top = _top.Replace("@", "");
                        if (_top.IndexOf("%") == -1)
                            _top += ((NumberVar)caller).MyVariableNumberValue + "%";
                        else
                            _top = _top.Replace("%", (((NumberVar)caller).MyVariableNumberValue + "%"));
                        Draw(ref timer);
                    }
                    else if (_myExpressionLevel == ExpressionLevel.Denominator)
                    {
                        _bot = _bot.Replace("@", "");
                        _bot = _bot.Trim('%');
                        _bot += ((NumberVar)caller).MyVariableNumberValue + "%";
                        Draw(ref timer);
                    }
                    #endregion Number Or Variable
                }
                else
                {
                    SignCounterAct((KeyboardSign)caller);
                }

                History EndedUpWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                if (!ignore && (startedWith.Changed(EndedUpWith)))
                {
                    myHistory.Add(startedWith);
                }

                #endregion food
            }
            catch
            {
                Undo();
                errorHandler(this, e);
                //Draw(ref timer);
            }
        }
        private bool Undo()
        {
            if (myHistory.Count > 0)
            {
                for (int i = myHistory.Count - 1; i >= 0; i--)
                {
                    if (myHistory[i] != null)
                    {
                        History previous = myHistory[i];
                        _top = previous.TopBottoms[0];
                        _bot = previous.TopBottoms[1];
                        _myExpressionLevel = previous.ExpLevel;
                        _myTermLevel = previous.TermLevel;
                        myHistory[i] = null;
                        Draw(ref timer);
                        return true;
                    }
                }
            }
            return false;
        }
        private void SignCounterAct(KeyboardSign sign)
        {

            #region plus minus Mult

            if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "+%");
                else _top += "+";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "+%");
                else _bot += "+";
            }
            else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "+")
            {
                _top += "'+'";
                _bot += "'+'";
            }

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "-%");
                else _top += "-";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "-%");
                else _bot += "-";
            }
            else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "-")
            {
                _top += "'-'";
                _bot += "'-'";
            }

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "*")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "*%");
                else _top += "*";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "*")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "*%");
                else _bot += "*";
            }
            else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "*")
            {
                _top += "'*'";
                _bot += "'*'";
            }
            #endregion plus minus

            #region braces

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "(%");
                else _top += "(%";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "(%");
                else _bot += "(%";
            }
            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", ")%");
                else _top += ")%";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", ")%");
                else _bot += ")%";
            }

            #endregion braces

            #region Split
            else if (CanAddAnotherSplit(_top))
            {
                if (_myTermLevel == TermLevel.Base && sign.MyKey == ">" )
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "'>'%");
                    else _top += "'>'%";
                }
                else if (_myTermLevel == TermLevel.Base && sign.MyKey == ">=" )
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "'>='%");
                    else _top += "'>='%";
                }
                else if (_myTermLevel == TermLevel.Base && sign.MyKey == "<=" )
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "'<='%");
                    else _top += "'<='%";
                }
                else if (_myTermLevel == TermLevel.Base && sign.MyKey == "<" )
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "'<'%");
                    else _top += "'<'%";
                }
            }

            #endregion Split

            else throw new NotImplementedException();
            Draw(ref timer);
        }
        private bool CanAddAnotherSplit(string _top)
        {
            string[] pieces = new string[] { ">=", "<=",">", "<"};
            int count=0;

            for (int i = 0; i < _top.Length-1; i++)
            {
                string current = _top.Substring(i, 2);

                for (int k = 0; k < pieces.Length; k++)
                {
                    if (current.IndexOf(pieces[k]) != -1 && current == ">=" || current == "<=")
                    {
                        count++;
                        i += 1;
                        break;
                    }
                    else if (current[0].ToString() == pieces[k])
                        count++;
                }
            }

            for (int k = 0; k < pieces.Length; k++)
            {
                if (_top[_top.Length -1].ToString().IndexOf(pieces[k]) != -1)
                    count++;
            }

            if (count > 1)
                return false;
            else return true;

            
        }
        public void Change(Other other)
        {
            #region Small
            if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    if (_top.IndexOf("%") == -1)
                        _top += "^";
                    else _top = _top.Replace("%", "^%");
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    if (_bot.IndexOf("%") == -1)
                        _bot += "^";
                    else _bot = _bot.Replace("%", "^%");
                }

                _myTermLevel = TermLevel.Power;
                Draw(ref timer);
            }
            else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    if (_top.IndexOf("%") == -1)
                        _top += "^";
                    else _top = _top.Replace("%", "^%");
                    _myTermLevel = TermLevel.Base;
                }


                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    if (_bot.IndexOf("%") == -1)
                        _bot += "^";
                    else _bot = _bot.Replace("%", "^%");
                    _myTermLevel = TermLevel.Base;
                }

                Draw(ref timer);
            }
            #endregion Small

/*
            #region Big
            else if (other.MyKeyboardItem == KeyboardItem.Up && _myExpressionLevel != ExpressionLevel.Numerator)
            {
                if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    _myExpressionLevel = ExpressionLevel.MiddleSigns;
                    _bot = _bot.Replace("%", "");
                    _bot = _bot.Replace("@", "");
                    Draw(ref timer);
                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
                {
                    _myExpressionLevel = ExpressionLevel.Numerator;
                    //throw new NotImplementedException(); // remove selection from the Middle sign
                    _top += "%";

                    Draw(ref timer);
                }

                // else if(_myExpressionLevel == ExpressionLevel.MiddleSigns
            }
            else if (other.MyKeyboardItem == KeyboardItem.Down && _myExpressionLevel != ExpressionLevel.Denominator)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    _myExpressionLevel = ExpressionLevel.MiddleSigns;
                    _top = _top.Replace("%", "");
                    _top = _top.Replace("@", "");

                    Draw(ref timer);

                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
                {
                    _myExpressionLevel = ExpressionLevel.Denominator;
                    if (_bot.Trim() != "")
                        _bot += "%";
                    else _bot = "@";
                    Draw(ref timer);
                }
            }
            #endregion Big
            */
        }
        EventHandler handler;
        public Keyboard GetKeyboard()
        {
            handler = new EventHandler(InputHandler);
            return new Keyboard(ref handler);
        }
        public void Draw(ref DispatcherTimer timer)
        {
            if (_top.Length > 0 || _bot.Length > 0)
            {
                _myDisplayArea.Children.Clear();
                _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
            }
            else
            {
                _top = "@";
                timer.Tick += new EventHandler(ToggleSelected);
                _myDisplayArea.Children.Clear();
                _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
            }
        }
        public void Draw()
        {
            if (_top.Length > 0 || _bot.Length > 0)
            {
                _myDisplayArea.Children.Clear();
                _myDisplayArea.Children.Add(new Step(GetEquation()));
            }
        }
        public Equation GetEquation()
        {
            //_top = "4-3x<5x>=9";
            Equation equation = null;
            List<string[]> splitPoints = GetSplitPoints(_top);

            if (splitPoints.Count > 0)
            {
                equation = new Equation((new Sign(splitPoints[0][1])).SignType);
                equation.Left.Add(GetExpression(_top, "", new Range(0, (int.Parse(splitPoints[0][0]))-1), null));
                string leftOver = _top.Substring((int.Parse(splitPoints[0][0])) + (splitPoints[0][1].Length));

                if (splitPoints.Count > 1)
                {
                    splitPoints = GetSplitPoints(leftOver);
                    
                    equation.Right.Add(GetExpression(leftOver, "", new Range(0, (int.Parse(splitPoints[0][0])) - 1), null));
                    equation.Right.Add(new Sign(splitPoints[0][1]));
                    string leftOverAgain = leftOver.Substring((int.Parse(splitPoints[0][0])) + (splitPoints[0][1].Length));
                    equation.Right.Add(GetExpression(leftOverAgain, "", new Range(0, leftOverAgain.Length -1), null));
                }
                else
                equation.Right.Add(GetExpression(_top, "", new Range(int.Parse(splitPoints[0][0]) 
                    + splitPoints[0][1].Length, _top.Length - 1), null));
            }
             
            else
            {
                equation = new Equation(SignType.greater);
                equation.IsComplete = false;
                equation.Left.Add(GetExpression(_top, "", new Range(0, _top.Length - 1), null));
            }
            return equation;
        }
        public List<string[]> GetSplitPoints(string top)
        {
            List<string[]> answer = new List<string[]>();
            string[] pieces = new string[] {">=", "<=", "<", ">"};

            for (int i = 0; i < top.Length - 1; i++)
            {
                string current = top.Substring(i, 2);

                for (int k = 0; k < pieces.Length; k++)
                {
                    if (current.IndexOf(pieces[k]) != -1 && current == ">=" || current == "<=")
                    {
                        answer.Add(new string[] { i.ToString(), pieces[k] });
                        i += 1;
                        break;
                    }
                    else if (current[0].ToString() == pieces[k])
                    {
                        answer.Add(new string[] { i.ToString(), pieces[k] });
                    }
                }
            }

            for (int k = 0; k < pieces.Length; k++)
            {
                if (top[top.Length - 1].ToString().IndexOf(pieces[k]) != -1)
                    answer.Add(new string[] { (top.Length - 1).ToString(), pieces[k] });
            }

            return answer;
        }
        private int GetClosestIndex(string source, string searchOne, string searchTwo)
        {
            if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) != -1)
            {
                return (source.IndexOf(searchTwo) < source.IndexOf(searchOne)) ?
                    source.IndexOf(searchTwo) : source.IndexOf(searchOne);
            }
            else if (source.IndexOf(searchOne) == -1 && source.IndexOf(searchTwo) != -1)
            {
                return source.IndexOf(searchTwo);
            }
            else if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) == -1)
            {
                return source.IndexOf(searchOne);
            }
            return -1;
        }
        private Term GetMyTerm(string value)
        {
            value = value.Trim('\'');
            value = value.Replace("(%", "");
            value = value.Replace(")%", "");
            if (value.Trim() == "@" || value.Trim('%') == "")
            {
                Term term = new Term(2);
                term.ExpressJoke = true;
                term.Sign = "+";
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            if (value.Length == 1 && (value[0] == '-' || value[0] == '+'))
            {
                Term term = new Term(1);
                term.Sign = value;
                term.Joke = true;
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            else if (value == "-%" || value == "+%")
            {
                Term term = new Term(1);
                term.Sign = value[0].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                term.Joke = true;
                return term;
            }
            else
            {
                #region Term TT
                string[] pieces = value.Split('.');
                Term term = null;
                string sign = "+";
                if (pieces.Length > 1)
                    throw new NotImplementedException();// do something about them multiples...

                if (pieces[0].StartsWith("-"))
                {
                    sign = "-";
                }
                pieces[0] = pieces[0].Trim('-', '+');

                string coeF = "";
                char _base = ' ';
                string power = "";
                int indexPow = -3;
                int indexBase = -3;
                int indexCoe = -3;
                LookingFor lookingFor = LookingFor.myC;
                LookingFor foundFor = LookingFor.None;
                bool outsideAfter = false;
                bool outsideBefore = false;

                for (int i = 0; i < pieces[0].Length; i++)
                {
                    if (lookingFor != LookingFor.None)
                    {
                        #region Looking
                        int valueInt = -1;

                        if (int.TryParse(pieces[0][i].ToString(), out valueInt))
                        {
                            if (lookingFor == LookingFor.myC)
                                coeF += pieces[0][i].ToString();
                            else if (lookingFor == LookingFor.myP)
                                power += pieces[0][i].ToString();
                        }
                        else if (pieces[0][i] == '%')
                        {
                            if (i == 0)
                            {
                                foundFor = lookingFor;
                                outsideBefore = true;
                            }
                            else if (i == pieces[0].Length - 1 && lookingFor != LookingFor.myP)
                            {
                                outsideAfter = true;
                                foundFor = lookingFor;
                            }
                            else
                            {
                                if (lookingFor == LookingFor.myC)
                                    indexCoe = coeF.Length - 1;
                                else if (lookingFor == LookingFor.myP)
                                    indexPow = power.Length - 1;
                            }
                        }
                        else if (pieces[0][i] == '%')
                        {
                            //if (lookingFor == LookingFor.myB)
                            //{
                            //    lookingFor = LookingFor.myP;
                            //    _base += pieces[0][i];
                            //}
                            if (lookingFor == LookingFor.myP)
                            {
                                indexPow = power.Length - 1;
                            }
                            else throw new NotImplementedException();

                        }
                        else if (pieces[0][i] == '^')
                        {
                            if (lookingFor != LookingFor.myP)
                                lookingFor = LookingFor.myP;
                            else
                                lookingFor = LookingFor.None;
                        }
                        else if ("abcdefghijklmnopqrstuvwxyz".IndexOf(pieces[0][i]) != -1)
                        {
                            _base = pieces[0][i];
                            if (i + 1 < pieces[0].Length)
                                if (pieces[0][i + 1] == '%')
                                {
                                    indexBase = 0; ;
                                    i++;
                                }
                            //lookingFor = LookingFor.myP;
                        }
                        #endregion Looking
                    }
                    else
                    {
                        if (pieces[0][i] == '%')
                            outsideAfter = true;
                    }
                }

                SelectionCursor cursor = null;

                if (indexPow != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Power, indexPow);
                else if (indexCoe != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Coefficient, indexCoe);
                if (indexBase != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Base, indexBase);
                else if (outsideBefore)
                    cursor = new SelectionCursor(GetFoundFor(foundFor), -1);
                else if (outsideAfter)
                    cursor = new SelectionCursor(SelectedPieceType.OutSide, -2);

                if (power.Length == 0)
                    power = "1";


                if (coeF.Length != 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length != 0 && _base.ToString().Trim() == "" && power.Length != 0)
                {
                    term = new Term(int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length == 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(power));
                }

                if (cursor != null && term != null)
                {
                    term.Selected = true;
                    term.MySelectedindex = cursor.Index;
                    term.MySelectedPieceType = cursor.MySelectedPieceType;
                }
                term.Sign = sign;

                return term;

                #endregion Term TT
            }
            //throw new NotImplementedException();
        }
        public SelectedPieceType GetFoundFor(LookingFor fo)
        {
            if (fo == LookingFor.myB)
                return SelectedPieceType.Base;
            else if (fo == LookingFor.myC)
                return SelectedPieceType.Coefficient;
            else if (fo == LookingFor.myP)
                return SelectedPieceType.Power;
            else return SelectedPieceType.Base;
        }
        private MathBase.Expression GetExpression(string sourceTop, string sourceBot, Range rangeTop, Range rangeBot)
        {
            MathBase.Expression exp = new MathBase.Expression();

            #region Numerator

            string piece = (sourceTop.Substring(rangeTop.Start, 1 + (rangeTop.End - rangeTop.Start))).Trim('\'');

            if (piece.Length == 2 || piece.Length == 1)
            {
                if (piece[0] == '(' || piece[0] == ')')
                {
                    Brace brace = new Brace(piece[0]);
                    if (piece.IndexOf("%") != -1)
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                }
                else if (piece == "@")
                {
                    Term term = new Term(1);
                    term.ExpressJoke = true;
                    term.Sign = "+";
                    term.Selected = true;
                    term.MySelectedPieceType = SelectedPieceType.Coefficient;
                    term.MySelectedindex = -1;
                    exp.Numerator.Add(term);
                }
                else
                {
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece));
                }
            }
            else
            {
                int startIndex = 0;
                for (int i = 0; i < piece.Length; i++)
                {
                    Act(ref exp, ref piece, i, ref startIndex);
                }
            }

            #endregion Numerator

            #region Denominator

            if (sourceBot.Trim() != "")
            {
                MathBase.Expression dimExp = new MathBase.Expression();
                string pieceBot = sourceBot.Substring(rangeBot.Start, 1 + (rangeBot.End - rangeBot.Start));

                if (pieceBot.Length == 2 || pieceBot.Length == 1)
                {
                    if (pieceBot[0] == '(' || pieceBot[0] == ')')
                    {
                        dimExp.Numerator.Add(new Brace(pieceBot[0]));
                    }
                    else
                    {
                        if (pieceBot.Trim() != "%")
                            dimExp.Numerator.Add(GetMyTerm(pieceBot));
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (int i = 0; i < pieceBot.Length; i++)
                    {
                        Act(ref dimExp, ref pieceBot, i, ref startIndex);
                    }
                }

                exp.Denominator = new List<IExpressionPiece>(dimExp.Numerator);
            }

            #endregion Denominator

            return exp;
        }
        private void Decoy()
        {

        }
        private void Act(ref MathBase.Expression exp, ref string piece, int i, ref int startIndex)
        {
            if (piece[i] == '(' || piece[i] == ')')
            {
                if (startIndex == i)
                {
                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                }
                else if (i != 0 && piece[i - 1] != '(' && piece[i - 1] != ')')
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));

                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                    //there is your fucken term bro!!
                }
            }
            else if ((i != piece.Length - 1 && i != startIndex && (piece[i] == '-' || piece[i] == '+')))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex)));
                startIndex = i;
            }
            else if ((i == piece.Length - 1 && (piece[i] == '-' || piece[i] == '+')))
            {
                if (i - 1 > 0 && piece[i - 1] == ')')
                {
                    startIndex = i + 1;
                }
                else
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    //there is your fucken term bro!!!
                    char part = piece[(i - startIndex) + startIndex];
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                    startIndex = i;
                }
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i] == '+' || piece[i] == '-')))
            {
                throw new NotImplementedException();
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i - 1] == '+' || piece[i - 1] == '-')
                && piece[i] == '%'))
            {
                Term term = new Term(2);
                term.Joke = true;
                term.Sign = piece[i - 1].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                exp.Numerator.Add(term);
                startIndex = i + 1;
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i != startIndex))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                startIndex = i + 1;
            }
        }

        public EquationType GetEquationType()
        {
            return EquationType.Algebraic;
        }

        #endregion Methods
    }

    public class BasicBuilder : Grid, IProblemBuilder
    {
        #region Properties

        TermLevel _myTermLevel;
        ExpressionLevel _myExpressionLevel;
        string _top = "";

        public string Top
        {
            get { return _top; }
            set { _top = value; }
        }
        string _bot = "";

        public string Bot
        {
            get { return _bot; }
            set { _bot = value; }
        }
        List<History> myHistory = new List<History>();
        WrapPanel _myDisplayArea;
        

        #endregion Properties

        #region Constructor
        DispatcherTimer timer;
        public BasicBuilder(ref DispatcherTimer timer)
        {
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 251);
            timer.Tick += new EventHandler(ToggleSelected);
            this.timer = timer;
            this.Children.Add(_myDisplayArea);
            this.Children.Add(GetKeyboard());
        }

        #endregion Constructor

        #region Methods
        EventHandler errorHandler;
        public EventHandler ErrorHandler
        {
            get { return errorHandler; }
            set { errorHandler = value; }
        }
        public void ToggleSelected(object sender, EventArgs e)
        {
        }
        public void InputHandler(object sender, EventArgs e)
        {
            try
            {
                #region Food
                bool ignore = false;
                History startedWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                IKeyboardItemID caller = (IKeyboardItemID)sender;

                if (_myExpressionLevel == ExpressionLevel.Numerator)
                    _top = _top.Replace("@", "");
                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                    _bot = _bot.Replace("@", "");

                if (caller.GetID() == KeyBoardItemIDType.Other)
                {
                    if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
                    {
                        Undo();
                        ignore = true;
                    }
                    else Change((Other)caller);
                }
                else if (caller.GetID() == KeyBoardItemIDType.NumberVar)
                {
                    #region Number Or Variable
                    //Add.
                    if (_myExpressionLevel == ExpressionLevel.Numerator)
                    {
                        _top = _top.Replace("@", "");
                        if (_top.IndexOf("%") == -1)
                            _top += ((NumberVar)caller).MyVariableNumberValue + "%";
                        else
                            _top = _top.Replace("%", (((NumberVar)caller).MyVariableNumberValue + "%"));
                        Draw(ref timer);
                    }
                    else if (_myExpressionLevel == ExpressionLevel.Denominator)
                    {
                        _bot = _bot.Replace("@", "");
                        _bot = _bot.Trim('%');
                        _bot += ((NumberVar)caller).MyVariableNumberValue + "%";
                        Draw(ref timer);
                    }
                    #endregion Number Or Variable
                }
                else
                {
                    SignCounterAct((KeyboardSign)caller);
                }

                History EndedUpWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                if (!ignore && (startedWith.Changed(EndedUpWith)))
                {
                    myHistory.Add(startedWith);
                }
                #endregion Food

            }
            catch
            {
                Undo();
                errorHandler(this, e);
                //Draw(ref timer);
            }
        }
        private bool Undo()
        {
            if (myHistory.Count > 0)
            {
                for (int i = myHistory.Count - 1; i >= 0; i--)
                {
                    if (myHistory[i] != null)
                    {
                        History previous = myHistory[i];
                        _top = previous.TopBottoms[0];
                        _bot = previous.TopBottoms[1];
                        _myExpressionLevel = previous.ExpLevel;
                        _myTermLevel = previous.TermLevel;
                        myHistory[i] = null;
                        Draw(ref timer);
                        return true;
                    }
                }
            }
            return false;
        }
        private void SignCounterAct(KeyboardSign sign)
        {
            
                #region plus minus Mult

                if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "+%");
                    else _top += "+";
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
                {
                    if (_bot.IndexOf("%") != -1)
                        _bot = _bot.Replace("%", "+%");
                    else _bot += "+";
                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "+")
                {
                    _top += "'+'";
                    _bot += "'+'";
                }

                else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "-%");
                    else _top += "-";
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
                {
                    if (_bot.IndexOf("%") != -1)
                        _bot = _bot.Replace("%", "-%");
                    else _bot += "-";
                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "-")
                {
                    _top += "'-'";
                    _bot += "'-'";
                }

                else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "*")
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "*%");
                    else _top += "*";
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "*")
                {
                    if (_bot.IndexOf("%") != -1)
                        _bot = _bot.Replace("%", "*%");
                    else _bot += "*";
                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "*")
                {
                    _top += "'*'";
                    _bot += "'*'";
                }
                #endregion plus minus

                #region braces

                else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "(%");
                    else _top += "(%";
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
                {
                    if (_bot.IndexOf("%") != -1)
                        _bot = _bot.Replace("%", "(%");
                    else _bot += "(%";
                }
                else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
                {
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", ")%");
                    else _top += ")%";
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
                {
                    if (_bot.IndexOf("%") != -1)
                        _bot = _bot.Replace("%", ")%");
                    else _bot += ")%";
                }

                #endregion braces

                #region Split

                else if ( _myTermLevel == TermLevel.Base && sign.MyKey == "=" && _top.IndexOf('=') == -1)
                {
                    _myExpressionLevel = ExpressionLevel.Numerator;
                    if (_top.IndexOf("%") != -1)
                        _top = _top.Replace("%", "'='%");
                    else _top += "'='%";

                    if (_bot.IndexOf("%") != -1)
                        _bot = _bot.Replace("%", "'='");
                    else _bot += "'='";
                }

                #endregion Split

                else throw new NotImplementedException();
                Draw(ref timer);
        }
        public void Change(Other other)
        {
            #region Small
            if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    if (_top.IndexOf("%") == -1)
                        _top += "^";
                    else _top = _top.Replace("%", "^%");
                }
                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    if (_bot.IndexOf("%") == -1)
                        _bot += "^";
                    else _bot = _bot.Replace("%", "^%");
                }

                _myTermLevel = TermLevel.Power;
                Draw(ref timer);
            }
            else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    if (_top.IndexOf("%") == -1)
                        _top += "^";
                    else _top = _top.Replace("%", "^%");
                    _myTermLevel = TermLevel.Base;
                }
            
           
                else if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    if (_bot.IndexOf("%") == -1)
                        _bot += "^";
                    else _bot = _bot.Replace("%", "^%");
                    _myTermLevel = TermLevel.Base;
                }

                Draw(ref timer);
            }
            #endregion Small 
            #region Big
            else if (other.MyKeyboardItem == KeyboardItem.Up && _myExpressionLevel != ExpressionLevel.Numerator)
            {
                if (_myExpressionLevel == ExpressionLevel.Denominator)
                {
                    _myExpressionLevel = ExpressionLevel.MiddleSigns;
                    _bot = _bot.Replace("%", "");
                    _bot = _bot.Replace("@", "");
                    Draw(ref timer);
                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
                {
                    _myExpressionLevel = ExpressionLevel.Numerator;
                    //throw new NotImplementedException(); // remove selection from the Middle sign
                    _top += "%";
                    
                    Draw(ref timer);
                }

                // else if(_myExpressionLevel == ExpressionLevel.MiddleSigns
            }
            else if (other.MyKeyboardItem == KeyboardItem.Down && _myExpressionLevel != ExpressionLevel.Denominator)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                    _myExpressionLevel = ExpressionLevel.MiddleSigns;
                    _top = _top.Replace("%", "");
                    _top = _top.Replace("@", "");
           
                    Draw(ref timer);
                    
                }
                else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
                {
                    _myExpressionLevel = ExpressionLevel.Denominator;
                    if (_bot.Trim() != "")
                        _bot += "%";
                    else _bot = "@";
                    Draw(ref timer);
                }
            }
                #endregion Big

        }
        EventHandler handler;
        public Keyboard GetKeyboard()
        {
            handler = new EventHandler(InputHandler);
            return new Keyboard(ref handler);
        }
        public void Draw(ref DispatcherTimer timer)
        {
                if (_top.Length > 0 || _bot.Length > 0)
                {
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
                }
                else
                {
                    _top = "@";
                    timer.Tick += new EventHandler(ToggleSelected);
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
                }
            //maybe do more???
        }
        public Equation GetEquation()
        {
            if (_top.Trim() == "" && _bot.Trim() != "")
                _top = "+1";
            string rightTop = "";
            string rightBot = "";
            string _top1 = "";
            string _bot1 = "";
            Equation eq = new Equation(SignType.equal);
            if (_top.IndexOf("'='") == -1)
            {
                eq.IsComplete = false;
                _top1 = _top;
                _bot1 = _bot;
            }
            else
            {
                rightTop = _top.Substring(_top.IndexOf("'='") + 3);
                if (rightTop[0] == '%')
                {
                    eq.SplitSelected = true;
                }
                _top1 = _top.Substring(0, _top.IndexOf("'='"));

                rightBot = _bot.Substring(_bot.IndexOf("'='") + 3);
                _bot1 = _bot.Substring(0, _bot.IndexOf("'='"));
                eq.IsComplete = true;
            }

            if ((_top1.IndexOf("'+'") != -1 || _top1.IndexOf("'-'") != -1))
            {
                #region More than one expression

                while (_top1.IndexOf("'+'") != -1 || _top1.IndexOf("'-'") != -1)
                {
                    string expTop = "";
                    string expBot = "";

                    int indexTop = GetClosestIndex(_top1, "'+'", "'-'");
                    int indexBot = GetClosestIndex(_bot1, "'+'", "'-'");

                    expTop = _top1.Substring(0, indexTop );
                    expBot = _bot.Substring(0, indexBot);

                    eq.Left.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1)));

                    eq.Left.Add(new Sign(_top1.Substring(indexTop, 3).Trim('\'')));

                    _top1 = _top1.Substring(indexTop + 3);
                    _bot1 = _bot1.Substring(indexBot + 3);


                    if (_top1.IndexOf("'+'") == -1 && _top1.IndexOf("'-'") == -1)
                    {
                        if ((_top1.Trim() == "" && _bot1.Trim() == ""))
                        {
                            _top1 = "@";
                            _myExpressionLevel = ExpressionLevel.Numerator;
                        }
                        eq.Left.Add(GetExpression(_top1, (_bot1 == "%")? "@":_bot1, new Range(0, _top1.Length - 1), new Range(0, _bot1.Length - 1)));
                    }
                }

                #endregion More than one expression
            }
            else
            {
                eq.Left.Add(GetExpression(_top1,_bot1, new Range(0, _top1.Length - 1), new Range(0,_bot1.Length -1)));
            }

            if (eq.IsComplete)
            {
                if ((rightTop.IndexOf("'+'") != -1 || rightTop.IndexOf("'-'") != -1))
                {
                    #region More than one expression

                    while (rightTop.IndexOf("'+'") != -1 || rightTop.IndexOf("'-'") != -1)
                    {
                        string expTop = "";
                        string expBot = "";

                        int indexTop = GetClosestIndex(rightTop, "'+'", "'-'");
                        int indexBot = GetClosestIndex(rightBot, "'+'", "'-'");

                        expTop = rightTop.Substring(0, indexTop);
                        expBot = rightBot.Substring(0, indexBot);

                        eq.Right.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1)));

                        eq.Right.Add(new Sign(rightTop.Substring(indexTop, 3).Trim('\'')));

                        rightTop = rightTop.Substring(indexTop + 3);
                        rightBot = rightBot.Substring(indexBot + 3);


                        if (rightTop.IndexOf("'+'") == -1 && rightTop.IndexOf("'-'") == -1)
                        {
                            if ((rightTop.Trim() == "" && rightBot.Trim() == ""))
                            {
                                rightTop = "@";
                                _myExpressionLevel = ExpressionLevel.Numerator;
                            }
                            eq.Right.Add(GetExpression(rightTop, (rightBot == "%")? "@":rightBot, new Range(0, rightTop.Length - 1), new Range(0, rightBot.Length - 1)));
                        }
                    }

                    #endregion More than one expression
                }
                else  
                    eq.Right.Add(GetExpression(rightTop, rightBot,new Range(0, rightTop.Length -1), new Range(0, rightBot.Length -1)));
            }

            return eq;
        }
        private int GetClosestIndex(string source, string searchOne, string searchTwo)
        {
            if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) != -1)
            {
                return (source.IndexOf(searchTwo) < source.IndexOf(searchOne)) ?
                    source.IndexOf(searchTwo) : source.IndexOf(searchOne);
            }
            else if (source.IndexOf(searchOne) == -1 && source.IndexOf(searchTwo) != -1)
            {
                return source.IndexOf(searchTwo);
            }
            else if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) == -1)
            {
                return source.IndexOf(searchOne);
            }
            return -1;
        }
        private Term GetMyTerm(string value)
        {
            value = value.Replace("(%", "");
            value = value.Replace(")%", "");
            if (value.Trim() == "@" || value.Trim('%') == "")
            {
                Term term = new Term(2);
                term.ExpressJoke = true;
                term.Sign = "+";
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            if (value.Length == 1 && (value[0] == '-' || value[0] == '+'))
            {
                Term term = new Term(1);
                term.Sign = value;
                term.Joke = true;
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex =  -1;
                return term;
            }
            else if (value == "-%" || value == "+%")
            {
                Term term = new Term(1);
                term.Sign = value[0].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                term.Joke = true;
                return term;
            }
            else
            {
                #region Term TT
                string[] pieces = value.Split('.');
                Term term = null;
                string sign = "+";
                if (pieces.Length > 1)
                    throw new NotImplementedException();// do something about them multiples...

                if (pieces[0].StartsWith("-"))
                {
                    sign = "-";
                }
                pieces[0] = pieces[0].Trim('-', '+');

                string coeF = "";
                char _base = ' ';
                string power = "";
                int indexPow = -3;
                int indexBase = -3;
                int indexCoe = -3;
                LookingFor lookingFor = LookingFor.myC;
                LookingFor foundFor = LookingFor.None;
                bool outsideAfter = false;
                bool outsideBefore = false;

                for (int i = 0; i < pieces[0].Length; i++)
                {
                    if (lookingFor != LookingFor.None)
                    {
                        #region Looking
                        int valueInt = -1;

                        if (int.TryParse(pieces[0][i].ToString(), out valueInt))
                        {
                            if (lookingFor == LookingFor.myC)
                                coeF += pieces[0][i].ToString();
                            else if (lookingFor == LookingFor.myP)
                                power += pieces[0][i].ToString();
                        }
                        else if (pieces[0][i] == '%')
                        {
                            if (i == 0)
                            {
                                foundFor = lookingFor;
                                outsideBefore = true;
                            }
                            else if (i == pieces[0].Length - 1 && lookingFor != LookingFor.myP)
                            {
                                outsideAfter = true;
                                foundFor = lookingFor;
                            }
                            else
                            {
                                if (lookingFor == LookingFor.myC)
                                    indexCoe = coeF.Length - 1;
                                else if (lookingFor == LookingFor.myP)
                                    indexPow = power.Length - 1;
                            }
                        }
                        else if (pieces[0][i] == '%')
                        {
                            //if (lookingFor == LookingFor.myB)
                            //{
                            //    lookingFor = LookingFor.myP;
                            //    _base += pieces[0][i];
                            //}
                            if (lookingFor == LookingFor.myP)
                            {
                                indexPow = power.Length - 1;
                            }
                            else throw new NotImplementedException();

                        }
                        else if (pieces[0][i] == '^')
                        {
                            if (lookingFor != LookingFor.myP)
                                lookingFor = LookingFor.myP;
                            else
                                lookingFor = LookingFor.None;
                        }
                        else if ("abcdefghijklmnopqrstuvwxyz".IndexOf(pieces[0][i]) != -1)
                        {
                            _base = pieces[0][i];
                            if (i + 1 < pieces[0].Length)
                                if (pieces[0][i + 1] == '%')
                                {
                                    indexBase = 0; ;
                                    i++;
                                }
                            //lookingFor = LookingFor.myP;
                        }
                        #endregion Looking
                    }
                    else
                    {
                        if (pieces[0][i] == '%')
                            outsideAfter = true;
                    }
                }

                SelectionCursor cursor = null;

                if (indexPow != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Power, indexPow);
                else if (indexCoe != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Coefficient, indexCoe);
                if (indexBase != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Base, indexBase);
                else if (outsideBefore)
                    cursor = new SelectionCursor(GetFoundFor(foundFor), -1);
                else if (outsideAfter)
                    cursor = new SelectionCursor(SelectedPieceType.OutSide, -2);

                if (power.Length == 0)
                    power = "1";


                if (coeF.Length != 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length != 0 && _base.ToString().Trim() == "" && power.Length != 0)
                {
                    term = new Term(int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length == 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(power));
                }

                if (cursor != null && term != null)
                {
                    term.Selected = true;
                    term.MySelectedindex = cursor.Index;
                    term.MySelectedPieceType = cursor.MySelectedPieceType;
                }
                term.Sign = sign;

                return term;

                #endregion Term TT
            }
            //throw new NotImplementedException();
        }
        public SelectedPieceType GetFoundFor(LookingFor fo)
        {
            if (fo == LookingFor.myB)
                return SelectedPieceType.Base;
            else if (fo == LookingFor.myC)
                return SelectedPieceType.Coefficient;
            else if (fo == LookingFor.myP)
                return SelectedPieceType.Power;
            else return SelectedPieceType.Base;
        }
        private MathBase.Expression GetExpression(string sourceTop, string sourceBot, Range rangeTop, Range rangeBot)
        { 
            MathBase.Expression exp = new MathBase.Expression();

            #region Numerator

            string piece = sourceTop.Substring(rangeTop.Start, 1 + (rangeTop.End - rangeTop.Start));

            if (piece.Length == 2 || piece.Length == 1)
            {
                if (piece[0] == '(' || piece[0] == ')')
                {
                    Brace brace = new Brace(piece[0]);
                    if(piece.IndexOf("%") != -1)
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                }
                else if (piece == "@")
                {
                    Term term = new Term(1);
                    term.ExpressJoke = true;
                    term.Sign = "+";
                    term.Selected = true;
                    term.MySelectedPieceType = SelectedPieceType.Coefficient;
                    term.MySelectedindex = -1;
                    exp.Numerator.Add(term);
                }
                else
                {
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece));
                }
            }
            else
            {
                int startIndex = 0;
                for (int i = 0; i < piece.Length; i++)
                {
                    Act(ref exp, ref piece, i, ref startIndex);
                }
            }

            #endregion Numerator

            #region Denominator

            if (sourceBot.Trim() != "")
            {
                MathBase.Expression dimExp = new MathBase.Expression();
                string pieceBot = sourceBot.Substring(rangeBot.Start, 1 + (rangeBot.End - rangeBot.Start));

                if (pieceBot.Length == 2 || pieceBot.Length == 1)
                {
                    if (pieceBot[0] == '(' || pieceBot[0] == ')')
                    {
                        dimExp.Numerator.Add(new Brace(pieceBot[0]));
                    }
                    else
                    {
                        if (pieceBot.Trim() != "%")
                            dimExp.Numerator.Add(GetMyTerm(pieceBot));
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (int i = 0; i < pieceBot.Length; i++)
                    {
                        Act(ref dimExp, ref pieceBot, i, ref startIndex);
                    }
                }

                exp.Denominator = new List<IExpressionPiece>(dimExp.Numerator);
            }

            #endregion Denominator

            return exp;
        }
        private void Decoy()
        {
            
        }
        private void Act(ref MathBase.Expression exp,ref string piece,int i,ref int startIndex)
        {
            if (piece[i] == '(' || piece[i] == ')')
            {
                if (startIndex == i)
                {
                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                }
                else if (i != 0 && piece[i - 1] != '(' && piece[i - 1] != ')')
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex+1)));

                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                    //there is your fucken term bro!!
                }
            }
            else if ((i != piece.Length - 1 && i != startIndex && ( piece[i] == '-' || piece[i] == '+')))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex =(i - startIndex);
                if (piece.Trim() != "%")
                exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex )));
                startIndex = i;
            }
            else if ((i == piece.Length - 1 && ( piece[i] == '-' || piece[i] == '+')))
            {
                if (i - 1 > 0 && piece[i - 1] == ')')
                {
                    startIndex = i + 1;
                }
                else
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    //there is your fucken term bro!!!
                    char part = piece[(i - startIndex) + startIndex];
                    if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                    startIndex = i;
                }
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i] == '+' || piece[i] == '-')))
            {
                throw new NotImplementedException();
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i == startIndex &&  (piece[i-1] == '+' || piece[i-1] == '-')
                && piece[i ] == '%'))
            {
                Term term = new Term(2);
                term.Joke = true;
                term.Sign = piece[i - 1].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                exp.Numerator.Add(term);
                startIndex = i + 1;
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i != startIndex))
            {
                if (startIndex -1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                startIndex = i + 1;
            }
        }
        public EquationType GetEquationType()
        {
            return EquationType.Algebraic;
        }

        #endregion Methods
    }

    public class LogarithimsBuilder : Grid, IProblemBuilder
    {
        #region Properties

        TermLevel _myTermLevel;
        ExpressionLevel _myExpressionLevel = ExpressionLevel.Numerator;
        string _top = "";

        public string Top
        {
            get { return _top; }
            set { _top = value; }
        }
        string _bot = "";

        public string Bot
        {
            get { return _bot; }
            set { _bot = value; }
        }
        List<History> myHistory = new List<History>();
        WrapPanel _myDisplayArea;
        #endregion Properties

        #region Save Properties

        string _savedAs = "";

        public string SavedAs
        {
            get { return _savedAs; }
            set { _savedAs = value; }
        }

        #endregion Save Properties

        #region Constructor
        DispatcherTimer timer;
        public LogarithimsBuilder(ref DispatcherTimer timer)
        {
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 251);
            timer.Tick += new EventHandler(ToggleSelected);
            this.timer = timer;
            this.Children.Add(_myDisplayArea);
            this.Children.Add(GetKeyboard());
        }
        public LogarithimsBuilder(string savedAs, string topSaved, string botSaved, EventHandler handler)
        {
            _savedAs = savedAs;
            Top = topSaved.Replace("equals", "=");
            Bot = botSaved.Replace("equals", "=");
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 5);
            //timer.Tick += new EventHandler(ToggleSelected);
            this.Children.Add(_myDisplayArea);
            this.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
            Draw();
            AddDeleteKey();
        }

        #endregion Constructor

        #region Methods
        private void AddDeleteKey()
        {
            Grid grid = new Grid();
            grid.Width = 50;
            grid.Height = 50;
            grid.Background = GetBackGround();
            grid.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(DeleteFromStorage);
            grid.Margin = new Thickness(700, 0, 0, 0);
            this.Children.Add(grid);
        }
        private ImageBrush GetBackGround()
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/deleteIcon.png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }

        private void DeleteFromStorage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SettingsAssistant assistant = new SettingsAssistant();
            assistant.changeSetting(new valuePair(SavedAs + "top", "deleted"));
            Top = "deleted";
            this.Visibility = System.Windows.Visibility.Collapsed;
            //call handler on the xaml page. to refresh(initialize)
        }
        EventHandler errorHandler;
        public EventHandler ErrorHandler
        {
            get { return errorHandler; }
            set { errorHandler = value; }
        }
        public void ToggleSelected(object sender, EventArgs e)
        {
        }
        public void InputHandler(object sender, EventArgs e)
        {
            try
            {
                #region Food
                bool ignore = false;
                History startedWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                IKeyboardItemID caller = (IKeyboardItemID)sender;

                if (_myExpressionLevel == ExpressionLevel.Numerator)
                    _top = _top.Replace("@", "");

                if (caller.GetID() == KeyBoardItemIDType.Other)
                {
                    if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
                    {
                        Undo();
                        ignore = true;
                    }
                    else Change((Other)caller);
                }
                else if (caller.GetID() == KeyBoardItemIDType.NumberVar)
                {
                    #region Number Or Variable
                    //Add.
                    if (_myExpressionLevel == ExpressionLevel.Numerator)
                    {
                        _top += ((NumberVar)caller).MyVariableNumberValue;
                        Draw(ref timer);
                    }
                    else if (_myExpressionLevel == ExpressionLevel.Denominator)
                    {
                        _top += ((NumberVar)caller).MyVariableNumberValue;
                        Draw(ref timer);
                    }
                    #endregion Number Or Variable
                }
                else
                {
                    SignCounterAct((KeyboardSign)caller);
                }

                History EndedUpWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

                if (!ignore && (startedWith.Changed(EndedUpWith)))
                {
                    myHistory.Add(startedWith);
                }

                #endregion Food

            }
            catch
            {
                Undo();
                errorHandler(this, e);
                //Draw(ref timer);
            }
        }
        private bool Undo()
        {
            if (myHistory.Count > 0)
            {
                for (int i = myHistory.Count - 1; i >= 0; i--)
                {
                    if (myHistory[i] != null)
                    {
                        History previous = myHistory[i];
                        _top = previous.TopBottoms[0];
                        _bot = previous.TopBottoms[1];
                        _myExpressionLevel = previous.ExpLevel;
                        _myTermLevel = previous.TermLevel;
                        myHistory[i] = null;
                        Draw(ref timer);
                        return true;
                    }
                }
            }
            return false;
        }
        private void SignCounterAct(KeyboardSign sign)
        {

            #region plus minus Mult

            if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
            {
                    _top += "+";
            }

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
            {
                 _top += "-";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && sign.MyKey == "-")
            {
                _top = _top.Substring(0, _top.Length - 1); 
                _top += "'-'";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && sign.MyKey == "+")
            {
                _top = _top.Substring(0, _top.Length - 1); 
                _top += "'+'";
            }

            #endregion plus minus

            #region braces

            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "(%");
                else _top += "(%";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "(%");
                else _bot += "(%";
            }
            else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", ")%");
                else _top += ")%";
            }
            else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
            {
                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", ")%");
                else _bot += ")%";
            }

            #endregion braces

            #region Split

            else if (_myTermLevel == TermLevel.Base && sign.MyKey == "=" && _top.IndexOf('=') == -1)
            {
                _myExpressionLevel = ExpressionLevel.Numerator;
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "'='");
                else _top += "'='";

                if (_bot.IndexOf("%") != -1)
                    _bot = _bot.Replace("%", "'='");
                else _bot += "'='";
            }

            #endregion Split

            else throw new NotImplementedException();
            Draw(ref timer);
        }
        public void Change(Other other)
        {
            #region Small
            if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                        _top += "^";

                }

                _myTermLevel = TermLevel.Power;
                Draw(ref timer);
            }
            else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power)
            {
                if (_myExpressionLevel == ExpressionLevel.Numerator)
                {
                        _top += "^";
                }

                Draw(ref timer);
            }
            #endregion Small
            #region Big
            else if (other.MyKeyboardItem == KeyboardItem.Up )//&& _myExpressionLevel != ExpressionLevel.Numerator)
            {
                //if (_myExpressionLevel == ExpressionLevel.Denominator)
                //{
                    _myExpressionLevel = ExpressionLevel.Numerator;
                    _top += "~";
                    Draw(ref timer);
                //}

                // else if(_myExpressionLevel == ExpressionLevel.MiddleSigns
            }
            else if (other.MyKeyboardItem == KeyboardItem.Down)// && _myExpressionLevel != ExpressionLevel.Denominator)
            {
                //if (_myExpressionLevel == ExpressionLevel.Numerator)
                //{
                    _myExpressionLevel = ExpressionLevel.Denominator;
                    _top += "~";
                    Draw(ref timer);

                //}
            }
            #endregion Big

            if (other.MyKeyboardItem == KeyboardItem.Log)
            {
                _top += "log";
                Draw(ref timer);
            }

        }
        EventHandler handler;
        public Keyboard GetKeyboard()
        {
            handler = new EventHandler(InputHandler);
            return new Keyboard(ref handler);
        }
        public void Draw(ref DispatcherTimer timer)
        {
                if (_top.Length > 0 || _bot.Length > 0)
                {
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
                }
                else
                {
                    _top = "@";
                    timer.Tick += new EventHandler(ToggleSelected);
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));
                }
        }
        public void Draw()
        {
            if (_top.Length > 0 || _bot.Length > 0)
            {
                _myDisplayArea.Children.Clear();
                _myDisplayArea.Children.Add(new Step(GetEquation()));
            }
        }
        public LogarithmicEquation GetEquation()
        {
            //_top = "2log~b~(x)'='log~b~(4)'+'log~b~(x-1)";
            //_top = "2log~b~x'+'";//'='log~b~4'+'log~b~x-1";
            string left = "", right = "";
            LogarithmicEquation eq = new LogarithmicEquation(SignType.equal);

            if (_top.IndexOf("'='") != -1)
            {
                right = _top.Substring(_top.IndexOf("'='") + 3);
                left = _top.Substring(0, _top.IndexOf("'='"));
            }
            else
            {
              left = _top;
              eq.IsComplete = false;
            }

            eq.Left.AddRange(GetPieces(left));
            if (right.Trim() != "")
                eq.Right.AddRange(GetPieces(right));

            return eq;
        }
        private List<IAlgebraPiece> GetPieces(string value)
        {
            List<IAlgebraPiece> answer = new List<IAlgebraPiece>();

            if ((value.IndexOf("'+'") != -1 || value.IndexOf("'-'") != -1))
            {
                #region More than one expression

                while (value.IndexOf("'+'") != -1 || value.IndexOf("'-'") != -1)
                {
                    string expTop = "";
                    int indexTop = GetClosestIndex(value, "'+'", "'-'");
                    expTop = value.Substring(0, indexTop);
                    answer.Add(GetMyLogTerm(expTop));
                    answer.Add(new Sign(value.Substring(indexTop, 3).Trim('\'')));

                    value = value.Substring(indexTop + 3);

                    if (value.IndexOf("'+'") == -1 && value.IndexOf("'-'") == -1)
                    {
                        if ((value.Trim() == "" && value.Trim() == ""))
                        {
                            value = "@";
                        }
                        answer.Add(GetMyLogTerm(value));
                    }
                }

                #endregion More than one expression
            }
            else
            {
                answer.Add(GetMyLogTerm(value));
            }

            return answer;
        }
        private int GetClosestIndex(string source, string searchOne, string searchTwo)
        {
            if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) != -1)
            {
                return (source.IndexOf(searchTwo) < source.IndexOf(searchOne)) ?
                    source.IndexOf(searchTwo) : source.IndexOf(searchOne);
            }
            else if (source.IndexOf(searchOne) == -1 && source.IndexOf(searchTwo) != -1)
            {
                return source.IndexOf(searchTwo);
            }
            else if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) == -1)
            {
                return source.IndexOf(searchOne);
            }
            return -1;
        }
        private Term GetMyTerm(string value)
        {
            value = value.Replace("(%", "");
            value = value.Replace(")%", "");
            if (value.Trim() == "@" || value.Trim('%') == "")
            {
                Term term = new Term(2);
                term.ExpressJoke = true;
                term.Sign = "+";
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            if (value.Length == 1 && (value[0] == '-' || value[0] == '+'))
            {
                Term term = new Term(1);
                term.Sign = value;
                term.Joke = true;
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            else if (value == "-%" || value == "+%")
            {
                Term term = new Term(1);
                term.Sign = value[0].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                term.Joke = true;
                return term;
            }
            else
            {
                #region Term TT
                string[] pieces = value.Split('.');
                Term term = null;
                string sign = "+";
                if (pieces.Length > 1)
                    throw new NotImplementedException();// do something about them multiples...

                if (pieces[0].StartsWith("-"))
                {
                    sign = "-";
                }
                pieces[0] = pieces[0].Trim('-', '+');

                string coeF = "";
                char _base = ' ';
                string power = "";
                int indexPow = -3;
                int indexBase = -3;
                int indexCoe = -3;
                LookingFor lookingFor = LookingFor.myC;
                LookingFor foundFor = LookingFor.None;
                bool outsideAfter = false;
                bool outsideBefore = false;

                for (int i = 0; i < pieces[0].Length; i++)
                {
                    if (lookingFor != LookingFor.None)
                    {
                        #region Looking
                        int valueInt = -1;

                        if (int.TryParse(pieces[0][i].ToString(), out valueInt))
                        {
                            if (lookingFor == LookingFor.myC)
                                coeF += pieces[0][i].ToString();
                            else if (lookingFor == LookingFor.myP)
                                power += pieces[0][i].ToString();
                        }
                        else if (pieces[0][i] == '%')
                        {
                            if (i == 0)
                            {
                                foundFor = lookingFor;
                                outsideBefore = true;
                            }
                            else if (i == pieces[0].Length - 1 && lookingFor != LookingFor.myP)
                            {
                                outsideAfter = true;
                                foundFor = lookingFor;
                            }
                            else
                            {
                                if (lookingFor == LookingFor.myC)
                                    indexCoe = coeF.Length - 1;
                                else if (lookingFor == LookingFor.myP)
                                    indexPow = power.Length - 1;
                            }
                        }
                        else if (pieces[0][i] == '%')
                        {
                            //if (lookingFor == LookingFor.myB)
                            //{
                            //    lookingFor = LookingFor.myP;
                            //    _base += pieces[0][i];
                            //}
                            if (lookingFor == LookingFor.myP)
                            {
                                indexPow = power.Length - 1;
                            }
                            else throw new NotImplementedException();

                        }
                        else if (pieces[0][i] == '^')
                        {
                            if (lookingFor != LookingFor.myP)
                                lookingFor = LookingFor.myP;
                            else
                                lookingFor = LookingFor.None;
                        }
                        else if ("abcdefghijklmnopqrstuvwxyz".IndexOf(pieces[0][i]) != -1)
                        {
                            _base = pieces[0][i];
                            if (i + 1 < pieces[0].Length)
                                if (pieces[0][i + 1] == '%')
                                {
                                    indexBase = 0; ;
                                    i++;
                                }
                            //lookingFor = LookingFor.myP;
                        }
                        #endregion Looking
                    }
                    else
                    {
                        if (pieces[0][i] == '%')
                            outsideAfter = true;
                    }
                }

                SelectionCursor cursor = null;

                if (indexPow != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Power, indexPow);
                else if (indexCoe != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Coefficient, indexCoe);
                if (indexBase != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Base, indexBase);
                else if (outsideBefore)
                    cursor = new SelectionCursor(GetFoundFor(foundFor), -1);
                else if (outsideAfter)
                    cursor = new SelectionCursor(SelectedPieceType.OutSide, -2);

                if (power.Length == 0)
                    power = "1";


                if (coeF.Length != 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length != 0 && _base.ToString().Trim() == "" && power.Length != 0)
                {
                    term = new Term(int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length == 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(power));
                }

                if (cursor != null && term != null)
                {
                    term.Selected = true;
                    term.MySelectedindex = cursor.Index;
                    term.MySelectedPieceType = cursor.MySelectedPieceType;
                }
                term.Sign = sign;

                return term;

                #endregion Term TT
            }
            //throw new NotImplementedException();
        }
        private string FixForShow(string value)
        {
            int appearances = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '~')
                    appearances++;
            }

            if (appearances > 2)
                return value.Substring(0, value.Length - 1);
            else return value;
        }
        private IAlgebraPiece GetMyLogTerm(string value)
        {
            value = value.Trim('%', '@');
            value = FixForShow(value);
            string expression = "";
            string myBase = "";
            int coeF = -10;


            if (value.Trim() == "" || value.Trim() == "@")
            {
                LogTerm term = new LogTerm(1, ' ', null);
                term.IsComplete = false;
                term.ShowBase = false;
                term.ShowLog = false;
                return term;
            }
            if(int.TryParse(value.Trim('@', '%'), out coeF))
            {
                LogTerm term = new LogTerm(coeF, ' ', null);
                term.IsComplete = false;
                term.ShowBase = false;
                term.ShowLog = false;
                return term;
            }

            if (value.LastIndexOf("~") != -1 && value.Substring(0, value.LastIndexOf("~") -1).IndexOf("~") != -1)
            {
                expression = value.Substring(value.LastIndexOf("~") + 1);
            }
            if (value.ToLower().IndexOf("log") != -1)
            {
                if (value.ToLower().IndexOf("log") == 0)
                    coeF = 1;
                else
                    coeF = int.Parse((value.Substring(0, value.ToLower().IndexOf("log")).Trim('%','@')));

                int startingInd = value.ToLower().IndexOf("log") + 3;
                if (startingInd + 2 < value.Length || (value.IndexOf("~") != -1 && startingInd + 2 < value.Length))
                    myBase = value.Substring(startingInd, 2).Trim('~');
                else if(value.IndexOf("~") != -1 && expression == "")
                    myBase = value.Substring(startingInd).Trim('~');

                if (myBase.Trim() != "")
                    return new LogTerm(coeF, myBase.Trim()[0], GetExpression(expression, "", new Range(0, expression.Length - 1), null));
                else
                {
                    LogTerm term = new LogTerm(coeF, ' ', null);
                    term.IsComplete = false;
                    term.ShowBase = false;
                    term.ShowLog = true;
                    return term;
                }
            }

            else throw new NotImplementedException();

        }
        public SelectedPieceType GetFoundFor(LookingFor fo)
        {
            if (fo == LookingFor.myB)
                return SelectedPieceType.Base;
            else if (fo == LookingFor.myC)
                return SelectedPieceType.Coefficient;
            else if (fo == LookingFor.myP)
                return SelectedPieceType.Power;
            else return SelectedPieceType.Base;
        }
        private MathBase.Expression GetExpression(string sourceTop, string sourceBot, Range rangeTop, Range rangeBot)
        {
            MathBase.Expression exp = new MathBase.Expression();

            #region Numerator

            string piece = sourceTop.Substring(rangeTop.Start, 1 + (rangeTop.End - rangeTop.Start));

            if (piece.Length == 2 || piece.Length == 1)
            {
                if (piece[0] == '(' || piece[0] == ')')
                {
                    Brace brace = new Brace(piece[0]);
                    if (piece.IndexOf("%") != -1)
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                }
                else if (piece == "@")
                {
                    Term term = new Term(1);
                    term.ExpressJoke = true;
                    term.Sign = "+";
                    term.Selected = true;
                    term.MySelectedPieceType = SelectedPieceType.Coefficient;
                    term.MySelectedindex = -1;
                    exp.Numerator.Add(term);
                }
                else if (piece.IndexOf("-") != -1 || piece.IndexOf("+") != -1)
                {
                    if (piece.IndexOf("-") != -1 && piece.EndsWith("-"))
                    {
                        exp.Numerator.Add(GetMyTerm(piece.Trim('-')));
                        Term term = new Term(1);
                        term.Joke = true;
                        term.Sign = "-";
                        term.Selected = true;
                        term.MySelectedPieceType = SelectedPieceType.Coefficient;
                        term.MySelectedindex = -1;
                        exp.Numerator.Add(term);
                    }
                    else if(piece.EndsWith("+"))
                    {
                        exp.Numerator.Add(GetMyTerm(piece.Trim('+')));
                        Term term = new Term(1);
                        term.Joke = true;
                        term.Sign = "+";
                        term.Selected = true;
                        term.MySelectedPieceType = SelectedPieceType.Coefficient;
                        term.MySelectedindex = -1;
                        exp.Numerator.Add(term);
                    }
                }
                else
                {
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece));
                }
            }
            else
            {
                int startIndex = 0;
                for (int i = 0; i < piece.Length; i++)
                {
                    Act(ref exp, ref piece, i, ref startIndex);
                }
            }

            #endregion Numerator

            #region Denominator

            if (sourceBot.Trim() != "")
            {
                MathBase.Expression dimExp = new MathBase.Expression();
                string pieceBot = sourceBot.Substring(rangeBot.Start, 1 + (rangeBot.End - rangeBot.Start));

                if (pieceBot.Length == 2 || pieceBot.Length == 1)
                {
                    if (pieceBot[0] == '(' || pieceBot[0] == ')')
                    {
                        dimExp.Numerator.Add(new Brace(pieceBot[0]));
                    }
                    else
                    {
                        if (pieceBot.Trim() != "%")
                            dimExp.Numerator.Add(GetMyTerm(pieceBot));
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (int i = 0; i < pieceBot.Length; i++)
                    {
                        Act(ref dimExp, ref pieceBot, i, ref startIndex);
                    }
                }

                exp.Denominator = new List<IExpressionPiece>(dimExp.Numerator);
            }

            #endregion Denominator

            return exp;
        }
        private void Act(ref MathBase.Expression exp, ref string piece, int i, ref int startIndex)
        {
            if (piece[i] == '(' || piece[i] == ')')
            {
                if (startIndex == i)
                {
                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                }
                else if (i != 0 && piece[i - 1] != '(' && piece[i - 1] != ')')
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));

                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                    //there is your fucken term bro!!
                }
            }
            else if ((i != piece.Length - 1 && i != startIndex && (piece[i] == '-' || piece[i] == '+')))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex)));
                startIndex = i;
            }
            else if ((i == piece.Length - 1 && (piece[i] == '-' || piece[i] == '+')))
            {
                if (i - 1 > 0 && piece[i - 1] == ')')
                {
                    startIndex = i + 1;
                }
                else
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    //there is your fucken term bro!!!
                    char part = piece[(i - startIndex) + startIndex];
                    if (piece.Trim() != "%")
                    {
                        string pie = piece.Substring(startIndex, endIndex + 1);
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));

                        if(pie.EndsWith("-") || pie.EndsWith("+"))
                        {
                            Term term = new Term(1);
                            term.Joke = true;
                            term.Sign = pie[pie.Length -1].ToString();
                            term.Selected = true;
                            term.MySelectedPieceType = SelectedPieceType.Coefficient;
                            term.MySelectedindex = -1;
                            exp.Numerator.Add(term);
                        }
                    }
                    startIndex = i;
                }
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i] == '+' || piece[i] == '-')))
            {
                throw new NotImplementedException();
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i - 1] == '+' || piece[i - 1] == '-')
                && piece[i] == '%'))
            {
                Term term = new Term(2);
                term.Joke = true;
                term.Sign = piece[i - 1].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                exp.Numerator.Add(term);
                startIndex = i + 1;
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i != startIndex))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                startIndex = i + 1;
            }
        }
        public EquationType GetEquationType()
        {
            return EquationType.Logarithmic;
        }

        #endregion Methods
    }

    public class ExponentialBuilder : Grid, IProblemBuilder
    {
        #region Properties
        EventHandler errorHandler;
        public EventHandler ErrorHandler
        {
            get { return errorHandler; }
            set { errorHandler = value; }
        }
        TermLevel _myTermLevel = TermLevel.Base;
        TermLevel _myExpressionLevel = TermLevel.Base;
        string _top = "";

        public string Top
        {
            get { return _top; }
            set { _top = value; }
        }
        List<History> myHistory = new List<History>();
        WrapPanel _myDisplayArea;


        #endregion Properties

        #region Save Properties

        string _savedAs = "";

        public string SavedAs
        {
            get { return _savedAs; }
            set { _savedAs = value; }
        }

        #endregion Save Properties

        #region Constructor
        DispatcherTimer timer;
        public ExponentialBuilder(ref DispatcherTimer timer)
        {
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 251);
            timer.Tick += new EventHandler(ToggleSelected);
            this.timer = timer;
            this.Children.Add(_myDisplayArea);
            this.Children.Add(GetKeyboard());
        }
        public ExponentialBuilder(string savedAs, string topSaved, EventHandler handler)
        {
            _savedAs = savedAs;
            Top = topSaved.Replace("equals", "=");
            //Bot = botSaved.Replace("equals", "=");
            this._myDisplayArea = new WrapPanel();
            this._myDisplayArea.Margin = new Thickness(32, 25, 0, 10);
            //timer.Tick += new EventHandler(ToggleSelected);
            //this.timer = timer;
            this.Children.Add(_myDisplayArea);
            //this.Children.Add(GetKeyboard());
            this.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
            Draw();
            AddDeleteKey();
        }

        #endregion Constructor

        #region Methods

        private void AddDeleteKey()
        {
            Grid grid = new Grid();
            grid.Width = 50;
            grid.Height = 50;
            grid.Background = GetBackGround();
            grid.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(DeleteFromStorage);
            grid.Margin = new Thickness(700, 0, 0, 0);
            this.Children.Add(grid);
        }
        private ImageBrush GetBackGround()
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/deleteIcon.png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }

        private void DeleteFromStorage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SettingsAssistant assistant = new SettingsAssistant();
            assistant.changeSetting(new valuePair(SavedAs + "top", "deleted"));
            Top = "deleted";
            this.Visibility = System.Windows.Visibility.Collapsed;
        }


        public void ToggleSelected(object sender, EventArgs e)
        {
        }
        public void InputHandler(object sender, EventArgs e)
        {
            try
            {
                #region food

                bool ignore = false;
                History startedWith = new History(_top, "", _myExpressionLevel, _myTermLevel);

                IKeyboardItemID caller = (IKeyboardItemID)sender;

                if (_myExpressionLevel == TermLevel.Base)
                    _top = _top.Replace("@", "");

                if (caller.GetID() == KeyBoardItemIDType.Other)
                {
                    if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
                    {
                        Undo();
                        ignore = true;
                    }
                    else Change((Other)caller);
                }
                else if (caller.GetID() == KeyBoardItemIDType.NumberVar)
                {
                    #region Number Or Variable
                    //Add.

                    _top = _top.Replace("@", "");
                    if (_top.IndexOf("%") == -1)
                        _top += ((NumberVar)caller).MyVariableNumberValue + "%";
                    else
                        _top = _top.Replace("%", (((NumberVar)caller).MyVariableNumberValue + "%"));
                    Draw(ref timer);
                    #endregion Number Or Variable
                }
                else
                {
                    SignCounterAct((KeyboardSign)caller);
                }

                History EndedUpWith = new History(_top, "", _myExpressionLevel, _myTermLevel);

                if (!ignore && (startedWith.Changed2(EndedUpWith)))
                {
                    myHistory.Add(startedWith);
                }
                #endregion food
            }
            catch
            {
                Undo();
                //Undo();
                errorHandler(this, e);
                //Draw(ref timer);
            }
        }
        private bool Undo()
        {
            if (myHistory.Count > 0)
            {
                for (int i = myHistory.Count - 1; i >= 0; i--)
                {
                    if (myHistory[i] != null)
                    {
                        History previous = myHistory[i];
                        _top = previous.TopBottoms[0];
                        //_bot = previous.TopBottoms[1];
                        _myExpressionLevel = previous.TermLevel2;
                        _myTermLevel = previous.TermLevel;
                        myHistory[i] = null;
                        Draw(ref timer);
                        return true;
                    }
                }
            }
            return false;
        }
        private void SignCounterAct(KeyboardSign sign)
        {

            #region plus minus Mult

            if (_myExpressionLevel ==  TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "+%");
                else _top += "+";
            }
            else if (_myExpressionLevel ==  TermLevel.Base && sign.MyKey == "+")
            {
                //_top += "'+'";
            }

            else if (_myExpressionLevel == TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "-%");
                else _top += "-";
            }
            else if (_myExpressionLevel ==  TermLevel.Base && sign.MyKey == "-")
            {
                //_top += "'-'";
            }
            else if (_myExpressionLevel ==  TermLevel.Base && sign.MyKey == "*")
            {
                //_top += "'*'";
            }
            #endregion plus minus

            #region braces

            else if (_myExpressionLevel ==  TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "(%");
                else _top += "(%";
            }
            
            else if (_myExpressionLevel ==  TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", ")%");
                else _top += ")%";
            }


            #endregion braces

            #region Split

            else if (_myExpressionLevel == TermLevel.Base &&
                _myTermLevel == TermLevel.Base && sign.MyKey == "=" && _top.IndexOf('=') == -1)
            {
                if (_top.IndexOf("%") != -1)
                    _top = _top.Replace("%", "'='%");
                else _top += "'='%";
            }

            #endregion Split

            else throw new NotImplementedException();
            Draw(ref timer);
        }
        public void Change(Other other)
        {
            #region Small
            if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base
                && _myExpressionLevel == TermLevel.Power)
            {
                if (_top.IndexOf("%") == -1)
                    _top += "^";
                else _top = _top.Replace("%", "^%");

                _myTermLevel = TermLevel.Power;
                Draw(ref timer);
            }
            else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power
                && _myExpressionLevel == TermLevel.Power)
            {
                if (_top.IndexOf("%") == -1)
                    _top += "^";
                else _top = _top.Replace("%", "^%");
                _myTermLevel = TermLevel.Base;

                Draw(ref timer);
            }
            #endregion Small

            #region Big
            else if (other.MyKeyboardItem == KeyboardItem.Up && _myExpressionLevel != TermLevel.Power)
            {
                    _myExpressionLevel = TermLevel.Power;
                    _top = _top.Replace("%", "");
                    _top = _top.Replace("@", "");
                    _top += "#";
                    Draw(ref timer);
            }
            else if (other.MyKeyboardItem == KeyboardItem.Down && _myExpressionLevel !=  TermLevel.Base)
            {
               
                    _myExpressionLevel = TermLevel.Base;
                    _top = _top.Replace("%", "");
                    _top = _top.Replace("@", "");
                    _top += "#";
                    Draw(ref timer);
            }
            #endregion Big

        }
        EventHandler handler;
        public Keyboard GetKeyboard()
        {
            handler = new EventHandler(InputHandler);
            return new Keyboard(ref handler);
        }
        public void Draw(ref DispatcherTimer timer)
        {
                //_top = "9#4x-1#'='27#5-x#";
                if (_top.Length > 0)
                {
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));//, ref timer));
                }
                else
                {
                    _top = "@";
                    _myDisplayArea.Children.Clear();
                    _myDisplayArea.Children.Add(new Step(GetEquation(), ref timer));//, ref timer));
                }


            //maybe do more???
        }
        public void Draw()
        {
            //_top = "9#4x-1#'='27#5-x#";
            if (_top.Length > 0)
            {
                _myDisplayArea.Children.Clear();
                _myDisplayArea.Children.Add(new Step(GetEquation()));//, ref timer));
            }

            //maybe do more???
        }
        public ExponentialEquation GetEquation()
        {
            //if (_top.Trim() == "")
                //_top = "@";
            string rightTop = "";

            string _top1 = "";
            ExponentialEquation eq = new ExponentialEquation(SignType.equal);
            if (_top.IndexOf("'='") == -1)
            {
                eq.IsComplete = false;
                _top1 = _top;
            }
            else
            {
                rightTop = _top.Substring(_top.IndexOf("'='") + 3);
                if (rightTop[0] == '%')
                {
                    //eq.SplitSelected = true;
                }
                _top1 = _top.Substring(0, _top.IndexOf("'='"));
                //eq.IsComplete = true;
            }

            if ((_top1.IndexOf("'+'") != -1 || _top1.IndexOf("'-'") != -1))
            {
                #region More than one expression

                //while (_top1.IndexOf("'+'") != -1 || _top1.IndexOf("'-'") != -1)
                //{
                //    string expTop = "";
                //    string expBot = "";

                //    int indexTop = GetClosestIndex(_top1, "'+'", "'-'");

                //    expTop = _top1.Substring(0, indexTop);

                //    eq.Left.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1)));

                //    eq.Left.Add(new Sign(_top1.Substring(indexTop, 3).Trim('\'')));

                //    _top1 = _top1.Substring(indexTop + 3);


                //    if (_top1.IndexOf("'+'") == -1 && _top1.IndexOf("'-'") == -1)
                //    {
                //        if ((_top1.Trim() == "" && _bot1.Trim() == ""))
                //        {
                //            _top1 = "@";
                //            //_myExpressionLevel = ExpressionLevel.Numerator;
                //        }
                //        eq.Left.Add(GetExpression(_top1, (_bot1 == "%") ? "@" : _bot1, new Range(0, _top1.Length - 1), new Range(0, _bot1.Length - 1)));
                //    }
                //}

                #endregion More than one expression
            }
            else
            {
                if(_top1.Trim() != "")
                eq.Left.Add(GetMyTermExpo(_top1));
            }

            if (rightTop.Trim () != "")
            {
                if ((rightTop.IndexOf("'+'") != -1 || rightTop.IndexOf("'-'") != -1))
                {
                    #region More than one expression

                    //while (rightTop.IndexOf("'+'") != -1 || rightTop.IndexOf("'-'") != -1)
                    //{
                    //    string expTop = "";
                    //    string expBot = "";

                    //    int indexTop = GetClosestIndex(rightTop, "'+'", "'-'");
                    //    int indexBot = GetClosestIndex(rightBot, "'+'", "'-'");

                    //    expTop = rightTop.Substring(0, indexTop);
                    //    expBot = rightBot.Substring(0, indexBot);

                    //    eq.Right.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1)));

                    //    eq.Right.Add(new Sign(rightTop.Substring(indexTop, 3).Trim('\'')));

                    //    rightTop = rightTop.Substring(indexTop + 3);
                    //    rightBot = rightBot.Substring(indexBot + 3);


                    //    if (rightTop.IndexOf("'+'") == -1 && rightTop.IndexOf("'-'") == -1)
                    //    {
                    //        if ((rightTop.Trim() == "" && rightBot.Trim() == ""))
                    //        {
                    //            rightTop = "@";
                    //            _myExpressionLevel = ExpressionLevel.Numerator;
                    //        }
                    //        eq.Right.Add(GetExpression(rightTop, (rightBot == "%") ? "@" : rightBot, new Range(0, rightTop.Length - 1), new Range(0, rightBot.Length - 1)));
                    //    }
                    //}

                    #endregion More than one expression
                }
                else
                {
                    if(rightTop.Trim() != "")
                    eq.Right.Add(GetMyTermExpo(rightTop));
                }
            }

            return eq;
        }
        private ExponentialTerm GetMyTermExpo(string value)
        {
            value = value.Replace("(%", "");
            value = value.Replace(")%", "");
            if (value.Trim() == "@" || value.Trim('%') == "")
            {
                ExponentialTerm term = new ExponentialTerm(2);
                term.ExpressJoke = true;
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.BaseExpo;
                term.MySelectedindex = -1;
                return term;
            }
            if (value.Length == 1 && (value[0] == '-' || value[0] == '+'))
            {
                ExponentialTerm term = new ExponentialTerm(1);
                term.Joke = true;
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.BaseExpo;
                term.MySelectedindex = -1;
                return term;
            }
            else if (value == "-%" || value == "+%")
            {
                ExponentialTerm term = new ExponentialTerm(1);
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.BaseExpo;
                term.MySelectedindex = -1;
                term.Joke = true;
                return term;
            }
            else
            {
                int myBase = -1;
                string myPower = "";

                if (value.IndexOf("#") == -1)
                {
                     myBase = int.Parse(value.Trim('%'));
                }
                else
                {
                    myBase = int.Parse(value.Substring(0, value.IndexOf("#")));
                    myPower = value.Substring(value.IndexOf("#")).Trim('#');
                }

                if (myPower.Trim() == "")
                {
                    ExponentialTerm term =   new ExponentialTerm(myBase);
                    if (value.IndexOf("%") != -1)
                    {
                        term.Selected = true;
                        term.MySelectedindex = -1;
                        term.MySelectedPieceType = SelectedPieceType.BaseExpo;
                    }
                    return term;
                }
                else
                {
                    MathBase.Expression exp = GetExpression(myPower, "", new Range(0, myPower.Length - 1), new Range(0, 0));
                    return new ExponentialTerm(myBase, exp);
                }
            }
        }
        private int GetClosestIndex(string source, string searchOne, string searchTwo)
        {
            if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) != -1)
            {
                return (source.IndexOf(searchTwo) < source.IndexOf(searchOne)) ?
                    source.IndexOf(searchTwo) : source.IndexOf(searchOne);
            }
            else if (source.IndexOf(searchOne) == -1 && source.IndexOf(searchTwo) != -1)
            {
                return source.IndexOf(searchTwo);
            }
            else if (source.IndexOf(searchOne) != -1 && source.IndexOf(searchTwo) == -1)
            {
                return source.IndexOf(searchOne);
            }
            return -1;
        }
        private Term GetMyTerm(string value)
        {
            value = value.Replace("(%", "");
            value = value.Replace(")%", "");
            if (value.Trim() == "@" || value.Trim('%') == "")
            {
                Term term = new Term(2);
                term.ExpressJoke = true;
                term.Sign = "+";
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            if (value.Length == 1 && (value[0] == '-' || value[0] == '+'))
            {
                Term term = new Term(1);
                term.Sign = value;
                term.Joke = true;
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                return term;
            }
            else if (value == "-%" || value == "+%")
            {
                Term term = new Term(1);
                term.Sign = value[0].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                term.Joke = true;
                return term;
            }
            else
            {
                #region Term TT
                string[] pieces = value.Split('.');
                Term term = null;
                string sign = "+";
                if (pieces.Length > 1)
                    throw new NotImplementedException();// do something about them multiples...

                if (pieces[0].StartsWith("-"))
                {
                    sign = "-";
                }
                pieces[0] = pieces[0].Trim('-', '+');

                string coeF = "";
                char _base = ' ';
                string power = "";
                int indexPow = -3;
                int indexBase = -3;
                int indexCoe = -3;
                LookingFor lookingFor = LookingFor.myC;
                LookingFor foundFor = LookingFor.None;
                bool outsideAfter = false;
                bool outsideBefore = false;

                for (int i = 0; i < pieces[0].Length; i++)
                {
                    if (lookingFor != LookingFor.None)
                    {
                        #region Looking
                        int valueInt = -1;

                        if (int.TryParse(pieces[0][i].ToString(), out valueInt))
                        {
                            if (lookingFor == LookingFor.myC)
                                coeF += pieces[0][i].ToString();
                            else if (lookingFor == LookingFor.myP)
                                power += pieces[0][i].ToString();
                        }
                        else if (pieces[0][i] == '%')
                        {
                            if (i == 0)
                            {
                                foundFor = lookingFor;
                                outsideBefore = true;
                            }
                            else if (i == pieces[0].Length - 1 && lookingFor != LookingFor.myP)
                            {
                                outsideAfter = true;
                                foundFor = lookingFor;
                            }
                            else
                            {
                                if (lookingFor == LookingFor.myC)
                                    indexCoe = coeF.Length - 1;
                                else if (lookingFor == LookingFor.myP)
                                    indexPow = power.Length - 1;
                            }
                        }
                        else if (pieces[0][i] == '%')
                        {
                            //if (lookingFor == LookingFor.myB)
                            //{
                            //    lookingFor = LookingFor.myP;
                            //    _base += pieces[0][i];
                            //}
                            if (lookingFor == LookingFor.myP)
                            {
                                indexPow = power.Length - 1;
                            }
                            else throw new NotImplementedException();

                        }
                        else if (pieces[0][i] == '^')
                        {
                            if (lookingFor != LookingFor.myP)
                                lookingFor = LookingFor.myP;
                            else
                                lookingFor = LookingFor.None;
                        }
                        else if ("abcdefghijklmnopqrstuvwxyz".IndexOf(pieces[0][i]) != -1)
                        {
                            _base = pieces[0][i];
                            if (i + 1 < pieces[0].Length)
                                if (pieces[0][i + 1] == '%')
                                {
                                    indexBase = 0; ;
                                    i++;
                                }
                            //lookingFor = LookingFor.myP;
                        }
                        #endregion Looking
                    }
                    else
                    {
                        if (pieces[0][i] == '%')
                            outsideAfter = true;
                    }
                }

                SelectionCursor cursor = null;

                if (indexPow != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Power, indexPow);
                else if (indexCoe != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Coefficient, indexCoe);
                if (indexBase != -3)
                    cursor = new SelectionCursor(SelectedPieceType.Base, indexBase);
                else if (outsideBefore)
                    cursor = new SelectionCursor(GetFoundFor(foundFor), -1);
                else if (outsideAfter)
                    cursor = new SelectionCursor(SelectedPieceType.OutSide, -2);

                if (power.Length == 0)
                    power = "1";


                if (coeF.Length != 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length != 0 && _base.ToString().Trim() == "" && power.Length != 0)
                {
                    term = new Term(int.Parse(coeF), int.Parse(power));
                }
                else if (coeF.Length == 0 && _base.ToString().Trim() != "" && power.Length != 0)
                {
                    term = new Term(_base, int.Parse(power));
                }

                if (cursor != null && term != null)
                {
                    term.Selected = true;
                    term.MySelectedindex = cursor.Index;
                    term.MySelectedPieceType = cursor.MySelectedPieceType;
                }
                term.Sign = sign;

                return term;

                #endregion Term TT
            }
            //throw new NotImplementedException();
        }
        public SelectedPieceType GetFoundFor(LookingFor fo)
        {
            if (fo == LookingFor.myB)
                return SelectedPieceType.Base;
            else if (fo == LookingFor.myC)
                return SelectedPieceType.Coefficient;
            else if (fo == LookingFor.myP)
                return SelectedPieceType.Power;
            else return SelectedPieceType.Base;
        }
        private MathBase.Expression GetExpression(string sourceTop, string sourceBot, Range rangeTop, Range rangeBot)
        {
            MathBase.Expression exp = new MathBase.Expression();

            #region Numerator

            string piece = sourceTop.Substring(rangeTop.Start, 1 + (rangeTop.End - rangeTop.Start));

            if (piece.Length == 2 || piece.Length == 1)
            {
                if (piece[0] == '(' || piece[0] == ')')
                {
                    Brace brace = new Brace(piece[0]);
                    if (piece.IndexOf("%") != -1)
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                }
                else if (piece == "@")
                {
                    Term term = new Term(1);
                    term.ExpressJoke = true;
                    term.Sign = "+";
                    term.Selected = true;
                    term.MySelectedPieceType = SelectedPieceType.Coefficient;
                    term.MySelectedindex = -1;
                    exp.Numerator.Add(term);
                }
                else
                {
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece));
                }
            }
            else
            {
                int startIndex = 0;
                for (int i = 0; i < piece.Length; i++)
                {
                    Act(ref exp, ref piece, i, ref startIndex);
                }
            }

            #endregion Numerator

            #region Denominator

            if (sourceBot.Trim() != "")
            {
                MathBase.Expression dimExp = new MathBase.Expression();
                string pieceBot = sourceBot.Substring(rangeBot.Start, 1 + (rangeBot.End - rangeBot.Start));

                if (pieceBot.Length == 2 || pieceBot.Length == 1)
                {
                    if (pieceBot[0] == '(' || pieceBot[0] == ')')
                    {
                        dimExp.Numerator.Add(new Brace(pieceBot[0]));
                    }
                    else
                    {
                        if (pieceBot.Trim() != "%")
                            dimExp.Numerator.Add(GetMyTerm(pieceBot));
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (int i = 0; i < pieceBot.Length; i++)
                    {
                        Act(ref dimExp, ref pieceBot, i, ref startIndex);
                    }
                }

                exp.Denominator = new List<IExpressionPiece>(dimExp.Numerator);
            }

            #endregion Denominator

            return exp;
        }
        private void Decoy()
        {

        }
        private void Act(ref MathBase.Expression exp, ref string piece, int i, ref int startIndex)
        {
            if (piece[i] == '(' || piece[i] == ')')
            {
                if (startIndex == i)
                {
                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                }
                else if (i != 0 && piece[i - 1] != '(' && piece[i - 1] != ')')
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));

                    Brace brace = new Brace(piece[i]);
                    if (piece.Length > i + 1 && piece[i + 1] == '%')
                        brace.Selected = true;
                    exp.Numerator.Add(brace);
                    startIndex = i + 1;
                    //there is your fucken term bro!!
                }
            }
            else if ((i != piece.Length - 1 && i != startIndex && (piece[i] == '-' || piece[i] == '+')))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex)));
                startIndex = i;
            }
            else if ((i == piece.Length - 1 && (piece[i] == '-' || piece[i] == '+')))
            {
                if (i - 1 > 0 && piece[i - 1] == ')')
                {
                    startIndex = i + 1;
                }
                else
                {
                    if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                        startIndex -= 1;

                    int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
                        (i - startIndex) + 1 : (i - startIndex);
                    //there is your fucken term bro!!!
                    char part = piece[(i - startIndex) + startIndex];
                    if (piece.Trim() != "%")
                        exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                    startIndex = i;
                }
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i] == '+' || piece[i] == '-')))
            {
                throw new NotImplementedException();
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i == startIndex && (piece[i - 1] == '+' || piece[i - 1] == '-')
                && piece[i] == '%'))
            {
                Term term = new Term(2);
                term.Joke = true;
                term.Sign = piece[i - 1].ToString();
                term.Selected = true;
                term.MySelectedPieceType = SelectedPieceType.Coefficient;
                term.MySelectedindex = -1;
                exp.Numerator.Add(term);
                startIndex = i + 1;
                //add a joker term for show
            }
            else if ((i == piece.Length - 1 && i != startIndex))
            {
                if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
                    startIndex -= 1;

                int endIndex = (i - startIndex);
                if (piece.Trim() != "%")
                    exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1)));
                startIndex = i + 1;
            }
        }
        public EquationType GetEquationType()
        {
            return EquationType.Exponential;
        }

        #endregion Methods
    }
    public enum TermLevel
    {
        Base, Power
    }
    public enum ExpressionLevel
    {
        Numerator, Denominator, MiddleSigns
    }
    public enum LookingFor
    {
        myC, myB, myP, None
    }
}
