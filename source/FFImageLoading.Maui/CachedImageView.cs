using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FFImageLoading.Helpers;
using FFImageLoading.Maui.Args;
using System.Windows.Input;
using FFImageLoading.Cache;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FFImageLoading.Maui
{
	[Preserve(AllMembers = true)]
	public class CachedImageView : Border
	{
		public double EdgeWidth => Padding.VerticalThickness + StrokeThickness * 2;
		public double EdgeHeight => Padding.HorizontalThickness + StrokeThickness * 2;
		public double ImageWidth => ImageView.Width;
		public double ImageHeight => ImageView.Height;
		public CachedImage ImageView { get; private set; }

		/// <summary>
		/// CachedImageView by Microspaze
		/// </summary>
		public CachedImageView()
		{
			//Border's default width will fullfill parent element. Set to zero to hide it before image loaded.
			WidthRequest = -1;
			ImageView = new CachedImage();
			ImageView.SizeChanged += OnSizeChanged;
			Content = ImageView;
		}

		/// <summary>
		/// Keep border's size updated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSizeChanged(object sender, EventArgs e)
		{
			if (ImageWidth <= 0 || ImageHeight <= 0) { return; }
			base.Stroke = Stroke;
			//base.WidthRequest = ImageWidth + EdgeWidth;
			//base.HeightRequest = ImageHeight + EdgeHeight;
			MaximumWidthRequest = ImageWidth + EdgeWidth;
			//MaximumHeightRequest = ImageHeight + EdgeHeight;
			//Arrange(new Rect(0,0, ImageWidth + EdgeWidth, ImageHeight + EdgeHeight));
		}

		#region Bindable Definitions

		/// <summary>
		/// The aspect property.
		/// </summary>
		public static readonly BindableProperty AspectProperty = BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(CachedImageView), Aspect.AspectFit, propertyChanged: (bindable, oldValue, newValue)=> OnImageViewPropertyChanged(bindable, nameof(Aspect), newValue));

		/// <summary>
		/// The override stroke property.
		/// </summary>
		public static readonly new BindableProperty StrokeProperty = BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(CachedImageView));

		/// <summary>
		/// The override widthRequest property.
		/// </summary>
		public static readonly new BindableProperty WidthRequestProperty = BindableProperty.Create(nameof(WidthRequest), typeof(double), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(WidthRequest), newValue));

		/// <summary>
		/// The override heightRequest property.
		/// </summary>
		public static readonly new BindableProperty HeightRequestProperty = BindableProperty.Create(nameof(HeightRequest), typeof(double), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(HeightRequest), newValue));

		/// <summary>
		/// The is loading property key.
		/// </summary>
		public static readonly BindablePropertyKey IsLoadingPropertyKey = BindableProperty.CreateReadOnly(nameof(IsLoading), typeof(bool), typeof(CachedImageView), false);
		public static readonly BindableProperty IsLoadingProperty = IsLoadingPropertyKey.BindableProperty;

		/// <summary>
		/// The is opaque property.
		/// </summary>
		public static readonly BindableProperty IsOpaqueProperty = BindableProperty.Create(nameof(IsOpaque), typeof(bool), typeof(CachedImageView), false, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(IsOpaque), newValue));

		/// <summary>
		/// The source property.
		/// </summary>
		public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(CachedImageView), default(ImageSource), BindingMode.OneWay, coerceValue: CoerceImageSource, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(Source), newValue));

		/// <summary>
		/// The retry count property.
		/// </summary>
		public static readonly BindableProperty RetryCountProperty = BindableProperty.Create(nameof(RetryCount), typeof(int), typeof(CachedImageView), 3, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(RetryCount), newValue));

		/// <summary>
		/// The retry delay property.
		/// </summary>
		public static readonly BindableProperty RetryDelayProperty = BindableProperty.Create(nameof(RetryDelay), typeof(int), typeof(CachedImageView), 250, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(RetryDelay), newValue));

		/// <summary>
		/// The loading delay property.
		/// </summary>
		public static readonly BindableProperty LoadingDelayProperty = BindableProperty.Create(nameof(LoadingDelay), typeof(int?), typeof(CachedImageView), default(int?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(LoadingDelay), newValue));

		/// <summary>
		/// The downsample width property.
		/// </summary>
		public static readonly BindableProperty DownsampleWidthProperty = BindableProperty.Create(nameof(DownsampleWidth), typeof(double), typeof(CachedImageView), 0d, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(DownsampleWidth), newValue));

		/// <summary>
		/// The downsample height property.
		/// </summary>
		public static readonly BindableProperty DownsampleHeightProperty = BindableProperty.Create(nameof(DownsampleHeight), typeof(double), typeof(CachedImageView), 0d, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(DownsampleHeight), newValue));

		/// <summary>
		/// The downsample to view size property.
		/// </summary>
		public static readonly BindableProperty DownsampleToViewSizeProperty = BindableProperty.Create(nameof(DownsampleToViewSize), typeof(bool), typeof(CachedImageView), false, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(DownsampleToViewSize), newValue));

		/// <summary>
		/// The downsample use dip units property.
		/// </summary>
		public static readonly BindableProperty DownsampleUseDipUnitsProperty = BindableProperty.Create(nameof(DownsampleUseDipUnits), typeof(bool), typeof(CachedImageView), false, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(DownsampleUseDipUnits), newValue));

		/// <summary>
		/// The cache duration property.
		/// </summary>
		public static readonly BindableProperty CacheDurationProperty = BindableProperty.Create(nameof(CacheDuration), typeof(TimeSpan?), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(CacheDuration), newValue));

		/// <summary>
		/// The loading priority property.
		/// </summary>
		public static readonly BindableProperty LoadingPriorityProperty = BindableProperty.Create(nameof(LoadingPriority), typeof(Work.LoadingPriority), typeof(CachedImageView), Work.LoadingPriority.Normal, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(LoadingPriority), newValue));

		/// <summary>
		/// The bitmap optimizations property.
		/// </summary>
		public static readonly BindableProperty BitmapOptimizationsProperty = BindableProperty.Create(nameof(BitmapOptimizations), typeof(bool?), typeof(CachedImageView), default(bool?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(BitmapOptimizations), newValue));

		/// <summary>
		/// The fade animation for cached images enabled property.
		/// </summary>
		public static readonly BindableProperty FadeAnimationForCachedImagesProperty = BindableProperty.Create(nameof(FadeAnimationForCachedImages), typeof(bool?), typeof(CachedImageView), default(bool?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(FadeAnimationForCachedImages), newValue));

		/// <summary>
		/// The fade animation enabled property.
		/// </summary>
		public static readonly BindableProperty FadeAnimationEnabledProperty = BindableProperty.Create(nameof(FadeAnimationEnabled), typeof(bool?), typeof(CachedImageView), default(bool?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(FadeAnimationEnabled), newValue));

		/// <summary>
		/// The fade animation duration property.
		/// </summary>
		public static readonly BindableProperty FadeAnimationDurationProperty = BindableProperty.Create(nameof(FadeAnimationDuration), typeof(int?), typeof(CachedImageView), default(int?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(FadeAnimationDuration), newValue));

		/// <summary>
		/// The loading placeholder property.
		/// </summary>
		public static readonly BindableProperty LoadingPlaceholderProperty = BindableProperty.Create(nameof(LoadingPlaceholder), typeof(ImageSource), typeof(CachedImageView), default(ImageSource), coerceValue: CoerceImageSource, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(LoadingPlaceholder), newValue));

		/// <summary>
		/// The error placeholder property.
		/// </summary>
		public static readonly BindableProperty ErrorPlaceholderProperty = BindableProperty.Create(nameof(ErrorPlaceholder), typeof(ImageSource), typeof(CachedImageView), default(ImageSource), coerceValue: CoerceImageSource, propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(ErrorPlaceholder), newValue));

		/// <summary>
		/// The TransformPlaceholders property.
		/// </summary>
		public static readonly BindableProperty TransformPlaceholdersProperty = BindableProperty.Create(nameof(TransformPlaceholders), typeof(bool?), typeof(CachedImageView), default(bool?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(TransformPlaceholders), newValue));

		/// <summary>
		/// The transformations property.
		/// </summary>
		public static readonly BindableProperty TransformationsProperty = BindableProperty.Create(nameof(Transformations), typeof(List<Work.ITransformation>), typeof(CachedImageView), new List<Work.ITransformation>(), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(Transformations), newValue));

		/// <summary>
		/// The invalidate layout after loaded property.
		/// </summary>
		public static readonly BindableProperty InvalidateLayoutAfterLoadedProperty = BindableProperty.Create(nameof(InvalidateLayoutAfterLoaded), typeof(bool?), typeof(CachedImageView), default(bool?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(InvalidateLayoutAfterLoaded), newValue));

		/// <summary>
		/// The cache type property.
		/// </summary>
		public static readonly BindableProperty CacheTypeProperty = BindableProperty.Create(nameof(CacheType), typeof(CacheType?), typeof(CachedImageView), default(CacheType?), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(CacheType), newValue));

		#endregion

		#region Bindable Properties

		/// <summary>
		/// Gets or sets the aspect.
		/// </summary>
		/// <value>The aspect.</value>
		public Aspect Aspect
		{
			get => (Aspect)GetValue(AspectProperty);
			set => SetValue(AspectProperty, value);
		}

		/// <summary>
		/// Gets or sets the aspect.
		/// </summary>
		/// <value>The aspect.</value>
		public Brush Stroke
		{
			get => (Brush)GetValue(StrokeProperty);
			set => SetValue(StrokeProperty, value);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is loading.
		/// </summary>
		/// <value><c>true</c> if this instance is loading; otherwise, <c>false</c>.</value>
		public bool IsLoading => (bool)GetValue(IsLoadingProperty);

		/// <summary>
		/// Gets or sets a value indicating whether this instance is opaque.
		/// </summary>
		/// <value><c>true</c> if this instance is opaque; otherwise, <c>false</c>.</value>
		public bool IsOpaque
		{
			get => (bool)GetValue(IsOpaqueProperty);
			set => SetValue(IsOpaqueProperty, value);
		}

		/// <summary>
		/// Gets or sets the source.
		/// </summary>
		/// <value>The source.</value>
		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource Source
		{
			get => (ImageSource)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		/// <summary>
		/// If image loading fails automatically retry it a number of times, with a specific delay. Sets number of retries.
		/// </summary>
		public int RetryCount
		{
			get => (int)GetValue(RetryCountProperty);
			set => SetValue(RetryCountProperty, value);
		}

		/// <summary>
		/// If image loading fails automatically retry it a number of times, with a specific delay. Sets delay in milliseconds between each trial
		/// </summary>
		public int RetryDelay
		{
			get => (int)GetValue(RetryDelayProperty);
			set => SetValue(RetryDelayProperty, value);
		}

		/// <summary>
		/// Sets delay in milliseconds before image loading
		/// </summary>
		public int? LoadingDelay
		{
			get => (int?)GetValue(LoadingDelayProperty);
			set => SetValue(LoadingDelayProperty, value);
		}

		/// <summary>
		/// Reduce memory usage by downsampling the image. Aspect ratio will be kept even if width/height values are incorrect.
		/// Optional DownsampleWidth parameter, if value is higher than zero it will try to downsample to this width while keeping aspect ratio.
		/// </summary>
		public double DownsampleWidth
		{
			get => (double)GetValue(DownsampleWidthProperty);
			set => SetValue(DownsampleWidthProperty, value);
		}

		/// <summary>
		/// Reduce memory usage by downsampling the image. Aspect ratio will be kept even if width/height values are incorrect.
		/// Optional DownsampleHeight parameter, if value is higher than zero it will try to downsample to this height while keeping aspect ratio.
		/// </summary>
		public double DownsampleHeight
		{
			get => (double)GetValue(DownsampleHeightProperty);
			set => SetValue(DownsampleHeightProperty, value);
		}

		/// <summary>
		/// Reduce memory usage by downsampling the image. Aspect ratio will be kept even if width/height values are incorrect.
		/// DownsampleWidth and DownsampleHeight properties will be automatically set to view size
		/// </summary>
		/// <value><c>true</c> if downsample to view size; otherwise, <c>false</c>.</value>
		public bool DownsampleToViewSize
		{
			get => (bool)GetValue(DownsampleToViewSizeProperty);
			set => SetValue(DownsampleToViewSizeProperty, value);
		}

		/// <summary>
		/// If set to <c>true</c> DownsampleWidth and DownsampleHeight properties
		/// will use density independent pixels for downsampling
		/// </summary>
		/// <value><c>true</c> if downsample use dip units; otherwise, <c>false</c>.</value>
		public bool DownsampleUseDipUnits
		{
			get => (bool)GetValue(DownsampleUseDipUnitsProperty);
			set => SetValue(DownsampleUseDipUnitsProperty, value);
		}

		/// <summary>
		/// How long the file will be cached on disk.
		/// </summary>
		public TimeSpan? CacheDuration
		{
			get => (TimeSpan?)GetValue(CacheDurationProperty);
			set => SetValue(CacheDurationProperty, value);
		}

		/// <summary>
		/// Defines the loading priority, the default is LoadingPriority.Normal
		/// </summary>
		public Work.LoadingPriority LoadingPriority
		{
			get => (Work.LoadingPriority)GetValue(LoadingPriorityProperty);
			set => SetValue(LoadingPriorityProperty, value);
		}

		/// <summary>
		/// Enables or disables the bitmap optimizations.
		/// </summary>
		/// <value>The bitmap optimizations.</value>
		public bool? BitmapOptimizations
		{
			get => (bool?)GetValue(BitmapOptimizationsProperty);
			set => SetValue(BitmapOptimizationsProperty, value);
		}

		/// <summary>
		/// Indicates if the fade animation effect for cached images should be enabled. By default this value comes from ImageService.Instance.Config.FadeAnimationForCachedImages.
		/// </summary>
		public bool? FadeAnimationForCachedImages
		{
			get => (bool?)GetValue(FadeAnimationForCachedImagesProperty);
			set => SetValue(FadeAnimationForCachedImagesProperty, value);
		}

		/// <summary>
		/// Indicates if the fade animation effect should be enabled. By default this value comes from ImageService.Instance.Config.FadeAnimationEnabled.
		/// </summary>
		public bool? FadeAnimationEnabled
		{
			get => (bool?)GetValue(FadeAnimationEnabledProperty);
			set => SetValue(FadeAnimationEnabledProperty, value);
		}

		/// <summary>
		/// Sets fade animation effect duration. By default this value comes from ImageService.Instance.Config.FadeAnimationDuration.
		/// </summary>
		public int? FadeAnimationDuration
		{
			get => (int?)GetValue(FadeAnimationDurationProperty);
			set => SetValue(FadeAnimationDurationProperty, value);
		}

		/// <summary>
		/// Gets or sets the loading placeholder image.
		/// </summary>
		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource LoadingPlaceholder
		{
			get => (ImageSource)GetValue(LoadingPlaceholderProperty);
			set => SetValue(LoadingPlaceholderProperty, value);
		}

		/// <summary>
		/// Gets or sets the error placeholder image.
		/// </summary>
		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource ErrorPlaceholder
		{
			get => (ImageSource)GetValue(ErrorPlaceholderProperty);
			set => SetValue(ErrorPlaceholderProperty, value);
		}

		/// <summary>
		/// Indicates if transforms should be applied to placeholders. By default this value comes from ImageService.Instance.Config.TransformPlaceholders.
		/// </summary>
		/// <value>The transform placeholders.</value>
		public bool? TransformPlaceholders
		{
			get => (bool?)GetValue(TransformPlaceholdersProperty);
			set => SetValue(TransformPlaceholdersProperty, value);
		}

		/// <summary>
		/// Gets or sets the transformations.
		/// </summary>
		/// <value>The transformations.</value>
		public List<Work.ITransformation> Transformations
		{
			get => (List<Work.ITransformation>)GetValue(TransformationsProperty);
			set => SetValue(TransformationsProperty, value);
		}

		/// <summary>
		/// Specifies if view layout should be invalidated after image is loaded.
		/// </summary>
		/// <value>The invalidate layout after loaded.</value>
		public bool? InvalidateLayoutAfterLoaded
		{
			get => (bool?)GetValue(InvalidateLayoutAfterLoadedProperty);
			set => SetValue(InvalidateLayoutAfterLoadedProperty, value);
		}

		/// <summary>
		/// Set the cache storage type, (Memory, Disk, All). by default cache is set to All.
		/// </summary>
		public CacheType? CacheType
		{
			get => (CacheType?)GetValue(CacheTypeProperty);
			set => SetValue(CacheTypeProperty, value);
		}

		#endregion

		#region Events & Commands

		/// <summary>
		/// Occurs after image loading success.
		/// </summary>
		public event EventHandler<CachedImageEvents.SuccessEventArgs> Success;

		/// <summary>
		/// Occurs after image loading error.
		/// </summary>
		public event EventHandler<CachedImageEvents.ErrorEventArgs> Error;

		/// <summary>
		/// Occurs after every image loading.
		/// </summary>
		public event EventHandler<CachedImageEvents.FinishEventArgs> Finish;

		/// <summary>
		/// Occurs when an image starts downloading from web.
		/// </summary>
		public event EventHandler<CachedImageEvents.DownloadStartedEventArgs> DownloadStarted;

		/// <summary>
		/// This callback can be used for reading download progress
		/// </summary>
		public event EventHandler<CachedImageEvents.DownloadProgressEventArgs> DownloadProgress;

		/// <summary>
		/// Called after file is succesfully written to the disk.
		/// </summary>
		public event EventHandler<CachedImageEvents.FileWriteFinishedEventArgs> FileWriteFinished;


		/// <summary>
		/// The SuccessCommandProperty.
		/// </summary>
		public static readonly BindableProperty SuccessCommandProperty = BindableProperty.Create(nameof(SuccessCommand), typeof(ICommand), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(SuccessCommand), newValue));

		/// <summary>
		/// The ErrorCommandProperty.
		/// </summary>
		public static readonly BindableProperty ErrorCommandProperty = BindableProperty.Create(nameof(ErrorCommand), typeof(ICommand), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(ErrorCommand), newValue));

		/// <summary>
		/// The FinishCommandProperty.
		/// </summary>
		public static readonly BindableProperty FinishCommandProperty = BindableProperty.Create(nameof(FinishCommand), typeof(ICommand), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(FinishCommand), newValue));

		/// <summary>
		/// The DownloadStartedCommandProperty.
		/// </summary>
		public static readonly BindableProperty DownloadStartedCommandProperty = BindableProperty.Create(nameof(DownloadStartedCommand), typeof(ICommand), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(DownloadStartedCommand), newValue));

		/// <summary>
		/// The DownloadProgressCommandProperty.
		/// </summary>
		public static readonly BindableProperty DownloadProgressCommandProperty = BindableProperty.Create(nameof(DownloadProgressCommand), typeof(ICommand), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(DownloadProgressCommand), newValue));

		/// <summary>
		/// The FileWriteFinishedCommandProperty.
		/// </summary>
		public static readonly BindableProperty FileWriteFinishedCommandProperty = BindableProperty.Create(nameof(FileWriteFinishedCommand), typeof(ICommand), typeof(CachedImageView), propertyChanged: (bindable, oldValue, newValue) => OnImageViewPropertyChanged(bindable, nameof(FileWriteFinishedCommand), newValue));


		/// <summary>
		/// Gets or sets the SuccessCommand.
		/// Occurs after image loading success.
		/// Command parameter: CachedImageEvents.SuccessEventArgs
		/// </summary>
		/// <value>The success command.</value>
		public ICommand SuccessCommand
		{
			get => (ICommand)GetValue(SuccessCommandProperty);
			set => SetValue(SuccessCommandProperty, value);
		}

		/// <summary>
		/// Gets or sets the ErrorCommand.
		/// Occurs after image loading error.
		/// Command parameter: CachedImageEvents.ErrorEventArgs
		/// </summary>
		/// <value>The error command.</value>
		public ICommand ErrorCommand
		{
			get => (ICommand)GetValue(ErrorCommandProperty);
			set => SetValue(ErrorCommandProperty, value);
		}

		/// <summary>
		/// Gets or sets the FinishCommand.
		/// Occurs after every image loading.
		/// Command parameter: CachedImageEvents.FinishEventArgs
		/// </summary>
		/// <value>The finish command.</value>
		public ICommand FinishCommand
		{
			get => (ICommand)GetValue(FinishCommandProperty);
			set => SetValue(FinishCommandProperty, value);
		}

		/// <summary>
		/// Gets or sets the DownloadStartedCommand.
		/// Occurs when an image starts downloading from web.
		/// Command parameter: DownloadStartedEventArgs
		/// </summary>
		/// <value>The download started command.</value>
		public ICommand DownloadStartedCommand
		{
			get => (ICommand)GetValue(DownloadStartedCommandProperty);
			set => SetValue(DownloadStartedCommandProperty, value);
		}

		/// <summary>
		/// Gets or sets the DownloadProgressCommand.
		/// This callback can be used for reading download progress
		/// Command parameter: DownloadProgressEventArgs
		/// </summary>
		/// <value>The download started command.</value>
		public ICommand DownloadProgressCommand
		{
			get => (ICommand)GetValue(DownloadProgressCommandProperty);
			set => SetValue(DownloadProgressCommandProperty, value);
		}

		/// <summary>
		/// Gets or sets the FileWriteFinishedCommand.
		/// Called after file is succesfully written to the disk.
		/// Command parameter: FileWriteFinishedEventArgs
		/// </summary>
		/// <value>The download started command.</value>
		public ICommand FileWriteFinishedCommand
		{
			get => (ICommand)GetValue(FileWriteFinishedCommandProperty);
			set => SetValue(FileWriteFinishedCommandProperty, value);
		}


		internal void OnSuccess(CachedImageEvents.SuccessEventArgs e)
		{
			Success?.Invoke(this, e);
		}

		internal void OnError(CachedImageEvents.ErrorEventArgs e)
		{
			Error?.Invoke(this, e);
		}

		internal void OnFinish(CachedImageEvents.FinishEventArgs e)
		{
			Finish?.Invoke(this, e);
		}

		internal void OnDownloadStarted(CachedImageEvents.DownloadStartedEventArgs e)
		{
			DownloadStarted?.Invoke(this, e);
		}

		internal void OnDownloadProgress(CachedImageEvents.DownloadProgressEventArgs e)
		{
			DownloadProgress?.Invoke(this, e);
		}

		internal void OnFileWriteFinished(CachedImageEvents.FileWriteFinishedEventArgs e)
		{
			FileWriteFinished?.Invoke(this, e);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Set loading property value.
		/// </summary>
		/// <param name="isLoading"></param>
		public void SetIsLoading(bool isLoading)
		{
			SetValue(IsLoadingPropertyKey, isLoading);
		}

		/// <summary>
		/// Reloads the image.
		/// </summary>
		public void ReloadImage()
		{
			ImageView?.ReloadImage();
		}

		/// <summary>
		/// Cancels image loading tasks
		/// </summary>
		public void Cancel()
		{
			ImageView?.Cancel();
		}

		/// <summary>
		/// Gets the image as JPG.
		/// </summary>
		/// <returns>The image as JPG.</returns>
		public Task<byte[]> GetImageAsJpgAsync(int quality = 90, int desiredWidth = 0, int desiredHeight = 0)
		{
			return ImageView?.GetImageAsJpgAsync(quality, desiredWidth, desiredHeight);
		}

		/// <summary>
		/// Gets the image as PNG
		/// </summary>
		/// <returns>The image as PNG.</returns>
		public Task<byte[]> GetImageAsPngAsync(int desiredWidth = 0, int desiredHeight = 0)
		{
			return ImageView?.GetImageAsPngAsync(desiredWidth, desiredHeight);
		}

		/// <summary>
		/// Invalidates cache for a specified image source.
		/// </summary>
		/// <param name="source">Image source.</param>
		/// <param name="cacheType">Cache type.</param>
		/// <param name = "removeSimilar">If set to <c>true</c> removes all image cache variants
		/// (downsampling and transformations variants)</param>
		public async Task InvalidateCache(ImageSource source, CacheType cacheType, bool removeSimilar = false)
		{
			await ImageView?.InvalidateCache(source, cacheType, removeSimilar);
		}

		/// <summary>
		/// Invalidates cache for a specified key.
		/// </summary>
		/// <param name="key">Image key.</param>
		/// <param name="cacheType">Cache type.</param>
		/// <param name = "removeSimilar">If set to <c>true</c> removes all image cache variants
		/// (downsampling and transformations variants)</param>
		public async Task InvalidateCache(string key, CacheType cacheType, bool removeSimilar = false)
		{
			await ImageView?.InvalidateCache(key, cacheType, removeSimilar);
		}

		#endregion

		#region Private Methods

		private static void OnImageViewPropertyChanged(BindableObject bindable, string propertyName, object newValue)
		{
			if (bindable is CachedImageView imageView && imageView.ImageView != null)
			{
				if (propertyName == WidthRequestProperty.PropertyName)
				{
					var widthRequest = (double)newValue;
					widthRequest -= imageView.EdgeWidth;
					imageView.ImageView.SetValue(propertyName, widthRequest);
					//imageView.ImageView.SetValue(MaximumWidthRequestProperty.PropertyName, widthRequest);
				}
				else if (propertyName == HeightRequestProperty.PropertyName)
				{
					var heightRequest = (double)newValue;
					heightRequest -= imageView.EdgeHeight;
					imageView.ImageView.SetValue(propertyName, heightRequest);
					//imageView.ImageView.SetValue(MaximumHeightRequestProperty.PropertyName, heightRequest);
				}
				else
				{
					imageView.ImageView.SetValue(propertyName, newValue);
				}
			}
		}

		private static object CoerceImageSource(BindableObject bindable, object newValue)
		{
			return ((CachedImageView)bindable).CoerceImageSource(newValue);
		}

		protected virtual ImageSource CoerceImageSource(object newValue)
		{
			var uriImageSource = newValue as UriImageSource;

			if (uriImageSource?.Uri?.OriginalString != null)
			{
				if (uriImageSource.Uri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
					return ImageSource.FromFile(uriImageSource.Uri.LocalPath);

				if (uriImageSource.Uri.Scheme.Equals("resource", StringComparison.OrdinalIgnoreCase))
					return new EmbeddedResourceImageSource(uriImageSource.Uri);

				if (uriImageSource.Uri.OriginalString.IsDataUrl())
					return new DataUrlImageSource(uriImageSource.Uri.OriginalString);
			}

			return newValue as ImageSource;
		}

		#endregion
	}
}
