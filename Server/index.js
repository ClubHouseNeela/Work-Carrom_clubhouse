const express = require('express')();
const server = require('http').createServer(express);

const io = require('socket.io')(server, {
    
});
const { Console } = require('console');
var shortID = require('shortid');
var Room = require('./Classes/Room.js');

var rooms = [];

io.on('connection',function(socket){

    console.log('Connection Made');

    if(rooms.length > 0 && rooms[rooms.length - 1].players < 2)
    {
        rooms[rooms.length - 1].players++;
    }
    else
    {
        rooms[rooms.length] = new Room(shortID.generate(),1);
    }
    socket.join(rooms[rooms.length - 1].roomID)
    socket.room = rooms.length - 1;
    socket.roomID = rooms[rooms.length - 1].roomID
    socket.playerNumber = rooms[rooms.length - 1].players;
    socket.on('PlayerStrikerPositionChanged', function(data)
    {
        var strikerPos =
        {
            position: data
        }
        socket.broadcast.to(socket.roomID).emit('OpponentStrikerPositionChanged', strikerPos);
    });
    socket.on('PlayerStrikerShoot', function(data)
    {
        io.in(socket.roomID).emit('StrikerShoot', data);
    });

    socket.on('TimerStart', function(data)
    {
        // socket.timer = data
        // socket.timerStarted = true
    });

    socket.on('PlayerSelectedPieceColour', function(data)
    {
        rooms[socket.room].playerTargetPieceColours.push(data);
        if(rooms[socket.room].playerTargetPieceColours.length == 2)
        {
            var pieceColours;
            if(rooms[socket.room].playerTargetPieceColours[0] == rooms[socket.room].playerTargetPieceColours[1])
            {
                rooms[socket.room].coloursFlipped = true;
                pieceColours =
                {
                    flipped: true
                }
                socket.emit("GeneratePieces",pieceColours);
            }
            else
            {
                pieceColours =
                {
                    flipped: false
                }
                socket.emit("GeneratePieces",pieceColours);
            }
            io.in(socket.roomID).emit("BlackAndWhiteModeStartGame",pieceColours);
        }
        else
        {
            var pieceColours =
            {
                flipped: false
            }
            socket.emit("GeneratePieces",pieceColours);
        }
    });

    socket.on('TurnEnd', function()
    {
        console.log("Turn end from",socket.playerNumber);
        rooms[socket.room].endTurnCount++;
        if(rooms[socket.room].endTurnCount == 2)
        {
            rooms[socket.room].endTurnCount = 0;
            console.log("Sending command to end turn");
            io.in(socket.roomID).emit('TurnEnded');
        }
    });

    socket.on('PieceInfo', function(data)
    {
        socket.broadcast.to(socket.roomID).emit('PieceInfo', data);
    });

    socket.on('TimerEnd', function()
    {
        console.log(socket.playerNumber, " Sent timer end");
        // if(socket.timerStarted)
        // {
        //     socket.timer = 0;
        //     socket.timerStarted = false;
        // }
        socket.broadcast.to(socket.roomID).emit('TimerEnded');
    });

    socket.on('disconnect', function() 
    {
        if(socket.timerStarted)
        {
            socket.timer = 0;
            socket.timerStarted = false;
        }
        if(rooms[socket.room].gameStarted)
        {
            socket.broadcast.to(socket.roomID).emit('PlayerDisconnected');
        }
        rooms[socket.room].players--;
        if(rooms[socket.room].players == 0)
        {
            rooms.splice(socket.room,1)
        }
		console.log("Disconnected");
    });

   
    if(rooms[rooms.length - 1].players == 2)
    {   
        console.log("Starting game....");
        rooms[rooms.length - 1].gameStarted = true;
        var gameFlags =
        {
            turn: true
        }
        socket.emit('StartGame', gameFlags);
        gameFlags =
        {
            turn: false
        }
        socket.broadcast.to(socket.roomID).emit('StartGame', gameFlags);
    }
});


server.listen(process.env.PORT || 3000, () => console.log('Server Started'));



