using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Analytics : MonoBehaviour
{
    public bool isOn = true;
    public string urlstring = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfVrT6ligtfribWCDNh-wktCJSHhnP67eUaK8ZQD-ZOsoNzRQ/formResponse";
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.gameEndEvent += SendData;
    }

    void SendData()
    {
        if (isOn)
        {
            if (GameHandler.Instance.p1RoundWins > GameHandler.Instance.p2RoundWins)
            {
                StartCoroutine(Post(true));
            }
            else StartCoroutine(Post(false));
        }
    }
    public IEnumerator Post(bool p1win)
    {
        WWWForm form = new WWWForm();
        //P1 Char
        form.AddField("entry.437435726", GameHandler.Instance.characters[GameHandler.p1CharacterID].name.ToString());
        //P2 Char
        form.AddField("entry.1259043234", GameHandler.Instance.characters[GameHandler.p2CharacterID].name.ToString());
        if (p1win)
        {
            form.AddField("entry.354554606", GameHandler.Instance.characters[GameHandler.p1CharacterID].name.ToString() + " P1");
        }
        else
        {
            form.AddField("entry.354554606", GameHandler.Instance.characters[GameHandler.p2CharacterID].name.ToString() + " P2");
        }
        form.AddField("entry.566602199", ComboSystem.Instance.p1Max.ToString());
        form.AddField("entry.515275570", ComboSystem.Instance.p2Max.ToString());
        form.AddField("entry.860370369", (GameHandler.Instance.round1Time + GameHandler.Instance.round2Time + GameHandler.Instance.round3Time).ToString());
        form.AddField("entry.870801350", ComboSystem.Instance.p1Average.ToString());
        form.AddField("entry.148751348", ComboSystem.Instance.p2Average.ToString());
        form.AddField("entry.1811973827", GameHandler.Instance.p1RoundWins.ToString());
        form.AddField("entry.2030400954", GameHandler.Instance.p2RoundWins.ToString());
        form.AddField("entry.1145007404", GameHandler.Instance.round1Time.ToString());
        form.AddField("entry.779551068", GameHandler.Instance.round2Time.ToString());
        form.AddField("entry.1290024063", GameHandler.Instance.round3Time.ToString());
        form.AddField("entry.1800439950", GameHandler.Instance.round1Winner.ToString());
        form.AddField("entry.1660622320", GameHandler.Instance.round2Winner.ToString());
        form.AddField("entry.576785787", GameHandler.Instance.round3Winner.ToString());

        byte[] data = form.data;
        UnityWebRequest www = UnityWebRequest.Post(urlstring, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            //   Debug.Log("Form upload complete!");
        }
    }

}
