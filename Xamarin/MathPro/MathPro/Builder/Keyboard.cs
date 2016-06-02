using System;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathBase;
using MathPro.Display;

namespace MathPro.Builder
{
	public sealed class Keyboard : ScrollView
	{
		private readonly KeyboardWrapper _wraper;
		public Keyboard(ref EventHandler handler, Context context)
			: base(context)
		{
			_wraper = new KeyboardWrapper(ref handler, context);
			AddView(_wraper);
		}
		public void Reset(ref EventHandler handler, Context context)
		{
			_wraper.Reset(ref handler, context);
		}
	}
	public sealed class KeyboardWrapper : HorizontalScrollView
	{
		private readonly KeyboardCore _core;
		public KeyboardWrapper(ref EventHandler handler, Context context)
			:base(context)
		{
			_core = new KeyboardCore(ref handler, context);
			AddView(_core);
		}

		public void Reset(ref EventHandler handler, Context context)
		{
			_core.Reset(ref handler,context);
		}
	}
	public sealed class KeyboardCore : LinearLayout
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		#endregion Properties

		#region Constructor

		public KeyboardCore(ref EventHandler handler, Context context)
			:base(context)
		{
			Reset(ref handler, context);
		}

		public void Reset(ref EventHandler handler, Context context)
		{
			RemoveAllViews();
			LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(730), _helper.Factor(290));
			Layout(43, 45, 0, 0);
			AddView(GetInner(ref handler, context));
		}

		#endregion Constructor

		#region Methods

		private LinearLayout GetInner(ref EventHandler handler, Context context)
		{
			var box = new LinearLayout(context) {Orientation = Orientation.Vertical};
			box.AddView(GetNumbersVa(ref handler,context));
			box.AddView(GetSigns(ref handler, context));
			box.AddView(GetOther(ref handler, context));
			box.AddView(GetNumbersVa2(ref handler, context));
			box.AddView(GetOtherTwo(ref handler,context));
			return box;
		}

		private LinearLayout GetSigns(ref EventHandler handler, Context context)
		{
			var panel = Hoster(context);

			var values = new[] { "=", "+", "-", "*", "<", "<=", ">", ">=", "(", ")" };

			foreach (var value in values)
			{
				var curr = new KeyboardSign(value,context);
				curr.Click += handler;
				panel.AddView(curr);
			}
			return panel;
		}
		private LinearLayout GetNumbersVa(ref EventHandler handler, Context context)
		{
			var arrayV = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			var panel = Hoster(context);

			foreach (char myChar in arrayV)
			{
				var curr = new NumberVar(myChar,context);
				curr.Click += handler;
				panel.AddView(curr);
			}
			return panel;

		}
		private LinearLayout GetNumbersVa2(ref EventHandler handler, Context context)
		{
			var arrayV = new[] { 'a', 'b', 't', 'u', 'v', 'w', 'x' };
			var panel = Hoster(context);

			foreach (char myChar in arrayV)
			{
				var curr = new NumberVar(myChar, context);
				curr.Click += handler;
				panel.AddView(curr);
			}
			return panel;

		}
		private LinearLayout Hoster(Context context)
		{
			var panel = new LinearLayout(context) { LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) };

			//panel.Width = 730;
			return panel;
		}
		private LinearLayout GetOther(ref EventHandler handler, Context context)
		{
			var panel = Hoster(context);

			var items = new[] { KeyboardItem.Up, KeyboardItem.Down, KeyboardItem.DownSmall, KeyboardItem.UpSmall, KeyboardItem.Undo, KeyboardItem.F };

			foreach (KeyboardItem item in items)
			{
				var curr = new Other(item,context);
				curr.Click +=  handler;
				panel.AddView(curr);
			}
			return panel;
		}
		private LinearLayout GetOtherTwo(ref EventHandler handler, Context context)
		{
			var panel = Hoster(context);

			// Log, e, Cos, Sin, Tan, Cot, Sec, Scs
			var items = new[] {KeyboardItem.Log, KeyboardItem.E, KeyboardItem.Sin, KeyboardItem.Cos, KeyboardItem.Tan,
                                                       KeyboardItem.Cot, KeyboardItem.Sec, KeyboardItem.Scs};

			foreach (var item in items)
			{
				var curr = new Other(item, context);
				curr.Click += handler;
				panel.AddView(curr);
			}
			return panel;
		}

		#endregion Methods

	}
	public sealed class NumberVar : RelativeLayout, IKeyboardItemId
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		public char MyVariableNumberValue { get; set; }

		#endregion Properties

		#region Constructor

		public NumberVar(char var, Context context)
			:base(context)
		{
			MyVariableNumberValue = var;

			LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(33),_helper.Factor(20));
			SetBackgroundResource(new TermUtility().GetBackgroundResourceNumOrChar("boarder"));
			AddView(GetInner(var.ToString(CultureInfo.InvariantCulture),context));
		}

		#endregion Constructor

		#region Methods

		private RelativeLayout GetInner(string background, Context context)
		{
			var grid = new RelativeLayout(context){LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(18), _helper.Factor(16))};
			
			grid.SetBackgroundResource(new TermUtility().GetBackgroundResourceNumOrChar(background));
			return grid;
		}


		#endregion Methods

		public KeyBoardItemIdType GetId()
		{
			return KeyBoardItemIdType.NumberVar;
		}
	}
	public sealed class KeyboardSign : RelativeLayout, IKeyboardItemId
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		string _myKey;

		public string MyKey
		{
			get { return _myKey; }
			set { _myKey = value; }
		}

		#endregion Properties

		#region Constructor

		public KeyboardSign(string key, Context context)
			:base(context)
		{
			_myKey = key;
			LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(33), _helper.Factor(20));
			
			SetBackgroundResource(new TermUtility().GetBackgroundResourceNumOrChar("boarder"));
			AddView(GetInner(context));
		}

		#endregion Constructor

		#region Methods

		private RelativeLayout GetInner(Context context)
		{
			var grid = new RelativeLayout(context) {LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(18), _helper.Factor(16))};
			

			if (_myKey == "-")
				grid.LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(28),_helper.Factor(8));

			if (_myKey != ")" && _myKey != "(")
				grid.SetBackgroundResource(new TermUtility().GetBackgroundResourceNumOrChar(FixSign(new Sign(_myKey))));
			else
			{
				grid.SetBackgroundResource(_myKey == "("
					? new TermUtility().GetBackgroundResourceNumOrChar("openBrace")
					: new TermUtility().GetBackgroundResourceNumOrChar("closeBrace"));
			}
			return grid;
		}

		private string FixSign(Sign signType)
		{
			if (signType.SignType == SignType.Add)
				return "plus";
			if (signType.SignType == SignType.Subtract)
				return "minus";
			if (signType.SignType == SignType.Divide)
				return "divide";
			if (signType.SignType == SignType.equal)
				return "equal";
			if (signType.SignType == SignType.greater)
				return "greaterthan";
			if (signType.SignType == SignType.greaterEqual)
				return "greaterOrEqual";
			if (signType.SignType == SignType.Multiply)
				return "multiply";
			if (signType.SignType == SignType.Or)
				return "or";
			if (signType.SignType == SignType.smaller)
				return "smallerthan";
			if (signType.SignType == SignType.smallerEqual)
				return "smallerOrEqual";
			throw new NotImplementedException();
		}


		#endregion Methods

		public KeyBoardItemIdType GetId()
		{
			return KeyBoardItemIdType.KeyboardSign;
		}
	}
	public sealed class Other : RelativeLayout, IKeyboardItemId
	{
		#region Properties
		private readonly Helper _helper = new Helper();
		readonly TermUtility _utility = new TermUtility();
		KeyboardItem _myKeyboardItem;

		public KeyboardItem MyKeyboardItem
		{
			get { return _myKeyboardItem; }
			set { _myKeyboardItem = value; }
		}
		#endregion Properties

		#region Constructor

		public Other(KeyboardItem item, Context context)
			:base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(33),_helper.Factor(20));

			_myKeyboardItem = item;
			SetBackgroundResource(_utility.GetBackgroundResourceNumOrChar("boarder"));

			if (InLogTrigList(item))
			{
				AddView(GetInner(new TermUtility().GetBackgroundResourceNumOrChar(HandForLopTrigItem(item)),context));
			}
			else if (_myKeyboardItem == KeyboardItem.DownSmall ||
				_myKeyboardItem == KeyboardItem.UpSmall)
				AddView(GetInner(context));
			else if (_myKeyboardItem == KeyboardItem.Undo)
			{
				AddView(GetInner(new TermUtility().GetBackgroundResourceNumOrChar(item.ToString().ToLower()), context));
			}
			else if (_myKeyboardItem == KeyboardItem.F)
			{
				AddView(GetInner(new TermUtility().GetBackgroundResourceNumOrChar(item.ToString().ToLower()),context));
			}
			else
				AddView(GetInner(new TermUtility().GetBackgroundResourceNumOrChar("arrow" + item.ToString().ToLower()),context));
		}

		#endregion Constructor

		#region Methods

		private RelativeLayout GetInner(Context context)
		{
			var grid = new RelativeLayout(context) {LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(18), _helper.Factor(18))};

			grid.SetBackgroundResource(_myKeyboardItem == KeyboardItem.UpSmall
				? _utility.GetBackgroundResourceNumOrChar("arrowUp")
				: _utility.GetBackgroundResourceNumOrChar("arrowDown"));
			return grid;
		}
		private RelativeLayout GetInner(int brush, Context context)
		{
			var grid = new RelativeLayout(context) {LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(25), _helper.Factor(25))};

			grid.SetBackgroundResource( brush);
			return grid;
		}

		#endregion Methods

		public bool InLogTrigList(KeyboardItem compare)
		{
			KeyboardItem[] items =
			{KeyboardItem.Log, KeyboardItem.E, KeyboardItem.Sin, KeyboardItem.Cos, KeyboardItem.Tan,
				KeyboardItem.Cot, KeyboardItem.Sec, KeyboardItem.Scs};

			return items.Any(item => item == compare);
		}
		public string HandForLopTrigItem(KeyboardItem item)
		{
			if (item == KeyboardItem.Log)
				return "log";
			if (item == KeyboardItem.E)
				return "e";
			if (item == KeyboardItem.Sin)
				return "sin";
			if (item == KeyboardItem.Cos)
				return "cos";
			if (item == KeyboardItem.Tan)
				return "tan";
			if (item == KeyboardItem.Cot)
				return "cot";
			if (item == KeyboardItem.Sec)
				return "sec";
			if (item == KeyboardItem.Scs)
				return "cosec";
			throw new NotImplementedException();
		}

		public KeyBoardItemIdType GetId()
		{
			return KeyBoardItemIdType.Other;
		}
	}
	public enum KeyboardItem
	{
		Left, Right, Up, Down, UpSmall, DownSmall, Undo, Log, E, Cos, Sin, Tan, Cot, Sec, Scs, F
	}
	public interface IKeyboardItemId
	{
		KeyBoardItemIdType GetId();
	}
	public enum KeyBoardItemIdType
	{
		Other, KeyboardSign, NumberVar
	}
}