using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class GameController : MonoBehaviour
{
    //public
    public Players players;
    public Authentication auth;
    public SocketConnection sockCon;
    public GameObject CardPref;
    public Teams teams;
    public Image hokmIcon;
    public GameObject hokmSelect;
    public Sprite[] hokmIcons;
    public Transform[] place;
    public int lastPlayer;
    public Chat chat;
    public PlayersOptions playersOption;
    public ReportObject report;
    public ReportMsg reportMsg;
    public BackGround BG;
    public EndTeams end;
    public GameObject status;

    // private
    private bool childDestroyReq = false;
    // Start is called before the first frame update
    void Start()
    {
        auth = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Authentication>();
        sockCon = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<SocketConnection>();
        sockCon.gameCtrl = GetComponent<GameController>();
        auth.gameCtrl = GetComponent<GameController>();
        players.init(sockCon.opponents, sockCon.seq);
        //BG.OnSetBackgroundAspect();
        sockCon.OnReadySignal();
        StartCoroutine(auth.OnChangeStatus("AWAIT"));
        if (auth.vip.expires != "")
        {
            players.player.cardhandler.deck.ChangeDeck(auth.user.DeckOfCard);
            StartCoroutine(OnGetBackground(auth.user.Background));
        }
        else
        {
            auth.adobject.active = true;
        }
        auth.adobject.HideStanBanner();
    }
    IEnumerator OnGetBackground(string id)
    {
        CoroutineWithData co = new CoroutineWithData(this, auth.GetTexture("backgrounds/bc (", id));
        yield return co.coroutine;
        string result = co.result.ToString();
        Texture2D texture = new Texture2D(1, 1);
        string err = "";
        try
        {
            texture = (Texture2D)co.result;
        }
        catch (System.Exception)
        {
            err = co.result.ToString();
        }
        if (err == "")
        {
            BG.background.init(texture, id);
            BG.setBackground();
        }
        else
        {
            Debug.Log(err);
        }

    }

    public void OnPlayerOptionButton(int id)
    {
        bool isOpen = playersOption.options[id].isOpen;
        if (isOpen)
        {
            playersOption.options[id].parent.gameObject.SetActive(false);
            playersOption.options[id].isOpen = true;
        }
        else
        {
            playersOption.options[id].parent.gameObject.SetActive(true);
            playersOption.options[id].isOpen = true;
        }
        
    }
    public void OnOpenReportSubmition(int index)
    {
        report.parent.gameObject.SetActive(true);
        reportMsg = new ReportMsg(sockCon.opponents.users[sockCon.seq[index]].id, 1, "");
    }
    public void OnReportSubmit(string txt)
    {
        if (report.rulebreak.isOn)
        {
            reportMsg.type = 1;
        }
        else if(report.rudeness.isOn)
        {
            reportMsg.type = 2;
        }
        reportMsg.message = txt;
        //send the report
        report.submit.interactable = false;
        report.submit.transform.GetChild(0).GetComponent<Text>().text = "...";
        StartCoroutine(OnReportSubmit(reportMsg));
    }
    IEnumerator OnReportSubmit(ReportMsg msg)
    {
        CoroutineWithData co = new CoroutineWithData(this, auth.OnSetReport(msg));
        yield return co.coroutine;

        string result = co.result.ToString();
        if (result == "ok")
        {
            //show that report has been send seccussfully
            report.submit.transform.GetChild(0).GetComponent<Text>().text = "ﺪﺷ لﺎﺳﺭﺍ شﺭﺍﺰﮔ";

        }
        else
        {
            //show what happend to the report
            report.submit.interactable = true;
            report.submit.transform.GetChild(0).GetComponent<Text>().text = "لﺎﺳﺭﺍ و ﺪﯿﺋﺎﺗ";

        }
    }
    public void OnChatButtonClick()
    {
        chat.newMessageIcon.SetActive(false);
        StartCoroutine(DeleyUpdateUI());
    }
    public void OnSendChat(string txt)
    {
        if (chat.input.text != "" || txt != "")
        {
            sockCon.OnSendChat(new ChatMsg( sockCon.opponents.users[0].name,
                                            sockCon.opponents.users[0].id,
                                            txt,
                                            sockCon.opponents.room));
            chat.input.text = "";
        }
    }
    public void OnSendChat(Text text)
    {
        sockCon.OnSendChat(new ChatMsg(sockCon.opponents.users[0].name,
                                        sockCon.opponents.users[0].id,
                                        text.text,
                                        sockCon.opponents.room));
        chat.input.text = "";
    }

    public void NewChat(ChatMsg msg)
    {
        if (sockCon.opponents.users[0].id == msg.userID)
        {
            GameObject obj = Instantiate(chat.mePrefab, chat.parent, false);
            chat.createMeChat(obj.transform, obj.transform.GetChild(0).GetChild(0).GetComponent<Text>(), msg);
            StartCoroutine(DeleyUpdateUI());
        }
        else
        {
            for (int i = 1; i < sockCon.opponents.users.Length; i++)
            {
                if (sockCon.opponents.users[i].id == msg.userID)
                {
                    GameObject obj;
                    obj = Instantiate(chat.otherPrefab, chat.parent, false);
                    chat.createOtherChat(obj.transform,
                        obj.transform.GetChild(1).GetChild(0).GetComponent<Text>(),
                        obj.transform.GetChild(0).GetChild(1).GetComponent<Text>(),
                        obj.transform.GetChild(0).GetChild(0).GetComponent<Image>(),
                        msg,sockCon.opponents.users[i].Avatar.data.sprite);
                    StartCoroutine(DeleyUpdateUI());
                    chat.newMessageIcon.SetActive(true);
                    break;
                }
            }
        }
    }
    public void OnInput()
    {
        chat.input.Select();
        StartCoroutine(OnInputEnum());
    }
    public IEnumerator OnInputEnum()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (KeyboardHeight.height > 0)
            {
                chat.inputParent.anchoredPosition = new Vector2(chat.inputParent.anchoredPosition.x, KeyboardHeight.height);
                break;
            }
        }
    }
    public void OnInputDown()
    {
        chat.inputParent.anchoredPosition = new Vector2(chat.inputParent.anchoredPosition.x, 0);
    }
    public void OnDelayLayout(ContentSizeFitter csf)
    {
        StartCoroutine(DeleyInputLayout(csf));
    }
    public IEnumerator DeleyInputLayout(ContentSizeFitter csf)
    {
        csf.enabled = false;
        yield return new WaitForSeconds(0.05f);
        csf.enabled = true;
    }

    IEnumerator DeleyUpdateUI()
    {
        chat.parent.GetComponent<ContentSizeFitter>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        chat.parent.GetComponent<ContentSizeFitter>().enabled = true;
    }
    public void next()
    {
        // Destroying cards from last round
        //if (childDestroyReq)
        //{
        //    if (sockCon.seq[0] == sockCon.stage.playedPlayer)
        //    {
        //        for (int i = 1; i < place.Length; i++)
        //        {
        //            players.player.cardhandler.DestroyChildes(place[i]);
        //        }
        //    }
        //    else
        //    {
        //        foreach (Transform item in place)
        //        {
        //            players.player.cardhandler.DestroyChildes(item);
        //        }
        //    }
        //    childDestroyReq = false;
        //}

        int stageNumber = sockCon.stage.GetStageNumber;
        switch (stageNumber)
        {
            case 1:
                GenarateCard(sockCon.stage);
                break;
            case 2:
                foreach (Transform item in place)
                {
                    players.player.cardhandler.DestroyChildes(item);
                }
                GeneratePlayersCard(5);
                ChangeTheRuler();
                break;
            case 3:
                //set the hokm
                int hokmIndex = GetIndexByCardType(sockCon.stage.hokm);
                if (hokmIndex != -1)
                {
                    hokmIcon.sprite = hokmIcons[hokmIndex];
                }
                break;
            case 4:
                //cards 4 by 4 set for players
                GeneratePlayersCard(4);
                break;
            case 5:
                // set teamCounts and teamScores and Update the Cards On the ground
                //GenarateCard(sockCon.stage);
                if (sockCon.stage.msg == "OK4")
                {
                    GeneratePlayersCard(4);
                }
                else
                {
                UpdatePlayerCards();
                }
                break;
            case 6:
                UpdatePlayerCards();
                DestroyPlayersCards();
                break;
            default:
                break;
        }
        teams.Update(sockCon.stage, sockCon.seq);
        UpdatePlayerTimer();
    }

    public void OnEndTheMatch()
    {
        switch (sockCon.stage.teamScore.WichTeam(sockCon.opponents.users[0].number))
        {
            case "team0":
                if (sockCon.stage.teamScore.team0 >= 7)
                {
                    WinnerSetup(0);
                }
                else
                {
                    LoserSetup(0);
                }
                break;
            case "team1":
                if (sockCon.stage.teamScore.team1 >= 7)
                {
                    WinnerSetup(1);
                }
                else
                {
                    LoserSetup(1);
                }

                break;
            default:
                break;
        }
    }
    public void LoserSetup(int losser)
    {
        int youTeam = 0;
        int otherTeam = 0;
        int yourScore = 0;
        if (losser == 0)
        {
            youTeam = sockCon.stage.teamScore.team0;
            otherTeam = sockCon.stage.teamScore.team1;
            yourScore = (otherTeam - youTeam) * 5;
        }
        else
        {
            youTeam = sockCon.stage.teamScore.team1;
            otherTeam = sockCon.stage.teamScore.team0;
            yourScore = (otherTeam - youTeam) * 5;
        }
        
        end.SetUpResult("other", youTeam, otherTeam, yourScore);
        StartCoroutine(auth.OnUpdateCurrency());
        Time.timeScale = 0.2f;
    }
    public void WinnerSetup(int winner) 
    {
        int youTeam = 0;
        int otherTeam = 0;
        int yourScore = 0;
        if (winner == 0)
        {
            youTeam = sockCon.stage.teamScore.team0;
            otherTeam = sockCon.stage.teamScore.team1;
            yourScore = (youTeam - otherTeam) * 5;
        }
        else
        {
            youTeam = sockCon.stage.teamScore.team1;
            otherTeam = sockCon.stage.teamScore.team0;
            yourScore = (youTeam - otherTeam) * 5;
        }

        end.SetUpResult("you", youTeam, otherTeam, yourScore);
        StartCoroutine(auth.OnUpdateCurrency());
        Time.timeScale = 0.2f;
    }
    public void OnBackHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void DestroyPlayersCards()
    {
        players.player.cardhandler.DestroyChildes(players.player.cardhandler.transform);
        players.player.cardhandler.SetZero();
        for (int i = 0; i < 3; i++)
        {
            players.player.cardhandler.DestroyChildes(players.opponents[i].cardhandler.transform);
            players.opponents[i].cardhandler.SetZero();
        }
    }
    public void ChangeTheRuler()
    {
        //reset the ruler icon
        players.player.avatar.transform.GetChild(1).GetComponent<Image>().enabled = false;
        for (int i = 0; i < 3; i++)
        {
            players.opponents[i].avatar.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }
        //set the new ruler icon
        if (sockCon.seq[0] == sockCon.stage.ruler)
        {
            players.player.avatar.transform.GetChild(1).GetComponent<Image>().enabled = true;
        }
        else
        {
            for (int i = 1; i < 4; i++)
            {
                if (sockCon.seq[i] == sockCon.stage.ruler)
                {
                    players.opponents[i - 1].avatar.transform.GetChild(1).GetComponent<Image>().enabled = true;
                    break;
                }
            }
        }
    }
    public void UpdatePlayerTimer()
    {
        players.player.avatar.transform.GetChild(2).GetComponent<PlayerImageTimer>().OnStop();
        foreach (OtherPlayers item in players.opponents)
        {
            item.avatar.transform.GetChild(2).GetComponent<PlayerImageTimer>().OnStop();
        }

        if (sockCon.seq[0] == sockCon.stage.nextPlayer)
        {
            players.player.avatar.transform.GetChild(2).GetComponent<PlayerImageTimer>().OnStart(sockCon.stage.timeout/1000);
        }
        else
        {
            for (int i = 1; i < 4; i++)
            {
                if (sockCon.seq[i] == sockCon.stage.nextPlayer)
                {
                players.opponents[i - 1].avatar.transform.GetChild(2).GetComponent<PlayerImageTimer>().OnStart(sockCon.stage.timeout/1000);
                }
            }
        }
    }
    public int GetIndexByCardType(int type)
    {
        switch (type)
        {
            case 100:
                return 0;
            case 200:
                return 1;
            case 300:
                return 2;
            case 400:
                return 3;
            default:
                return -1;
        }
    }
    public void OnChooseHokm(int hokm)
    {
        sockCon.OnSetStage(new SocketStageMsg(int.Parse(sockCon.opponents.users[0].number), hokm));
    }
    public void UpdatePlayerCards()
    {
        
        
        if (sockCon.stage.cardsOnGround.cardsIndex == 4)
        {
            if (lastPlayer == sockCon.seq[0]
            && players.player.cardhandler.IsCard(sockCon.stage.cardsOnGround.GetCard(sockCon.seq[0])))
            {
                lastPlayer = sockCon.stage.nextPlayer;
                players.player.cardhandler.OnCardPlayed(sockCon.stage.cardsOnGround.GetCard(sockCon.seq[0]));
            }
            else
            {
                for (int i = 1; i < 4; i++)
                {
                    if (lastPlayer == sockCon.seq[i])
                    {
                        lastPlayer = sockCon.stage.nextPlayer;
                        players.opponents[i - 1].cardhandler.OnCardActionInit(sockCon.stage.cardsOnGround.GetCard(sockCon.seq[i]));
                        break;
                    }
                }
            }
            // Animation for cards Destroy
            // for now its destroyed in next round
            //childDestroyReq = true;
            for (int i = 0; i < 4; i++)
            {
                Camera.main.GetComponent<AudioSource>().Play();
                if (place[i].childCount > 0)
                {
                    place[i].GetChild(0).GetComponent<Animator>().enabled = true;
                    players.player.cardhandler.DestroyWithAnimation(place[i].GetChild(0).GetComponent<Animator>(), i);
                }
            }
        }
        else
        {
            if (sockCon.stage.playedPlayer == sockCon.seq[0]
            && players.player.cardhandler.IsCard(sockCon.stage.cardsOnGround.GetCard(sockCon.seq[0]))
            && sockCon.stage.playedPlayer != sockCon.stage.nextPlayer)
            {
                lastPlayer = sockCon.stage.nextPlayer;
                players.player.cardhandler.OnCardPlayed(sockCon.stage.cardsOnGround.GetCard(sockCon.seq[0]));
            }
            else
            {

                for (int i = 1; i < 4; i++)
                {
                    if (sockCon.stage.playedPlayer == sockCon.seq[i])
                    {
                        lastPlayer = sockCon.stage.nextPlayer;
                        players.opponents[i - 1].cardhandler.OnCardActionInit(sockCon.stage.cardsOnGround.GetCard(sockCon.seq[i]));
                        break;
                    }
                }
            }
        }
    }
    public void GeneratePlayersCard(int length)
    {
        int start = StartPoint(players.player.cardhandler.card);
        players.player.cardhandler.GenerateCards(sockCon.stage.playerCards.GetCard(sockCon.seq[0],start,start+length));
        for (int i = 1; i < 4; i++)
        {
            players.opponents[i-1].cardhandler.GenerateCards(sockCon.stage.playerCards.GetCard(sockCon.seq[i], start, start+length));
        }
        if (sockCon.seq[0] == sockCon.stage.ruler && sockCon.stage.GetStageNumber == 2)
        {
            hokmSelect.SetActive(true);
        }
    }
    public int StartPoint(Card[] card)
    {
        int startPoint = -1;
        for (int i = 0; i < card.Length; i++)
        {
            if (card[i].id == 0)
            {
                startPoint = i;
                break;
            }
        }
        return startPoint;
    }
    public void GenarateCard(PlayersObjectMsg msg)
    {
        int[] cards = new int[4] {
            msg.cardsOnGround.C0,
            msg.cardsOnGround.C1,
            msg.cardsOnGround.C2,
            msg.cardsOnGround.C3
        };
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = Instantiate(CardPref, place[i], false);
            obj.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            obj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            obj.GetComponent<Image>().sprite = players.player.cardhandler.deck.GetTexture(cards[sockCon.seq[i]]);
            if (place[i].childCount > 0)
            {
                place[i].GetChild(0).GetComponent<Animator>().enabled = true;
                players.player.cardhandler.DestroyWithAnimation(place[i].GetChild(0).GetComponent<Animator>(),i);
            }
        }
    }
    
    public void OnSendFriendRequest(int index)
    {
        string playerID = sockCon.opponents.users[index].id;
        playersOption.options[index-1].addfriend.text = "...";
        playersOption.options[index-1].addfriendbutt.interactable = false;
        StartCoroutine(OnSendFriendRequestEnum(index-1,playerID));
    }
    IEnumerator OnSendFriendRequestEnum(int index,string id)
    {
        CoroutineWithData co = new CoroutineWithData(this, auth.OnFriendRequest(id));
        yield return co.coroutine;

        string result = co.result.ToString();
        if (result == "ok")
        {
            playersOption.options[index].addfriend.text = "request sended";
            yield return new WaitForSeconds(0.5f);
            playersOption.options[index].parent.gameObject.SetActive(false);
        }
        else
        {
            playersOption.options[index].addfriend.text = "request faild";
            yield return new WaitForSeconds(1f);
            playersOption.options[index].addfriendbutt.interactable = true;
            playersOption.options[index].addfriend.text = "add friend";
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
