using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace ClinSchd.Infrastructure.Controls
{
	public class AsyncWrapper: UserControl
	{
		protected override void OnInitialized (EventArgs e)
		{
			base.OnInitialized (e);
			Grid grid = new Grid ();

			loadingImage = new GifImage (new System.Uri("pack://application:,,,/ClinSchd.Infrastructure;component/Controls/Resources/ajax-loader.gif"));
			loadingImage.Width = 66;
			loadingImage.Height = 66;
			loadingImage.Visibility = BusyOnLoad ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
			loadingImage.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			loadingImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			
			contentPresenter = new ContentPresenter ();
			contentPresenter.Content = Content;

			grid.Children.Add (contentPresenter);
			grid.Children.Add (loadingImage);
			Content = grid;

			this.Loaded += new RoutedEventHandler (AsyncWrapper_Loaded);
		}

		void AsyncWrapper_Loaded (object sender, RoutedEventArgs e)
		{
			if (!isRegistered) {
				RegisterWithModel ();
				isRegistered = true;
			}
		}

		bool isRegistered;
		Image loadingImage;
		ContentPresenter contentPresenter;
		public void EnableScreen ()
		{
			contentPresenter.Opacity = 1;
			loadingImage.Visibility = System.Windows.Visibility.Hidden;
			this.IsEnabled = true;
		}

		public void DisableScreen ()
		{
			contentPresenter.Opacity = .5;
			loadingImage.Visibility = System.Windows.Visibility.Visible;
			this.IsEnabled = false;
		}

		public static readonly DependencyProperty BackingModelProperty =
			DependencyProperty.Register ("BackingModel", typeof (AsyncValidatableModel), typeof (AsyncWrapper));

		public AsyncValidatableModel BackingModel 
		{
			get { return (AsyncValidatableModel)GetValue (BackingModelProperty); }
			set { SetValue (BackingModelProperty, value); } 
		}

		public static readonly DependencyProperty BusyOnLoadProperty =
			DependencyProperty.Register ("BusyOnLoad", typeof (bool), typeof (AsyncWrapper));

		public bool BusyOnLoad
		{
			get { return (bool)GetValue (BusyOnLoadProperty); }
			set { SetValue (BusyOnLoadProperty, value); }
		}

		private void RegisterWithModel ()
		{
			if (BackingModel == null) {
				throw new System.InvalidCastException ("AsyncWrapper must have its DataContext bound to an instance of AsyncValidatableModel.");
			}
			BackingModel.AsyncWrapper = this;
		}

		class GifImage : Image
		{
			public int FrameIndex
			{
				get { return (int)GetValue (FrameIndexProperty); }
				set { SetValue (FrameIndexProperty, value); }
			}

			public static readonly DependencyProperty FrameIndexProperty =
				DependencyProperty.Register ("FrameIndex", typeof (int), typeof (GifImage), new UIPropertyMetadata (0, new PropertyChangedCallback (ChangingFrameIndex)));

			static void ChangingFrameIndex (DependencyObject obj, DependencyPropertyChangedEventArgs ev)
			{
				GifImage ob = obj as GifImage;
				ob.Source = ob.gf.Frames[(int)ev.NewValue];
				ob.InvalidateVisual ();
			}
			GifBitmapDecoder gf;
			Int32Animation anim;
			public GifImage (Uri uri)
			{
				gf = new GifBitmapDecoder (uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
				anim = new Int32Animation (0, gf.Frames.Count - 1, new Duration (new TimeSpan (0, 0, 0, gf.Frames.Count / 12, (int)((gf.Frames.Count / 12.0 - gf.Frames.Count / 12) * 1000))));
				anim.RepeatBehavior = RepeatBehavior.Forever;
				Source = gf.Frames[0];
			}
			bool animationIsWorking = false;
			protected override void OnRender (DrawingContext dc)
			{
				base.OnRender (dc);
				if (!animationIsWorking) {
					BeginAnimation (FrameIndexProperty, anim);
					animationIsWorking = true;
				}
			}
		}
	}
}
