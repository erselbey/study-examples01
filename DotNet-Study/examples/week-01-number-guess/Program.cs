using System;

namespace DotNetStudy.Examples.Week01;

public static class Program
{
    // Giriş noktası: dotnet run komutu çalıştırıldığında bu metot çağrılır.
    public static void Main()
    {
        const int maxAttempts = 5;
        var secretNumber = new Random().Next(1, 21); // 1-20 arası rastgele sayı
        var attemptsLeft = maxAttempts;

        Console.WriteLine("Sayı Tahmin Oyununa Hoş Geldin!");
        Console.WriteLine("1 ile 20 arasında rastgele seçilen sayıyı bulmak için 5 hakkın var.");

        while (attemptsLeft > 0)
        {
            Console.WriteLine();
            Console.Write($"Tahminini gir (Kalan hak: {attemptsLeft}): ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out var guess))
            {
                Console.WriteLine("Sayı dışında bir şey girdin. Lütfen geçerli bir sayı yaz.");
                continue;
            }

            attemptsLeft--;

            if (guess == secretNumber)
            {
                Console.WriteLine("Tebrikler! Doğru sayıyı buldun 🎉");
                return;
            }

            // Kullanıcıya rehberlik edecek basit ipuçları
            if (guess < secretNumber)
            {
                Console.WriteLine("Daha büyük bir sayı dene.");
            }
            else
            {
                Console.WriteLine("Daha küçük bir sayı dene.");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Oyunun sonu! Doğru sayı {secretNumber} olacaktı. Bir sonraki denemede görüşürüz.");
    }
}
