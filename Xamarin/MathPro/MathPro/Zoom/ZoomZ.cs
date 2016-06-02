using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace MathPro.Zoom
{
	public class ZoomZ: LinearLayout
	{
		private readonly ScaleGestureDetector _mGestureDetector;
		public ZoomZ(Context mContext)
			: base(mContext)
		{
			_mGestureDetector = new ScaleGestureDetector(mContext, new ScaleListener());
			//set height and widht
			//set orientation
			//set padsding
			//pass event handler for handling onscale.
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			_mGestureDetector.OnTouchEvent(e);
			return true;
		}


		public override void Draw(Canvas canvas)
		{
			//super.onDraw(canvas);

			//canvas.save();
			//canvas.scale(mScaleFactor, mScaleFactor);
			//...
			//Your onDraw() code
			//...
			base.Draw(canvas);
		}


	}

	public class ScaleListener : ScaleGestureDetector.IOnScaleGestureListener
	{

		public bool OnScale(ScaleGestureDetector detector)
		{
			
			return true;
		}

		public bool OnScaleBegin(ScaleGestureDetector detector)
		{
			return true;
			//throw new NotImplementedException();
		}

		public void OnScaleEnd(ScaleGestureDetector detector)
		{
			//throw new NotImplementedException();
		}


		public IntPtr Handle
		{
			get { return new IntPtr(); }
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}
	}
}