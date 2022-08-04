const connection = $.hubConnection("http://localhost:8080");
const stockTickerHubProxy = connection.createHubProxy("StockTickerHub");

function connectionStateChanged(state) {
  var stateConversion = {
    0: "connecting",
    1: "connected",
    2: "reconnecting",
    4: "disconnected",
  };
  console.log(
    "SignalR state changed from: " +
      stateConversion[state.oldState] +
      " to: " +
      stateConversion[state.newState]
  );
}
connection.stateChanged(connectionStateChanged);

console.log(connection.state);

setTimeout(() => console.log(connection.state), 2000);

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
