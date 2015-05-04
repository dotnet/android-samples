/*
 * Copyright (C) 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
using Android.Print;
using Android.Content.Res;
using Java.Lang;
using Android.Util;
using Android.Print.Pdf;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace MonoDroid.ApiDemo
{
	/**
 	* This class demonstrates how to implement custom printing support.
 	* 
 	* This activity shows the list of the MotoGP champions by year and
 	* brand. The print option in the overflow menu allows the user to
 	* print the content. The list list of items is laid out to such that
 	* it fits the options selected by the user from the UI such as page
 	* size. Hence, for different page sizes the printed content will have
 	* different page count.
 	* 
 	* This sample demonstrates how to completely implement a {@link
 	* PrintDocumentAdapter} in which:
 	* 
 	* <li>Layout based on the selected print options is performed.</li>
 	* <li>Layout work is performed only if print options change would change the content.</li>
 	* <li>Layout result is properly reported.</li>
 	* <li>Only requested pages are written.</li>
 	* <li>Write result is properly reported.</li>
 	* <li>Both Layout and write respond to cancellation.</li>
 	* <li>Layout and render of views is demonstrated.</li>
 	*/
	[Activity (Label = "@string/print_custom_content", Name = "monodroid.apidemo.PrintCustomContent")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class PrintCustomContent : ListActivity
	{
		static readonly int MILS_IN_INCH = 1000;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ListAdapter = new MotoGpStatAdapter (LoadMotoGpStats (), LayoutInflater);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.print_custom_content, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_print) {
				Print ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		void Print ()
		{
			var printManager = (PrintManager)GetSystemService (Context.PrintService);

			printManager.Print ("MotoGP stats", new MyPrintDocumentAdapter (this), null);
		}

		class MyPrintDocumentAdapter : PrintDocumentAdapter
		{
			int mRenderPageWidth;
			int mRenderPageHeight;
			PrintAttributes mPrintAttributes;
			PrintDocumentInfo mDocumentInfo;
			Context mPrintContext;
			PrintCustomContent pcc;

			public MyPrintDocumentAdapter (PrintCustomContent self)
			{
				pcc = self;
			}

			public override void OnLayout (PrintAttributes oldAttributes, PrintAttributes newAttributes, 
			                               CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
			{
				// If we are already cancelled, don't do any work.
				if (cancellationSignal.IsCanceled) {
					callback.OnLayoutCancelled ();
					return;
				}

				// Now we determined if the print attributes changed in a way that
				// would change the layout and if so we will do a layout pass.
				bool layoutNeeded = false;

				int density = System.Math.Max (newAttributes.GetResolution ().HorizontalDpi,
					              newAttributes.GetResolution ().VerticalDpi);

				// Note that we are using the PrintedPdfDocument class which creates
				// a PDF generating canvas whose size is in points (1/72") not screen
				// pixels. Hence, this canvas is pretty small compared to the screen.
				// The recommended way is to layout the content in the desired size,
				// in this case as large as the printer can do, and set a translation
				// to the PDF canvas to shrink in. Note that PDF is a vector format
				// and you will not lose data during the transformation.

				// The content width is equal to the page width minus the margins times
				// the horizontal printer density. This way we get the maximal number
				// of pixels the printer can put horizontally.
				int marginLeft = (int)(density * (float)newAttributes.MinMargins.LeftMils / MILS_IN_INCH);
				int marginRight = (int)(density * (float)newAttributes.MinMargins.RightMils / MILS_IN_INCH);
				int contentWidth = (int)(density * (float)newAttributes.GetMediaSize ()
					.WidthMils / MILS_IN_INCH) - marginLeft - marginRight;
				if (mRenderPageWidth != contentWidth) {
					mRenderPageWidth = contentWidth;
					layoutNeeded = true;
				}

				// The content height is equal to the page height minus the margins times
				// the vertical printer resolution. This way we get the maximal number
				// of pixels the printer can put vertically.
				int marginTop = (int)(density * (float)newAttributes.MinMargins.TopMils / MILS_IN_INCH);
				int marginBottom = (int)(density * (float)newAttributes.MinMargins.BottomMils / MILS_IN_INCH);
				int contentHeight = (int)(density * (float)newAttributes.GetMediaSize ()
					.HeightMils / MILS_IN_INCH) - marginTop - marginBottom;
				if (mRenderPageHeight != contentHeight) {
					mRenderPageHeight = contentHeight;
					layoutNeeded = true;
				}

				// Create a context for resources at printer density. We will
				// be inflating views to render them and would like them to use
				// resources for a density the printer supports.
				if (mPrintContext == null || mPrintContext.Resources.Configuration.DensityDpi != density) {
					var configuration = new Configuration ();
					configuration.DensityDpi = density;
					mPrintContext = pcc.CreateConfigurationContext (configuration);
					mPrintContext.SetTheme (Android.Resource.Style.ThemeHoloLight);
				}

				// If no layout is needed that we did a layout at least once and
				// the document info is not null, also the second argument is false
				// to notify the system that the content did not change. This is
				// important as if the system has some pages and the content didn't
				// change the system will ask, the application to write them again.
				if (!layoutNeeded) {
					callback.OnLayoutFinished (mDocumentInfo, false);
					return;
				}

				// For demonstration purposes we will do the layout off the main
				// thread but for small content sizes like this one it is OK to do
				// that on the main thread.
				var asyncTask = new MyOnLayoutAsyncTask (this, newAttributes, cancellationSignal, callback);
				asyncTask.ExecuteOnExecutor (AsyncTask.ThreadPoolExecutor, (Java.Lang.Void[])null);
			}

			void MeasureView (View view)
			{
				int widthMeasureSpec = ViewGroup.GetChildMeasureSpec (
					                       View.MeasureSpec.MakeMeasureSpec (mRenderPageWidth,
						                       MeasureSpecMode.Exactly), 0, view.LayoutParameters.Width);

				int heightMeasureSpec = ViewGroup.GetChildMeasureSpec (
					                        View.MeasureSpec.MakeMeasureSpec (mRenderPageHeight,
						                        MeasureSpecMode.Exactly), 0, view.LayoutParameters.Height);
				view.Measure (widthMeasureSpec, heightMeasureSpec);
			}

			PageRange[] ComputeWrittenPageRanges (SparseIntArray writtenPages)
			{
				var pageRanges = new List<PageRange> ();

				int start = -1;
				int end = -1;
				int writtenPageCount = writtenPages.Size ();
				for (int i = 0; i < writtenPageCount; i++) {
					if (start < 0) {
						start = writtenPages.ValueAt (i);
					}
					int oldEnd = end = start;
					while (i < writtenPageCount && (end - oldEnd) <= 1) {
						oldEnd = end;
						end = writtenPages.ValueAt (i);
						i++;
					}
					var pageRange = new PageRange (start, end);
					pageRanges.Add (pageRange);
					start = end = -1;
				}

				return pageRanges.ToArray ();
			}

			bool ContainsPage (PageRange[] pageRanges, int page)
			{
				int pageRangeCount = pageRanges.Length;
				for (int i = 0; i < pageRangeCount; i++) {
					if (pageRanges [i].Start <= page
					    && pageRanges [i].End >= page) {
						return true;
					}
				}
				return false;
			}

			class MyOnLayoutAsyncTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, PrintDocumentInfo>
			{
				MyPrintDocumentAdapter self;
				PrintAttributes newAttributes;
				CancellationSignal cancellationSignal;
				LayoutResultCallback callback;
				// Store the data as we will layout off the main thread.
				List<MotoGpStatItem> items;

				public MyOnLayoutAsyncTask (MyPrintDocumentAdapter self, PrintAttributes newAttributes, 
				                            CancellationSignal cancellationSignal, LayoutResultCallback callback)
				{
					this.self = self;
					this.newAttributes = newAttributes;
					this.cancellationSignal = cancellationSignal;
					this.callback = callback;

					items = ((MotoGpStatAdapter)self.pcc.ListAdapter).CloneItems ();
				}

				protected override void OnPreExecute ()
				{
					// First register for cancellation requests.
					cancellationSignal.CancelEvent += delegate {
						Cancel (true);
					};

					// Stash the attributes as we will need them for rendering.
					self.mPrintAttributes = newAttributes;
				}

				protected override Java.Lang.Object DoInBackground (params Java.Lang.Object[] native_parms)
				{
					try {
						// Create an adapter with the stats and an inflater
						// to load resources for the printer density.
						var adapter = new MotoGpStatAdapter (items,
							              (LayoutInflater)self.mPrintContext.GetSystemService (
								              Context.LayoutInflaterService));

						int currentPage = 0;
						int pageContentHeight = 0;
						int viewType = -1;
						View view = null;
						var dummyParent = new LinearLayout (self.mPrintContext);
						dummyParent.Orientation = Android.Widget.Orientation.Vertical;

						int itemCount = adapter.Count;
						for (int i = 0; i < itemCount; i++) {
							// Be nice and respond to cancellation.
							if (IsCancelled) {
								return null;
							}

							// Get the next view.
							int nextViewType = adapter.GetItemViewType (i);
							if (viewType == nextViewType) {
								view = adapter.GetView (i, view, dummyParent); 
							} else {
								view = adapter.GetView (i, null, dummyParent);
							}
							viewType = nextViewType;

							// Measure the next view
							self.MeasureView (view);

							// Add the height but if the view crosses the page
							// boundary we will put it to the next page.
							pageContentHeight += view.MeasuredHeight;
							if (pageContentHeight > self.mRenderPageHeight) {
								pageContentHeight = view.MeasuredHeight;
								currentPage++;
							}
						}

						// Create a document info describing the result.
						PrintDocumentInfo info = new PrintDocumentInfo
							.Builder ("MotoGP_stats.pdf")
							.SetContentType (PrintContentType.Document)
							.SetPageCount (currentPage + 1)
							.Build ();

						// We completed the layout as a result of print attributes
						// change. Hence, if we are here the content changed for
						// sure which is why we pass true as the second argument.
						callback.OnLayoutFinished (info, true);
						return info;
					} catch (Java.Lang.Exception e) {
						// An unexpected error, report that we failed and
						// one may pass in a human readable localized text
						// for what the error is if known.
						callback.OnLayoutFailed ((string)null);
						throw new RuntimeException (e);
					}
				}

				protected override void OnPostExecute (Java.Lang.Object result)
				{
					// Update the cached info to send it over if the next
					// layout pass does not result in a content change.
					self.mDocumentInfo = (PrintDocumentInfo)result;
				}

				protected override void OnCancelled ()
				{
					// Task was cancelled, report that.
					callback.OnLayoutCancelled ();
				}

				protected override PrintDocumentInfo RunInBackground (params Java.Lang.Void[] @params)
				{
					return null;
				}
			}

			public override void OnWrite (PageRange[] pages, ParcelFileDescriptor destination, 
			                              CancellationSignal cancellationSignal, WriteResultCallback callback)
			{
				// If we are already cancelled, don't do any work.
				if (cancellationSignal.IsCanceled) {
					callback.OnWriteCancelled ();
					return;
				}

				var onWriteAsyncTask = new MyOnWriteAsyncTask (this, pages, destination, cancellationSignal, callback);
				onWriteAsyncTask.ExecuteOnExecutor (AsyncTask.ThreadPoolExecutor, (Java.Lang.Void[])null);
			}

			class MyOnWriteAsyncTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, PrintDocumentInfo>
			{
				SparseIntArray mWrittenPages = new SparseIntArray ();
				PrintedPdfDocument mPdfDocument;
				MyPrintDocumentAdapter self;
				PageRange[] pages;
				ParcelFileDescriptor destination;
				CancellationSignal cancellationSignal;
				WriteResultCallback callback;
				// Store the data as we will layout off the main thread.
				List<MotoGpStatItem> items;

				public MyOnWriteAsyncTask (MyPrintDocumentAdapter self, PageRange[] pages, ParcelFileDescriptor destination, 
				                           CancellationSignal cancellationSignal, WriteResultCallback callback)
				{
					this.self = self;
					this.pages = pages;
					this.destination = destination;
					this.cancellationSignal = cancellationSignal;
					this.callback = callback;

					items = ((MotoGpStatAdapter)self.pcc.ListAdapter).CloneItems ();
					mPdfDocument = new PrintedPdfDocument (self.mPrintContext, self.mPrintAttributes);
				}

				protected override void OnPreExecute ()
				{
					// First register for cancellation requests.
					cancellationSignal.CancelEvent += delegate {
						Cancel (true);
					};
				}

				protected override Java.Lang.Object DoInBackground (params Java.Lang.Object[] native_parms)
				{
					// Go over all the pages and write only the requested ones.
					// Create an adapter with the stats and an inflater
					// to load resources for the printer density.
					var adapter = new MotoGpStatAdapter (items,
						              (LayoutInflater)self.mPrintContext.GetSystemService (
							              Context.LayoutInflaterService));

					int currentPage = -1;
					int pageContentHeight = 0;
					int viewType = -1;
					View view = null;
					Android.Graphics.Pdf.PdfDocument.Page page = null;
					var dummyParent = new LinearLayout (self.mPrintContext);
					dummyParent.Orientation = Android.Widget.Orientation.Vertical;

					// The content is laid out and rendered in screen pixels with
					// the width and height of the paper size times the print
					// density but the PDF canvas size is in points which are 1/72",
					// so we will scale down the content.
					float scale = System.Math.Min (
						              (float)mPdfDocument.PageContentRect.Width () / self.mRenderPageWidth,
						              (float)mPdfDocument.PageContentRect.Height () / self.mRenderPageHeight);

					int itemCount = adapter.Count;
					for (int i = 0; i < itemCount; i++) {
						// Be nice and respond to cancellation.
						if (IsCancelled) {
							return null;
						}

						// Get the next view.
						int nextViewType = adapter.GetItemViewType (i);
						if (viewType == nextViewType) {
							view = adapter.GetView (i, view, dummyParent);
						} else {
							view = adapter.GetView (i, null, dummyParent);
						}
						viewType = nextViewType;

						// Measure the next view
						self.MeasureView (view);

						// Add the height but if the view crosses the page
						// boundary we will put it to the next one.
						pageContentHeight += view.MeasuredHeight;
						if (currentPage < 0 || pageContentHeight > self.mRenderPageHeight) {
							pageContentHeight = view.MeasuredHeight;
							currentPage++;
							// Done with the current page - finish it.
							if (page != null) {
								mPdfDocument.FinishPage (page);
							}
							// If the page is requested, render it.
							if (self.ContainsPage (pages, currentPage)) {
								page = mPdfDocument.StartPage (currentPage);
								page.Canvas.Scale (scale, scale);
								// Keep track which pages are written.
								mWrittenPages.Append (mWrittenPages.Size (), currentPage);
							} else {
								page = null;
							}
						}

						// If the current view is on a requested page, render it.
						if (page != null) {
							// Layout an render the content.
							view.Layout (0, 0, view.MeasuredWidth, view.MeasuredHeight);
							view.Draw (page.Canvas);
							// Move the canvas for the next view.
							page.Canvas.Translate (0, view.Height);
						}
					}

					// Done with the last page.
					if (page != null) {
						mPdfDocument.FinishPage (page);
					}

					// Write the data and return success or failure.
					try {
						var fos = new Java.IO.FileOutputStream (destination.FileDescriptor);
						mPdfDocument.WriteTo (new OutputStreamInvoker (fos));
						// Compute which page ranges were written based on
						// the bookkeeping we maintained.
						var pageRanges = self.ComputeWrittenPageRanges (mWrittenPages);
						callback.OnWriteFinished (pageRanges);
					} catch (IOException) {
						callback.OnWriteFailed ((string)null);
					} finally {
						mPdfDocument.Close ();
					}

					return null;
				}

				protected override void OnCancelled ()
				{
					// Task was cancelled, report that.
					callback.OnWriteCancelled ();
					mPdfDocument.Close ();
				}

				protected override PrintDocumentInfo RunInBackground (params Java.Lang.Void[] @params)
				{
					return null;
				}
			}
		}

		List<MotoGpStatItem> LoadMotoGpStats ()
		{
			string[] years = Resources.GetStringArray (Resource.Array.motogp_years);
			string[] champions = Resources.GetStringArray (Resource.Array.motogp_champions);
			string[] constructors = Resources.GetStringArray (Resource.Array.motogp_constructors);

			var items = new List<MotoGpStatItem> ();

			int itemCount = years.Length;
			for (int i = 0; i < itemCount; i++) {
				var item = new MotoGpStatItem ();
				item.Year = years [i];
				item.Champion = champions [i];
				item.Constructor = constructors [i];
				items.Add (item);
			}

			return items;
		}

		class MotoGpStatItem : Java.Lang.Object
		{
			public string Year { get; set; }

			public string Champion { get; set; }

			public string Constructor { get; set; }
		}

		class MotoGpStatAdapter : BaseAdapter
		{
			List<MotoGpStatItem> mItems;
			LayoutInflater mInflater;

			public MotoGpStatAdapter (List<MotoGpStatItem> items, LayoutInflater inflater)
			{
				mItems = items;
				mInflater = inflater;
			}

			public List<MotoGpStatItem> CloneItems ()
			{
				return new List<MotoGpStatItem> (mItems);
			}

			public override int Count {
				get {
					return mItems.Count;
				}
			}

			public override Java.Lang.Object GetItem (int position)
			{
				return mItems [position];
			}

			public override long GetItemId (int position)
			{
				return position;
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				if (convertView == null) {
					convertView = mInflater.Inflate (Resource.Layout.motogp_stat_item, parent, false);
				}

				var item = (MotoGpStatItem)GetItem (position);

				var yearView = (TextView)convertView.FindViewById (Resource.Id.year);
				yearView.Text = item.Year;

				var championView = (TextView)convertView.FindViewById (Resource.Id.champion);
				championView.Text = item.Champion;

				var constructorView = (TextView)convertView.FindViewById (Resource.Id.constructor);
				constructorView.Text = item.Constructor;

				return convertView;
			}
		}
	}
}

