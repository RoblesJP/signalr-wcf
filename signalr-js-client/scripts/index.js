const connection = $.hubConnection("http://localhost:8080");
connection.logging = true;
const stockTickerHubProxy = connection.createHubProxy("StockTickerHub");

console.log(stockTickerHubProxy);

connection
  .start()
  .done(() => {
    stockTickerHubProxy
      .invoke("Test")
      .done(() => console.log("Server Method Test() called..."))
      .fail((err) => console.log(err));
  })
  .fail((err) => console.log(err));

stockTickerHubProxy.on("test", () => {
  console.log("Hello World!");
});
