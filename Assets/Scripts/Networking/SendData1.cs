using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SendData1 
{
    public string player_id;
    public string room_id;
    public string game_mode;
    public WinningDetails winning_details;
    public string game_end_time;
    public string wallet_amt;//to be sent by base app
    public string game_status;//fully played or left
    public string game_id;
    public string battle_tournament_id;
    public string id;
}
