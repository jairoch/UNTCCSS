using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Security.Claims;
using UNTCCSS.Data;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Layout
{
    public partial class AdminPanel : LayoutComponentBase
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ILogger<AdminPanel> Logger { get; set; }
        [Inject] IUsersRepositorio UserService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        private ApplicationUser User;

        private string currentUrl;
        private string Name;
        private string Email;
        private async Task ToggleSidebar()
        {
            if (JSRuntime != null)
            {
                await JSRuntime.InvokeVoidAsync("toggleSidebar");
            }
        }

        private async Task ToggleMoreMenu()
        {
            if (JSRuntime != null)
            {
                await JSRuntime.InvokeVoidAsync("toggleMoreMenu");
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && JSRuntime != null)
            {
                await JSRuntime.InvokeVoidAsync("removeSidebarActiveOnResize");
                StateHasChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
            // Obtener el estado de autenticación
            User = await UserService.GetUserWithProfileAsync();
            if (User != null)
            {
                Name = User.Perfil?.Nombres;
                Email = User.Email;
            }
        }
        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }
        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    } 
}
