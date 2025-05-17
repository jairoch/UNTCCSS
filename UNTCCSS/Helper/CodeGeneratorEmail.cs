using System;

namespace UNTCCSS.Helper
{
    public static class CodeGeneratorEmail
    {
        public static string CodigoRcuperacionContraseña()
        {
            const int longitudCodigo = 4;
            const string caracteres = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Random random = new Random();
            char[] codigoAleatorio = new char[longitudCodigo];

            // Generar los caracteres aleatorios en mayúsculas
            for (int i = 0; i < longitudCodigo; i++)
            {
                codigoAleatorio[i] = caracteres[random.Next(caracteres.Length)];
            }

            // Retornar el código generado
            return new string(codigoAleatorio);
        }
    }
}
