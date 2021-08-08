using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedGame;
using UnityGGPO;
public class FgGameManager : GameManager
{
    public override void StartLocalGame() {
        StartGame(new LocalRunner(new FgGame(2)));
    }

    public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex) {
        var game = new GGPORunner("testgame", new FgGame(connections.Count), perfPanel);
        game.Init(connections, playerIndex);
        StartGame(game);
        Debug.Log("start ggpo game");
    }
}
