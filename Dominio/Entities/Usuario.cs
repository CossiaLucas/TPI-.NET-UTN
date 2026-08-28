using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Usuario
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string ClaveHash { get; set; }
        public string Salt { get; private set; }

        public string DNI { get; set; }
        public List<Direccion> Direcciones { get; set; } = new List<Direccion>();

        public List<Favorito> Favorito { get; set; } = new List<Favorito>();
        public List<Venta> Ventas { get; set; } = new List<Venta>();

        public Carrito? Carrito { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Telefono { get; set; }
        public bool IsAdmin { get; set; }

        public Usuario(string nombre, string apellido, string email, string password, string dni, DateTime fechaNacimiento, string telefono, bool isAdmin)
        {
            Nombre = nombre;
            Apellido = apellido;
            SetEmail(email);
            SetPassword(password);
            DNI = dni;
            FechaNacimiento = fechaNacimiento;
            Telefono = telefono;
            IsAdmin = isAdmin;
            FechaAlta = DateTime.Now; // Se establece la fecha de alta al momento de crear el usuario
        }

        private Usuario() { }

        public void SetId(int id)
        {
            if (id < 0)
                throw new ArgumentException("El Id debe ser mayor que 0.", nameof(id));
            Id = id;
        }

        public void SetEmail(string email)
        {
            if (!EsEmailValido(email))
                throw new ArgumentException("El email no tiene un formato válido.", nameof(email));
            Email = email;
        }

        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede ser nula o vacía.", nameof(password));

            if (password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.", nameof(password));

            Salt = GenerateSalt();
            ClaveHash = HashPassword(password, Salt);
        }

        public void SetFechaCreacion(DateTime fechaCreacion)
        {
            if (fechaCreacion == default)
                throw new ArgumentException("La fecha de creación no puede ser nula.", nameof(fechaCreacion));
            FechaAlta = fechaCreacion;
        }

        public void SetIsAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            string hashedInput = HashPassword(password, Salt);
            return ClaveHash == hashedInput;
        }

        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256);
            byte[] hashBytes = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hashBytes);
        }

        private static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
