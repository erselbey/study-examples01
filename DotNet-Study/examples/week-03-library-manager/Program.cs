using System;
using System.Collections.Generic;

namespace DotNetStudy.Examples.Week03;

// Domain katmanı
public abstract class LibraryItem
{
    protected LibraryItem(Guid id, string title, string author)
    {
        Id = id;
        Title = title;
        Author = author;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Author { get; }
    public virtual string Describe() => $"{Title} - {Author}";
}

public interface IBorrowable
{
    Guid Id { get; }
    int LoanPeriodInDays { get; }
    bool IsBorrowed { get; }
    void Borrow();
    void Return();
}

public sealed class Book : LibraryItem, IBorrowable
{
    public Book(Guid id, string title, string author, int loanPeriodInDays)
        : base(id, title, author)
    {
        LoanPeriodInDays = loanPeriodInDays;
    }

    public int LoanPeriodInDays { get; }
    public bool IsBorrowed { get; private set; }

    public void Borrow()
    {
        if (IsBorrowed)
        {
            throw new InvalidOperationException("Kitap zaten ödünç verilmiş durumda.");
        }

        IsBorrowed = true;
    }

    public void Return() => IsBorrowed = false;
}

public sealed class Magazine : LibraryItem
{
    public Magazine(Guid id, string title, string author, string issue)
        : base(id, title, author)
    {
        Issue = issue;
    }

    public string Issue { get; }

    public override string Describe() => $"{base.Describe()} | Sayı: {Issue}";
}

public sealed class LoanService
{
    private readonly Dictionary<Guid, DateTime> _activeLoans = new();

    public void Borrow(IBorrowable item, string memberName)
    {
        if (item.IsBorrowed)
        {
            throw new LoanLimitExceededException($"'{item.Id}' numaralı eser şu anda ödünç alınamaz.");
        }

        item.Borrow();
        _activeLoans[item.Id] = DateTime.UtcNow.AddDays(item.LoanPeriodInDays);
        Console.WriteLine($"Üye: {memberName} | Teslim tarihi: {_activeLoans[item.Id]:d}");
    }

    public void Return(IBorrowable item)
    {
        item.Return();
        _activeLoans.Remove(item.Id);
        Console.WriteLine("İade işlemi tamamlandı.");
    }

    public void PrintActiveLoans()
    {
        Console.WriteLine("Aktif ödünç kayıtları:");
        foreach (var entry in _activeLoans)
        {
            Console.WriteLine($"Item Id: {entry.Key} => Due: {entry.Value:d}");
        }
    }
}

public sealed class LoanLimitExceededException : Exception
{
    public LoanLimitExceededException(string message) : base(message)
    {
    }
}

// Uygulama katmanı
public static class Program
{
    public static void Main()
    {
        var items = SeedItems();
        var loanService = new LoanService();

        Console.WriteLine("Kütüphane Envanteri");
        foreach (var item in items)
        {
            Console.WriteLine(item.Describe());
        }

        // Örnek senaryo: kitap ödünç alıp iade etmek
        var borrowableItem = (IBorrowable)items[0];
        loanService.Borrow(borrowableItem, memberName: "Ayşe");
        loanService.PrintActiveLoans();

        loanService.Return(borrowableItem);
        loanService.PrintActiveLoans();
    }

    private static List<LibraryItem> SeedItems() =>
        new()
        {
            new Book(Guid.NewGuid(), "Temiz Kod", "Robert C. Martin", loanPeriodInDays: 14),
            new Book(Guid.NewGuid(), "CLR via C#", "Jeffrey Richter", loanPeriodInDays: 21),
            new Magazine(Guid.NewGuid(), "Dotnet Bülteni", "Community", issue: "2024/09")
        };
}
