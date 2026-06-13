using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Cli;

public class CreateUserCommand
{
    public static async Task<bool> HandleCommandAsync(string[] args, IServiceProvider serviceProvider)
    {
        if (args.Length == 0 || args[0] != "create-user")
        {
            return false;
        }

        Console.WriteLine("\n=== New User Wizard ===\n");

        Console.Write("Email: ");
        string? email = Console.ReadLine()?.Trim();

        Console.Write("Username: ");
        string? userName = Console.ReadLine()?.Trim();

        Console.Write("Password: ");
        string password = ReadPassword();

        Console.Write("Confirm Password: ");
        string confirmPassword = ReadPassword();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            PrintError("All fields are required!");
            return true;
        }

        if (password != confirmPassword)
        {
            PrintError("The passwords are not identical!");
            return true;
        }

        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        if (await userManager.FindByEmailAsync(email) != null)
        {
            PrintError($"A user with the email '{email}' already exists.");
            return true;
        }

        if (await userManager.FindByNameAsync(userName) != null)
        {
            PrintError($"A user with the name '{userName}' already exists.");
            return true;
        }

        var user = new User
        {
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            Email = email,
            NormalizedEmail = email.ToUpper(),
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            PrintError("Error creating user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"   - {error.Description}");
            }
            return true;
        }

        PrintSuccess("User has been successfully created!");
        PrintInfo($"Email:    {email}");
        PrintInfo($"Username: {userName}");
        PrintInfo($"ID:       {user.Id}\n");

        return true; 
    }

    private static string ReadPassword()
    {
        var password = string.Empty;
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                password += keyInfo.KeyChar;
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] {message}");
        Console.ResetColor();
    }

    private static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[SUCCESS] {message}");
        Console.ResetColor();
    }

    private static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] {message}");
        Console.ResetColor();
    }
}