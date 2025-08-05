using QuotaChecker;

var scale = Scale.GetScale();
scale.QuotaLimit = 2;
while (true)
{
    if (scale.IsQuotaReached)
    {
        Console.WriteLine($"Processed [{scale.Unit}] units. Quota has been reached!");
        break;
    }
    else
    {
        Console.WriteLine($"Processed [{scale.Unit}] units. Quota is still available.");
    }

    Thread.Sleep(2000);
}