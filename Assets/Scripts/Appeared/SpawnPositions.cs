using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Com.MiniBattle.Game
{


    public class SpawnPositions : MonoBehaviour
    {
        #region Serializable Fields

        [Serializable]
        public struct SpawnPos_Name
        {
            public string name;
            public Vector3 spawnpos;
        }

        #endregion

        #region Public Fields

        public SpawnPos_Name[] spawnPos;

        #endregion

        #region Public Methods

        public Vector3 spawnPosCharacter(string character_Name)
        {
            Vector3 spawn = new Vector3(0,0,0);

            for(int i=0; i< spawnPos.Length;i++)
            {
                if(character_Name == spawnPos[i].name)
                {
                    spawn = spawnPos[i].spawnpos;
                }
            }

            return spawn;
        }

        #endregion


    }
}
