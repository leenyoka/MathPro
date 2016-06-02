using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;
using MathPro.Builders;
using MathPro.Display;

namespace MathPro.Builder
{
	public sealed class LogarithimsBuilder : LinearLayout, IProblemBuilder
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		TermLevel _myTermLevel;
		ExpressionLevel _myExpressionLevel = ExpressionLevel.Numerator;
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
		List<History> myHistory = new List<History>();
		 LinearLayout _myDisplayArea;
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
		public Keyboard Keyboard;
		public LogarithimsBuilder(ref DispatcherTimer timer, Context context)
			:base(context)
		{
			Orientation = Orientation.Vertical;
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			_myDisplayArea = new LinearLayout(context)
			{
				LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
			};
			_myDisplayArea.Layout(32, 25, 0, 51);
			timer.Tick += ToggleSelected;
			_timer = timer;
			_context = context;
			AddView(new BuilderDisplayArea(context, _myDisplayArea));
			Keyboard = GetKeyboard(context);
			AddView(Keyboard);
		}

		#endregion Constructor

		#region Methods
		



		EventHandler _errorHandler;
		public EventHandler ErrorHandler
		{
			get { return _errorHandler; }
			set { _errorHandler = value; }
		}
		public void ToggleSelected(object sender, EventArgs e)
		{
		}

		private readonly Context _context;
		public void InputHandler(object sender, EventArgs e)
		{
			try
			{
				#region Food
				bool ignore = false;
				History startedWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

				IKeyboardItemId caller = (IKeyboardItemId)sender;

				if (_myExpressionLevel == ExpressionLevel.Numerator)
					_top = _top.Replace("@", "");

				if (caller.GetId() == KeyBoardItemIdType.Other)
				{
					if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
					{
						Undo(_context);
						ignore = true;
					}
					else Change((Other)caller,_context);
				}
				else if (caller.GetId() == KeyBoardItemIdType.NumberVar)
				{
					#region Number Or Variable
					//Add.
					if (_myExpressionLevel == ExpressionLevel.Numerator)
					{
						_top += ((NumberVar)caller).MyVariableNumberValue;
						Draw(ref _timer,_context);
					}
					else if (_myExpressionLevel == ExpressionLevel.Denominator)
					{
						_top += ((NumberVar)caller).MyVariableNumberValue;
						Draw(ref _timer,_context);
					}
					#endregion Number Or Variable
				}
				else
				{
					SignCounterAct((KeyboardSign)caller,_context);
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
				Undo(_context);
				_errorHandler(this, e);
				//Draw(ref timer);
			}
		}
		private void Undo(Context context)
		{
			if (myHistory.Count > 0)
			{
				for (var i = myHistory.Count - 1; i >= 0; i--)
				{
					if (myHistory[i] == null) continue;
					var previous = myHistory[i];
					_top = previous.TopBottoms[0];
					_bot = previous.TopBottoms[1];
					_myExpressionLevel = previous.ExpLevel;
					_myTermLevel = previous.TermLevel;
					myHistory[i] = null;
					Draw(ref _timer, context);
					return;
				}
			}
		}

		private void SignCounterAct(KeyboardSign sign, Context context)
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
					_top = _top.Replace("%", "'='");
				else _top += "'='";

				if (_bot.IndexOf("%", StringComparison.Ordinal) != -1)
					_bot = _bot.Replace("%", "'='");
				else _bot += "'='";
			}

			#endregion Split

