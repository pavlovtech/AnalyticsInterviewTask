﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sandbox;

// Tasks
// 1. Review code & explain what it does
// 2. Identify issues
// 3. Propose improvements
// 4. Agree an improvements with interviewer and implement those

public class User(string name, double cashBalance, double creditCardBalance)
{
    public string Name = name;
    public double CashBalance = cashBalance;
    public double CreditCardBalance = creditCardBalance;

    public override string ToString() =>
        $"Name: {Name}; CashBalance: {CashBalance:N2}; CreditCardBalance: {CreditCardBalance:N2}";
}

public class Product(string name, double price)
{
    public string Name = name;
    public double Price = price;

    public override string ToString() =>
        $"Name: {Name}; Price: {Price:N2}";
}

public abstract class OrderProcessorBase
{
    protected abstract void ProcessPayment(User user, IEnumerable<Product> items);

    protected abstract Task GerenateReceiptAsync(User user, IEnumerable<Product> items);

    public Task ProcessAsync(User user, IEnumerable<Product> items)
    {
        try
        {
            Console.WriteLine($"{GetType().Name}: Processing payment");
            ProcessPayment(user, items);

            Console.WriteLine($"{GetType().Name}: Generating Receipt");
            return GerenateReceiptAsync(user, items);
        }
        catch (Exteption e)
        {
            Console.WriteLine($"{GetType().Name}: Error processing order: {e.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine($"{GetType().Name}: Order Processed");
        }
    }
}

public class CashOrderProcessor : OrderProcessorBase
{
    protected override void ProcessPayment(User user, IEnumerable<Product> items)
    {
        var sum = items.Sum(p => p.Price);
        if (user.CashBalance < sum) throw new ApplicationException("Insufficient Cash funds");

        user.CashBalance -= sum;
    }

    protected override async Task GerenateReceiptAsync(User user, IEnumerable<Product> items)
    {
        Console.WriteLine($"Receipt for {user.Name}:\n{string.Join("\n", items)}\n");
    }
}

public class CreditCardOrderProcessor : OrderProcessorBase
{
    protected override void ProcessPayment(User user, IEnumerable<Product> items)
    {
        user.CreditCardBalance -= items.Sum(p => p.Price);
    }

    protected override async Task GerenateReceiptAsync(User user, IEnumerable<Product> items)
    {
        await Task.Delay(1000);
        throw new ApplicationException("Error generating receipt");
    }
}

public static class Program
{
    public static IEnumerable<Product> GetCartProducts()
    {
        yield return new("Laptop", 800);
        throw new ApplicationException("Product is out of stock");
        yield return new("Smartphone", 700);
    }

    public static async Task Main()
    {
        Console.WriteLine("Starting Application");
        User user = new("John Smith", 1000, 1000);
        IEnumerable<Product> items = GetCartProducts();

        Console.WriteLine($"Processing order for [{user}] using Cash");
        await new CashOrderProcessor().ProcessAsync(user, items);

        Console.WriteLine($"Processing order for [{user}] using Credit Card");
        await new CreditCardOrderProcessor().ProcessAsync(user, items);
    }
}