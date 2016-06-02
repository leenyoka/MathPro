using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using MathBase;
using MathPro.Display;

namespace MathPro.Builder
{
    [Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
		private readonly Helper _helper = new Helper();
		private GeneralBuilder _builder;
		readonly ISharedPreferences _defaultSharedPreferences =
		PreferenceManager.GetDefaultSharedPreferences(Application.Context);
	    protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
		    Start();

			var value2 = ReadPreferent("Action");
			(FindViewById<Button>(Resource.Id.myButton)).SetText(value2, TextView.BufferType.Normal);
		    (FindViewById<Button>(Resource.Id.myButton)).Click += Solve;
		    ZoomSetup();
        }
		private void ZoomSetup()
		{

			(FindViewById(Resource.Id.zoomIn)).Click += ZoomIn;
			(FindViewById(Resource.Id.zoomOut)).Click += ZoomOut;
		}

		private void ZoomIn(object sender, EventArgs e)
		{
			ChangeZoom(1);
		}

		private void ZoomOut(object sender, EventArgs e)
		{
			ChangeZoom(-1);
		}

		public void ChangeZoom(int change)
		{
			_helper.ChangeZoom(change, ((Button)FindViewById(Resource.Id.zoomIn)),
				((Button)FindViewById(Resource.Id.zoomOut)), SetUp);
		}

		public void SetUp()
		{
			_builder.Reset(this);
		}
		private string ReadPreferent(string name)
		{
			var number = _defaultSharedPreferences.GetString(name, "");
			return number.ToLower();

		}
		private void Solve(object sender, EventArgs e)
		{
			if (_builder.Top.Trim().Length > 0)
			{
				SetPreference("TopEqu", _builder.Top);
				SetPreference("BotEqu", _builder.Bot);

				var i = new Intent(this, typeof (SolutionPlayer));
				StartActivity(i);
			}
		}
		public void SetPreference(string name, string value)
		{
			var editor = _defaultSharedPreferences.Edit();
			editor.PutString(name, value);
			editor.Apply();
		}

	    private void Start()
	    {
			var host = FindViewById<LinearLayout>(Resource.Id.hostess);
			var timer = new DispatcherTimer();
			_builder = new GeneralBuilder(ref timer, EquationType.Algebraic, MathBase.Action.Solve, this);
			host.AddView(_builder);
	    }
    }
}

