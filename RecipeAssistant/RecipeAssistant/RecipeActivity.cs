
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Org.Json;
using Android.Views.Animations;
using Android.Graphics;

namespace RecipeAssistant
{
	[Activity (Label = "@string/app_name")]			
	public class RecipeActivity : Activity
	{
		const string Tag = "RecipeAssistant";
		string recipeName;
		Recipe recipe;
		ImageView imageView;
		TextView titleTextView, summaryTextView, ingredientsTextView;
		LinearLayout stepsLayout;

		protected override void OnStart ()
		{
			base.OnStart ();
			Intent intent = Intent;
			recipeName = intent.GetStringExtra (Constants.RecipeNameToLoad);
			if (Log.IsLoggable (Tag, LogPriority.Debug)) {
				Log.Debug (Tag, "Intent: " + intent.ToString () + " " + recipeName);
			}
			LoadRecipe ();
		}
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.recipe);
			titleTextView = (TextView)FindViewById (Resource.Id.recipeTextTitle);
			summaryTextView = (TextView)FindViewById (Resource.Id.recipeTextSummary);
			imageView = (ImageView)FindViewById (Resource.Id.recipeImageView);
			ingredientsTextView = (TextView)FindViewById (Resource.Id.textIngredients);
			stepsLayout = (LinearLayout)FindViewById (Resource.Id.layoutSteps);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.action_cook:
				StartCooking ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		private void LoadRecipe () {
			JSONObject jsonObject = AssetUtils.LoadJSONAsset (this, recipeName);
			if (jsonObject != null) {
				recipe = Recipe.FromJson (this, jsonObject);
				if (recipe != null)
					DisplayRecipe (recipe);
			}
		}

		private void DisplayRecipe (Recipe recipe) {
			Animation fadeIn = AnimationUtils.LoadAnimation (this, Android.Resource.Animation.FadeIn);
			titleTextView.Animation = fadeIn;
			titleTextView.Text = recipe.TitleText;
			summaryTextView.Text = recipe.SummaryText;
			if (recipe.RecipeImage != null) {
				imageView.Animation = fadeIn;
				Bitmap recipeImage = AssetUtils.LoadBitmapAsset (this, recipe.RecipeImage);
				imageView.SetImageBitmap (recipeImage);
			}
			ingredientsTextView.Text = recipe.IngredientsText;

			FindViewById (Resource.Id.ingredientsHeader).Animation = fadeIn;
			FindViewById (Resource.Id.ingredientsHeader).Visibility = ViewStates.Visible;
			FindViewById (Resource.Id.stepsHeader).Animation = fadeIn;

			FindViewById (Resource.Id.stepsHeader).Animation = fadeIn;

			LayoutInflater inf = LayoutInflater.From (this);
			stepsLayout.RemoveAllViews ();
			var stepNumber = 1;
			foreach (Recipe.RecipeStep step in recipe.RecipeSteps) {
				View view = inf.Inflate (Resource.Layout.step_item, null);
				ImageView iv = (ImageView)view.FindViewById (Resource.Id.stepImageView);
				if (step.StepImage == null) {
					iv.Visibility = ViewStates.Gone;
				} else {
					Bitmap stepImage = AssetUtils.LoadBitmapAsset (this, step.StepImage);
					iv.SetImageBitmap (stepImage);
				}
				((TextView)view.FindViewById (Resource.Id.textStep)).Text = (stepNumber++) + ". " + step.StepText;
				stepsLayout.AddView(view);
			}
		}

		private void StartCooking() {
			Intent intent = new Intent (this, typeof(RecipeService));
			intent.SetAction (Constants.ActionStartCooking);
			intent.PutExtra(Constants.ExtraRecipe, recipe.ToBundle());
			StartService(intent);
		}
	}
}

