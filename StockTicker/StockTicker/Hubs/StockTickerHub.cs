using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace StockTicker.Hubs
{
    [HubName("StockTickerHub")]
    public class StockTickerHub : Hub
    {
        private readonly StockTickerService _stockTickerService;

        public StockTickerHub() : this(StockTickerService.Instance)
        {

        }

        public StockTickerHub(StockTickerService stockTicker)
        {
            _stockTickerService = stockTicker;
        }


        [HubMethodName("Test")]
        public async Task Test()
        {
            await Clients.All.test();
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stockTickerService.GetAllStocks();
        }

        public string GetMarketState()
        {
            return _stockTickerService.MarketState.ToString();
        }

        public async Task OpenMarket()
        {
            await _stockTickerService.OpenMarket();
        }

        public async Task CloseMarket()
        {
            await _stockTickerService.CloseMarket();
        }
    }
}