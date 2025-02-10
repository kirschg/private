using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using vizsgaremek.Models;

internal class Program
{
    public static int SaltLength = 64;
    public static Dictionary<string,User> LoggedInUsers = new Dictionary<string,User>();
    public static string GenerateSalt()
    {
        Random random = new Random();
        string karakterek = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string salt = "";
        for (int i = 0; i < SaltLength; i++)
        {
            salt += karakterek[random.Next(karakterek.Length)];
        }
        return salt;
    }

    public static string CreateSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }

    public async static Task SendEmail(string mailAddress, string subject, string body)
    {
        MailMessage mailMessage = new MailMessage();
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
        mailMessage.From = new MailAddress("thro941@gmail.com");
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        smtpClient.Port = 587;
        smtpClient.Credentials = new System.Net.NetworkCredential("thro941@gmail.com", "beequhrajjmiqtaa");
        smtpClient.EnableSsl = true;
        await smtpClient.SendMailAsync(mailMessage);
    }

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}