using System.Security.Cryptography;
using System.Text;

namespace SistemaInventario.Services
{
    public class EncryptPass
    {
        // Método para encriptar una cadena de texto utilizando SHA256
        public string encryptSHA256(string texto)
        {
            // Crear una instancia de SHA256
            using (SHA256 sha256Hash = SHA256.Create()) 
            {
                // Computar el hash a partir del texto
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));

                StringBuilder builder = new StringBuilder();

                // Convertir el resultado a formato hexadecimal
                for (int i = 0; i < bytes.Length; i++)
                {
                    // Formatear el valor como hexadecimal
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString(); // Devolver la cadena encriptada
            }
        }
    }
}
