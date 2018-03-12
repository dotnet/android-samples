using Android.Content;

namespace AutofillService.datasource
{
	public interface IPackageVerificationDataSource
	{
		/// <summary>
		/// Verifies that the signatures in the passed Context match what is currently in
		/// storage. If there are no current signatures in storage for this packageName, it will store
		/// the signatures from the passed Context.
		/// </summary>
		/// <returns><c>true</c>, if signatures for this packageName are not currently in storage
		/// or if the signatures in the passed Context match what is currently in storage,
		/// <c>false</c> if the signatures in the passed Context do not match what is
		/// currently in storage or if an Exception was thrown while generating the signatures.</returns>
		/// <param name="context">Context.</param>
		/// <param name="packageName">Package name.</param>
		bool PutPackageSignatures(Context context, string packageName);

		/// <summary>
		/// Clears all signature data currently in storage.
		/// </summary>
		/// <returns>The clear.</returns>
		/// <param name="context">Context.</param>
		void Clear(Context context);
	}
}
