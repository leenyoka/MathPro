using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace MathPro
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class Settings : Activity
	{
		private readonly Helper _helper = new Helper();
		private Spinner _spinner;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.setting_screen);
			_spinner = FindViewById<Spinner>(Resource.Id.spinnerInitialLoad);

			var levels = new [] {"1", "2", "3", "4", "5"};
			_spinner.Adapter = new ArrayAdapter(this,Android.Resource.Layout.SimpleSpinnerItem,levels);
			SetSelection();
			_spinner.ItemSelected += SelectedItemChanged;
		}

		private void SetSelection()
		{
			var value =_helper.ReadPreferent("zoom_level");
			if (value == "")
			{
				_helper.SetPreference("zoom_level", "2");
				value = _helper.ReadPreferent("zoom_level");
			}
			_spinner.SetSelection(int.Parse(value)-1);
		}
		private void SelectedItemChanged(object sender, EventArgs e)
		{
			_helper.SetPreference("zoom_level", _spinner.SelectedItem.ToString());
		}
	
	}
}