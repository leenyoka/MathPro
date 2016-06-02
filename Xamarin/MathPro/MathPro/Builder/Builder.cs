using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;
using MathPro.Builders;
using MathPro.Display;

namespace MathPro.Builder
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
			_topBottoms = new[] { top, bot };
			_termLevel = termLevel;
			_expLevel = expLevel;
		}
		public History(string top, string bot, TermLevel expLevel, TermLevel termLevel)
		{
			_topBottoms = new[] { top, bot };
			_termLevel = termLevel;
			_termLevel2 = expLevel;
		}

		#endregion Constructor

		#region Methods

		public bool Changed(History piece)
		{
			if (piece._expLevel != _expLevel ||
				piece._termLevel != _termLevel ||
				piece._topBottoms[0] != _topBottoms[0] ||
				piece._topBottoms[1] != _topBottoms[1])
				return true;
			return false;
		}

		public bool Changed2(History piece)
		{
			if (piece._termLevel2 != _termLevel2 ||
				piece._termLevel != _termLevel ||
				piece._topBottoms[0] != _topBottoms[0] ||
				piece._topBottoms[1] != _topBottoms[1])
				return true;
			return false;
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
		MyC, MyB, MyP, None
	}
	public sealed class GeneralBuilder : LinearLayout, IProblemBuilder
	{
		#region Properties

		private Keyboard _keyboard;
		private readonly Helper _helper = new Helper();
		TermLevel _myTermLevel;
		ExpressionLevel _myExpressionLevel;
		string _top = "";

		public new string Top
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

		readonly List<History> _myHistory = new List<History>();
		readonly LinearLayout _myDisplayArea;

		public EquationType EqType { get; set; }

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
		DispatcherTimer _timer;

		public DispatcherTimer Timer
		{
			get { return _timer; }
			set { _timer = value; }
		}
		public GeneralBuilder(ref DispatcherTimer timer, EquationType eqType, MathBase.Action action,  Context context)
			:base(context)
		{
			Orientation = Orientation.Vertical;
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			_myDisplayArea = new LinearLayout(context)
			{
				LayoutParameters =
					new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
			};
			
			timer.Tick += ToggleSelected;
			Timer = timer;
			AddView(new BuilderDisplayArea(context, _myDisplayArea));
			_keyboard = GetKeyboard(context);
			AddView(_keyboard);
			_action = action;
			_context = context;
			EqType = eqType;
		}
		public GeneralBuilder(EquationType eqType, MathBase.Action action, Context context)
			: base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			_myDisplayArea = new LinearLayout(context)
			{
				LayoutParameters =
					new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
			};

			_myDisplayArea.Layout(32, 25, 0, 32);
			AddView(new BuilderDisplayArea(context, _myDisplayArea));
			_keyboard = GetKeyboard(context);
			AddView(_keyboard);
			_action = action;
			_context = context;
			EqType = eqType;
		}

		#endregion Constructor

		#region Methods
		public void Reset(Context context)
		{
			RemoveView(_keyboard);
			_keyboard = GetKeyboard(context);
			AddView(_keyboard);
			Draw(context);
		}

		/*
		private void AddDeleteKey()
		{
			Grid grid = new Grid();
			grid.Width = 50;
			grid.Height = 50;
			//grid.Background = GetBackGround();
			//grid.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(DeleteFromStorage);
			grid.Layout(700, 0, 0, 0);
			AddView(grid);
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
			Visibility = System.Windows.Visibility.Collapsed;
		}
		*/
		public void ToggleSelected(object sender, EventArgs e)
		{
		}

		private readonly Context _context;
		public void InputHandler(object sender, EventArgs e)
		{
			try
			{
				#region Do this ish
				var ignore = false;
				var startedWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

				var caller = (IKeyboardItemId)sender;

				if (_myExpressionLevel == ExpressionLevel.Numerator)
					_top = _top.Replace("@", "");
				else if (_myExpressionLevel == ExpressionLevel.Denominator)
					_bot = _bot.Replace("@", "");

				if (caller.GetId() == KeyBoardItemIdType.Other)
				{
					if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
					{
						Undo(_context);
						ignore = true;
					}
					else Change((Other)caller);
				}
				else if (caller.GetId() == KeyBoardItemIdType.NumberVar)
				{
					#region Number Or Variable
					//Add.
					if (_myExpressionLevel == ExpressionLevel.Numerator)
					{
						_top = _top.Replace("@", "");
						if (_top.IndexOf("%", StringComparison.Ordinal) == -1)
							_top += ((NumberVar)caller).MyVariableNumberValue + "%";
						else
							_top = _top.Replace("%", (((NumberVar)caller).MyVariableNumberValue + "%"));
						Draw(ref _timer, _context);
					}
					else if (_myExpressionLevel == ExpressionLevel.Denominator)
					{
						_bot = _bot.Replace("@", "");
						_bot = _bot.Trim('%');
						_bot += ((NumberVar)caller).MyVariableNumberValue + "%";
						Draw(ref _timer, _context);
					}
					#endregion Number Or Variable
				}
				else
				{
					SignCounterAct((KeyboardSign)caller, _context);
				}

				var endedUpWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

				if (!ignore && (startedWith.Changed(endedUpWith)))
				{
					_myHistory.Add(startedWith);
				}

				#endregion Do this ish
			}
			catch
			{
				Undo(_context);
				//_errorHandler(this, e);
				//Draw(ref timer);
			}
		}
		private void Undo(Context context)
		{
			if (_myHistory.Count <= 0) return;
			for (var i = _myHistory.Count - 1; i >= 0; i--)
			{
				if (_myHistory[i] == null) continue;
				var previous = _myHistory[i];
				_top = previous.TopBottoms[0];
				_bot = previous.TopBottoms[1];
				_myExpressionLevel = previous.ExpLevel;
				_myTermLevel = previous.TermLevel;
				_myHistory[i] = null;
				Draw(ref _timer, context);
				return;
			}
		}

		private void SignCounterAct(KeyboardSign sign, Context context)
		{
			#region plus minus Mult

			if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "+%");
				else _top += "+";
			}
			else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
			{
				if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
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
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "-%");
				else _top += "-";
			}
			else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
			{
				if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
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
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "*%");
				else _top += "*";
			}
			else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "*")
			{
				if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
					_bot = _bot.Replace("%", "*%");
				else _bot += "*";
			}
			else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "*" && _action == MathBase.Action.Simplify)
			{
				_top += "'*'";
				_bot += "'*'";
			}
			else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "/" && _action == MathBase.Action.Simplify)
			{
				_top += "'/'";
				_bot += "'/'";
			}
			#endregion plus minus

			#region braces

			else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "(%");
				else _top += "(%";
			}
			else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
			{
				if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
					_bot = _bot.Replace("%", "(%");
				else _bot += "(%";
			}
			else if (_myExpressionLevel == ExpressionLevel.Numerator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", ")%");
				else _top += ")%";
			}
			else if (_myExpressionLevel == ExpressionLevel.Denominator && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
			{
				if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
					_bot = _bot.Replace("%", ")%");
				else _bot += ")%";
			}

			#endregion braces

			#region Split

			else if (_myTermLevel == TermLevel.Base && sign.MyKey == "=" && _top.IndexOf('=') == -1)
			{
				_myExpressionLevel = ExpressionLevel.Numerator;
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "'='%");
				else _top += "'='%";

				if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
					_bot = _bot.Replace("%", "'='");
				else _bot += "'='";
			}

			#endregion Split

			else throw new NotImplementedException();
			Draw(ref _timer, context);
		}
		public bool Change(Other other)
		{
			var draw = false;

			#region Small
			if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base)
			{
				if (_myExpressionLevel == ExpressionLevel.Numerator)
				{
					if (_top.IndexOf("%", StringComparison.Ordinal) == -1)
						_top += "^";
					else _top = _top.Replace("%", "^%");
				}
				else if (_myExpressionLevel == ExpressionLevel.Denominator)
				{
					if (_bot.IndexOf("%", StringComparison.Ordinal) == -1)
						_bot += "^";
					else _bot = _bot.Replace("%", "^%");
				}

				_myTermLevel = TermLevel.Power;
				draw = true;
			}
			else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power)
			{
				if (_myExpressionLevel == ExpressionLevel.Numerator)
				{
					if (_top.IndexOf("%", StringComparison.Ordinal) == -1)
						_top += "^";
					else _top = _top.Replace("%", "^%");
					_myTermLevel = TermLevel.Base;
				}


				else if (_myExpressionLevel == ExpressionLevel.Denominator)
				{
					if (_bot.IndexOf("%", StringComparison.Ordinal) == -1)
						_bot += "^";
					else _bot = _bot.Replace("%", "^%");
					_myTermLevel = TermLevel.Base;
				}

				draw = true;
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
					draw = true;
				}
				else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
				{
					_myExpressionLevel = ExpressionLevel.Numerator;
					//throw new NotImplementedException(); // remove selection from the Middle sign
					_top += "%";

					draw = true;
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

					draw = true;

				}
				else if (_myExpressionLevel == ExpressionLevel.MiddleSigns)
				{
					_myExpressionLevel = ExpressionLevel.Denominator;
					if (_bot.Trim() != "")
						_bot += "%";
					else _bot = "@";
					draw = true;
				}
			}
			#endregion Big

			if (_action == MathBase.Action.Plot && other.MyKeyboardItem == KeyboardItem.F)
			{
				if (_myExpressionLevel == ExpressionLevel.Numerator)
				{
					if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
						_top =_top.Replace("%", "f%");
					else _top += "f";
					draw = true;
				}
				else
				{
					if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
						_bot =_bot.Replace("%", "f%");
					else _bot += "f";
					draw = true;
				}
			}

			if (!draw) return false;
			Draw(ref _timer, _context);
			return true;
		}
		EventHandler _handler;
		public Keyboard GetKeyboard(Context context)
		{
			_handler = InputHandler;
			var answer = new Keyboard(ref _handler,context);
			return answer;
		}
		public void Draw(ref DispatcherTimer timer, Context context)
		{
			if (_top.Length > 0 || _bot.Length > 0)
			{
				_myDisplayArea.RemoveAllViews();//.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), ref timer,context));
			}
			else
			{
				_top = "@";
				timer.Tick += ToggleSelected;
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), ref timer, context));
			}
		}
		private void Draw(Context context)
		{
			if (_top.Length > 0 || _bot.Length > 0)
			{
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), context));
			}
		}
		EventHandler _errorHandler;
		public EventHandler ErrorHandler
		{
			get { return _errorHandler; }
			set { _errorHandler = value; }
		}
		public Equation GetEquation(Context context)
		{
			if (_top.Trim() == "" && _bot.Trim() != "")
				_top = "+1";
			var rightTop = "";
			var rightBot = "";
			var top1 = "";
			var bot1 = "";
			var eq = new Equation(SignType.equal);
			if (_top.IndexOf("'='", StringComparison.Ordinal) == -1)
			{
				eq.IsComplete = false;
				top1 = _top;
				bot1 = _bot;
			}
			else
			{
				rightTop = _top.Substring(_top.IndexOf("'='", StringComparison.Ordinal) + 3);
				if (rightTop[0] == '%')
				{
					eq.SplitSelected = true;
				}
				top1 = _top.Substring(0, _top.IndexOf("'='", StringComparison.Ordinal));

				rightBot = _bot.Substring(_bot.IndexOf("'='", StringComparison.Ordinal) + 3);
				bot1 = _bot.Substring(0, _bot.IndexOf("'='", StringComparison.Ordinal));
				eq.IsComplete = true;
			}

			if ((top1.IndexOf("'+'", StringComparison.Ordinal) != -1 || top1.IndexOf("'-'", StringComparison.Ordinal) != -1 
				|| top1.IndexOf("'*'", StringComparison.Ordinal) != -1 || top1.IndexOf("'/'", StringComparison.Ordinal) != -1))
			{
				#region More than one expression

				while (top1.IndexOf("'+'", StringComparison.Ordinal) != -1 || top1.IndexOf("'-'", StringComparison.Ordinal) != -1 
					|| top1.IndexOf("'*'", StringComparison.Ordinal) != -1 || top1.IndexOf("'/'", StringComparison.Ordinal) != -1)
				{
					var expTop = "";
					var expBot = "";

					var indexTop = GetClosestIndex(top1, GetClosestIndex(top1, GetClosestIndex(top1, "'+'", "'-'"), "'*'"), "'/'");
					var indexBot = GetClosestIndex(bot1, GetClosestIndex(bot1, GetClosestIndex(bot1, "'+'", "'-'"), "'*'"), "'/'");

					expTop = top1.Substring(0, indexTop);
					expBot = _bot.Substring(0, indexBot);

					eq.Left.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1), context));

					eq.Left.Add(new Sign(top1.Substring(indexTop, 3).Trim('\'')));

					top1 = top1.Substring(indexTop + 3);
					bot1 = bot1.Substring(indexBot + 3);


					if (top1.IndexOf("'+'", StringComparison.Ordinal) == -1 && top1.IndexOf("'-'", StringComparison.Ordinal) == -1 
						&& top1.IndexOf("'*'", StringComparison.Ordinal) == -1 && top1.IndexOf("'/'", StringComparison.Ordinal) == -1)
					{
						if ((top1.Trim() == "" && bot1.Trim() == ""))
						{
							top1 = "@";
							_myExpressionLevel = ExpressionLevel.Numerator;
						}
						eq.Left.Add(GetExpression(top1, (bot1 == "%") ? "@" : bot1, new Range(0, top1.Length - 1), new Range(0, bot1.Length - 1), context));
					}
				}

				#endregion More than one expression
			}
			else
			{
				eq.Left.Add(GetExpression(top1, bot1, new Range(0, top1.Length - 1), new Range(0, bot1.Length - 1), context));
			}

			if (eq.IsComplete)
			{
				if ((rightTop.IndexOf("'+'", StringComparison.Ordinal) != -1 || rightTop.IndexOf("'-'", StringComparison.Ordinal) != -1 
					|| top1.IndexOf("'*'", StringComparison.Ordinal) != -1 || top1.IndexOf("'/'", StringComparison.Ordinal) != -1))
				{
					#region More than one expression

					while (rightTop.IndexOf("'+'", StringComparison.Ordinal) != -1 || rightTop.IndexOf("'-'", StringComparison.Ordinal) != -1 
						|| top1.IndexOf("'/'", StringComparison.Ordinal) != -1)
					{
						var expTop = "";
						var expBot = "";

						var indexTop = GetClosestIndex(rightTop, GetClosestIndex(rightTop, GetClosestIndex(rightTop, "'+'", "'-'"), "'*'"), "'/'");
						var indexBot = GetClosestIndex(rightBot, GetClosestIndex(rightBot, GetClosestIndex(rightBot, "'+'", "'-'"), "'*'"), "'/'");

						expTop = rightTop.Substring(0, indexTop);
						expBot = rightBot.Substring(0, indexBot);

						eq.Right.Add(GetExpression(expTop, expBot, new Range(0, expTop.Length - 1), new Range(0, expBot.Length - 1),context));

						eq.Right.Add(new Sign(rightTop.Substring(indexTop, 3).Trim('\'')));

						rightTop = rightTop.Substring(indexTop + 3);
						rightBot = rightBot.Substring(indexBot + 3);


						if (rightTop.IndexOf("'+'", StringComparison.Ordinal) == -1 && rightTop.IndexOf("'-'", StringComparison.Ordinal) == -1
							&& rightTop.IndexOf("'*'", StringComparison.Ordinal) == -1 && top1.IndexOf("'/'", StringComparison.Ordinal) == -1)
						{
							if ((rightTop.Trim() == "" && rightBot.Trim() == ""))
							{
								rightTop = "@";
								_myExpressionLevel = ExpressionLevel.Numerator;
							}
							eq.Right.Add(GetExpression(rightTop, (rightBot == "%") ? "@" : rightBot, new Range(0, rightTop.Length - 1), new Range(0, rightBot.Length - 1),context));
						}
					}

					#endregion More than one expression
				}
				else
					eq.Right.Add(GetExpression(rightTop, rightBot, new Range(0, rightTop.Length - 1), new Range(0, rightBot.Length - 1),context));
			}

			return eq;
		}
		private int GetClosestIndex(string source, string searchOne, string searchTwo)
		{
			if (source.IndexOf(searchOne, StringComparison.Ordinal) != -1 && source.IndexOf(searchTwo, StringComparison.Ordinal) != -1)
			{
				return (source.IndexOf(searchTwo, StringComparison.Ordinal) < source.IndexOf(searchOne, StringComparison.Ordinal)) ?
					source.IndexOf(searchTwo, StringComparison.Ordinal) : source.IndexOf(searchOne, StringComparison.Ordinal);
			}
			if (source.IndexOf(searchOne, StringComparison.Ordinal) == -1 && source.IndexOf(searchTwo, StringComparison.Ordinal) != -1)
			{
				return source.IndexOf(searchTwo, StringComparison.Ordinal);
			}
			if (source.IndexOf(searchOne, StringComparison.Ordinal) != -1 && source.IndexOf(searchTwo, StringComparison.Ordinal) == -1)
			{
				return source.IndexOf(searchOne, StringComparison.Ordinal);
			}
			return -1;
		}
		private int GetClosestIndex(string source, int prev, string search)
		{
			if (source.IndexOf(search, StringComparison.Ordinal) == -1)
				return prev;
			if (source.IndexOf(search, StringComparison.Ordinal) < prev)
				return source.IndexOf(search, StringComparison.Ordinal);
			if (prev == -1)
				return source.IndexOf(search, StringComparison.Ordinal);
			return prev;
		}

		private Term GetMyTerm(string value, Context context)
		{
			value = value.Replace("(%", "");
			value = value.Replace(")%", "");
			if (value.Trim() == "@" || value.Trim('%') == "")
			{
				var term = new Term(2)
				{
					ExpressJoke = true,
					Sign = "+",
					Selected = true,
					MySelectedPieceType = SelectedPieceType.Coefficient,
					MySelectedindex = -1
				};
				return term;
			}
			if (value.Length == 1 && (value[0] == '-' || value[0] == '+'))
			{
				var term = new Term(1)
				{
					Sign = value,
					Joke = true,
					Selected = true,
					MySelectedPieceType = SelectedPieceType.Coefficient,
					MySelectedindex = -1
				};
				return term;
			}
			if (value == "-%" || value == "+%")
			{
				var term = new Term(1)
				{
					Sign = value[0].ToString(CultureInfo.InvariantCulture),
					Selected = true,
					MySelectedPieceType = SelectedPieceType.Coefficient,
					MySelectedindex = -1,
					Joke = true
				};
				return term;
			}
			else
			{
				#region Term TT
				var pieces = value.Split('.');
				Term term = null;
				var sign = "+";
				if (pieces.Length > 1)
					throw new NotImplementedException();// do something about them multiples...

				if (pieces[0].StartsWith("-"))
				{
					sign = "-";
				}
				pieces[0] = pieces[0].Trim('-', '+');

				var coeF = "";
				var _base = ' ';
				var power = "";
				var indexPow = -3;
				var indexBase = -3;
				var indexCoe = -3;
				var lookingFor = LookingFor.MyC;
				var foundFor = LookingFor.None;
				var outsideAfter = false;
				var outsideBefore = false;

				for (int i = 0; i < pieces[0].Length; i++)
				{
					if (lookingFor != LookingFor.None)
					{
						#region Looking
						var valueInt = -1;

						if (int.TryParse(pieces[0][i].ToString(CultureInfo.InvariantCulture), out valueInt))
						{
							switch (lookingFor)
							{
								case LookingFor.MyC:
									coeF += pieces[0][i].ToString(CultureInfo.InvariantCulture);
									break;
								case LookingFor.MyP:
									power += pieces[0][i].ToString(CultureInfo.InvariantCulture);
									break;
							}
						}
						else if (pieces[0][i] == '%')
						{
							if (i == 0)
							{
								foundFor = lookingFor;
								outsideBefore = true;
							}
							else if (i == pieces[0].Length - 1 && lookingFor != LookingFor.MyP)
							{
								outsideAfter = true;
								foundFor = lookingFor;
							}
							else
							{
								switch (lookingFor)
								{
									case LookingFor.MyC:
										indexCoe = coeF.Length - 1;
										break;
									case LookingFor.MyP:
										indexPow = power.Length - 1;
										break;
								}
							}
						}
						else if (pieces[0][i] == '%')
						{
							//if (lookingFor == LookingFor.myB)
							//{
							//    lookingFor = LookingFor.myP;
							//    _base += pieces[0][i];
							//}
							if (lookingFor == LookingFor.MyP)
							{
								indexPow = power.Length - 1;
							}
							else throw new NotImplementedException();

						}
						else if (pieces[0][i] == '^')
						{
							lookingFor = lookingFor != LookingFor.MyP ? LookingFor.MyP : LookingFor.None;
						}
						else if ("abcdefghijklmnopqrstuvwxyz".IndexOf(pieces[0][i]) != -1)
						{
							_base = pieces[0][i];
							if (i + 1 >= pieces[0].Length) continue;
							if (pieces[0][i + 1] != '%') continue;
							indexBase = 0;
							i++;
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
					cursor = new SelectionCursor(SelectedPieceType.Power, indexPow,context);
				else if (indexCoe != -3)
					cursor = new SelectionCursor(SelectedPieceType.Coefficient, indexCoe, context);
				if (indexBase != -3)
					cursor = new SelectionCursor(SelectedPieceType.Base, indexBase, context);
				else if (outsideBefore)
					cursor = new SelectionCursor(GetFoundFor(foundFor), -1, context);
				else if (outsideAfter)
					cursor = new SelectionCursor(SelectedPieceType.OutSide, -2, context);

				if (power.Length == 0)
					power = "1";


				if (coeF.Length != 0 && _base.ToString(CultureInfo.InvariantCulture).Trim() != "" && power.Length != 0)
				{
					term = new Term(_base, int.Parse(coeF), int.Parse(power));
				}
				else if (coeF.Length != 0 && _base.ToString(CultureInfo.InvariantCulture).Trim() == "" && power.Length != 0)
				{
					term = new Term(int.Parse(coeF), int.Parse(power));
				}
				else if (coeF.Length == 0 && _base.ToString(CultureInfo.InvariantCulture).Trim() != "" && power.Length != 0)
				{
					term = new Term(_base, int.Parse(power));
				}

				if (cursor != null && term != null)
				{
					term.Selected = true;
					term.MySelectedindex = cursor.Index;
					term.MySelectedPieceType = cursor.MySelectedPieceType;
				}
				if (term != null)
				{
					term.Sign = sign;

					return term;
				}

				#endregion Term TT
			}
			throw new NotImplementedException();
		}
		public SelectedPieceType GetFoundFor(LookingFor fo)
		{
			if (fo == LookingFor.MyB)
				return SelectedPieceType.Base;
			if (fo == LookingFor.MyC)
				return SelectedPieceType.Coefficient;
			if (fo == LookingFor.MyP)
				return SelectedPieceType.Power;
			return SelectedPieceType.Base;
		}

		private Expression GetExpression(string sourceTop, string sourceBot, Range rangeTop, Range rangeBot, Context context)
		{
			var exp = new Expression();

			#region Numerator

			string piece = sourceTop.Substring(rangeTop.Start, 1 + (rangeTop.End - rangeTop.Start));

			if (piece.Length == 2 || piece.Length == 1)
			{
				if (piece[0] == '(' || piece[0] == ')')
				{
					var brace = new Brace(piece[0]);
					if (piece.IndexOf("%", StringComparison.Ordinal) != -1)
						brace.Selected = true;
					exp.Numerator.Add(brace);
				}
				else if (piece == "@")
				{
					var term = new Term(1)
					{
						ExpressJoke = true,
						Sign = "+",
						Selected = true,
						MySelectedPieceType = SelectedPieceType.Coefficient,
						MySelectedindex = -1
					};
					exp.Numerator.Add(term);
				}
				else
				{
					if (piece.Trim() != "%")
						exp.Numerator.Add(GetMyTerm(piece, context));
				}
			}
			else
			{
				var startIndex = 0;
				for (int i = 0; i < piece.Length; i++)
				{
					Act(ref exp, ref piece, i, ref startIndex, context);
				}
			}

			#endregion Numerator

			#region Denominator

			if (sourceBot.Trim() != "")
			{
				var dimExp = new Expression();
				var pieceBot = sourceBot.Substring(rangeBot.Start, 1 + (rangeBot.End - rangeBot.Start));

				if (pieceBot.Length == 2 || pieceBot.Length == 1)
				{
					if (pieceBot[0] == '(' || pieceBot[0] == ')')
					{
						dimExp.Numerator.Add(new Brace(pieceBot[0]));
					}
					else
					{
						if (pieceBot.Trim() != "%")
							dimExp.Numerator.Add(GetMyTerm(pieceBot, context));
					}
				}
				else
				{
					var startIndex = 0;
					for (var i = 0; i < pieceBot.Length; i++)
					{
						Act(ref dimExp, ref pieceBot, i, ref startIndex, context);
					}
				}

				exp.Denominator = new List<IExpressionPiece>(dimExp.Numerator);
			}

			#endregion Denominator

			return exp;
		}

		private void Act(ref Expression exp, ref string piece, int i, ref int startIndex, Context context)
		{
			if (piece[i] == '(' || piece[i] == ')')
			{
				if (startIndex == i)
				{
					var brace = new Brace(piece[i]);
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
						exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1), context));

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
					exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex), context));
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
					if (piece.Trim() != "%")
						exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1), context));
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
				var term = new Term(2)
				{
					Joke = true,
					Sign = piece[i - 1].ToString(CultureInfo.InvariantCulture),
					Selected = true,
					MySelectedPieceType = SelectedPieceType.Coefficient,
					MySelectedindex = -1
				};
				exp.Numerator.Add(term);
				startIndex = i + 1;
				//add a joker term for show
			}
			else if ((i == piece.Length - 1 && i != startIndex))
			{
				if (startIndex - 1 > 0 && piece[startIndex - 1] == '-')
					startIndex -= 1;

				var endIndex = (i - startIndex);
				if (piece.Trim() != "%")
					exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1), context));
				startIndex = i + 1;
			}
		}
		public EquationType GetEquationType()
		{
			return EquationType.General;
		}
		#endregion Methods
	}
}