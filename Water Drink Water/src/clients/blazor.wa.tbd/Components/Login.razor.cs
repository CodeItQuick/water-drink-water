using blazor.wa.tbd.Services;
using Microsoft.AspNetCore.Components;

namespace blazor.wa.tbd.Components;

public partial class Login
{
    private readonly LoginFormData formData = new();
    [CascadingParameter] private ConsumptionStateProvider ConsumptionState { get; set; }
    [Inject] public UserService UserService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public AuthService AuthService { get; set; }

    public bool ShowAuthError { get; set; }
    public string Error { get; set; }

    public async Task ExecuteLogin()
    {
        ShowAuthError = false;

        var result = await AuthService.Authenticate(formData.Email, formData.Password);

        if (!result)
        {
            Error = "Invalid email or password";
            ShowAuthError = true;
        }
        else
        {
            await ConsumptionState.RefreshContext();
            NavigationManager.NavigateTo("/log");
        }
    }

    public class LoginFormData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}