module.exports = class Room {
	constructor(roomID, players) {
		this.roomID = roomID;
		this.players = players;
		this.gameStarted = false;
		this.playerTargetPieceColours = []
		this.endTurnCount = 0;
        // Other values depending on game variation
	}
}