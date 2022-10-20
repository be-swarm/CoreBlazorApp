using Android.App;
using Android.Content.PM;
using Android.Runtime;

namespace TestMauiBlazor
{
	[Application]
	public class MainApplication : MauiApplication
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership)
			: base(handle, ownership)
		{
		}

		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	}



	[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
				  Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
				  DataScheme = CALLBACK_SCHEME)]
	public class WebAuthenticationCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
	{
		const string CALLBACK_SCHEME = "com.beswarm.testmauiblazor";

	}
}