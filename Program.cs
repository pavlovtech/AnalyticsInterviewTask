namespace AnalyticsInterviewTask;

// Tasks:
// 1. Review the code below & explain what it does
// 2. Identify and explain issues, propose improvements
// 3. Agree on few improvements with interviewers, implement those

public class User
{
    public string Name;
    public double CashBalance;
    public double CreditCardBalance;

    public User(string name, double cashBalance, double creditCardBalance)
    {
        Name = name;
        CashBalance = cashBalance;
        CreditCardBalance = creditCardBalance;
    }

    public override string ToString() =>
        $"Name: {Name}; CashBalance: {CashBalance:N2}; CreditCardBalance: {CreditCardBalance:N2}";
}

public class Product
{
    public string Name;
    public double Price;

    public Product(string name, double price)
    {
        Name = name;
        Price = price;
    }

    public override string ToString() =>
        $"Name: {Name}; Price: {Price:N2}";
}

public abstract class OrderProcessorBase
{
    protected abstract void ProcessPayment(User user, IEnumerable<Product> items);

    protected abstract Task GenerateReceiptAsync(User user, IEnumerable<Product> items);

    public Task ProcessAsync(User user, IEnumerable<Product> items)
    {
        try
        {
            Console.WriteLine($"{GetType().Name}: Processing Payment");
            ProcessPayment(user, items);

            Console.WriteLine($"{GetType().Name}: Generating Receipt");
            return GenerateReceiptAsync(user, items);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{GetType().Name}: Error processing order: {e.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine($"{GetType().Name}: Order Processing Finished");
        }
    }
}

public class CashOrderProcessor : OrderProcessorBase
{
    protected override void ProcessPayment(User user, IEnumerable<Product> items)
    {
        var sum = items.Sum(p => p.Price);
        if (user.CashBalance < sum)
            throw new ApplicationException("Insufficient Cash funds");

        user.CashBalance -= sum;
    }

    protected override Task GenerateReceiptAsync(User user, IEnumerable<Product> items)
    {
        Console.WriteLine($"Receipt for {user.Name}:\n{string.Join("\n", items)}\n");
        return Task.CompletedTask;
    }
}

public class CreditCardOrderProcessor : OrderProcessorBase
{
    protected override void ProcessPayment(User user, IEnumerable<Product> items)
    {
        user.CreditCardBalance -= items.Sum(p => p.Price);
    }

    protected override async Task GenerateReceiptAsync(User user, IEnumerable<Product> items)
    {
        await Task.Delay(1000);
        ////throw new ApplicationException("Error generating receipt");
    }
}

public static class Program
{
    public static IEnumerable<Product> GetCartProducts()
    {
        yield return new("Laptop", 800);
        ////throw new ApplicationException("Product is out of stock");
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
