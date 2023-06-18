# SimRacing.Telemetry.Receiver.F1.23

Note: This is a c# class to receive telemetry data. It is not an application and does not include a user interface. Sample project coming soon.
Note: Work in progress. Further documentation coming.

The F1_23_Telemetry_Receiver class is a UDP packet receiver specifically designed to work with the 2022 Formula 1 data streams. It provides an event-driven architecture, enabling users to subscribe to different kinds of packet data types (like Car Status, Lap Data, Session Data etc.) and handle them according to their specific needs. The class supports a default or user-specified IP address and port for listening to incoming packets. It's useful for developers aiming to integrate with such games or build applications needing real-time in-game data.

UDP Specification from EA:
https://docs.google.com/document/d/1Q6Js58vndPNzym5FducmQwcjCEKnJJJf/edit?usp=sharing&ouid=109802826249883598381&rtpof=true&sd=true

Usage

First, you create an instance of the F1_2022_UDP_Receiver class.
Use the default constructor to listen for any IP and the game's default port.
```c#
F1_23_Telemetry_Receiver receiver = new F1_23_Telemetry_Receiver();
```
or specify the IP address and port
```c#
F1_23_Telemetry_Receiver receiver = new F1_23_Telemetry_Receiver("192.168.1.43", 20777);
```
Then, subscribe to the events you are interested in.
```c#
receiver.CarStatusDataPacketReceived += (sender, e) => {
    // Handle the car status data packet
    PacketCarStatusData carStatus = e.PacketCarStatusData;
    // your code here
};

receiver.LapDataPacketReceived += (sender, e) => {
    // Handle the lap data packet
    PacketLapData lapData = e.PacketLapData;
    // your code here
};
```
Repeat for other packet types as necessary.
When you are ready to start receiving packets, call the StartReceiving method:
```c#
receiver.StartReceiving();
```
This will start a background task that continuously listens for incoming packets and raises the appropriate events when packets of the subscribed types are received.

Finally, when you are done receiving packets, call the StopReceiving method to stop the background task:
```c#
receiver.StopReceiving();
```
This usage pattern enables you to process different kinds of data packets in different ways, according to your specific application's needs, while the F1_2022_UDP_Receiver class takes care of the low-level details of receiving and parsing the packets.
