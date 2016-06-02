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
    public sealed class ViewTerm: RelativeLayout
    {

	    public ViewTermLinearInner Inner { get; set; }
		private readonly Helper _helper = new Helper();
		#region Constructor

		public ViewTerm(Term source, IEnumerable<ViewPropeties> properties, Context context)
			: base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			Inner = new ViewTermLinearInner(source, properties, context);
			
			Prepare(source);
			AddView(Inner);
		}
		public ViewTerm(Term source, IEnumerable<ViewPropeties> properties, ref DispatcherTimer timer, Context context)
			: base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			Inner = new ViewTermLinearInner(source, properties, ref timer, context);
			
			Prepare(source);
			AddView(Inner);
		}

		#endregion Constructor

	    private void Prepare(Term _myTerm)
	    {
		    if (_myTerm.Root <= 1) return;
		    SetBackgroundResource(Resource.Drawable.root);
		    LayoutParameters = new RelativeLayout.LayoutParams(_helper.Factor(66), _helper.Factor(30));
		    SetPadding(15,10,0,0);
	    }

    }

	public class ViewTermLinearInner : LinearLayout
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		List<SelectionCursor> _mySelect = new List<SelectionCursor>();

		private Context _context;

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

		public ViewTermLinearInner(Term source, IEnumerable<ViewPropeties> properties, Context context)
			: base(context)
		{
			_myTerm = source;
			_context = context;

			if (properties != null)
				_myProperties = new List<ViewPropeties>(properties);
			else _myProperties = new List<ViewPropeties>();
			Prepare();
		}
		public ViewTermLinearInner(Term source, IEnumerable<ViewPropeties> properties, ref DispatcherTimer timer, Context context)
			: base(context)
		{
			_myTerm = source;
			_context = context;
			_myProperties = properties != null ? new List<ViewPropeties>(properties) : new List<ViewPropeties>();
			Prepare();
			if (source.Selected)
				Select(source.MySelectedPieceType, source.MySelectedindex, ref timer);
		}

		#endregion Constructor
		#region Methods

		#region Player Methods

		public void HideAll()
		{
			for (var i = 0; i < ChildCount; i++)
			{
				GetChildAt(i).Visibility = ViewStates.Gone;
			}
		}
		public bool ShowNext()
		{
			for (int i = 0; i < ChildCount; i++)
			{
				if (GetChildAt(i).Visibility == ViewStates.Gone
					&& !DontTouch(GetChildAt(i)))
				{
					GetChildAt(i).Visibility = ViewStates.Visible;
					return true;
				}
			}
			return false;
		}
		private bool DontTouch(View relativeLayout)
		{
			try
			{
				var cursor = (SelectionCursor)relativeLayout;
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
				/*
				//space waster
				var relativeLayout = new RelativeLayout(_context);

				if (!NoDenominators())
					relativeLayout.LayoutParameters = new LayoutParams(20, 29);
				else
				{
					var params1 = new LayoutParams(20, 16) { TopMargin = -10 };
					relativeLayout.LayoutParameters = params1;

				}
				AddView(relativeLayout);
				//SetBackgroundResource(Resource.Drawable.root);
				//this.Background = GetBackGround("root");
				 */
			}
			_mySelect.Add(new SelectionCursor(SelectedPieceType.Sign, -1, _context));
			AddView(_mySelect[_mySelect.Count - 1]);

			if ((!IsFirst() || _myTerm.Sign == "-" || _myTerm.TwoSigns || _myTerm.Joke) && !_myTerm.ExpressJoke)
			{
				AddView(GetSign());
				_mySelect.Add(new SelectionCursor(SelectedPieceType.Sign, 0, _context));
				AddView(_mySelect[_mySelect.Count - 1]);
			}
			_mySelect.Add(new SelectionCursor(SelectedPieceType.Coefficient, -1, _context));
			AddView(_mySelect[_mySelect.Count - 1]);

			if (MyTerm.Joke || _myTerm.ExpressJoke)
				_mySelect[_mySelect.Count - 1].Active = true;

			if (!MyTerm.Joke && !_myTerm.ExpressJoke)
			{
				#region Not joke
				if (_myTerm.Constant ||
					_myTerm.CoEfficient != 1)
				{
					int count = 0;
					foreach (RelativeLayout myRelativeLayout in GetCoefficient())
					{
						AddView(myRelativeLayout);
						_mySelect.Add(new SelectionCursor(SelectedPieceType.Coefficient, count, _context));
						AddView(_mySelect[_mySelect.Count - 1]);
						count++;
						//break;
					}
				}
				_mySelect.Add(new SelectionCursor(SelectedPieceType.Base, -1, _context));
				AddView(_mySelect[_mySelect.Count - 1]);

				if (!_myTerm.Constant)
				{
					AddView(GetTermBase());
					_mySelect.Add(new SelectionCursor(SelectedPieceType.Base, 0, _context));
					AddView(_mySelect[_mySelect.Count - 1]);
				}
				_mySelect.Add(new SelectionCursor(SelectedPieceType.Power, -1, _context));
				AddView(_mySelect[_mySelect.Count - 1]);

				if (_myTerm.Power > 1 || _myTerm.PowerSign == "-")
				{
					List<RelativeLayout> items = GetPower(_myTerm.PowerSign, _myTerm.Power);

					for (int i = 0; i < items.Count; i++)
					{
						AddView(items[i]);
						_mySelect.Add(new SelectionCursor(SelectedPieceType.Power, i, _context));
						AddView(_mySelect[_mySelect.Count - 1]);
					}
				}

				if (_myTerm.MultipledBy != null && _myTerm.MultipledBy.Count > 0)
				{

					List<RelativeLayout> items = GetMultiples();

					for (int i = 0; i < items.Count; i++)
					{
						AddView(items[i]);
					}
				}

				_mySelect.Add(new SelectionCursor(SelectedPieceType.OutSide, -2, _context));
				AddView(_mySelect[_mySelect.Count - 1]);

				#endregion Not Joke
			}

			//
		}

		private List<RelativeLayout> GetMultiples()
		{
			var answer = new List<RelativeLayout>();

			for (var i = 0; i < _myTerm.MultipledBy.Count; i++)
			{
				answer.Add(GetTermMultipleSign());
				_mySelect.Add(new SelectionCursor(SelectedPieceType.Sign, 0, _context));
				AddView(_mySelect[_mySelect.Count - 1]);
				answer.Add(GetTermBase(_myTerm.MultipledBy[i].TermBase.ToString(CultureInfo.InvariantCulture)));
				_mySelect.Add(new SelectionCursor(SelectedPieceType.Base, i, _context));
				AddView(_mySelect[_mySelect.Count - 1]);

				if (_myTerm.MultipledBy[i].Power <= 1 && _myTerm.MultipledBy[i].PowerSign != "-") continue;
				answer.AddRange(GetPower(_myTerm.MultipledBy[i].PowerSign,
					_myTerm.MultipledBy[i].Power));
				_mySelect.Add(new SelectionCursor(SelectedPieceType.Power, i, _context));
				AddView(_mySelect[_mySelect.Count - 1]);
			}

			return answer;
		}
		private List<RelativeLayout> GetPower(string powerSign, int powerV)
		{
			List<RelativeLayout> answer = new List<RelativeLayout>();

			if (powerSign == "-")
			{
				RelativeLayout pSign = new RelativeLayout(_context);
				RelativeLayout pSignInner = new RelativeLayout(_context);

				//if (NoDenominators() && NoDevisors())
				//{

				pSign.LayoutParameters = new LayoutParams(_helper.Factor(7), _helper.Factor(16));
				var parems = new LayoutParams(_helper.Factor(9), _helper.Factor(2));
				parems.TopMargin = -12;
				pSignInner.LayoutParameters = parems;
				pSignInner.SetBackgroundResource(Resource.Drawable.minus);
				//}
				//else throw new NotImplementedException();

				pSign.AddView(pSignInner);
				answer.Add(pSign);
			}
			foreach (var myChar in powerV.ToString(CultureInfo.InvariantCulture))
			{
				var power = new RelativeLayout(_context);
				var powerInner = new RelativeLayout(_context);
				power.LayoutParameters = new LayoutParams(_helper.Factor(15), _helper.Factor(16));
				var parems = new LayoutParams(_helper.Factor(9), _helper.Factor(10)) { TopMargin = -12 };
				powerInner.LayoutParameters = parems;

				powerInner.SetBackgroundResource(GetBackgroundResourceNumOrChar(myChar));
				power.AddView(powerInner);
				answer.Add(power);
			}

			return answer;
		}

		private int GetBackgroundResourceNumOrChar(char value)
		{
			return utility.GetBackgroundResourceNumOrChar(value);
		}
		private RelativeLayout GetTermBase()
		{
			return GetTermBase(_myTerm.TermBase.ToString(CultureInfo.InvariantCulture));
		}
		private RelativeLayout GetTermBase(string baseValue)
		{
			var relativeLayout = new RelativeLayout(_context);

			//if ((NoDevisors() && NoDenominators() ||
			//    (NoDevisors() && !NoDenominators())))
			//{
			relativeLayout.LayoutParameters = new LayoutParams(_helper.Factor(20), _helper.Factor(20));
			// }
			//else throw new NotImplementedException();
			relativeLayout.SetBackgroundResource(GetBackgroundResourceNumOrChar(baseValue[0]));
			return relativeLayout;
		}
		private RelativeLayout GetTermMultipleSign()
		{
			var relativeLayout = new RelativeLayout(_context);
			var inner = new RelativeLayout(_context);
			if (NoDevisors() && NoDenominators())
			{
				relativeLayout.LayoutParameters = new LayoutParams(_helper.Factor(20), _helper.Factor(20));

				inner.LayoutParameters = new LayoutParams(_helper.Factor(7), _helper.Factor(7));
				//inner.Margin = new Thickness(0, 0, 0, 0);
			}
			else throw new NotImplementedException();

			inner.SetBackgroundResource(Resource.Drawable.termMultiply);
			relativeLayout.AddView(inner);
			return relativeLayout;
		}
		private IEnumerable<RelativeLayout> GetCoefficient()
		{
			return _myTerm.CoEfficient.ToString(CultureInfo.InvariantCulture).Select(myChar => GetTermBase(myChar.ToString(CultureInfo.InvariantCulture))).ToList();
		}

		private RelativeLayout GetSign()
		{
			var relativeLayout = new RelativeLayout(_context);
			var inner = new RelativeLayout(_context);
			var params1 = new LayoutParams(_helper.Factor(20), _helper.Factor(20)) { TopMargin = _myTerm.Sign == "-" ? _helper.Factor(9) : _helper.Factor(5) };
			relativeLayout.LayoutParameters = params1;

			var params2 = new LayoutParams(_helper.Factor(9), _helper.Factor(9));
			inner.LayoutParameters = params2;

			if (_myTerm.Sign == "-")
				inner.LayoutParameters = new LayoutParams(_helper.Factor(9), _helper.Factor(3));


			inner.SetBackgroundResource(!MyTerm.TwoSigns ? FixSign(new Sign(_myTerm.Sign)) : Resource.Drawable.plusMinus);
			//}
			//else throw new NotImplementedException();
			relativeLayout.AddView(inner);
			return relativeLayout;
		}

		private int FixSign(Sign signType)
		{
			return utility.FixSign(signType);
		}
		TermUtility utility = new TermUtility();
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
					_mySelect[i].Visibility = ViewStates.Gone;
					//timer.Tick += new EventHandler(Toggle);
					answer = true;
					break;
				}

			}
			return answer;
		}
		private void Toggle(object sender, EventArgs e)
		{
			foreach (SelectionCursor t in _mySelect)
			{
				if (t.Active)
				{
					t.GetChildAt(0).Visibility = t.GetChildAt(0).Visibility == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
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