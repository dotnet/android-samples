using System;
using Android.OS;
using System.Collections.Generic;
using Android.Content;
using Org.Json;
using Android.Util;

namespace RecipeAssistant
{
	public class Recipe
	{
		const string Tag = "RecipeAssistant";

		public string TitleText, SummaryText, RecipeImage, IngredientsText;

		public class RecipeStep {
			public string StepImage, StepText;

			public Bundle ToBundle() {
				var bundle = new Bundle ();
				bundle.PutString (Constants.RecipeFieldStepText, StepText);
				bundle.PutString (Constants.RecipeFieldStepImage, StepImage);
				return bundle;
			}

			public static RecipeStep FromBundle(Bundle bundle) {
				var recipeStep = new RecipeStep ();
				recipeStep.StepText = bundle.GetString (Constants.RecipeFieldStepText);
				recipeStep.StepImage = bundle.GetString (Constants.RecipeFieldStepImage);
				return recipeStep;
			}
		}

		public List<RecipeStep> RecipeSteps;

		public Recipe() {
			RecipeSteps = new List<RecipeStep> ();
		}

		public static Recipe FromJson(Context context, JSONObject json) {
			var recipe = new Recipe ();
			try 
			{
				recipe.TitleText = json.GetString(Constants.RecipeFieldTitle);
				recipe.SummaryText = json.GetString(Constants.RecipeFieldSummary);
				if (json.Has(Constants.RecipeFieldImage)) {
					recipe.RecipeImage = json.GetString(Constants.RecipeFieldImage);
				}
				JSONArray ingredients = json.GetJSONArray(Constants.RecipeFieldIngredients);
				recipe.IngredientsText = "";
				for (int i = 0; i < ingredients.Length(); i++)
				{
					recipe.IngredientsText += " - " + ingredients.GetJSONObject(i).GetString(Constants.RecipeFieldText) + "\n";
				}

				JSONArray steps = json.GetJSONArray(Constants.RecipeFieldSteps);
				for (int i = 0; i < steps.Length(); i++)
				{
					var step = steps.GetJSONObject(i);
					var recipeStep = new RecipeStep();
					recipeStep.StepText = step.GetString(Constants.RecipeFieldText);
					if (step.Has(Constants.RecipeFieldName)) {
						recipeStep.StepImage = step.GetString(Constants.RecipeFieldImage);
					}
					recipe.RecipeSteps.Add(recipeStep);
				}
			}
			catch (Exception ex) {
				Log.Error (Tag, "Error loading recipe: " + ex);
				return null;
			}
			return recipe;
		}

		public Bundle ToBundle()
		{
			var bundle = new Bundle ();
			bundle.PutString (Constants.RecipeFieldTitle, TitleText);
			bundle.PutString (Constants.RecipeFieldSummary, SummaryText);
			bundle.PutString (Constants.RecipeFieldImage, RecipeImage);
			bundle.PutString (Constants.RecipeFieldIngredients, IngredientsText);
			if (RecipeSteps != null) {
				List<IParcelable> stepBundles = new List<IParcelable> (RecipeSteps.Count);
				foreach (RecipeStep recipeStep in RecipeSteps) {
					stepBundles.Add (recipeStep.ToBundle ());
				}
				bundle.PutParcelableArrayList (Constants.RecipeFieldSteps, stepBundles);
			}
			return bundle;
		}

		public static Recipe FromBundle(Bundle bundle) {
			var recipe = new Recipe ();
			recipe.TitleText = bundle.GetString (Constants.RecipeFieldTitle);
			recipe.SummaryText = bundle.GetString (Constants.RecipeFieldSummary);
			recipe.RecipeImage = bundle.GetString (Constants.RecipeFieldImage);
			recipe.IngredientsText = bundle.GetString (Constants.RecipeFieldIngredients);
			var stepBundles = bundle.GetParcelableArrayList (Constants.RecipeFieldSteps);
			if (stepBundles != null) {
				foreach (IParcelable stepBundle in stepBundles) {
					recipe.RecipeSteps.Add (RecipeStep.FromBundle ((Bundle)stepBundle));
				}
			}
			return recipe;
		}
	}
}

