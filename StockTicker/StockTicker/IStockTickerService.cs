using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;

namespace StockTicker
{
    [ServiceContract]
    public interface IStockTickerService
    {
        MarketState MarketState { get; }

        [OperationContract]
        IEnumerable<Stock> GetAllStocks();

        [OperationContract]
        Task OpenMarket();

        [OperationContract]
        Task CloseMarket();
    }
}