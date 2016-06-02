using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using MathBase;
using MathPro.Builder;
using MathPro.Display;

namespace MathPro.Builders
{
	 [Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class ExponentialBuilderPage : Activity
	{
		 private readonly Helper _helper = new Helper();
		private ExponentialBuilder builder;
		readonly ISharedPreferences _defaultSharedPreferences =
		PreferenceManager.GetDefaultSharedPreferences(Application.Context);
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);
			Start();

			(FindViewById<Button>(Resource.Id.myButton)).SetText("solve", TextView.BufferType.Normal);
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
			builder.Reset(this);
		}
		private void Solve(object sender, EventArgs e)
		{
			if (builder.Top.Trim().Length > 0)
			{
				SetPreference("TopEqu", builder.Top);
				//SetPreference("BotEqu", builder.Bot);

				var i = new Intent(this, typeof(SolutionPlayer));
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
			builder = new ExponentialBuilder(ref timer, this);
			host.AddView(builder);
		}
	}
}