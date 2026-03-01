using System;

namespace CreateSHA256HASH
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Utility program to hash your password to be used in the appsettings.json file of RetroVaultWebApp.");
            Console.WriteLine("Enter the password you want to hash:");
            var password = Console.ReadLine();

            // Hash supplied password with SHA256 when configured to do so
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                Console.WriteLine("SHA256 hash of the password: " + Convert.ToBase64String(hash));
            }
            Console.WriteLine("Hit enter when done...");
            Console.ReadLine();
        }
    }
}