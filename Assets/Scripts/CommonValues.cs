using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonValues
{
    public enum Colour
    {
        BLACK,
        WHITE,
        RED
    }

    public enum GameMode
    {
        LOCAL_MULTIPLAYER,
        FREESTYLE,
        BLACK_AND_WHITE,
        PRACTICE
    }

    public static class PlayerPrefKeys
    {
        public const string ROOM_ID = "ROOM_ID";
        public const string NEED_TO_RECONNECT = "NEED_TO_RECONNECT";
        public const string GAME_MODE = "GAME_MODE";
        public const string PLAYER_COLOUR = "PLAYER_COLOUR";
        public const string PLAYER_NUMBER = "PLAYER_NUMBER";
        public const string COLOURS_FLIPPED = "COLOURS_FLIPPED";
        public const string HAS_BOT = "HAS_BOT";
        public const string PLAYER_MOBILE_NUMBER = "PLAYER_MOBILE_NUMBER";
        public const string OPPONENT_MOBILE_NUMBER = "OPPONENT_MOBILE_NUMBER";
        public const string RANDOM_SEED = "RANDOM_SEED";
    }
}
