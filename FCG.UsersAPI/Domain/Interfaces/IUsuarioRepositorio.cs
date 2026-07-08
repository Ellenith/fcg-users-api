using FCG.UsersAPI.Domain.Entidades;

namespace FCG.UsersAPI.Domain.Interfaces;

public interface IUsuarioRepositorio
{
    Task<Usuario?>          BuscarPorIdAsync(Guid id);
    Task<Usuario?>          BuscarPorEmailAsync(string email);
    Task<IEnumerable<Usuario>> ListarTodosAsync();
    Task<bool>              EmailExisteAsync(string email);
    Task<bool>              ExisteAlgumUsuarioAsync();
    Task                    AdicionarAsync(Usuario usuario);
    void                    Atualizar(Usuario usuario);
    Task<int>               SalvarAsync();
}