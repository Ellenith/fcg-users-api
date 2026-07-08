using FCG.UsersAPI.Domain.Entidades;
using FCG.UsersAPI.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG.UsersAPI.Infraestrutura.Persistencia;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly UsersDbContext _context;

    public UsuarioRepositorio(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> BuscarPorIdAsync(Guid id)
        => await _context.Usuarios.FindAsync(id);

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
        => await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

    public async Task<IEnumerable<Usuario>> ListarTodosAsync()
        => await _context.Usuarios.AsNoTracking().ToListAsync();

    public async Task<bool> EmailExisteAsync(string email)
        => await _context.Usuarios
            .AnyAsync(u => u.Email == email.ToLowerInvariant());

    public async Task<bool> ExisteAlgumUsuarioAsync()
        => await _context.Usuarios.AnyAsync();

    public async Task AdicionarAsync(Usuario usuario)
        => await _context.Usuarios.AddAsync(usuario);

    public void Atualizar(Usuario usuario)
        => _context.Usuarios.Update(usuario);

    public async Task<int> SalvarAsync()
        => await _context.SaveChangesAsync();
}