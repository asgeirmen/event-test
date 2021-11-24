# Getting started with Kafka
This project comes with a Kafka `docker-compose` document. Start Kafka with follwoing command:
```
docker-compose up -d
```
When the Kafka cluster is up and running, you can connect to it using following configuration:
* Zookeeper Host: `localhost`
* Zookeeper Port: `2181`
* Bootstrap servers: `localhost:9092`

I found a handy GUI tool called [Offset Explorer](https://www.kafkatool.com/download.html). Using this tool it is easy to create and view topics and partitions. 

From **Offset Explorer**, add a connection to the Kafka cluster on localhost as follows:
* File > Add New Connection...
* In the **Properties** tab
  * Cluster name: `Localhost`
  * Kafka Cluster Version: `0.11` (other versions will probably work too)
  * Zookeeper host: `localhost`
  * Zookeeper port: `2181`
  * chroot path: /
* In the **Advanced** tab
  * Bootstrap servers: `localhost:9092`
* Click **Add**.

Now you can add a topic as follows:
* Right click **Topics** under **Localhost** cluser.
* Select **Create topic**
* Enter `EventTest.ValueEntered` as Name
* Enter `4` as Partition count
* Leave Replica count as `1`.

# Test the EventTest application with Kafka
Check the `appsettings.json` file under the EventTest folder and make sure `Kafka` is configured as a Transport.
```json
{
  "BusConfig": {
    "Transport": "Kafka", 

    "Kafka": {
      "BootstrapServers": "localhost:9092"
    }
  }
}
```
Run following command to build the application.
```
dotnet build EventTest
````
Run following command to publish events to the `EventTest.ValueEntered` topic.
```
dotnet run --no-build -p EventTest publish
````
Now you can enter event values in the command line. You can also create multiple events with following command, which in this case creates 100 events.
```
dotnet run --no-build -p EventTest publish -c 100
````
Start a new terminal and run following command to consume events:
```
dotnet run --no-build -p EventTest consume
```
You can start multiple terminals to test multiple consumers and publishers. When a new consumer is started, it may take some time for the messages to arrive since the Kafka cluster needs to re-distribute partitions after a new consumer is connected.
