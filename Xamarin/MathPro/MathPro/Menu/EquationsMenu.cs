using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using MathBase;
using MathPro.Builders;

namespace MathPro.Menu
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class EquationsMenu: Activity
	{
		readonly ISharedPreferences _defaultSharedPreferences =
			PreferenceManager.GetDefaultSharedPreferences(Application.Context);
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Equations_Menu);


			(FindViewById<Button>(Resource.Id.btnBasic)).Click += Equations;
			(FindViewById<Button>(Resource.Id.btnExponential)).Click += Exponential;
			(FindViewById<Button>(Resource.Id.btnLogarithmic)).Click += Logarithmic;
			(FindViewById<Button>(Resource.Id.btnInequalities)).Click += Inequalities;
		}

		private void Equations(object sender, EventArgs e)
		{
			var i = new Intent(this, typeof(AlgebraMenu));
			StartActivity(i);
		}
		private void Inequalities(object sender, EventArgs e)
		{
			SetPreference("Action", MathBase.Action.Inequalities.ToString());
			SetPreference("SolveForXStepType", SolveForXStepType.Equation.ToString());
			var i = new Intent(this, typeof(InequalitiesBuilderPage));
			StartActivity(i);
		}
		private void Logarithmic(object sender, EventArgs e)
		{
			SetPreference("Action", MathBase.Action.Solve.ToString());
			SetPreference("SolveForXStepType", SolveForXStepType.LogEquation.ToString());
			var i = new Intent(this, typeof(LogarithmicBuilderPage));
			StartActivity(i);
		}
		private void Exponential(object sender, EventArgs e)
		{
			SetPreference("Action", MathBase.Action.Solve.ToString());
			SetPreference("SolveForXStepType", SolveForXStepType.ExpoEquation.ToString());
			var i = new Intent(this, typeof(ExponentialBuilderPage));
			StartActivity(i);
		}
		public void SetPreference(string name, string value)
		{
			var editor = _defaultSharedPreferences.Edit();
			editor.PutString(name, value);
			editor.Apply();
		}
	}
}