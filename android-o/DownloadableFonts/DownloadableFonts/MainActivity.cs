using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using System.Collections.Generic;
using Java.Util;
using Android.Support.Design.Widget;
using Android.Text;
using Java.Lang;
using Android.Util;
using Android.Support.V4.Provider;

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

		HashSet<string> FamilyNameSet;

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

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			InitializeSeekBars();
			FamilyNameSet = new HashSet<string>();
			FamilyNameSet.UnionWith(Resources.GetStringArray(Resource.Array.family_names));

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
				.WithWidth(ProgressToWidth(WidthSeekBar.Progress)
				.WithWeight(ProgressToWeight(WeightSeekBar.Progress)
				.WithItalic(ProgressToItalic(ItalicSeekBar.Progress)
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
		}

		// TODO: falta agregar mas



		bool IsValidFamilyName(string familyName)
		{
			return familyName != null && FamilyNameSet.Contains(familyName);
		}
	}
}

