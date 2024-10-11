namespace Shared.Exceptions
{
    public class StockNotSyncedEcpetion  : Exception
    {
        public StockNotSyncedEcpetion(string ticker) : base($"Stock with ticker: \"{ticker}\" was not syced in the database")
        {
        }
    }
}
