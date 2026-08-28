using Dominio.Entities;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Data.Repositories;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioDTO> CreateAsync(UsuarioCreateDTO dto)
        {
            var existente = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existente is not null)
                throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");

            // isAdmin siempre en false, nunca se asigna desde DTO publico
            var usuario = new Usuario(
                nombre: dto.Nombre,
                apellido: dto.Apellido,
                email: dto.Email,
                password: dto.Clave,
                dni: dto.DNI,
                fechaNacimiento: dto.FechaNacimiento,
                telefono: dto.Telefono,
                isAdmin: false);

            await _usuarioRepository.AddAsync(usuario);
            return ToDto(usuario);
        }

        public async Task<UsuarioDTO?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            return usuario is null ? null : ToDto(usuario);
        }

        public async Task<List<UsuarioDTO>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(ToDto).ToList();
        }

        public async Task<bool> UpdateAsync(UsuarioUpdateDTO dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(dto.Id);
            if (usuario is null)
                return false;

            usuario.Nombre = dto.Nombre;
            usuario.Apellido = dto.Apellido;
            usuario.Telefono = dto.Telefono;

            if (!string.IsNullOrWhiteSpace(dto.Clave))
                usuario.SetPassword(dto.Clave); // reusa la validación + regenera salt/hash

            return await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _usuarioRepository.DeleteAsync(id);
        }

        private static UsuarioDTO ToDto(Usuario usuario)
        {
            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                DNI = usuario.DNI,
                FechaNacimiento = usuario.FechaNacimiento,
                FechaAlta = usuario.FechaAlta,
                Telefono = usuario.Telefono,
                IsAdmin = usuario.IsAdmin
            };
        }
    }
}
