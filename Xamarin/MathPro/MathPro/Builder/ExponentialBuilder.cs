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
	public sealed class ExponentialBuilder : LinearLayout, IProblemBuilder
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		public EventHandler ErrorHandler { get; set; }

		TermLevel _myTermLevel = TermLevel.Base;
		TermLevel _myExpressionLevel = TermLevel.Base;
		string _top = "";

		public string Top
		{
			get { return _top; }
			set { _top = value; }
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
		DispatcherTimer timer;
		private Keyboard _keyboard;
		public ExponentialBuilder(ref DispatcherTimer timer, Context context)
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
			this.timer = timer;
			_context = context;
			AddView(new BuilderDisplayArea(context,_myDisplayArea));
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
				var startedWith = new History(_top, "", _myExpressionLevel, _myTermLevel);

				var caller = (IKeyboardItemId)sender;

				if (_myExpressionLevel == TermLevel.Base)
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

					_top = _top.Replace("@", "");
					if (_top.IndexOf("%", StringComparison.Ordinal) == -1)
						_top += ((NumberVar)caller).MyVariableNumberValue + "%";
					else
						_top = _top.Replace("%", (((NumberVar)caller).MyVariableNumberValue + "%"));
					Draw(ref timer,_context);
					#endregion Number Or Variable
				}
				else
				{
					SignCounterAct((KeyboardSign)caller,_context);
				}

				var endedUpWith = new History(_top, "", _myExpressionLevel, _myTermLevel);

				if (!ignore && (startedWith.Changed2(endedUpWith)))
				{
					myHistory.Add(startedWith);
				}
				#endregion food
			}
			catch
			{
				Undo(_context);
				//Undo();
				//_errorHandler(this, e);
				//Draw(ref timer);
			}
		}
		private void Undo(Context context)
		{
			if (myHistory.Count > 0)
			{
				for (var i = myHistory.Count - 1; i >= 0; i--)
				{
					if (myHistory[i] != null)
					{
						var previous = myHistory[i];
						_top = previous.TopBottoms[0];
						//_bot = previous.TopBottoms[1];
						_myExpressionLevel = previous.TermLevel2;
						_myTermLevel = previous.TermLevel;
						myHistory[i] = null;
						Draw(ref timer, context);
						return;
					}
				}
			}
		}

		private void SignCounterAct(KeyboardSign sign, Context context)
		{

			#region plus minus Mult

			if (_myExpressionLevel == TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == "+")
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "+%");
				else _top += "+";
			}
			else if (_myExpressionLevel == TermLevel.Base && sign.MyKey == "+")
			{
				//_top += "'+'";
			}

			else if (_myExpressionLevel == TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == "-")
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "-%");
				else _top += "-";
			}
			else if (_myExpressionLevel == TermLevel.Base && sign.MyKey == "-")
			{
				//_top += "'-'";
			}
			else if (_myExpressionLevel == TermLevel.Base && sign.MyKey == "*")
			{
				//_top += "'*'";
			}
			#endregion plus minus

			#region braces

			else if (_myExpressionLevel == TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == "(")
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "(%");
				else _top += "(%";
			}

			else if (_myExpressionLevel == TermLevel.Power && _myTermLevel == TermLevel.Base && sign.MyKey == ")")
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", ")%");
				else _top += ")%";
			}


			#endregion braces

			#region Split

			else if (_myExpressionLevel == TermLevel.Base &&
				_myTermLevel == TermLevel.Base && sign.MyKey == "=" && _top.IndexOf('=') == -1)
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) != -1)
					_top = _top.Replace("%", "'='%");
				else _top += "'='%";
			}

			#endregion Split

			else throw new NotImplementedException();
			Draw(ref timer, context);
		}
		public void Change(Other other, Context context)
		{
			#region Small
			if (other.MyKeyboardItem == KeyboardItem.UpSmall && _myTermLevel == TermLevel.Base
				&& _myExpressionLevel == TermLevel.Power)
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) == -1)
					_top += "^";
				else _top = _top.Replace("%", "^%");

				_myTermLevel = TermLevel.Power;
				Draw(ref timer, context);
			}
			else if (other.MyKeyboardItem == KeyboardItem.DownSmall && _myTermLevel == TermLevel.Power
				&& _myExpressionLevel == TermLevel.Power)
			{
				if (_top.IndexOf("%", StringComparison.Ordinal) == -1)
					_top += "^";
				else _top = _top.Replace("%", "^%");
				_myTermLevel = TermLevel.Base;

				Draw(ref timer, context);
			}
			#endregion Small

			#region Big
			else if (other.MyKeyboardItem == KeyboardItem.Up && _myExpressionLevel != TermLevel.Power)
			{
				_myExpressionLevel = TermLevel.Power;
				_top = _top.Replace("%", "");
				_top = _top.Replace("@", "");
				_top += "#";
				Draw(ref timer, context);
			}
			else if (other.MyKeyboardItem == KeyboardItem.Down && _myExpressionLevel != TermLevel.Base)
			{

				_myExpressionLevel = TermLevel.Base;
				_top = _top.Replace("%", "");
				_top = _top.Replace("@", "");
				_top += "#";
				Draw(ref timer, context);
			}
			#endregion Big

		}
		EventHandler _handler;
		public Keyboard GetKeyboard(Context context)
		{
			_handler = InputHandler;
			return new Keyboard(ref _handler, context);
		}
		public void Draw(ref DispatcherTimer mtimer, Context context)
		{
			//_top = "9#4x-1#'='27#5-x#";
			if (_top.Length > 0)
			{
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), ref mtimer, context));//, ref timer));
			}
			else
			{
				_top = "@";
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context), ref mtimer,context));//, ref timer));
			}


			//maybe do more???
		}
		public void Draw(Context context)
		{
			//_top = "9#4x-1#'='27#5-x#";
			if (_top.Length > 0)
			{
				_myDisplayArea.RemoveAllViews();
				_myDisplayArea.AddView(new Step(GetEquation(context),context));//, ref timer));
			}

			//maybe do more???
		}
		public ExponentialEquation GetEquation(Context context)
		{
			//if (_top.Trim() == "")
			//_top = "@";
			string rightTop = "";

			var _top1 = "";
			var eq = new ExponentialEquation(SignType.equal);
			if (_top.IndexOf("'='", StringComparison.Ordinal) == -1)
			{
				eq.IsComplete = false;
				_top1 = _top;
			}
			else
			{
				rightTop = _top.Substring(_top.IndexOf("'='", StringComparison.Ordinal) + 3);
				if (rightTop[0] == '%')
				{
					//eq.SplitSelected = true;
				}
				_top1 = _top.Substring(0, _top.IndexOf("'='", StringComparison.Ordinal));
				//eq.IsComplete = true;
			}

			if ((_top1.IndexOf("'+'", StringComparison.Ordinal) != -1 || _top1.IndexOf("'-'", StringComparison.Ordinal) != -1))
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
				if (_top1.Trim() != "")
					eq.Left.Add(GetMyTermExpo(_top1, context));
			}

			if (rightTop.Trim() != "")
			{
				if ((rightTop.IndexOf("'+'", StringComparison.Ordinal) != -1 || rightTop.IndexOf("'-'", StringComparison.Ordinal) != -1))
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
					if (rightTop.Trim() != "")
						eq.Right.Add(GetMyTermExpo(rightTop,context));
				}
			}

			return eq;
		}
		private ExponentialTerm GetMyTermExpo(string value, Context context)
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
			if (value == "-%" || value == "+%")
			{
				var term = new ExponentialTerm(1)
				{
					Selected = true,
					MySelectedPieceType = SelectedPieceType.BaseExpo,
					MySelectedindex = -1,
					Joke = true
				};
				return term;
			}
			var MyBase = -1;
			var MyPower = "";

			if (value.IndexOf("#", StringComparison.Ordinal) == -1)
			{
				MyBase = int.Parse(value.Trim('%'));
			}
			else
			{
				MyBase = int.Parse(value.Substring(0, value.IndexOf("#", StringComparison.Ordinal)));
				MyPower = value.Substring(value.IndexOf("#", StringComparison.Ordinal)).Trim('#');
			}

			if (MyPower.Trim() == "")
			{
				var term = new ExponentialTerm(MyBase);
				if (value.IndexOf("%", StringComparison.Ordinal) != -1)
				{
					term.Selected = true;
					term.MySelectedindex = -1;
					term.MySelectedPieceType = SelectedPieceType.BaseExpo;
				}
				return term;
			}
			var exp = GetExpression(MyPower, "", new Range(0, MyPower.Length - 1), new Range(0, 0), context);
			return new ExponentialTerm(MyBase, exp);
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

						if (int.TryParse(pieces[0][i].ToString(CultureInfo.InvariantCulture), out valueInt))
						{
							if (lookingFor == LookingFor.MyC)
								coeF += pieces[0][i].ToString(CultureInfo.InvariantCulture);
							else if (lookingFor == LookingFor.MyP)
								power += pieces[0][i].ToString(CultureInfo.InvariantCulture);
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
								if (lookingFor == LookingFor.MyC)
									indexCoe = coeF.Length - 1;
								else if (lookingFor == LookingFor.MyP)
									indexPow = power.Length - 1;
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
							if (lookingFor != LookingFor.MyP)
								lookingFor = LookingFor.MyP;
							else
								lookingFor = LookingFor.None;
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
				for (var i = 0; i < piece.Length; i++)
				{
					Act(ref exp, ref piece, i, ref startIndex,context);
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

					var endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
						(i - startIndex) + 1 : (i - startIndex);
					if (piece.Trim() != "%")
						exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1),context));

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

					var endIndex = ((i - startIndex) + 1 < piece.Length && (piece[i - startIndex + 1] == '%')) ?
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
					exp.Numerator.Add(GetMyTerm(piece.Substring(startIndex, endIndex + 1), context));
				startIndex = i + 1;
			}
		}
		public EquationType GetEquationType()
		{
			return EquationType.Exponential;
		}

		#endregion Methods
	}
}