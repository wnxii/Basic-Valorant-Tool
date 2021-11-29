using Aesoftware.Data;
using Aesoftware.Manager;
using Aesoftware.ModulePage;
using Aesoftware.Other;
using Aesoftware.Page;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ValAPINet;

namespace Aesoftware.ModuleManager
{
    public class ValorantManager
    {
        private static ValorantManager instance = null;
        private static readonly object padlock = new object();
        private bool isInit = false;

        Auth auth = null;
        LiteValorant liteValorant = null;
        Content content = null;
        Timer timer = null;
        String currentGameState = null;


        // To-do: Find a more dynamic way to add data, but I guess not is fine too :/
        // Filling info field
        Username username = new Username();
        Balance balance = new Balance();
        MMR mmr = new MMR();
        Storefront storefront = new Storefront();
        AccountXP accountXP = new AccountXP();
        UserPresence userPresence = new UserPresence();
        CoreGetMatch coreGetMatch = new CoreGetMatch();
        GetParty getParty = new GetParty();
        CoreGetPlayer coreGetPlayer = new CoreGetPlayer();
        PregameGetPlayer pregameGetPlayer = new PregameGetPlayer();
        PregameGetMatch pregameGetMatch = new PregameGetMatch();
        UserPresence.Presence currentUserPresence = null;

        List<Scrapper.ValorantPlayer> currentPartyPlayerList = new List<Scrapper.ValorantPlayer>();
        List<Scrapper.ValorantPlayer> currentPlayerList = new List<Scrapper.ValorantPlayer>();

