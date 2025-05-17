using Blazored.Modal.Services;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using UNTCCSS.ModelsDto;
using UNTCCSS.Data;
using UNTCCSS.Repositorios.IRepositorios;
using UNTCCSS.Repositorios;

namespace UNTCCSS.Components.Modal
{
    public partial class ConfiguracionRoles : ComponentBase
    {
        [Inject] IUsersRepositorio UsersService { get; set; }
        [Inject] IAutenticacionRepositorio AuthService { get; set; }
        [Parameter] public UsuariosDto User { get; set; } = new UsuariosDto();
        [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;

        private string selectedRole = "";
        private List<ApplicationRole> ListaRoles = new();
        private string MensajeAdd = "";
        private string MensajeClear = "";
        private AuthData authData = new();
        protected override async Task OnInitializedAsync()
        {
            authData = await AuthService.ObtenerDatosAutenticacionAsync();
            await ObtenerRoles();
        }
        private async Task Close()
        {
            await BlazoredModal.CloseAsync(ModalResult.Ok(true));
        }
        private async Task ObtenerRoles()
        {
            if(UsersService != null)
            {
                ListaRoles = await UsersService.ObtenerRoles();
            }
        }

        private async Task RemoveRole(ApplicationRole role)
        {
            if (User.Roles.Count == 1)
            {
                MensajeClear = "⛔ No se puede eliminar el último rol. El usuario debe tener al menos un rol asignado.";
                StateHasChanged();
                await Task.Delay(4000); //4 Segundos
                MensajeClear = "";
                StateHasChanged();
                return;
            }

            if (UsersService != null)
            {
                bool response = await UsersService.RemoveRoleUser(role.Id, User.Id);

                if (response)
                {
                    User.Roles.Remove(role);
                    StateHasChanged();
                }
            }
        }

        private async Task AddRole()
        {
            if (UsersService != null && !string.IsNullOrEmpty(selectedRole))
            {
                var role = ListaRoles.FirstOrDefault(r => r.Id == selectedRole);

                if (User.Roles.Any(r => r.Id == role.Id))
                {
                    MensajeAdd = "⚠️ Este usuario ya posee el rol seleccionado. No es necesario volver a asignarlo.";
                    StateHasChanged();
                    await Task.Delay(4000);//4Segundos
                    MensajeAdd = "";
                    StateHasChanged();
                    return;
                }

                if (role != null)
                {
                    bool response = await UsersService.AddRoleUser(role.Id, User.Id);

                    if (response)
                    {
                        User.Roles.Add(role); 
                        selectedRole = "";
                        MensajeAdd = "";
                        StateHasChanged();
                    }
                }
            }
        }
    }
}
