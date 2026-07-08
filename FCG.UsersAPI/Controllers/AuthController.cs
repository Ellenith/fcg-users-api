// ============================================================
// AuthController.cs — Endpoint de autenticação
// POST /auth/login → retorna JWT
// ============================================================

using FCG.UsersAPI.Domain.Entidades;
using FCG.UsersAPI.Domain.Interfaces;
using FCG.UsersAPI.Infraestrutura.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace FCG.UsersAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepositorio  _repositorio;
    private readonly PasswordHasherServico _passwordHasher;
    private readonly JwtServico           _jwtServico;

    public AuthController(
        IUsuarioRepositorio repositorio,
        PasswordHasherServico passwordHasher,
        JwtServico jwtServico)
    {
        _repositorio    = repositorio;
        _passwordHasher = passwordHasher;
        _jwtServico     = jwtServico;
    }

    [HttpPost("login")]
    [EndpointSummary("Autenticar usuário")]
    [EndpointDescription("Retorna token JWT para uso nos endpoints protegidos.")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _repositorio.BuscarPorEmailAsync(request.Email);
        if (usuario is null)
            throw new DomainException("E-mail ou senha inválidos.");

        if (!_passwordHasher.VerificarSenha(request.Senha, usuario.PasswordHash))
            throw new DomainException("E-mail ou senha inválidos.");

        var token = _jwtServico.GerarToken(usuario);

        return Ok(new LoginResponse(token, usuario.Id, usuario.Name, usuario.Email, usuario.Role));
    }
}

public record LoginRequest(string Email, string Senha);
public record LoginResponse(string Token, Guid Id, string Nome, string Email, string Role);