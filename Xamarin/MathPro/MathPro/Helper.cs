using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Widget;
using MathBase;
using MathPro.Builder;
using MathPro.Display;
using Action = MathBase.Action;

namespace MathPro
{
	public class Helper
	{
		readonly ISharedPreferences _defaultSharedPreferences =
			PreferenceManager.GetDefaultSharedPreferences(Application.Context);

		public double MinZoom = 1;
		public double MaxZoom = 5;
		public void SetPreference(string name, string value)
		{
			var editor = _defaultSharedPreferences.Edit();
			editor.PutString(name, value);
			editor.Apply();
		}
		public string ReadPreferent(string name)
		{
			var number = _defaultSharedPreferences.GetString(name, "");
			return number.ToLower();

		}
		DispatcherTimer _timer = new DispatcherTimer();
		public ISolveForXStep GetProblemDefinition(SolveForXStepType type, Action action, Context context)
		{
			var top = ReadPreferent("TopEqu");
			var bot = ReadPreferent("BotEqu");

			if (type == SolveForXStepType.Equation || type == SolveForXStepType.FactorEquations)
			{
				if (action == Action.Inequalities)
				{
					var builder = new InequalitiesBuilder(ref _timer, context) { Top = top, Bot = bot };
					return builder.GetEquation(context);
				}
				else
				{
					var builder = new GeneralBuilder(EquationType.Algebraic, action, context) {Top = top, Bot = bot};
					return builder.GetEquation(context);
				}
			}
			if (type == SolveForXStepType.ExpoEquation)
			{

				var builder = new ExponentialBuilder(ref _timer, context) { Top = top };
				return builder.GetEquation(context);
			}
			if (type == SolveForXStepType.LogEquation)
			{
				var builder = new LogarithimsBuilder(ref _timer, context) { Top = top, Bot = bot };
				return builder.GetEquation(context);
			}
			return null;
		}

		public int Factor(int value)
		{
			return int.Parse(Math.Round(value * GetZoomLevel()).ToString(CultureInfo.InvariantCulture).Split('.')[0]);
		}

		private double GetZoomLevel()
		{
			var value = ReadPreferent("zoom_level");

			return value == "" ? Convert.ToDouble("2,0") : Convert.ToDouble(value);
		}

		public void ChangeZoom(int change, Button zoomIn, Button zoomOut, System.Action followWith)
		{
			var zoomStr = ReadPreferent("zoom_level");
			var zoom = zoomStr != "" ? Convert.ToDouble(zoomStr) : 2;

			if (!(zoom + change <= MaxZoom) || !(zoom + change >= MinZoom)) return;
			SetPreference("zoom_level", (zoom + change).ToString(CultureInfo.InvariantCulture));

			zoomIn.Enabled = zoom + change < MaxZoom;

			zoomOut.Enabled = zoom + change > MinZoom;

			followWith.Invoke();

		}
	}
}