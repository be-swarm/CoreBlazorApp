using BeSwarm.CoreBlazorApp.Components;

using Microsoft.AspNetCore.Components;


namespace Beswarm.coremauiap
{
    public partial class LoginButton : IDisposable
    {
        [CascadingParameter] BeSwarmWebApiSession Session { get; set; } = default!;


        protected override async Task OnAfterRenderAsync(bool FirstTime)
        {
            if (FirstTime)
            {
                if (Session.Platform != Platforms.Maui)
                    throw new Exception("<LogginButton> is not supported by this plaform. ");
                Session.StateChanged += async (ChangeEvents e) => await InvokeAsync(StateHasChanged);
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(FirstTime);
        }

        void IDisposable.Dispose()
        {
            Session.StateChanged -= async (ChangeEvents e) => await InvokeAsync(StateHasChanged);
        }
        private async Task Login()
        {
            await AuthClient.LoginAsync(Session);
        }
        private async Task Logout()
        {
            await Session.Logout();
        }

    }
}
