import { Room, Client, AuthContext } from "colyseus";
import { MyRoomState, Player } from "./schema/MyRoomState";

export type PositionMessage = {
  x: number,
  y: number
}

export class MyRoom extends Room<MyRoomState> {

  state = new MyRoomState();

  onCreate (options: any) {
    this.setMetadata({
      //Set room metadata here, e.g.,
      //RoomName: options.RoomName
      roomType: "akash_demo",
      description: "Akash Colyseus Unity SDK Demo Room"
    })
    
    console.log("Akash Demo Room created!", this.roomId);
  }

  onAuth(client: Client, options: any, context: AuthContext) {
    return true;
  }

  onJoin (client: Client, options: any) {
    console.log("Akash Demo:", client.sessionId, "joined!");

    // Create new player with session ID
    const newPlayer = new Player();
    newPlayer.playerId = client.sessionId;
    
    // Set random spawn position around center (near Akash logo)
    const spawnRadius = 5;
    const angle = Math.random() * Math.PI * 2;
    newPlayer.x = Math.cos(angle) * spawnRadius;
    newPlayer.y = Math.sin(angle) * spawnRadius;
    
    this.state.players.set(client.sessionId, newPlayer);
    
    // Update player count
    this.state.playerCount = this.state.players.size;
    
    // Send welcome message to the client.
    client.send("welcomeMessage", 
      `Welcome to the Akash Colyseus Demo! Player count: ${this.state.playerCount}`);

    // Listen to position changes from the client.
    this.onMessage("position", (client, position: PositionMessage) => {
      const player = this.state.players.get(client.sessionId);
      if (player) {
        player.x = position.x;
        player.y = position.y;
        console.log(`Akash Demo: Player ${client.sessionId} moved to`, {x: position.x, y: position.y});
      }
    });
    
    // Broadcast player joined message to all clients
    this.broadcast("player_joined", {
      playerId: client.sessionId,
      playerCount: this.state.playerCount
    });
  }

  onLeave (client: Client, consented: boolean) {
    console.log("Akash Demo:", client.sessionId, "left!");
    
    this.state.players.delete(client.sessionId);
    this.state.playerCount = this.state.players.size;
    
    // Broadcast player left message to remaining clients
    this.broadcast("player_left", {
      playerId: client.sessionId,
      playerCount: this.state.playerCount
    });
  }

  onDispose() {
    console.log("Akash Demo: Room", this.roomId, "disposing...");
  }
}