			else throw new NotImplementedException();
			Draw(ref _timer, context);
		}

		public void Reset(Context context)
		{
			RemoveView(Keyboard);
			Keyboard = GetKeyboard(_context);//.Reset(ref _handler, context);
			AddView(Keyboard);
			//LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, _helper.Factor(160));
			Draw(context);
		}
		public void Change(Other other, Context context)
		{
			#region Small
			if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base)
			{
				if (_myExpressionLevel == ExpressionLevel.Numerator)
				{
					_top += "^";

				}

				_myTermLevel = TermLevel.Power;
				Draw(ref _timer, context);
			}
			else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power)
			{
				if (_myExpressionLevel == ExpressionLevel.Numerator)
				{
					_top += "^";
				}

				Draw(ref _timer, context);
			}
			#endregion Small
			#region Big
			else if (other.MyKeyboardItem == KeyboardItem.Up)//&& _myExpressionLevel != ExpressionLevel.Numerator)
			{
				//if (_myExpressionLevel == ExpressionLevel.Denominator)
				//{
				_myExpressionLevel = ExpressionLevel.Numerator;
				_top += "~";
				Draw(ref _timer, context);
				//}

				// else if(_myExpressionLevel == ExpressionLevel.MiddleSigns
			}
			else if (other.MyKeyboardItem == KeyboardItem.Down)// && _myExpressionLevel != ExpressionLevel.Denominator)
			{
				//if (_myExpressionLevel == ExpressionLevel.Numerator)
				//{
				_myExpressionLevel = ExpressionLevel.Denominator;
				_top += "~";
				Draw(ref _timer, context);

				//}
			}
			#endregion Big

			if (other.MyKeyboardItem == KeyboardItem.Log)
			{
				_top += "log";
				Draw(ref _timer, context);
			}

		}
		EventHandler _handler;
		public Keyboard GetKeyboard(Context context)
		{
			_handler = InputHandler;
			return new Keyboard(ref _handler, context);
		}
		public void Draw(ref DispatcherTimer timer, Context context)
		{
			if (_top.Length > 0 || _bot.Length > 0)
			{
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), ref timer, context));
			}
			else
			{
				_top = "@";
				timer.Tick += ToggleSelected;
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), ref timer, context));
			}
		}
		public void Draw(Context context)
		{
			if (_top.Length > 0 || _bot.Length > 0)
			{
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), context));
			}
		}
		public LogarithmicEquation GetEquation(Context context)
		{
			//_top = "2log~b~(x)'='log~b~(4)'+'log~b~(x-1)";
			//_top = "2log~b~x'+'";//'='log~b~4'+'log~b~x-1";
			string left = "", right = "";
			var eq = new LogarithmicEquation(SignType.equal);

			if (_top.IndexOf("'='", StringComparison.Ordinal) != -1)
			{
				right = _top.Substring(_top.IndexOf("'='", StringComparison.Ordinal) + 3);
				left = _top.Substring(0, _top.IndexOf("'='", StringComparison.Ordinal));
			}
			else
			{
				left = _top;
				eq.IsComplete = false;
			}

			eq.Left.AddRange(GetPieces(left,context));
			if (right.Trim() != "")
				eq.Right.AddRange(GetPieces(right, context));
			return eq;
		}
		private IEnumerable<IAlgebraPiece> GetPieces(string value, Context context)
		{
			var answer = new List<IAlgebraPiece>();

			if ((value.IndexOf("'+'", StringComparison.Ordinal) != -1 || value.IndexOf("'-'", StringComparison.Ordinal) != -1))
			{
				#region More than one expression

				while (value.IndexOf("'+'", StringComparison.Ordinal) != -1 || value.IndexOf("'-'", StringComparison.Ordinal) != -1)
				{
					var expTop = "";
					var indexTop = GetClosestIndex(value, "'+'", "'-'");
					expTop = value.Substring(0, indexTop);
					answer.Add(GetMyLogTerm(expTop, context));
					answer.Add(new Sign(value.Substring(indexTop, 3).Trim('\'')));

					value = value.Substring(indexTop + 3);

					if (value.IndexOf("'+'", StringComparison.Ordinal) == -1 && value.IndexOf("'-'", StringComparison.Ordinal) == -1)
					{
						if ((value.Trim() == "" && value.Trim() == ""))
						{
							value = "@";
						}
						answer.Add(GetMyLogTerm(value, context));
					}
				}

				#endregion More than one expression
			}
			else
			{
				answer.Add(GetMyLogTerm(value,context));
			}

			return answer;
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

				for (var i = 0; i < pieces[0].Length; i++)
				{
					if (lookingFor != LookingFor.None)
					{
						#region Looking
						var valueInt = -1;

						if (int.TryParse(pieces[0][i].ToString(), out valueInt))
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
							//    lookingFor = LookingFor.MyP;
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
							//lookingFor = LookingFor.MyP;
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
					cursor = new SelectionCursor(SelectedPieceType.Power, indexPow, context);
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
				term.Sign = sign;

				return term;

				#endregion Term TT
			}
			//throw new NotImplementedException();
		}
		private string FixForShow(string value)
		{
			var appearances = value.Count(t => t == '~');

			return appearances > 2 ? value.Substring(0, value.Length - 1) : value;
		}
		private IAlgebraPiece GetMyLogTerm(string value, Context context)
		{
			value = value.Trim('%', '@');
			value = FixForShow(value);
			var expression = "";
			var myBase = "";
			var coeF = -10;


			if (value.Trim() == "" || value.Trim() == "@")
			{
				var term = new LogTerm(1, ' ', null) {IsComplete = false, ShowBase = false, ShowLog = false};
				return term;
			}
			if (int.TryParse(value.Trim('@', '%'), out coeF))
			{
				var term = new LogTerm(coeF, ' ', null) {IsComplete = false, ShowBase = false, ShowLog = false};
				return term;
			}

			if (value.LastIndexOf("~", StringComparison.Ordinal) != -1 && 
				value.Substring(0, value.LastIndexOf("~", StringComparison.Ordinal) - 1).IndexOf("~", StringComparison.Ordinal) != -1)
			{
				expression = value.Substring(value.LastIndexOf("~", StringComparison.Ordinal) + 1);
			}
			if (value.ToLower().IndexOf("log", StringComparison.Ordinal) != -1)
			{
				coeF = value.ToLower().IndexOf("log", StringComparison.Ordinal) == 0 ? 1 
					: int.Parse((value.Substring(0, value.ToLower().IndexOf("log", StringComparison.Ordinal)).Trim('%', '@')));

				var startingInd = value.ToLower().IndexOf("log", StringComparison.Ordinal) + 3;
				if (startingInd + 2 < value.Length || (value.IndexOf("~", StringComparison.Ordinal) != -1 && startingInd + 2 < value.Length))
					myBase = value.Substring(startingInd, 2).Trim('~');
				else if (value.IndexOf("~", StringComparison.Ordinal) != -1 && expression == "")
					myBase = value.Substring(startingInd).Trim('~');

				if (myBase.Trim() != "")
					return new LogTerm(coeF, myBase.Trim()[0], GetExpression(expression, "", new Range(0, expression.Length - 1), null, context));
				var term = new LogTerm(coeF, ' ', null) {IsComplete = false, ShowBase = false, ShowLog = true};
				return term;
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
				else if (piece.IndexOf("-", StringComparison.Ordinal) != -1 || piece.IndexOf("+", StringComparison.Ordinal) != -1)
				{
					if (piece.IndexOf("-", StringComparison.Ordinal) != -1 && piece.EndsWith("-"))
					{
						exp.Numerator.Add(GetMyTerm(piece.Trim('-'),context));
						var term = new Term(1)
						{
							Joke = true,
							Sign = "-",
							Selected = true,
							MySelectedPieceType = SelectedPieceType.Coefficient,
							MySelectedindex = -1
						};
						exp.Numerator.Add(term);
					}
					else if (piece.EndsWith("+"))
					{
						exp.Numerator.Add(GetMyTerm(piece.Trim('+'), context));
						var term = new Term(1)
						{
							Joke = true,
							Sign = "+",
							Selected = true,
							MySelectedPieceType = SelectedPieceType.Coefficient,
							MySelectedindex = -1
						};
						exp.Numerator.Add(term);
					}
				}
				else
				{
					if (piece.Trim() != "%")
						exp.Numerator.Add(GetMyTerm(piece, context));
				}
			}
			else
			{
				int startIndex = 0;
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
						Act(ref dimExp, ref pieceBot, i, ref startIndex,context);
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

					var endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
						(i - startIndex) + 1 : (i - startIndex);
					if (piece.Trim() != "%")
						exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1), context));

					var brace = new Brace(piece[i]);
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
					//char part = piece[(i - startIndex) + startIndex];
					if (piece.Trim() != "%")
					{
						string pie = piece.Substring(startIndex, endIndex + 1);
						exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1), context));

						if (pie.EndsWith("-") || pie.EndsWith("+"))
						{
							var term = new Term(1)
							{
								Joke = true,
								Sign = pie[pie.Length - 1].ToString(),
								Selected = true,
								MySelectedPieceType = SelectedPieceType.Coefficient,
								MySelectedindex = -1
							};
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
			return EquationType.Logarithmic;
		}

		#endregion Methods
	}
}