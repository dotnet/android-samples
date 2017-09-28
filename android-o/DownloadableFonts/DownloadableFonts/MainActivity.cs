using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Text;
using Java.Lang;
using Android.Util;
using Android.Support.V4.Provider;
using Java.Util;

namespace DownloadableFonts
{
	[Activity(Label = "DownloadableFonts", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		const string Tag = "MainActivity";

    	Handler Handler = null;

		TextView DownloadableFontTextView;
		SeekBar WidthSeekBar;
		SeekBar WeightSeekBar;
		SeekBar ItalicSeekBar;
		CheckBox BestEffort;
		Button RequestDownloadButton;

		HashSet FamilyNameSet;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			InitializeSeekBars();
			FamilyNameSet = new HashSet(Resources.GetStringArray(Resource.Array.family_names));

			DownloadableFontTextView = FindViewById(Resource.Id.textview) as TextView;
			ArrayAdapter<string> adapter = new ArrayAdapter<string>(this,
					Android.Resource.Layout.SimpleDropDownItem1Line,
					Resources.GetStringArray(Resource.Array.family_names));
			var familyNameInput = FindViewById(Resource.Id.auto_complete_family_name_input) as TextInputLayout;
			var autoCompleteFamilyName = FindViewById(Resource.Id.auto_complete_family_name) as AutoCompleteTextView;
			autoCompleteFamilyName.Adapter = adapter;

			autoCompleteFamilyName.AddTextChangedListener(new AutoCompleteFamilyNameTextWatcher 
				{ 
					Activity = this, FamilyNameInput = familyNameInput
				}
			);
			
			RequestDownloadButton = FindViewById(Resource.Id.button_request) as Button;
			RequestDownloadButton.Click += (sender, e) => {
				var familyName = autoCompleteFamilyName.Text;
				if (!IsValidFamilyName(familyName))
				{
					familyNameInput.ErrorEnabled = true;
					familyNameInput.Error = GetString(Resource.String.invalid_family_name);
					Toast.MakeText(this, Resource.String.invalid_input, ToastLength.Short).Show();
					return;
				}
				RequestDownload(familyName);
				RequestDownloadButton.Enabled = false;
			};
			BestEffort = FindViewById(Resource.Id.checkbox_best_effort) as CheckBox;
		}

		void RequestDownload(string familyName)
		{
			QueryBuilder queryBuilder = new QueryBuilder(familyName)
				.WithWidth(ProgressToWidth(WidthSeekBar.Progress))
				.WithWeight(ProgressToWeight(WeightSeekBar.Progress))
				.WithItalic(ProgressToItalic(ItalicSeekBar.Progress))
				.WithBestEffort(BestEffort.Checked);
			string query = queryBuilder.Build();

			Log.Debug(Tag, "Requesting a font. Query: " + query);
			FontRequest request = new FontRequest(
					"com.google.android.gms.fonts",
					"com.google.android.gms",
					query,
					Resource.Array.com_google_android_gms_fonts_certs);

			var progressBar = FindViewById(Resource.Id.progressBar) as ProgressBar;
			progressBar.Visibility = Android.Views.ViewStates.Visible;

			FontsContractCompat.FontRequestCallback callback = new FontRequestCallbackImpl
			{
				mActivity = this,
				mDownloadableFontTextView = DownloadableFontTextView,
				mRequestDownloadButton = RequestDownloadButton,
				mProgressBar = progressBar
			};

			FontsContractCompat.RequestFont(this, request, callback, GetHandlerThreadHandler());
		}

		void InitializeSeekBars()
		{
			// Setup WidthSeekBar
			WidthSeekBar = FindViewById(Resource.Id.seek_bar_width) as SeekBar;
			int widthValue = (int)(100 * (float) Constants.WIDTH_DEFAULT / Constants.WIDTH_MAX);
			WidthSeekBar.Progress = widthValue;
			var widthTextView = FindViewById(Resource.Id.textview_width) as TextView;
			widthTextView.Text = String.ValueOf(widthValue);
			var widthSeekBarListener = new SeekBarListenerImpl
			{
				mActivity = this,
				mTextView = widthTextView,
				mSeekBarType = SeekBarListenerImpl.SeekBarType.WIDTH
			};
			WidthSeekBar.SetOnSeekBarChangeListener(widthSeekBarListener);

			// Setup WeightSeekBar
			WeightSeekBar = FindViewById(Resource.Id.seek_bar_weight) as SeekBar;
			float weightValue = Constants.WEIGHT_DEFAULT / Constants.WEIGHT_MAX * 100;
			WeightSeekBar.Progress = (int) weightValue;
			var weightTextView = FindViewById(Resource.Id.textview_weight) as TextView;
			weightTextView.Text = String.ValueOf(Constants.WEIGHT_DEFAULT);
			var weightSeekBarListener = new SeekBarListenerImpl
			{
				mActivity = this,
				mTextView = weightTextView,
				mSeekBarType = SeekBarListenerImpl.SeekBarType.WEIGHT
			};
			WeightSeekBar.SetOnSeekBarChangeListener(weightSeekBarListener);

			// Setup ItalicSeekBar
			ItalicSeekBar = FindViewById(Resource.Id.seek_bar_italic) as SeekBar;
			ItalicSeekBar.Progress = (int) Constants.ITALIC_DEFAULT;
			var italicTextView = FindViewById(Resource.Id.textview_italic) as TextView;
			italicTextView.Text = String.ValueOf(Constants.ITALIC_DEFAULT);
			var italicSeekBarListener = new SeekBarListenerImpl
			{
				mActivity = this,
				mTextView = italicTextView,
				mSeekBarType = SeekBarListenerImpl.SeekBarType.ITALIC
			};
			ItalicSeekBar.SetOnSeekBarChangeListener(italicSeekBarListener);
		}

		bool IsValidFamilyName(string familyName)
		{
			return familyName != null && FamilyNameSet.Contains(familyName);
		}

		Handler GetHandlerThreadHandler()
		{
			if (Handler == null)
			{
				HandlerThread handlerThread = new HandlerThread("fonts");
				handlerThread.Start();
				Handler = new Handler(handlerThread.Looper);
			}
			return Handler;
		}

		/**
	     * Converts progress from a SeekBar to the value of width.
	     * @param progress is passed from 0 to 100 inclusive
	     * @return the converted width
	     */
		float ProgressToWidth(int progress)
		{
			return progress == 0 ? 1 : progress * Constants.WIDTH_MAX / 100;
		}

		/**
		 * Converts progress from a SeekBar to the value of weight.
		 * @param progress is passed from 0 to 100 inclusive
		 * @return the converted weight
		 */
		int ProgressToWeight(int progress)
		{
			if (progress == 0)
			{
				return 1; // The range of the weight is between (0, 1000) (exclusive)
			}
			else if (progress == 100)
			{
				return Constants.WEIGHT_MAX - 1; // The range of the weight is between (0, 1000) (exclusive)
			}
			else
			{
				return Constants.WEIGHT_MAX * progress / 100;
			}
		}

		/**
	     * Converts progress from a SeekBar to the value of italic.
	     * @param progress is passed from 0 to 100 inclusive.
	     * @return the converted italic
	     */
		float ProgressToItalic(int progress)
		{
			return (float)progress / 100f;
		}

		#region Inner Classes
		class AutoCompleteFamilyNameTextWatcher : Object, ITextWatcher
		{
			public MainActivity Activity { get; set; }
			public TextInputLayout FamilyNameInput { get; set; }


			public void AfterTextChanged(IEditable s)
			{
				// No op
			}

			public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
			{
				// No op
			}

			public void OnTextChanged(ICharSequence charSequence, int start, int count, int after)
			{
				if (Activity.IsValidFamilyName(charSequence.ToString()))
				{
					FamilyNameInput.ErrorEnabled = false;
					FamilyNameInput.Error = "";
				}
				else
				{
					FamilyNameInput.ErrorEnabled = true;
					FamilyNameInput.Error = Activity.GetString(Resource.String.invalid_family_name);
				}
			}
		}

		class FontRequestCallbackImpl : FontsContractCompat.FontRequestCallback
		{
			public TextView mDownloadableFontTextView { get; set; }
			public ProgressBar mProgressBar { get; set; }
			public Button mRequestDownloadButton { get; set; }
			public MainActivity mActivity;

			public override void OnTypefaceRetrieved(Android.Graphics.Typeface typeface)
			{
				mDownloadableFontTextView.Typeface = typeface;
				mProgressBar.Visibility = Android.Views.ViewStates.Gone;
				mRequestDownloadButton.Enabled = true;
			}

			public override void OnTypefaceRequestFailed(int reason)
			{
				Toast.MakeText(mActivity, mActivity.GetString(Resource.String.request_failed, reason),
							   ToastLength.Long).Show();
				mProgressBar.Visibility = Android.Views.ViewStates.Gone;
				mRequestDownloadButton.Enabled = true;
			}
		}

		class SeekBarListenerImpl : Object, SeekBar.IOnSeekBarChangeListener
		{
			public enum SeekBarType
			{
				WIDTH, WEIGHT, ITALIC
			}

			public SeekBarType mSeekBarType { get; set; }
			public TextView mTextView { get; set; }
			public MainActivity mActivity { get; set; }

			public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
			{
				var progressText = "";
				switch (mSeekBarType)
				{
					case SeekBarType.WIDTH:
						progressText = String.ValueOf(mActivity.ProgressToWidth(progress));
						break;
					case SeekBarType.WEIGHT:
						progressText = String.ValueOf(mActivity.ProgressToWeight(progress));
						break;
					case SeekBarType.ITALIC:
						progressText = String.ValueOf(mActivity.ProgressToItalic(progress));
						break;
				}
				mTextView.Text = progressText;
			}

			public void OnStartTrackingTouch(SeekBar seekBar)
			{
				// No op
			}

			public void OnStopTrackingTouch(SeekBar seekBar)
			{
				// No op
			}
		}
		#endregion
	}
}

