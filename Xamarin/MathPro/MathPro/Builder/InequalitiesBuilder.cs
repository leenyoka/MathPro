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
	public sealed class InequalitiesBuilder : LinearLayout, IProblemBuilder
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		string _savedAs = "";

		public string SavedAs
		{
			get { return _savedAs; }
			set { _savedAs = value; }
		}
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

		#endregion Properties

		#region Constructor
		DispatcherTimer _timer;
		private Keyboard _keyboard;
		public DispatcherTimer Timer
		{
			get { return _timer; }
			set { _timer = value; }
		}
		public InequalitiesBuilder(ref DispatcherTimer timer, Context context)
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
			_keyboard = GetKeyboard(context);
			AddView(_keyboard);
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
		public EventHandler ErrorHandler { get; set; }

		public void ToggleSelected(object sender, EventArgs e)
		{
		}

		private readonly Context _context;
		public void InputHandler(object sender, EventArgs e)
		{
			try
			{
				#region food
				var ignore = false;
				var startedWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

				var caller = (IKeyboardItemId)sender;

				if (_myExpressionLevel == ExpressionLevel.Numerator)
					_top = _top.Replace("@", "");
				else if (_myExpressionLevel == ExpressionLevel.Denominator)
					_bot = _bot.Replace("@", "");

				switch (caller.GetId())
				{
					case KeyBoardItemIdType.Other:
						if (((Other)caller).MyKeyboardItem == KeyboardItem.Undo)
						{
							Undo(_context);
							ignore = true;
						}
						else Change((Other)caller, _context);
						break;
					case KeyBoardItemIdType.NumberVar:
						switch (_myExpressionLevel)
						{
							case ExpressionLevel.Numerator:
								_top = _top.Replace("@", "");
								if (_top.IndexOf("%", StringComparison.Ordinal) == -1)
									_top += ((NumberVar)caller).MyVariableNumberValue + "%";
								else
									_top = _top.Replace("%", (((NumberVar)caller).MyVariableNumberValue + "%"));
								Draw(ref _timer, _context);
								break;
							case ExpressionLevel.Denominator:
								_bot = _bot.Replace("@", "");
								_bot = _bot.Trim('%');
								_bot += ((NumberVar)caller).MyVariableNumberValue + "%";
								Draw(ref _timer, _context);
								break;
						}
						break;
					default:
						SignCounterAct((KeyboardSign)caller, _context);
						break;
				}

				var endedUpWith = new History(_top, _bot, _myExpressionLevel, _myTermLevel);

				if (!ignore && (startedWith.Changed(endedUpWith)))
				{
					_myHistory.Add(startedWith);
				}

				#endregion food
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
			if (_myHistory.Count > 0)
			{
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
			else if (_myExpressionLevel == ExpressionLevel.MiddleSigns && sign.MyKey == "*")
			{
				_top += "'*'";
				_bot += "'*'";
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
			else if (CanAddAnotherSplit(_top))
			{
				if (_myTermLevel == TermLevel.Base && sign.MyKey == ">")
				{
					if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
						_top = _top.Replace("%", "'>'%");
					else _top += "'>'%";
				}
				else if (_myTermLevel == TermLevel.Base && sign.MyKey == ">=")
				{
					if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
						_top = _top.Replace("%", "'>='%");
					else _top += "'>='%";
				}
				else if (_myTermLevel == TermLevel.Base && sign.MyKey == "<=")
				{
					if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
						_top = _top.Replace("%", "'<='%");
					else _top += "'<='%";
				}
				else if (_myTermLevel == TermLevel.Base && sign.MyKey == "<")
				{
					if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
						_top = _top.Replace("%", "'<'%");
					else _top += "'<'%";
				}
			}

			#endregion Split

			else throw new NotImplementedException();
			Draw(ref _timer, context);
		}
		private bool CanAddAnotherSplit(string _top)
		{
			var pieces = new[] { ">=", "<=", ">", "<" };
			var count = 0;

			for (var i = 0; i < _top.Length - 1; i++)
			{
				var current = _top.Substring(i, 2);

				for (var k = 0; k < pieces.Length; k++)
				{
					if (current.IndexOf(pieces[k], StringComparison.Ordinal) != -1 && current == ">=" || current == "<=")
					{
						count++;
						i += 1;
						break;
					}
					if (current[0].ToString(CultureInfo.InvariantCulture) == pieces[k])
						count++;
				}
			}

			count += pieces.Count(t => _top[_top.Length - 1].ToString
				(CultureInfo.InvariantCulture).IndexOf(t, StringComparison.Ordinal) != -1);

			return count <= 1;
		}
		public void Change(Other other, Context context)
		{
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
				Draw(ref _timer,context);
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

				Draw(ref _timer,context);
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
		public Equation GetEquation(Context context)
		{
			//_top = "4-3x<5x>=9";
			Equation equation;
			var splitPoints = GetSplitPoints(_top);

			if (splitPoints.Count > 0)
			{
				equation = new Equation((new Sign(splitPoints[0][1])).SignType);
				equation.Left.Add(GetExpression(_top, "", new Range(0, (int.Parse(splitPoints[0][0])) - 1), null, context));
				var leftOver = _top.Substring((int.Parse(splitPoints[0][0])) + (splitPoints[0][1].Length));

				if (splitPoints.Count > 1)
				{
					splitPoints = GetSplitPoints(leftOver);

					equation.Right.Add(GetExpression(leftOver, "", new Range(0, (int.Parse(splitPoints[0][0])) - 1), null,context));
					equation.Right.Add(new Sign(splitPoints[0][1]));
					var leftOverAgain = leftOver.Substring((int.Parse(splitPoints[0][0])) + (splitPoints[0][1].Length));
					equation.Right.Add(GetExpression(leftOverAgain, "", new Range(0, leftOverAgain.Length - 1), null, context));
				}
				else
					equation.Right.Add(GetExpression(_top, "", new Range(int.Parse(splitPoints[0][0])
						+ splitPoints[0][1].Length, _top.Length - 1), null,context));
			}

			else
			{
				equation = new Equation(SignType.greater) {IsComplete = false};
				equation.Left.Add(GetExpression(_top, "", new Range(0, _top.Length - 1), null, context));
			}
			return equation;
		}
		public List<string[]> GetSplitPoints(string top)
		{
			var answer = new List<string[]>();
			string[] pieces = { ">=", "<=", "<", ">" };

			for (int i = 0; i < top.Length - 1; i++)
			{
				var current = top.Substring(i, 2);

				for (var k = 0; k < pieces.Length; k++)
				{
					if (current.IndexOf(pieces[k], StringComparison.Ordinal) != -1 && current == ">=" || current == "<=")
					{
						answer.Add(new[] { i.ToString(CultureInfo.InvariantCulture), pieces[k] });
						i += 1;
						break;
					}
					if (current[0].ToString(CultureInfo.InvariantCulture) == pieces[k])
					{
						answer.Add(new[] { i.ToString(CultureInfo.InvariantCulture), pieces[k] });
					}
				}
			}

			answer.AddRange(from t in pieces where top[top.Length - 1].ToString(CultureInfo.InvariantCulture)
								.IndexOf(t, StringComparison.Ordinal) != -1 select new[] {(top.Length - 1).ToString(CultureInfo.InvariantCulture), t});

			return answer;
		}

		private Term GetMyTerm(string value, Context context)
		{
			value = value.Trim('\'');
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
							//if (lookingFor == LookingFor.MyB)
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
							if (pieces[0][i + 1] == '%')
							{
								indexBase = 0;
								i++;
							}
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

			var piece = (sourceTop.Substring(rangeTop.Start, 1 + (rangeTop.End - rangeTop.Start))).Trim('\'');

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
				for (var i = 0; i < piece.Length; i++)
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
							dimExp.Numerator.Add(GetMyTerm(pieceBot,context));
					}
				}
				else
				{
					int startIndex = 0;
					for (int i = 0; i < pieceBot.Length; i++)
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

					int endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
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

				int endIndex = (i - startIndex);
				if (piece.Trim() != "%")
					exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1),context));
				startIndex = i + 1;
			}
		}

		public EquationType GetEquationType()
		{
			return EquationType.Algebraic;
		}

		#endregion Methods
	}
}