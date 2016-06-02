using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using MathBase;
using MathPro.Builder;
using MathPro.Display;
using Action = MathBase.Action;

namespace MathPro.Graphs
{
    [Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
    public class GraphBuilder : Activity
    {
		private GeneralBuilder _builder;
		readonly ISharedPreferences _defaultSharedPreferences =
		PreferenceManager.GetDefaultSharedPreferences(Application.Context);
	    protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
		    Start();
			(FindViewById<Button>(Resource.Id.myButton)).SetText("Plot", TextView.BufferType.Normal);
		    (FindViewById<Button>(Resource.Id.myButton)).Click += Solve;
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

				var i = new Intent(this, typeof(StraighLine));
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
			_builder = new GeneralBuilder(ref timer, EquationType.Algebraic, Action.Solve, this);
			host.AddView(_builder);
	    }

    }
}

