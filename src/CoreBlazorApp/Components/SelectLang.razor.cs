using BeSwarm.WebApiClient.Models;

using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Components;


public partial class SelectLang : IDisposable
{
    SelectReferentialItem refme = default!;
    [CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
    ReferentialItemLang selected = new();
    List<ReferentialItemLang> langs { get; set; } = new();
    protected override async Task OnAfterRenderAsync(bool FirstTime)
    {
        if (FirstTime)
        {
            Session.EnvironmentHasChanged += async (ChangeEvents e) => await OnRefresh(e);
            await OnRefresh(ChangeEvents.Lang);
        }
    }
    private async Task OnRefresh(ChangeEvents e)
    {
        //if (e == ChangeEvents.Login || e == ChangeEvents.Lang||e==ChangeEvents.Init)  // reload langs
        {
            var res = await Session.SessionWebApi.GetReferentialItems("langs", Session.Lang);
            langs = res.Datas;
            if (res.IsError)
            {   // show error if needed

            }
            StateHasChanged();
            if (refme is { }) await refme.OnRefresh(e);

        }
    }
    void IDisposable.Dispose()
    {
        Session.EnvironmentHasChanged -= async (ChangeEvents e) => await OnRefresh(e);
    }
    private async Task OnSelectedLangChanged(ReferentialItemLang value)
    {
        await Session.SetLang(value.Uid);

    }

}

