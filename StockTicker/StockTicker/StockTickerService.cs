using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using StockTicker.Hubs;

namespace StockTicker
{
    public class StockTickerService : IStockTickerService
    {
        private readonly static Lazy<StockTickerService> _instance = new Lazy<StockTickerService>
            (() => new StockTickerService(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));

        private readonly SemaphoreSlim _marketStateLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _updateStockPricesLock = new SemaphoreSlim(1, 1);

        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();

        // Stock can go up or down by a percentage of this factor on each change
        private readonly double _rangePercent = 0.002;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private Timer _timer;
        private volatile bool _updatingStockPrices;
        private volatile MarketState _marketState;

        private IHubConnectionContext<dynamic> Clients { get; set; }

        public MarketState MarketState
        {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public static StockTickerService Instance 
        { 
            get { return _instance.Value; } 
        }

        public StockTickerService()
        {
  
        }

        public StockTickerService(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            LoadDefaultStocks();
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        public async Task OpenMarket()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState != MarketState.Open)
                {
                    // _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

                    MarketState = MarketState.Open;

                    await BroadcastMarketStateChange(MarketState.Open);
                }
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        public async Task CloseMarket()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState == MarketState.Open)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }

                    MarketState = MarketState.Closed;

                    await BroadcastMarketStateChange(MarketState.Closed);
                }
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        private void LoadDefaultStocks()
        {
            _stocks.Clear();

            var stocks = new List<Stock>
            {
                new Stock { Symbol = "MSFT", Price = 107.56m },
                new Stock { Symbol = "AAPL", Price = 215.49m },
                new Stock { Symbol = "GOOG", Price = 1221.16m }
            };

            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));
        }

        private async Task BroadcastMarketStateChange(MarketState marketState)
        {
            switch (marketState)
            {
                case MarketState.Open:
                    await Clients.All.marketOpened();
                    break;
                case MarketState.Closed:
                    await Clients.All.marketClosed();
                    break;
                default:
                    break;
            }
        }
    }

    public enum MarketState
    {
        Closed,
        Open
    }
}