# EliteDangerous-UDP-Dispatcher
Small service to dispatch incoming UDP packets from Elite: Dangerous to multiple registered subscribers

## Current status
- Webserver is for show, but will be used to update the registered dispatchers/subscribers
- Dispatchers for TCP, UDP, Webhook/Http POST and Console

## How to run

- Requires .NET Core 2.1 SDK

1. `git clone https://github.com/FuelRats/EliteDangerous-UDP-Dispatcher.git`
2. `cd EliteDangerous-UDP-Dispatcher`
3. `dotnet build`
4. `cd ED.UdpListener`
5. `dotnet run`

And it should fire up a console, that outputs the currently loaded modules (found in `configuration.json`)

## Coming Features

Ability to push to these types of clients:
- WebSocket
- File

Filter out types of events to different subscribers:
- Journal
- Status
- Shipyard
- Outfitting
- Market
