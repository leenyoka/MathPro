using System.Globalization;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace MathPro.Geometry
{
	public sealed class PointView : ScrollView
	{
		private readonly PointViewCore _core;
		public PointView(int number, Context context)
			: base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, 260);
			_core = new PointViewCore(number, context);
			AddView(_core);
		}

		public PointView(string name, Context context)
			: base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, 260);
			_core = new PointViewCore(name, context);
			AddView(_core);
		}

		public Point GetPoint()
		{
			return _core.GetPoint();
		}
	}

	public sealed class PointViewCore : LinearLayout
	{
		#region Properties
		//private readonly Helper _helper = new Helper();
		private readonly Context _context;
		private readonly int[] _values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -2, -3, -4, -5, -6, -7, -8, -9 };
		public Spinner XValue { get; set; }
		public Spinner YValue { get; set; }
		#endregion Properties

		#region Constructor

		public PointViewCore(int number, Context context)
			:base(context)
		{
			Orientation = Orientation.Vertical;
			_context = context;
			LayoutParameters = new ViewGroup.LayoutParams(200, 260);
			SetBackgroundResource( Resource.Drawable.border);
			AddView(GetLabel( "Point " + number, Color.White));
			AddView(GetLabel("X Value", Color.DarkGray ));
			SetX();
			AddView(GetLabel("Y Value", Color.DarkGray));
			SetY();
		}
		public PointViewCore(string name, Context context)
			: base(context)
		{
			Orientation = Orientation.Vertical;
			_context = context;
			LayoutParameters = new ViewGroup.LayoutParams(200, 260);
			SetBackgroundResource(Resource.Drawable.border);
			AddView(GetLabel(name, Color.White));
			AddView(GetLabel("X Value", Color.DarkGray));
			SetX();
			AddView(GetLabel("Y Value", Color.DarkGray));
			SetY();
		}
		#endregion Constructor

		#region Methods


		private TextView GetLabel(string number, Color color)
		{
			var block = new TextView(_context)
			{
				Text =  number,
				//LayoutParameters = new ViewGroup.LayoutParams(_helper.Factor(100), _helper.Factor(40)),
				LayoutParameters = new ViewGroup.LayoutParams(100, 40),
				Gravity = GravityFlags.Center
			};
			block.SetTextColor(color);

			return block;
		}
		private void SetX()
		{
			XValue = new Spinner(_context)
			{
				Adapter = GetAdapter(),
				LayoutParameters = new ViewGroup.LayoutParams(100, ViewGroup.LayoutParams.WrapContent)
			};

			AddView(XValue);
		}

		private ArrayAdapter GetAdapter()
		{
			var adapter = new ArrayAdapter(_context, Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			foreach (var item in _values)
			{
				adapter.Add(item.ToString(CultureInfo.InvariantCulture));
			}
			return adapter;
		}
		private void SetY()
		{
			YValue = new Spinner(_context)
			{
				Adapter = GetAdapter(),
				LayoutParameters = new ViewGroup.LayoutParams(100, ViewGroup.LayoutParams.WrapContent)
			};
			AddView(YValue);
		}

		public Point GetPoint()
		{
			var x = int.Parse(XValue.SelectedItem.ToString());
			var y = int.Parse(YValue.SelectedItem.ToString());

			return new Point(x, y);
		}
		#endregion Methods
	}
}