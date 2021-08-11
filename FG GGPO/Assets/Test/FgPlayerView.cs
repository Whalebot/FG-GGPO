using SharedGame;
using UnityEngine;
using UnityEngine.UI;
using UnityGGPO;


public class FgPlayerView : MonoBehaviour {
    public Text txtStatus;
    public Image imgProgress;
    public Text feedback;
   [SerializeField] bool[] inputs = new bool[4];
    [SerializeField] bool[] buttons = new bool[6];

    public void Populate(Ship shipGs, PlayerConnectionInfo info) {
        inputs = new bool[4];
        buttons = new bool[6];
        transform.position = shipGs.position;


        //Send Input
        for (int i = 0; i < shipGs.dir.Length; i++) {
            inputs[i] = shipGs.dir[i];
        }
        for (int i = 0; i < shipGs.buttons.Length; i++)
        {
            buttons[i] = shipGs.buttons[i];
        }

        string status = "";
        int progress = -1;
        switch (info.state) {
            case PlayerConnectState.Connecting:
                status = (info.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL) ? "Local Player" : "Connecting...";
                break;

            case PlayerConnectState.Synchronizing:
                progress = info.connect_progress;
                status = (info.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL) ? "Local Player" : "Synchronizing...";
                break;

            case PlayerConnectState.Disconnected:
                status = "Disconnected";
                break;

            case PlayerConnectState.Disconnecting:
                status = "Waiting for player...";
                progress = (Utils.TimeGetTime() - info.disconnect_start) * 100 / info.disconnect_timeout;
                break;
        }

        if (progress > 0) {
            imgProgress.gameObject.SetActive(true);
            imgProgress.fillAmount = progress / 100f;
        }
        else {
            imgProgress.gameObject.SetActive(false);
        }

        if (status.Length > 0) {
            txtStatus.gameObject.SetActive(true);
            txtStatus.text = status;
        }
        else {
            txtStatus.gameObject.SetActive(false);
        }
    }
}
