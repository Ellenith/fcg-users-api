// ============================================================
// UsuarioController.cs — Endpoints de usuários
//
// POST   /usuarios      → cadastro (público)
// GET    /usuarios      → listar todos (Admin)
// GET    /usuarios/{id} → buscar por id (autenticado)
// PUT    /usuarios/{id} → atualizar perfil (autenticado)
// ============================================================

using FCG.Contratos;
using FCG.UsersAPI.Domain.Entidades;
using FCG.UsersAPI.Domain.Interfaces;
using FCG.UsersAPI.Infraestrutura.Servicos;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.UsersAPI.Controllers;

[ApiController]
[Route("usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioRepositorio _repositorio;
    private readonly PasswordHasherServico _passwordHasher;
    private readonly IPublishEndpoint _publishEndpoint;

    public UsuarioController(
        IUsuarioRepositorio repositorio,
        PasswordHasherServico passwordHasher,
        IPublishEndpoint publishEndpoint)
    {
        _repositorio     = repositorio;
        _passwordHasher  = passwordHasher;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    [EndpointSummary("Cadastrar usuário")]
    [EndpointDescription("Primeiro usuário cadastrado vira Admin automaticamente.")]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarUsuarioRequest request)
    {
        if (await _repositorio.EmailExisteAsync(request.Email))
            throw new DomainException("E-mail já cadastrado.");

        var hash  = _passwordHasher.GerarHash(request.Senha);
        var role  = !await _repositorio.ExisteAlgumUsuarioAsync() ? "Admin" : "User";
        var usuario = Usuario.Create(request.Nome, request.Email, request.Senha, hash, role);

        await _repositorio.AdicionarAsync(usuario);
        await _repositorio.SalvarAsync();

        // Publica o evento para o NotificationsAPI
        await _publishEndpoint.Publish(new UserCreatedEvent(
            usuario.Id,
            usuario.Name,
            usuario.Email,
            usuario.Role,
            usuario.CreatedAt));

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = usuario.Id },
            new UsuarioResponse(usuario));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Listar usuários")]
    [EndpointDescription("Requer role Admin.")]
    public async Task<IActionResult> ListarTodos()
    {
        var usuarios = await _repositorio.ListarTodosAsync();
        return Ok(usuarios.Select(u => new UsuarioResponse(u)));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [EndpointSummary("Buscar usuário por Id")]
    [EndpointDescription("Usuário só pode ver o próprio perfil. Admin pode ver qualquer usuário.")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var usuarioLogadoId = ObterUsuarioLogadoId();
        if (!User.IsInRole("Admin") && usuarioLogadoId != id)
            return Forbid();

        var usuario = await _repositorio.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound("Usuário não encontrado.");

        return Ok(new UsuarioResponse(usuario));
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [EndpointSummary("Atualizar perfil")]
    [EndpointDescription("Atualiza nome e senha. Usuário só pode atualizar o próprio perfil.")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request)
    {
        var usuarioLogadoId = ObterUsuarioLogadoId();
        if (!User.IsInRole("Admin") && usuarioLogadoId != id)
            return Forbid();

        var usuario = await _repositorio.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound("Usuário não encontrado.");

        var novoHash = _passwordHasher.GerarHash(request.Senha);
        usuario.UpdateProfile(request.Nome, request.Senha, novoHash);

        _repositorio.Atualizar(usuario);
        await _repositorio.SalvarAsync();

        return Ok(new UsuarioResponse(usuario));
    }

    private Guid ObterUsuarioLogadoId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }
}

// DTOs
public record CadastrarUsuarioRequest(string Nome, string Email, string Senha);
public record AtualizarUsuarioRequest(string Nome, string Senha);
public record UsuarioResponse(Guid Id, string Nome, string Email, string Role, DateTime CriadoEm, string Mensagem)
{
    public UsuarioResponse(Usuario u)
        : this(u.Id, u.Name, u.Email, u.Role, u.CreatedAt.ToLocalTime(),
               u.Role == "Admin" ? "Usuário administrador criado com sucesso!" : "Usuário criado com sucesso!") { }
}