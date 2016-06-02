using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using MathBase;
using MathPro.Builder;

namespace MathPro.Menu
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class AlgebraMenu : Activity
	{
		readonly ISharedPreferences _defaultSharedPreferences = 
			PreferenceManager.GetDefaultSharedPreferences(Application.Context);
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Algebra_Menu);
			

			(FindViewById<Button>(Resource.Id.btnSolve)).Click += Solve;
			(FindViewById<Button>(Resource.Id.btnSimplify)).Click += Simplify;
			(FindViewById<Button>(Resource.Id.btnDistribute)).Click += Distribute;
			(FindViewById<Button>(Resource.Id.btnFactor)).Click += Factor;
			
		}
		private void Solve(object sender, EventArgs e)
		{
			Proceed(SolveForXStepType.Equation, MathBase.Action.Solve);
		}
		private void Factor(object sender, EventArgs e)
		{
			Proceed(SolveForXStepType.Equation, MathBase.Action.Factor);
		}
		private void Distribute(object sender, EventArgs e)
		{
			Proceed(SolveForXStepType.Equation, MathBase.Action.Distribute);
		}
		private void Simplify(object sender, EventArgs e)
		{
			Proceed(SolveForXStepType.Equation, MathBase.Action.Simplify);
		}

		private void Proceed(SolveForXStepType type, MathBase.Action action)
		{
			SetPreference("SolveForXStepType", type.ToString());
			SetPreference("Action", action.ToString());
			var i = new Intent(this, typeof(MainActivity));
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