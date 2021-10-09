using System.Collections;
using System.Collections.Generic;
using SharedGame;
using UnityEngine;
using System;

public class FgGameView : MonoBehaviour, IGameView
{
    public FgPlayerView shipPrefab;

    private FgPlayerView[] shipViews = Array.Empty<FgPlayerView>();
    private GameManager gameManager => GameManager.Instance;

    private void ResetView(FgGame gs)
    {
        var shipGss = gs._players;
        shipViews = new FgPlayerView[shipGss.Length];

        for (int i = 0; i < shipGss.Length; ++i)
        {
            shipViews[i] = Instantiate(shipPrefab, transform);

        }
    }

    public void UpdateGameView(IGameRunner runner)
    {
        var gs = (FgGame)runner.Game;
        var ngs = runner.GameInfo;
        var shipsGss = gs._players;
        if (shipViews.Length != shipsGss.Length)
        {
            ResetView(gs);
        }
        for (int i = 0; i < shipsGss.Length; ++i)
        {
            shipViews[i].Populate(shipsGss[i], ngs.players[i]);
            //if (i == 0)
            //    GameHandler.Instance.p2Transform.position = shipsGss[i].position;
            //else
            //    GameHandler.Instance.p2Transform.position = shipsGss[i].position;
        }
    }

    private void Update()
    {
        if (gameManager.IsRunning)
        {
            UpdateGameView(gameManager.Runner);
        }
    }
}
