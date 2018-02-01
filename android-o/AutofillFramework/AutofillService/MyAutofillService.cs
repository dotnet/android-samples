using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Service.Autofill;
using Android.Util;
using AutofillService;
using AutofillService.Datasource;
using Java.Lang;
using Resource = AutofillService.Resource;

namespace AutofillFramework
{
    [Service(Label = "Multi-Dataset Autofill Service", Permission = Manifest.Permission.BindAutofillService)]
    [IntentFilter(new[] {"android.service.autofill.AutofillService"})]
    [MetaData("android.autofill", Resource = "@xml/multidataset_service")]
    [Register("com.xamarin.AutofillFramework.multidatasetservice.MyAutofillService")]
    public class MyAutofillService : Android.Service.Autofill.AutofillService
    {
        public override void OnFillRequest(FillRequest request, CancellationSignal cancellationSignal,
            FillCallback callback)
        {
            var structure = request.FillContexts[request.FillContexts.Count - 1].Structure;
            var packageName = structure.ActivityComponent.PackageName;
            if (!SharedPrefsPackageVerificationRepository.GetInstance()
                .PutPackageSignatures(ApplicationContext, packageName))
            {
                callback.OnFailure(
                    ApplicationContext.GetString(AutofillService.Resource.String.invalid_package_signature));
                return;
            }

            var data = request.ClientState;
            if (CommonUtil.VERBOSE)
            {
                Log.Verbose(CommonUtil.TAG, "onFillRequest(): data=" + CommonUtil.BundleToString(data));
                CommonUtil.DumpStructure(structure);
            }

            cancellationSignal.SetOnCancelListener(new CancelListener());

            // Parse AutoFill data in Activity
            var parser = new StructureParser(ApplicationContext, structure);
            // TODO: try / catch on other places (onSave, auth activity, etc...)
            try
            {
                parser.ParseForFill();
            }
            catch (SecurityException e)
            {
                // TODO: handle cases where DAL didn't pass by showing a custom UI asking the user
                // to confirm the mapping. Might require subclassing SecurityException.
                Log.Warn(CommonUtil.TAG, "Security exception handling " + request, e);
                callback.OnFailure(e.Message);
                return;
            }

            var autofillFields = parser.GetAutofillFields();
            var responseBuilder = new FillResponse.Builder();
            // Check user's settings for authenticating Responses and Datasets.
            var responseAuth = MyPreferences.GetInstance(this).IsResponseAuth();
            var autofillIds = autofillFields.GetAutofillIds();
            if (responseAuth && autofillIds.Length != 0)
            {
                // If the entire Autofill Response is authenticated, AuthActivity is used
                // to generate Response.
                var sender = AuthActivity.GetAuthIntentSenderForResponse(this);
                var presentation = AutofillHelper.NewRemoteViews(PackageName,
                    GetString(Resource.String.autofill_sign_in_prompt), Resource.Drawable.ic_lock_black_24dp);
                responseBuilder.SetAuthentication(autofillIds, sender, presentation);
                callback.OnSuccess(responseBuilder.Build());
            }
            else
            {
                var datasetAuth = MyPreferences.GetInstance(this).IsDatasetAuth();
                var clientFormDataMap = SharedPrefsAutofillRepository.GetInstance().GetFilledAutofillFieldCollection(
                    this, autofillFields.GetFocusedHints(), autofillFields.GetAllHints());
                var response = AutofillHelper.NewResponse(this, datasetAuth, autofillFields, clientFormDataMap);
                callback.OnSuccess(response);
            }
        }

        public class CancelListener : Java.Lang.Object, CancellationSignal.IOnCancelListener
        {
            public void OnCancel()
            {
                Log.Warn(CommonUtil.TAG, "Cancel autofill not implemented in this sample.");
            }
        }


        public override void OnSaveRequest(SaveRequest request, SaveCallback callback)
        {
            var context = request.FillContexts;
            var structure = context[context.Count - 1].Structure;
            var packageName = structure.ActivityComponent.PackageName;
            if (!SharedPrefsPackageVerificationRepository.GetInstance()
                .PutPackageSignatures(ApplicationContext, packageName))
            {
                callback.OnFailure(
                    ApplicationContext.GetString(Resource.String.invalid_package_signature));
                return;
            }

            var data = request.ClientState;
            if (CommonUtil.VERBOSE)
            {
                Log.Verbose(CommonUtil.TAG, "onSaveRequest(): data=" + CommonUtil.BundleToString(data));
                CommonUtil.DumpStructure(structure);
            }

            var parser = new StructureParser(ApplicationContext, structure);
            parser.ParseForSave();
            var filledAutofillFieldCollection = parser.GetClientFormData();
            SharedPrefsAutofillRepository.GetInstance()
                .SaveFilledAutofillFieldCollection(this, filledAutofillFieldCollection);
        }

        public override void OnConnected()
        {
            Log.Debug(CommonUtil.TAG, "OnConnected");
        }

        public override void OnDisconnected()
        {
            Log.Debug(CommonUtil.TAG, "OnDisconnected");
        }
    }
}