        ValorantManager()
        {
        }
        public static ValorantManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                            instance = new ValorantManager();
                    }
                }

                return instance;
            }
        }

        public void Init()
        {
            if (isInit)
                return;

            isInit = true;
        }

        public void LoadAuthWithCredentials(string username, string password, ValAPINet.Region region)
        {
            auth = null;
            auth = Auth.Login(username, password, region);

            if (string.IsNullOrEmpty(auth.AccessToken))
            {
                FormManager.Instance.ShowMesageBoxButton("Riot Authentication Error", "Error trying to load authentication", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                FormManager.Instance.ShowMesageBoxButton("Riot Authentication Success", "Successfully loaded authentication", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormManager.Instance.CloseForm("RiotAuthenticationForm");
            }
        }

        public void LoadAuthWithNoCredentials(ValAPINet.Region region)
        {
            auth = null;

            // Check for valorant process
            /*if (valorantProcess != running)
                FormManager.Instance.ShowMesageBoxButton("Riot Authentication Error", "Valorant has to be running!", MessageBoxButtons.OK, MessageBoxIcon.Error);*/

            ModuleMenuList moduleMenuList = FormManager.Instance.moduleMenuItemList.Where(moduleMenuItem => moduleMenuItem.ModuleName == "PremiumValorant").FirstOrDefault();

/*            if (moduleMenuList.CanUse == 0)
            {
                SecurityManager.Instance.AddAuditLog("RiotAuthenticationForm", AuditAction.LITEVALORANT_AUTH_FAILED, AccountManager.Instance.currentAccount.Id, "No access to premium valorant", region.ToString());
                FormManager.Instance.ShowMesageBoxButton("Access Error", "You do not have access to module: PremiumValorant", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
*/
            auth = Websocket.GetAuthLocal(region, true);

            if (string.IsNullOrEmpty(auth.AccessToken))
            {
                SecurityManager.Instance.AddAuditLog("RiotAuthenticationForm", AuditAction.LITEVALORANT_AUTH_FAILED, AccountManager.Instance.currentAccount.Id, "No credentials auth failed", region.ToString());
                FormManager.Instance.ShowMesageBoxButton("Riot Authentication Error", "Error trying to load authentication", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SecurityManager.Instance.AddAuditLog("RiotAuthenticationForm", AuditAction.LITEVALORANT_AUTH_SUCCESS, AccountManager.Instance.currentAccount.Id, "No credentials auth success", region.ToString());
                FormManager.Instance.ShowMesageBoxButton("Riot Authentication Success", "Successfully loaded authentication", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormManager.Instance.CloseForm("RiotAuthenticationForm");
            }
        }

        public void RefreshDataOnLiteValorant()
        {
            if (!FormManager.Instance.IsFormActive("LiteValorant"))
                return;

            if (content == null)
                content = Content.GetContentWrappedAPI();

            if (liteValorant == null)
                liteValorant = FormManager.Instance.GetForm("LiteValorant") as LiteValorant;

            if (liteValorant == null || auth == null || content == null)
                return;

            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = 1000;
                timer.Tick += new EventHandler(PullPresenceData);
                timer.Start();
            }

            FetchData();
            RefreshAccountUI();
            RefreshStoreUI();
        }

        public void FetchData()
        {
            try
            {
                username = Username.GetUsername(auth);
                //DebugJson(username.jObject, "username");
            }
            catch (Exception error)
            {

            }
            try
            {
                balance = Balance.GetBalance(auth);
                //ViewDebugJson(balance.jObject, "balance"); ;
            }
            catch (Exception error)
            {

            }
            try
            {
                mmr = MMR.GetMMR(auth);
                //DebugJson(MMR.GetMMR(auth, "3cbdf171-5ed6-5d48-b2bb-2229e13edee5").jObject, "mmr");
            }
            catch (Exception error)
            {

            }
            try
            {
                storefront = Storefront.GetOffers(auth);
                //ViewDebugJson(storefront.jObject, "storefront");
            }
            catch (Exception error)
            {

            }
            try
            {
                accountXP = AccountXP.GetOffers(auth);
                //ViewDebugJson(accountXP.jObject, "accountXP");
            }
            catch (Exception error)
            {

            }
            try
            {
                coreGetPlayer = CoreGetPlayer.GetPlayer(auth);
                //DebugJson(coreGetPlayer.jObject, "coregetplayer");
            }
            catch (Exception error)
            {

            }
            try
            {
                getParty = GetParty.Party(auth);
                //DebugJson(getParty.jObject, "getparty");
            }
            catch (Exception error)
            {

            }
            try
            {
                coreGetMatch = CoreGetMatch.GetMatch(auth, coreGetPlayer.MatchID);
                //DebugJson(coreGetMatch.jObject, "coregetmatch");
            }
            catch (Exception error)
            {

            }
            try
            {
                userPresence = UserPresence.GetPresence();
/*                if (userPresence != null)
                    DebugJson(userPresence.jObject, "userpresence");*/
            }
            catch (Exception error)
            {

            }
            try
            {
                pregameGetPlayer = PregameGetPlayer.GetPlayer(auth);
                //DebugJson(pregameGetPlayer, "pregameGetPlayer");
                pregameGetMatch = PregameGetMatch.GetMatch(auth, pregameGetPlayer.MatchID);
                //DebugJson(pregameGetMatch, "pregameGetMatch");
            }
            catch (Exception error)
            {

            }
        }

        public void RefreshAccountUI()
        {
            liteValorant.infoListView.Items.Clear();
            liteValorant.competitiveListView.Items.Clear();
            liteValorant.storeListView.Items.Clear();

            liteValorant.infoListView.Items.Add(new ListViewItem(new string[] { "Riot ID:", username.GameName + "#" + username.TagLine }));
            liteValorant.infoListView.Items.Add(new ListViewItem(new string[] { "Valorant Points:", balance.ValorantPoints.ToString() }));
            liteValorant.infoListView.Items.Add(new ListViewItem(new string[] { "Radianite Points:", balance.RadianitePoints.ToString() }));
            liteValorant.infoListView.Items.Add(new ListViewItem(new string[] { "Level:", accountXP.Progress.Level.ToString() }));

            liteValorant.competitiveListView.Items.Add(new ListViewItem(new string[] { "Rank:", Ranks.GetRankFormatted(mmr.Rank) }));
            liteValorant.competitiveListView.Items.Add(new ListViewItem(new string[] { "Rank Rating:", mmr.RankedRating.ToString() }));
            liteValorant.competitiveListView.Items.Add(new ListViewItem(new string[] { "Number Of Wins:", mmr.NumberOfWins.ToString() }));
            liteValorant.competitiveListView.Items.Add(new ListViewItem(new string[] { "Number Of Games Played:", mmr.NumberOfGames.ToString() }));
            liteValorant.competitiveListView.Items.Add(new ListViewItem(new string[] { "Leaderboard Rank:", mmr.LeaderboardRank.ToString() }));
        }

        public void RefreshStoreUI()
        {
            foreach (string itemOffer in storefront.SkinsPanelLayout.SingleItemOffers)
            {
                Content.SkinLevel skinLevel = content.SkinLevels.Where(skin => skin.ID.ToUpper() == itemOffer.ToUpper()).FirstOrDefault();
                liteValorant.storeListView.Items.Add(new ListViewItem(new string[] { skinLevel.Name }));
            }
        }

        public void PullPresenceData(object sender, EventArgs e)
        {
            ProcessLockAgent();
            try
            {
                userPresence = UserPresence.GetPresence();
                //DebugJson(userPresence.jObject, "userpresence");
            }
            catch (Exception error)
            {

            }

            if (userPresence == null || userPresence.presences == null)
                return;

            currentUserPresence = userPresence.presences.Where(x => x.puuid == auth.subject).FirstOrDefault();
            if (currentUserPresence == null || currentGameState == currentUserPresence.privinfo.sessionLoopState)
                return;
            currentGameState = currentUserPresence.privinfo.sessionLoopState;

            currentPartyPlayerList = Scrapper.ScrapPartyPlayers(auth, userPresence, currentUserPresence.privinfo.partyId);

            // MENUS, PREGAME, INGAME
            // Invalid, CustomGame/Matchmaking ,Matchmaking
            switch (currentUserPresence.privinfo.sessionLoopState)
            {
                case "PREGAME":
                    currentPlayerList = Scrapper.ScrapPregamePlayers(auth, userPresence);
                    break;
                case "INGAME":
                    currentPlayerList = Scrapper.ScrapIngamePlayers(auth, userPresence);
                    break;
                default:
                    currentPlayerList.Clear();
                    break;
            }

            UpdateLiveMatchUI();
        }

        public UserPresence.Presence ScrapCurrentPlayer()
        {
            foreach (UserPresence.Presence presence in userPresence.presences)
                if (presence.puuid == auth.subject)
                    return presence;

            return null;
        }

        public void UpdateLiveMatchUI()
        {
            liteValorant.liveMatchDataGrid.Rows.Clear();
            liteValorant.liveMatchDataGrid.Refresh();

            foreach (Scrapper.ValorantPlayer valorantPlayer in currentPlayerList)
            {
                var row = (DataGridViewRow)liteValorant.liveMatchDataGrid.RowTemplate.Clone();

                row.CreateCells(liteValorant.liveMatchDataGrid, valorantPlayer.PartyId, valorantPlayer.Level, valorantPlayer.AgentName, valorantPlayer.DisplayName, valorantPlayer.CompetitveRank, valorantPlayer.PeakCompetitiveRank, valorantPlayer.CompetitveRankRating);
                liteValorant.liveMatchDataGrid.Rows.Add(row);
            }
        }

        public void ProcessLockAgent()
        {
            if ((ValAPINet.AgentSelect)liteValorant.comboBox1.SelectedValue == ValAPINet.AgentSelect.None || currentGameState != "PREGAME")
                return;

            SelectAgent.LockAgent(auth, currentPlayerList.Where(x => x.Puuid == currentUserPresence.puuid).FirstOrDefault().MatchId, Agent.AgentDictionary[liteValorant.comboBox1.SelectedValue.ToString()]);
        }

        public void DodgeAgentSelect()
        {
            Pregame.PregameLeaveMatch(auth);
        }

        public void DisassociatePlayer()
        {
            coreGetPlayer = CoreGetPlayer.GetPlayer(auth);
            if (coreGetPlayer.MatchID == null)
                return;
            CoreGetMatch.DisassociatePlayer(auth, coreGetPlayer.MatchID);
        }

        public void DebugJson(Object jsonObject, string title = null)
        {
            DebugJSONTree debugJsonForm = new DebugJSONTree();
            debugJsonForm.Show();
            if (title != null)
                debugJsonForm.Text = title;
            debugJsonForm.DebugTreeView.SetObjectAsJson(jsonObject);
        }
    }
}
