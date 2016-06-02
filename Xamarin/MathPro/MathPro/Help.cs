using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace MathPro
{
	[Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class Help : Activity
	{
		private int _current = 1;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.help_screen);
			SetImage();
			(FindViewById(Resource.Id.help_right)).Click += RightClick;
			(FindViewById(Resource.Id.help_left)).Click += LeftClick;
			(FindViewById(Resource.Id.help_demo)).Click += Demo;

			
		}

		private void SetImage()
		{
			var image = (ImageView)FindViewById(Resource.Id.help_image);
			switch (_current)
			{
				case 1: image.SetImageResource(Resource.Drawable.help_1);break;
				case 2: image.SetImageResource(Resource.Drawable.help_2); break;
				case 3: image.SetImageResource(Resource.Drawable.help_3); break;
				case 4: image.SetImageResource(Resource.Drawable.help_4); break;
				case 5: image.SetImageResource(Resource.Drawable.help_5); break;
				case 6: image.SetImageResource(Resource.Drawable.help_6); break;
				case 7: image.SetImageResource(Resource.Drawable.help_7); break;
				case 8: image.SetImageResource(Resource.Drawable.help_8); break;
				case 9: image.SetImageResource(Resource.Drawable.help_9); break;
			}
			((TextView)(FindViewById(Resource.Id.help_count))).
				SetText(_current.ToString(CultureInfo.InvariantCulture),TextView.BufferType.Normal);
		}

		private void LeftClick(object sender, EventArgs e)
		{
			if (_current > 1)
			{
				_current--;
			}
			else
			{
				_current = 9;
			}
			SetImage();
		}
		private void RightClick(object sender, EventArgs e)
		{
			if (_current < 9)
			{
				_current++;
			}
			else
			{
				_current = 1;
			}
			SetImage();
		}

		private void Demo(object sender, EventArgs e)
		{
			var uri = Android.Net.Uri.Parse("https://www.youtube.com/watch?v=cVN0n4vneJ0");
			var intent = new Intent(Intent.ActionView, uri);
			StartActivity(intent);
		}
	}
}