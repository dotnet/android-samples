using Android.App.Assist;
using Android.Content;
using Android.Util;
using Android.Views.Autofill;
using AutofillService.Datasource;
using AutofillService.Model;
using Java.Lang;

namespace AutofillService
{
    /**
     * Parser for an AssistStructure object. This is invoked when the Autofill Service receives an
     * AssistStructure from the client Activity, representing its View hierarchy. In this sample, it
     * parses the hierarchy and collects autofill metadata from {@link ViewNode}s along the way.
     */
    public class StructureParser
    {
        private AutofillFieldMetadataCollection mAutofillFields = new AutofillFieldMetadataCollection();
        private Context mContext;
        private AssistStructure mStructure;
        private FilledAutofillFieldCollection mFilledAutofillFieldCollection;

        public StructureParser(Context context, AssistStructure structure)
        {
            mContext = context;
            mStructure = structure;
        }

        public void ParseForFill()
        {
            Parse(true);
        }

        public void ParseForSave()
        {
            Parse(false);
        }

        /**
         * Traverse AssistStructure and add ViewNode metadata to a flat list.
         */
        private void Parse(bool forFill)
        {
            if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "Parsing structure for " + mStructure.ActivityComponent);
            int nodes = mStructure.WindowNodeCount;
            mFilledAutofillFieldCollection = new FilledAutofillFieldCollection();
            var webDomain = new StringBuilder();
            for (int i = 0; i < nodes; i++)
            {
                var node = mStructure.GetWindowNodeAt(i);
                var view = node.RootViewNode;
                ParseLocked(forFill, view, webDomain);
            }

            if (webDomain.Length() > 0)
            {
                var packageName = mStructure.ActivityComponent.PackageName;
                var valid = SharedPrefsDigitalAssetLinksRepository.GetInstance()
                    .IsValid(mContext, webDomain.ToString(), packageName);
                if (!valid)
                {
                    throw new SecurityException(mContext.GetString(Resource.String.invalid_link_association, webDomain,
                        packageName));
                }

                if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "Domain " + webDomain + " is valid for " + packageName);
            }
            else
            {
                if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "no web domain");
            }
        }

        private void ParseLocked(bool forFill, AssistStructure.ViewNode viewNode, StringBuilder validWebDomain)
        {
            var webDomain = viewNode.WebDomain;
            if (webDomain != null)
            {
                if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "child web domain: " + webDomain);
                if (validWebDomain.Length() > 0)
                {
                    if (webDomain != validWebDomain.ToString())
                    {
                        throw new SecurityException("Found multiple web domains: valid= "
                                                    + validWebDomain + ", child=" + webDomain);
                    }
                }
                else
                {
                    validWebDomain.Append(webDomain);
                }
            }

            if (viewNode.GetAutofillHints() != null)
            {
                var filteredHints = AutofillHints.FilterForSupportedHints(
                    viewNode.GetAutofillHints());
                if (filteredHints != null && filteredHints.Length > 0)
                {
                    if (forFill)
                    {
                        mAutofillFields.Add(new AutofillFieldMetadata(viewNode));
                    }
                    else
                    {
                        var filledAutofillField = new FilledAutofillField(viewNode.GetAutofillHints());
                        AutofillValue autofillValue = viewNode.AutofillValue;
                        if (autofillValue.IsText)
                        {
                            // Using toString of AutofillValue.getTextValue in order to save it to
                            // SharedPreferences.
                            filledAutofillField.SetTextValue(autofillValue.TextValue);
                        }
                        else if (autofillValue.IsDate)
                        {
                            filledAutofillField.SetDateValue(autofillValue.DateValue);
                        }
                        else if (autofillValue.IsList)
                        {
                            filledAutofillField.SetListValue(viewNode.GetAutofillOptions(),
                                autofillValue.ListValue);
                        }

                        mFilledAutofillFieldCollection.Add(filledAutofillField);
                    }
                }
            }

            int childrenSize = viewNode.ChildCount;
            if (childrenSize > 0)
            {
                for (int i = 0; i < childrenSize; i++)
                {
                    ParseLocked(forFill, viewNode.GetChildAt(i), validWebDomain);
                }
            }
        }

        public AutofillFieldMetadataCollection GetAutofillFields()
        {
            return mAutofillFields;
        }

        public FilledAutofillFieldCollection GetClientFormData()
        {
            return mFilledAutofillFieldCollection;
        }
    }
}