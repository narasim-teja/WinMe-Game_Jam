using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using Com.MyCompany.MyGame;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;
        
        public GameObject playerPrefab;
        public int TimerNumberOfSpawn = 0;

        public int getSpawn()
        {
            return TimerNumberOfSpawn;
        }

        public void inrSpawn()
        {
            TimerNumberOfSpawn = TimerNumberOfSpawn + 1;
        }
        /*public int matchLength = 180;
        public Text timerUI;

        private int currentMatchTime;
        private Coroutine timerCoroutine;

        public enum EventCodes : byte
        {
            NewPlayer,
            UpdatePlayers,
            ChangeStat,
            NewMatch,
            RefreshTimer
        }

        private void RefreshTimerUI()
        {
            int minutes = Mathf.FloorToInt(currentMatchTime / 60F);
            int seconds = Mathf.FloorToInt(currentMatchTime % 60F);
            timerUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        private void InitializeTimer()
        {
            currentMatchTime = matchLength;
            RefreshTimerUI();

            if(PhotonNetwork.IsMasterClient)
            {
                timerCoroutine = StartCoroutine(Timer());
            }
            
        }
        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(1f);
            currentMatchTime -= 1;

            if(currentMatchTime <= 0 ) {
                timerCoroutine = null;
                
            }
            else
            {
                RefreshTimer_S();
                timerCoroutine = StartCoroutine(Timer());
            }
        }

        public void RefreshTimer_S()
        {
            object[] package = new object[] { currentMatchTime };
            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.RefreshTimer,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All},
                new ExitGames.Client.Photon.SendOptions { Reliability = true } );
        }

        public void RefreshTimer_R(object[] data)
        {
            currentMatchTime = (int)data[0];
            RefreshTimerUI();   
        }*/
        private void Start()
        {
            //timerUI = GameObject.Find("carPrefabPUN/scoreCanvas/Left/timer").GetComponent<Text>();
            //InitializeTimer();
            

            Instance = this;
            
            if ( PlayerManager.LocalPlayerInstance == null)
            {
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            }

        }

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("CloverStadium");
        }

        #endregion

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        #endregion

    }
}
