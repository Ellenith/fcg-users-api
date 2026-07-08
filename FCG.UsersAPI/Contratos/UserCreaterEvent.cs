// ============================================================
// UserCreatedEvent.cs — Contrato do evento de usuário criado
//
// Publicado pelo UsersAPI após cadastro bem-sucedido.
// Consumido pelo NotificationsAPI para enviar boas-vindas.
//
// IMPORTANTE: todos os microsserviços que consomem este evento
// devem ter uma cópia idêntica deste contrato.
// ============================================================

namespace FCG.Contratos;

public record UserCreatedEvent(
    Guid     UsuarioId,
    string   Nome,
    string   Email,
    string   Role,
    DateTime CriadoEm);