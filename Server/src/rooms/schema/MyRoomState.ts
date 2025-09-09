import { MapSchema, Schema, type } from "@colyseus/schema";

export class Player extends Schema {

  @type("number") x: number = 0;
  @type("number") y: number = 0;
  @type("string") playerId: string = "";

}

export class MyRoomState extends Schema {
	@type({map: Player})
	players: MapSchema<Player> = new MapSchema<Player>();
	
	@type("number")
	playerCount: number = 0;
}
